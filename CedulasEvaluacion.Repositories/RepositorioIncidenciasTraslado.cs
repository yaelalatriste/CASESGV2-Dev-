using CedulasEvaluacion.Entities.TrasladoExp;
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
    public class RepositorioIncidenciasTraslado : IRepositorioIncidenciasTraslado
    {
        private readonly string _connectionString;

        public RepositorioIncidenciasTraslado (IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }

        public async Task<int> InsertaIncidencia(IncidenciasTraslado incidenciasTraslado)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_insertaIncidenciaTraslado", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", incidenciasTraslado.Id)).Direction = System.Data.ParameterDirection.Output;
                        if (incidenciasTraslado.Id != 0)
                            cmd.Parameters.Add(new SqlParameter("@ids", incidenciasTraslado.Id));


                        cmd.Parameters.Add(new SqlParameter("@cedulaId", incidenciasTraslado.CedulaTrasladoId));
                        cmd.Parameters.Add(new SqlParameter("@pregunta", incidenciasTraslado.Pregunta));
                        cmd.Parameters.Add(new SqlParameter("@comentarios", incidenciasTraslado.Comentarios));
                        cmd.Parameters.Add(new SqlParameter("@fechaIncumplida", incidenciasTraslado.FechaIncumplida.Date));

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
                return -1;
            }
        }
        public async Task<int> ActualizaIncidencia(IncidenciasTraslado incidenciasTraslado)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_actualizaIncidenciaTraslado", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", incidenciasTraslado.Id));
                        cmd.Parameters.Add(new SqlParameter("@comentarios", incidenciasTraslado.Comentarios));
                        cmd.Parameters.Add(new SqlParameter("@fechaIncumplida", incidenciasTraslado.FechaIncumplida.Date));

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
        public async Task<List<IncidenciasTraslado>> getIncidencias(int cedulaId)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getIncidenciasTraslado", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", cedulaId));
                        var response = new List<IncidenciasTraslado>();
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
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<int> EliminaIncumplimiento(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_eliminaIncidenciaTraslado", sql))
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
        private IncidenciasTraslado MapToValue(SqlDataReader reader)
        {
            return new IncidenciasTraslado {
                Id = (int)reader["Id"],
                CedulaTrasladoId = (int)reader["CedulaTrasladoId"],
                Pregunta = (int)reader["Pregunta"],
                FechaIncumplida = Convert.ToDateTime(reader["FechaIncumplida"]),
                Comentarios = reader["Comentarios"] != DBNull.Value ? reader["Comentarios"].ToString() : ""
            };
        }
    }
}
