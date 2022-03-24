using CedulasEvaluacion.Entities.MCatalogoServicios;
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
    public class RepositorioCatalogoServicios : IRepositorioCatalogoServicios
    {
        private readonly string _connectionString;

        public RepositorioCatalogoServicios(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }

        public async Task<List<CatalogoServicios>> GetCatalogoServicios()
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getCatalogoServicios", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var response = new List<CatalogoServicios>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueCatalogoServicios(reader));
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

        private CatalogoServicios MapToValueCatalogoServicios(SqlDataReader reader)
        {
            return new CatalogoServicios
            {
                Id = (int)reader["Id"],
                Nombre = reader["Nombre"].ToString(),
                Descripcion = reader["Descripcion"].ToString(),
            };
        }
    }
}
