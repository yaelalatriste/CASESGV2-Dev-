using CedulasEvaluacion.Entities.MMensajeria;
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
    public class RepositorioReporteCedula :IRepositorioReporteCedula
    {
        private readonly string _connectionString;
        public RepositorioReporteCedula(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }
        public async Task<IEnumerable<VCedulas>> getCedulasMensajeria()
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getCedulasMensajeria", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@user", 3));
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
    }
}
