using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace CedulasEvaluacion.Repositories
{
    public class RepositorioEntregablesLimpieza : IRepositorioEntregables
    {
        /************************************************ Servicio 1 - Limpieza **********************************/
        private readonly string _connectionString;
        public RepositorioEntregablesLimpieza(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }

        public async Task<List<Entregables>> getEntregables(int cedula)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getEntregablesByCedula", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedula", cedula));
                        var response = new List<Entregables>();
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

        public async Task<int> entregableFactura(Entregables entregables)
        {
            DateTime date = DateTime.Now;
            string date_str = date.ToString("yyyyMMddHHmmss");
            int id = 0;


            if (entregables.Id != 0)
            {
                int isDeleted = await eliminaArchivo(entregables);
            }

            string saveFile = await guardaArchivo(entregables.Archivo, entregables.Folio, date_str);
            try
            {
                if (saveFile.Equals("Ok"))
                {
                    using (SqlConnection sql = new SqlConnection(_connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_insertarActualizarEntregableInasistenciaLimpieza", sql))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@id", entregables.Id)).Direction = System.Data.ParameterDirection.Output;
                            if (entregables.Id != 0)
                                cmd.Parameters.Add(new SqlParameter("@ids", entregables.Id));

                            cmd.Parameters.Add(new SqlParameter("@cedulaLimpiezaId", entregables.CedulaLimpiezaId));
                            cmd.Parameters.Add(new SqlParameter("@tipo", entregables.Tipo));
                            cmd.Parameters.Add(new SqlParameter("@archivo", (date_str + "_" + entregables.Archivo.FileName)));
                            cmd.Parameters.Add(new SqlParameter("@tamanio", entregables.Archivo.Length));
                            cmd.Parameters.Add(new SqlParameter("@comentarios", entregables.Comentarios));

                            await sql.OpenAsync();
                            await cmd.ExecuteNonQueryAsync();

                            id = Convert.ToInt32(cmd.Parameters["@id"].Value);
                            
                        }
                    }
                }
                return id;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return 0;
            }
        }

        public async Task<string> guardaArchivo(IFormFile archivo, string folio, string date)
        {
            long size = archivo.Length;
            string folderCedula = folio;

            string newPath = Directory.GetCurrentDirectory() + "\\Entregables\\" + folderCedula;
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            using (var stream = new FileStream(newPath + "\\" + (date + "_" + archivo.FileName), FileMode.Create))
            {
                try
                {
                    await (archivo).CopyToAsync(stream);
                    return "Ok";
                }
                catch (Exception ex)
                {
                    return ex.Message.ToString();
                }
            }
        }

        public async Task<int> eliminaArchivo(Entregables entregable)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_eliminaEntregableLimpieza", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", entregable.Id));
                        cmd.Parameters.Add(new SqlParameter("@archivo",System.Data.SqlDbType.VarChar, 500)).Direction = System.Data.ParameterDirection.Output;
                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        string archivo = (cmd.Parameters["@archivo"].Value).ToString();
                        string newPath = Directory.GetCurrentDirectory() + "\\Entregables\\" + entregable.Folio + "\\" + archivo;
                        File.Delete(newPath);

                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return 0;
            }
        }

        public async Task<int> buscaEntregable(int id, string tipo)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_buscaEntregable", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", id));
                        cmd.Parameters.Add(new SqlParameter("@tipo", tipo));
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                return 1;
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

        /*Fin de los metodos para obtener los entregables*/

        /*Metodos para obtener los entregables adicionales*/
        public async Task<int> insertaCapacitacion(RespuestasAdicionales respuestasAdicionales)
        {
            int id = 0;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_insertaCapacitacionLimpieza", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", respuestasAdicionales.Id)).Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", respuestasAdicionales.CedulaLimpiezaId));
                        cmd.Parameters.Add(new SqlParameter("@pregunta", respuestasAdicionales.Pregunta));
                        cmd.Parameters.Add(new SqlParameter("@respuesta", respuestasAdicionales.Respuesta));
                        cmd.Parameters.Add(new SqlParameter("@penalizable", respuestasAdicionales.Penalizable));
                        
                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        id = Convert.ToInt32(cmd.Parameters["@id"].Value);
                    }
                }

                return id;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return -1;
            }
        }

        public async Task<int> insertaEntregablesBD(RespuestasAdicionales respuestasAdicionales)
        {
            int id = 0;

            DateTime date = DateTime.Now;
            string date_str = date.ToString("yyyyMMddHHmmss");


            if (respuestasAdicionales.Id != 0)
            {
                int isDeleted = await eliminaArchivoBD(respuestasAdicionales);
            }

            string saveFile = await guardaArchivo(respuestasAdicionales.Archivo, respuestasAdicionales.Folio, date_str);
            try
            {
                if (saveFile.Equals("Ok"))
                {
                    using (SqlConnection sql = new SqlConnection(_connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_insertaEntregableBDLimpieza", sql))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@id", respuestasAdicionales.Id)).Direction = System.Data.ParameterDirection.Output;
                            if(respuestasAdicionales.Id != 0)
                                cmd.Parameters.Add(new SqlParameter("@ids", respuestasAdicionales.Id));
                            cmd.Parameters.Add(new SqlParameter("@cedulaId", respuestasAdicionales.CedulaLimpiezaId));
                            cmd.Parameters.Add(new SqlParameter("@respuesta", respuestasAdicionales.Respuesta));
                            cmd.Parameters.Add(new SqlParameter("@archivo", (date_str + "_" + respuestasAdicionales.Archivo.FileName)));
                            cmd.Parameters.Add(new SqlParameter("@comentarios", respuestasAdicionales.Comentarios));
                            cmd.Parameters.Add(new SqlParameter("@fechaEntrega", respuestasAdicionales.FechaEntrega));
                            cmd.Parameters.Add(new SqlParameter("@fechaLimite", respuestasAdicionales.FechaLimite));
                            cmd.Parameters.Add(new SqlParameter("@penalizable", respuestasAdicionales.Penalizable));
                            cmd.Parameters.Add(new SqlParameter("@prioridad", respuestasAdicionales.Prioridad));

                            await sql.OpenAsync();
                            await cmd.ExecuteNonQueryAsync();

                            id = Convert.ToInt32(cmd.Parameters["@id"].Value);
                        }
                    }
                }
                return id;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return -1;
            }
        }

        public async Task<int> eliminaArchivoBD(RespuestasAdicionales respuestasAdicionales)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_eliminaEntregableBDLimpieza", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", respuestasAdicionales.Id));
                        cmd.Parameters.Add(new SqlParameter("@archivo", System.Data.SqlDbType.VarChar, 500)).Direction = System.Data.ParameterDirection.Output;
                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        string archivo = (cmd.Parameters["@archivo"].Value).ToString();
                        string newPath = Directory.GetCurrentDirectory() + "\\Entregables\\" + respuestasAdicionales.Folio + "\\" + archivo;
                        File.Delete(newPath);

                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return 0;
            }
        }

        public async Task<int> apruebaRechazaEntregable(Entregables entregables)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_autorizarRechazarEntregable", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", entregables.Id));
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", entregables.CedulaLimpiezaId));
                        cmd.Parameters.Add(new SqlParameter("@estatus", entregables.Estatus));
                        cmd.Parameters.Add(new SqlParameter("@servicio", "Limpieza"));

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

        public async Task<int> capturaHistorial(HistorialEntregables historialEntregables)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_insertaHistorialEntregables", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", "Limpieza"));
                        cmd.Parameters.Add(new SqlParameter("@tipo", historialEntregables.Tipo));
                        cmd.Parameters.Add(new SqlParameter("@cedula", historialEntregables.CedulaId));
                        cmd.Parameters.Add(new SqlParameter("@usuario", historialEntregables.UsuarioId));
                        cmd.Parameters.Add(new SqlParameter("@estatus", historialEntregables.Estatus));
                        cmd.Parameters.Add(new SqlParameter("@comentarios", historialEntregables.Comentarios));

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

        public async Task<List<HistorialEntregables>> getHistorialEntregables(object id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getHistorialEntregables", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", "Limpieza"));
                        cmd.Parameters.Add(new SqlParameter("@cedula", id));
                        var response = new List<HistorialEntregables>();
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

        public async Task<RespuestasAdicionales> getCapacitacionLimpieza(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getCapacitacionLimpieza", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", id));
                        var response = new RespuestasAdicionales();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response = MapToValueCapacitacion(reader);
                            }
                        }

                        return response;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<List<RespuestasAdicionales>> getEntregablesBDLimpieza(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getEntregablesBajoDemandaByCedula", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedula", id));
                        var response = new List<RespuestasAdicionales>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueEntregablesBD(reader));
                            }
                        }

                        return response;
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<int> eliminaEntregableBD(RespuestasAdicionales respuestasAdicionales)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_eliminaEntregableBDLimpieza", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", respuestasAdicionales.Id));
                        cmd.Parameters.Add(new SqlParameter("@archivo", System.Data.SqlDbType.VarChar, 500)).Direction = System.Data.ParameterDirection.Output;
                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        string archivo = (cmd.Parameters["@archivo"].Value).ToString();
                        string newPath = Directory.GetCurrentDirectory() + "\\Entregables\\" + respuestasAdicionales.Folio + "\\" + archivo;
                        File.Delete(newPath);

                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return 0;
            }
        }

        public async Task<int> calculaCalificacionEntregableBD(int cedula)
        {
            var send = 0;
            decimal calificacion = 0;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd2 = new SqlCommand("sp_calificacionEntregablesBDLimpieza", sql))
                    {
                        cmd2.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd2.Parameters.Add(new SqlParameter("@cedulaId", cedula));
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

        private RespuestasAdicionales MapToValueCapacitacion(SqlDataReader reader)
        {
            return new RespuestasAdicionales 
            { 
                Id = (int) reader["id"],
                CedulaLimpiezaId = (int) reader["CedulaLimpiezaId"],
                Pregunta = reader["Pregunta"].ToString(),
                Respuesta = (bool) reader["Respuesta"],
                Penalizable = (bool) reader["Penalizable"],
                MontoPenalizacion = (decimal)reader["MontoPenalizacion"],
                FechaCreacion = (DateTime) reader["FechaCreacion"]
            };
        }

        private RespuestasAdicionales MapToValueEntregablesBD(SqlDataReader reader)
        {
            return new RespuestasAdicionales
            {
                Id = (int)reader["id"],
                CedulaLimpiezaId = (int)reader["CedulaLimpiezaId"],
                Prioridad = (int)reader["Prioridad"],
                NombreArchivo = reader["Archivo"].ToString(),
                Pregunta = reader["Pregunta"].ToString(),
                Comentarios = reader["Comentarios"].ToString(),
                Respuesta = (bool)reader["Respuesta"],
                Penalizable = (bool)reader["Penalizable"],
                MontoPenalizacion = (decimal)reader["MontoPenalizacion"],
                FechaCreacion = (DateTime)reader["FechaCreacion"],
                FechaEntrega = (DateTime)reader["FechaEntrega"],
                FechaLimite = (DateTime)reader["FechaLimite"]
            };
        }
        private HistorialEntregables MapToValueHistorial(SqlDataReader reader)
        {
            return new HistorialEntregables
            {
                Servicio = reader["Servicio"].ToString(),
                Tipo = reader["Tipo"].ToString(),
                CedulaId = (int)reader["CedulaId"],
                UsuarioId = (int)reader["UsuarioId"],
                Estatus = reader["Estatus"].ToString(),
                Comentarios = reader["Comentarios"].ToString(),
                FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"])
            };
        }
        private Entregables MapToValue(SqlDataReader reader)
        {
            return new Entregables
            {
                Id = (int)reader["Id"],
                Tipo = reader["Tipo"].ToString(),
                Estatus = reader["Estatus"] != DBNull.Value ? reader["Estatus"].ToString() : "",
                NombreArchivo = reader["Archivo"].ToString(),
                FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"].ToString()),
                Comentarios = reader["Comentarios"].ToString(),
                Icono = reader["Icono"].ToString(),
                Color = reader["Color"].ToString()
            };
        }
        /*Fin de los metodos para obtener los entregables adicionales*/
    }
}
