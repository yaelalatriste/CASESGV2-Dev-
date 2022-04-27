﻿using Microsoft.AspNetCore.Mvc;
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

namespace CedulasEvaluacion.Controllers
{
    public class ReporteCedulaController:Controller
    {

        private string [] tiposIncidencia = {"Recoleccion","Entrega","Acuses", "Mal Estado","Extraviadas","Robadas" };
        private string[] tiposCelular = { "Alta_Equipo", "Alta", "Baja", "Reactivacion", "Suspension", "Perfil", "SIM", "CambioNumeroRegion", 
                                          "VozDatos", "Diagnostico", "Reparacion"};
        private string[] rCelular = { "altas con entrega de equipo", "altas sin entrega de equipo", "bajas de servicio", "reactivaciones por robo/extravío", "suspensiones por robo/extravío", "cambios de perfil", "switcheos de SIM Card", "cambios de número o región",
                                          "solicitudes de voz y/o datos", "solicitudes de diagnóstico de equipo", "solicitudes de reparación de equipo"};


        private readonly IWebHostEnvironment web;
        private readonly IRepositorioEvaluacionServicios vCedula;
        
        private readonly IRepositorioReporteCedula vrCedula;
        private readonly IRepositorioMensajeria vMensajeria;
        private readonly IRepositorioIncidenciasMensajeria iMensajeria;
        private readonly IRepositorioLimpieza vLimpieza;
        private readonly IRepositorioIncidencias iLimpieza;
        private readonly IRepositorioIncidenciasMuebles iMuebles;
        private readonly IRepositorioIncidenciasCelular iCelular;

        public ReporteCedulaController(IWebHostEnvironment vweb,IRepositorioReporteCedula vvReporte, IRepositorioIncidenciasMensajeria iiMensajeria,
            IRepositorioMensajeria viMensajeria, IRepositorioLimpieza viLimpieza, IRepositorioIncidencias iiLimpieza, IRepositorioEvaluacionServicios viCedula, 
            IRepositorioIncidenciasMuebles IiMuebles, IRepositorioIncidenciasCelular iiCelular)
        {
            this.web = vweb;
            this.vCedula = viCedula;
            this.vrCedula = vvReporte;
            this.vMensajeria = viMensajeria;
            this.iMensajeria = iiMensajeria;
            this.vLimpieza = viLimpieza;
            this.iLimpieza = iiLimpieza;
            this.iMuebles = IiMuebles;
            this.iCelular = iiCelular;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        [Route("/cedula/mensajeria/{id?}")]
        public async Task<IActionResult> GeneraCedula(int id)
        {
            var incidencias = new List<IncidenciasMensajeria>();
            LocalReport local = new LocalReport();
            var path = Directory.GetCurrentDirectory()+ "\\Reports\\CedulaMensajeria.rdlc";
            local.ReportPath = path;
            var cedulas = await vrCedula.getReporteMensajeria(id);
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

        [Route("/cedula/limpieza/{id?}")]
        public async Task<IActionResult> GeneraCedulaLimpieza(int id)
        {
            var incidencias = new List<VIncidenciasLimpieza>();
            LocalReport local = new LocalReport();
            var path = Directory.GetCurrentDirectory() + "\\Reports\\CedulaLimpieza.rdlc";
            local.ReportPath = path;
            var cedulas = await vrCedula.getReporteLimpieza(id);
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
                        incidencias = await iLimpieza.getIncidencias(id);
                        local.DataSources.Add(new ReportDataSource("IncidenciasPO", incidencias));
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No se presentarón incidencias en el inmueble en el mes de evaluación.") });
                        local.DataSources.Add(new ReportDataSource ("IncidenciasPO", incidencias));
                    }
                }
                else if (i == 2)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No aplica la entrega de acuses en el mes de evaluación.")});
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No se presentarón incidencias en el inmueble en el mes de evaluación.")});
                    }
                }
                else if (i == 3)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "Se presentaron incidencias en el inmueble, las cuales se describen a continuación: ")});
                        incidencias = await iLimpieza.getIncidenciasEquipo(id);
                        local.DataSources.Add(new ReportDataSource("IncidenciasEquipo", incidencias));
                    }
                    else
                    {
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "No se presentarón incidencias en el inmueble en el mes de evaluación.")});
                        local.DataSources.Add(new ReportDataSource("IncidenciasEquipo", incidencias));
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
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador del servicio no entregó el SUA de su personal en tiempo y forma.")});
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
                    if (respuestas[i].Respuesta == false)
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
                        local.SetParameters(new[] { new ReportParameter("pregunta" + (i + 1), "El prestador no cumplió con el personal necesario para realizar la prestación del servicio.") });
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
