using CedulasEvaluacion.Entities.MAgua;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.Vistas;
using CedulasEvaluacion.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Repositories
{
    public class RepositorioAgua : IRepositorioAgua
    {
        private readonly string _connectionString;

        public RepositorioAgua(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection"); ;
        }

        public async Task<List<VCedulas>> GetCedulasAgua(int user)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getCedulasAgua", sql))
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
                        cmd.Parameters.Add(new SqlParameter("@servicio", 9));
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

        public async Task<int> insertaCedula(CedulaAgua cedulaAgua)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_nuevaCedulaAgua", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", cedulaAgua.Id)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new SqlParameter("@usuario", cedulaAgua.UsuarioId));
                        cmd.Parameters.Add(new SqlParameter("@inmueble", cedulaAgua.InmuebleId));
                        cmd.Parameters.Add(new SqlParameter("@folio", generaFolio(cedulaAgua.InmuebleId, cedulaAgua.Mes)));
                        cmd.Parameters.Add(new SqlParameter("@mes", cedulaAgua.Mes));
                        cmd.Parameters.Add(new SqlParameter("@anio", cedulaAgua.Anio));

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

        public async Task<CedulaAgua> CedulaById(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_cedulaAguaById", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        CedulaAgua response = null;
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
                        using (SqlCommand cmd = new SqlCommand("sp_insertaActualizaRespuestasAgua", sql))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int)).Direction = ParameterDirection.Output;
                            cmd.Parameters.Add(new SqlParameter("@cedula", r.CedulaAguaId));
                            cmd.Parameters.Add(new SqlParameter("@pregunta", r.Pregunta));
                            cmd.Parameters.Add(new SqlParameter("@respuesta", r.Respuesta));
                            if (r.Detalles != null)
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
            }
            catch (Exception ex)
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
                    using (SqlCommand cmd = new SqlCommand("sp_getRespuestasAgua", sql))
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
                    using (SqlCommand cmd = new SqlCommand("sp_calculaCalificacionAgua", sql))
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

        public async Task<int> apruebaRechazaCedula(CedulaAgua cedulaAgua)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_autorizarRechazarCedula", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", cedulaAgua.Id));
                        cmd.Parameters.Add(new SqlParameter("@estatus", cedulaAgua.Estatus));
                        cmd.Parameters.Add(new SqlParameter("@servicio", 9));

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
        }//

        public async Task<int> capturaHistorial(HistorialCedulas historialCedulas)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_insertaHistorialCedulas", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", "Agua Para Beber"));
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
        }//

        public async Task<List<HistorialCedulas>> getHistorialAgua(object id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getHistorialCedulaEvaluacion", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", "Agua para Beber"));
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
        }//

        private VCedulas MapToValue(SqlDataReader reader)
        {
            return new VCedulas()
            {
                Id = (int)reader["Id"],
                Folio = reader["Folio"].ToString(),
                Nombre = reader["Nombre"].ToString(),
                Mes = reader["Mes"].ToString(),
                Anio = (int)reader["Anio"],
                Servicio = reader["Servicio"].ToString(),
                Estatus = reader["Estatus"].ToString()
            };
        }

        private CedulaAgua MapToValueCedula(SqlDataReader reader)
        {
            return new CedulaAgua()
            {
                Id = (int)reader["Id"],
                ServicioId = (int)reader["ServicioId"],
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

        private RespuestasEncuesta MapToValueRespuestas(SqlDataReader reader)
        {
            return new RespuestasEncuesta
            {
                CedulaAguaId = (int)reader["CedulaAguaId"],
                Pregunta = (int)reader["Pregunta"],
                Respuesta = Convert.ToBoolean(reader["Respuesta"]),
                Detalles = reader["Detalles"].ToString(),
                Penalizable = reader["Penalizable"] != DBNull.Value ? Convert.ToBoolean(reader["Penalizable"]) : false,
                MontoPenalizacion = reader["MontoPenalizacion"] != DBNull.Value ? Convert.ToDecimal(reader["MontoPenalizacion"]) : 0
            };
        }

        private HistorialCedulas MapToValueHistorial(SqlDataReader reader)
        {
            return new HistorialCedulas
            {
                Servicio = reader["Servicio"].ToString(),
                CedulaId = (int)reader["CedulaId"],
                UsuarioId = (int)reader["UsuarioId"],
                Estatus = reader["Estatus"].ToString(),
                Comentarios = reader["Comentarios"].ToString(),
                FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"])
            };
        }

        private string generaFolio(int inmuebleId, string mes)
        {
            string servicio = "AGUA";
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
