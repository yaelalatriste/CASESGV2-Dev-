using CedulasEvaluacion.Entities.MCedula;
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
    public class RepositorioEvaluacionServicios : IRepositorioEvaluacionServicios
    {
        private readonly string _connectionString;

        public RepositorioEvaluacionServicios(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }

        public async Task<int> VerificaCedula(int servicio, int anio, string mes, int inmueble)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_buscaCedulaRegistrada", sql))
                    {
                        cmd.Parameters.Add(new SqlParameter("@servicioId", servicio));
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@anio", anio));
                        cmd.Parameters.Add(new SqlParameter("@mes", mes));
                        cmd.Parameters.Add(new SqlParameter("@inmueble", inmueble));
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

        public async Task<List<VCedulas>> GetCedulasEvaluacion(int servicio, int user)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getCedulasEvaluacion", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
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

        public async Task<int> insertaCedula(CedulaEvaluacion cedula)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_insertaCedulaEvaluacion", sql))
                    {
                        string folio = await GetFolioCedula(cedula.ServicioId);
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", cedula.Id)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new SqlParameter("@servicioId", cedula.ServicioId));
                        cmd.Parameters.Add(new SqlParameter("@usuario", cedula.UsuarioId));
                        cmd.Parameters.Add(new SqlParameter("@inmueble", cedula.InmuebleId));
                        if(cedula.InmuebleDestinoId != 0)
                            cmd.Parameters.Add(new SqlParameter("@inmuebleDestino", cedula.InmuebleDestinoId));
                        cmd.Parameters.Add(new SqlParameter("@folio", generaFolio(cedula.InmuebleId, cedula.Mes, await GetFolioCedula(cedula.ServicioId))));
                        cmd.Parameters.Add(new SqlParameter("@mes", cedula.Mes));
                        cmd.Parameters.Add(new SqlParameter("@anio", cedula.Anio));

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

        public async Task<string> GetFolioCedula(int servicio)
        {
            string folio = "";
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getFolioCedula", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                folio = reader["Valor"].ToString();
                            }
                        }
                    }
                }
                return folio;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }

        public async Task<CedulaEvaluacion> CedulaById(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getCedulaEvaluacionById", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        CedulaEvaluacion response = null;
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
                        using (SqlCommand cmd = new SqlCommand("sp_insertaActualizaRespuestasEvaluacion", sql))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int)).Direction = ParameterDirection.Output;
                            cmd.Parameters.Add(new SqlParameter("@cedula", r.CedulaEvaluacionId));
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
                    using (SqlCommand cmd = new SqlCommand("sp_getRespuestasEvaluacion", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", id));
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

        public async Task<int> enviaRespuestas(int servicio, int cedula)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_calculaEvaluacionServicio", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicioId", servicio));
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

        public async Task<int> apruebaRechazaCedula(CedulaEvaluacion cedula)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_autorizarRechazarCedula", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", cedula.Id));
                        cmd.Parameters.Add(new SqlParameter("@estatus", cedula.Estatus));
                        cmd.Parameters.Add(new SqlParameter("@servicio", cedula.ServicioId));

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

        public async Task<List<HistorialCedulas>> getHistorial(object id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getHistorialCedulaEvaluacion", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
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

        private VCedulas MapToValue(SqlDataReader reader)
        {
            return new VCedulas()
            {
                Id = (int)reader["Id"],
                Folio = reader["Folio"].ToString(),
                Nombre = reader["Nombre"].ToString(),
                Destino = reader["Destino"].ToString(),
                Mes = reader["Mes"].ToString(),
                Anio = (int)reader["Anio"],
                Servicio = reader["Servicio"].ToString(),
                Estatus = reader["Estatus"].ToString()
            };
        }

        private CedulaEvaluacion MapToValueCedula(SqlDataReader reader)
        {
            return new CedulaEvaluacion()
            {
                Id = (int)reader["Id"],
                ServicioId = (int)reader["ServicioId"],
                UsuarioId = (int)reader["UsuarioId"],
                InmuebleId = (int)reader["InmuebleId"],
                InmuebleDestinoId = reader["InmuebleDestinoId"] != DBNull.Value ? (int)reader["InmuebleDestinoId"]:0,
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
                CedulaEvaluacionId = (int)reader["CedulaEvaluacionId"],
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

        private string generaFolio(int inmuebleId, string mes, string servicio)
        {
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
