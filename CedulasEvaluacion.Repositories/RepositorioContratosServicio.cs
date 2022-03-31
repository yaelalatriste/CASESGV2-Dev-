using CedulasEvaluacion.Entities.MContratos;
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
    public class RepositorioContratosServicio : IRepositorioContratosServicio
    {
        private readonly string _connectionString;

        public RepositorioContratosServicio(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }
        public async Task<List<ContratosServicio>> GetContratosServicios(int servicio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getContratosByServicio", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        var response = new List<ContratosServicio>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueContratos(reader));
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
        public async Task<ContratosServicio> GetContratoServicioActivo(int servicio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getContratoServicioActivo", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        var response = new ContratosServicio();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response = MapToValueContratos(reader);
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
        public async Task<ContratosServicio> GetContratoServicioById(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getContratoServicioById", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        var response = new ContratosServicio();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response = MapToValueContratos(reader);
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
        public async Task<int> InsertaContrato(ContratosServicio contratosServicio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_insertaContratoServicio", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", contratosServicio.Id)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new SqlParameter("@usuarioId", contratosServicio.UsuarioId));
                        cmd.Parameters.Add(new SqlParameter("@servicioId", contratosServicio.ServicioId));
                        cmd.Parameters.Add(new SqlParameter("@contrato", contratosServicio.NumeroContrato));
                        cmd.Parameters.Add(new SqlParameter("@empresa", contratosServicio.Empresa));
                        cmd.Parameters.Add(new SqlParameter("@representante", contratosServicio.Representante));
                        cmd.Parameters.Add(new SqlParameter("@fechaInicio", contratosServicio.FechaInicio));
                        cmd.Parameters.Add(new SqlParameter("@fechaFin", contratosServicio.FechaFin));
                        cmd.Parameters.Add(new SqlParameter("@activo", contratosServicio.Activo));


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
        private ContratosServicio MapToValueContratos(SqlDataReader reader)
        {
            return new ContratosServicio
            {
                Id = (int)reader["Id"],
                ServicioId = (int)reader["ServicioId"],
                NumeroContrato = reader["NoContrato"] != DBNull.Value ? reader["NoContrato"].ToString() : "",
                Empresa = reader["Empresa"] != DBNull.Value ? reader["Empresa"].ToString() : "",
                Representante = reader["Representante"] != DBNull.Value ? reader["Representante"].ToString() : "",
                FechaInicio = reader["FechaInicio"] != DBNull.Value ? Convert.ToDateTime(reader["FechaInicio"]) : DateTime.Now,
                FechaFin = reader["FechaFin"] != DBNull.Value ? Convert.ToDateTime(reader["FechaFin"]) : DateTime.Now,
                Activo = reader["Activo"] != DBNull.Value ? (bool)reader["Activo"] : false
            };
        }
    }
}
