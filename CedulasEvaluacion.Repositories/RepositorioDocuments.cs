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
    public class RepositorioDocuments : IRepositorioDocuments
    {
        private readonly string _connectionString;

        public RepositorioDocuments(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection"); ;
        }

        public async Task<List<ActaEntregaRecepcion>> getFacturasActa(int cedula, int servicio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getDatosActaERLimpieza", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", cedula));
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        var response = new List<ActaEntregaRecepcion>();
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

        public async Task<ActaEntregaRecepcion> getDatosActa(int cedula, int servicio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getDatosActaERLimpieza", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", cedula));
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        var response = new ActaEntregaRecepcion();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response = MapToValue(reader);
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

        private ActaEntregaRecepcion MapToValue(SqlDataReader reader)
        {
            return new ActaEntregaRecepcion
            {
                Id = (int)reader["Id"],
                Mes = reader["Mes"].ToString(),
                Folio = reader["Folio"].ToString(),
                Anio = (int)reader["Anio"],
                Contrato = reader["Contrato"].ToString(),
                TipoInmueble = reader["TipoInmueble"].ToString(),
                Servicio = reader["Servicio"].ToString(),
                Inmueble = reader["Inmueble"].ToString(),
                InmuebleC = reader["InmuebleC"].ToString(),
                Direccion = reader["Direccion"].ToString(),
                Estado = reader["Estado"].ToString(),
                Administrador = reader["Administrador"].ToString(),
                Elaboro = reader["Elaboro"].ToString(),
                Reviso = reader["Reviso"].ToString(),
                FechaInicio = Convert.ToDateTime(reader["FechaInicio"]),
                FechaFin = Convert.ToDateTime(reader["FechaFin"]),
            };
        }
    }
}
