using CASESGCedulasEvaluacion.Entities.Vistas;
using CedulasEvaluacion.Interfaces;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.Vistas;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;

namespace CedulasEvaluacion.Repositories
{
    public class RepositorioLimpieza : IRepositorioLimpieza
    {
        private readonly string _connectionString;

        public RepositorioLimpieza(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }

        private SqlConnection conexion()
        {
            return new SqlConnection(_connectionString);
        }

        //obtenemos las cedulas de limpieza
        public async Task<List<VCedulas>> GetCedulasLimpieza(int user)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getCedulasLimpieza", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@user", user));
                        var response = new List<VCedulas>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValue(reader));
                            }
                        }

                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }

        //creamos la nueva cedula
        public async Task<int> NuevaCedula(CedulaLimpieza cedulaLimpieza)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_NuevaCedulaLimpieza", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", cedulaLimpieza.Id)).Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(new SqlParameter("@usuario", cedulaLimpieza.UsuarioId));
                        cmd.Parameters.Add(new SqlParameter("@inmueble", cedulaLimpieza.InmuebleId));
                        cmd.Parameters.Add(new SqlParameter("@folio", generaFolio(cedulaLimpieza.InmuebleId,cedulaLimpieza.Mes)));
                        cmd.Parameters.Add(new SqlParameter("@mes", cedulaLimpieza.Mes));
                        cmd.Parameters.Add(new SqlParameter("@anio", cedulaLimpieza.Anio));

                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                        int id = Convert.ToInt32(cmd.Parameters["@id"].Value);
                        return id;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return 0;
            }
        }

        /*Verificamos que no exista la cedula, en caso contrario devolvemos esa cedula con los datos que ya fueron capturados previamente*/
        public async Task<int> VerificaCedula(int anio, string mes, int inmueble)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_verificaPeriodo", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@anio", anio));
                        cmd.Parameters.Add(new SqlParameter("@mes", mes));
                        cmd.Parameters.Add(new SqlParameter("@inmueble", inmueble));
                        cmd.Parameters.Add(new SqlParameter("@servicio", 1));
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                return Convert.ToInt32(reader["Id"].ToString());
                            }
                            return 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return -1;
            }
        }

        /*Obtenemos la cedula guardada*/
        public async Task<CedulaLimpieza> CedulaById(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_cedulaLimpiezaById", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        CedulaLimpieza response = null;
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response = MapToValueCedula(reader);
                            }
                        }
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }

        //guardamos las respuestas de la evaluacion de la cedula
        public async Task<int> GuardaRespuestas(List<RespuestasEncuesta> respuestasEncuestas)
        {
            SqlConnection sqlConexion = conexion();
            SqlCommand Comm = null;
            int id = 0;
            try
            {
                sqlConexion.Open();
                foreach (var r in respuestasEncuestas)
                {
                    id = 0;
                    Comm = sqlConexion.CreateCommand();
                    Comm.CommandText = "dbo.sp_insertaActualizaRespuestasLimpieza";
                    Comm.CommandType = CommandType.StoredProcedure;
                    Comm.Parameters.Add("@id", SqlDbType.Int).Direction = ParameterDirection.Output;
                    Comm.Parameters.Add("@cedula", SqlDbType.Int).Value = r.CedulaLimpiezaId;
                    Comm.Parameters.Add("@pregunta", SqlDbType.Int).Value = r.Pregunta;
                    Comm.Parameters.Add("@respuesta", SqlDbType.Bit).Value = r.Respuesta;
                    Comm.Parameters.Add("@detalles", SqlDbType.VarChar, 256).Value = r.Detalles;

                    int i = await Comm.ExecuteNonQueryAsync();
                    if (i > 0)
                    {
                        id = Convert.ToInt32(Comm.Parameters["@id"].Value);
                    }

                    if (id == 0)
                    {
                        Comm.Dispose();
                        sqlConexion.Close();
                        sqlConexion.Dispose();
                        return -1;
                    }
                }

            }
            finally
            {
                Comm.Dispose();
                sqlConexion.Close();
                sqlConexion.Dispose();
            }
            return -1;
        }

        public async Task<List<RespuestasEncuesta>> obtieneRespuestas(int id) {
            List<RespuestasEncuesta> respuestas = new List<RespuestasEncuesta>();
            RespuestasEncuesta resp = null;
            SqlConnection sqlConexion = conexion();
            SqlCommand Comm = null;
            SqlDataReader reader = null;
            try
            {
                sqlConexion.Open();
                Comm = sqlConexion.CreateCommand();
                Comm.CommandText = "dbo.sp_getRespuestasLimpieza";
                Comm.CommandType = CommandType.StoredProcedure;
                Comm.Parameters.Add("@cedId", SqlDbType.Int).Value = id;
                reader = await Comm.ExecuteReaderAsync();
                while (reader.Read())
                {
                    resp = new RespuestasEncuesta();
                    resp.CedulaLimpiezaId = Convert.ToInt32(reader["CedulaLimpiezaId"]);
                    resp.Pregunta = Convert.ToInt32(reader["Pregunta"]);
                    resp.Respuesta = Convert.ToBoolean(reader["Respuesta"]);
                    resp.Detalles = reader["Detalles"].ToString();
                    if (reader["Penalizable"] != DBNull.Value)
                        resp.Penalizable= Convert.ToBoolean(reader["Penalizable"]);
                    if (reader["MontoPenalizacion"] != DBNull.Value)
                        resp.MontoPenalizacion = Convert.ToDecimal(reader["MontoPenalizacion"]);

                    respuestas.Add(resp);
                }

            }
            finally
            {
                if (reader != null)
                    reader.Close();

                Comm.Dispose();
                sqlConexion.Close();
                sqlConexion.Dispose();
            }
            return respuestas;
        }

        /*Envío de la Cedula*/
        public async Task<int> enviaRespuestas(List<RespuestasEncuesta> respuestasEncuestas)
        {
            var send = 0;
            decimal calificacion = 0;
            try
            {
                foreach (var question in respuestasEncuestas)
                {
                    using (SqlConnection sql = new SqlConnection(_connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_calculaPenalizacionesPregunta", sql))
                        {   
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@penaTotal", SqlDbType.Decimal)).Direction = ParameterDirection.Output;
                            cmd.Parameters.Add(new SqlParameter("@pregunta", question.Pregunta));
                            cmd.Parameters.Add(new SqlParameter("@cedulaId", question.CedulaLimpiezaId));

                            await sql.OpenAsync();

                            var update = await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }

                foreach (var question in respuestasEncuestas)
                {
                    using (SqlConnection sql = new SqlConnection(_connectionString))
                    {
                        using (SqlCommand cmd2 = new SqlCommand("sp_calculaCalificacionLimpieza", sql))
                        {
                            cmd2.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd2.Parameters.Add(new SqlParameter("@calificacionFinal", SqlDbType.VarChar, 4)).Direction = ParameterDirection.Output;
                            cmd2.Parameters.Add(new SqlParameter("@pregunta", question.Pregunta));
                            cmd2.Parameters.Add(new SqlParameter("@cedulaId", question.CedulaLimpiezaId));
                            cmd2.Parameters.Add(new SqlParameter("@calificacion", calificacion));
                            await sql.OpenAsync();
                            send = await cmd2.ExecuteNonQueryAsync();
                            string p = cmd2.Parameters["@calificacionFinal"].Value.ToString();

                            calificacion = Convert.ToDecimal(cmd2.Parameters["@calificacionFinal"].Value.ToString());
                        }
                    }
                }

                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd2 = new SqlCommand("sp_calificacionEntregablesBDLimpieza", sql))
                    {
                        cmd2.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd2.Parameters.Add(new SqlParameter("@cedulaId", respuestasEncuestas[0].CedulaLimpiezaId));
                        await sql.OpenAsync();
                        await cmd2.ExecuteNonQueryAsync();
                    }
                }

                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd2 = new SqlCommand("sp_calificacionCapacitacionLimpieza", sql))
                    {
                        cmd2.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd2.Parameters.Add(new SqlParameter("@cedulaId", respuestasEncuestas[0].CedulaLimpiezaId));
                        await sql.OpenAsync();
                        await cmd2.ExecuteNonQueryAsync();
                    }
                }

                return send == 1 ? send : 0;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return -1;
            }
        }
        /*FIN del envío de la Cedula*/

        public async Task<int> apruebaRechazaCedula(CedulaLimpieza cedulaLimpieza)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_autorizarRechazarCedula", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", cedulaLimpieza.Id));
                        cmd.Parameters.Add(new SqlParameter("@estatus", cedulaLimpieza.Estatus));
                        cmd.Parameters.Add(new SqlParameter("@servicio", 1));

                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return -1;
            }
        }

        public async Task<int> capturaHistorial(HistorialCedulas historialCedulas)
        {
            SqlConnection sqlConexion = conexion();
            SqlCommand Comm = null;

            try
            {
                sqlConexion.Open();
                Comm = sqlConexion.CreateCommand();
                Comm.CommandText = "dbo.sp_insertaHistorial";
                Comm.CommandType = CommandType.StoredProcedure;
                Comm.Parameters.Add("@servicio", SqlDbType.VarChar, 50).Value = "Limpieza";
                Comm.Parameters.Add("@cedula", SqlDbType.Int).Value = historialCedulas.CedulaId;
                Comm.Parameters.Add("@usuario", SqlDbType.Int).Value = historialCedulas.UsuarioId;
                Comm.Parameters.Add("@estatus", SqlDbType.VarChar, 50).Value = historialCedulas.Estatus;
                Comm.Parameters.Add("@comentarios", SqlDbType.VarChar, 200).Value = historialCedulas.Comentarios;

                int i = await Comm.ExecuteNonQueryAsync();
                if (i > 0)
                {
                    return i;
                }

            }
            finally
            {
                Comm.Dispose();
                sqlConexion.Close();
                sqlConexion.Dispose();
            }
            return -1;
        }

        public async Task<List<HistorialCedulas>> getHistorial(int cedula)
        {
            HistorialCedulas historial = null;
            List<HistorialCedulas> lHistorial = new List<HistorialCedulas>();
            SqlConnection sqlConexion = conexion();
            SqlCommand Comm = null;
            SqlDataReader reader = null;
            try
            {
                sqlConexion.Open();
                Comm = sqlConexion.CreateCommand();
                Comm.CommandText = "dbo.sp_getHistorialCedula";
                Comm.CommandType = CommandType.StoredProcedure;
                Comm.Parameters.Add("@servicio", SqlDbType.VarChar, 50).Value = "Limpieza";
                Comm.Parameters.Add("@cedula", SqlDbType.Int).Value = cedula;
                reader = await Comm.ExecuteReaderAsync();
                while (reader.Read())
                {
                    historial = new HistorialCedulas();
                    historial.Servicio = reader["Servicio"].ToString();
                    historial.CedulaId = Convert.ToInt32(reader["CedulaId"]);
                    historial.UsuarioId = Convert.ToInt32(reader["UsuarioId"]);
                    historial.Estatus = reader["Estatus"].ToString();
                    historial.Comentarios = reader["Comentarios"].ToString();
                    historial.FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"].ToString());

                    lHistorial.Add(historial);
                }

            }
            finally
            {
                if (reader != null)
                    reader.Close();

                Comm.Dispose();
                sqlConexion.Close();
                sqlConexion.Dispose();
            }
            return lHistorial;
        }

        public async Task<int> EliminaCedula(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_eliminarCedulaLimpieza", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return -1;
            }
        }


        /******************* Metodos Locales *********************/
        protected string GetIPAddress()
        {
            string host = Dns.GetHostName();

            IPAddress[] ip = Dns.GetHostAddresses(host);

            return ip[1].ToString();
        }
        private VCedulas MapToValue(SqlDataReader reader)
        {
            return new VCedulas()
            {
                Id = (int)reader["Id"],
                Nombre = reader["Nombre"].ToString(),
                Folio = reader["Folio"].ToString(),
                Mes = reader["Mes"].ToString(),
                Anio = (int)reader["Anio"],
                Servicio = reader["Servicio"].ToString(),
                Estatus = reader["Estatus"].ToString()
            };
        }

        private CedulaLimpieza MapToValueCedula(SqlDataReader reader)
        {
            return new CedulaLimpieza()
            {
                Id = (int)reader["Id"],
                ServicioId = (int)reader["ServicioId"],
                UsuarioId = (int)reader["UsuarioId"],
                InmuebleId = (int)reader["InmuebleId"],
                Folio = reader["Folio"].ToString(),
                Mes = reader["Mes"].ToString(),
                Anio = (int)reader["Anio"],
                Calificacion = reader["Calificacion"] != DBNull.Value ? (decimal)reader["Calificacion"]:5,
                PenaCalificacion = reader["PenaCalificacion"] != DBNull.Value ? (decimal)reader["PenaCalificacion"] : 0,
                Estatus = reader["Estatus"].ToString(),
                FechaCreacion = (DateTime)reader["FechaCreacion"]
            };
        }

        private string generaFolio(int inmuebleId, string mes)
        {
            string servicio = "LIMP";
            string date = DateTime.Now.ToString("yyyy");
            return inmuebleId < 9 ? servicio + "-0" + inmuebleId + "-" + date + convertirMes(mes) : servicio + "-" + inmuebleId + "-" + date + convertirMes(mes);
        }

        private string convertirMes(string mes)
        {
            string[] meses = { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
            for (var i = 0; i < meses.Length; i++)
            {
                if (meses[i] == mes)
                    return (i + 1) > 9 ? (i+1)+"" : "0"+(i+1);
            }
            return "";
        }

    }
}
