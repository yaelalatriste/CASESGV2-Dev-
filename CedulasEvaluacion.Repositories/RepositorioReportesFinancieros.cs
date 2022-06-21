using CedulasEvaluacion.Entities.MFinancieros;
using CedulasEvaluacion.Entities.Reportes;
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
    public class RepositorioReportesFinancieros : IRepositorioReportesFinancieros
    {
        private readonly string _connectionString;

        public RepositorioReportesFinancieros(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }

        public async Task<List<ReporteCedula>> GetCedulasFinancieros(string mes, int anio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_generaReporteMensualPAT", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@mes", mes));
                        cmd.Parameters.Add(new SqlParameter("@anio", anio));
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

        public async Task<List<ReporteCedula>> GetReportePagos(string mes, int anio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_generaReportePagos", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@mes", mes));
                        cmd.Parameters.Add(new SqlParameter("@anio", anio));
                        var response = new List<ReporteCedula>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValuePagos(reader));
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
            return new ReporteCedula
            {
                Id = (int)reader["Id"],
                Servicio = reader["Servicio"] != DBNull.Value ? reader["Servicio"].ToString() : "",
                Inmueble = reader["Inmueble"] != DBNull.Value ? reader["Inmueble"].ToString() : "",
                Folio = reader["Folio"] != DBNull.Value ? reader["Folio"].ToString() : "",
                Mes = reader["Mes"] != DBNull.Value ? reader["Mes"].ToString() : "",
                Anio = reader["Anio"] != DBNull.Value ? (int)reader["Anio"] : 0,
                Estatus = reader["Estatus"] != DBNull.Value ? reader["Estatus"].ToString() : "",
                Empresa = reader["Empresa"] != DBNull.Value ? reader["Empresa"].ToString() : "",
            };
        }

        private ReporteCedula MapToValuePagos(SqlDataReader reader)
        {
            return new ReporteCedula
            {
                Id = (int) reader["Id"],
                Servicio = reader["Servicio"] != DBNull.Value ? reader["Servicio"].ToString():"",
                Anio = reader["Anio"] != DBNull.Value ? (int)reader["Anio"] : 0,
                Mes = reader["Mes"] != DBNull.Value ? reader["Mes"].ToString() : "",
                TotalCedulas = reader["TotalCedulas"] != DBNull.Value ? (int) reader["TotalCedulas"] : 0,
                DiasTranscurridos = reader["DiasTranscurridos"] != DBNull.Value ? (int) reader["DiasTranscurridos"] : 0,
                NumeroOficio = reader["NumeroOficio"] != DBNull.Value ? reader["NumeroOficio"].ToString():"",
                FechaTramitado = reader["FechaTramitado"] != DBNull.Value ? Convert.ToDateTime(reader["FechaTramitado"]):Convert.ToDateTime("01/01/1990"),
                FechaPagado = reader["FechaPagado"] != DBNull.Value ? Convert.ToDateTime(reader["FechaPagado"]):Convert.ToDateTime("01/01/1990"),
                Estatus = reader["Estatus"] != DBNull.Value ? reader["Estatus"].ToString() : "",
            };
        }
    }
}
