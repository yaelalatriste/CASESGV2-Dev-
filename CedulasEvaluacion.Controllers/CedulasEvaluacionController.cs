using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using CedulasEvaluacion.Interfaces;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel;
using CedulasEvaluacion.Entities.Reportes;
using CedulasEvaluacion.Entities.MMensajeria;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.Vistas;
using System.Globalization;
using System.IO;
using Microsoft.Reporting.NETCore;
using CedulasEvaluacion.Entities.MMuebles;
using CedulasEvaluacion.Entities.MCelular;
using CedulasEvaluacion.Entities.MConvencional;
using CedulasEvaluacion.Entities.MFumigacion;
using CedulasEvaluacion.Entities.MAgua;
using CedulasEvaluacion.Entities.MResiduos;
using CedulasEvaluacion.Entities.MAnalisis;
using CedulasEvaluacion.Entities.MTransporte;
using CedulasEvaluacion.Entities.TrasladoExp;

namespace CedulasEvaluacion.Controllers
{
    public class CedulasEvaluacionController:Controller
    {

        private string [] tiposIncidencia = {"Recoleccion","Entrega","Acuses", "Mal Estado","Extraviadas","Robadas" };
        private string [] incidenciasAgua = {"","Fechas","Horas","Numeral"};
        private string[] tiposCelular = { "Alta_Equipo", "Alta", "Baja", "Reactivacion", "Suspension", "Perfil", "SIM", "CambioNumeroRegion", 
                                          "VozDatos", "Diagnostico", "Reparacion"};
        private string[] rCelular = { "altas con entrega de equipo", "altas sin entrega de equipo", "bajas de servicio", "reactivaciones por robo/extravío", "suspensiones por robo/extravío", "cambios de perfil", "switcheos de SIM Card", "cambios de número o región",
                                          "solicitudes de voz y/o datos", "solicitudes de diagnóstico de equipo", "solicitudes de reparación de equipo"};
        private string[] tiposConv = { "contratacion_instalacion", "cableado", "entregaAparato", "cambioDomicilio", "reubicacion", "identificadorLlamadas", "troncales", "internet",
                                          "serviciosTelefonia", "cancelacion", "reportesFallas"};
        private string[] rConv = { "nuevas contrataciones", "cableados interiores para la instalación de líneas telefónicas","entregas de aparatos telefónicos", "cambios de domicilio", 
            "reubicaciones de lineas telefónicas comerciales en el mismo inmueble", "activaciones para el identificador de llamadas", 
            "instalaciones de troncales y DID's",
                                   "solicitudes de instalación de internet", "habilitación de servicios",
                                          "solicitudes de cancelación de servicios", "reportes de fallas"};


        private readonly IWebHostEnvironment web;
        
        private readonly IRepositorioEvaluacionServicios vCedula;        
        private readonly IRepositorioReporteCedula vrCedula;
        private readonly IRepositorioIncidencias iLimpieza;
        private readonly IRepositorioIncidenciasFumigacion iFumigacion;
        private readonly IRepositorioIncidenciasMensajeria iMensajeria;
        private readonly IRepositorioIncidenciasMuebles iMuebles;
        private readonly IRepositorioIncidenciasCelular iCelular;
        private readonly IRepositorioIncidenciasConvencional iConvencional;
        private readonly IRepositorioIncidenciasAgua iAgua;
        private readonly IRepositorioIncidenciasResiduos iResiduos;
        private readonly IRepositorioIncidenciasAnalisis iAnalisis;
        private readonly IRepositorioIncidenciasTransporte iTransporte;
        private readonly IRepositorioIncidenciasTraslado iTraslado;

        public CedulasEvaluacionController(IWebHostEnvironment vweb,IRepositorioReporteCedula vvReporte, IRepositorioIncidenciasMensajeria iiMensajeria,
            IRepositorioIncidencias iiLimpieza, IRepositorioEvaluacionServicios viCedula, IRepositorioIncidenciasFumigacion iiFumigacion, IRepositorioIncidenciasTraslado iiTraslado,
            IRepositorioIncidenciasMuebles IiMuebles, IRepositorioIncidenciasCelular iiCelular, IRepositorioIncidenciasAgua iiAgua, IRepositorioIncidenciasResiduos iiResiduos,
            IRepositorioIncidenciasConvencional iiConvencional, IRepositorioIncidenciasAnalisis iiAnalisis, IRepositorioIncidenciasTransporte iiTransporte)
        {
            this.web = vweb;
            this.vCedula = viCedula;
            this.vrCedula = vvReporte;
            this.iLimpieza = iiLimpieza;
            this.iFumigacion= iiFumigacion;
            this.iMensajeria = iiMensajeria;
            this.iCelular = iiCelular;
            this.iMuebles= IiMuebles;
            this.iConvencional = iiConvencional;
            this.iAgua = iiAgua;
            this.iResiduos = iiResiduos;
            this.iAnalisis = iiAnalisis;
            this.iTransporte= iiTransporte;
            this.iTraslado= iiTraslado;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        [Route("/cedula/mensajeria/{servicio}/{id?}")]
        public async Task<IActionResult> GeneraCedulaMensajeria(int servicio, int id)
        {
            var incidencias = new List<IncidenciasMensajeria>();
            LocalReport local = new LocalReport();
            var path = Directory.GetCurrentDirectory()+ "\\Reports\\CedulaMensajeria.rdlc";
            local.ReportPath = path;
            var cedulas = await vrCedula.getCedulaByServicio(servicio, id);
            var respuestas = await vCedula.obtieneRespuestas(id);
            for (int i = 0; i < respuestas.Count; i++)
            {
                if (i < 6)
                {
                    incidencias = await iMensajeria.getIncidenciasByTipoMensajeria(id, tiposIncidencia[i]);
                    tiposIncidencia[i] = tiposIncidencia[i] == "Mal Estado" ? "MalEstado" : tiposIncidencia[i];
                    local.DataSources.Add(new ReportDataSource("Incidencias" + tiposIncidencia[i], incidencias));
                }

                if (i < 2)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "Se presentaron Incidencias en el inmueble, las cuales se describen a continuación: ") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No se presentarón incidencias en el inmueble en el mes de evaluación.")});
                    }
                }
                else if (i == 2)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        if (respuestas[i].Detalles.Equals("N/A"))
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No aplica la entrega de acuses en el mes de evaluación.") });
                        }
                        else
                        {
                            local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "Se presentaron Incidencias en el inmueble, las cuales se describen a continuación: ")});
                        }
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No se presentarón incidencias en el inmueble en el mes de evaluación.") });
                    }
                }
                else if (i > 2 && i < 6)
                {
                    if (respuestas[i].Respuesta == true)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "Se presentaron Incidencias en el inmueble, las cuales se describen a continuación: ")});
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No se presentarón incidencias en el inmueble en el mes de evaluación.")});
                    }
                }
                else
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio no entregó suficiente material de embalaje para las guías.") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio si entregó suficiente material de embalaje para las guías.")});
                    }
                }
            }
            local.DataSources.Add(new ReportDataSource("CedulaMensajeria", cedulas));
            var pdf = local.Render("PDF");
            return File(pdf,"application/pdf");
        }

        [Route("/cedula/limpieza/{servicio}/{id?}")]
        public async Task<IActionResult> GeneraCedulaLimpieza(int servicio, int id)
        {
            var incidencias = new List<VIncidenciasLimpieza>();
            LocalReport local = new LocalReport();
            var path = Directory.GetCurrentDirectory() + "\\Reports\\CedulaLimpieza.rdlc";
            local.ReportPath = path;
            var cedulas = await vrCedula.getCedulaByServicio(servicio, id);
            var respuestas = await vCedula.obtieneRespuestas(id);
            for (int i = 0; i < respuestas.Count; i++)
            {
                if (i == 0)
                {
                    local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El cierre de mes se efectuó el " + Convert.ToDateTime(respuestas[i].Detalles).ToString("dd") +
                                   " de " + Convert.ToDateTime(respuestas[i].Detalles).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " " + Convert.ToDateTime(respuestas[i].Detalles).ToString("yyyy") + ".")});
                }
                else if (i == 1)
                {
                    incidencias = await iLimpieza.getIncidencias(id);
                    local.DataSources.Add(new ReportDataSource("IncidenciasPO", incidencias));
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "Se presentaron incidencias en el inmueble, las cuales se describen a continuación: ") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No se presentaron incidencias en el inmueble en el mes de evaluación.") });
                    }
                }
                else if (i == 2)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal de limieza no portó identificación y/o uniforme en todo momento.")});
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal de limieza portó identificación y/o uniforme en todo momento.") });
                    }
                }
                else if (i == 3)
                {
                    incidencias = await iLimpieza.getIncidenciasEquipo(id);
                    local.DataSources.Add(new ReportDataSource("IncidenciasEquipo", incidencias));
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "Se presentaron incidencias en el inmueble, las cuales se describen a continuación: ")});
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No se presentaron incidencias en el inmueble en el mes de evaluación.")});
                    }
                }
                else if (i == 4)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio no tuvo inasistencias en el mes de evaluación.") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio presentó " + respuestas[i].Detalles + " inasistencias del personal de limpieza en el mes de evaluación.") });
                    }
                }
                else if (i == 5)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio no entregó el SUA de su personal en tiempo y forma, " +
                            "la administración comentó lo siguiente: "+respuestas[i].Detalles)});
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador de servicio entregó el SUA de su personal en tiempo y forma, la fecha de entrega" +
                                " fue el " + Convert.ToDateTime(respuestas[i].Detalles.Split("|")[0]).ToString("dd") +
                                       " de " + Convert.ToDateTime(respuestas[i].Detalles.Split("|")[0]).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " " +
                                       Convert.ToDateTime(respuestas[i].Detalles.Split("|")[0]).ToString("yyyy") + " correspondiente al mes de " +
                                       respuestas[i].Detalles.Split("|")[1] + ".") });
                    }
                }
                else if (i == 6)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio no entregó el acta Entrega - Recepción en tiempo y forma.") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador de servicio entregó el acta Entrega - Recepción en tiempo y forma " +
                            "el " + Convert.ToDateTime(respuestas[i].Detalles).ToString("dd") +
                                   " de " + Convert.ToDateTime(respuestas[i].Detalles).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " " +
                                   Convert.ToDateTime(respuestas[i].Detalles).ToString("yyyy") + ".")});
                    }
                }
            }
            local.DataSources.Add(new ReportDataSource("CedulaLimpieza", cedulas));
            var pdf = local.Render("PDF");
            return File(pdf, "application/pdf");
        }

        [Route("/cedula/fumigacion/{servicio}/{id?}")]
        public async Task<IActionResult> GeneraCedulaFumigacion(int servicio,int id)
        {
            var incidencias = new List<IncidenciasFumigacion>();
            LocalReport local = new LocalReport();
            var path = Directory.GetCurrentDirectory() + "\\Reports\\CedulaFumigacion.rdlc";
            local.ReportPath = path;
            var cedulas = await vrCedula.getCedulaByServicio(servicio, id);
            var respuestas = await vCedula.obtieneRespuestas(id);
            for (int i = 0; i < respuestas.Count; i++)
            {
                if (i == 0)
                {
                    local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El cierre de mes se efectuó el " + Convert.ToDateTime(respuestas[i].Detalles).ToString("dd") +
                                   " de " + Convert.ToDateTime(respuestas[i].Detalles).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " " + Convert.ToDateTime(respuestas[i].Detalles).ToString("yyyy") + ".")});
                }
                else if (i == 1)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "Se presentaron Incidencias en el inmueble, las cuales se describen a continuación: ") });
                        incidencias = await iFumigacion.GetIncidenciasPregunta(id,respuestas[i].Pregunta);
                        local.DataSources.Add(new ReportDataSource("IncidenciasFechas", incidencias));
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No se presentarón incidencias en el inmueble en el mes de evaluación.") });
                        local.DataSources.Add(new ReportDataSource("IncidenciasFechas", incidencias));
                    }
                }
                else if (i == 2)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "Se presentaron incidencias en el inmueble, las cuales se describen a continuación: ") });
                        incidencias = await iFumigacion.GetIncidenciasPregunta(id,respuestas[i].Pregunta);
                        local.DataSources.Add(new ReportDataSource("IncidenciasHora", incidencias));
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No se presentarón incidencias en el inmueble en el mes de evaluación.") });
                        local.DataSources.Add(new ReportDataSource("IncidenciasHora", incidencias));
                    }
                }
                else if (i == 3)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "Se presentaron incidencias en el inmueble, las cuales se describen a continuación: ") });
                        incidencias = await iFumigacion.GetIncidenciasPregunta(id, respuestas[i].Pregunta);
                        local.DataSources.Add(new ReportDataSource("IncidenciasFauna", incidencias));
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No se presentarón incidencias en el inmueble en el mes de evaluación.") });
                        local.DataSources.Add(new ReportDataSource("IncidenciasFauna", incidencias));
                    }
                }
                else if (i == 4)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador no cumplió con la regulación vigente de los productos.") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador cumplió con la regulación vigente de los productos.") });
                    }
                }
                else if (i == 5)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No se entregó el reporte de servicios por parte del prestador.") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El reporte de servicios se entregó el" +
                            "el " + Convert.ToDateTime(respuestas[i].Detalles).ToString("dd") +
                                   " de " + Convert.ToDateTime(respuestas[i].Detalles).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " " +
                                   Convert.ToDateTime(respuestas[i].Detalles).ToString("yyyy") + ".")});
                    }
                }
                else if (i == 6)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No se entregó el listado de personal por parte del prestador.") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El reporte de servicios se entregó el listado del personal " +
                            "el " + Convert.ToDateTime(respuestas[i].Detalles).ToString("dd") +
                                   " de " + Convert.ToDateTime(respuestas[i].Detalles).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " " +
                                   Convert.ToDateTime(respuestas[i].Detalles).ToString("yyyy") + ".")});
                    }
                }
                else if (i == 7)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador de servicios no entregó el SUA de su personal, comentó lo siguiente: "
                            +respuestas[i].Detalles) });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador de servicio entregó el SUA de su personal, la fecha de entrega" +
                                " fue el " + Convert.ToDateTime(respuestas[i].Detalles.Split("|")[0]).ToString("dd") +
                                       " de " + Convert.ToDateTime(respuestas[i].Detalles.Split("|")[0]).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " " +
                                       Convert.ToDateTime(respuestas[i].Detalles.Split("|")[0]).ToString("yyyy") + " correspondiente al mes de " +
                                       respuestas[i].Detalles.Split("|")[1] + ".") });
                    }
                }
            }
            local.DataSources.Add(new ReportDataSource("CedulaFumigacion", cedulas));
            var pdf = local.Render("PDF");
            return File(pdf, "application/pdf");
        }

        [Route("/cedula/muebles/{servicio}/{id?}")]
        public async Task<IActionResult> GeneraCedulaMuebles(int servicio, int id)
        {
            var incidencias = new List<IncidenciasMuebles>();
            LocalReport local = new LocalReport();
            var path = Directory.GetCurrentDirectory() + "\\Reports\\CedulaBienesMuebles.rdlc";
            local.ReportPath = path;
            var cedulas = await vrCedula.getCedulaByServicio(servicio, id);
            var respuestas = await vCedula.obtieneRespuestas(id);
            for (int i = 0; i < respuestas.Count; i++)
            {
                if (i == 0)
                {
                    if (respuestas[i].Respuesta == false) 
                    { 
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador de servicios no cumplió con la fecha y hora solicitada para la prestación del servicio, la fecha en que se solicito fue," +
                                     Convert.ToDateTime(respuestas[i].Detalles.Split("|")[0]) +" y la fecha y hora de llegada fue:"+ Convert.ToDateTime(respuestas[i].Detalles.Split("|")[1])
                                     + ".")});
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador de servicios cumplió con la fecha y hora solicitada para la prestación del servicio.") });
                    }
                }
                else if (i == 1)
                {
                    incidencias = await iMuebles.GetIncidenciasPregunta(id, (i + 1));
                    local.DataSources.Add(new ReportDataSource("IncidenciasPregunta2", incidencias));
                    if (respuestas[i].Respuesta == false && respuestas[i].Detalles != "N/A")
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador no cumplió con la maquinaria, equipo y herramientas para la prestación del servicio.") });
                        
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador cumplió con la maquinaria, equipo y herramientas para la prestación del servicio.") });
                    }
                }
                else if (i == 2)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador no cumplió con la unidad de transporte solicitada para la prestación del servicio.") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador cumplió con la unidad de transporte solicitada para la prestación del servicio.") });
                    }
                }
                else if (i == 3)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador no cumplió con el personal necesario para realizar la prestación del servicio, se solicitaron "+
                            respuestas[i].Detalles.Split("|")[0]+" persona(s) y se nos brindaron "+respuestas[i].Detalles.Split("|")[1]+" persona(s) para efectuar el servicio.") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador cumplió con el personal necesario para realizar la prestación del servicio.") });
                    }
                }
                else if (i == 4)
                {
                    incidencias = await iMuebles.GetIncidenciasPregunta(id, (i + 1));
                    local.DataSources.Add(new ReportDataSource("IncidenciasMuebles", incidencias));
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal no cumplió con las actividades contempladas en el programa de operación, las cuales se describen a continuación:") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal cumplió con las actividades contempladas en el programa de operación, no presentó incidencias en el mes.") });
                    }
                }
            }
            local.DataSources.Add(new ReportDataSource("CedulaMuebles", cedulas));
            var pdf = local.Render("PDF");
            return File(pdf, "application/pdf");
        }

        [Route("/cedula/trasladoExp/{servicio}/{id?}")]
        public async Task<IActionResult> GeneraCedulaTraslado(int servicio, int id)
        {
            var incidencias = new List<IncidenciasTraslado>();
            LocalReport local = new LocalReport();
            var path = Directory.GetCurrentDirectory() + "\\Reports\\CedulaTraslado.rdlc";
            local.ReportPath = path;
            var cedulas = await vrCedula.getCedulaByServicio(servicio, id);
            var respuestas = await vCedula.obtieneRespuestas(id);
            for (int i = 0; i < respuestas.Count; i++)
            {
                if (i == 0)
                {
                    incidencias = await iTraslado.getIncidenciasByPregunta(id, (i + 1));
                    local.DataSources.Add(new ReportDataSource("IncidenciasPregunta1", incidencias));
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal no se presentó el personal solicitado para prestar el servicio, a continuación se describe la incidencia: ") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal se presentó el personal solicitado para prestar el servicio.") });
                    }
                }
                else if (i == 1)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal no se presentó debidamente uniformado e identificado al prestar el servicio.") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal se presentó debidamente uniformado e identificado al prestar el servicio.") });
                    }
                }
                else if (i == 2)
                {
                    incidencias = await iTraslado.getIncidenciasByPregunta(id, (i + 1));
                    local.DataSources.Add(new ReportDataSource("IncidenciasPregunta3", incidencias));
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal no se presentó debidamente uniformado e identificado al prestar el servicio.") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal se presentó debidamente uniformado e identificado al prestar el servicio.") });
                    }
                }
            }
            local.DataSources.Add(new ReportDataSource("CedulaTraslado", cedulas));
            var pdf = local.Render("PDF");
            return File(pdf, "application/pdf");
        }

        [Route("/cedula/celular/{servicio}/{id?}")]
        public async Task<IActionResult> GeneraCedulaCelular(int servicio, int id)
        {
            var incidencias = new List<IncidenciasCelular>();
            LocalReport local = new LocalReport();
            var path = Directory.GetCurrentDirectory() + "\\Reports\\CedulaCelular.rdlc";
            local.ReportPath = path;
            var cedulas = await vrCedula.getCedulaByServicio(servicio, id);
            var respuestas = await vCedula.obtieneRespuestas(id);
            for (int i = 0; i < respuestas.Count; i++)
            {
                    incidencias = await iCelular.ListIncidenciasTipoCelular(id, tiposCelular[i]);
                    local.DataSources.Add(new ReportDataSource("Incidencias" + tiposCelular[i], incidencias));
                if (respuestas[i].Respuesta == true)
                {
                    local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "Se presentarón " + rCelular[i] + " en el mes y se describen a continuación: ") });
                }
                else
                {
                    local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No se presentarón " + rCelular[i] + " en el mes.") });
                }
            }
            local.DataSources.Add(new ReportDataSource("CedulaCelular", cedulas));
            var pdf = local.Render("PDF");
            return File(pdf, "application/pdf");
        }

        [Route("/cedula/convencional/{servicio}/{id?}")]
        public async Task<IActionResult> GeneraCedulaConvencional(int servicio, int id)
        {
            var incidencias = new List<IncidenciasConvencional>();
            LocalReport local = new LocalReport();
            var path = Directory.GetCurrentDirectory() + "\\Reports\\CedulaConvencional.rdlc";
            local.ReportPath = path;
            var cedulas = await vrCedula.getCedulaByServicio(servicio, id);
            var respuestas = await vCedula.obtieneRespuestas(id);
            for (int i = 0; i < respuestas.Count; i++)
            {
                incidencias = await iConvencional.ListIncidenciasTipoConvencional(id, tiposConv[i]);
                local.DataSources.Add(new ReportDataSource("Incidencias" + tiposConv[i], incidencias));
                if (respuestas[i].Respuesta == true)
                {
                    local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "Se presentarón " + rConv[i] + " en el mes y se describen a continuación: ") });
                }
                else
                {
                    local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No se presentarón " + rConv[i] + " en el mes.") });
                }
            }
            local.DataSources.Add(new ReportDataSource("CedulaConvencional", cedulas));
            var pdf = local.Render("PDF");
            return File(pdf, "application/pdf");
        }

        [Route("/cedula/agua/{servicio}/{id?}")]
        public async Task<IActionResult> GeneraCedulaAgua(int servicio, int id)
        {
            var incidencias = new List<IncidenciasAgua>();
            LocalReport local = new LocalReport();
            var path = Directory.GetCurrentDirectory() + "\\Reports\\CedulaAguaParaBeber.rdlc";
            local.ReportPath = path;
            var cedulas = await vrCedula.getCedulaByServicio(servicio, id);
            var respuestas = await vCedula.obtieneRespuestas(id);
            for (int i = 0; i < respuestas.Count; i++)
            {
                if (i == 0)
                {
                    local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El cierre de mes se efectuó el " + Convert.ToDateTime(respuestas[i].Detalles).ToString("dd") +
                                   " de " + Convert.ToDateTime(respuestas[i].Detalles).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " " + Convert.ToDateTime(respuestas[i].Detalles).ToString("yyyy") + ".")});
                }
                else if (i == 1 || i == 2 || i == 3)
                {
                    incidencias = await iAgua.GetIncidenciasPregunta(id, respuestas[i].Pregunta);
                    local.DataSources.Add(new ReportDataSource("Incidencias" + incidenciasAgua[i], incidencias));
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + respuestas[i].Pregunta, "Se presentaron Incidencias en el inmueble, las cuales se describen a continuación: ") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No se presentarón incidencias en el inmueble en el mes de evaluación.") });
                    }
                }
                else if (i == 4)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador no cumplió con la regulación vigente de los garrafones.") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador cumplió con la regulación vigente de los garrafones.") });
                    }
                }
                else if (i == 5)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio no entregó el reporte de servicio en tiempo y forma.") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador de servicio entregó el reporte de servicio en tiempo y forma, la fecha de entrega" +
                                " fue el " + Convert.ToDateTime(respuestas[i].Detalles).ToString("dd") +
                                       " de " + Convert.ToDateTime(respuestas[i].Detalles).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " " +
                                       Convert.ToDateTime(respuestas[i].Detalles).ToString("yyyy") + ".") });
                    }
                }
            }
            local.DataSources.Add(new ReportDataSource("CedulaAgua", cedulas));
            var pdf = local.Render("PDF");
            return File(pdf, "application/pdf");
        }

        [Route("/cedula/residuos/{servicio}/{id?}")]
        public async Task<IActionResult> GeneraCedulaRPBI(int servicio, int id)
        {
            var incidencias = new List<IncidenciasResiduos>();
            LocalReport local = new LocalReport();
            var path = Directory.GetCurrentDirectory() + "\\Reports\\CedulaResiduos.rdlc";
            local.ReportPath = path;
            var cedulas = await vrCedula.getCedulaByServicio(servicio, id);
            var respuestas = await vCedula.obtieneRespuestas(id);
            for (int i = 0; i < respuestas.Count; i++)
            {

                if (i == 0)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El servicio no se llevó a cabo en el día programado, se llevó a cabo en la fecha " + Convert.ToDateTime(respuestas[i].Detalles).ToString("dd") +
                                   " de " + Convert.ToDateTime(respuestas[i].Detalles).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " " + Convert.ToDateTime(respuestas[i].Detalles).ToString("yyyy") + ".")});
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El servicio si se llevó a cabo en el día programado.") });
                    }
                }
                else if (i == 1)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal de servicio no presentó identificación en todo momento.") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal de servicio presentó identificación en todo momento.") });
                    }
                }
                else if (i == 2)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "La recolección no se llevó a cabo de acuerdo a las condiciones técnicas descritas en el anexo técnico.") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "La recolección se llevó a cabo de acuerdo a las condiciones técnicas descritas en el anexo técnico.") });
                    }
                }
                else if (i == 3)
                {
                    incidencias = await iResiduos.getIncidenciasTipo(id, "ManifiestoEntrega");
                    local.DataSources.Add(new ReportDataSource("IncidenciasManifiesto", incidencias));
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "Faltaron datos en el manifiesto de entrega, los cuales se describen a continuación: ") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El manifiesto de entrega se entrego con todos los datos necesarios.") });
                    }
                }
                else if (i == 4)
                {
                    incidencias = await iResiduos.getIncidencias(id);
                    local.DataSources.Add(new ReportDataSource("IncidenciasResiduos", incidencias));
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "Se presentarón incidencias en el mes, las cuales se describen a continuación: ") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No se presentarón incidencias en el mes.") });
                    }
                }
            }
            local.DataSources.Add(new ReportDataSource("CedulaResiduos", cedulas));
            var pdf = local.Render("PDF");
            return File(pdf, "application/pdf");
        }

        [Route("/cedula/analisis/{servicio}/{id?}")]
        public async Task<IActionResult> GeneraCedulaAnalisis(int servicio, int id)
        {
            var incidencias = new List<IncidenciasAnalisis>();
            LocalReport local = new LocalReport();
            var path = Directory.GetCurrentDirectory() + "\\Reports\\CedulaAnalisis.rdlc";
            local.ReportPath = path;
            var cedulas = await vrCedula.getCedulaByServicio(servicio, id);
            var respuestas = await vCedula.obtieneRespuestas(id);
            for (int i = 0; i < respuestas.Count; i++)
            {
                if (i == 0)
                {
                    local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El cierre de mes se realizó el día: " +
                        Convert.ToDateTime(respuestas[i].Detalles).ToString("dd") +" del mes "+ Convert.ToDateTime(respuestas[i].Detalles).ToString("MMMM", CultureInfo.CreateSpecificCulture("es"))+
                        " de "+Convert.ToDateTime(respuestas[i].Detalles).ToString("yyyy")+".") });
                }
                else if (i == 1)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El servicio no se llevo a cabo en la fecha programada, la fecha en que se realizo "+
                            " fue el " + Convert.ToDateTime(respuestas[i].Detalles).ToString("dd") +
                                       " de " + Convert.ToDateTime(respuestas[i].Detalles).ToString("MMMM", CultureInfo.CreateSpecificCulture("es")) + " " +
                                       Convert.ToDateTime(respuestas[i].Detalles).ToString("yyyy") +".") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El servicio se llevó a cabo en la fecha programada.") });
                    }
                }
                else if (i == 2)
                {
                    incidencias = await iAnalisis.GetIncidenciasPregunta(id, respuestas[i].Pregunta);
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal de servicio no cumplió con la maquinaria, equipo o  herramientas necesarias para prestar el servicio, se describen las incidencias: .") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal de servicio cumplió con la maquinaria, equipo o  herramientas necesarias para prestar el servicio.") });
                    }
                    local.DataSources.Add(new ReportDataSource("IncidenciasAnalisis", incidencias));
                }
                else if (i == 3)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio no portó uniforme e identificación en todo momento: ") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio portó uniforme e identificación en todo momento.") });
                    }
                }
                else if (i == 4)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio recolectó un total de " + respuestas[i].Detalles + " muestras, no cumpliendo " +
                        " con el número de muestras minimas de 7.") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio si recolectó el número total de muestras solicitadas.") });
                    }
                }
                else if (i == 5)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador no se llevó a cabo el servicio en la fecha programada.") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador llevó a cabo el servicio en la fecha programada.") });
                    }
                }
                else if (i == 6)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio no entregó el reporte del servicio.") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio entregó el reporte del servicio el día: " +
                        Convert.ToDateTime(respuestas[i].Detalles).ToString("dd")+" de "+Convert.ToDateTime(respuestas[i].Detalles).ToString("MMMM", CultureInfo.CreateSpecificCulture("es"))
                        +" de "+Convert.ToDateTime(respuestas[i].Detalles).ToString("yyyy")+".") });
                    }
                }
            }
            local.DataSources.Add(new ReportDataSource("CedulaAnalisis", cedulas));
            var pdf = local.Render("PDF");
            return File(pdf, "application/pdf");
        }

        [Route("/cedula/transporte/{servicio}/{id?}")]
        public async Task<IActionResult> GeneraCedulaTransporte(int servicio, int id)
        {
            var incidencias = new List<IncidenciasTransporte>();
            LocalReport local = new LocalReport();
            var path = Directory.GetCurrentDirectory() + "\\Reports\\CedulaTransporte.rdlc";
            local.ReportPath = path;
            var cedulas = await vrCedula.getCedulaByServicio(servicio, id);
            var respuestas = await vCedula.obtieneRespuestas(id);
            for (int i = 0; i < respuestas.Count; i++)
            {
                if (i == 0)
                {
                    local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El cierre de mes se realizó el día: " +
                        Convert.ToDateTime(respuestas[i].Detalles).ToString("dd") +" del mes "+ Convert.ToDateTime(respuestas[i].Detalles).ToString("MMMM", CultureInfo.CreateSpecificCulture("es"))+
                        " de "+Convert.ToDateTime(respuestas[i].Detalles).ToString("yyyy")+".") });
                }
                else if (i == 1)
                {
                    incidencias = await iTransporte.GetIncidenciasPregunta(id, respuestas[i].Pregunta);
                    local.DataSources.Add(new ReportDataSource("IncidenciasPregunta"+(i+1), incidencias));
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El servicio presentó incidencias en el mes, se describen a continuación:") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal de servicio cumplió con los horarios y recorridos establecidos en el contrato.") });
                    }
                }
                else if (i == 2)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "Se llevó a cabo el registro de los servidores públicos en este mes.") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No se llevó a cabo el registro de los servidores públicos en este mes, los comentarios son los siguientes: " +
                        respuestas[i].Detalles) });
                    }
                }
                else if (i == 3)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "La ocupación promedio fue menor o igual al 50%, los comentarios son los siguientes: " +
                            respuestas[i].Detalles) });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "La ocupación promedio fue mayor o igual al 50%.") });
                    }
                }
                else if (i == 4)
                {
                    incidencias = await iTransporte.GetIncidenciasPregunta(id, respuestas[i].Pregunta);
                    local.DataSources.Add(new ReportDataSource("IncidenciasPregunta" + (i + 1), incidencias));
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal no cumplió con la supervisión del servicio, se presentaron las siguientes incidencias: ") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal cumplió con la supervisión del servicio.") });
                    }
                }
                else if (i == 5)
                {
                    incidencias = await iTransporte.GetIncidenciasPregunta(id, respuestas[i].Pregunta);
                    local.DataSources.Add(new ReportDataSource("IncidenciasPregunta" + (i + 1), incidencias));
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal no se comportó de forma cortés, amable y portó el uniforme e identificación de forma correcta. ") });
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El personal se comportó de forma cortés, amable y portó el uniforme e identificación de forma correcta.") });
                    }
                }
            }
            local.DataSources.Add(new ReportDataSource("CedulaTransporte", cedulas));
            var pdf = local.Render("PDF");
            return File(pdf, "application/pdf");
        }

        [Route("/financieros/limpieza")]
        public async Task<IActionResult> ReporteLimpieza()
        {
            LocalReport local = new LocalReport();
            var path = Directory.GetCurrentDirectory() + "\\Reports\\ReporteFinancieros.rdlc";
            local.ReportPath = path;
            var cedulas = await vrCedula.getReporteFinancierosLimpieza();
            local.DataSources.Add(new ReportDataSource("ReporteFinancierosLimpieza", cedulas));

            var excel = local.Render("EXCELOPENXML");
            return File(excel, "application/msexcel", "ReporteLimpieza_" + DateTime.Now + ".xls");
        }
    }
}
