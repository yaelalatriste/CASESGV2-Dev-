using CedulasEvaluacion.Entities.MCatalogoServicios;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Repositories
{
    public class RepositorioEntregablesContrato : IRepositorioEntregablesContrato
    {
        private readonly string _connectionString;

        public RepositorioEntregablesContrato(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }

        public async Task<List<EntregablesContrato>> GetEntregablesCS(int contrato)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getEntregablesCS", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@contrato", contrato));
                        var response = new List<EntregablesContrato>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueEntregablesContratos(reader));
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

        public async Task<List<EntregablesContrato>> GetEntregableCsById(int id)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getEntregableCsById", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        var response = new List<EntregablesContrato>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueEntregablesContratos(reader));
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

        public async Task<int> InsertaContrato(EntregablesContrato entregable)
        {
            DateTime date = DateTime.Now;
            string date_str = date.ToString("yyyyMMddHHmmss");
            int id = 0;

            if (entregable.Id != 0)
            {
                int isDeleted = await eliminaArchivo(entregable);
            }

            string saveFile = await guardaArchivo(entregable.Archivo, "Contrato_"+entregable.ContratoId,date_str);
            try
            {
                if (saveFile.Equals("Ok"))
                {
                    using (SqlConnection sql = new SqlConnection(_connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_insertaContratoServicio", sql))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@id", entregable.Id)).Direction = ParameterDirection.Output;
                            cmd.Parameters.Add(new SqlParameter("@contratoId", entregable.ContratoId));
                            cmd.Parameters.Add(new SqlParameter("@descripcion", entregable.Descripcion));
                            cmd.Parameters.Add(new SqlParameter("@tipoContrato", entregable.TipoContrato));
                            cmd.Parameters.Add(new SqlParameter("@tipo", entregable.Tipo));
                            cmd.Parameters.Add(new SqlParameter("@archivo", entregable.Archivo));
                            cmd.Parameters.Add(new SqlParameter("@tamanio", entregable.Tamanio));
                            cmd.Parameters.Add(new SqlParameter("@inicioPeriodo", entregable.InicioPeriodo));
                            cmd.Parameters.Add(new SqlParameter("@finPeriodo", entregable.FinPeriodo));
                            cmd.Parameters.Add(new SqlParameter("@fechaProgramada", entregable.FechaProgramada));
                            cmd.Parameters.Add(new SqlParameter("@fechaEntrega", entregable.FechaEntrega));
                            cmd.Parameters.Add(new SqlParameter("@comentarios", entregable.Comentarios));


                            await sql.OpenAsync();
                            await cmd.ExecuteNonQueryAsync();
                            id = Convert.ToInt32(cmd.Parameters["@id"].Value);
                            return id;
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

        public async Task<string> guardaArchivo(IFormFile archivo, string contrato, string date)
        {
            long size = archivo.Length;
            string folderCedula = "Contrato_"+contrato;

            string newPath = Directory.GetCurrentDirectory() + "\\Entregables\\" + folderCedula;
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            using (var stream = new FileStream(newPath + "\\" + (date + "_" + archivo.FileName), FileMode.Create))
            {
                try
                {
                    await (archivo).CopyToAsync(stream);
                    return "Ok";
                }
                catch (Exception ex)
                {
                    return ex.Message.ToString();
                }
            }
        }

        public async Task<int> eliminaArchivo(EntregablesContrato entregable)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_eliminaEntregableContrato", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", entregable.Id));
                        cmd.Parameters.Add(new SqlParameter("@archivo", SqlDbType.VarChar, 500)).Direction = ParameterDirection.Output;
                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        string archivo = (cmd.Parameters["@archivo"].Value).ToString();
                        string newPath = Directory.GetCurrentDirectory() + "\\Entregables\\Contrato_" + entregable.ContratoId + "\\" + archivo;
                        File.Delete(newPath);

                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return 0;
            }
        }

        private EntregablesContrato MapToValueEntregablesContratos(SqlDataReader reader)
        {
            return new EntregablesContrato {
                Id = (int)reader["Id"],
                ContratoId = (int)reader["ContratoId"],
                Descripcion = reader["Descripcion"] != DBNull.Value ? reader["Descripcion"].ToString() : "",
                Tipo = reader["Tipo"] != DBNull.Value ? reader["Tipo"].ToString() : "",
                TipoContrato = reader["TipoContrato"] != DBNull.Value ? reader["TipoContrato"].ToString() : "",
                InicioPeriodo = reader["InicioPeriodo"] != DBNull.Value ? Convert.ToDateTime(reader["InicioPeriodo"].ToString()):DateTime.Now,
                FinPeriodo = reader["FinPeriodo"] != DBNull.Value ?Convert.ToDateTime(reader["FinPeriodo"].ToString()):DateTime.Now,
                FechaProgramada = reader["FechaProgramada"] != DBNull.Value ? Convert.ToDateTime(reader["FechaProgramada"].ToString()):DateTime.Now,
                FechaEntrega = reader["FechaEntrega"] != DBNull.Value ?Convert.ToDateTime(reader["FechaEntrega"].ToString()):DateTime.Now,
                Comentarios = reader["Comentarios"] != DBNull.Value ? reader["Comentarios"].ToString():"",
                NombreArchivo = reader["Archivo"].ToString(),
                FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"].ToString()),
            };
        }
    }
}
