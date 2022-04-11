using CedulasEvaluacion.Entities.MMensajeria;
using CedulasEvaluacion.Entities.Reportes;
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
        public async Task<IEnumerable<ReporteCedula>> getReporteMensajeria(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_ReporteMensajeriaById", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        var response = new List<ReporteCedula>();
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

        private ReporteCedula MapToValue(SqlDataReader reader)
        {
            return new ReporteCedula()
            {
                Id = (int)reader["Id"],
                Inmueble = reader["Inmueble"].ToString(),
                Folio = reader["Folio"].ToString(),
                Mes = reader["Mes"].ToString(),
                Anio = (int)reader["Anio"],
                Administracion = reader["Administracion"].ToString(),
                Servicio = reader["Servicio"].ToString(),
                Estatus = reader["Estatus"].ToString(),
                Calificacion = reader["Calificacion"].ToString(),
                FechaCreacion = reader["FechaCreacion"].ToString(),
                Facturas = reader["Facturas"].ToString(),
                MontosFacturas = reader["MontosFacturas"].ToString(),
            };
        }
    }
}
