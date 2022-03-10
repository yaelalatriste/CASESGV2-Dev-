using CedulasEvaluacion.Entities.Login;
using CedulasEvaluacion.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using CedulasEvaluacion.Entities.Models;
using System.Xml;
using Newtonsoft.Json;
using CedulasEvaluacion.Entities.Vistas;

namespace CedulasEvaluacion.Repositories
{
    public class RepositorioLogin : IRepositorioLogin
    {
        private readonly string _connectionString;

        public RepositorioLogin(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection"); ;
        }

        //Busca el numero de usuario uxxxxx(x) con el que se se está intentando inciar sesión
        //y si existe actualizamos la contraseña por si existe alguna actualización
        public async Task<int> buscaUsuario(string usuario, string password)
        {
            int success = -1;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_buscaActualizaUsuario", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@usuario", usuario));
                        cmd.Parameters.Add(new SqlParameter("@password", password));
                        var response = new List<Dashboard>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                success = 1;
                                break;
                            }
                        }
                    }
                    return success;
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return success;
            }
        }

        public async Task<DatosUsuario> login(string usuario, string password)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_validaUsuario", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@usuario", usuario));
                        cmd.Parameters.Add(new SqlParameter("@password", password));
                        var response = new DatosUsuario();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response = MapToValueDU(reader);
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

        public async Task<List<Dashboard>> totalCedulas(int user)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_dashboard", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@usuario", user));
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

        public async Task<List<Dashboard>> CedulasEstatus(int user, string estatus)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_cedulasByEstatus", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@usuario", user));
                        cmd.Parameters.Add(new SqlParameter("@estatus", estatus));
                        var response = new List<Dashboard>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueEstatus(reader));
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

        public async Task<List<VCedulas>> ConcentradoCedulas(int user,string estatus)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getConcentradoCedulas", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@usuario", user));
                        cmd.Parameters.Add(new SqlParameter("@estatus", estatus));
                        var response = new List<VCedulas>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueConcentrado(reader));
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

        public async Task<List<VModulosUsuario>> getModulosByUser(int user)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getModulosByUser", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@usuario", user));
                        var response = new List<VModulosUsuario>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueVModulos(reader));
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

        private VModulosUsuario MapToValueVModulos(SqlDataReader reader)
        {
            return new VModulosUsuario
            {
                Id = (int)reader["Id"],
                PerfilId = (int)reader["PerfilId"],
                Empleado = reader["Empleado"].ToString(),
                Modulo = reader["Modulo"].ToString()
            };
        }

        private Dashboard MapToValue(SqlDataReader reader)
        {
            return new Dashboard {
                Estatus = reader["Estatus"].ToString(),
                Fondo = reader["Fondo"].ToString(),
                Icono = reader["Icono"].ToString(),
                Total = (int)reader["Total"]
            };
        }

        private Dashboard MapToValueEstatus(SqlDataReader reader)
        {
            return new Dashboard
            {
                Estatus = reader["Estatus"].ToString(),
                Servicio = reader["Servicio"].ToString(),
                Fondo = reader["Fondo"].ToString(),
                Icono = reader["Icono"].ToString(),
                Total = (int)reader["Total"]
            };
        }

        private DatosUsuario MapToValueDU(SqlDataReader reader)
        {
            return new DatosUsuario
            {
                Id = Convert.ToInt32(reader["Id"]),
                Expediente = Convert.ToInt32(reader["Expediente"]),
                Usuario = (reader["Usuario"]).ToString(),
                Empleado = (reader["Empleado"]).ToString(),
                CorreoElectronico = (reader["CorreoElectronico"]).ToString(),
                Puesto = (reader["Puesto"]).ToString(),
                Area = (reader["Area"]).ToString(),
                ClaveInmueble = Convert.ToInt32(reader["ClaveInmueble"]),
                Estatus = (reader["Estatus"]).ToString(),
                Perfiles = (reader["Perfiles"]).ToString(),
            };
        }

        private VCedulas MapToValueConcentrado(SqlDataReader reader)
        {
            return new VCedulas()
            {
                Id = (int)reader["Id"],
                Folio = reader["Folio"].ToString(),
                Abreviacion = reader["Abreviacion"].ToString(),
                Nombre = reader["Nombre"].ToString(),
                Mes = reader["Mes"].ToString(),
                Anio = (int)reader["Anio"],
                Servicio = reader["Servicio"].ToString(),
                Estatus = reader["Estatus"].ToString()
            };
        }
    }
}
