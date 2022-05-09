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
        private readonly IRepositorioEntregablesMensajeria veMensajeria;

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

        [Route("/reporte/fumigacion/{id}")]
        public async Task<IActionResult> CedulaFumigacion(int id)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), moduloFumigacion(), "preliminar");
            if (success == 1)
            {
                string strFacturas = "";
                decimal totalFacturas = 0;

                CedulaEvaluacion cedula = new CedulaEvaluacion();
                cedula = await vCedula.CedulaById(id);
                cedula.inmuebles = await vInmuebles.inmuebleById(cedula.InmuebleId);
                cedula.usuarios = await vUsuarios.getUserById(cedula.UsuarioId);
                cedula.iEntregables = await vEntregables.getEntregables(id);
                cedula.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedula.RespuestasEncuesta = await vCedula.obtieneRespuestas(id);
                cedula.facturas = new List<Facturas>();
                cedula.facturas = await vFacturas.getFacturas(id, 2);
                cedula.incidencias = new Entities.MIncidencias.ModelsIncidencias();

                Document document = new Document();
                var path = @"E:\Plantillas CASESGV2\DocsV2\ReporteFumigacion.docx";
                document.LoadFromFile(path);

                //Creamos la Tabla
                Section tablas = document.AddSection();

                if (!cedula.RespuestasEncuesta[0].Detalles.Equals(null))
                {
                    document.Replace("||FechaCierre||", "El cierre de mes se efectuó el " + Convert.ToDateTime(cedula.RespuestasEncuesta[0].Detalles).ToString("dd") + " de " +
                        Convert.ToDateTime(cedula.RespuestasEncuesta[0].Detalles.Split("|")[0]).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " +
                        Convert.ToDateTime(cedula.RespuestasEncuesta[0].Detalles).ToString("yyyy") + ".", false, true);
                }

                cedula.incidencias.fumigacion = await iFumigacion.GetIncidenciasPregunta(id, 2);
                //obtenemos el documento con marcas
                if (cedula.incidencias.fumigacion.Count > 0)
                {
                    Table tablaActividades = tablas.AddTable(true);

                    String[] cabeceraFechas = { "Tipo", "Fecha Programada", "Fecha Realizada", "Comentarios" };

                    tablaActividades.ResetCells(cedula.incidencias.fumigacion.Count + 1, cabeceraFechas.Length);

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

                    for (int r = 0; r < cedula.incidencias.fumigacion.Count; r++)
                    {
                        if (cedula.incidencias.fumigacion[r].Pregunta.Equals(2 + ""))
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
                                    TR2 = p2.AppendText(cedula.incidencias.fumigacion[r].Tipo);
                                }
                                if (c == 1)
                                {
                                    TR2 = p2.AppendText(cedula.incidencias.fumigacion[r].FechaProgramada.ToString("dd/MM/yyyy"));
                                }
                                if (c == 2)
                                {
                                    TR2 = p2.AppendText(cedula.incidencias.fumigacion[r].FechaRealizada.ToString("dd/MM/yyyy"));
                                }
                                if (c == 3)
                                {
                                    TR2 = p2.AppendText(cedula.incidencias.fumigacion[r].Comentarios);
                                }
                                //Formato de celdas
                                p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                                TR2.CharacterFormat.FontName = "Arial";
                                TR2.CharacterFormat.FontSize = 9;
                            }
                        }
                    }

                    BookmarksNavigator marcaActividades = new BookmarksNavigator(document);
                    marcaActividades.MoveToBookmark("Fechas", true, true);
                    marcaActividades.InsertTable(tablaActividades);
                    document.Replace("||Fechas||", "El personal no cumplió con las actividades contempladas en el programa de operación, las cuales se describen a continuación: ", false, true);
                }
                else
                {
                    document.Replace("||Fechas||", "El personal cumplió con las actividades contempladas en el programa de operación, no presentó incidencias en el mes.", false, true);
                }

                cedula.incidencias.fumigacion = await iFumigacion.GetIncidenciasPregunta(id, 3);
                //obtenemos el documento con marcas
                if (cedula.incidencias.fumigacion.Count > 0)
                {
                    Table tablaHoras = tablas.AddTable(true);

                    String[] cabeceraHoras = { "Tipo", "Hora Programada", "Hora Realizada", "Comentarios" };

                    tablaHoras.ResetCells(cedula.incidencias.fumigacion.Count + 1, cabeceraHoras.Length);

                    TableRow recRow = tablaHoras.Rows[0];
                    recRow.IsHeader = true;
                    recRow.Height = 10;

                    recRow.RowFormat.BackColor = Color.FromArgb(81, 25, 162);
                    recRow.RowFormat.Borders.BorderType = BorderStyle.Single;


                    for (int i = 0; i < cabeceraHoras.Length; i++)
                    {
                        //Alineacion de celdas
                        Paragraph p = recRow.Cells[i].AddParagraph();
                        tablaHoras.Rows[0].Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        p.Format.HorizontalAlignment = HorizontalAlignment.Center;

                        //Formato de datos
                        TextRange TR = p.AppendText(cabeceraHoras[i]);
                        TR.CharacterFormat.FontName = "Arial";
                        TR.CharacterFormat.FontSize = 9;
                        TR.CharacterFormat.Bold = true;
                        TR.CharacterFormat.TextColor = Color.White;
                    }

                    for (int r = 0; r < cedula.incidencias.fumigacion.Count; r++)
                    {
                        if (cedula.incidencias.fumigacion[r].Pregunta.Equals(3 + ""))
                        {
                            TableRow DataRow = tablaHoras.Rows[r + 1];
                            //Fila Height
                            DataRow.Height = 5;
                            for (int c = 0; c < cabeceraHoras.Length; c++)
                            {
                                TextRange TR2 = null;
                                //Alineacion de Celdas
                                DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                                //Llenar datos en filas
                                Paragraph p2 = DataRow.Cells[c].AddParagraph();
                                if (c == 0)
                                {
                                    TR2 = p2.AppendText(cedula.incidencias.fumigacion[r].Tipo);
                                }
                                if (c == 1)
                                {
                                    TR2 = p2.AppendText(cedula.incidencias.fumigacion[r].HoraProgramada + "");
                                }
                                if (c == 2)
                                {
                                    TR2 = p2.AppendText(cedula.incidencias.fumigacion[r].HoraRealizada + "");
                                }
                                if (c == 3)
                                {
                                    TR2 = p2.AppendText(cedula.incidencias.fumigacion[r].Comentarios);
                                }
                                //Formato de celdas
                                p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                                TR2.CharacterFormat.FontName = "Arial";
                                TR2.CharacterFormat.FontSize = 9;
                            }
                        }
                    }

                    BookmarksNavigator marcaActividades = new BookmarksNavigator(document);
                    marcaActividades.MoveToBookmark("Horas", true, true);
                    marcaActividades.InsertTable(tablaHoras);
                    document.Replace("||Horas||", "El personal no cumplió con las horas pactadas para la ejecución del servicio, las cuales se describen a continuación: ", false, true);
                }
                else
                {
                    document.Replace("||Horas||", "El personal cumplió con las horas pactadas para la ejecución del servicio, no presentó incidencias en el mes.", false, true);
                }

                cedula.incidencias.fumigacion = await iFumigacion.GetIncidenciasPregunta(id, 4);
                //obtenemos el documento con marcas
                if (cedula.incidencias.fumigacion.Count > 0)
                {
                    Table tablaFauna = tablas.AddTable(true);

                    String[] cabeceraFauna = { "Tipo", "Fecha Realizada", "Fecha de Reaparición", "Comentarios" };

                    tablaFauna.ResetCells(cedula.incidencias.fumigacion.Count + 1, cabeceraFauna.Length);

                    TableRow recRow = tablaFauna.Rows[0];
                    recRow.IsHeader = true;
                    recRow.Height = 10;

                    recRow.RowFormat.BackColor = Color.FromArgb(81, 25, 162);
                    recRow.RowFormat.Borders.BorderType = BorderStyle.Single;


                    for (int i = 0; i < cabeceraFauna.Length; i++)
                    {
                        //Alineacion de celdas
                        Paragraph p = recRow.Cells[i].AddParagraph();
                        tablaFauna.Rows[0].Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        p.Format.HorizontalAlignment = HorizontalAlignment.Center;

                        //Formato de datos
                        TextRange TR = p.AppendText(cabeceraFauna[i]);
                        TR.CharacterFormat.FontName = "Arial";
                        TR.CharacterFormat.FontSize = 9;
                        TR.CharacterFormat.Bold = true;
                        TR.CharacterFormat.TextColor = Color.White;
                    }

                    for (int r = 0; r < cedula.incidencias.fumigacion.Count; r++)
                    {
                        if (cedula.incidencias.fumigacion[r].Pregunta.Equals(4 + ""))
                        {
                            TableRow DataRow = tablaFauna.Rows[r + 1];
                            //Fila Height
                            DataRow.Height = 5;
                            for (int c = 0; c < cabeceraFauna.Length; c++)
                            {
                                TextRange TR2 = null;
                                //Alineacion de Celdas
                                DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                                //Llenar datos en filas
                                Paragraph p2 = DataRow.Cells[c].AddParagraph();
                                if (c == 0)
                                {
                                    TR2 = p2.AppendText(cedula.incidencias.fumigacion[r].Tipo);
                                }
                                if (c == 1)
                                {
                                    TR2 = p2.AppendText(cedula.incidencias.fumigacion[r].FechaProgramada.ToString("dd/MM/yyyy"));
                                }
                                if (c == 2)
                                {
                                    TR2 = p2.AppendText(cedula.incidencias.fumigacion[r].FechaRealizada.ToString("dd/MM/yyyy"));
                                }
                                if (c == 3)
                                {
                                    TR2 = p2.AppendText(cedula.incidencias.fumigacion[r].Comentarios);
                                }
                                //Formato de celdas
                                p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                                TR2.CharacterFormat.FontName = "Arial";
                                TR2.CharacterFormat.FontSize = 9;
                            }
                        }
                    }

                    BookmarksNavigator marcaActividades = new BookmarksNavigator(document);
                    marcaActividades.MoveToBookmark("Fauna", true, true);
                    marcaActividades.InsertTable(tablaFauna);
                    document.Replace("||Fauna||", "El personal no cumplió con la efectividad del servicio ya que hubo reaparición de fauna nociva, las cuales se describen a continuación: ", false, true);
                }
                else
                {
                    document.Replace("||Fauna||", "El personal cumplió con la efectividad del servicio ya que NO hubo reaparición de fauna nociva.", false, true);
                }

                if (cedula.RespuestasEncuesta[4].Respuesta == true)
                {
                    document.Replace("||Productos||", "El prestador cumplió con la regulación vigente de los productos.", false, true);
                }
                else
                {
                    document.Replace("||Productos||", "El prestador no cumplió con la regulación vigente de los productos.", false, true);
                }

                if (cedula.RespuestasEncuesta[5].Respuesta == true)
                {
                    document.Replace("||ReporteServicios||", "El reporte de servicios se entregó en tiempo y forma el " +
                        Convert.ToDateTime(cedula.RespuestasEncuesta[5].Detalles).ToString("dd") + " de " +
                        Convert.ToDateTime(cedula.RespuestasEncuesta[5].Detalles).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " +
                        Convert.ToDateTime(cedula.RespuestasEncuesta[5].Detalles).ToString("yyyy") + ".", false, true);
                }
                else
                {
                    document.Replace("||ReporteServicios||", "No se entregó el reporte de servicios por parte del prestador.", false, true);
                }

                if (cedula.RespuestasEncuesta[6].Respuesta == true)
                {
                    document.Replace("||ListadoPersonal||", "El listado de personal se entregó en tiempo y forma el " +
                        Convert.ToDateTime(cedula.RespuestasEncuesta[6].Detalles).ToString("dd") + " de " +
                        Convert.ToDateTime(cedula.RespuestasEncuesta[6].Detalles).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " +
                        Convert.ToDateTime(cedula.RespuestasEncuesta[6].Detalles).ToString("yyyy") + ".", false, true);
                }
                else
                {
                    document.Replace("||ListadoPersonal||", "No se entregó el reporte de servicios por parte del prestador.", false, true);
                }

                if (cedula.RespuestasEncuesta[7].Respuesta == false)
                {
                    document.Replace("||SUA||", "El prestador de servicio no entregó el SUA de su personal." + "\nComentarios: " +
                                         cedula.RespuestasEncuesta[7].Detalles.Split("|")[1], false, true);
                }
                else
                {
                    string numeromes = DateTime.ParseExact(cedula.Mes, "MMMM", @System.Globalization.CultureInfo.CreateSpecificCulture("es")).Month > 9 ? DateTime.ParseExact(cedula.Mes, "MMMM", @System.Globalization.CultureInfo.CreateSpecificCulture("es")).Month + "" : "0" + DateTime.ParseExact(cedula.Mes, "MMMM", @System.Globalization.CultureInfo.CreateSpecificCulture("es")).Month;
                    string fecha = cedula.RespuestasEncuesta[7].Detalles.Split("|")[0];
                    string entregaSua = Convert.ToDateTime(fecha).Year + "-" + numeromes + "-22";
                    string diaSua = Convert.ToDateTime(entregaSua).DayOfWeek + "";
                    string diaEntrega = Convert.ToDateTime(fecha).Day + "";
                    string dia = fecha.Split("-")[2];
                    document.Replace("||SUA||", "El prestador de servicio entrego el SUA de su personal en tiempo y forma, la fecha de entrega fue el " +
                                         Convert.ToDateTime(fecha).Day + " de " +
                                         Convert.ToDateTime(fecha).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " +
                                         Convert.ToDateTime(fecha).Year + " correspondiente al mes de " +
                                         cedula.RespuestasEncuesta[7].Detalles.Split("|")[1], false, true);
                }


                document.Replace("||Folio||", cedula.Folio, false, true);

                document.Replace("||Mes||", cedula.Mes, false, true);

                document.Replace("||Administracion||", cedula.inmuebles.Nombre, false, true);

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

                document.Replace("||Factura||", strFacturas, false, true);

                document.Replace("||Total||", String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:C}", vFacturas.obtieneTotalFacturas(cedula.facturas)) + "", false, true);
                if (!cedula.Estatus.Equals("Enviada a DAS") && !cedula.Estatus.Equals("Rechazada"))
                {
                    document.Replace("||C||", cedula.Calificacion.ToString(), false, true);
                }
                else
                {
                    document.Replace("||C||", "Pendiente", false, true);
                }

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
                return File(toArray, "application/pdf", "CedulaFumigacion_" + cedula.Mes + ".pdf");
            }
            return Redirect("/error/denied");
        }

        [Route("/reporte/agua/{id}")]
        public async Task<IActionResult> CedulaAgua(int id)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), moduloAgua(), "cédula");
            if (success == 1)
            {
                string strFacturas = "";
                decimal totalFacturas = 0;

                CedulaEvaluacion cedula = new CedulaEvaluacion();
                cedula = await vCedula.CedulaById(id);
                cedula.inmuebles = await vInmuebles.inmuebleById(cedula.InmuebleId);
                cedula.usuarios = await vUsuarios.getUserById(cedula.UsuarioId);
                cedula.iEntregables = await vEntregables.getEntregables(id);
                cedula.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedula.RespuestasEncuesta = await vCedula.obtieneRespuestas(id);
                cedula.facturas = new List<Facturas>();
                cedula.facturas = await vFacturas.getFacturas(id, 9);
                cedula.incidencias = new Entities.MIncidencias.ModelsIncidencias();

                Document document = new Document();
                var path = @"E:\Plantillas CASESGV2\DocsV2\ReporteAgua.docx";
                document.LoadFromFile(path);

                //Creamos la Tabla
                Section tablas = document.AddSection();

                if (!cedula.RespuestasEncuesta[0].Detalles.Equals(null))
                {
                    document.Replace("||FechaCierre||", "El cierre de mes se efectuó el " + Convert.ToDateTime(cedula.RespuestasEncuesta[0].Detalles).ToString("dd") + " de " +
                        Convert.ToDateTime(cedula.RespuestasEncuesta[0].Detalles.Split("|")[0]).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " +
                        Convert.ToDateTime(cedula.RespuestasEncuesta[0].Detalles).ToString("yyyy") + ".", false, true);
                }

                cedula.incidencias.agua = await iAgua.GetIncidenciasPregunta(id, 2);
                //obtenemos el documento con marcas
                if (cedula.incidencias.agua.Count > 0)
                {
                    Table tablaActividades = tablas.AddTable(true);

                    String[] cabeceraFechas = { "Tipo", "Fecha Programada", "Fecha Atención", "Comentarios" };

                    tablaActividades.ResetCells(cedula.incidencias.agua.Count + 1, cabeceraFechas.Length);

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

                    for (int r = 0; r < cedula.incidencias.agua.Count; r++)
                    {
                        if (cedula.incidencias.agua[r].Pregunta.Equals(2 + ""))
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
                                    TR2 = p2.AppendText(cedula.incidencias.agua[r].Tipo);
                                }
                                if (c == 1)
                                {
                                    TR2 = p2.AppendText(cedula.incidencias.agua[r].FechaProgramada.ToString("dd/MM/yyyy"));
                                }
                                if (c == 2)
                                {
                                    TR2 = p2.AppendText(cedula.incidencias.agua[r].FechaRealizada.ToString("dd/MM/yyyy"));
                                }
                                if (c == 3)
                                {
                                    TR2 = p2.AppendText(cedula.incidencias.agua[r].Comentarios);
                                }
                                //Formato de celdas
                                p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                                TR2.CharacterFormat.FontName = "Arial";
                                TR2.CharacterFormat.FontSize = 9;
                            }
                        }
                    }

                    BookmarksNavigator marcaActividades = new BookmarksNavigator(document);
                    marcaActividades.MoveToBookmark("Fechas", true, true);
                    marcaActividades.InsertTable(tablaActividades);
                    document.Replace("||Fechas||", "El personal no cumplió con las actividades contempladas en el programa de operación, las cuales se describen a continuación: ", false, true);
                }
                else
                {
                    document.Replace("||Fechas||", "El personal cumplió con las actividades contempladas en el programa de operación, no presentó incidencias en el mes.", false, true);
                }

                cedula.incidencias.agua = await iAgua.GetIncidenciasPregunta(id, 3);
                //obtenemos el documento con marcas
                if (cedula.incidencias.agua.Count > 0)
                {
                    Table tablaHoras = tablas.AddTable(true);

                    String[] cabeceraHoras = { "Tipo", "Hora Programada", "Hora Realizada", "Comentarios" };

                    tablaHoras.ResetCells(cedula.incidencias.agua.Count + 1, cabeceraHoras.Length);

                    TableRow recRow = tablaHoras.Rows[0];
                    recRow.IsHeader = true;
                    recRow.Height = 10;

                    recRow.RowFormat.BackColor = Color.FromArgb(81, 25, 162);
                    recRow.RowFormat.Borders.BorderType = BorderStyle.Single;


                    for (int i = 0; i < cabeceraHoras.Length; i++)
                    {
                        //Alineacion de celdas
                        Paragraph p = recRow.Cells[i].AddParagraph();
                        tablaHoras.Rows[0].Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        p.Format.HorizontalAlignment = HorizontalAlignment.Center;

                        //Formato de datos
                        TextRange TR = p.AppendText(cabeceraHoras[i]);
                        TR.CharacterFormat.FontName = "Arial";
                        TR.CharacterFormat.FontSize = 9;
                        TR.CharacterFormat.Bold = true;
                        TR.CharacterFormat.TextColor = Color.White;
                    }

                    for (int r = 0; r < cedula.incidencias.agua.Count; r++)
                    {
                        if (cedula.incidencias.agua[r].Pregunta.Equals(3 + ""))
                        {
                            TableRow DataRow = tablaHoras.Rows[r + 1];
                            //Fila Height
                            DataRow.Height = 5;
                            for (int c = 0; c < cabeceraHoras.Length; c++)
                            {
                                TextRange TR2 = null;
                                //Alineacion de Celdas
                                DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                                //Llenar datos en filas
                                Paragraph p2 = DataRow.Cells[c].AddParagraph();
                                if (c == 0)
                                {
                                    TR2 = p2.AppendText(cedula.incidencias.agua[r].Tipo);
                                }
                                if (c == 1)
                                {
                                    TR2 = p2.AppendText(cedula.incidencias.agua[r].HoraProgramada + "");
                                }
                                if (c == 2)
                                {
                                    TR2 = p2.AppendText(cedula.incidencias.agua[r].HoraRealizada + "");
                                }
                                if (c == 3)
                                {
                                    TR2 = p2.AppendText(cedula.incidencias.agua[r].Comentarios);
                                }
                                //Formato de celdas
                                p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                                TR2.CharacterFormat.FontName = "Arial";
                                TR2.CharacterFormat.FontSize = 9;
                            }
                        }
                    }

                    BookmarksNavigator marcaActividades = new BookmarksNavigator(document);
                    marcaActividades.MoveToBookmark("Horas", true, true);
                    marcaActividades.InsertTable(tablaHoras);
                    document.Replace("||Horas||", "El personal no cumplió con las horas pactadas para la ejecución del servicio, las cuales se describen a continuación: ", false, true);
                }
                else
                {
                    document.Replace("||Horas||", "El personal cumplió con las horas pactadas para la ejecución del servicio, no presentó incidencias en el mes.", false, true);
                }

                cedula.incidencias.agua = await iAgua.GetIncidenciasPregunta(id, 4);
                //obtenemos el documento con marcas
                if (cedula.incidencias.agua.Count > 0)
                {
                    Table tablaFauna = tablas.AddTable(true);

                    String[] cabeceraFauna = { "Tipo", "Fecha Realizada", "Fecha Recibida", "Comentarios" };

                    tablaFauna.ResetCells(cedula.incidencias.agua.Count + 1, cabeceraFauna.Length);

                    TableRow recRow = tablaFauna.Rows[0];
                    recRow.IsHeader = true;
                    recRow.Height = 10;

                    recRow.RowFormat.BackColor = Color.FromArgb(81, 25, 162);
                    recRow.RowFormat.Borders.BorderType = BorderStyle.Single;


                    for (int i = 0; i < cabeceraFauna.Length; i++)
                    {
                        //Alineacion de celdas
                        Paragraph p = recRow.Cells[i].AddParagraph();
                        tablaFauna.Rows[0].Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        p.Format.HorizontalAlignment = HorizontalAlignment.Center;

                        //Formato de datos
                        TextRange TR = p.AppendText(cabeceraFauna[i]);
                        TR.CharacterFormat.FontName = "Arial";
                        TR.CharacterFormat.FontSize = 9;
                        TR.CharacterFormat.Bold = true;
                        TR.CharacterFormat.TextColor = Color.White;
                    }

                    for (int r = 0; r < cedula.incidencias.agua.Count; r++)
                    {
                        if (cedula.incidencias.agua[r].Pregunta.Equals(4 + ""))
                        {
                            TableRow DataRow = tablaFauna.Rows[r + 1];
                            //Fila Height
                            DataRow.Height = 5;
                            for (int c = 0; c < cabeceraFauna.Length; c++)
                            {
                                TextRange TR2 = null;
                                //Alineacion de Celdas
                                DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                                //Llenar datos en filas
                                Paragraph p2 = DataRow.Cells[c].AddParagraph();
                                if (c == 0)
                                {
                                    TR2 = p2.AppendText(cedula.incidencias.agua[r].Tipo);
                                }
                                if (c == 1)
                                {
                                    TR2 = p2.AppendText(cedula.incidencias.agua[r].FechaProgramada.ToString("dd/MM/yyyy"));
                                }
                                if (c == 2)
                                {
                                    TR2 = p2.AppendText(cedula.incidencias.agua[r].FechaRealizada.ToString("dd/MM/yyyy"));
                                }
                                if (c == 3)
                                {
                                    TR2 = p2.AppendText(cedula.incidencias.agua[r].Comentarios);
                                }
                                //Formato de celdas
                                p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                                TR2.CharacterFormat.FontName = "Arial";
                                TR2.CharacterFormat.FontSize = 9;
                            }
                        }
                    }

                    BookmarksNavigator marcaActividades = new BookmarksNavigator(document);
                    marcaActividades.MoveToBookmark("Garrafones", true, true);
                    marcaActividades.InsertTable(tablaFauna);
                    document.Replace("||Garrafones||", "El personal no cumplió con la efectividad del servicio ya que los garrafones no cumplieron con las características requeridas por el consejo.", false, true);
                }
                else
                {
                    document.Replace("||Garrafones||", "El personal cumplió con la efectividad del servicio ya que los garrafones cumplieron con las características requeridas por el consejo.", false, true);
                }

                if (cedula.RespuestasEncuesta[4].Respuesta == true)
                {
                    document.Replace("||Productos||", "El prestador cumplió con la regulación vigente de los garrafones.", false, true);
                }
                else
                {
                    document.Replace("||Productos||", "El prestador no cumplió con la regulación vigente de los garrafones.", false, true);
                }

                if (cedula.RespuestasEncuesta[5].Respuesta == true)
                {
                    document.Replace("||ReporteServicios||", "El reporte de servicios se entregó en tiempo y forma el " +
                        Convert.ToDateTime(cedula.RespuestasEncuesta[5].Detalles).ToString("dd") + " de " +
                        Convert.ToDateTime(cedula.RespuestasEncuesta[5].Detalles).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " +
                        Convert.ToDateTime(cedula.RespuestasEncuesta[5].Detalles).ToString("yyyy") + ".", false, true);
                }
                else
                {
                    document.Replace("||ReporteServicios||", "No se entregó el reporte de servicios por parte del prestador.", false, true);
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

                document.Replace("||Administracion||", cedula.inmuebles.Nombre, false, true);

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
                return File(toArray, "application/pdf", "CedulaAgua_" + cedula.Mes + ".pdf");
            }
            return Redirect("/error/denied");
        }

        [Route("/reporte/residuos/{id}")]
        public async Task<IActionResult> CedulaResiduos(int id)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), moduloRPBI(), "cédula");
            if (success == 1)
            {
                string strFacturas = "";
                decimal totalFacturas = 0;

                CedulaEvaluacion cedula = new CedulaEvaluacion();
                cedula = await vCedula.CedulaById(id);
                cedula.inmuebles = await vInmuebles.inmuebleById(cedula.InmuebleId);
                cedula.usuarios = await vUsuarios.getUserById(cedula.UsuarioId);
                cedula.incidencias = new Entities.MIncidencias.ModelsIncidencias();
                cedula.incidencias.incidenciasResiduos = await iResiduos.getIncidencias(id);
                cedula.incidencias.incidenciasManifiesto = await iResiduos.getIncidenciasTipo(id, "ManifiestoEntrega");
                cedula.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedula.RespuestasEncuesta = await vCedula.obtieneRespuestas(id);
                cedula.facturas = new List<Facturas>();
                cedula.facturas = await vFacturas.getFacturas(id, 7);

                Document document = new Document();
                var path = @"E:\Plantillas CASESGV2\DocsV2\ReporteResiduos.docx";
                document.LoadFromFile(path);

                //Creamos la Tabla
                Section tablas = document.AddSection();

                if (cedula.RespuestasEncuesta[0].Detalles.Equals(null) || cedula.RespuestasEncuesta[0].Detalles.Equals(""))
                {
                    document.Replace("||Servicio||", "El servicio se llevó a cabo en el día programado.", false, true);
                }
                else
                {
                    string fechaProgramada = cedula.RespuestasEncuesta[0].Detalles.Split("|")[0];
                    string fechaRealizada = cedula.RespuestasEncuesta[0].Detalles.Split("|")[1];
                    document.Replace("||Servicio||", "El servicio no se llevó a cabo en el día programado, la fecha programada era el " +
                        Convert.ToDateTime(fechaProgramada).ToString("dd") + " de " +
                        Convert.ToDateTime(fechaProgramada).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " +
                        Convert.ToDateTime(fechaProgramada).ToString("yyyy") + " y\n el servicio se efectuó el " + Convert.ToDateTime(fechaRealizada).ToString("dd") + " de " +
                        Convert.ToDateTime(fechaRealizada).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " +
                        Convert.ToDateTime(fechaRealizada).ToString("yyyy") + ".", false, true);
                }

                //obtenemos el documento con marcas
                if (cedula.incidencias.incidenciasResiduos.Count > 0)
                {
                    Table tablaActividades = tablas.AddTable(true);

                    String[] cabeceraFechas = { "Tipo", "Comentarios" };

                    tablaActividades.ResetCells(cedula.incidencias.incidenciasResiduos.Count + 1, cabeceraFechas.Length);

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

                    for (int r = 0; r < cedula.incidencias.incidenciasResiduos.Count; r++)
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
                                TR2 = p2.AppendText(cedula.incidencias.incidenciasResiduos[r].Tipo);
                            }
                            if (c == 1)
                            {
                                TR2 = p2.AppendText(cedula.incidencias.incidenciasResiduos[r].Comentarios);
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
                    document.Replace("||Equipo||", "El personal del servicio no portó el equipo de protección en todo momento, se presentaron las siguientes incidencias: ", false, true);
                }
                else
                {
                    document.Replace("||Equipo||", "El personal de servicio portó el equipo de protección en todo momento, se presentaron las siguientes incidencias: ", false, true);
                }

                //obtenemos el documento con marcas
                if (cedula.incidencias.incidenciasManifiesto.Count > 0)
                {
                    Table tablaHoras = tablas.AddTable(true);
                    string coment = "";
                    String[] cabeceraHoras = { "Tipo", "Datos Faltantes" };

                    int filas = cedula.incidencias.incidenciasManifiesto.ElementAt(0).Comentarios.Split("|").Length - 1;

                    string[] comentarios = cedula.incidencias.incidenciasManifiesto.ElementAt(0).Comentarios.Split("|");

                    tablaHoras.ResetCells(filas + 1, cabeceraHoras.Length);

                    TableRow recRow = tablaHoras.Rows[0];
                    recRow.IsHeader = true;
                    recRow.Height = 10;

                    recRow.RowFormat.BackColor = Color.FromArgb(81, 25, 162);
                    recRow.RowFormat.Borders.BorderType = BorderStyle.Single;


                    for (int i = 0; i < cabeceraHoras.Length; i++)
                    {
                        //Alineacion de celdas
                        Paragraph p = recRow.Cells[i].AddParagraph();
                        tablaHoras.Rows[0].Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        p.Format.HorizontalAlignment = HorizontalAlignment.Center;

                        //Formato de datos
                        TextRange TR = p.AppendText(cabeceraHoras[i]);
                        TR.CharacterFormat.FontName = "Arial";
                        TR.CharacterFormat.FontSize = 9;
                        TR.CharacterFormat.Bold = true;
                        TR.CharacterFormat.TextColor = Color.White;
                    }

                    for (int r = 0; r < filas; r++)
                    {
                        TableRow DataRow = tablaHoras.Rows[r + 1];
                        //Fila Height
                        DataRow.Height = 5;
                        for (int c = 0; c < cabeceraHoras.Length; c++)
                        {
                            TextRange TR2 = null;
                            //Alineacion de Celdas
                            DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                            //Llenar datos en filas
                            Paragraph p2 = DataRow.Cells[c].AddParagraph();
                            if (c == 0)
                            {
                                TR2 = p2.AppendText("Manifiesto Entrega");
                            }
                            if (c == 1)
                            {
                                if (comentarios[r].Equals("DatosTitularMedico"))
                                {
                                    coment = "Nombre, Sello y Firma del titular del servicio médico";
                                }
                                else if (comentarios[r].Equals("ConsultorioBrindaServicio"))
                                {
                                    coment = "Domicilio del Consultorio que brinda el servicio";
                                }
                                else if (comentarios[r].Equals("DescripcionResiduo"))
                                {
                                    coment = "Descripción e identificación del Tipo de Residuo";
                                }
                                else if (comentarios[r].Equals("DatosRecoleccion"))
                                {
                                    coment = "Nombre y firma del responsable de la recolección";
                                }
                                else if (comentarios[r].Equals("DetallesResiduo"))
                                {
                                    coment = "Cantidad y Unidad de medida del Residuo";
                                }
                                else if (comentarios[r].Equals("EnvaseUtilizado"))
                                {
                                    coment = "Envase o contenedor utilizado";
                                }
                                else if (comentarios[r].Equals("ManejoSeguro"))
                                {
                                    coment = "Instrucciones especiales e información para su manejo seguro";
                                }
                                else if (comentarios[r].Equals("CentroDestinatario"))
                                {
                                    coment = "Descripción General de la Transportación y del Centro de Acopio o Destinatario";
                                }
                                else if (comentarios[r].Equals("RegistroAmbiental"))
                                {
                                    coment = "Número de Registro Ambiental";
                                }
                                else if (comentarios[r].Equals("NumeroManifiesto"))
                                {
                                    coment = "Número de Manifiesto";
                                }
                                else if (comentarios[r].Equals("Clasificacion"))
                                {
                                    coment = "Clasificación";
                                }
                                TR2 = p2.AppendText(coment);
                            }
                            //Formato de celdas
                            p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                            TR2.CharacterFormat.FontName = "Arial";
                            TR2.CharacterFormat.FontSize = 9;
                        }
                    }

                    BookmarksNavigator marcaActividades = new BookmarksNavigator(document);
                    marcaActividades.MoveToBookmark("Manifiesto", true, true);
                    marcaActividades.InsertTable(tablaHoras);
                    document.Replace("||Manifiesto||", "El personal no cumplió con todos los datos necesariós en el manifiesto de entrega, faltaron los siguientes datos: ", false, true);
                }
                else
                {
                    document.Replace("||Manifiesto||", "El personal cumplió con todos los datos en el manifiesto de entrega.", false, true);
                }

                if (cedula.RespuestasEncuesta[1].Respuesta == true)
                {
                    document.Replace("||Identificacion||", "El personal de servicio presentó identificación en todo momento.", false, true);
                }
                else
                {
                    document.Replace("||Identificacion||", "El personal de servicio no presentó identificación en todo momento.", false, true);
                }

                if (cedula.RespuestasEncuesta[2].Respuesta == true)
                {
                    document.Replace("||Recoleccion||", "La recolección se llevó a cabo de acuerdo a las condiciones técnicas descritas en el anexo técnico.", false, true);
                }
                else
                {
                    document.Replace("||Recoleccion||", "La recolección no se llevó a cabo de acuerdo a las condiciones técnicas descritas en el anexo técnico.", false, true);
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

                document.Replace("||Administracion||", cedula.inmuebles.Nombre, false, true);

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
                return File(toArray, "application/pdf", "CedulaRPBI_" + cedula.Mes + ".pdf");
            }
            return Redirect("/error/denied");
        }

        [Route("/reporte/transporte/{id}")]
        public async Task<IActionResult> CedulaTransporte(int id)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), moduloTransporte(), "cédula");
            if (success == 1)
            {
                string strFacturas = "";
                decimal totalFacturas = 0;

                CedulaEvaluacion cedula = new CedulaEvaluacion();
                cedula = await vCedula.CedulaById(id);
                cedula.inmuebles = await vInmuebles.inmuebleById(cedula.InmuebleId);
                cedula.usuarios = await vUsuarios.getUserById(cedula.UsuarioId);
                cedula.incidencias.transporte = await iTransporte.GetIncidenciasPregunta(id, 2);
                cedula.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedula.RespuestasEncuesta = await vTransporte.obtieneRespuestas(id);
                cedula.facturas = new List<Facturas>();
                cedula.facturas = await vFacturas.getFacturas(id, 10);

                Document document = new Document();
                var path = @"E:\Plantillas CASESGV2\DocsV2\ReporteTransporte.docx";
                document.LoadFromFile(path);

                //Creamos la Tabla
                Section tablas = document.AddSection();

                if (!cedula.RespuestasEncuesta[0].Detalles.Equals(null))
                {
                    document.Replace("||FechaCierre||", "El cierre de mes se efectuó el " + Convert.ToDateTime(cedula.RespuestasEncuesta[0].Detalles).ToString("dd") + " de " +
                        Convert.ToDateTime(cedula.RespuestasEncuesta[0].Detalles.Split("|")[0]).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " de " +
                        Convert.ToDateTime(cedula.RespuestasEncuesta[0].Detalles).ToString("yyyy") + ".", false, true);
                }

                //obtenemos el documento con marcas
                if (cedula.incidencias.transporte.Count > 0)
                {
                    Table tablaActividades = tablas.AddTable(true);

                    String[] cabeceraFechas = { "Tipo", "Fecha de la Incidencia", "Hora Presentada", "Comentarios" };

                    tablaActividades.ResetCells(cedula.incidencias.transporte.Count + 1, cabeceraFechas.Length);

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

                    for (int r = 0; r < cedula.incidencias.transporte.Count; r++)
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
                                TR2 = p2.AppendText(cedula.incidencias.transporte[r].Tipo);
                            }
                            if (c == 1)
                            {
                                TR2 = p2.AppendText(cedula.incidencias.transporte[r].FechaIncidencia.ToString("dd/MM/yyyy"));
                            }
                            if (c == 2)
                            {
                                TR2 = p2.AppendText(cedula.incidencias.transporte[r].HoraPresentada + "");
                            }
                            if (c == 3)
                            {
                                TR2 = p2.AppendText(cedula.incidencias.transporte[r].Comentarios);
                            }
                            //Formato de celdas
                            p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                            TR2.CharacterFormat.FontName = "Arial";
                            TR2.CharacterFormat.FontSize = 9;
                        }
                    }

                    BookmarksNavigator marcaActividades = new BookmarksNavigator(document);
                    marcaActividades.MoveToBookmark("Horarios", true, true);
                    marcaActividades.InsertTable(tablaActividades);
                    document.Replace("||Horarios||", "El personal de servicio no cumplió con los horarios y recorridos establecidos en el contrato.", false, true);
                }
                else
                {
                    document.Replace("||Horarios||", "El personal de servicio cumplió con los horarios y recorridos establecidos en el contrato.", false, true);
                }

                if (cedula.RespuestasEncuesta[2].Respuesta == true)
                {
                    document.Replace("||Registro||", "Se llevó a cabo el registro de los servidores públicos en este mes.", false, true);
                }
                else
                {
                    document.Replace("||Registro||", "No se llevó a cabo el registro de los servidores públicos en este mes, los comentarios son los siguientes:" +
                        "\n\n" + cedula.RespuestasEncuesta[2].Detalles, false, true);
                }

                if (cedula.RespuestasEncuesta[3].Respuesta == true)
                {
                    document.Replace("||Ocupacion||", "La ocupación promedio fue mayor o igual al 50%.", false, true);
                }
                else
                {
                    document.Replace("||Ocupacion||", "La ocupación promedio fue menor o igual al 50%, los comentarios son los siguientes:" +
                        "\n\n" + cedula.RespuestasEncuesta[3].Detalles, false, true);
                }

                cedula.incidencias.transporte = await iTransporte.GetIncidenciasPregunta(id, 5);
                //obtenemos el documento con marcas
                if (cedula.incidencias.transporte.Count > 0)
                {
                    Table tablaHoras = tablas.AddTable(true);

                    String[] cabeceraHoras = { "Tipo", "Fecha de la Incidencia", "Comentarios" };

                    tablaHoras.ResetCells(cedula.incidencias.transporte.Count + 1, cabeceraHoras.Length);

                    TableRow recRow = tablaHoras.Rows[0];
                    recRow.IsHeader = true;
                    recRow.Height = 10;

                    recRow.RowFormat.BackColor = Color.FromArgb(81, 25, 162);
                    recRow.RowFormat.Borders.BorderType = BorderStyle.Single;


                    for (int i = 0; i < cabeceraHoras.Length; i++)
                    {
                        //Alineacion de celdas
                        Paragraph p = recRow.Cells[i].AddParagraph();
                        tablaHoras.Rows[0].Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        p.Format.HorizontalAlignment = HorizontalAlignment.Center;

                        //Formato de datos
                        TextRange TR = p.AppendText(cabeceraHoras[i]);
                        TR.CharacterFormat.FontName = "Arial";
                        TR.CharacterFormat.FontSize = 9;
                        TR.CharacterFormat.Bold = true;
                        TR.CharacterFormat.TextColor = Color.White;
                    }

                    for (int r = 0; r < cedula.incidencias.transporte.Count; r++)
                    {
                        TableRow DataRow = tablaHoras.Rows[r + 1];
                        //Fila Height
                        DataRow.Height = 5;
                        for (int c = 0; c < cabeceraHoras.Length; c++)
                        {
                            TextRange TR2 = null;
                            //Alineacion de Celdas
                            DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                            //Llenar datos en filas
                            Paragraph p2 = DataRow.Cells[c].AddParagraph();
                            if (c == 0)
                            {
                                TR2 = p2.AppendText(cedula.incidencias.transporte[r].Tipo);
                            }
                            if (c == 1)
                            {
                                TR2 = p2.AppendText(cedula.incidencias.transporte[r].FechaIncidencia.ToString("dd/MM/yyyy"));
                            }
                            if (c == 2)
                            {
                                TR2 = p2.AppendText(cedula.incidencias.transporte[r].Comentarios);
                            }
                            //Formato de celdas
                            p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                            TR2.CharacterFormat.FontName = "Arial";
                            TR2.CharacterFormat.FontSize = 9;
                        }
                    }

                    BookmarksNavigator marcaActividades = new BookmarksNavigator(document);
                    marcaActividades.MoveToBookmark("Supervision", true, true);
                    marcaActividades.InsertTable(tablaHoras);
                    document.Replace("||Supervision||", "El personal no cumplió con la supervisión del servicio, se presentaron las siguientes incidencias: ", false, true);
                }
                else
                {
                    document.Replace("||Supervision||", "El personal cumplió con la supervisión del servicio.", false, true);
                }

                cedula.incidencias.transporte = await iTransporte.GetIncidenciasPregunta(id, 6);
                //obtenemos el documento con marcas
                if (cedula.incidencias.transporte.Count > 0)
                {
                    Table tablaHoras = tablas.AddTable(true);

                    String[] cabeceraHoras = { "Tipo", "Fecha de la Incidencia", "Comentarios" };

                    tablaHoras.ResetCells(cedula.incidencias.transporte.Count + 1, cabeceraHoras.Length);

                    TableRow recRow = tablaHoras.Rows[0];
                    recRow.IsHeader = true;
                    recRow.Height = 10;

                    recRow.RowFormat.BackColor = Color.FromArgb(81, 25, 162);
                    recRow.RowFormat.Borders.BorderType = BorderStyle.Single;


                    for (int i = 0; i < cabeceraHoras.Length; i++)
                    {
                        //Alineacion de celdas
                        Paragraph p = recRow.Cells[i].AddParagraph();
                        tablaHoras.Rows[0].Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        p.Format.HorizontalAlignment = HorizontalAlignment.Center;

                        //Formato de datos
                        TextRange TR = p.AppendText(cabeceraHoras[i]);
                        TR.CharacterFormat.FontName = "Arial";
                        TR.CharacterFormat.FontSize = 9;
                        TR.CharacterFormat.Bold = true;
                        TR.CharacterFormat.TextColor = Color.White;
                    }

                    for (int r = 0; r < cedula.incidencias.transporte.Count; r++)
                    {
                        TableRow DataRow = tablaHoras.Rows[r + 1];
                        //Fila Height
                        DataRow.Height = 5;
                        for (int c = 0; c < cabeceraHoras.Length; c++)
                        {
                            TextRange TR2 = null;
                            //Alineacion de Celdas
                            DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                            //Llenar datos en filas
                            Paragraph p2 = DataRow.Cells[c].AddParagraph();
                            if (c == 0)
                            {
                                TR2 = p2.AppendText(cedula.incidencias.transporte[r].Tipo);
                            }
                            if (c == 1)
                            {
                                TR2 = p2.AppendText(cedula.incidencias.transporte[r].FechaIncidencia.ToString("dd/MM/yyyy"));
                            }
                            if (c == 2)
                            {
                                TR2 = p2.AppendText(cedula.incidencias.transporte[r].Comentarios);
                            }
                            //Formato de celdas
                            p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                            TR2.CharacterFormat.FontName = "Arial";
                            TR2.CharacterFormat.FontSize = 9;
                        }
                    }

                    BookmarksNavigator marcaActividades = new BookmarksNavigator(document);
                    marcaActividades.MoveToBookmark("Comportamiento", true, true);
                    marcaActividades.InsertTable(tablaHoras);
                    document.Replace("||Comportamiento||", "El personal no se comportó de forma cortés, amable y no portó el uniforme e identificación de forma correcta.", false, true);
                }
                else
                {
                    document.Replace("||Comportamiento||", "El personal se comportó de forma cortés, amable y portó el uniforme e identificación de forma correcta.", false, true);
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

                document.Replace("||Administracion||", cedula.inmuebles.Nombre, false, true);

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
                return File(toArray, "application/pdf", "CedulaTransporte_" + cedula.Mes + ".pdf");
            }
            return Redirect("/error/denied");
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

        /*Datos del Modulo*/

        /****Telefonía Celular***/
        [Route("/reporteCelular/{id}")]
        public async Task<IActionResult> ReporteCelular(int id)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), moduloTelefoniaCelular(), "preliminar");

            string strFacturas = "";
            decimal totalFacturas = 0;

            TelefoniaCelular telCel = new TelefoniaCelular();
            telCel = await vCelular.CedulaById(id);
            telCel.usuarios = await vUsuarios.getUserById(telCel.UsuarioId);
            telCel.altaEntrega = await iCelular.ListIncidenciasTipoCelular(telCel.Id, "Alta_Equipo");
            telCel.altasentrega = await iCelular.ListIncidenciasTipoCelular(telCel.Id, "Alta");
            telCel.bajaServicio = await iCelular.ListIncidenciasTipoCelular(telCel.Id, "Baja");
            telCel.reactivacion = await iCelular.ListIncidenciasTipoCelular(telCel.Id, "Reactivacion");
            telCel.suspension = await iCelular.ListIncidenciasTipoCelular(telCel.Id, "Suspension");
            telCel.cambioPerfil = await iCelular.ListIncidenciasTipoCelular(telCel.Id, "Perfil");
            telCel.switcheoCard = await iCelular.ListIncidenciasTipoCelular(telCel.Id, "SIM");
            telCel.cambioRegion = await iCelular.ListIncidenciasTipoCelular(telCel.Id, "CambioNumeroRegion");
            telCel.servicioVozDatos = await iCelular.ListIncidenciasTipoCelular(telCel.Id, "VozDatos");
            telCel.diagnostico = await iCelular.ListIncidenciasTipoCelular(telCel.Id, "Diagnostico");
            telCel.reparacion = await iCelular.ListIncidenciasTipoCelular(telCel.Id, "Reparacion");
            telCel.RespuestasEncuesta = new List<RespuestasEncuesta>();
            telCel.RespuestasEncuesta = await vMensajeria.obtieneRespuestas(id);
            telCel.facturas = new List<Facturas>();
            telCel.facturas = await vFacturas.getFacturas(id, 5);

            Document document = new Document();
            var path = @"E:\Plantillas CASESGV2\DocsV2\ReporteTelCelularValid.docx";
            document.LoadFromFile(path);

            //Creamos la Tabla
            Section tablas = document.AddSection();

            //Resultados Pregunta 1
            if (telCel.altaEntrega.Count > 0)
            {

                Table tablaActividades = tablas.AddTable(true);

                String[] cabeceraActividades = { "Linea", "Perfil", "Fecha Solicitud", "Fecha Atención", "Horas Atención", "Horas Retraso" };

                tablaActividades.ResetCells(telCel.altaEntrega.Count + 1, cabeceraActividades.Length);

                TableRow recRow = tablaActividades.Rows[0];
                recRow.IsHeader = true;
                recRow.Height = 10;

                recRow.RowFormat.BackColor = Color.FromArgb(81, 25, 162);
                recRow.RowFormat.Borders.BorderType = BorderStyle.Single;


                for (int i = 0; i < cabeceraActividades.Length; i++)
                {
                    //Alineacion de celdas
                    Paragraph p = recRow.Cells[i].AddParagraph();
                    tablaActividades.Rows[0].Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Bottom;
                    p.Format.HorizontalAlignment = HorizontalAlignment.Center;

                    //Formato de datos
                    TextRange TR = p.AppendText(cabeceraActividades[i]);
                    TR.CharacterFormat.FontName = "Arial";
                    TR.CharacterFormat.FontSize = 9;
                    TR.CharacterFormat.Bold = true;
                    TR.CharacterFormat.TextColor = Color.White;
                }

                for (int r = 0; r < telCel.altaEntrega.Count; r++)
                {
                    TableRow DataRow = tablaActividades.Rows[r + 1];
                    //Fila Height
                    DataRow.Height = 5;
                    for (int c = 0; c < cabeceraActividades.Length; c++)
                    {
                        TextRange TR2 = null;
                        //Alineacion de Celdas
                        DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        //Llenar datos en filas
                        Paragraph p2 = DataRow.Cells[c].AddParagraph();
                        if (c == 0)
                        {

                            TR2 = p2.AppendText(telCel.altaEntrega[r].Linea);
                        }

                        if (c == 1)
                        {

                            TR2 = p2.AppendText(telCel.altaEntrega[r].perfilesCelular.Nombre);
                        }

                        if (c == 2)
                        {
                            TR2 = p2.AppendText(telCel.altaEntrega[r].FechaSolicitud.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 3)
                        {
                            TR2 = p2.AppendText(telCel.altaEntrega[r].FechaAtencion.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 4)
                        {
                            TR2 = p2.AppendText(telCel.altaEntrega[r].HorasAtencion + "");
                        }

                        if (c == 5)
                        {
                            TR2 = p2.AppendText(telCel.altaEntrega[r].HorasRetraso + "");
                        }

                        //Formato de celdas
                        p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                        TR2.CharacterFormat.FontName = "Arial";
                        TR2.CharacterFormat.FontSize = 9;
                    }
                }
                document.Replace("||Pregunta1||", "Se presentarón altas con entrega de equipo.", false, true);
                BookmarksNavigator marcaActividades = new BookmarksNavigator(document);
                marcaActividades.MoveToBookmark("Pregunta1", true, true);
                marcaActividades.InsertTable(tablaActividades);
            }
            else
            {
                document.Replace("||Pregunta1||", "No se presentarón altas con entrega de equipo.", false, true);
            }

            //Resultados Pregunta 2
            if (telCel.altasentrega.Count > 0)
            {

                Table tablaActividades = tablas.AddTable(true);

                String[] cabeceraActividades = { "Linea", "Perfil", "Fecha Solicitud", "Fecha Atención", "Horas Atención", "Horas Retraso" };

                tablaActividades.ResetCells(telCel.altasentrega.Count + 1, cabeceraActividades.Length);

                TableRow recRow = tablaActividades.Rows[0];
                recRow.IsHeader = true;
                recRow.Height = 10;

                recRow.RowFormat.BackColor = Color.FromArgb(81, 25, 162);
                recRow.RowFormat.Borders.BorderType = BorderStyle.Single;


                for (int i = 0; i < cabeceraActividades.Length; i++)
                {
                    //Alineacion de celdas
                    Paragraph p = recRow.Cells[i].AddParagraph();
                    tablaActividades.Rows[0].Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Bottom;
                    p.Format.HorizontalAlignment = HorizontalAlignment.Center;

                    //Formato de datos
                    TextRange TR = p.AppendText(cabeceraActividades[i]);
                    TR.CharacterFormat.FontName = "Arial";
                    TR.CharacterFormat.FontSize = 9;
                    TR.CharacterFormat.Bold = true;
                    TR.CharacterFormat.TextColor = Color.White;
                }

                for (int r = 0; r < telCel.altasentrega.Count; r++)
                {
                    TableRow DataRow = tablaActividades.Rows[r + 1];
                    //Fila Height
                    DataRow.Height = 5;
                    for (int c = 0; c < cabeceraActividades.Length; c++)
                    {
                        TextRange TR2 = null;
                        //Alineacion de Celdas
                        DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        //Llenar datos en filas
                        Paragraph p2 = DataRow.Cells[c].AddParagraph();
                        if (c == 0)
                        {

                            TR2 = p2.AppendText(telCel.altasentrega[r].Linea);
                        }

                        if (c == 1)
                        {

                            TR2 = p2.AppendText(telCel.altasentrega[r].perfilesCelular.Nombre);
                        }

                        if (c == 2)
                        {
                            TR2 = p2.AppendText(telCel.altasentrega[r].FechaSolicitud.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 3)
                        {
                            TR2 = p2.AppendText(telCel.altasentrega[r].FechaAtencion.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 4)
                        {
                            TR2 = p2.AppendText(telCel.altasentrega[r].HorasAtencion + "");
                        }

                        if (c == 5)
                        {
                            TR2 = p2.AppendText(telCel.altasentrega[r].HorasRetraso + "");
                        }

                        //Formato de celdas
                        p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                        TR2.CharacterFormat.FontName = "Arial";
                        TR2.CharacterFormat.FontSize = 9;
                    }
                }
                document.Replace("||Pregunta2||", "Se presentarón altas sin entrega de equipo.", false, true);
                BookmarksNavigator marcaActividades = new BookmarksNavigator(document);
                marcaActividades.MoveToBookmark("Pregunta2", true, true);
                marcaActividades.InsertTable(tablaActividades);
            }
            else
            {
                document.Replace("||Pregunta2||", "No se presentarón altas sin entrega de equipo.", false, true);
            }

            //Resultados Pregunta 3
            if (telCel.bajaServicio.Count > 0)
            {

                Table tablaActividades3 = tablas.AddTable(true);

                String[] cabeceraActividades = { "Linea", "Perfil", "Fecha Solicitud", "Fecha Atención", "Horas Atención", "Horas Retraso" };

                tablaActividades3.ResetCells(telCel.bajaServicio.Count + 1, cabeceraActividades.Length);

                TableRow recRow = tablaActividades3.Rows[0];
                recRow.IsHeader = true;
                recRow.Height = 10;

                recRow.RowFormat.BackColor = Color.FromArgb(81, 25, 162);
                recRow.RowFormat.Borders.BorderType = BorderStyle.Single;


                for (int i = 0; i < cabeceraActividades.Length; i++)
                {
                    //Alineacion de celdas
                    Paragraph p = recRow.Cells[i].AddParagraph();
                    tablaActividades3.Rows[0].Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Bottom;
                    p.Format.HorizontalAlignment = HorizontalAlignment.Center;

                    //Formato de datos
                    TextRange TR = p.AppendText(cabeceraActividades[i]);
                    TR.CharacterFormat.FontName = "Arial";
                    TR.CharacterFormat.FontSize = 9;
                    TR.CharacterFormat.Bold = true;
                    TR.CharacterFormat.TextColor = Color.White;
                }

                for (int r = 0; r < telCel.bajaServicio.Count; r++)
                {
                    TableRow DataRow = tablaActividades3.Rows[r + 1];
                    //Fila Height
                    DataRow.Height = 5;
                    for (int c = 0; c < cabeceraActividades.Length; c++)
                    {
                        TextRange TR2 = null;
                        //Alineacion de Celdas
                        DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        //Llenar datos en filas
                        Paragraph p2 = DataRow.Cells[c].AddParagraph();
                        if (c == 0)
                        {

                            TR2 = p2.AppendText(telCel.bajaServicio[r].Linea);
                        }

                        if (c == 1)
                        {
                            TR2 = p2.AppendText(telCel.bajaServicio[r].perfilesCelular.Nombre);
                        }

                        if (c == 2)
                        {
                            TR2 = p2.AppendText(telCel.bajaServicio[r].FechaSolicitud.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 3)
                        {
                            TR2 = p2.AppendText(telCel.bajaServicio[r].FechaAtencion.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 4)
                        {
                            TR2 = p2.AppendText(telCel.bajaServicio[r].HorasAtencion + "");
                        }

                        if (c == 5)
                        {
                            TR2 = p2.AppendText(telCel.bajaServicio[r].HorasRetraso + "");
                        }

                        //Formato de celdas
                        p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                        TR2.CharacterFormat.FontName = "Arial";
                        TR2.CharacterFormat.FontSize = 9;
                    }
                }
                document.Replace("||Pregunta3||", "Se presentarón bajas de servicio en este mes.", false, true);
                BookmarksNavigator marcaActividades = new BookmarksNavigator(document);
                marcaActividades.MoveToBookmark("Pregunta3", true, true);
                marcaActividades.InsertTable(tablaActividades3);
            }
            else
            {
                document.Replace("||Pregunta3||", "No se presentarón bajas de servicio en este mes.", false, true);
            }

            //Resultados Pregunta 4
            if (telCel.reactivacion.Count > 0)
            {

                Table tablaActividades3 = tablas.AddTable(true);

                String[] cabeceraActividades = { "Linea", "Perfil", "Fecha Solicitud", "Fecha Atención", "Horas Atención", "Horas Retraso" };

                tablaActividades3.ResetCells(telCel.reactivacion.Count + 1, cabeceraActividades.Length);

                TableRow recRow = tablaActividades3.Rows[0];
                recRow.IsHeader = true;
                recRow.Height = 10;

                recRow.RowFormat.BackColor = Color.FromArgb(81, 25, 162);
                recRow.RowFormat.Borders.BorderType = BorderStyle.Single;


                for (int i = 0; i < cabeceraActividades.Length; i++)
                {
                    //Alineacion de celdas
                    Paragraph p = recRow.Cells[i].AddParagraph();
                    tablaActividades3.Rows[0].Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Bottom;
                    p.Format.HorizontalAlignment = HorizontalAlignment.Center;

                    //Formato de datos
                    TextRange TR = p.AppendText(cabeceraActividades[i]);
                    TR.CharacterFormat.FontName = "Arial";
                    TR.CharacterFormat.FontSize = 9;
                    TR.CharacterFormat.Bold = true;
                    TR.CharacterFormat.TextColor = Color.White;
                }

                for (int r = 0; r < telCel.reactivacion.Count; r++)
                {
                    TableRow DataRow = tablaActividades3.Rows[r + 1];
                    //Fila Height
                    DataRow.Height = 5;
                    for (int c = 0; c < cabeceraActividades.Length; c++)
                    {
                        TextRange TR2 = null;
                        //Alineacion de Celdas
                        DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        //Llenar datos en filas
                        Paragraph p2 = DataRow.Cells[c].AddParagraph();
                        if (c == 0)
                        {

                            TR2 = p2.AppendText(telCel.reactivacion[r].Linea);
                        }

                        if (c == 1)
                        {
                            TR2 = p2.AppendText(telCel.reactivacion[r].perfilesCelular.Nombre);
                        }

                        if (c == 2)
                        {
                            TR2 = p2.AppendText(telCel.reactivacion[r].FechaSolicitud.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 3)
                        {
                            TR2 = p2.AppendText(telCel.reactivacion[r].FechaAtencion.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 4)
                        {
                            TR2 = p2.AppendText(telCel.reactivacion[r].HorasAtencion + "");
                        }

                        if (c == 5)
                        {
                            TR2 = p2.AppendText(telCel.reactivacion[r].HorasRetraso + "");
                        }

                        //Formato de celdas
                        p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                        TR2.CharacterFormat.FontName = "Arial";
                        TR2.CharacterFormat.FontSize = 9;
                    }
                }
                document.Replace("||Pregunta4||", "Se presentarón reactivaciones por robo/extravío en este mes.", false, true);
                BookmarksNavigator marcaActividades = new BookmarksNavigator(document);
                marcaActividades.MoveToBookmark("Pregunta4", true, true);
                marcaActividades.InsertTable(tablaActividades3);
            }
            else
            {
                document.Replace("||Pregunta4||", "No se presentarón reactivaciones por robo/extravío en este mes.", false, true);
            }

            //Resultados Pregunta 5
            if (telCel.suspension.Count > 0)
            {

                Table tablaActividades3 = tablas.AddTable(true);

                String[] cabeceraActividades = { "Linea", "Perfil", "Fecha Solicitud", "Fecha Atención", "Horas Atención", "Horas Retraso" };

                tablaActividades3.ResetCells(telCel.suspension.Count + 1, cabeceraActividades.Length);

                TableRow recRow = tablaActividades3.Rows[0];
                recRow.IsHeader = true;
                recRow.Height = 10;

                recRow.RowFormat.BackColor = Color.FromArgb(81, 25, 162);
                recRow.RowFormat.Borders.BorderType = BorderStyle.Single;


                for (int i = 0; i < cabeceraActividades.Length; i++)
                {
                    //Alineacion de celdas
                    Paragraph p = recRow.Cells[i].AddParagraph();
                    tablaActividades3.Rows[0].Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Bottom;
                    p.Format.HorizontalAlignment = HorizontalAlignment.Center;

                    //Formato de datos
                    TextRange TR = p.AppendText(cabeceraActividades[i]);
                    TR.CharacterFormat.FontName = "Arial";
                    TR.CharacterFormat.FontSize = 9;
                    TR.CharacterFormat.Bold = true;
                    TR.CharacterFormat.TextColor = Color.White;
                }

                for (int r = 0; r < telCel.suspension.Count; r++)
                {
                    TableRow DataRow = tablaActividades3.Rows[r + 1];
                    //Fila Height
                    DataRow.Height = 5;
                    for (int c = 0; c < cabeceraActividades.Length; c++)
                    {
                        TextRange TR2 = null;
                        //Alineacion de Celdas
                        DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        //Llenar datos en filas
                        Paragraph p2 = DataRow.Cells[c].AddParagraph();
                        if (c == 0)
                        {

                            TR2 = p2.AppendText(telCel.suspension[r].Linea);
                        }

                        if (c == 1)
                        {

                            TR2 = p2.AppendText(telCel.suspension[r].perfilesCelular.Nombre);
                        }

                        if (c == 2)
                        {
                            TR2 = p2.AppendText(telCel.suspension[r].FechaSolicitud.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 3)
                        {
                            TR2 = p2.AppendText(telCel.suspension[r].FechaAtencion.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 4)
                        {
                            TR2 = p2.AppendText(telCel.suspension[r].HorasAtencion + "");
                        }

                        if (c == 5)
                        {
                            TR2 = p2.AppendText(telCel.suspension[r].HorasRetraso + "");
                        }

                        //Formato de celdas
                        p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                        TR2.CharacterFormat.FontName = "Arial";
                        TR2.CharacterFormat.FontSize = 9;
                    }
                }
                document.Replace("||Pregunta5||", "Se presentarón suspensiones por robo/extravío en este mes.", false, true);
                BookmarksNavigator marcaActividades = new BookmarksNavigator(document);
                marcaActividades.MoveToBookmark("Pregunta5", true, true);
                marcaActividades.InsertTable(tablaActividades3);
            }
            else
            {
                document.Replace("||Pregunta5||", "No se presentarón suspensiones por robo/extravío en este mes.", false, true);
            }

            //Resultados Pregunta 6
            if (telCel.cambioPerfil.Count > 0)
            {

                Table tablaActividades3 = tablas.AddTable(true);

                String[] cabeceraActividades = { "Linea", "Perfil", "Fecha Solicitud", "Fecha Atención", "Horas Atención", "Horas Retraso" };

                tablaActividades3.ResetCells(telCel.cambioPerfil.Count + 1, cabeceraActividades.Length);

                TableRow recRow = tablaActividades3.Rows[0];
                recRow.IsHeader = true;
                recRow.Height = 10;

                recRow.RowFormat.BackColor = Color.FromArgb(81, 25, 162);
                recRow.RowFormat.Borders.BorderType = BorderStyle.Single;


                for (int i = 0; i < cabeceraActividades.Length; i++)
                {
                    //Alineacion de celdas
                    Paragraph p = recRow.Cells[i].AddParagraph();
                    tablaActividades3.Rows[0].Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Bottom;
                    p.Format.HorizontalAlignment = HorizontalAlignment.Center;

                    //Formato de datos
                    TextRange TR = p.AppendText(cabeceraActividades[i]);
                    TR.CharacterFormat.FontName = "Arial";
                    TR.CharacterFormat.FontSize = 9;
                    TR.CharacterFormat.Bold = true;
                    TR.CharacterFormat.TextColor = Color.White;
                }

                for (int r = 0; r < telCel.cambioPerfil.Count; r++)
                {
                    TableRow DataRow = tablaActividades3.Rows[r + 1];
                    //Fila Height
                    DataRow.Height = 5;
                    for (int c = 0; c < cabeceraActividades.Length; c++)
                    {
                        TextRange TR2 = null;
                        //Alineacion de Celdas
                        DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        //Llenar datos en filas
                        Paragraph p2 = DataRow.Cells[c].AddParagraph();
                        if (c == 0)
                        {

                            TR2 = p2.AppendText(telCel.cambioPerfil[r].Linea);
                        }

                        if (c == 1)
                        {

                            TR2 = p2.AppendText(telCel.cambioPerfil[r].perfilesCelular.Nombre);
                        }

                        if (c == 2)
                        {
                            TR2 = p2.AppendText(telCel.cambioPerfil[r].FechaSolicitud.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 3)
                        {
                            TR2 = p2.AppendText(telCel.cambioPerfil[r].FechaAtencion.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 4)
                        {
                            TR2 = p2.AppendText(telCel.cambioPerfil[r].HorasAtencion + "");
                        }

                        if (c == 5)
                        {
                            TR2 = p2.AppendText(telCel.cambioPerfil[r].HorasRetraso + "");
                        }

                        //Formato de celdas
                        p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                        TR2.CharacterFormat.FontName = "Arial";
                        TR2.CharacterFormat.FontSize = 9;
                    }
                }
                document.Replace("||Pregunta6||", "Se presentarón  en este mes.", false, true);
                BookmarksNavigator marcaActividades = new BookmarksNavigator(document);
                marcaActividades.MoveToBookmark("Pregunta6", true, true);
                marcaActividades.InsertTable(tablaActividades3);
            }
            else
            {
                document.Replace("||Pregunta6||", "No hubo cambios de Perfil en la evaluación de este mes.", false, true);
            }

            //Resultados Pregunta 7
            if (telCel.switcheoCard.Count > 0)
            {

                Table tablaActividades3 = tablas.AddTable(true);

                String[] cabeceraActividades = { "Linea", "Perfil", "Fecha Solicitud", "Fecha Atención", "Horas Atención", "Horas Retraso" };

                tablaActividades3.ResetCells(telCel.switcheoCard.Count + 1, cabeceraActividades.Length);

                TableRow recRow = tablaActividades3.Rows[0];
                recRow.IsHeader = true;
                recRow.Height = 10;

                recRow.RowFormat.BackColor = Color.FromArgb(81, 25, 162);
                recRow.RowFormat.Borders.BorderType = BorderStyle.Single;


                for (int i = 0; i < cabeceraActividades.Length; i++)
                {
                    //Alineacion de celdas
                    Paragraph p = recRow.Cells[i].AddParagraph();
                    tablaActividades3.Rows[0].Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Bottom;
                    p.Format.HorizontalAlignment = HorizontalAlignment.Center;

                    //Formato de datos
                    TextRange TR = p.AppendText(cabeceraActividades[i]);
                    TR.CharacterFormat.FontName = "Arial";
                    TR.CharacterFormat.FontSize = 9;
                    TR.CharacterFormat.Bold = true;
                    TR.CharacterFormat.TextColor = Color.White;
                }

                for (int r = 0; r < telCel.switcheoCard.Count; r++)
                {
                    TableRow DataRow = tablaActividades3.Rows[r + 1];
                    //Fila Height
                    DataRow.Height = 5;
                    for (int c = 0; c < cabeceraActividades.Length; c++)
                    {
                        TextRange TR2 = null;
                        //Alineacion de Celdas
                        DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        //Llenar datos en filas
                        Paragraph p2 = DataRow.Cells[c].AddParagraph();
                        if (c == 0)
                        {

                            TR2 = p2.AppendText(telCel.switcheoCard[r].Linea);
                        }

                        if (c == 1)
                        {

                            TR2 = p2.AppendText(telCel.switcheoCard[r].perfilesCelular.Nombre);
                        }

                        if (c == 2)
                        {
                            TR2 = p2.AppendText(telCel.switcheoCard[r].FechaSolicitud.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 3)
                        {
                            TR2 = p2.AppendText(telCel.switcheoCard[r].FechaAtencion.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 4)
                        {
                            TR2 = p2.AppendText(telCel.switcheoCard[r].HorasAtencion + "");
                        }

                        if (c == 5)
                        {
                            TR2 = p2.AppendText(telCel.switcheoCard[r].HorasRetraso + "");
                        }

                        //Formato de celdas
                        p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                        TR2.CharacterFormat.FontName = "Arial";
                        TR2.CharacterFormat.FontSize = 9;
                    }
                }
                document.Replace("||Pregunta7||", "Se presentarón switcheos de SIM Card en este mes.", false, true);
                BookmarksNavigator marcaActividades = new BookmarksNavigator(document);
                marcaActividades.MoveToBookmark("Pregunta7", true, true);
                marcaActividades.InsertTable(tablaActividades3);
            }
            else
            {
                document.Replace("||Pregunta7||", "No se presentarón switcheos de SIM Card en este mes.", false, true);
            }

            //Resultados Pregunta 8
            if (telCel.cambioRegion.Count > 0)
            {

                Table tablaActividades3 = tablas.AddTable(true);

                String[] cabeceraActividades = { "Linea", "Perfil", "Fecha Solicitud", "Fecha Atención", "Horas Atención", "Horas Retraso" };

                tablaActividades3.ResetCells(telCel.cambioRegion.Count + 1, cabeceraActividades.Length);

                TableRow recRow = tablaActividades3.Rows[0];
                recRow.IsHeader = true;
                recRow.Height = 10;

                recRow.RowFormat.BackColor = Color.FromArgb(81, 25, 162);
                recRow.RowFormat.Borders.BorderType = BorderStyle.Single;


                for (int i = 0; i < cabeceraActividades.Length; i++)
                {
                    //Alineacion de celdas
                    Paragraph p = recRow.Cells[i].AddParagraph();
                    tablaActividades3.Rows[0].Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Bottom;
                    p.Format.HorizontalAlignment = HorizontalAlignment.Center;

                    //Formato de datos
                    TextRange TR = p.AppendText(cabeceraActividades[i]);
                    TR.CharacterFormat.FontName = "Arial";
                    TR.CharacterFormat.FontSize = 9;
                    TR.CharacterFormat.Bold = true;
                    TR.CharacterFormat.TextColor = Color.White;
                }

                for (int r = 0; r < telCel.cambioRegion.Count; r++)
                {
                    TableRow DataRow = tablaActividades3.Rows[r + 1];
                    //Fila Height
                    DataRow.Height = 5;
                    for (int c = 0; c < cabeceraActividades.Length; c++)
                    {
                        TextRange TR2 = null;
                        //Alineacion de Celdas
                        DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        //Llenar datos en filas
                        Paragraph p2 = DataRow.Cells[c].AddParagraph();
                        if (c == 0)
                        {

                            TR2 = p2.AppendText(telCel.cambioRegion[r].Linea);
                        }

                        if (c == 1)
                        {

                            TR2 = p2.AppendText(telCel.cambioRegion[r].perfilesCelular.Nombre);
                        }

                        if (c == 2)
                        {
                            TR2 = p2.AppendText(telCel.cambioRegion[r].FechaSolicitud.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 3)
                        {
                            TR2 = p2.AppendText(telCel.cambioRegion[r].FechaAtencion.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 4)
                        {
                            TR2 = p2.AppendText(telCel.cambioRegion[r].HorasAtencion + "");
                        }

                        if (c == 5)
                        {
                            TR2 = p2.AppendText(telCel.cambioRegion[r].HorasRetraso + "");
                        }

                        //Formato de celdas
                        p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                        TR2.CharacterFormat.FontName = "Arial";
                        TR2.CharacterFormat.FontSize = 9;
                    }
                }
                document.Replace("||Pregunta8||", "Se presentarón cambios de número o región en este mes.", false, true);
                BookmarksNavigator marcaActividades = new BookmarksNavigator(document);
                marcaActividades.MoveToBookmark("Pregunta8", true, true);
                marcaActividades.InsertTable(tablaActividades3);
            }
            else
            {
                document.Replace("||Pregunta8||", "No se presentarón cambios de número o región en este mes.", false, true);
            }

            //Resultados Pregunta 9
            if (telCel.servicioVozDatos.Count > 0)
            {

                Table tablaActividades3 = tablas.AddTable(true);

                String[] cabeceraActividades = { "Linea", "Perfil", "Fecha Solicitud", "Fecha Atención", "Horas Atención", "Horas Retraso" };

                tablaActividades3.ResetCells(telCel.servicioVozDatos.Count + 1, cabeceraActividades.Length);

                TableRow recRow = tablaActividades3.Rows[0];
                recRow.IsHeader = true;
                recRow.Height = 10;

                recRow.RowFormat.BackColor = Color.FromArgb(81, 25, 162);
                recRow.RowFormat.Borders.BorderType = BorderStyle.Single;


                for (int i = 0; i < cabeceraActividades.Length; i++)
                {
                    //Alineacion de celdas
                    Paragraph p = recRow.Cells[i].AddParagraph();
                    tablaActividades3.Rows[0].Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Bottom;
                    p.Format.HorizontalAlignment = HorizontalAlignment.Center;

                    //Formato de datos
                    TextRange TR = p.AppendText(cabeceraActividades[i]);
                    TR.CharacterFormat.FontName = "Arial";
                    TR.CharacterFormat.FontSize = 9;
                    TR.CharacterFormat.Bold = true;
                    TR.CharacterFormat.TextColor = Color.White;
                }

                for (int r = 0; r < telCel.servicioVozDatos.Count; r++)
                {
                    TableRow DataRow = tablaActividades3.Rows[r + 1];
                    //Fila Height
                    DataRow.Height = 5;
                    for (int c = 0; c < cabeceraActividades.Length; c++)
                    {
                        TextRange TR2 = null;
                        //Alineacion de Celdas
                        DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        //Llenar datos en filas
                        Paragraph p2 = DataRow.Cells[c].AddParagraph();
                        if (c == 0)
                        {

                            TR2 = p2.AppendText(telCel.servicioVozDatos[r].Linea);
                        }

                        if (c == 1)
                        {

                            TR2 = p2.AppendText(telCel.servicioVozDatos[r].perfilesCelular.Nombre);
                        }

                        if (c == 2)
                        {
                            TR2 = p2.AppendText(telCel.servicioVozDatos[r].FechaSolicitud.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 3)
                        {
                            TR2 = p2.AppendText(telCel.servicioVozDatos[r].FechaAtencion.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 4)
                        {
                            TR2 = p2.AppendText(telCel.servicioVozDatos[r].HorasAtencion + "");
                        }

                        if (c == 5)
                        {
                            TR2 = p2.AppendText(telCel.servicioVozDatos[r].HorasRetraso + "");
                        }

                        //Formato de celdas
                        p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                        TR2.CharacterFormat.FontName = "Arial";
                        TR2.CharacterFormat.FontSize = 9;
                    }
                }
                document.Replace("||Pregunta9||", "Se presentarón  en este mes.", false, true);
                BookmarksNavigator marcaActividades = new BookmarksNavigator(document);
                marcaActividades.MoveToBookmark("Pregunta9", true, true);
                marcaActividades.InsertTable(tablaActividades3);
            }
            else
            {
                document.Replace("||Pregunta9||", "No se presentarón solicitudes de voz y/o datos en este mes.", false, true);
            }

            //Resultados Pregunta 10
            if (telCel.diagnostico.Count > 0)
            {

                Table tablaActividades3 = tablas.AddTable(true);

                String[] cabeceraActividades = { "Linea", "Perfil", "Fecha Solicitud", "Fecha Atención", "Horas Atención", "Horas Retraso" };

                tablaActividades3.ResetCells(telCel.diagnostico.Count + 1, cabeceraActividades.Length);

                TableRow recRow = tablaActividades3.Rows[0];
                recRow.IsHeader = true;
                recRow.Height = 10;

                recRow.RowFormat.BackColor = Color.FromArgb(81, 25, 162);
                recRow.RowFormat.Borders.BorderType = BorderStyle.Single;


                for (int i = 0; i < cabeceraActividades.Length; i++)
                {
                    //Alineacion de celdas
                    Paragraph p = recRow.Cells[i].AddParagraph();
                    tablaActividades3.Rows[0].Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Bottom;
                    p.Format.HorizontalAlignment = HorizontalAlignment.Center;

                    //Formato de datos
                    TextRange TR = p.AppendText(cabeceraActividades[i]);
                    TR.CharacterFormat.FontName = "Arial";
                    TR.CharacterFormat.FontSize = 9;
                    TR.CharacterFormat.Bold = true;
                    TR.CharacterFormat.TextColor = Color.White;
                }

                for (int r = 0; r < telCel.diagnostico.Count; r++)
                {
                    TableRow DataRow = tablaActividades3.Rows[r + 1];
                    //Fila Height
                    DataRow.Height = 5;
                    for (int c = 0; c < cabeceraActividades.Length; c++)
                    {
                        TextRange TR2 = null;
                        //Alineacion de Celdas
                        DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        //Llenar datos en filas
                        Paragraph p2 = DataRow.Cells[c].AddParagraph();
                        if (c == 0)
                        {

                            TR2 = p2.AppendText(telCel.diagnostico[r].Linea);
                        }

                        if (c == 1)
                        {

                            TR2 = p2.AppendText(telCel.diagnostico[r].perfilesCelular.Nombre);
                        }

                        if (c == 2)
                        {
                            TR2 = p2.AppendText(telCel.diagnostico[r].FechaSolicitud.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 3)
                        {
                            TR2 = p2.AppendText(telCel.diagnostico[r].FechaAtencion.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 4)
                        {
                            TR2 = p2.AppendText(telCel.diagnostico[r].HorasAtencion + "");
                        }

                        if (c == 5)
                        {
                            TR2 = p2.AppendText(telCel.diagnostico[r].HorasRetraso + "");
                        }

                        //Formato de celdas
                        p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                        TR2.CharacterFormat.FontName = "Arial";
                        TR2.CharacterFormat.FontSize = 9;
                    }
                }
                document.Replace("||Pregunta10||", "Se presentarón  en este mes.", false, true);
                BookmarksNavigator marcaActividades = new BookmarksNavigator(document);
                marcaActividades.MoveToBookmark("Pregunta10", true, true);
                marcaActividades.InsertTable(tablaActividades3);
            }
            else
            {
                document.Replace("||Pregunta10||", "No se presentarón solicitudes de diagnóstico de equipo en este mes.", false, true);
            }

            //Resultados Pregunta 11
            if (telCel.reparacion.Count > 0)
            {

                Table tablaActividades3 = tablas.AddTable(true);

                String[] cabeceraActividades = { "Linea", "Perfil", "Fecha Solicitud", "Fecha Atención", "Horas Atención", "Horas Retraso" };

                tablaActividades3.ResetCells(telCel.reparacion.Count + 1, cabeceraActividades.Length);

                TableRow recRow = tablaActividades3.Rows[0];
                recRow.IsHeader = true;
                recRow.Height = 10;

                recRow.RowFormat.BackColor = Color.FromArgb(81, 25, 162);
                recRow.RowFormat.Borders.BorderType = BorderStyle.Single;


                for (int i = 0; i < cabeceraActividades.Length; i++)
                {
                    //Alineacion de celdas
                    Paragraph p = recRow.Cells[i].AddParagraph();
                    tablaActividades3.Rows[0].Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Bottom;
                    p.Format.HorizontalAlignment = HorizontalAlignment.Center;

                    //Formato de datos
                    TextRange TR = p.AppendText(cabeceraActividades[i]);
                    TR.CharacterFormat.FontName = "Arial";
                    TR.CharacterFormat.FontSize = 9;
                    TR.CharacterFormat.Bold = true;
                    TR.CharacterFormat.TextColor = Color.White;
                }

                for (int r = 0; r < telCel.reparacion.Count; r++)
                {
                    TableRow DataRow = tablaActividades3.Rows[r + 1];
                    //Fila Height
                    DataRow.Height = 5;
                    for (int c = 0; c < cabeceraActividades.Length; c++)
                    {
                        TextRange TR2 = null;
                        //Alineacion de Celdas
                        DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        //Llenar datos en filas
                        Paragraph p2 = DataRow.Cells[c].AddParagraph();
                        if (c == 0)
                        {

                            TR2 = p2.AppendText(telCel.reparacion[r].Linea);
                        }

                        if (c == 1)
                        {

                            TR2 = p2.AppendText(telCel.reparacion[r].perfilesCelular.Nombre);
                        }

                        if (c == 2)
                        {
                            TR2 = p2.AppendText(telCel.reparacion[r].FechaSolicitud.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 3)
                        {
                            TR2 = p2.AppendText(telCel.reparacion[r].FechaAtencion.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 4)
                        {
                            TR2 = p2.AppendText(telCel.reparacion[r].HorasAtencion + "");
                        }

                        if (c == 5)
                        {
                            TR2 = p2.AppendText(telCel.reparacion[r].HorasRetraso + "");
                        }

                        //Formato de celdas
                        p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                        TR2.CharacterFormat.FontName = "Arial";
                        TR2.CharacterFormat.FontSize = 9;
                    }
                }
                document.Replace("||Pregunta11||", "Se presentarón solicitudes de reparación de equipo en este mes.", false, true);
                BookmarksNavigator marcaActividades = new BookmarksNavigator(document);
                marcaActividades.MoveToBookmark("Pregunta11", true, true);
                marcaActividades.InsertTable(tablaActividades3);
            }
            else
            {
                document.Replace("||Pregunta11||", "No se presentarón  en este mes.", false, true);
            }



            document.Replace("||Folio||", telCel.Folio, false, true);

            document.Replace("||Mes||", telCel.Mes, false, true);

            document.Replace("||C||", telCel.Calificacion.ToString(), false, true);

            document.Replace("||dia||", telCel.FechaCreacion.GetValueOrDefault().Day + "", false, true);
            document.Replace("||MesE||", Convert.ToDateTime(telCel.FechaCreacion).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")), false, true);
            document.Replace("||Anio||", Convert.ToDateTime(telCel.FechaCreacion.GetValueOrDefault()).Year + "", false, true);


            for (int i = 0; i < telCel.facturas.Count; i++)
            {
                if ((telCel.facturas.Count - 1) != i)
                {
                    strFacturas += telCel.facturas[i].comprobante.Serie + telCel.facturas[i].comprobante.Folio + "/";
                }
                else
                {
                    strFacturas += telCel.facturas[i].comprobante.Serie + telCel.facturas[i].comprobante.Folio;
                }
            }

            document.Replace("||Factura||", strFacturas, false, true);

            document.Replace("||Total||", String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:C}", vFacturas.obtieneTotalFacturas(telCel.facturas)) + "", false, true);

            if (!telCel.Estatus.Equals("Aceptada"))
            {
                BookmarksNavigator marcaNota = new BookmarksNavigator(document);
                marcaNota.MoveToBookmark("Nota", true, true);
                marcaNota.InsertText("NOTA: Esta cédula no cuenta con ningún valor ya que aún no se AUTORIZA por parte de la Dirección de Administración de Servicios.");
            }


            //Salvar y Lanzar

            byte[] toArray = null;
            using (MemoryStream ms1 = new MemoryStream())
            {
                document.SaveToStream(ms1, Spire.Doc.FileFormat.PDF);
                toArray = ms1.ToArray();
            }
            return File(toArray, "application/pdf", "CedulaCelular_" + telCel.Mes + ".pdf");
        }

        /****Telefonía Celular***/
        [Route("/reporteConvencional/{id}")]
        public async Task<IActionResult> ReporteConvencional(int id)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), moduloTelefoniaConvencional(), "preliminar");

            string strFacturas = "";
            decimal totalFacturas = 0;

            TelefoniaConvencional telCon = new TelefoniaConvencional();
            telCon = await vConvencional.CedulaById(id);
            telCon.usuarios = await vUsuarios.getUserById(telCon.UsuarioId);
            telCon.contratacion = await iConvencional.ListIncidenciasTipoConvencional(telCon.Id, "contratacion_instalacion");
            telCon.cableado = await iConvencional.ListIncidenciasTipoConvencional(telCon.Id, "cableado");
            telCon.entregaAparato = await iConvencional.ListIncidenciasTipoConvencional(telCon.Id, "entregaAparato");
            telCon.cambioDomicilio = await iConvencional.ListIncidenciasTipoConvencional(telCon.Id, "cambioDomicilio");
            telCon.reubicacion = await iConvencional.ListIncidenciasTipoConvencional(telCon.Id, "reubicacion");
            telCon.identificador = await iConvencional.ListIncidenciasTipoConvencional(telCon.Id, "identificadorLlamadas");
            telCon.instalaciónTroncal = await iConvencional.ListIncidenciasTipoConvencional(telCon.Id, "troncales");
            telCon.contratacionInternet = await iConvencional.ListIncidenciasTipoConvencional(telCon.Id, "internet");
            telCon.habilitacionServicios = await iConvencional.ListIncidenciasTipoConvencional(telCon.Id, "serviciosTelefonia");
            telCon.cancelacionServicios = await iConvencional.ListIncidenciasTipoConvencional(telCon.Id, "cancelacion");
            telCon.reporteFallas = await iConvencional.ListIncidenciasTipoConvencional(telCon.Id, "reportesFallas");
            telCon.RespuestasEncuesta = new List<RespuestasEncuesta>();
            telCon.RespuestasEncuesta = await vMensajeria.obtieneRespuestas(id);
            telCon.facturas = new List<Facturas>();
            telCon.facturas = await vFacturas.getFacturas(id, 6);

            Document document = new Document();
            var path = @"E:\Plantillas CASESGV2\DocsV2\ReporteTelConvencionalValid.docx";
            document.LoadFromFile(path);

            //Creamos la Tabla
            Section tablas = document.AddSection();

            //Resultados Pregunta 1
            if (telCon.contratacion.Count > 0)
            {

                Table tablaActividades = tablas.AddTable(true);

                String[] cabeceraActividades = { "Linea", "Fecha Solicitud", "Fecha Atención", "Horas Atención", "Horas Retraso" };

                tablaActividades.ResetCells(telCon.contratacion.Count + 1, cabeceraActividades.Length);

                TableRow recRow = tablaActividades.Rows[0];
                recRow.IsHeader = true;
                recRow.Height = 10;

                recRow.RowFormat.BackColor = Color.FromArgb(81, 25, 162);
                recRow.RowFormat.Borders.BorderType = BorderStyle.Single;


                for (int i = 0; i < cabeceraActividades.Length; i++)
                {
                    //Alineacion de celdas
                    Paragraph p = recRow.Cells[i].AddParagraph();
                    tablaActividades.Rows[0].Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Bottom;
                    p.Format.HorizontalAlignment = HorizontalAlignment.Center;

                    //Formato de datos
                    TextRange TR = p.AppendText(cabeceraActividades[i]);
                    TR.CharacterFormat.FontName = "Arial";
                    TR.CharacterFormat.FontSize = 9;
                    TR.CharacterFormat.Bold = true;
                    TR.CharacterFormat.TextColor = Color.White;
                }

                for (int r = 0; r < telCon.contratacion.Count; r++)
                {
                    TableRow DataRow = tablaActividades.Rows[r + 1];
                    //Fila Height
                    DataRow.Height = 5;
                    for (int c = 0; c < cabeceraActividades.Length; c++)
                    {
                        TextRange TR2 = null;
                        //Alineacion de Celdas
                        DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        //Llenar datos en filas
                        Paragraph p2 = DataRow.Cells[c].AddParagraph();
                        if (c == 0)
                        {

                            TR2 = p2.AppendText(telCon.contratacion[r].Linea);
                        }

                        if (c == 1)
                        {
                            TR2 = p2.AppendText(telCon.contratacion[r].FechaSolicitud.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 2)
                        {
                            TR2 = p2.AppendText(telCon.contratacion[r].FechaAtencion.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 3)
                        {
                            TR2 = p2.AppendText(telCon.contratacion[r].DiasAtencion + "");
                        }

                        if (c == 4)
                        {
                            TR2 = p2.AppendText(telCon.contratacion[r].DiasRetraso + "");
                        }

                        //Formato de celdas
                        p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                        TR2.CharacterFormat.FontName = "Arial";
                        TR2.CharacterFormat.FontSize = 9;
                    }
                }
                document.Replace("||Pregunta1||", "Se realizaron nuevas contrataciones en este mes.", false, true);
                BookmarksNavigator marcaActividades = new BookmarksNavigator(document);
                marcaActividades.MoveToBookmark("Pregunta1", true, true);
                marcaActividades.InsertTable(tablaActividades);
            }
            else
            {
                document.Replace("||Pregunta1||", "No se realizaron nuevas contrataciones en este mes.", false, true);
            }

            //Resultados Pregunta 2
            if (telCon.cableado.Count > 0)
            {

                Table tablaActividades = tablas.AddTable(true);

                String[] cabeceraActividades = { "Linea", "Fecha Solicitud", "Fecha Atención", "Horas Atención", "Horas Retraso" };

                tablaActividades.ResetCells(telCon.cableado.Count + 1, cabeceraActividades.Length);

                TableRow recRow = tablaActividades.Rows[0];
                recRow.IsHeader = true;
                recRow.Height = 10;

                recRow.RowFormat.BackColor = Color.FromArgb(81, 25, 162);
                recRow.RowFormat.Borders.BorderType = BorderStyle.Single;


                for (int i = 0; i < cabeceraActividades.Length; i++)
                {
                    //Alineacion de celdas
                    Paragraph p = recRow.Cells[i].AddParagraph();
                    tablaActividades.Rows[0].Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Bottom;
                    p.Format.HorizontalAlignment = HorizontalAlignment.Center;

                    //Formato de datos
                    TextRange TR = p.AppendText(cabeceraActividades[i]);
                    TR.CharacterFormat.FontName = "Arial";
                    TR.CharacterFormat.FontSize = 9;
                    TR.CharacterFormat.Bold = true;
                    TR.CharacterFormat.TextColor = Color.White;
                }

                for (int r = 0; r < telCon.cableado.Count; r++)
                {
                    TableRow DataRow = tablaActividades.Rows[r + 1];
                    //Fila Height
                    DataRow.Height = 5;
                    for (int c = 0; c < cabeceraActividades.Length; c++)
                    {
                        TextRange TR2 = null;
                        //Alineacion de Celdas
                        DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        //Llenar datos en filas
                        Paragraph p2 = DataRow.Cells[c].AddParagraph();
                        if (c == 0)
                        {

                            TR2 = p2.AppendText(telCon.cableado[r].Linea);
                        }

                        if (c == 1)
                        {
                            TR2 = p2.AppendText(telCon.cableado[r].FechaSolicitud.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 2)
                        {
                            TR2 = p2.AppendText(telCon.cableado[r].FechaAtencion.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 3)
                        {
                            TR2 = p2.AppendText(telCon.cableado[r].DiasAtencion + "");
                        }

                        if (c == 4)
                        {
                            TR2 = p2.AppendText(telCon.cableado[r].DiasRetraso + "");
                        }

                        //Formato de celdas
                        p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                        TR2.CharacterFormat.FontName = "Arial";
                        TR2.CharacterFormat.FontSize = 9;
                    }
                }
                document.Replace("||Pregunta2||", "Se realizaron cableados interiores para la instalación de líneas telefónicas.", false, true);
                BookmarksNavigator marcaActividades = new BookmarksNavigator(document);
                marcaActividades.MoveToBookmark("Pregunta2", true, true);
                marcaActividades.InsertTable(tablaActividades);
            }
            else
            {
                document.Replace("||Pregunta2||", "No se realizaron cableados interiores para la instalación de líneas telefónicas.", false, true);
            }

            //Resultados Pregunta 3
            if (telCon.entregaAparato.Count > 0)
            {

                Table tablaActividades3 = tablas.AddTable(true);

                String[] cabeceraActividades = { "Linea", "Fecha Solicitud", "Fecha Atención", "Horas Atención", "Horas Retraso" };

                tablaActividades3.ResetCells(telCon.entregaAparato.Count + 1, cabeceraActividades.Length);

                TableRow recRow = tablaActividades3.Rows[0];
                recRow.IsHeader = true;
                recRow.Height = 10;

                recRow.RowFormat.BackColor = Color.FromArgb(81, 25, 162);
                recRow.RowFormat.Borders.BorderType = BorderStyle.Single;


                for (int i = 0; i < cabeceraActividades.Length; i++)
                {
                    //Alineacion de celdas
                    Paragraph p = recRow.Cells[i].AddParagraph();
                    tablaActividades3.Rows[0].Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Bottom;
                    p.Format.HorizontalAlignment = HorizontalAlignment.Center;

                    //Formato de datos
                    TextRange TR = p.AppendText(cabeceraActividades[i]);
                    TR.CharacterFormat.FontName = "Arial";
                    TR.CharacterFormat.FontSize = 9;
                    TR.CharacterFormat.Bold = true;
                    TR.CharacterFormat.TextColor = Color.White;
                }

                for (int r = 0; r < telCon.entregaAparato.Count; r++)
                {
                    TableRow DataRow = tablaActividades3.Rows[r + 1];
                    //Fila Height
                    DataRow.Height = 5;
                    for (int c = 0; c < cabeceraActividades.Length; c++)
                    {
                        TextRange TR2 = null;
                        //Alineacion de Celdas
                        DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        //Llenar datos en filas
                        Paragraph p2 = DataRow.Cells[c].AddParagraph();
                        if (c == 0)
                        {

                            TR2 = p2.AppendText(telCon.entregaAparato[r].Linea);
                        }

                        if (c == 1)
                        {
                            TR2 = p2.AppendText(telCon.entregaAparato[r].FechaSolicitud.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 2)
                        {
                            TR2 = p2.AppendText(telCon.entregaAparato[r].FechaAtencion.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 3)
                        {
                            TR2 = p2.AppendText(telCon.entregaAparato[r].DiasAtencion + "");
                        }

                        if (c == 4)
                        {
                            TR2 = p2.AppendText(telCon.entregaAparato[r].DiasRetraso + "");
                        }

                        //Formato de celdas
                        p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                        TR2.CharacterFormat.FontName = "Arial";
                        TR2.CharacterFormat.FontSize = 9;
                    }
                }
                document.Replace("||Pregunta3||", "Se realizaron entregas de aparatos telefónicos en este mes.", false, true);
                BookmarksNavigator marcaActividades = new BookmarksNavigator(document);
                marcaActividades.MoveToBookmark("Pregunta3", true, true);
                marcaActividades.InsertTable(tablaActividades3);
            }
            else
            {
                document.Replace("||Pregunta3||", "No se realizaron entregas de aparatos telefónicos en este mes.", false, true);
            }

            //Resultados Pregunta 4
            if (telCon.cambioDomicilio.Count > 0)
            {

                Table tablaActividades3 = tablas.AddTable(true);

                String[] cabeceraActividades = { "Linea", "Fecha Solicitud", "Fecha Atención", "Horas Atención", "Horas Retraso" };

                tablaActividades3.ResetCells(telCon.cambioDomicilio.Count + 1, cabeceraActividades.Length);

                TableRow recRow = tablaActividades3.Rows[0];
                recRow.IsHeader = true;
                recRow.Height = 10;

                recRow.RowFormat.BackColor = Color.FromArgb(81, 25, 162);
                recRow.RowFormat.Borders.BorderType = BorderStyle.Single;


                for (int i = 0; i < cabeceraActividades.Length; i++)
                {
                    //Alineacion de celdas
                    Paragraph p = recRow.Cells[i].AddParagraph();
                    tablaActividades3.Rows[0].Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Bottom;
                    p.Format.HorizontalAlignment = HorizontalAlignment.Center;

                    //Formato de datos
                    TextRange TR = p.AppendText(cabeceraActividades[i]);
                    TR.CharacterFormat.FontName = "Arial";
                    TR.CharacterFormat.FontSize = 9;
                    TR.CharacterFormat.Bold = true;
                    TR.CharacterFormat.TextColor = Color.White;
                }

                for (int r = 0; r < telCon.cambioDomicilio.Count; r++)
                {
                    TableRow DataRow = tablaActividades3.Rows[r + 1];
                    //Fila Height
                    DataRow.Height = 5;
                    for (int c = 0; c < cabeceraActividades.Length; c++)
                    {
                        TextRange TR2 = null;
                        //Alineacion de Celdas
                        DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        //Llenar datos en filas
                        Paragraph p2 = DataRow.Cells[c].AddParagraph();
                        if (c == 0)
                        {

                            TR2 = p2.AppendText(telCon.cambioDomicilio[r].Linea);
                        }

                        if (c == 1)
                        {
                            TR2 = p2.AppendText(telCon.cambioDomicilio[r].FechaSolicitud.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 2)
                        {
                            TR2 = p2.AppendText(telCon.cambioDomicilio[r].FechaAtencion.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 3)
                        {
                            TR2 = p2.AppendText(telCon.cambioDomicilio[r].DiasAtencion + "");
                        }

                        if (c == 4)
                        {
                            TR2 = p2.AppendText(telCon.cambioDomicilio[r].DiasRetraso + "");
                        }

                        //Formato de celdas
                        p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                        TR2.CharacterFormat.FontName = "Arial";
                        TR2.CharacterFormat.FontSize = 9;
                    }
                }
                document.Replace("||Pregunta4||", "Se presentarón cambios de domicilio en este mes.", false, true);
                BookmarksNavigator marcaActividades = new BookmarksNavigator(document);
                marcaActividades.MoveToBookmark("Pregunta4", true, true);
                marcaActividades.InsertTable(tablaActividades3);
            }
            else
            {
                document.Replace("||Pregunta4||", "No se cambios de domicilio en este mes.", false, true);
            }

            //Resultados Pregunta 5
            if (telCon.reubicacion.Count > 0)
            {

                Table tablaActividades3 = tablas.AddTable(true);

                String[] cabeceraActividades = { "Linea", "Fecha Solicitud", "Fecha Atención", "Horas Atención", "Horas Retraso" };

                tablaActividades3.ResetCells(telCon.reubicacion.Count + 1, cabeceraActividades.Length);

                TableRow recRow = tablaActividades3.Rows[0];
                recRow.IsHeader = true;
                recRow.Height = 10;

                recRow.RowFormat.BackColor = Color.FromArgb(81, 25, 162);
                recRow.RowFormat.Borders.BorderType = BorderStyle.Single;


                for (int i = 0; i < cabeceraActividades.Length; i++)
                {
                    //Alineacion de celdas
                    Paragraph p = recRow.Cells[i].AddParagraph();
                    tablaActividades3.Rows[0].Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Bottom;
                    p.Format.HorizontalAlignment = HorizontalAlignment.Center;

                    //Formato de datos
                    TextRange TR = p.AppendText(cabeceraActividades[i]);
                    TR.CharacterFormat.FontName = "Arial";
                    TR.CharacterFormat.FontSize = 9;
                    TR.CharacterFormat.Bold = true;
                    TR.CharacterFormat.TextColor = Color.White;
                }

                for (int r = 0; r < telCon.reubicacion.Count; r++)
                {
                    TableRow DataRow = tablaActividades3.Rows[r + 1];
                    //Fila Height
                    DataRow.Height = 5;
                    for (int c = 0; c < cabeceraActividades.Length; c++)
                    {
                        TextRange TR2 = null;
                        //Alineacion de Celdas
                        DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        //Llenar datos en filas
                        Paragraph p2 = DataRow.Cells[c].AddParagraph();
                        if (c == 0)
                        {

                            TR2 = p2.AppendText(telCon.reubicacion[r].Linea);
                        }

                        if (c == 1)
                        {
                            TR2 = p2.AppendText(telCon.reubicacion[r].FechaSolicitud.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 2)
                        {
                            TR2 = p2.AppendText(telCon.reubicacion[r].FechaAtencion.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 3)
                        {
                            TR2 = p2.AppendText(telCon.reubicacion[r].DiasAtencion + "");
                        }

                        if (c == 4)
                        {
                            TR2 = p2.AppendText(telCon.reubicacion[r].DiasRetraso + "");
                        }

                        //Formato de celdas
                        p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                        TR2.CharacterFormat.FontName = "Arial";
                        TR2.CharacterFormat.FontSize = 9;
                    }
                }
                document.Replace("||Pregunta5||", "Se presentarón suspensiones por robo/extravío en este mes.", false, true);
                BookmarksNavigator marcaActividades = new BookmarksNavigator(document);
                marcaActividades.MoveToBookmark("Pregunta5", true, true);
                marcaActividades.InsertTable(tablaActividades3);
            }
            else
            {
                document.Replace("||Pregunta5||", "No se presentarón suspensiones por robo/extravío en este mes.", false, true);
            }

            //Resultados Pregunta 6
            if (telCon.identificador.Count > 0)
            {

                Table tablaActividades3 = tablas.AddTable(true);

                String[] cabeceraActividades = { "Linea", "Fecha Solicitud", "Fecha Atención", "Horas Atención", "Horas Retraso" };

                tablaActividades3.ResetCells(telCon.identificador.Count + 1, cabeceraActividades.Length);

                TableRow recRow = tablaActividades3.Rows[0];
                recRow.IsHeader = true;
                recRow.Height = 10;

                recRow.RowFormat.BackColor = Color.FromArgb(81, 25, 162);
                recRow.RowFormat.Borders.BorderType = BorderStyle.Single;


                for (int i = 0; i < cabeceraActividades.Length; i++)
                {
                    //Alineacion de celdas
                    Paragraph p = recRow.Cells[i].AddParagraph();
                    tablaActividades3.Rows[0].Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Bottom;
                    p.Format.HorizontalAlignment = HorizontalAlignment.Center;

                    //Formato de datos
                    TextRange TR = p.AppendText(cabeceraActividades[i]);
                    TR.CharacterFormat.FontName = "Arial";
                    TR.CharacterFormat.FontSize = 9;
                    TR.CharacterFormat.Bold = true;
                    TR.CharacterFormat.TextColor = Color.White;
                }

                for (int r = 0; r < telCon.identificador.Count; r++)
                {
                    TableRow DataRow = tablaActividades3.Rows[r + 1];
                    //Fila Height
                    DataRow.Height = 5;
                    for (int c = 0; c < cabeceraActividades.Length; c++)
                    {
                        TextRange TR2 = null;
                        //Alineacion de Celdas
                        DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        //Llenar datos en filas
                        Paragraph p2 = DataRow.Cells[c].AddParagraph();
                        if (c == 0)
                        {

                            TR2 = p2.AppendText(telCon.identificador[r].Linea);
                        }

                        if (c == 1)
                        {
                            TR2 = p2.AppendText(telCon.identificador[r].FechaSolicitud.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 2)
                        {
                            TR2 = p2.AppendText(telCon.identificador[r].FechaAtencion.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 3)
                        {
                            TR2 = p2.AppendText(telCon.identificador[r].DiasAtencion + "");
                        }

                        if (c == 4)
                        {
                            TR2 = p2.AppendText(telCon.identificador[r].DiasRetraso + "");
                        }

                        //Formato de celdas
                        p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                        TR2.CharacterFormat.FontName = "Arial";
                        TR2.CharacterFormat.FontSize = 9;
                    }
                }
                document.Replace("||Pregunta6||", "Se realizaron activaciones para el identificador de llamadas en este mes.", false, true);
                BookmarksNavigator marcaActividades = new BookmarksNavigator(document);
                marcaActividades.MoveToBookmark("Pregunta6", true, true);
                marcaActividades.InsertTable(tablaActividades3);
            }
            else
            {
                document.Replace("||Pregunta6||", "No se realizaron activaciones para el identificador de llamadas en este mes.", false, true);
            }

            //Resultados Pregunta 7
            if (telCon.instalaciónTroncal.Count > 0)
            {

                Table tablaActividades3 = tablas.AddTable(true);

                String[] cabeceraActividades = { "Linea", "Fecha Solicitud", "Fecha Atención", "Horas Atención", "Horas Retraso" };

                tablaActividades3.ResetCells(telCon.instalaciónTroncal.Count + 1, cabeceraActividades.Length);

                TableRow recRow = tablaActividades3.Rows[0];
                recRow.IsHeader = true;
                recRow.Height = 10;

                recRow.RowFormat.BackColor = Color.FromArgb(81, 25, 162);
                recRow.RowFormat.Borders.BorderType = BorderStyle.Single;


                for (int i = 0; i < cabeceraActividades.Length; i++)
                {
                    //Alineacion de celdas
                    Paragraph p = recRow.Cells[i].AddParagraph();
                    tablaActividades3.Rows[0].Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Bottom;
                    p.Format.HorizontalAlignment = HorizontalAlignment.Center;

                    //Formato de datos
                    TextRange TR = p.AppendText(cabeceraActividades[i]);
                    TR.CharacterFormat.FontName = "Arial";
                    TR.CharacterFormat.FontSize = 9;
                    TR.CharacterFormat.Bold = true;
                    TR.CharacterFormat.TextColor = Color.White;
                }

                for (int r = 0; r < telCon.instalaciónTroncal.Count; r++)
                {
                    TableRow DataRow = tablaActividades3.Rows[r + 1];
                    //Fila Height
                    DataRow.Height = 5;
                    for (int c = 0; c < cabeceraActividades.Length; c++)
                    {
                        TextRange TR2 = null;
                        //Alineacion de Celdas
                        DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        //Llenar datos en filas
                        Paragraph p2 = DataRow.Cells[c].AddParagraph();
                        if (c == 0)
                        {

                            TR2 = p2.AppendText(telCon.instalaciónTroncal[r].Linea);
                        }

                        if (c == 1)
                        {
                            TR2 = p2.AppendText(telCon.instalaciónTroncal[r].FechaSolicitud.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 2)
                        {
                            TR2 = p2.AppendText(telCon.instalaciónTroncal[r].FechaAtencion.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 3)
                        {
                            TR2 = p2.AppendText(telCon.instalaciónTroncal[r].DiasAtencion + "");
                        }

                        if (c == 4)
                        {
                            TR2 = p2.AppendText(telCon.instalaciónTroncal[r].DiasRetraso + "");
                        }

                        //Formato de celdas
                        p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                        TR2.CharacterFormat.FontName = "Arial";
                        TR2.CharacterFormat.FontSize = 9;
                    }
                }
                document.Replace("||Pregunta7||", "Se presentarón instalaciones de troncales y DID's en este mes.", false, true);
                BookmarksNavigator marcaActividades = new BookmarksNavigator(document);
                marcaActividades.MoveToBookmark("Pregunta7", true, true);
                marcaActividades.InsertTable(tablaActividades3);
            }
            else
            {
                document.Replace("||Pregunta7||", "No se presentarón instalaciones de troncales y DID's en este mes.", false, true);
            }

            //Resultados Pregunta 8
            if (telCon.contratacionInternet.Count > 0)
            {

                Table tablaActividades3 = tablas.AddTable(true);

                String[] cabeceraActividades = { "Linea", "Fecha Solicitud", "Fecha Atención", "Horas Atención", "Horas Retraso" };

                tablaActividades3.ResetCells(telCon.contratacionInternet.Count + 1, cabeceraActividades.Length);

                TableRow recRow = tablaActividades3.Rows[0];
                recRow.IsHeader = true;
                recRow.Height = 10;

                recRow.RowFormat.BackColor = Color.FromArgb(81, 25, 162);
                recRow.RowFormat.Borders.BorderType = BorderStyle.Single;


                for (int i = 0; i < cabeceraActividades.Length; i++)
                {
                    //Alineacion de celdas
                    Paragraph p = recRow.Cells[i].AddParagraph();
                    tablaActividades3.Rows[0].Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Bottom;
                    p.Format.HorizontalAlignment = HorizontalAlignment.Center;

                    //Formato de datos
                    TextRange TR = p.AppendText(cabeceraActividades[i]);
                    TR.CharacterFormat.FontName = "Arial";
                    TR.CharacterFormat.FontSize = 9;
                    TR.CharacterFormat.Bold = true;
                    TR.CharacterFormat.TextColor = Color.White;
                }

                for (int r = 0; r < telCon.contratacionInternet.Count; r++)
                {
                    TableRow DataRow = tablaActividades3.Rows[r + 1];
                    //Fila Height
                    DataRow.Height = 5;
                    for (int c = 0; c < cabeceraActividades.Length; c++)
                    {
                        TextRange TR2 = null;
                        //Alineacion de Celdas
                        DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        //Llenar datos en filas
                        Paragraph p2 = DataRow.Cells[c].AddParagraph();
                        if (c == 0)
                        {

                            TR2 = p2.AppendText(telCon.contratacionInternet[r].Linea);
                        }

                        if (c == 1)
                        {
                            TR2 = p2.AppendText(telCon.contratacionInternet[r].FechaSolicitud.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 2)
                        {
                            TR2 = p2.AppendText(telCon.contratacionInternet[r].FechaAtencion.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 3)
                        {
                            TR2 = p2.AppendText(telCon.contratacionInternet[r].DiasAtencion + "");
                        }

                        if (c == 4)
                        {
                            TR2 = p2.AppendText(telCon.contratacionInternet[r].DiasRetraso + "");
                        }

                        //Formato de celdas
                        p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                        TR2.CharacterFormat.FontName = "Arial";
                        TR2.CharacterFormat.FontSize = 9;
                    }
                }
                document.Replace("||Pregunta8||", "Se realizaron solicitudes de instalación de internet en este mes.", false, true);
                BookmarksNavigator marcaActividades = new BookmarksNavigator(document);
                marcaActividades.MoveToBookmark("Pregunta8", true, true);
                marcaActividades.InsertTable(tablaActividades3);
            }
            else
            {
                document.Replace("||Pregunta8||", "No se realizaron solicitudes de instalación de internet en este mes.", false, true);
            }

            //Resultados Pregunta 9
            if (telCon.habilitacionServicios.Count > 0)
            {

                Table tablaActividades3 = tablas.AddTable(true);

                String[] cabeceraActividades = { "Linea", "Fecha Solicitud", "Fecha Atención", "Horas Atención", "Horas Retraso" };

                tablaActividades3.ResetCells(telCon.habilitacionServicios.Count + 1, cabeceraActividades.Length);

                TableRow recRow = tablaActividades3.Rows[0];
                recRow.IsHeader = true;
                recRow.Height = 10;

                recRow.RowFormat.BackColor = Color.FromArgb(81, 25, 162);
                recRow.RowFormat.Borders.BorderType = BorderStyle.Single;


                for (int i = 0; i < cabeceraActividades.Length; i++)
                {
                    //Alineacion de celdas
                    Paragraph p = recRow.Cells[i].AddParagraph();
                    tablaActividades3.Rows[0].Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Bottom;
                    p.Format.HorizontalAlignment = HorizontalAlignment.Center;

                    //Formato de datos
                    TextRange TR = p.AppendText(cabeceraActividades[i]);
                    TR.CharacterFormat.FontName = "Arial";
                    TR.CharacterFormat.FontSize = 9;
                    TR.CharacterFormat.Bold = true;
                    TR.CharacterFormat.TextColor = Color.White;
                }

                for (int r = 0; r < telCon.habilitacionServicios.Count; r++)
                {
                    TableRow DataRow = tablaActividades3.Rows[r + 1];
                    //Fila Height
                    DataRow.Height = 5;
                    for (int c = 0; c < cabeceraActividades.Length; c++)
                    {
                        TextRange TR2 = null;
                        //Alineacion de Celdas
                        DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        //Llenar datos en filas
                        Paragraph p2 = DataRow.Cells[c].AddParagraph();
                        if (c == 0)
                        {

                            TR2 = p2.AppendText(telCon.habilitacionServicios[r].Linea);
                        }

                        if (c == 1)
                        {
                            TR2 = p2.AppendText(telCon.habilitacionServicios[r].FechaSolicitud.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 2)
                        {
                            TR2 = p2.AppendText(telCon.habilitacionServicios[r].FechaAtencion.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 3)
                        {
                            TR2 = p2.AppendText(telCon.habilitacionServicios[r].DiasAtencion + "");
                        }

                        if (c == 4)
                        {
                            TR2 = p2.AppendText(telCon.habilitacionServicios[r].DiasRetraso + "");
                        }

                        //Formato de celdas
                        p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                        TR2.CharacterFormat.FontName = "Arial";
                        TR2.CharacterFormat.FontSize = 9;
                    }
                }
                document.Replace("||Pregunta9||", "Se solicitaron habilitación de servicios en este mes.", false, true);
                BookmarksNavigator marcaActividades = new BookmarksNavigator(document);
                marcaActividades.MoveToBookmark("Pregunta9", true, true);
                marcaActividades.InsertTable(tablaActividades3);
            }
            else
            {
                document.Replace("||Pregunta9||", "No se solicitó habilitación de servicios en este mes.", false, true);
            }

            //Resultados Pregunta 10
            if (telCon.cancelacionServicios.Count > 0)
            {

                Table tablaActividades3 = tablas.AddTable(true);

                String[] cabeceraActividades = { "Linea", "Fecha Solicitud", "Fecha Atención", "Horas Atención", "Horas Retraso" };

                tablaActividades3.ResetCells(telCon.cancelacionServicios.Count + 1, cabeceraActividades.Length);

                TableRow recRow = tablaActividades3.Rows[0];
                recRow.IsHeader = true;
                recRow.Height = 10;

                recRow.RowFormat.BackColor = Color.FromArgb(81, 25, 162);
                recRow.RowFormat.Borders.BorderType = BorderStyle.Single;


                for (int i = 0; i < cabeceraActividades.Length; i++)
                {
                    //Alineacion de celdas
                    Paragraph p = recRow.Cells[i].AddParagraph();
                    tablaActividades3.Rows[0].Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Bottom;
                    p.Format.HorizontalAlignment = HorizontalAlignment.Center;

                    //Formato de datos
                    TextRange TR = p.AppendText(cabeceraActividades[i]);
                    TR.CharacterFormat.FontName = "Arial";
                    TR.CharacterFormat.FontSize = 9;
                    TR.CharacterFormat.Bold = true;
                    TR.CharacterFormat.TextColor = Color.White;
                }

                for (int r = 0; r < telCon.cancelacionServicios.Count; r++)
                {
                    TableRow DataRow = tablaActividades3.Rows[r + 1];
                    //Fila Height
                    DataRow.Height = 5;
                    for (int c = 0; c < cabeceraActividades.Length; c++)
                    {
                        TextRange TR2 = null;
                        //Alineacion de Celdas
                        DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        //Llenar datos en filas
                        Paragraph p2 = DataRow.Cells[c].AddParagraph();
                        if (c == 0)
                        {

                            TR2 = p2.AppendText(telCon.cancelacionServicios[r].Linea);
                        }

                        if (c == 1)
                        {
                            TR2 = p2.AppendText(telCon.cancelacionServicios[r].FechaSolicitud.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 2)
                        {
                            TR2 = p2.AppendText(telCon.cancelacionServicios[r].FechaAtencion.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 3)
                        {
                            TR2 = p2.AppendText(telCon.cancelacionServicios[r].DiasAtencion + "");
                        }

                        if (c == 4)
                        {
                            TR2 = p2.AppendText(telCon.cancelacionServicios[r].DiasRetraso + "");
                        }

                        //Formato de celdas
                        p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                        TR2.CharacterFormat.FontName = "Arial";
                        TR2.CharacterFormat.FontSize = 9;
                    }
                }
                document.Replace("||Pregunta10||", "Se presentarón solicitudes de cancelación de servicios en este mes.", false, true);
                BookmarksNavigator marcaActividades = new BookmarksNavigator(document);
                marcaActividades.MoveToBookmark("Pregunta10", true, true);
                marcaActividades.InsertTable(tablaActividades3);
            }
            else
            {
                document.Replace("||Pregunta10||", "No se solicitudes de cancelación de servicios en este mes.", false, true);
            }

            //Resultados Pregunta 11
            if (telCon.reporteFallas.Count > 0)
            {

                Table tablaActividades3 = tablas.AddTable(true);

                String[] cabeceraActividades = { "Linea", "Fecha Solicitud", "Fecha Atención", "Horas Atención", "Horas Retraso" };

                tablaActividades3.ResetCells(telCon.reporteFallas.Count + 1, cabeceraActividades.Length);

                TableRow recRow = tablaActividades3.Rows[0];
                recRow.IsHeader = true;
                recRow.Height = 10;

                recRow.RowFormat.BackColor = Color.FromArgb(81, 25, 162);
                recRow.RowFormat.Borders.BorderType = BorderStyle.Single;


                for (int i = 0; i < cabeceraActividades.Length; i++)
                {
                    //Alineacion de celdas
                    Paragraph p = recRow.Cells[i].AddParagraph();
                    tablaActividades3.Rows[0].Cells[i].CellFormat.VerticalAlignment = VerticalAlignment.Bottom;
                    p.Format.HorizontalAlignment = HorizontalAlignment.Center;

                    //Formato de datos
                    TextRange TR = p.AppendText(cabeceraActividades[i]);
                    TR.CharacterFormat.FontName = "Arial";
                    TR.CharacterFormat.FontSize = 9;
                    TR.CharacterFormat.Bold = true;
                    TR.CharacterFormat.TextColor = Color.White;
                }

                for (int r = 0; r < telCon.reporteFallas.Count; r++)
                {
                    TableRow DataRow = tablaActividades3.Rows[r + 1];
                    //Fila Height
                    DataRow.Height = 5;
                    for (int c = 0; c < cabeceraActividades.Length; c++)
                    {
                        TextRange TR2 = null;
                        //Alineacion de Celdas
                        DataRow.Cells[c].CellFormat.VerticalAlignment = VerticalAlignment.Middle;
                        //Llenar datos en filas
                        Paragraph p2 = DataRow.Cells[c].AddParagraph();
                        if (c == 0)
                        {

                            TR2 = p2.AppendText(telCon.reporteFallas[r].Linea);
                        }

                        if (c == 1)
                        {
                            TR2 = p2.AppendText(telCon.reporteFallas[r].FechaSolicitud.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 2)
                        {
                            TR2 = p2.AppendText(telCon.reporteFallas[r].FechaAtencion.ToString("dd/MM/yyy hh:mm"));
                        }

                        if (c == 3)
                        {
                            TR2 = p2.AppendText(telCon.reporteFallas[r].DiasAtencion + "");
                        }

                        if (c == 4)
                        {
                            TR2 = p2.AppendText(telCon.reporteFallas[r].DiasRetraso + "");
                        }

                        //Formato de celdas
                        p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                        TR2.CharacterFormat.FontName = "Arial";
                        TR2.CharacterFormat.FontSize = 9;
                    }
                }
                document.Replace("||Pregunta11||", "Se presentarón reportes de fallas en este mes.", false, true);
                BookmarksNavigator marcaActividades = new BookmarksNavigator(document);
                marcaActividades.MoveToBookmark("Pregunta11", true, true);
                marcaActividades.InsertTable(tablaActividades3);
            }
            else
            {
                document.Replace("||Pregunta11||", "No presentarón reportes de fallas en este mes.", false, true);
            }



            document.Replace("||Folio||", telCon.Folio, false, true);

            document.Replace("||Mes||", telCon.Mes, false, true);

            document.Replace("||C||", telCon.Calificacion.ToString(), false, true);

            document.Replace("||dia||", telCon.FechaCreacion.GetValueOrDefault().Day + "", false, true);
            document.Replace("||MesE||", Convert.ToDateTime(telCon.FechaCreacion).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")), false, true);
            document.Replace("||Anio||", Convert.ToDateTime(telCon.FechaCreacion.GetValueOrDefault()).Year + "", false, true);


            for (int i = 0; i < telCon.facturas.Count; i++)
            {
                if ((telCon.facturas.Count - 1) != i)
                {
                    strFacturas += telCon.facturas[i].comprobante.Serie + telCon.facturas[i].comprobante.Folio + "/";
                }
                else
                {
                    strFacturas += telCon.facturas[i].comprobante.Serie + telCon.facturas[i].comprobante.Folio;
                }
            }

            document.Replace("||Factura||", strFacturas, false, true);

            document.Replace("||Total||", String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:C}", vFacturas.obtieneTotalFacturas(telCon.facturas)) + "", false, true);

            if (!telCon.Estatus.Equals("Aceptada"))
            {
                BookmarksNavigator marcaNota = new BookmarksNavigator(document);
                marcaNota.MoveToBookmark("Nota", true, true);
                marcaNota.InsertText("NOTA: Esta cédula no cuenta con ningún valor ya que aún no se AUTORIZA por parte de la Dirección de Administración de Servicios.");
            }


            //Salvar y Lanzar

            byte[] toArray = null;
            using (MemoryStream ms1 = new MemoryStream())
            {
                document.SaveToStream(ms1, Spire.Doc.FileFormat.PDF);
                toArray = ms1.ToArray();
            }
            return File(toArray, "application/pdf", "CedulaConvencional_" + telCon.Mes + ".pdf");
        }

        [Route("/reporte/muebles/{id}")]
        public async Task<IActionResult> CedulaMuebles(int id)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), moduloMuebles(), "cédula");
            if (success == 1)
            {
                string strFacturas = "";
                decimal totalFacturas = 0;

                CedulaMuebles cedula = new CedulaMuebles();
                cedula = await vMuebles.CedulaById(id);
                cedula.inmuebleOrigen = await vInmuebles.inmuebleById(cedula.InmuebleOrigenId);
                cedula.inmuebleDestino = await vInmuebles.inmuebleById(cedula.InmuebleDestinoId);
                cedula.usuarios = await vUsuarios.getUserById(cedula.UsuarioId);
                cedula.iEntregables = await vEntregables.getEntregables(id);
                cedula.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedula.RespuestasEncuesta = await vMuebles.obtieneRespuestas(id);
                cedula.facturas = new List<Facturas>();
                cedula.facturas = await vFacturas.getFacturas(id, 11);

                Document document = new Document();
                var path = @"E:\Plantillas CASESGV2\DocsV2\ReporteMuebles.docx";
                document.LoadFromFile(path);

                //Creamos la Tabla
                Section tablas = document.AddSection();

                if (cedula.RespuestasEncuesta[0].Respuesta == false)
                {
                    document.Replace("||Horarios||", "El prestador de servicios no cumplió con la fecha y hora solicitada para la prestación del servicio, la fecha en que se solicito fue," +
                                     Convert.ToDateTime(cedula.RespuestasEncuesta[0].Detalles.Split("|")[0]) + " y la fecha y hora de llegada fue:" + Convert.ToDateTime(cedula.RespuestasEncuesta[0].Detalles.Split("|")[1])
                                     + ".", false, true);
                }
                else
                {
                    document.Replace("||Horarios||", "El prestador de servicios cumplió con la fecha y hora solicitada para la prestación del servicio.", false, true);
                }

                if (cedula.RespuestasEncuesta[1].Respuesta == true)
                {
                    document.Replace("||Equipo||", "El prestador cumplió con la maquinaria, equipo y herramientas para la prestación del servicio.", false, true);
                }
                else
                {
                    document.Replace("||Equipo||", "El prestador no cumplió con la maquinaria, equipo y herramientas para la prestación del servicio.", false, true);
                }

                if (cedula.RespuestasEncuesta[2].Respuesta == true)
                {
                    document.Replace("||Transporte||", "El prestador cumplió con la unidad de transporte solicitada para la prestación del servicio.", false, true);
                }
                else
                {
                    document.Replace("||Transporte||", "El prestador no cumplió con la unidad de transporte solicitada para la prestación del servicio.", false, true);
                }

                if (cedula.RespuestasEncuesta[3].Respuesta == true)
                {
                    document.Replace("||Personal||", "El prestador cumplió con el personal necesario para realizar la prestación del servicio.", false, true);
                }
                else
                {
                    document.Replace("||Personal||", "El prestador no cumplió con el personal necesario para realizar la prestación del servicio.", false, true);
                }

                cedula.incidencias = await iMuebles.GetIncidenciasPregunta(id, 5);
                //obtenemos el documento con marcas
                if (cedula.incidencias.Count > 0)
                {
                    Table tablaActividades = tablas.AddTable(true);

                    String[] cabeceraFechas = { "Tipo", "Fecha y Hora Programada", "Fecha y Hora de Respuesta", "Comentarios" };

                    tablaActividades.ResetCells(cedula.incidencias.Count + 1, cabeceraFechas.Length);

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

                    for (int r = 0; r < cedula.incidencias.Count; r++)
                    {
                        if (cedula.incidencias[r].Pregunta.Equals(5 + ""))
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
                                    TR2 = p2.AppendText(cedula.incidencias[r].Tipo);
                                }
                                if (c == 1)
                                {
                                    TR2 = p2.AppendText(cedula.incidencias[r].FechaSolicitud.ToString("dd/MM/yyyy HH:mm"));
                                }
                                if (c == 2)
                                {
                                    TR2 = p2.AppendText(cedula.incidencias[r].FechaRespuesta.ToString("dd/MM/yyyy HH:mm"));
                                }
                                if (c == 3)
                                {
                                    TR2 = p2.AppendText(cedula.incidencias[r].Comentarios);
                                }
                                //Formato de celdas
                                p2.Format.HorizontalAlignment = HorizontalAlignment.Center;
                                TR2.CharacterFormat.FontName = "Arial";
                                TR2.CharacterFormat.FontSize = 9;
                            }
                        }
                    }

                    BookmarksNavigator marcaActividades = new BookmarksNavigator(document);
                    marcaActividades.MoveToBookmark("Solicitudes", true, true);
                    marcaActividades.InsertTable(tablaActividades);
                    document.Replace("||Solicitudes||", "El personal no cumplió con las actividades contempladas en el programa de operación, las cuales se describen a continuación: ", false, true);
                }
                else
                {
                    document.Replace("||Solicitudes||", "El personal cumplió con las actividades contempladas en el programa de operación, no presentó incidencias en el mes.", false, true);
                }

                document.Replace("||Folio||", cedula.Folio, false, true);

                document.Replace("||Mes||", cedula.Mes, false, true);
                if (cedula.Estatus.Equals("Autorizada"))
                {
                    document.Replace("||C||", cedula.Calificacion.ToString(), false, true);
                }
                else
                {
                    document.Replace("||C||", "Pendiente", false, true);
                }

                document.Replace("||Origen||", cedula.inmuebleOrigen.Nombre, false, true);
                document.Replace("||Destino||", cedula.inmuebleDestino.Nombre, false, true);

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
                return File(toArray, "application/pdf", "CedulaMuebles_" + cedula.Mes + ".pdf");
            }
            return Redirect("/error/denied");
        }

        [Route("/reporte/analisis/{id}")]
        public async Task<IActionResult> CedulaAnalisis(int id)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), moduloAnalisis(), "cédula");
            if (success == 1)
            {
                string strFacturas = "";
                decimal totalFacturas = 0;

                CedulaEvaluacion cedula = new CedulaEvaluacion();
                cedula = await vCedula.CedulaById(id);
                cedula.inmuebles = await vInmuebles.inmuebleById(cedula.InmuebleId);
                cedula.usuarios = await vUsuarios.getUserById(cedula.UsuarioId);
                cedula.incidencias.analisis = await iAnalisis.GetIncidencias(id);
                cedula.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedula.RespuestasEncuesta = await vAnalisis.obtieneRespuestas(id);
                cedula.facturas = new List<Facturas>();
                cedula.facturas = await vFacturas.getFacturas(id, 8);

                Document document = new Document();
                var path = @"E:\Plantillas CASESGV2\DocsV2\ReporteAnalisis.docx";
                document.LoadFromFile(path);

                //Creamos la Tabla
                Section tablas = document.AddSection();

                document.Replace("||Administracion||", cedula.inmuebles.Nombre, false, true);
                //P1
                document.Replace("||Cierre||", "El cierre de mes se realizó el día: " + Convert.ToDateTime(cedula.RespuestasEncuesta[0].Detalles).ToString("dd/MM/yyyy") + ".", false, true);

                //P2
                if (cedula.RespuestasEncuesta[1].Respuesta == true)
                {
                    document.Replace("||FechaProgramada||", "El prestador llevó a cabo el servicio en la fecha programada.", false, true);
                }
                else
                {
                    document.Replace("||FechaProgramada||", "El prestador no llevó a cabo el servicio en la fecha programada, ya que el servicio se programo para el día: " +
                        cedula.RespuestasEncuesta[1].Detalles.Split("|")[0] + " y se llevó a cabo el día: " + cedula.RespuestasEncuesta[1].Detalles.Split("|")[1] + ".", false, true);
                }

                //P3
                if (cedula.incidencias.analisis.Count > 0)
                {
                    Table tablaActividades = tablas.AddTable(true);

                    String[] cabeceraFechas = { "Fecha de la Incidencia", "Tipo", "Comentarios" };

                    tablaActividades.ResetCells(cedula.incidencias.analisis.Count + 1, cabeceraFechas.Length);

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

                    for (int r = 0; r < cedula.incidencias.analisis.Count; r++)
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
                                TR2 = p2.AppendText(cedula.incidencias.analisis[r].FechaIncidencia.ToString("dd/MM/yyyy"));
                            }
                            if (c == 1)
                            {
                                TR2 = p2.AppendText(cedula.incidencias.analisis[r].Tipo);
                            }
                            if (c == 2)
                            {
                                TR2 = p2.AppendText(cedula.incidencias.analisis[r].Comentarios);
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

                //P4
                if (cedula.RespuestasEncuesta[3].Respuesta == true)
                {
                    document.Replace("||Uniforme||", "El prestador del servicio portó uniforme e identificación en todo momento.", false, true);
                }
                else
                {
                    document.Replace("||Uniforme||", "El prestador del servicio no portó uniforme e identificación en todo momento.", false, true);
                }

                //P5
                if (cedula.RespuestasEncuesta[4].Respuesta == true)
                {
                    document.Replace("||Recolección||", "El prestador del servicio si recolectó el número total de muestras solicitadas.", false, true);
                }
                else
                {
                    document.Replace("||Recolección||", "El prestador del servicio recolectó un total de " + cedula.RespuestasEncuesta[4].Detalles + " muestras, no cumpliendo " +
                        " con el número de muestras minimas de 7.", false, true);
                }

                if (cedula.RespuestasEncuesta[5].Respuesta == true)
                {
                    document.Replace("||Resultados||", "El prestador llevó a cabo el servicio en la fecha programada.", false, true);
                }
                else
                {
                    document.Replace("||Resultados||", "El prestador no llevó a cabo el servicio en la fecha programada, ya que el servicio se programo para el día: " +
                        cedula.RespuestasEncuesta[5].Detalles.Split("|")[0] + " y se llevó a cabo el día: " + cedula.RespuestasEncuesta[5].Detalles.Split("|")[1] + ".", false, true);
                }

                if (cedula.RespuestasEncuesta[6].Respuesta == true)
                {
                    document.Replace("||Reporte||", "El prestador del servicio entregó el reporte del servicio el día: " +
                        Convert.ToDateTime(cedula.RespuestasEncuesta[6].Detalles).ToString("dd/MM/yyyy") + ".", false, true);
                }
                else
                {
                    document.Replace("||Reporte||", "El prestador no entregó el reporte del servicio.", false, true);
                }


                document.Replace("||Folio||", cedula.Folio, false, true);

                document.Replace("||Mes||", cedula.Mes, false, true);
                if (cedula.Estatus.Equals("Autorizada"))
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
                return File(toArray, "application/pdf", "CedulaAnalisis_" + cedula.Mes + ".pdf");
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

            document.Replace("|InmuebleP|", cedula.InmuebleC, false, true);
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

            document.Replace("|InmuebleP|", cedula.InmuebleC, false, true);
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
