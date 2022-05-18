using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using CedulasEvaluacion.Entities.Models;
using System.Threading.Tasks;
using System.Drawing;
using CedulasEvaluacion.Entities.Vistas;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using System.Linq;
using System.Globalization;
using CedulasEvaluacion.Entities.MFacturas;
using CedulasEvaluacion.Entities.MMensajeria;
using CedulasEvaluacion.Entities.MCelular;
using CedulasEvaluacion.Entities.MConvencional;
using Microsoft.AspNetCore.Authorization;
using CedulasEvaluacion.Entities.MFumigacion;
using CedulasEvaluacion.Entities.MAgua;
using CedulasEvaluacion.Entities.MResiduos;
using CedulasEvaluacion.Entities.MTransporte;
using CedulasEvaluacion.Entities.MMuebles;
using CedulasEvaluacion.Entities.TrasladoExp;
using CedulasEvaluacion.Entities.MAnalisis;
using CedulasEvaluacion.Entities.MCedula;

namespace CedulasEvaluacion.Controllers
{
    [Authorize]
    public class DocumentsController : ControllerBase
    {
        private readonly IRepositorioEvaluacionServicios vCedula;

        private readonly IRepositorioLimpieza vlimpieza;
        private readonly IRepositorioIncidencias vIncidencias;
        private readonly IRepositorioInmuebles vInmuebles;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IRepositorioDocuments vDocuments;
        private readonly IRepositorioEntregables vEntregables;

        private readonly IRepositorioMensajeria vMensajeria;
        private readonly IRepositorioIncidenciasMensajeria viMensajeria;

        private readonly IRepositorioCelular vCelular;
        private readonly IRepositorioIncidenciasCelular iCelular;

        private readonly IRepositorioTrasladoExp vTraslado;
        private readonly IRepositorioIncidenciasTraslado iTraslado;

        private readonly IRepositorioFumigacion vFumigacion;
        private readonly IRepositorioIncidenciasFumigacion iFumigacion;

        private readonly IRepositorioAnalisis vAnalisis;
        private readonly IRepositorioIncidenciasAnalisis iAnalisis;

        private readonly IRepositorioMuebles vMuebles;
        private readonly IRepositorioIncidenciasMuebles iMuebles;

        private readonly IRepositorioTransporte vTransporte;
        private readonly IRepositorioIncidenciasTransporte iTransporte;

        private readonly IRepositorioAgua vAgua;
        private readonly IRepositorioIncidenciasAgua iAgua;

        private readonly IRepositorioResiduos vResiduos;
        private readonly IRepositorioIncidenciasResiduos iResiduos;

        private readonly IRepositorioConvencional vConvencional;
        private readonly IRepositorioIncidenciasConvencional iConvencional;
        private readonly IRepositorioEntregablesConvencional eConvencional;

        private readonly IRepositorioFacturas vFacturas;
        private readonly IRepositorioPerfiles vRepositorioPerfiles;

        public DocumentsController(IRepositorioEvaluacionServicios viCedula, IRepositorioFumigacion iVFumigacion, IRepositorioAgua iVAgua, IRepositorioInmuebles iVInmueble, IRepositorioUsuarios iVUsuario,
                                    IRepositorioIncidenciasFumigacion iIncidenciasFumigacion, IRepositorioIncidenciasAgua iIncidenciasAgua, IRepositorioIncidenciasResiduos iiResiduos,
                                    IRepositorioResiduos ivResiduos, IRepositorioTransporte ivTransporte, IRepositorioIncidenciasTransporte iiTransporte, IRepositorioIncidenciasTraslado iiTraslado,
                                   IRepositorioEntregables iVEntregables, IRepositorioPerfiles iRepositorioPerfiles, IRepositorioTrasladoExp ivTraslado,
                                    IRepositorioFacturas iFacturas, IRepositorioCelular iCelular,
                                    IRepositorioIncidenciasCelular ivCelular, IRepositorioConvencional iConvencional, IRepositorioIncidenciasConvencional ivConvencional,
                                    IRepositorioEntregablesConvencional ieConvencional, IRepositorioDocuments ivDocuments, IRepositorioIncidenciasMuebles iiMuebles,
                                    IRepositorioMuebles iVMuebles, IRepositorioAnalisis ivAnalisis, IRepositorioIncidenciasAnalisis iiAnalisis, IRepositorioInmuebles viInmuebles,
                                    IRepositorioFacturas viFacturas, IRepositorioIncidencias iiIncidencias)
        {

            this.vIncidencias = iiIncidencias ?? throw new ArgumentNullException(nameof(iiIncidencias));
            this.vCedula = viCedula ?? throw new ArgumentNullException(nameof(viCedula));
            this.vDocuments = ivDocuments ?? throw new ArgumentNullException(nameof(ivDocuments));

            this.vCelular = iCelular ?? throw new ArgumentNullException(nameof(iCelular));
            this.iCelular = ivCelular ?? throw new ArgumentNullException(nameof(ivCelular));

            this.vResiduos = ivResiduos ?? throw new ArgumentNullException(nameof(ivResiduos));
            this.iResiduos = iiResiduos ?? throw new ArgumentNullException(nameof(iiResiduos));

            this.vTransporte = ivTransporte ?? throw new ArgumentNullException(nameof(ivTransporte));
            this.iTransporte = iiTransporte ?? throw new ArgumentNullException(nameof(ivTransporte));

            this.vTraslado = ivTraslado ?? throw new ArgumentNullException(nameof(ivTraslado));
            this.iTraslado = iiTraslado ?? throw new ArgumentNullException(nameof(iiTraslado));

            this.vAgua = iVAgua ?? throw new ArgumentNullException(nameof(iVAgua));
            this.iAgua = iIncidenciasAgua ?? throw new ArgumentNullException(nameof(iIncidenciasAgua));

            this.vAnalisis = ivAnalisis ?? throw new ArgumentNullException(nameof(ivAnalisis));
            this.iAnalisis = iiAnalisis ?? throw new ArgumentNullException(nameof(iiAnalisis));

            this.vFumigacion = iVFumigacion ?? throw new ArgumentNullException(nameof(iVFumigacion));
            this.iFumigacion = iIncidenciasFumigacion ?? throw new ArgumentNullException(nameof(iIncidenciasFumigacion));

            this.vMuebles = iVMuebles ?? throw new ArgumentNullException(nameof(iVMuebles));
            this.iMuebles = iiMuebles ?? throw new ArgumentNullException(nameof(iiMuebles));

            this.vConvencional = iConvencional ?? throw new ArgumentNullException(nameof(iConvencional));
            this.iConvencional = ivConvencional ?? throw new ArgumentNullException(nameof(ivConvencional));
            this.eConvencional = ieConvencional ?? throw new ArgumentNullException(nameof(ieConvencional));

            this.vInmuebles = viInmuebles ?? throw new ArgumentNullException(nameof(viInmuebles));

            this.vUsuarios = iVUsuario ?? throw new ArgumentNullException(nameof(iVUsuario));
            this.vEntregables = iVEntregables ?? throw new ArgumentNullException(nameof(iVEntregables));
            this.vRepositorioPerfiles = iRepositorioPerfiles ?? throw new ArgumentNullException(nameof(iRepositorioPerfiles));
            this.vFacturas = viFacturas ?? throw new ArgumentNullException(nameof(viFacturas));
        }

        [Route("/reporte/trasladoExp/{id}")]
        public async Task<IActionResult> CedulaTraslado(int id)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), moduloTraslado(), "cédula");
            if (success == 1)
            {
                string strFacturas = "";
                decimal totalFacturas = 0;

                CedulaEvaluacion cedula = new CedulaEvaluacion();
                cedula = await vCedula.CedulaById(id);
                cedula.usuarios = await vUsuarios.getUserById(cedula.UsuarioId);
                cedula.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedula.RespuestasEncuesta = await vCedula.obtieneRespuestas(id);
                cedula.facturas = new List<Facturas>();
                cedula.facturas = await vFacturas.getFacturas(id, 4);

                Document document = new Document();
                var path = @"E:\Plantillas CASESGV2\DocsV2\ReporteTraslado.docx";
                document.LoadFromFile(path);

                //Creamos la Tabla
                Section tablas = document.AddSection();
                cedula.incidencias = new Entities.MIncidencias.ModelsIncidencias();
                cedula.incidencias.traslado = await iTraslado.getIncidenciasByPregunta(id, 1);
                //obtenemos el documento con marcas
                if (cedula.incidencias.traslado.Count > 0)
                {
                    Table tablaActividades = tablas.AddTable(true);

                    String[] cabeceraFechas = { "Fecha de la Incidencia", "Personal Solicitado", "Personal Brindado" };

                    tablaActividades.ResetCells(cedula.incidencias.traslado.Count + 1, cabeceraFechas.Length);

                    TableRow recRow = tablaActividades.Rows[0];
                    recRow.IsHeader = true;
                    recRow.Height = 10;

                    recRow.RowFormat.BackColor = Color.FromArgb(81, 25, 162);
                    recRow.RowFormat.Borders.BorderType = BorderStyle.Single;


                    for (int i = 0; i < cabeceraFechas.Length; i++)
                    {
                        //Alineacion de celdas
                        Paragraph p = recRow.Cells[i].AddParagraph();
                        tablaActividades.Rows[0].Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        p.Format.HorizontalAlignment = HorizontalAlignment.Center;

                        //Formato de datos
                        TextRange TR = p.AppendText(cabeceraFechas[i]);
                        TR.CharacterFormat.FontName = "Arial";
                        TR.CharacterFormat.FontSize = 9;
                        TR.CharacterFormat.Bold = true;
                        TR.CharacterFormat.TextColor = Color.White;
                    }

                    for (int r = 0; r < cedula.incidencias.traslado.Count; r++)
                    {
                        TableRow DataRow = tablaActividades.Rows[r + 1];
                        //Fila Height
                        DataRow.Height = 5;
                        for (int c = 0; c < cabeceraFechas.Length; c++)
                        {
                            TextRange TR2 = null;
                            //Alineacion de Celdas
                            DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                            //Llenar datos en filas
                            Paragraph p2 = DataRow.Cells[c].AddParagraph();
                            if (c == 0)
                            {
                                TR2 = p2.AppendText(cedula.incidencias.traslado[r].FechaIncumplida.ToString("dd/MM/yyyy"));
                            }
                            if (c == 1)
                            {
                                TR2 = p2.AppendText(cedula.incidencias.traslado[r].PersonalSolicitado + "");
                            }
                            if (c == 2)
                            {
                                TR2 = p2.AppendText(cedula.incidencias.traslado[r].PersonalBrindado + "");
                            }
                            //Formato de celdas
                            p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                            TR2.CharacterFormat.FontName = "Arial";
                            TR2.CharacterFormat.FontSize = 9;
                        }
                    }

                    BookmarksNavigator marcaActividades = new BookmarksNavigator(document);
                    marcaActividades.MoveToBookmark("Personal", true, true);
                    marcaActividades.InsertTable(tablaActividades);
                    document.Replace("||Personal||", "El personal de servicio no cumplió con la maquinaria, equipo o  herramientas necesarias para prestar el servicio.", false, true);
                }
                else
                {
                    document.Replace("||Personal||", "El personal de servicio cumplió con la maquinaria, equipo o  herramientas necesarias para prestar el servicio.", false, true);
                }

                cedula.incidencias.traslado = await iTraslado.getIncidenciasByPregunta(id, 3);
                //obtenemos el documento con marcas
                if (cedula.incidencias.traslado.Count > 0)
                {
                    Table tablaActividades = tablas.AddTable(true);

                    String[] cabeceraFechas = { "Fecha de la Incidencia", "Material o Equipo Faltante" };

                    tablaActividades.ResetCells(cedula.incidencias.traslado.Count + 1, cabeceraFechas.Length);

                    TableRow recRow = tablaActividades.Rows[0];
                    recRow.IsHeader = true;
                    recRow.Height = 10;

                    recRow.RowFormat.BackColor = Color.FromArgb(81, 25, 162);
                    recRow.RowFormat.Borders.BorderType = BorderStyle.Single;


                    for (int i = 0; i < cabeceraFechas.Length; i++)
                    {
                        //Alineacion de celdas
                        Paragraph p = recRow.Cells[i].AddParagraph();
                        tablaActividades.Rows[0].Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        p.Format.HorizontalAlignment = HorizontalAlignment.Center;

                        //Formato de datos
                        TextRange TR = p.AppendText(cabeceraFechas[i]);
                        TR.CharacterFormat.FontName = "Arial";
                        TR.CharacterFormat.FontSize = 9;
                        TR.CharacterFormat.Bold = true;
                        TR.CharacterFormat.TextColor = Color.White;
                    }

                    for (int r = 0; r < cedula.incidencias.traslado.Count; r++)
                    {
                        TableRow DataRow = tablaActividades.Rows[r + 1];
                        //Fila Height
                        DataRow.Height = 5;
                        for (int c = 0; c < cabeceraFechas.Length; c++)
                        {
                            TextRange TR2 = null;
                            //Alineacion de Celdas
                            DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                            //Llenar datos en filas
                            Paragraph p2 = DataRow.Cells[c].AddParagraph();
                            if (c == 0)
                            {
                                TR2 = p2.AppendText(cedula.incidencias.traslado[r].FechaIncumplida.ToString("dd/MM/yyyy"));
                            }
                            if (c == 1)
                            {
                                string coments = "";
                                string[] comentarios = cedula.incidencias.traslado[r].Comentarios.Split("|");
                                for (var m = 0; m < comentarios.Length - 1; m++)
                                {
                                    if ((m + 1) == comentarios.Length - 1)
                                    {
                                        coments += comentarios[m];
                                    }
                                    else
                                    {
                                        coments += comentarios[m] + ",";
                                    }

                                }
                                TR2 = p2.AppendText(coments);
                            }
                            //Formato de celdas
                            p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                            TR2.CharacterFormat.FontName = "Arial";
                            TR2.CharacterFormat.FontSize = 9;
                        }
                    }

                    BookmarksNavigator marcaActividades = new BookmarksNavigator(document);
                    marcaActividades.MoveToBookmark("Equipo", true, true);
                    marcaActividades.InsertTable(tablaActividades);
                    document.Replace("||Equipo||", "El personal de servicio no cumplió con la maquinaria, equipo o  herramientas necesarias para prestar el servicio.", false, true);
                }
                else
                {
                    document.Replace("||Equipo||", "El personal de servicio cumplió con la maquinaria, equipo o  herramientas necesarias para prestar el servicio.", false, true);
                }

                if (cedula.RespuestasEncuesta[1].Respuesta == true)
                {
                    document.Replace("||Uniforme||", "El personal se presentó debidamente uniformado e identificado al prestar el servicio.", false, true);
                }
                else
                {
                    document.Replace("||Uniforme||", "El personal no se presentó debidamente uniformado e identificado al prestar el servicio.", false, true);
                }

                document.Replace("||Folio||", cedula.Folio, false, true);

                document.Replace("||Mes||", cedula.Mes, false, true);
                if (!cedula.Estatus.Equals("Enviada a DAS") && !cedula.Estatus.Equals("Rechazada"))
                {
                    document.Replace("||C||", cedula.Calificacion.ToString(), false, true);
                }
                else
                {
                    document.Replace("||C||", "Pendiente", false, true);
                }

                document.Replace("||dia||", cedula.FechaCreacion.GetValueOrDefault().Day + "", false, true);
                document.Replace("||MesE||", Convert.ToDateTime(cedula.FechaCreacion).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")), false, true);
                document.Replace("||Anio||", Convert.ToDateTime(cedula.FechaCreacion.GetValueOrDefault()).Year + "", false, true);


                for (int i = 0; i < cedula.facturas.Count; i++)
                {
                    if ((cedula.facturas.Count - 1) != i)
                    {
                        strFacturas += cedula.facturas[i].comprobante.Serie + cedula.facturas[i].comprobante.Folio + "/";
                    }
                    else
                    {
                        strFacturas += cedula.facturas[i].comprobante.Serie + cedula.facturas[i].comprobante.Folio;
                    }
                }

                if (cedula.Estatus.Equals("Enviado a DAS") || cedula.Estatus.Equals("Rechazada"))
                {
                    BookmarksNavigator marcaNota = new BookmarksNavigator(document);
                    marcaNota.MoveToBookmark("Nota", true, true);
                    marcaNota.InsertText("NOTA: Esta cédula no cuenta con ningún valor ya que aún no está AUTORIZADA por parte de la Dirección de Administración de Servicios.");
                }

                document.Replace("||Factura||", strFacturas, false, true);

                document.Replace("||Total||", String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:C}", vFacturas.obtieneTotalFacturas(cedula.facturas)) + "", false, true);

                if (cedula.Estatus.Equals("Enviado a DAS") || cedula.Estatus.Equals("Rechazada"))
                {
                    BookmarksNavigator marcaNota = new BookmarksNavigator(document);
                    marcaNota.MoveToBookmark("Nota", true, true);
                    marcaNota.InsertText("NOTA: Esta cédula no cuenta con ningún valor ya que aún no está AUTORIZADA por parte de la Dirección de Administración de Servicios.");
                }


                //Salvar y Lanzar

                byte[] toArray = null;
                using (MemoryStream ms1 = new MemoryStream())
                {
                    document.SaveToStream(ms1, Spire.Doc.FileFormat.PDF);
                    toArray = ms1.ToArray();
                }
                return File(toArray, "application/pdf", "CedulaTraslado_" + cedula.Mes + ".pdf");
            }
            return Redirect("/error/denied");
        }

        [Route("/documents/actaLimpieza/{id?}")]
        public async Task<IActionResult> getActaEntregaRecepcionLimpieza(int id)
        {

            ActaEntregaRecepcion cedula = new ActaEntregaRecepcion();
            CedulaEvaluacion cedulaLimpieza = new CedulaEvaluacion();
            cedulaLimpieza = await vCedula.CedulaById(id);
            cedula = await vDocuments.getDatosActa(id, 1);
            cedula.facturas = new List<Facturas>();
            cedula.facturas = await vFacturas.getFacturas(id, 1);


            Document document = new Document();
            var path = @"E:\Plantillas CASESGV2\DocsV2\Acta ER\Acta Entrega - Recepción 2022.docx";
            document.LoadFromFile(path);

            //Creamos la Tabla
            Section tablas = document.AddSection();

            document.Replace("|Periodo|", cedula.FechaInicio.Day + " de " + cedula.FechaInicio.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " + cedula.FechaInicio.Year + " al " +
                 cedula.FechaFin.Day + " de " + cedula.FechaFin.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " + cedula.FechaFin.Year, false, true);

            document.Replace("|Ciudad|", cedula.Estado, false, true);
            if (cedula.Estado.Equals("Ciudad de México"))
            {
                document.Replace("|Estado|", "en la " + cedula.Estado, false, true);
            }
            else
            {
                document.Replace("|Estado|", "en el " + cedula.Estado, false, true);
                document.Replace("|Estado|", "en " + cedula.Estado, false, true);
            }

            document.Replace("|InmuebleP|", cedula.InmuebleC, false, true);
            document.Replace("|DomicilioInmueble|", cedula.Direccion, false, true);
            document.Replace("|ResponsableInmueble|", cedula.Administrador, false, true);
            document.Replace("|Reviso|", cedula.Reviso, false, true);
            document.Replace("|Usuario|", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cedula.Elaboro.ToLower()), false, true);
            document.Replace("|InmuebleCedula|", CultureInfo.CurrentCulture.TextInfo.ToUpper(cedula.Inmueble), false, true);
            document.Replace("|Inmueble|", CultureInfo.CurrentCulture.TextInfo.ToUpper(cedula.InmuebleC) + "\n", false, true);

            document.Replace("|Folio|", cedula.Folio, false, true);

            document.Replace("|MesEval|", cedula.Mes, false, true);


            DateTime fechaActual = DateTime.Now;
            document.Replace("|Dia|", fechaActual.Day + "", false, true);
            document.Replace("|Mes|", fechaActual.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")), false, true);
            document.Replace("|Anio|", fechaActual.Year + "", false, true);
            document.Replace("|Hora|", fechaActual.Hour + ":00", false, true);
            document.Replace("|DiaActual|", fechaActual.Day + " de " + fechaActual.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " + fechaActual.Year, false, true);
            document.Replace("|HoraActual|", fechaActual.Hour + ":00", false, true);
            document.Replace("|Hora|", fechaActual.Hour + ":00", false, true);
            if ((await vIncidencias.getIncidencias(id)).Count == 0)
            {
                document.Replace("|Declaraciones|", "Se hace constar que los servicios solicitados fueron atendidos a entera satisfacción del Consejo de la Judicatura Federal conforme se visualiza en la cédula automatizada para la supervisión y evaluación de servicios generales.", false, true);
            }
            else
            {
                document.Replace("|Declaraciones|", "\nSe hace constar que los servicios fueron recibidos por el Consejo de la Judicatura Federal, presentando incidencias, mismas que se vierten en la cédula automatizada para la supervisión y evaluación de servicios generales.", false, true);
            }

            string strFacturas = "";
            string strNotas = "";
            string strNTimbrado = "";
            string strCantidades = "";
            string strCantidadesNota = "";
            string strTimbrado = "";
            decimal total = 0, totalNC = 0;
            for (int i = 0; i < cedula.facturas.Count; i++)
            {
                if ((cedula.facturas.Count - 1) != i && i != 0)
                {
                    if (!cedula.facturas[i].comprobante.Serie.Equals("NC"))
                    {
                        strFacturas += cedula.facturas[i].comprobante.Serie + cedula.facturas[i].comprobante.Folio + "/";
                        strCantidades += Convert.ToInt32(cedula.facturas[i].concepto[0].Cantidad) + "/";
                        strTimbrado += cedula.facturas[i].timbreFiscal.FechaTimbrado + "/";

                        total += cedula.facturas[i].comprobante.Total;
                    }
                    else
                    {
                        strNotas += cedula.facturas[i].comprobante.Serie + cedula.facturas[i].comprobante.Folio + "/";
                        strNTimbrado += cedula.facturas[i].timbreFiscal.FechaTimbrado + "/";
                        strCantidadesNota += Convert.ToInt32(cedula.facturas[i].concepto[0].Cantidad) + "/";
                        totalNC += cedula.facturas[i].comprobante.Total;
                    }
                }
                else
                {
                    if (!cedula.facturas[i].comprobante.Serie.Equals("NC"))
                    {
                        strFacturas += cedula.facturas[i].comprobante.Serie + cedula.facturas[i].comprobante.Folio;
                        strCantidades += Convert.ToInt32(cedula.facturas[i].concepto[0].Cantidad);
                        strTimbrado += cedula.facturas[i].timbreFiscal.FechaTimbrado;
                        total += cedula.facturas[i].comprobante.Total;
                    }
                    else
                    {
                        strNotas += cedula.facturas[i].comprobante.Serie + cedula.facturas[i].comprobante.Folio;
                        strNTimbrado += cedula.facturas[i].timbreFiscal.FechaTimbrado;
                        strCantidadesNota += Convert.ToInt32(cedula.facturas[i].concepto[0].Cantidad);
                        totalNC += cedula.facturas[i].comprobante.Total;
                    }
                }
            }

            document.Replace("|ImporteIVA|", String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:C}", total), false, true);
            document.Replace("|CantidadServicios|", strCantidades + "", false, true);
            document.Replace("|FechaTimbrado|", strTimbrado, false, true);
            document.Replace("|FolioFactura|", strFacturas, false, true);

            //Notas de Crédito
            document.Replace("|ImporteNota|", String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:C}", totalNC), false, true);
            document.Replace("|CantidadNota|", strCantidadesNota.Equals("") ? "N/A" : strCantidadesNota + "", false, true);
            document.Replace("|TimbradoNota|", strNTimbrado.Equals("") ? "N/A" : strNTimbrado, false, true);
            document.Replace("|FolioNota|", strNotas.Equals("") ? "N/A" : strNotas, false, true);

            //Salvar y Lanzar

            byte[] toArray = null;
            using (MemoryStream ms1 = new MemoryStream())
            {
                document.SaveToStream(ms1, Spire.Doc.FileFormat.Docx2013);
                toArray = ms1.ToArray();
            }
            return File(toArray, "application/ms-word", "ActaER_Limpieza_" + cedula.Mes + ".docx");
        }

        [Route("/documents/alcanceActa/{id?}")]
        public async Task<IActionResult> getAActaEntregaRecepcionLimpieza(int id)
        {

            ActaEntregaRecepcion cedula = new ActaEntregaRecepcion();
            CedulaEvaluacion cedulaLimpieza = new CedulaEvaluacion();
            cedulaLimpieza = await vCedula.CedulaById(id);
            cedula = await vDocuments.getDatosActa(id, 1);
            cedula.facturas = new List<Facturas>();
            cedula.facturas = await vFacturas.getFacturas(id, 1);


            Document document = new Document();
            var path = @"E:\Plantillas CASESGV2\DocsV2\Acta ER\Acta Entrega - Recepción 2022 Alcance.docx";
            document.LoadFromFile(path);

            //Creamos la Tabla
            Section tablas = document.AddSection();

            document.Replace("|Periodo|", cedula.FechaInicio.Day + " de " + cedula.FechaInicio.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " + cedula.FechaInicio.Year + " al " +
                 cedula.FechaFin.Day + " de " + cedula.FechaFin.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " + cedula.FechaFin.Year, false, true);

            document.Replace("|Ciudad|", cedula.Estado, false, true);
            if (cedula.Estado.Equals("Ciudad de México"))
            {
                document.Replace("|Estado|", "en la " + cedula.Estado, false, true);
            }
            else
            {
                document.Replace("|Estado|", "en el " + cedula.Estado, false, true);
                document.Replace("|Estado|", "en " + cedula.Estado, false, true);
            }

            document.Replace("|InmuebleP|", cedula.InmuebleC, false, true);
            document.Replace("|DomicilioInmueble|", cedula.Direccion, false, true);
            document.Replace("|ResponsableInmueble|", cedula.Administrador, false, true);
            document.Replace("|Reviso|", cedula.Reviso, false, true);
            document.Replace("|Usuario|", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cedula.Elaboro.ToLower()), false, true);
            document.Replace("|InmuebleCedula|", CultureInfo.CurrentCulture.TextInfo.ToUpper(cedula.Inmueble), false, true);
            document.Replace("|Inmueble|", CultureInfo.CurrentCulture.TextInfo.ToUpper(cedula.InmuebleC) + "\n", false, true);

            document.Replace("|Folio|", cedula.Folio, false, true);

            document.Replace("|MesEval|", cedula.Mes, false, true);


            DateTime fechaActual = DateTime.Now;
            document.Replace("|Dia|", fechaActual.Day + "", false, true);
            document.Replace("|Mes|", fechaActual.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")), false, true);
            document.Replace("|Anio|", fechaActual.Year + "", false, true);
            document.Replace("|Hora|", fechaActual.Hour + ":00", false, true);
            document.Replace("|DiaActual|", fechaActual.Day + " de " + fechaActual.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " + fechaActual.Year, false, true);
            document.Replace("|HoraActual|", fechaActual.Hour + ":00", false, true);
            document.Replace("|Hora|", fechaActual.Hour + ":00", false, true);
            if ((await vIncidencias.getIncidencias(id)).Count == 0)
            {
                document.Replace("|Declaraciones|", "Se hace constar que los servicios solicitados fueron atendidos a entera satisfacción del Consejo de la Judicatura Federal conforme se visualiza en la cédula automatizada para la supervisión y evaluación de servicios generales.", false, true);
            }
            else
            {
                document.Replace("|Declaraciones|", "\nSe hace constar que los servicios fueron recibidos por el Consejo de la Judicatura Federal, presentando incidencias, mismas que se vierten en la cédula automatizada para la supervisión y evaluación de servicios generales.", false, true);
            }

            string strFacturas = "";
            string strNotas = "";
            string strNTimbrado = "";
            string strCantidades = "";
            string strCantidadesNota = "";
            string strTimbrado = "";
            decimal total = 0, totalNC = 0;
            for (int i = 0; i < cedula.facturas.Count; i++)
            {
                if ((cedula.facturas.Count - 1) != i && i != 0)
                {
                    if (!cedula.facturas[i].comprobante.Serie.Equals("NC"))
                    {
                        strFacturas += cedula.facturas[i].comprobante.Serie + cedula.facturas[i].comprobante.Folio + "/";
                        strCantidades += Convert.ToInt32(cedula.facturas[i].concepto[0].Cantidad) + "/";
                        strTimbrado += cedula.facturas[i].timbreFiscal.FechaTimbrado + "/";

                        total += cedula.facturas[i].comprobante.Total;
                    }
                    else
                    {
                        strNotas += cedula.facturas[i].comprobante.Serie + cedula.facturas[i].comprobante.Folio + "/";
                        strNTimbrado += cedula.facturas[i].timbreFiscal.FechaTimbrado + "/";
                        strCantidadesNota += Convert.ToInt32(cedula.facturas[i].concepto[0].Cantidad) + "/";
                        totalNC += cedula.facturas[i].comprobante.Total;
                    }
                }
                else
                {
                    if (!cedula.facturas[i].comprobante.Serie.Equals("NC"))
                    {
                        strFacturas += cedula.facturas[i].comprobante.Serie + cedula.facturas[i].comprobante.Folio;
                        strCantidades += Convert.ToInt32(cedula.facturas[i].concepto[0].Cantidad);
                        strTimbrado += cedula.facturas[i].timbreFiscal.FechaTimbrado;
                        total += cedula.facturas[i].comprobante.Total;
                    }
                    else
                    {
                        strNotas += cedula.facturas[i].comprobante.Serie + cedula.facturas[i].comprobante.Folio;
                        strNTimbrado += cedula.facturas[i].timbreFiscal.FechaTimbrado;
                        strCantidadesNota += Convert.ToInt32(cedula.facturas[i].concepto[0].Cantidad);
                        totalNC += cedula.facturas[i].comprobante.Total;
                    }
                }
            }

            document.Replace("|ImporteIVA|", String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:C}", total), false, true);
            document.Replace("|CantidadServicios|", strCantidades + "", false, true);
            document.Replace("|FechaTimbrado|", strTimbrado, false, true);
            document.Replace("|FolioFactura|", strFacturas, false, true);

            //Notas de Crédito
            document.Replace("|ImporteNota|", String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:C}", totalNC), false, true);
            document.Replace("|CantidadNota|", strCantidadesNota.Equals("") ? "N/A" : strCantidadesNota + "", false, true);
            document.Replace("|TimbradoNota|", strNTimbrado.Equals("") ? "N/A" : strNTimbrado, false, true);
            document.Replace("|FolioNota|", strNotas.Equals("") ? "N/A" : strNotas, false, true);

            //Salvar y Lanzar

            byte[] toArray = null;
            using (MemoryStream ms1 = new MemoryStream())
            {
                document.SaveToStream(ms1, Spire.Doc.FileFormat.Docx2013);
                toArray = ms1.ToArray();
            }
            return File(toArray, "application/ms-word", "ActaER_Limpieza_" + cedula.Mes + ".docx");
        }

        [Route("/documents/actaFumigacion/{id?}")]
        public async Task<IActionResult> getActaEntregaRecepcionFumigacion(int id)
        {

            ActaEntregaRecepcion cedula = new ActaEntregaRecepcion();
            CedulaEvaluacion cedulaFumigacion = new CedulaEvaluacion();
            cedulaFumigacion = await vCedula.CedulaById(id);
            cedula = await vDocuments.getDatosActa(id, 2);
            cedula.facturas = new List<Facturas>();
            cedula.facturas = await vFacturas.getFacturas(id, 2);


            Document document = new Document();
            var path = @"E:\Plantillas CASESGV2\DocsV2\Acta ER\Acta Entrega - Recepción 2022 Fumigacion.docx";
            document.LoadFromFile(path);

            //Creamos la Tabla
            Section tablas = document.AddSection();

            document.Replace("|Periodo|", cedula.FechaInicio.Day + " de " + cedula.FechaInicio.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " + cedula.FechaInicio.Year + " al " +
                 cedula.FechaFin.Day + " de " + cedula.FechaFin.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " + cedula.FechaFin.Year, false, true);

            document.Replace("|Ciudad|", cedula.Estado, false, true);
            if (cedula.Estado.Equals("Ciudad de México"))
            {
                document.Replace("|Estado|", "en la " + cedula.Estado, false, true);
            }
            else
            {
                document.Replace("|Estado|", "en el " + cedula.Estado, false, true);
                document.Replace("|Estado|", "en " + cedula.Estado, false, true);
            }

            document.Replace("|InmuebleP|", cedula.InmuebleC, false, true);
            document.Replace("|DomicilioInmueble|", cedula.Direccion, false, true);
            document.Replace("|ResponsableInmueble|", cedula.Administrador, false, true);
            document.Replace("|Reviso|", cedula.Reviso, false, true);
            document.Replace("|Usuario|", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cedula.Elaboro.ToLower()), false, true);
            document.Replace("|InmuebleCedula|", CultureInfo.CurrentCulture.TextInfo.ToUpper(cedula.Inmueble), false, true);
            document.Replace("|Inmueble|", CultureInfo.CurrentCulture.TextInfo.ToUpper(cedula.InmuebleC) + "\n", false, true);

            document.Replace("|Folio|", cedula.Folio, false, true);

            document.Replace("|MesEval|", cedula.Mes, false, true);


            DateTime fechaActual = DateTime.Now;
            document.Replace("|Dia|", fechaActual.Day + "", false, true);
            document.Replace("|Mes|", fechaActual.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")), false, true);
            document.Replace("|Anio|", fechaActual.Year + "", false, true);
            document.Replace("|Hora|", fechaActual.Hour + ":00", false, true);
            document.Replace("|DiaActual|", fechaActual.Day + " de " + fechaActual.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " + fechaActual.Year, false, true);
            document.Replace("|HoraActual|", fechaActual.Hour + ":00", false, true);
            document.Replace("|Hora|", fechaActual.Hour + ":00", false, true);
            if ((await iFumigacion.GetIncidenciasPregunta(id, 2)).Count == 0 && (await iFumigacion.GetIncidenciasPregunta(id, 3)).Count == 0 && (await iFumigacion.GetIncidenciasPregunta(id, 4)).Count == 0)
            {
                document.Replace("|Declaraciones|", "Se hace constar que los servicios solicitados fueron atendidos a entera satisfacción del Consejo de la Judicatura Federal conforme se visualiza en la cédula automatizada para la supervisión y evaluación de servicios generales.", false, true);
            }
            else
            {
                document.Replace("|Declaraciones|", "\nSe hace constar que los servicios fueron recibidos por el Consejo de la Judicatura Federal, presentando incidencias, mismas que se vierten en la cédula automatizada para la supervisión y evaluación de servicios generales.", false, true);
            }

            string strFacturas = "";
            string strNotas = "";
            string strNTimbrado = "";
            string strCantidades = "";
            string strCantidadesNota = "";
            string strTimbrado = "";
            decimal total = 0, totalNC = 0;
            for (int i = 0; i < cedula.facturas.Count; i++)
            {
                if ((cedula.facturas.Count - 1) != i && i != 0)
                {
                    if (!cedula.facturas[i].comprobante.Serie.Equals("NCRE"))
                    {
                        strFacturas += cedula.facturas[i].comprobante.Serie + cedula.facturas[i].comprobante.Folio + "/";
                        strTimbrado += cedula.facturas[i].timbreFiscal.FechaTimbrado + "/";

                        total += cedula.facturas[i].comprobante.Total;
                    }
                    else
                    {
                        strNotas += cedula.facturas[i].comprobante.Serie + cedula.facturas[i].comprobante.Folio;
                        strNTimbrado += cedula.facturas[i].timbreFiscal.FechaTimbrado;
                        totalNC += cedula.facturas[i].comprobante.Total;
                    }
                }
                else
                {
                    if (!cedula.facturas[i].comprobante.Serie.Equals("NCRE"))
                    {
                        strFacturas += cedula.facturas[i].comprobante.Serie + cedula.facturas[i].comprobante.Folio;
                        strTimbrado += cedula.facturas[i].timbreFiscal.FechaTimbrado;
                        total += cedula.facturas[i].comprobante.Total;
                    }
                    else
                    {
                        strNotas += cedula.facturas[i].comprobante.Serie + cedula.facturas[i].comprobante.Folio;
                        strNTimbrado += cedula.facturas[i].timbreFiscal.FechaTimbrado;
                        totalNC += cedula.facturas[i].comprobante.Total;
                    }
                }
            }
            int cantidad = 0;
            string concepto = "";
            for (int i = 0; i < cedula.facturas.Count; i++)
            {
                for (int j = 0; j < cedula.facturas[i].concepto.Count; j++)
                {
                    if ((cedula.facturas[i].concepto.Count - 1) != j && j != 0)
                    {
                        if (!cedula.facturas[i].comprobante.Serie.Equals("NCRE"))
                        {
                            if (concepto.Equals(cedula.facturas[i].concepto[j].Descripcion))
                            {
                                cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                strCantidades = cantidad + " - " + cedula.facturas[i].concepto[j].Descripcion + "\n";
                            }
                            else
                            {
                                concepto = cedula.facturas[i].concepto[j].Descripcion;
                                cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                strCantidades += cantidad + " - " + concepto + "\n";
                            }
                        }
                        else
                        {
                            if (concepto.Equals(cedula.facturas[i].concepto[j].Descripcion))
                            {
                                cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                strCantidadesNota = cantidad + " - " + cedula.facturas[i].concepto[j].Descripcion + "\n";
                            }
                            else
                            {
                                concepto = cedula.facturas[i].concepto[j].Descripcion;
                                cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                strCantidadesNota += cantidad + " - " + concepto + "\n";
                            }
                        }
                    }
                    else
                    {
                        if (!cedula.facturas[i].comprobante.Serie.Equals("NCRE"))
                        {
                            if (concepto.Equals(cedula.facturas[i].concepto[j].Descripcion))
                            {
                                cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                strCantidades = cantidad + " - " + cedula.facturas[i].concepto[j].Descripcion + "\n";
                            }
                            else
                            {
                                concepto = cedula.facturas[i].concepto[j].Descripcion;
                                cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                strCantidades += cantidad + " - " + concepto + "\n";
                            }
                        }
                        else
                        {
                            if (concepto.Equals(cedula.facturas[i].concepto[j].Descripcion))
                            {
                                cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                strCantidadesNota = cantidad + " - " + cedula.facturas[i].concepto[j].Descripcion + "\n";
                            }
                            else
                            {
                                concepto = cedula.facturas[i].concepto[j].Descripcion;
                                cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                strCantidadesNota += cantidad + " - " + concepto + "\n";
                            }
                        }
                    }
                }
                concepto = "";
                cantidad = 0;
            }

            document.Replace("|ImporteIVA|", String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:C}", total), false, true);
            document.Replace("|CantidadServicios|", strCantidades + "", false, true);
            document.Replace("|FechaTimbrado|", strTimbrado, false, true);
            document.Replace("|FolioFactura|", strFacturas, false, true);

            //Notas de Crédito
            document.Replace("|ImporteNota|", String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:C}", totalNC), false, true);
            document.Replace("|CantidadNota|", strCantidadesNota.Equals("") ? "N/A" : strCantidadesNota + "", false, true);
            document.Replace("|TimbradoNota|", strNTimbrado.Equals("") ? "N/A" : strNTimbrado, false, true);
            document.Replace("|FolioNota|", strNotas.Equals("") ? "N/A" : strNotas, false, true);

            //Salvar y Lanzar

            byte[] toArray = null;
            using (MemoryStream ms1 = new MemoryStream())
            {
                document.SaveToStream(ms1, Spire.Doc.FileFormat.Docx2013);
                toArray = ms1.ToArray();
            }
            return File(toArray, "application/ms-word", "ActaER_Fumigacion_" + cedula.Mes + ".docx");
        }

        [Route("/documents/actaMensajeria/{id?}")]
        public async Task<IActionResult> getActaEntregaRecepcionMensajeria(int id)
        {

            ActaEntregaRecepcion cedula = new ActaEntregaRecepcion();
            cedula = await vDocuments.getDatosActa(id, 3);
            cedula.facturas = new List<Facturas>();
            cedula.facturas = await vFacturas.getFacturas(id, 3);


            Document document = new Document();
            var path = @"E:\Plantillas CASESGV2\DocsV2\Acta ER\Acta Entrega - Recepción 2022 Mensajeria.docx";
            document.LoadFromFile(path);

            Section tablas = document.AddSection();

            document.Replace("|Periodo|", cedula.FechaInicio.Day + " de " + cedula.FechaInicio.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " + cedula.FechaInicio.Year + " al " +
                 cedula.FechaFin.Day + " de " + cedula.FechaFin.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " + cedula.FechaFin.Year, false, true);

            document.Replace("|Ciudad|", cedula.Estado, false, true);
            if (cedula.Estado.Equals("Ciudad de México"))
            {
                document.Replace("|Estado|", "en la " + cedula.Estado, false, true);
                document.Replace("|Coordinacion|", "COORDINACIÓN DE CONTROL OPERATIVO DE ADMINISTRACIONES DE EDIFICIOS", false, true);
                document.Replace("|Inmueble|", CultureInfo.CurrentCulture.TextInfo.ToUpper(cedula.InmuebleC), false, true);
            }
            else
            {
                document.Replace("|Estado|", "en el " + cedula.Estado, false, true);
                document.Replace("|Coordinacion|", "COORDINACIÓN DE ADMINISTRACIÓN REGIONAL", false, true);
                document.Replace("|Inmueble|", CultureInfo.CurrentCulture.TextInfo.ToUpper(cedula.TipoInmueble) + " " + CultureInfo.CurrentCulture.TextInfo.ToUpper(cedula.Estado), false, true);
            }

            document.Replace("|InmuebleP|", cedula.InmuebleC, false, true);
            document.Replace("|DomicilioInmueble|", cedula.Direccion, false, true);
            document.Replace("|ResponsableInmueble|", cedula.Administrador, false, true);
            document.Replace("|Reviso|", cedula.Reviso, false, true);
            document.Replace("|Usuario|", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cedula.Elaboro.ToLower()), false, true);
            document.Replace("|InmuebleCedula|", CultureInfo.CurrentCulture.TextInfo.ToUpper(cedula.Inmueble), false, true);

            document.Replace("|Folio|", cedula.Folio, false, true);

            document.Replace("|MesEval|", cedula.Mes, false, true);


            DateTime fechaActual = DateTime.Now;
            document.Replace("|Dia|", fechaActual.Day + "", false, true);
            document.Replace("|Mes|", fechaActual.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")), false, true);
            document.Replace("|Anio|", fechaActual.Year + "", false, true);
            document.Replace("|Hora|", fechaActual.Hour + ":00", false, true);
            document.Replace("|DiaActual|", fechaActual.Day + " de " + fechaActual.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " + fechaActual.Year, false, true);
            document.Replace("|HoraActual|", fechaActual.Hour + ":00", false, true);
            document.Replace("|Hora|", fechaActual.Hour + ":00", false, true);
            if ((await iFumigacion.GetIncidenciasPregunta(id, 2)).Count == 0 && (await iFumigacion.GetIncidenciasPregunta(id, 3)).Count == 0 && (await iFumigacion.GetIncidenciasPregunta(id, 4)).Count == 0)
            {
                document.Replace("|Declaraciones|", "Se hace constar que los servicios solicitados fueron atendidos a entera satisfacción del Consejo de la Judicatura Federal conforme se visualiza en la cédula automatizada para la supervisión y evaluación de servicios generales.", false, true);
            }
            else
            {
                document.Replace("|Declaraciones|", "Se hace constar que los servicios fueron recibidos por el Consejo de la Judicatura Federal, presentando incidencias, mismas que se vierten en la cédula automatizada para la supervisión y evaluación de servicios generales.", false, true);
            }

            string strFacturas = "";
            string strNotas = "";
            string strNTimbrado = "";
            string strCantidades = "";
            string strCantidadesNota = "";
            string strTimbrado = "";
            decimal total = 0, totalNC = 0;
            for (int i = 0; i < cedula.facturas.Count; i++)
            {
                if ((cedula.facturas.Count - 1) != i && i != 0)
                {
                    if (!cedula.facturas[i].receptor.usoCFDI.Equals("G02"))
                    {
                        strFacturas += cedula.facturas[i].comprobante.Folio + "\n";
                        strTimbrado += cedula.facturas[i].timbreFiscal.FechaTimbrado.ToString("dd/MM/yyyy") + "\n";

                        total += cedula.facturas[i].comprobante.Total;
                    }
                    else
                    {
                        strNotas += cedula.facturas[i].comprobante.Folio + "\n";
                        strNTimbrado += cedula.facturas[i].timbreFiscal.FechaTimbrado.ToString("dd/MM/yyyy") + "\n";
                        totalNC += cedula.facturas[i].comprobante.Total;
                    }
                }
                else
                {
                    if (!cedula.facturas[i].receptor.usoCFDI.Equals("G02"))
                    {
                        strFacturas += cedula.facturas[i].comprobante.Folio + "\n";
                        strTimbrado += cedula.facturas[i].timbreFiscal.FechaTimbrado.ToString("dd/MM/yyyy") + "\n";
                        total += cedula.facturas[i].comprobante.Total;
                    }
                    else
                    {
                        strNotas += cedula.facturas[i].comprobante.Folio + "\n";
                        strNTimbrado += cedula.facturas[i].timbreFiscal.FechaTimbrado.ToString("dd/MM/yyyy") + "\n";
                        totalNC += cedula.facturas[i].comprobante.Total;
                    }
                }
            }
            int cantidad = 0;
            decimal cantidadS = 0;
            string concepto = "";
            string unidad = "";
            for (int i = 0; i < cedula.facturas.Count; i++)
            {
                for (int j = 0; j < cedula.facturas[i].concepto.Count; j++)
                {
                    unidad = cedula.facturas[i].concepto[j].Unidad.Equals("ACTIVIDAD") ? "(ES)" : "(S)";
                    if ((cedula.facturas[i].concepto.Count - 1) != j && j != 0)
                    {
                        if (!cedula.facturas[i].receptor.usoCFDI.Equals("G02"))
                        {
                            if (concepto.Equals(cedula.facturas[i].concepto[j].Descripcion))
                            {
                                if (cedula.facturas[i].concepto[j].Unidad.Equals("GUIA"))
                                {
                                    cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                    strCantidades = cantidad + " - " + cedula.facturas[i].concepto[j].Descripcion + "\n";
                                }
                                else
                                {
                                    cantidadS += Convert.ToDecimal(cedula.facturas[i].concepto[j].Cantidad);
                                    strCantidades = cantidadS + " " + cedula.facturas[i].concepto[j].Unidad + unidad + " - " + cedula.facturas[i].concepto[j].Descripcion + "\n";
                                }
                            }
                            else
                            {
                                concepto = cedula.facturas[i].concepto[j].Descripcion;
                                if (cedula.facturas[i].concepto[j].Unidad.Equals("GUIA"))
                                {
                                    cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                    strCantidades += cantidad + " - " + cedula.facturas[i].concepto[j].Descripcion + "\n";
                                }
                                else
                                {
                                    cantidadS += Convert.ToDecimal(cedula.facturas[i].concepto[j].Cantidad);
                                    strCantidades += cantidadS + " " + cedula.facturas[i].concepto[j].Unidad + unidad + " - " + cedula.facturas[i].concepto[j].Descripcion + "\n";
                                }
                            }
                        }
                        else
                        {
                            if (concepto.Equals(cedula.facturas[i].concepto[j].Descripcion))
                            {
                                if (cedula.facturas[i].concepto[j].Unidad.Equals("GUIA"))
                                {
                                    cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                    strCantidadesNota = cantidad + " - " + cedula.facturas[i].concepto[j].Descripcion + "\n";
                                }
                                else
                                {
                                    cantidadS += Convert.ToDecimal(cedula.facturas[i].concepto[j].Cantidad);
                                    strCantidadesNota = cantidadS + " " + cedula.facturas[i].concepto[j].Unidad + unidad + " - " + cedula.facturas[i].concepto[j].Descripcion + "\n";
                                }
                            }
                            else
                            {
                                concepto = cedula.facturas[i].concepto[j].Descripcion;
                                if (cedula.facturas[i].concepto[j].Unidad.Equals("GUIA"))
                                {
                                    cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                    strCantidadesNota += cantidad + " - " + cedula.facturas[i].concepto[j].Descripcion + "\n";
                                }
                                else
                                {
                                    cantidadS += Convert.ToDecimal(cedula.facturas[i].concepto[j].Cantidad);
                                    strCantidadesNota += cantidadS + " " + cedula.facturas[i].concepto[j].Unidad + unidad + " - " + cedula.facturas[i].concepto[j].Descripcion + "\n";
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!cedula.facturas[i].receptor.usoCFDI.Equals("G02"))
                        {
                            if (concepto.Equals(cedula.facturas[i].concepto[j].Descripcion))
                            {
                                if (cedula.facturas[i].concepto[j].Unidad.Equals("GUIA"))
                                {
                                    cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                    strCantidades = cantidad + " GUÍA(S) - " + cedula.facturas[i].concepto[j].Descripcion + "\n";
                                }
                                else
                                {
                                    cantidadS += Convert.ToDecimal(cedula.facturas[i].concepto[j].Cantidad);
                                    strCantidades = cantidadS + " " + cedula.facturas[i].concepto[j].Unidad + unidad + " - " + cedula.facturas[i].concepto[j].Descripcion + "\n";
                                }
                            }
                            else
                            {
                                concepto = cedula.facturas[i].concepto[j].Descripcion;
                                if (cedula.facturas[i].concepto[j].Unidad.Equals("GUIA"))
                                {
                                    cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                    strCantidades += cantidad + " GUÍA(S) - " + cedula.facturas[i].concepto[j].Descripcion + "\n";
                                }
                                else
                                {
                                    cantidadS += Convert.ToDecimal(cedula.facturas[i].concepto[j].Cantidad);
                                    strCantidades += cantidadS + " " + cedula.facturas[i].concepto[j].Unidad + unidad + " - " + cedula.facturas[i].concepto[j].Descripcion + "\n";
                                }
                            }
                        }
                        else
                        {
                            if (concepto.Equals(cedula.facturas[i].concepto[j].Descripcion))
                            {
                                if (cedula.facturas[i].concepto[j].Unidad.Equals("GUIA"))
                                {
                                    cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                    strCantidadesNota = cantidad + " GUÍA(S) - " + cedula.facturas[i].concepto[j].Descripcion + "\n";
                                }
                                else
                                {
                                    cantidadS += Convert.ToDecimal(cedula.facturas[i].concepto[j].Cantidad);
                                    strCantidadesNota = cantidadS + " " + cedula.facturas[i].concepto[j].Unidad + unidad + "  - " + cedula.facturas[i].concepto[j].Descripcion + "\n";
                                }
                            }
                            else
                            {
                                if (cedula.facturas[i].concepto[j].Unidad.Equals("GUIA"))
                                {
                                    cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                    strCantidadesNota = cantidad + " GUÍA(S) - " + cedula.facturas[i].concepto[j].Descripcion + "\n";
                                }
                                else
                                {
                                    cantidadS += Convert.ToDecimal(cedula.facturas[i].concepto[j].Cantidad);
                                    strCantidadesNota += cantidadS + " " + cedula.facturas[i].concepto[j].Unidad + unidad + " - " + cedula.facturas[i].concepto[j].Descripcion + "\n";
                                }
                            }
                        }
                    }
                }
                concepto = "";
                cantidad = 0;
                cantidadS = 0;
            }

            document.Replace("|ImporteIVA|", String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:C}", total), false, true);
            document.Replace("|CantidadServicios|", strCantidades + "", false, true);
            document.Replace("|FechaTimbrado|", strTimbrado, false, true);
            document.Replace("|FolioFactura|", strFacturas, false, true);

            document.Replace("|ImporteNota|", String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:C}", totalNC), false, true);
            document.Replace("|CantidadNota|", strCantidadesNota.Equals("") ? "N/A" : strCantidadesNota + "", false, true);
            document.Replace("|TimbradoNota|", strNTimbrado.Equals("") ? "N/A" : Convert.ToDateTime(strNTimbrado).ToString("dd/MM/yyyy"), false, true);
            document.Replace("|FolioNota|", strNotas.Equals("") ? "N/A" : strNotas, false, true);


            byte[] toArray = null;
            using (MemoryStream ms1 = new MemoryStream())
            {
                document.SaveToStream(ms1, Spire.Doc.FileFormat.Docx2013);
                toArray = ms1.ToArray();
            }
            return File(toArray, "application/ms-word", "ActaER_Mensajeria_" + cedula.Mes + ".docx");
        }

        [Route("/documents/actaAgua/{id?}")]
        public async Task<IActionResult> getActaEntregaRecepcionAgua(int id)
        {

            ActaEntregaRecepcion cedula = new ActaEntregaRecepcion();
            CedulaAgua cedulaAgua = new CedulaAgua();
            cedulaAgua = await vAgua.CedulaById(id);
            cedula = await vDocuments.getDatosActa(id, 9);
            cedula.facturas = new List<Facturas>();
            cedula.facturas = await vFacturas.getFacturas(id, 9);


            Document document = new Document();
            var path = @"E:\Plantillas CASESGV2\DocsV2\Acta ER\Acta Entrega - Recepción 2022 Agua.docx";
            document.LoadFromFile(path);

            //Creamos la Tabla
            Section tablas = document.AddSection();

            document.Replace("|Periodo|", cedula.FechaInicio.Day + " de " + cedula.FechaInicio.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " + cedula.FechaInicio.Year + " al " +
                 cedula.FechaFin.Day + " de " + cedula.FechaFin.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " + cedula.FechaFin.Year, false, true);

            document.Replace("|Ciudad|", cedula.Estado, false, true);
            if (cedula.Estado.Equals("Ciudad de México"))
            {
                document.Replace("|Estado|", "en la " + cedula.Estado, false, true);
            }
            else
            {
                document.Replace("|Estado|", "en el " + cedula.Estado, false, true);
                document.Replace("|Estado|", "en " + cedula.Estado, false, true);
            }

            document.Replace("|InmuebleP|", cedula.InmuebleC, false, true);
            document.Replace("|DomicilioInmueble|", cedula.Direccion, false, true);
            document.Replace("|ResponsableInmueble|", cedula.Administrador, false, true);
            document.Replace("|Reviso|", cedula.Reviso, false, true);
            document.Replace("|Usuario|", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cedula.Elaboro.ToLower()), false, true);
            document.Replace("|InmuebleCedula|", CultureInfo.CurrentCulture.TextInfo.ToUpper(cedula.Inmueble), false, true);
            document.Replace("|Inmueble|", CultureInfo.CurrentCulture.TextInfo.ToUpper(cedula.InmuebleC) + "\n", false, true);

            document.Replace("|Folio|", cedula.Folio, false, true);

            document.Replace("|MesEval|", cedula.Mes, false, true);


            DateTime fechaActual = DateTime.Now;
            document.Replace("|Dia|", fechaActual.Day + "", false, true);
            document.Replace("|Mes|", fechaActual.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")), false, true);
            document.Replace("|Anio|", fechaActual.Year + "", false, true);
            document.Replace("|Hora|", fechaActual.Hour + ":00", false, true);
            document.Replace("|DiaActual|", fechaActual.Day + " de " + fechaActual.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " + fechaActual.Year, false, true);
            document.Replace("|HoraActual|", fechaActual.Hour + ":00", false, true);
            document.Replace("|Hora|", fechaActual.Hour + ":00", false, true);
            if ((await iAgua.GetIncidenciasPregunta(id, 2)).Count == 0 && (await iAgua.GetIncidenciasPregunta(id, 3)).Count == 0 && (await iAgua.GetIncidenciasPregunta(id, 4)).Count == 0)
            {
                document.Replace("|Declaraciones|", "Se hace constar que los servicios solicitados fueron atendidos a entera satisfacción del Consejo de la Judicatura Federal conforme se visualiza en la cédula automatizada para la supervisión y evaluación de servicios generales.", false, true);
            }
            else
            {
                document.Replace("|Declaraciones|", "\nSe hace constar que los servicios fueron recibidos por el Consejo de la Judicatura Federal, presentando incidencias, mismas que se vierten en la cédula automatizada para la supervisión y evaluación de servicios generales.", false, true);
            }

            string strFacturas = "";
            string strNotas = "";
            string strNTimbrado = "";
            string strCantidades = "";
            string strCantidadesNota = "";
            string strTimbrado = "";
            string total = "", totalNC = "";
            for (int i = 0; i < cedula.facturas.Count; i++)
            {
                if ((cedula.facturas.Count - 1) != i && i != 0)
                {
                    if (!cedula.facturas[i].comprobante.Serie.Equals("NCRE"))
                    {
                        strFacturas += (i + 1) + ".- " + cedula.facturas[i].comprobante.Serie + cedula.facturas[i].comprobante.Folio + "\n";
                        strTimbrado += (i + 1) + ".- " + cedula.facturas[i].timbreFiscal.FechaTimbrado.ToString("dd/MM/yyyy") + "\n";

                        total += (i + 1) + ".- " + String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:C}", cedula.facturas[i].comprobante.Total) + "\n";
                    }
                    else
                    {
                        strNotas += (i + 1) + ".- " + cedula.facturas[i].comprobante.Serie + cedula.facturas[i].comprobante.Folio + "\n";
                        strNTimbrado += (i + 1) + ".- " + cedula.facturas[i].timbreFiscal.FechaTimbrado.ToString("dd/MM/yyyy") + "\n";
                        totalNC += (i + 1) + ".- " + String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:C}", cedula.facturas[i].comprobante.Total) + "\n";
                    }
                }
                else
                {
                    if (!cedula.facturas[i].comprobante.Serie.Equals("NCRE"))
                    {
                        strFacturas += (i + 1) + ".- " + cedula.facturas[i].comprobante.Serie + cedula.facturas[i].comprobante.Folio + "\n";
                        strTimbrado += (i + 1) + ".- " + cedula.facturas[i].timbreFiscal.FechaTimbrado.ToString("dd/MM/yyyy") + "\n";
                        total += (i + 1) + ".- " + String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:C}", cedula.facturas[i].comprobante.Total) + "\n";
                    }
                    else
                    {
                        strNotas += (i + 1) + ".- " + cedula.facturas[i].comprobante.Serie + cedula.facturas[i].comprobante.Folio + "\n";
                        strNTimbrado += (i + 1) + ".- " + cedula.facturas[i].timbreFiscal.FechaTimbrado.ToString("dd/MM/yyyy") + "\n";
                        totalNC += (i + 1) + ".- " + String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:C}", cedula.facturas[i].comprobante.Total) + "\n";
                    }
                }
            }
            int cantidad = 0;
            string concepto = "";
            for (int i = 0; i < cedula.facturas.Count; i++)
            {
                for (int j = 0; j < cedula.facturas[i].concepto.Count; j++)
                {
                    if ((cedula.facturas[i].concepto.Count - 1) != j && j != 0)
                    {
                        if (!cedula.facturas[i].comprobante.Serie.Equals("NCRE"))
                        {
                            if (concepto.Equals(cedula.facturas[i].concepto[j].Descripcion))
                            {
                                cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                strCantidades = (i + 1) + ".- " + cantidad + " - " + cedula.facturas[i].concepto[j].Descripcion + "\n";
                            }
                            else
                            {
                                concepto = cedula.facturas[i].concepto[j].Descripcion;
                                cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                strCantidades += (i + 1) + ".- " + cantidad + " - " + concepto + "\n";
                            }
                        }
                        else
                        {
                            if (concepto.Equals(cedula.facturas[i].concepto[j].Descripcion))
                            {
                                cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                strCantidadesNota = (i + 1) + ".- " + cantidad + " - " + cedula.facturas[i].concepto[j].Descripcion + "\n";
                            }
                            else
                            {
                                concepto = cedula.facturas[i].concepto[j].Descripcion;
                                cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                strCantidadesNota += (i + 1) + ".- " + cantidad + " - " + concepto + "\n";
                            }
                        }
                    }
                    else
                    {
                        if (!cedula.facturas[i].comprobante.Serie.Equals("NCRE"))
                        {
                            if (concepto.Equals(cedula.facturas[i].concepto[j].Descripcion))
                            {
                                cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                strCantidades = (i + 1) + ".- " + cantidad + " - " + cedula.facturas[i].concepto[j].Descripcion + "\n";
                            }
                            else
                            {
                                concepto = cedula.facturas[i].concepto[j].Descripcion;
                                cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                strCantidades += (i + 1) + ".- " + cantidad + " - " + concepto + "\n";
                            }
                        }
                        else
                        {
                            if (concepto.Equals(cedula.facturas[i].concepto[j].Descripcion))
                            {
                                cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                strCantidadesNota = (i + 1) + ".- " + cantidad + " - " + cedula.facturas[i].concepto[j].Descripcion + "\n";
                            }
                            else
                            {
                                concepto = cedula.facturas[i].concepto[j].Descripcion;
                                cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                strCantidadesNota += (i + 1) + ".- " + cantidad + " - " + concepto + "\n";
                            }
                        }
                    }
                }
                concepto = "";
                cantidad = 0;
            }

            document.Replace("|ImporteIVA|", total, false, true);
            document.Replace("|CantidadServicios|", strCantidades + "", false, true);
            document.Replace("|FechaTimbrado|", strTimbrado, false, true);
            document.Replace("|FolioFactura|", strFacturas, false, true);

            //Notas de Crédito
            document.Replace("|ImporteNota|", String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:C}", totalNC), false, true);
            document.Replace("|CantidadNota|", strCantidadesNota.Equals("") ? "N/A" : strCantidadesNota + "", false, true);
            document.Replace("|TimbradoNota|", strNTimbrado.Equals("") ? "N/A" : strNTimbrado, false, true);
            document.Replace("|FolioNota|", strNotas.Equals("") ? "N/A" : strNotas, false, true);

            //Salvar y Lanzar

            byte[] toArray = null;
            using (MemoryStream ms1 = new MemoryStream())
            {
                document.SaveToStream(ms1, Spire.Doc.FileFormat.Docx2013);
                toArray = ms1.ToArray();
            }
            return File(toArray, "application/ms-word", "ActaER_Agua_" + cedula.Mes + ".docx");
        }

        [Route("/documents/actaRPBI/{id?}")]
        public async Task<IActionResult> getActaEntregaRecepcionRPBI(int id)
        {

            ActaEntregaRecepcion cedula = new ActaEntregaRecepcion();
            CedulaEvaluacion cedulaResiduos = new CedulaEvaluacion();
            cedulaResiduos = await vCedula.CedulaById(id);
            cedulaResiduos.usuarios = await vUsuarios.getUserById(cedulaResiduos.UsuarioId);
            cedula = await vDocuments.getDatosActa(id, 7);
            cedula.facturas = new List<Facturas>();
            cedula.facturas = await vFacturas.getFacturas(id, 7);


            Document document = new Document();
            var path = @"E:\Plantillas CASESGV2\DocsV2\Acta ER\Acta Entrega - Recepción 2022 RPBI.docx";
            document.LoadFromFile(path);

            //Creamos la Tabla
            Section tablas = document.AddSection();

            document.Replace("|Periodo|", cedula.FechaInicio.Day + " de " + cedula.FechaInicio.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " + cedula.FechaInicio.Year + " al " +
                 cedula.FechaFin.Day + " de " + cedula.FechaFin.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " + cedula.FechaFin.Year, false, true);

            document.Replace("|Ciudad|", cedula.Estado, false, true);
            if (cedula.Estado.Equals("Ciudad de México"))
            {
                document.Replace("|Estado|", "en la " + cedula.Estado, false, true);
            }
            else
            {
                document.Replace("|Estado|", "en el " + cedula.Estado, false, true);
                document.Replace("|Estado|", "en " + cedula.Estado, false, true);
            }

            document.Replace("|InmuebleP|", cedula.Inmueble, false, true);
            document.Replace("|DomicilioInmueble|", cedula.Direccion, false, true);
            document.Replace("|ResponsableInmueble|", cedula.Administrador, false, true);
            document.Replace("|Reviso|", cedula.Reviso, false, true);
            document.Replace("|Usuario|", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cedula.Elaboro.ToLower()), false, true);
            document.Replace("|InmuebleCedula|", CultureInfo.CurrentCulture.TextInfo.ToUpper(cedula.Inmueble), false, true);
            document.Replace("|Inmueble|", CultureInfo.CurrentCulture.TextInfo.ToUpper(cedula.InmuebleC) + "\n", false, true);

            document.Replace("|Folio|", cedula.Folio, false, true);

            document.Replace("|MesEval|", cedula.Mes, false, true);

            document.Replace("|Medico|", cedulaResiduos.usuarios.nombre_emp + " " + cedulaResiduos.usuarios.paterno_emp + " " +
                cedulaResiduos.usuarios.materno_emp, false, true);


            DateTime fechaActual = DateTime.Now;
            document.Replace("|Dia|", fechaActual.Day + "", false, true);
            document.Replace("|Mes|", fechaActual.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")), false, true);
            document.Replace("|Anio|", fechaActual.Year + "", false, true);
            document.Replace("|Hora|", fechaActual.Hour + ":00", false, true);
            document.Replace("|DiaActual|", fechaActual.Day + " de " + fechaActual.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " + fechaActual.Year, false, true);
            document.Replace("|HoraActual|", fechaActual.Hour + ":00", false, true);
            document.Replace("|Hora|", fechaActual.Hour + ":00", false, true);
            if ((await iAgua.GetIncidenciasPregunta(id, 2)).Count == 0 && (await iAgua.GetIncidenciasPregunta(id, 3)).Count == 0 && (await iAgua.GetIncidenciasPregunta(id, 4)).Count == 0)
            {
                document.Replace("|Declaraciones|", "Se hace constar que los servicios solicitados fueron atendidos a entera satisfacción del Consejo de la Judicatura Federal conforme se visualiza en la cédula automatizada para la supervisión y evaluación de servicios generales.", false, true);
            }
            else
            {
                document.Replace("|Declaraciones|", "\nSe hace constar que los servicios fueron recibidos por el Consejo de la Judicatura Federal, presentando incidencias, mismas que se vierten en la cédula automatizada para la supervisión y evaluación de servicios generales.", false, true);
            }

            string strFacturas = "";
            string strNotas = "";
            string strNTimbrado = "";
            string strCantidades = "";
            string strCantidadesNota = "";
            string strTimbrado = "";
            string total = "", totalNC = "";
            for (int i = 0; i < cedula.facturas.Count; i++)
            {
                if ((cedula.facturas.Count - 1) != i && i != 0)
                {
                    if (!cedula.facturas[i].comprobante.Serie.Equals("NCRE"))
                    {
                        strFacturas += (i + 1) + ".- " + cedula.facturas[i].comprobante.Serie + cedula.facturas[i].comprobante.Folio + "\n";
                        strTimbrado += (i + 1) + ".- " + cedula.facturas[i].timbreFiscal.FechaTimbrado.ToString("dd/MM/yyyy") + "\n";

                        total += (i + 1) + ".- " + String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:C}", cedula.facturas[i].comprobante.Total) + "\n";
                    }
                    else
                    {
                        strNotas += (i + 1) + ".- " + cedula.facturas[i].comprobante.Serie + cedula.facturas[i].comprobante.Folio + "\n";
                        strNTimbrado += (i + 1) + ".- " + cedula.facturas[i].timbreFiscal.FechaTimbrado.ToString("dd/MM/yyyy") + "\n";
                        totalNC += (i + 1) + ".- " + String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:C}", cedula.facturas[i].comprobante.Total) + "\n";
                    }
                }
                else
                {
                    if (!cedula.facturas[i].comprobante.Serie.Equals("NCRE"))
                    {
                        strFacturas += (i + 1) + ".- " + cedula.facturas[i].comprobante.Serie + cedula.facturas[i].comprobante.Folio + "\n";
                        strTimbrado += (i + 1) + ".- " + cedula.facturas[i].timbreFiscal.FechaTimbrado.ToString("dd/MM/yyyy") + "\n";
                        total += (i + 1) + ".- " + String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:C}", cedula.facturas[i].comprobante.Total) + "\n";
                    }
                    else
                    {
                        strNotas += (i + 1) + ".- " + cedula.facturas[i].comprobante.Serie + cedula.facturas[i].comprobante.Folio + "\n";
                        strNTimbrado += (i + 1) + ".- " + cedula.facturas[i].timbreFiscal.FechaTimbrado.ToString("dd/MM/yyyy") + "\n";
                        totalNC += (i + 1) + ".- " + String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:C}", cedula.facturas[i].comprobante.Total) + "\n";
                    }
                }
            }
            int cantidad = 0;
            string concepto = "";
            for (int i = 0; i < cedula.facturas.Count; i++)
            {
                for (int j = 0; j < cedula.facturas[i].concepto.Count; j++)
                {
                    if ((cedula.facturas[i].concepto.Count - 1) != j && j != 0)
                    {
                        if (!cedula.facturas[i].comprobante.Serie.Equals("NCRE"))
                        {
                            if (concepto.Equals(cedula.facturas[i].concepto[j].Descripcion))
                            {
                                cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                strCantidades = (i + 1) + ".- " + cantidad + " - " + cedula.facturas[i].concepto[j].Descripcion + "\n";
                            }
                            else
                            {
                                concepto = cedula.facturas[i].concepto[j].Descripcion;
                                cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                strCantidades += (i + 1) + ".- " + cantidad + " - " + concepto + "\n";
                            }
                        }
                        else
                        {
                            if (concepto.Equals(cedula.facturas[i].concepto[j].Descripcion))
                            {
                                cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                strCantidadesNota = (i + 1) + ".- " + cantidad + " - " + cedula.facturas[i].concepto[j].Descripcion + "\n";
                            }
                            else
                            {
                                concepto = cedula.facturas[i].concepto[j].Descripcion;
                                cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                strCantidadesNota += (i + 1) + ".- " + cantidad + " - " + concepto + "\n";
                            }
                        }
                    }
                    else
                    {
                        if (!cedula.facturas[i].comprobante.Serie.Equals("NCRE"))
                        {
                            if (concepto.Equals(cedula.facturas[i].concepto[j].Descripcion))
                            {
                                cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                strCantidades = (i + 1) + ".- " + cantidad + " - " + cedula.facturas[i].concepto[j].Descripcion + "\n";
                            }
                            else
                            {
                                concepto = cedula.facturas[i].concepto[j].Descripcion;
                                cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                strCantidades += (i + 1) + ".- " + cantidad + " - " + concepto + "\n";
                            }
                        }
                        else
                        {
                            if (concepto.Equals(cedula.facturas[i].concepto[j].Descripcion))
                            {
                                cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                strCantidadesNota = (i + 1) + ".- " + cantidad + " - " + cedula.facturas[i].concepto[j].Descripcion + "\n";
                            }
                            else
                            {
                                concepto = cedula.facturas[i].concepto[j].Descripcion;
                                cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                strCantidadesNota += (i + 1) + ".- " + cantidad + " - " + concepto + "\n";
                            }
                        }
                    }
                }
                concepto = "";
                cantidad = 0;
            }

            document.Replace("|ImporteIVA|", total, false, true);
            document.Replace("|CantidadServicios|", strCantidades + "", false, true);
            document.Replace("|FechaTimbrado|", strTimbrado, false, true);
            document.Replace("|FolioFactura|", strFacturas, false, true);

            //Notas de Crédito
            document.Replace("|ImporteNota|", String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:C}", totalNC), false, true);
            document.Replace("|CantidadNota|", strCantidadesNota.Equals("") ? "N/A" : strCantidadesNota + "", false, true);
            document.Replace("|TimbradoNota|", strNTimbrado.Equals("") ? "N/A" : strNTimbrado, false, true);
            document.Replace("|FolioNota|", strNotas.Equals("") ? "N/A" : strNotas, false, true);

            //Salvar y Lanzar

            byte[] toArray = null;
            using (MemoryStream ms1 = new MemoryStream())
            {
                document.SaveToStream(ms1, Spire.Doc.FileFormat.Docx2013);
                toArray = ms1.ToArray();
            }
            return File(toArray, "application/ms-word", "ActaER_RPBI_" + cedula.Mes + ".docx");
        }

        [Route("/documents/actaAnalisis/{id?}")]
        public async Task<IActionResult> getActaEntregaRecepcionAnalisis(int id)
        {

            ActaEntregaRecepcion cedula = new ActaEntregaRecepcion();
            CedulaEvaluacion cedulaResiduos = new CedulaEvaluacion();
            cedulaResiduos = await vCedula.CedulaById(id);
            cedulaResiduos.usuarios = await vUsuarios.getUserById(cedulaResiduos.UsuarioId);
            cedula = await vDocuments.getDatosActa(id, 8);
            cedula.facturas = new List<Facturas>();
            cedula.facturas = await vFacturas.getFacturas(id, 8);


            Document document = new Document();
            var path = @"E:\Plantillas CASESGV2\DocsV2\Acta ER\Acta Entrega - Recepción 2022 Analisis.docx";
            document.LoadFromFile(path);

            //Creamos la Tabla
            Section tablas = document.AddSection();

            document.Replace("|Periodo|", cedula.FechaInicio.Day + " de " + cedula.FechaInicio.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " + cedula.FechaInicio.Year + " al " +
                 cedula.FechaFin.Day + " de " + cedula.FechaFin.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " + cedula.FechaFin.Year, false, true);

            document.Replace("|Ciudad|", cedula.Estado, false, true);
            if (cedula.Estado.Equals("Ciudad de México"))
            {
                document.Replace("|Estado|", "en la " + cedula.Estado, false, true);
            }
            else
            {
                document.Replace("|Estado|", "en el " + cedula.Estado, false, true);
                document.Replace("|Estado|", "en " + cedula.Estado, false, true);
            }

            document.Replace("|InmuebleP|", cedula.Inmueble, false, true);
            document.Replace("|DomicilioInmueble|", cedula.Direccion, false, true);
            document.Replace("|ResponsableInmueble|", cedula.Administrador, false, true);
            document.Replace("|Reviso|", cedula.Reviso, false, true);
            document.Replace("|ResponsableCENDI|", CultureInfo.CurrentCulture.TextInfo.ToTitleCase(cedula.Elaboro.ToLower()), false, true);
            document.Replace("|InmuebleCedula|", CultureInfo.CurrentCulture.TextInfo.ToUpper(cedula.Inmueble), false, true);
            document.Replace("|Inmueble|", CultureInfo.CurrentCulture.TextInfo.ToUpper(cedula.InmuebleC) + "\n", false, true);

            document.Replace("|Folio|", cedula.Folio, false, true);

            document.Replace("|MesEval|", cedula.Mes, false, true);

            document.Replace("|Medico|", cedulaResiduos.usuarios.nombre_emp + " " + cedulaResiduos.usuarios.paterno_emp + " " +
                cedulaResiduos.usuarios.materno_emp, false, true);


            DateTime fechaActual = DateTime.Now;
            document.Replace("|Dia|", fechaActual.Day + "", false, true);
            document.Replace("|Mes|", fechaActual.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")), false, true);
            document.Replace("|Anio|", fechaActual.Year + "", false, true);
            document.Replace("|Hora|", fechaActual.Hour + ":00", false, true);
            document.Replace("|DiaActual|", fechaActual.Day + " de " + fechaActual.ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " + fechaActual.Year, false, true);
            document.Replace("|HoraActual|", fechaActual.Hour + ":00", false, true);
            document.Replace("|Hora|", fechaActual.Hour + ":00", false, true);
            if ((await iAgua.GetIncidenciasPregunta(id, 2)).Count == 0 && (await iAgua.GetIncidenciasPregunta(id, 3)).Count == 0 && (await iAgua.GetIncidenciasPregunta(id, 4)).Count == 0)
            {
                document.Replace("|Declaraciones|", "Se hace constar que los servicios solicitados fueron atendidos a entera satisfacción del Consejo de la Judicatura Federal conforme se visualiza en la cédula automatizada para la supervisión y evaluación de servicios generales.", false, true);
            }
            else
            {
                document.Replace("|Declaraciones|", "\nSe hace constar que los servicios fueron recibidos por el Consejo de la Judicatura Federal, presentando incidencias, mismas que se vierten en la cédula automatizada para la supervisión y evaluación de servicios generales.", false, true);
            }

            string strFacturas = "";
            string strNotas = "";
            string strNTimbrado = "";
            string strCantidades = "";
            string strCantidadesNota = "";
            string strTimbrado = "";
            string total = "", totalNC = "";
            for (int i = 0; i < cedula.facturas.Count; i++)
            {
                if ((cedula.facturas.Count - 1) != i && i != 0)
                {
                    if (!cedula.facturas[i].comprobante.Serie.Equals("NCRE"))
                    {
                        strFacturas += (i + 1) + ".- " + cedula.facturas[i].comprobante.Serie + cedula.facturas[i].comprobante.Folio + "\n";
                        strTimbrado += (i + 1) + ".- " + cedula.facturas[i].timbreFiscal.FechaTimbrado.ToString("dd/MM/yyyy") + "\n";

                        total += (i + 1) + ".- " + String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:C}", cedula.facturas[i].comprobante.Total) + "\n";
                    }
                    else
                    {
                        strNotas += (i + 1) + ".- " + cedula.facturas[i].comprobante.Serie + cedula.facturas[i].comprobante.Folio + "\n";
                        strNTimbrado += (i + 1) + ".- " + cedula.facturas[i].timbreFiscal.FechaTimbrado.ToString("dd/MM/yyyy") + "\n";
                        totalNC += (i + 1) + ".- " + String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:C}", cedula.facturas[i].comprobante.Total) + "\n";
                    }
                }
                else
                {
                    if (!cedula.facturas[i].comprobante.Serie.Equals("NCRE"))
                    {
                        strFacturas += (i + 1) + ".- " + cedula.facturas[i].comprobante.Serie + cedula.facturas[i].comprobante.Folio + "\n";
                        strTimbrado += (i + 1) + ".- " + cedula.facturas[i].timbreFiscal.FechaTimbrado.ToString("dd/MM/yyyy") + "\n";
                        total += (i + 1) + ".- " + String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:C}", cedula.facturas[i].comprobante.Total) + "\n";
                    }
                    else
                    {
                        strNotas += (i + 1) + ".- " + cedula.facturas[i].comprobante.Serie + cedula.facturas[i].comprobante.Folio + "\n";
                        strNTimbrado += (i + 1) + ".- " + cedula.facturas[i].timbreFiscal.FechaTimbrado.ToString("dd/MM/yyyy") + "\n";
                        totalNC += (i + 1) + ".- " + String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:C}", cedula.facturas[i].comprobante.Total) + "\n";
                    }
                }
            }
            int cantidad = 0;
            string concepto = "";
            for (int i = 0; i < cedula.facturas.Count; i++)
            {
                for (int j = 0; j < cedula.facturas[i].concepto.Count; j++)
                {
                    if ((cedula.facturas[i].concepto.Count - 1) != j && j != 0)
                    {
                        if (!cedula.facturas[i].comprobante.Serie.Equals("NCRE"))
                        {
                            if (concepto.Equals(cedula.facturas[i].concepto[j].Descripcion))
                            {
                                cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                strCantidades = (i + 1) + ".- " + cantidad + " - " + cedula.facturas[i].concepto[j].Descripcion + "\n";
                            }
                            else
                            {
                                concepto = cedula.facturas[i].concepto[j].Descripcion;
                                cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                strCantidades += (i + 1) + ".- " + cantidad + " - " + concepto + "\n";
                            }
                        }
                        else
                        {
                            if (concepto.Equals(cedula.facturas[i].concepto[j].Descripcion))
                            {
                                cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                strCantidadesNota = (i + 1) + ".- " + cantidad + " - " + cedula.facturas[i].concepto[j].Descripcion + "\n";
                            }
                            else
                            {
                                concepto = cedula.facturas[i].concepto[j].Descripcion;
                                cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                strCantidadesNota += (i + 1) + ".- " + cantidad + " - " + concepto + "\n";
                            }
                        }
                    }
                    else
                    {
                        if (!cedula.facturas[i].comprobante.Serie.Equals("NCRE"))
                        {
                            if (concepto.Equals(cedula.facturas[i].concepto[j].Descripcion))
                            {
                                cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                strCantidades = (i + 1) + ".- " + cantidad + " - " + cedula.facturas[i].concepto[j].Descripcion + "\n";
                            }
                            else
                            {
                                concepto = cedula.facturas[i].concepto[j].Descripcion;
                                cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                strCantidades += (i + 1) + ".- " + cantidad + " - " + concepto + "\n";
                            }
                        }
                        else
                        {
                            if (concepto.Equals(cedula.facturas[i].concepto[j].Descripcion))
                            {
                                cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                strCantidadesNota = (i + 1) + ".- " + cantidad + " - " + cedula.facturas[i].concepto[j].Descripcion + "\n";
                            }
                            else
                            {
                                concepto = cedula.facturas[i].concepto[j].Descripcion;
                                cantidad += Convert.ToInt32(cedula.facturas[i].concepto[j].Cantidad);
                                strCantidadesNota += (i + 1) + ".- " + cantidad + " - " + concepto + "\n";
                            }
                        }
                    }
                }
                concepto = "";
                cantidad = 0;
            }

            document.Replace("|ImporteIVA|", total, false, true);
            document.Replace("|CantidadServicios|", strCantidades + "", false, true);
            document.Replace("|FechaTimbrado|", strTimbrado, false, true);
            document.Replace("|FolioFactura|", strFacturas, false, true);

            //Notas de Crédito
            document.Replace("|ImporteNota|", String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:C}", totalNC), false, true);
            document.Replace("|CantidadNota|", strCantidadesNota.Equals("") ? "N/A" : strCantidadesNota + "", false, true);
            document.Replace("|TimbradoNota|", strNTimbrado.Equals("") ? "N/A" : strNTimbrado, false, true);
            document.Replace("|FolioNota|", strNotas.Equals("") ? "N/A" : strNotas, false, true);

            //Salvar y Lanzar

            byte[] toArray = null;
            using (MemoryStream ms1 = new MemoryStream())
            {
                document.SaveToStream(ms1, Spire.Doc.FileFormat.Docx2013);
                toArray = ms1.ToArray();
            }
            return File(toArray, "application/ms-word", "ActaER_Analisis_" + cedula.Mes + ".docx");
        }

        private int UserId()
        {
            return Convert.ToInt32(User.Claims.ElementAt(0).Value);
        }

        private string moduloLimpieza()
        {
            return "Limpieza";
        }
        private string moduloMensajeria()
        {
            return "Mensajería";
        }

        private string moduloTransporte()
        {
            return "Transporte_de_Personal";
        }

        private string moduloTraslado()
        {
            return "Traslado_de_Expedientes";
        }

        private string moduloMuebles()
        {
            return "Bienes_Muebles";
        }

        private string moduloAnalisis()
        {
            return "Análisis_Microbiológicos";
        }

        private string moduloRPBI()
        {
            return "RPBI";
        }

        private string moduloFumigacion()
        {
            return "Fumigación";
        }

        private string moduloAgua()
        {
            return "Agua_Para_Beber";
        }

        private string moduloTelefoniaCelular()
        {
            return "Telefonia_Celular";
        }
        private string moduloTelefoniaConvencional()
        {
            return "Telefonia_Convencional";
        }

    }
}
