using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using AspNetCore.Reporting;
using CedulasEvaluacion.Interfaces;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel;
using CedulasEvaluacion.Entities.Reportes;
using CedulasEvaluacion.Entities.MMensajeria;

namespace CedulasEvaluacion.Controllers
{
    public class ReporteCedulaController:Controller
    {

        private string [] tiposIncidencia = {"Recoleccion","Entrega","Acuses", "Mal Estado","Extraviadas","Robadas" };
        private readonly IWebHostEnvironment web;
        private readonly IRepositorioReporteCedula vrCedula;
        private readonly IRepositorioMensajeria vMensajeria;
        private readonly IRepositorioIncidenciasMensajeria iMensajeria;

        public ReporteCedulaController(IWebHostEnvironment vweb,IRepositorioReporteCedula vvReporte, IRepositorioIncidenciasMensajeria iiMensajeria,
            IRepositorioMensajeria viMensajeria)
        {
            this.web = vweb;
            this.vrCedula = vvReporte;
            this.vMensajeria = viMensajeria;
            this.iMensajeria = iiMensajeria;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        [Route("/mensajeria/cedula/{id?}")]
        public async Task<IActionResult> GeneraCedula(int id)
        {
            string mimtype = "";
            int extension = 1;
            var incidencias = new List<IncidenciasMensajeria>();
            var path = $"{this.web.WebRootPath}\\Reportes\\CedulaMensajeria.rdlc";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            LocalReport local = new LocalReport(path);
            var cedulas = await vrCedula.getReporteMensajeria(id);
            var respuestas = await vMensajeria.obtieneRespuestas(id);
            local.AddDataSource("ReporteMensajeria",cedulas);
            for (int i = 0; i<respuestas.Count; i++)
            {
                if (i < 6) {
                    incidencias = await iMensajeria.getIncidenciasByTipoMensajeria(id, tiposIncidencia[i]);
                    tiposIncidencia[i] = tiposIncidencia[i] == "Mal Estado" ? "MalEstado" : tiposIncidencia[i];
                    local.AddDataSource("Incidencias" + tiposIncidencia[i], incidencias);
                }
                if (i < 2)
                {
                    if (respuestas[i].Respuesta == false) {
                        parameters.Add("pregunta" + (i + 1), "Se presentaron Incidencias en el inmueble, las cuales se describen a continuación: ");
                    }
                    else
                    {
                        parameters.Add("pregunta" + (i + 1), "No se presentarón incidencias en el inmueble en el mes de evaluación.");
                    }
                }
                else if (i==2) 
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        parameters.Add("pregunta" + (i + 1), "Se presentaron Incidencias en el inmueble, las cuales se describen a continuación: ");
                    }
                    else
                    {
                        parameters.Add("pregunta" + (i + 1), "No se presentarón incidencias en el inmueble en el mes de evaluación.");
                    }
                }
                else if (i > 2 && i < 6)
                {
                    if (respuestas[i].Respuesta == true)
                    {
                        parameters.Add("pregunta" + (i + 1), "Se presentaron Incidencias en el inmueble, las cuales se describen a continuación: ");
                    }
                    else
                    {
                        parameters.Add("pregunta" + (i + 1), "No se presentarón incidencias en el inmueble en el mes de evaluación.");
                    }
                }
                else
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        parameters.Add("pregunta" + (i + 1), "El prestador del servicio no entregó suficiente material de embalaje para las guías.");
                    }
                    else
                    {
                        parameters.Add("pregunta" + (i + 1), "El prestador del servicio si entregó suficiente material de embalaje para las guías.");
                    }
                }
            }            

            var result = local.Execute(RenderType.Pdf,extension, parameters,mimtype);
            return File(result.MainStream,"application/pdf");
        }
    }
}
