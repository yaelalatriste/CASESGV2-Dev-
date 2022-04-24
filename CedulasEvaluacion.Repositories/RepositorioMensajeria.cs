using CedulasEvaluacion.Entities.MMensajeria;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.Vistas;
using CedulasEvaluacion.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Repositories
{
    public class RepositorioMensajeria : IRepositorioMensajeria
    {
        private readonly string _connectionString;

        public RepositorioMensajeria(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }

        //obtenemos las cedulas de limpieza
        public async Task<List<VCedulas>> GetCedulasMensajeria(int user)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getCedulasMensajeria", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
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

        public async Task<int> insertaCedula(CedulaMensajeria  cedulaMensajeria)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_nuevaCedulaMensajeria", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", cedulaMensajeria.Id)).Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(new SqlParameter("@usuario", cedulaMensajeria.UsuarioId));
                        cmd.Parameters.Add(new SqlParameter("@inmueble", cedulaMensajeria.InmuebleId));
                        cmd.Parameters.Add(new SqlParameter("@folio", generaFolio(cedulaMensajeria.InmuebleId,cedulaMensajeria.Mes)));
                        cmd.Parameters.Add(new SqlParameter("@mes", cedulaMensajeria.Mes));
                        cmd.Parameters.Add(new SqlParameter("@anio", cedulaMensajeria.Anio));

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
                        cmd.Parameters.Add(new SqlParameter("@servicio", 3));
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

        public async Task<CedulaMensajeria> CedulaById(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_cedulaMensajeriaById", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        CedulaMensajeria response = null;
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

        public async Task<int> GuardaRespuestas(List<RespuestasEncuesta> respuestasEncuestas)
        {
            int id = 0;
            try
            {
                foreach (var r in respuestasEncuestas)
                {
                    using (SqlConnection sql = new SqlConnection(_connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_insertaActualizaRespuestasMensajeria", sql))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int)).Direction = ParameterDirection.Output;
                            cmd.Parameters.Add(new SqlParameter("@cedula", r.CedulaMensajeriaId));
                            cmd.Parameters.Add(new SqlParameter("@pregunta", r.Pregunta));
                            cmd.Parameters.Add(new SqlParameter("@respuesta", r.Respuesta));
                            if(r.Detalles != null)
                                cmd.Parameters.Add(new SqlParameter("@detalles", r.Detalles));

                            await sql.OpenAsync();
                            int i = await cmd.ExecuteNonQueryAsync();
                            if (i > 0)
                            {
                                id = Convert.ToInt32(cmd.Parameters["@id"].Value);
                            }
                        }
                    }
                }
                return 1;
            }catch(Exception ex)
            {
                string msg = ex.Message;
                return -1;
            }
                        
        }

        public async Task<List<RespuestasEncuesta>> obtieneRespuestas(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getRespuestasMensajeria", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        var response = new List<RespuestasEncuesta>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueRespuestas(reader));
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

        public async Task<int> enviaRespuestas(int cedula)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_calculaPenalizacionesMensajeria", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", cedula));

                        await sql.OpenAsync();

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return -1;
            }
        }

        public async Task<int> apruebaRechazaCedula(CedulaMensajeria cedulaMensajeria, int v)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_autorizarRechazarCedula", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", cedulaMensajeria.Id));
                        cmd.Parameters.Add(new SqlParameter("@estatus", cedulaMensajeria.Estatus));
                        cmd.Parameters.Add(new SqlParameter("@servicioId", 3));

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
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_insertaHistorialCedulas", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", "Mensajería"));
                        cmd.Parameters.Add(new SqlParameter("@cedula", historialCedulas.CedulaId));
                        cmd.Parameters.Add(new SqlParameter("@usuario", historialCedulas.UsuarioId));
                        cmd.Parameters.Add(new SqlParameter("@estatus", historialCedulas.Estatus));
                        cmd.Parameters.Add(new SqlParameter("@comentarios", historialCedulas.Comentarios));

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

        public async Task<List<HistorialCedulas>> getHistorialMensajeria(object id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getHistorialCedulaEvaluacion", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", "Mensajería"));
                        cmd.Parameters.Add(new SqlParameter("@cedula", id));
                        var response = new List<HistorialCedulas>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueHistorial(reader));
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

        private HistorialCedulas MapToValueHistorial(SqlDataReader reader)
        {
            return new HistorialCedulas { 
                Servicio = reader["Servicio"].ToString(),
                CedulaId = (int)reader["CedulaId"],
                UsuarioId = (int)reader["UsuarioId"],
                Estatus = reader["Estatus"].ToString(),
                Comentarios = reader["Comentarios"].ToString(),
                FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"])
            };
        }

        private RespuestasEncuesta MapToValueRespuestas(SqlDataReader reader)
        {
            return new RespuestasEncuesta {
                CedulaMensajeriaId = (int)reader["CedulaMensajeriaId"],
                Pregunta = (int)reader["Pregunta"],
                Respuesta = Convert.ToBoolean(reader["Respuesta"]),
                Detalles = reader["Detalles"].ToString(),
                Penalizable = reader["Penalizable"] != DBNull.Value ? Convert.ToBoolean(reader["Penalizable"]) : false,
                MontoPenalizacion = reader["MontoPenalizacion"] != DBNull.Value ? Convert.ToDecimal(reader["MontoPenalizacion"]): 0
            };
        }

        /*Readers*/

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

        private CedulaMensajeria MapToValueCedula(SqlDataReader reader)
        {
            return new CedulaMensajeria()
            {
                Id = (int)reader["Id"],
                ServicioId = (int) reader["ServicioId"],
                UsuarioId = (int)reader["UsuarioId"],
                InmuebleId = (int)reader["InmuebleId"],
                Folio = reader["Folio"].ToString(),
                Mes = reader["Mes"].ToString(),
                Anio = (int)reader["Anio"],
                Calificacion = reader["Calificacion"] != DBNull.Value ? (decimal)reader["Calificacion"] : 5,
                PenaCalificacion = reader["PenaCalificacion"] != DBNull.Value ? (decimal)reader["PenaCalificacion"] : 0,
                Estatus = reader["Estatus"].ToString(),
                FechaCreacion = (DateTime)reader["FechaCreacion"]
            };
        }

        private string generaFolio(int inmuebleId, string mes)
        {
            string servicio = "MENS";
            string date = DateTime.Now.ToString("yyyy");
            return inmuebleId < 9 ? servicio + "-0" + inmuebleId + "-" + date + convertirMes(mes) : servicio + "-" + inmuebleId + "-" + date + convertirMes(mes);
        }

        private string convertirMes(string mes)
        {
            string[] meses = { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };
            for (var i = 0; i < meses.Length; i++)
            {
                if (meses[i] == mes)
                    return (i + 1) > 9 ? (i + 1) + "" : "0" + (i + 1);
            }
            return "";
        }

    }
}
