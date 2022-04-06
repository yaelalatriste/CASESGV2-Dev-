using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using AspNetCore.Reporting;
using CedulasEvaluacion.Interfaces;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    public class ReporteCedulaController:Controller
    {
        private readonly IWebHostEnvironment web;
        private readonly IRepositorioReporteCedula vrCedula;
        
        public ReporteCedulaController(IWebHostEnvironment vweb,IRepositorioReporteCedula vvReporte)
        {
            this.web = vweb;
            this.vrCedula = vvReporte;
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        [Route("/mensajeria/cedulaNueva")]
        public async Task<IActionResult> Print()
        {
            string mimtype = "";
            int extension = 1;
            var path = $"{this.web.WebRootPath}\\Reportes\\ReporteFinancieros.rdlc";
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            //parameters.Add("rp1","Prueba 123");
            var cedulas = await vrCedula.getCedulasMensajeria();
            LocalReport local = new LocalReport(path);
            local.AddDataSource("DataSet1",cedulas);
            var result = local.Execute(RenderType.Pdf,extension, parameters,mimtype);
            return File(result.MainStream,"application/pdf");
        }
    }
}
