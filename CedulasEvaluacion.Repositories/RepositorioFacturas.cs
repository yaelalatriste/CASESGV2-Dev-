using CedulasEvaluacion.Entities.MFacturas;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace CedulasEvaluacion.Repositories
{
    public class RepositorioFacturas : IRepositorioFacturas
    {
        private readonly string _connectionString;

        public RepositorioFacturas(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection"); ;
        }

        /*************************** Limpieza ****************************/
        
        public async Task<List<Facturas>> getFacturas(int cedula,int servicio)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getFacturas", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@cedula", cedula));
                        cmd.Parameters.Add(new SqlParameter("@servicio", servicio));
                        var response = new List<Facturas>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueFacturas(reader));
                            }
                        }

                        foreach(var r in response)
                        {
                            r.concepto = await getConceptosFactura(r.Id);
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

        public async Task<List<Concepto>> getConceptosFactura(int factura)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_getConceptosFactura", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@factura", factura));
                        var response = new List<Concepto>();
                        await sql.OpenAsync();

                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                response.Add(MapToValueConcepto(reader));
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

        public async Task<Facturas> insertaFacturas(Facturas facturas)
        {
            await copiaFactura(facturas.Xml);
            facturas = desglozaXML(facturas);
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_insertaFactura", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.BigInt)).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add(new SqlParameter("@servicioId", facturas.ServicioId));
                        cmd.Parameters.Add(new SqlParameter("@cedula", facturas.CedulaId));
                        cmd.Parameters.Add(new SqlParameter("@rfc", facturas.emisor.RFC));
                        cmd.Parameters.Add(new SqlParameter("@iva", facturas.traslado.Importe));
                        if (facturas.retencion != null)
                            cmd.Parameters.Add(new SqlParameter("@retencion", facturas.retencion.Importe));
                        cmd.Parameters.Add(new SqlParameter("@nombre", facturas.emisor.Nombre));
                        cmd.Parameters.Add(new SqlParameter("@tipo", facturas.Tipo));
                        cmd.Parameters.Add(new SqlParameter("@usoCFDI", facturas.receptor.usoCFDI));
                        cmd.Parameters.Add(new SqlParameter("@uuid", facturas.timbreFiscal.UUID));
                        cmd.Parameters.Add(new SqlParameter("@serie", facturas.comprobante.Serie));
                        cmd.Parameters.Add(new SqlParameter("@folio", facturas.comprobante.Folio));
                        cmd.Parameters.Add(new SqlParameter("@fechaTimbrado", facturas.timbreFiscal.FechaTimbrado));
                        cmd.Parameters.Add(new SqlParameter("@subtotal", facturas.comprobante.SubTotal));
                        cmd.Parameters.Add(new SqlParameter("@total", facturas.comprobante.Total));

                        await sql.OpenAsync();

                        await cmd.ExecuteNonQueryAsync();

                        facturas.Id = Convert.ToInt32(cmd.Parameters["@id"].Value);

                        return facturas;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }

        public async Task<int> insertaConceptoFacturas(Facturas facturas)
        {
            Facturas factura = await insertaFacturas(facturas);
            double iva = 0;
            try
            {
                foreach (var fac in factura.concepto)
                {
                    iva = Convert.ToDouble(fac.Importe) * 0.16;
                    using (SqlConnection sql = new SqlConnection(_connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_insertaConceptoFactura", sql))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@factura", factura.Id));
                            cmd.Parameters.Add(new SqlParameter("@cantidad", fac.Cantidad));
                            cmd.Parameters.Add(new SqlParameter("@claveProducto", fac.ClaveProdServ));
                            cmd.Parameters.Add(new SqlParameter("@claveUnidad", fac.ClaveUnidad));
                            cmd.Parameters.Add(new SqlParameter("@unidad", fac.Unidad));
                            cmd.Parameters.Add(new SqlParameter("@descripcion", fac.Descripcion));
                            if (facturas.datosExtra != null) {
                                cmd.Parameters.Add(new SqlParameter("@folioSap", facturas.datosExtra.FolioSAP));
                                cmd.Parameters.Add(new SqlParameter("@numeroCliente", facturas.datosExtra.NumeroCliente));
                                cmd.Parameters.Add(new SqlParameter("@observacion", facturas.datosTotales.observGeneral1));
                            }
                            cmd.Parameters.Add(new SqlParameter("@precioUnitario", fac.ValorUnitario));
                            cmd.Parameters.Add(new SqlParameter("@subtotal", fac.Importe));
                            cmd.Parameters.Add(new SqlParameter("@descuento", fac.Descuento));
                            cmd.Parameters.Add(new SqlParameter("@iva", iva));

                            await sql.OpenAsync();
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return -1;
            }
        }

        public Facturas desglozaXML(Facturas facturas)
        {
            string newPath = Directory.GetCurrentDirectory() + "\\Facturas";

            XmlDocument doc = new XmlDocument();
            doc.Load(newPath + "\\" + facturas.Xml.FileName);
            try
            {
                XmlNamespaceManager namespaceManager = new XmlNamespaceManager(doc.NameTable);
                namespaceManager.AddNamespace("cfdi", "http://www.sat.gob.mx/cfd/3");
                namespaceManager.AddNamespace("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");
                namespaceManager.AddNamespace("ext", "http://www.buzone.com.mx/XSD/Addenda/EMEBuzWS");

                XmlNodeList ndComprobante = doc.SelectNodes("//cfdi:Comprobante", namespaceManager);
                XmlNodeList ndEmisor = doc.SelectNodes("//cfdi:Emisor", namespaceManager);
                XmlNodeList ndConcepto = doc.SelectNodes("//cfdi:Conceptos/cfdi:Concepto", namespaceManager);
                XmlNodeList nDTimbrado = doc.SelectNodes("//cfdi:Complemento/tfd:TimbreFiscalDigital", namespaceManager);
                XmlNodeList nDExtras = doc.SelectNodes("//cfdi:Addenda/ext:ElementosExtra/ext:DatosGenerales", namespaceManager);
                XmlNodeList nDTraslado= doc.SelectNodes("//cfdi:Traslados/cfdi:Traslado", namespaceManager);
                XmlNodeList nDTotales= doc.SelectNodes("//cfdi:Addenda/ext:ElementosExtra/ext:DatosTotales", namespaceManager);
                XmlNodeList nDReceptor = doc.SelectNodes("//cfdi:Receptor", namespaceManager);
                XmlNodeList nDRetenciones = doc.SelectNodes("//cfdi:Retencion", namespaceManager);

                string jsonComprobante = JsonConvert.SerializeXmlNode(ndComprobante[0], Newtonsoft.Json.Formatting.None, true);
                string jsonEmisor = JsonConvert.SerializeXmlNode(ndEmisor[0], Newtonsoft.Json.Formatting.None, true);
                string jsonTraslado = JsonConvert.SerializeXmlNode(nDTraslado[0], Newtonsoft.Json.Formatting.None, true);
                string jsonTimbrado = JsonConvert.SerializeXmlNode(nDTimbrado[0], Newtonsoft.Json.Formatting.None, true);
                string jsonExtra = JsonConvert.SerializeXmlNode(nDExtras[0], Newtonsoft.Json.Formatting.None, true);
                string jsonTotales = JsonConvert.SerializeXmlNode(nDTotales[0], Newtonsoft.Json.Formatting.None, true);
                string jsonReceptor = JsonConvert.SerializeXmlNode(nDReceptor[0], Newtonsoft.Json.Formatting.None, true);
                string jsonRetenciones = JsonConvert.SerializeXmlNode(nDRetenciones[0], Newtonsoft.Json.Formatting.None, true);


                List<Concepto> listaConcepto = new List<Concepto>();
                //Concepto concept = null;
                for (int i = 0; i < ndConcepto.Count; i++)
                {
                    string jsonConcepto = JsonConvert.SerializeXmlNode(ndConcepto[i], Newtonsoft.Json.Formatting.None, true);
                    jsonConcepto = jsonConcepto.Replace("@", "");
                    Concepto concept = JsonConvert.DeserializeObject<Concepto>(jsonConcepto);
                    listaConcepto.Add(concept);
                }

                jsonComprobante = jsonComprobante.Replace("@", "");
                jsonEmisor = jsonEmisor.Replace("@", "");
                jsonTraslado = jsonTraslado.Replace("@", "");
                jsonTimbrado = jsonTimbrado.Replace("@", "");
                jsonReceptor = jsonReceptor.Replace("@", "");

                Comprobante comp = JsonConvert.DeserializeObject<Comprobante>(jsonComprobante);
                Emisor emisor = JsonConvert.DeserializeObject<Emisor>(jsonEmisor);
                Traslado traslado = JsonConvert.DeserializeObject<Traslado>(jsonTraslado);
                TimbreFiscal tFiscal = JsonConvert.DeserializeObject<TimbreFiscal>(jsonTimbrado);
                Receptor receptor = JsonConvert.DeserializeObject<Receptor>(jsonReceptor);

                if (facturas.ServicioId == 11)
                {
                    jsonRetenciones = jsonRetenciones.Replace("@", "");
                    Retencion retencion = JsonConvert.DeserializeObject<Retencion>(jsonRetenciones);
                    facturas.retencion = retencion;
                }

                if (facturas.ServicioId == 3)
                {
                    jsonExtra = jsonExtra.Replace("@", "");
                    jsonTotales = jsonTotales.Replace("@", "");
                    DatosExtra de = JsonConvert.DeserializeObject<DatosExtra>(jsonExtra);
                    DatosTotales tot = JsonConvert.DeserializeObject<DatosTotales>(jsonTotales);
                    facturas.datosExtra = de;
                    facturas.datosTotales = tot;
                }

                facturas.comprobante = comp;
                facturas.emisor = emisor;
                facturas.traslado = traslado;
                facturas.concepto = listaConcepto;
                facturas.timbreFiscal = tFiscal;
                facturas.receptor = receptor;

                File.Delete(newPath + "\\" + facturas.Xml.FileName);

                return facturas;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }

        public async Task<int> copiaFactura(IFormFile factura)
        {
            string newPath = Directory.GetCurrentDirectory() + "\\Facturas";
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            using (var stream = new FileStream(newPath + "\\" + factura.FileName, FileMode.Create))
            {
                try
                {
                    await(factura).CopyToAsync(stream);
                    return 1;
                }
                catch (Exception ex)
                {
                    string msg = ex.Message.ToString();
                    return -1;
                }
            }
        }

        /*Metodo para Actualizar los datos de la Factura*/
        public async Task<Facturas> updateFacturas(Facturas facturas)
         {
            await copiaFactura(facturas.Xml);
            facturas = desglozaXML(facturas);
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_actualizaFacturaLimpieza", sql))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", facturas.Id));
                        cmd.Parameters.Add(new SqlParameter("@servicioId", facturas.ServicioId));
                        cmd.Parameters.Add(new SqlParameter("@cedula", facturas.CedulaId));
                        cmd.Parameters.Add(new SqlParameter("@rfc", facturas.emisor.RFC));
                        cmd.Parameters.Add(new SqlParameter("@iva", facturas.traslado.Importe));
                        if(facturas.retencion != null)
                            cmd.Parameters.Add(new SqlParameter("@retencion", facturas.retencion.Importe));
                        cmd.Parameters.Add(new SqlParameter("@nombre", facturas.emisor.Nombre));
                        cmd.Parameters.Add(new SqlParameter("@usoCFDI", facturas.receptor.usoCFDI));
                        cmd.Parameters.Add(new SqlParameter("@uuid", facturas.timbreFiscal.UUID));
                        cmd.Parameters.Add(new SqlParameter("@serie", facturas.comprobante.Serie));
                        cmd.Parameters.Add(new SqlParameter("@folio", facturas.comprobante.Folio));
                        cmd.Parameters.Add(new SqlParameter("@fechaTimbrado", facturas.timbreFiscal.FechaTimbrado));
                        cmd.Parameters.Add(new SqlParameter("@subtotal", facturas.comprobante.SubTotal));
                        cmd.Parameters.Add(new SqlParameter("@total", facturas.comprobante.Total));

                        await sql.OpenAsync();
                        int id = await cmd.ExecuteNonQueryAsync();

                        return facturas;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return null;
            }
        }

        public async Task<int> updateConceptoFacturas(Facturas facturas)
        {
            Facturas factura = await updateFacturas(facturas);
            double iva = 0;
            try
            {
                foreach(var fac in factura.concepto)
                {
                    iva = Convert.ToDouble(fac.Importe) * 0.16;
                    using (SqlConnection sql = new SqlConnection(_connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_insertaConceptoFactura", sql))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.Add(new SqlParameter("@factura", factura.Id));
                            cmd.Parameters.Add(new SqlParameter("@cantidad", fac.Cantidad));
                            cmd.Parameters.Add(new SqlParameter("@claveProducto", fac.ClaveProdServ));
                            cmd.Parameters.Add(new SqlParameter("@claveUnidad", fac.ClaveUnidad));
                            cmd.Parameters.Add(new SqlParameter("@unidad", fac.Unidad));
                            cmd.Parameters.Add(new SqlParameter("@descripcion", fac.Descripcion));
                            if (facturas.datosExtra != null)
                            {
                                cmd.Parameters.Add(new SqlParameter("@folioSap", facturas.datosExtra.FolioSAP));
                                cmd.Parameters.Add(new SqlParameter("@numeroCliente", facturas.datosExtra.NumeroCliente));
                                cmd.Parameters.Add(new SqlParameter("@observacion", facturas.datosTotales.observGeneral1));
                            }
                            cmd.Parameters.Add(new SqlParameter("@precioUnitario", fac.ValorUnitario));
                            cmd.Parameters.Add(new SqlParameter("@subtotal", fac.Importe));
                            cmd.Parameters.Add(new SqlParameter("@descuento", fac.Descuento));
                            cmd.Parameters.Add(new SqlParameter("@iva", iva));

                            await sql.OpenAsync();
                            await cmd.ExecuteNonQueryAsync();
                        }
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                return -1;
            }
        }
        /*FIN Metodo para Actualizar los datos de la factura*/

        /*Metodo para eliminar una factura*/
        public async Task<int> deleteFactura(int factura)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_eliminaFactura", sql))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@id", factura));

                        await sql.OpenAsync();
                        int id = await cmd.ExecuteNonQueryAsync();

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
        /*FIN Metodo para eliminar una  factura*/

        public decimal obtieneTotalFacturas(List<Facturas> facturas)
        {
            decimal total = 0;
            foreach (var fac in facturas)
            {
                if (!fac.Tipo.Equals("NC")) {
                    total += fac.comprobante.Total;
                }
            }
            return total;
        }
        /************************* Fin Facturas Limpieza **************************/

        private Facturas MapToValueFacturas(SqlDataReader reader)
        {
            Comprobante compro = new Comprobante();
            compro.Folio = (long)reader["Folio"];
            compro.Serie = reader["Serie"].ToString();
            compro.SubTotal = Convert.ToDecimal(reader["Subtotal"]);
            compro.Total = Convert.ToDecimal(reader["Total"]);

            Receptor recept = new Receptor();
            recept.usoCFDI = reader["UsoCFDI"].ToString();

            Emisor emi = new Emisor();
            emi.Nombre = reader["Nombre"].ToString();
            emi.RFC = reader["RFC"].ToString();

            Traslado traslado = new Traslado();
            traslado.Importe= (decimal)reader["IVA"];

            Retencion retencion = new Retencion();
            retencion.Importe = reader["RetencionIVA"] != DBNull.Value ? (decimal)reader["RetencionIVA"]: 0;

            TimbreFiscal timbre = new TimbreFiscal();
            timbre.UUID = reader["UUID"].ToString();
            timbre.FechaTimbrado = Convert.ToDateTime(reader["FechaTimbrado"]);

            return new Facturas
            {
                Id = (int)reader["Id"],
                CedulaId = (int)reader["CedulaId"],
                Descripcion = reader["Descripcion"].ToString(),
                Tipo = reader["Tipo"] != DBNull.Value ? reader["Tipo"].ToString():"",
                comprobante = compro,
                emisor = emi,
                receptor = recept,
                timbreFiscal = timbre,
                traslado = traslado,
                retencion = retencion
            };
        }

        private Concepto MapToValueConcepto(SqlDataReader reader)
        {
            DatosExtra de = new DatosExtra();
            de.FolioSAP = reader["FolioSAP"] != DBNull.Value ? (long)reader["FolioSAP"]:0;
            de.NumeroCliente = reader["NumeroCliente"] != DBNull.Value ? (long)reader["NumeroCliente"]:0;

            DatosTotales dt = new DatosTotales();
            dt.observGeneral1 = reader["ObservacionGeneral"] != DBNull.Value ? reader["ObservacionGeneral"].ToString():"";

            return new Concepto { 
                FacturaId = (int)reader["FacturaId"],
                Cantidad = (decimal)reader["Cantidad"],
                ClaveProdServ = (int)reader["ClaveProducto"],
                ClaveUnidad =  reader["ClaveUnidad"].ToString(),
                Unidad = reader["Unidad"].ToString(),
                Descripcion = reader["Descripcion"].ToString(),
                ValorUnitario = (decimal) reader["PrecioUnitario"],
                Importe = (decimal)reader["Subtotal"],
                IVA = (decimal)reader["IVA"],
                datosExtra = de,
                datosTotales = dt
            };
        }
    }
}
