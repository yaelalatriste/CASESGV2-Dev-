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
    public class RepositorioFinancieros : IRepositorioFinancieros
    {
        private readonly string _connectionString;

        public RepositorioFinancieros(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }

        public async Task<List<Dashboard>> GetCedulasFinancieros()
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getCedulasFinancieros", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var response = new List<Dashboard>();
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

        public async Task<List<Dashboard>> GetDetalleServicio(string servicio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getDetalleServicioFinancieros", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        var response = new List<Dashboard>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueServicio(reader));
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

        //DashBoard de Financieros
        private Dashboard MapToValue(SqlDataReader reader)
        {
            return new Dashboard
            {
                Servicio = reader["Servicio"].ToString(),
                Fondo = reader["Fondo"].ToString(),
                Icono = reader["Icono"].ToString(),
                Total = (int)reader["Total"]
            };
        }

        //Detalle de Servicio
        private Dashboard MapToValueServicio(SqlDataReader reader)
        {
            return new Dashboard
            {
                Servicio = reader["Servicio"].ToString(),
                Estatus = reader["Estatus"].ToString(),
                Mes = reader["Mes"].ToString(),
                Fondo = reader["Fondo"].ToString(),
                Icono = reader["Icono"].ToString(),
                Total = (int)reader["Total"]
            };
        }
    }
}
