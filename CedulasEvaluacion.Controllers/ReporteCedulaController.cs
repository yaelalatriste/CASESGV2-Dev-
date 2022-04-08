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

        private string [] tiposIncidencia = {"Recoleccion","Entrega"};
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

        [Route("/mensajeria/cedulaNueva")]
        public async Task<IActionResult> Print()
        {
            string mimtype = "";
            int extension = 1;
            var incidencias = new List<IncidenciasMensajeria>();
            var path = $"{this.web.WebRootPath}\\Reportes\\CedulaMensajeria.rdlc";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            LocalReport local = new LocalReport(path);
            var cedulas = await vrCedula.getReporteMensajeria(56);
            incidencias = await iMensajeria.getIncidenciasByTipoMensajeria(56, "Recoleccion");
            var respuestas = await vMensajeria.obtieneRespuestas(56);
            local.AddDataSource("ReporteMensajeria",cedulas);
            for (int i = 0; i<2; i++)
            {
                incidencias = await iMensajeria.getIncidenciasByTipoMensajeria(56, tiposIncidencia[i]);
                if (i<2) 
                {
                    if (respuestas[i].Respuesta == false) {
                        local.AddDataSource("Incidencias" + tiposIncidencia[i], incidencias);
                        parameters.Add("pregunta" + (i + 1), "Se presentaron Incidencias en el inmueble, las cuales se describen a continuación: ");
                    }
                    else
                    {
                        parameters.Add("pregunta" + (i + 1), "No se presentarón incidencias en el inmueble en el mes de evaluación.");
                    }
                }
                else if (respuestas[i].Respuesta == true && i > 2 && i<6)
                {
                    if (respuestas[i].Respuesta == false)
                    {
                        local.AddDataSource("Incidencias" + tiposIncidencia[i], incidencias);
                        parameters.Add("pregunta" + (i + 1), "Se presentaron Incidencias en el inmueble, las cuales se describen a continuación: ");
                    }
                    else
                    {
                        parameters.Add("pregunta" + (i + 1), "No se presentarón incidencias en el inmueble en el mes de evaluación.");
                    }
                }
            }            

            var result = local.Execute(RenderType.Pdf,extension, parameters,mimtype);
            return File(result.MainStream,"application/pdf");
        }
    }
}
