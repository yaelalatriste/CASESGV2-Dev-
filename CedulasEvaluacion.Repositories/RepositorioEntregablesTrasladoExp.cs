using CedulasEvaluacion.Entities.Models;
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
    public class RepositorioEntregablesTrasladoExp : IRepositorioEntregablesTrasladoExp
    {
        /************************************************ Servicio 4 - Traslado Expedientes **********************************/
        private readonly string _connectionString;

        public RepositorioEntregablesTrasladoExp(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }

        public async Task<List<Entregables>> getEntregables(int cedula)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getEntregablesTrasladoByCedula", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedula", cedula));
                        var response = new List<Entregables>();
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
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<int> entregableFactura(Entregables entregables)
        {
            DateTime date = DateTime.Now;
            string date_str = date.ToString("yyyyMMddHHmmss");
            int id = 0;


            if (entregables.Id != 0)
            {
                int isDeleted = await eliminaArchivo(entregables);
            }

            string saveFile = await guardaArchivo(entregables.Archivo, entregables.Folio, date_str);
            try
            {
                if (saveFile.Equals("Ok"))
                {
                    using (SqlConnection sql = new SqlConnection(_connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_insertarActualizarEntregableTraslado", sql))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@id", entregables.Id)).Direction = ParameterDirection.Output;
                            if (entregables.Id != 0)
                                cmd.Parameters.Add(new SqlParameter("@ids", entregables.Id));

                            cmd.Parameters.Add(new SqlParameter("@cedulaTrasladoId", entregables.CedulaTrasladoId));
                            cmd.Parameters.Add(new SqlParameter("@tipo", entregables.Tipo));
                            cmd.Parameters.Add(new SqlParameter("@archivo", (date_str + "_" + entregables.Archivo.FileName)));
                            cmd.Parameters.Add(new SqlParameter("@tamanio", entregables.Archivo.Length));
                            cmd.Parameters.Add(new SqlParameter("@comentarios", entregables.Comentarios));

                            await sql.OpenAsync();
                            await cmd.ExecuteNonQueryAsync();

                            id = Convert.ToInt32(cmd.Parameters["@id"].Value);

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

        public async Task<string> guardaArchivo(IFormFile archivo, string folio, string date)
        {
            long size = archivo.Length;
            string folderCedula = folio;

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

        public async Task<int> eliminaArchivo(Entregables entregable)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_eliminaEntregableTraslado", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", entregable.Id));
                        cmd.Parameters.Add(new SqlParameter("@archivo", SqlDbType.VarChar, 500)).Direction = ParameterDirection.Output;
                        await sql.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        string archivo = (cmd.Parameters["@archivo"].Value).ToString();
                        string newPath = Directory.GetCurrentDirectory() + "\\Entregables\\" + entregable.Folio + "\\" + archivo;
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

        public async Task<int> buscaEntregable(int id, string tipo)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_buscaEntregableServicio", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedulaId", id));
                        cmd.Parameters.Add(new SqlParameter("@tipo", tipo));
                        cmd.Parameters.Add(new SqlParameter("@servicio", 4));
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                return 1;
                            }
                            return 0;
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

        private Entregables MapToValue(SqlDataReader reader)
        {
            return new Entregables
            {
                Id = (int)reader["Id"],
                Tipo = reader["Tipo"].ToString(),
                NombreArchivo = reader["Archivo"].ToString(),
                FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"].ToString()),
                Comentarios = reader["Comentarios"].ToString()
            };
        }
    }
}
