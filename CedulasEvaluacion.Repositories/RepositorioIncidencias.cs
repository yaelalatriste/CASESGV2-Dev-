using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.Vistas;
using CedulasEvaluacion.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Repositories
{
    public class RepositorioIncidencias : IRepositorioIncidencias
    {
        private readonly string _connectionString;

        public RepositorioIncidencias(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection"); ;
        }

        /*INCIDENCIAS (NO EQUIPO)*/

        public async Task<List<CatalogoIncidencias>> getTiposIncidencias()
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getTiposIncidenciasLimpieza", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        var response = new List<CatalogoIncidencias>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueCatalogoIncidencias(reader, "tipo"));
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

        public async Task<List<CatalogoIncidencias>> getNombresByTipos(string tipo)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.sp_getNombreByTipoIncidenciaLimpieza", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@tipo", tipo));
                        var response = new List<CatalogoIncidencias>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueCatalogoIncidencias(reader, "nombre"));
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

        public async Task<int> guardaIncidencia(IncidenciasLimpieza incidencia)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_insertarIncidenciaLimpieza", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", incidencia.Id)).Direction = System.Data.ParameterDirection.Output;
                        cmd.Parameters.Add(new SqlParameter("@cedula", incidencia.CedulaLimpiezaId));
                        cmd.Parameters.Add(new SqlParameter("@pregunta", incidencia.Pregunta));
                        cmd.Parameters.Add(new SqlParameter("@tipo", incidencia.Incidencia.Tipo));
                        cmd.Parameters.Add(new SqlParameter("@nombre", incidencia.Incidencia.Nombre));
                        cmd.Parameters.Add(new SqlParameter("@fechaIncidencia", incidencia.FechaIncidencia));
                        cmd.Parameters.Add(new SqlParameter("@comentarios", incidencia.Comentarios));
                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                        int id = (int)cmd.Parameters["@id"].Value;
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

        public async Task<List<VIncidenciasLimpieza>> getIncidencias(int cedulaId)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getIncidenciasLimpieza", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", cedulaId));
                        var response = new List<VIncidenciasLimpieza>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueIncidenciasLimpieza(reader));
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

        public async Task<IncidenciasLimpieza> getIncidenciaBeforeUpdate(int incidenciaId)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getIncidenciaLimpieza", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", incidenciaId));
                        var response = new IncidenciasLimpieza();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response = MapToValueIncidenciaLimpieza(reader);
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

        public async Task<int> updateIncidencia(IncidenciasLimpieza incidencia)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_actualizaIncidenciaLimpieza", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", incidencia.Id));
                        cmd.Parameters.Add(new SqlParameter("@cedula", incidencia.CedulaLimpiezaId));
                        cmd.Parameters.Add(new SqlParameter("@tipo", incidencia.Incidencia.Tipo));
                        cmd.Parameters.Add(new SqlParameter("@nombre", incidencia.Incidencia.Nombre));
                        cmd.Parameters.Add(new SqlParameter("@fechaIncidencia", incidencia.FechaIncidencia));
                        cmd.Parameters.Add(new SqlParameter("@comentarios", incidencia.Comentarios));
                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                        int id = (int)cmd.Parameters["@id"].Value;
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

        public async Task<int> deleteIncidencia(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_eliminaIncidencia", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
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
                return 0;
            }
        }


        /*FIN INCIDENCIAS (NO EQUIPO)*/

        /********************INCIDENCIAS DE EQUIPO*************************/

        public async Task<List<VIncidenciasLimpieza>> getIncidenciasEquipo(int cedulaId)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getIncidenciasEquipoLimpieza", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", cedulaId));
                        var response = new List<VIncidenciasLimpieza>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueIncidenciasLimpieza(reader));
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

        /********************FIN INCIDENCIAS DE EQUIPO*************************/

        private CatalogoIncidencias MapToValueCatalogoIncidencias(SqlDataReader reader, string tipo)
        {
            if (tipo.Equals("tipo"))
            {
                return new CatalogoIncidencias()
                {
                    Tipo = reader["Tipo"].ToString()
                };
            }
            else
            {
                return new CatalogoIncidencias()
                {
                    Nombre = reader["Nombre"].ToString()
                };
            }
        }

        private VIncidenciasLimpieza MapToValueIncidenciasLimpieza(SqlDataReader reader)
        {
            return new VIncidenciasLimpieza()
            {
                Id = (int)reader["Id"],
                FechaIncidencia = (DateTime)reader["FechaIncidencia"],
                Tipo = reader["Tipo"].ToString(),
                Nombre = reader["Nombre"].ToString(),
                Comentarios = reader["Comentarios"].ToString()
            };
        }

        private IncidenciasLimpieza MapToValueIncidenciaLimpieza(SqlDataReader reader)
        {
            CatalogoIncidencias inci = new CatalogoIncidencias();
            inci.Tipo = reader["Tipo"].ToString();
            inci.Nombre = reader["Nombre"].ToString();

            return new IncidenciasLimpieza()
            {
                Id = (int)reader["Id"],
                FechaIncidencia = (DateTime)reader["FechaIncidencia"],
                Comentarios = reader["Comentarios"].ToString(),
                Incidencia = inci
            };
        }
    }
}
