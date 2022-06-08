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
    public class RepositorioCedulasEvaluacion : IRepositorioCedulasEvaluacion
    {
        private readonly string _connectionString;

        public RepositorioCedulasEvaluacion(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection"); ;
        }

        public async Task<List<Usuarios>> GetUsuariosByAdministracion(int user)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getUsuariosByAdministracion", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@usuario", user));
                        var response = new List<Usuarios>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueUsuarios(reader));
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

        public async Task<int> GetVerificaFirmantes(string tipo,int user)
        {
            int r = 0;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getFirmanteTipo", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@usuario", user));
                        cmd.Parameters.Add(new SqlParameter("@tipo", tipo));
                        var response = new List<Usuarios>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                r = 1;
                            }
                            return r;
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
        private Usuarios MapToValueUsuarios(SqlDataReader reader)
        {
            return new Usuarios()
            {
                Id = (int)reader["Id"],
                NombreCompleto = reader["Empleado"].ToString(),
            };
        }
    }
}
