using CedulasEvaluacion.Entities.Models;
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
    public class RepositorioModulos : IRepositorioModulos
    {
        private readonly string _connectionString;

        public RepositorioModulos(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection"); ;
        }

        public async Task<List<Modulos>> getModulos()
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getModulos", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var response = new List<Modulos>();
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

        private Modulos MapToValue(SqlDataReader reader)
        {
            return new Modulos
            {
                Id = (int)reader["Id"],
                Nombre = reader["Nombre"].ToString()
            };
        }
    }
}
