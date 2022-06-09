using CedulasEvaluacion.Entities.MCedula;
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
        public async Task<int> GetVerificaFirmantes(string tipo,int inmueble,int servicio)
        {
            int r = 0;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getFirmanteTipo", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@inmueble", inmueble));
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
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
        public async Task<int> insertaFirmante(FirmantesServicio firmante)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_insertaFirmanteByCedula", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", firmante.Id)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new SqlParameter("@usuario", firmante.UsuarioId));
                        cmd.Parameters.Add(new SqlParameter("@inmueble", firmante.InmuebleId));
                        cmd.Parameters.Add(new SqlParameter("@servicio", firmante.ServicioId));
                        cmd.Parameters.Add(new SqlParameter("@tipo", firmante.Tipo));

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
