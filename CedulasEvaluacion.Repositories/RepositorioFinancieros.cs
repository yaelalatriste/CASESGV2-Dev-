﻿using CASESGCedulasEvaluacion.Entities.Vistas;
using CedulasEvaluacion.Entities.MFinancieros;
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
        public async Task<List<DashboardFinancieros>> GetCedulasFinancieros()
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getCedulasFinancieros", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        var response = new List<DashboardFinancieros>();
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
        public async Task<List<DashboardFinancieros>> GetDetalleServicio(string servicio, int anio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getDetalleServicioFinancieros", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        cmd.Parameters.Add(new SqlParameter("@anio", anio));
                        var response = new List<DashboardFinancieros>();
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
        public async Task<List<Oficio>> GetOficiosFinancieros(string servicio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getOficiosFinancieros", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        var response = new List<Oficio>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueOficios(reader));
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
        public async Task<Oficio> GetOficioById(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getOficioById", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        var response = new Oficio();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response = MapToValueOficios(reader);
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
        //Obtiene las Cédulas en Trámite de Pago de un Servicio Específico
        public async Task<List<DetalleCedula>> GetCedulasTramitePago(int id, int servicio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getCedulasTramitePagoMes", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        cmd.Parameters.Add(new SqlParameter("@oficio", id));
                        var response = new List<DetalleCedula>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueDetalle(reader));
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
        public async Task<List<DetalleCedula>> GetCedulasOficio(int id, int servicio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getCedulasOficio", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@oficio", id));
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        var response = new List<DetalleCedula>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueDetalle(reader));
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
        public async Task<int> insertarNuevoOficio(Oficio oficio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_insertaOficioFinancieros", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", oficio.Id)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new SqlParameter("@servicioId", oficio.ServicioId));
                        cmd.Parameters.Add(new SqlParameter("@anio", oficio.Anio));
                        cmd.Parameters.Add(new SqlParameter("@numero", oficio.NumeroOficio));

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
                return 0;
            }
        }
        public async Task<int> CancelarOficio(int id, int servicio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_cancelarOficioDGPPT", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@oficio", id));
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));

                        await sql.OpenAsync();
                        int i = await cmd.ExecuteNonQueryAsync();
                        if (i > 0)
                        {
                            return 1;
                        }
                        else
                        {
                            return -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return 0;
            }
        }
        public async Task<int> PagarOficio(int id, int servicio, DateTime fecha)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_pagarOficio", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@oficio", id));
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        cmd.Parameters.Add(new SqlParameter("@fechaPagado", fecha));

                        await sql.OpenAsync();
                        int i = await cmd.ExecuteNonQueryAsync();
                        if (i > 0)
                        {
                            return 1;
                        }
                        else
                        {
                            return -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return 0;
            }
        }
        public async Task<int> insertarCedulasOficio(List<CedulasOficio> cedulas)
        {
            int id = 0;
            try
            {
                foreach (var ced in cedulas)
                {
                    using (SqlConnection sql = new SqlConnection(_connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_insertaCedulaOficio", sql))
                        {
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@oficio", ced.OficioId));
                            cmd.Parameters.Add(new SqlParameter("@cedula", ced.CedulaId));
                            cmd.Parameters.Add(new SqlParameter("@servicio", ced.ServicioId));

                            await sql.OpenAsync();
                            int i = await cmd.ExecuteNonQueryAsync();
                            if (i > 0)
                            {
                                id = 1;
                            }
                            else
                            {
                                id = -1;
                            }
                        }
                    }
                }
                return id;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return 0;
            }
        }
        public async Task<int> EliminaCedulasOficio(int oficio, int servicio, int cedula)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_eliminarCedulaOficio", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@oficio", oficio));
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        cmd.Parameters.Add(new SqlParameter("@cedula", cedula));

                        await sql.OpenAsync();
                        int i = await cmd.ExecuteNonQueryAsync();
                        if (i > 0)
                        {
                            return 1;
                        }
                        else
                        {
                            return -1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return 0;
            }
        }
        public async Task<int> GetTramiteOficio(int id, int servicio)
        {
            int success = -1;
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_tramitarOficioDGPPT", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@oficio", id));
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));

                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                        int i = await cmd.ExecuteNonQueryAsync();
                        if (i > 0)
                        {
                            success = 1;
                        }
                        else
                        {
                            success = -1;
                        }

                        return success;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return 0;
            }
        }
        //DashBoard de Financieros
        private DashboardFinancieros MapToValue(SqlDataReader reader)
        {
            return new DashboardFinancieros
            {
                Servicio = reader["Servicio"].ToString(),
                Fondo = reader["Fondo"].ToString(),
                Icono = reader["Icono"].ToString(),
                Total = (int)reader["Total"]
            };
        }
        //Detalle de Servicio
        private DashboardFinancieros MapToValueServicio(SqlDataReader reader)
        {
            return new DashboardFinancieros
            {
                Servicio = reader["Servicio"].ToString(),
                Descripcion = reader["Descripcion"].ToString(),
                Estatus = reader["Estatus"].ToString(),
                Mes = reader["Mes"].ToString(),
                Fondo = reader["Fondo"].ToString(),
                Icono = reader["Icono"].ToString(),
                Total = (int)reader["Total"],
                ServicioId = (int)reader["ServicioId"]
            };
        }
        private DetalleCedula MapToValueDetalle(SqlDataReader reader)
        {
            return new DetalleCedula
            {
                Id = (int)reader["Id"],
                ServicioId = (int)reader["ServicioId"],
                Servicio = reader["Servicio"].ToString(),
                Inmueble = reader["Inmueble"].ToString(),
                Estatus = reader["Estatus"].ToString(),
                Folio = reader["Folio"].ToString(),
                Mes = reader["Mes"].ToString(),
                Anio = (int)reader["Anio"],
                Subtotal = (decimal)reader["Subtotal"],
                Total = (decimal)reader["Total"],
                IVA = (decimal)reader["IVA"],
            };
        }
        private Oficio MapToValueOficios(SqlDataReader reader)
        {
            return new Oficio
            {
                Id = (int)reader["Id"],
                Anio = (int)reader["Anio"],
                ServicioId = reader["ServicioId"] != DBNull.Value ? (int)reader["ServicioId"]:0,
                NumeroOficio = (int)reader["NumeroOficio"],
                Servicio = reader["Nombre"].ToString(),
                Estatus = reader["Estatus"].ToString(),
                SubtotalOficio = reader["SubtotalOficio"] != DBNull.Value ? Convert.ToDecimal(reader["SubtotalOficio"]):0,
                TotalOficio = reader["TotalOficio"] != DBNull.Value ? Convert.ToDecimal(reader["TotalOficio"]) : 0,
                FechaPagado = reader["FechaPagado"] != DBNull.Value ? Convert.ToDateTime(reader["FechaPagado"]):Convert.ToDateTime("01/01/0001"),
            };
        }
    }
}
