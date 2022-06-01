using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.NETCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    public class ReportesFinancierosController : Controller
    {
        private readonly IWebHostEnvironment web;

        private readonly IRepositorioReportesFinancieros vReporte;

        public ReportesFinancierosController(IWebHostEnvironment vweb, IRepositorioReportesFinancieros viReporte)
        {
            this.web = vweb;
            this.vReporte = viReporte;
        }

        [Route("/financieros/reportePAT/{mes}/{anio}")]
        public async Task<IActionResult> GeneraCedulaLimpieza(string mes, int anio)
        {
            LocalReport local = new LocalReport();
            var path = Directory.GetCurrentDirectory() + "\\Reports\\ReportePAT.rdlc";
            local.ReportPath = path;
            var cedulas = await vReporte.GetCedulasFinancieros(mes,anio);            
            local.DataSources.Add(new ReportDataSource("ReportePAT", cedulas));
            local.SetParameters(new[] { new ReportParameter("mes", mes) });
            local.SetParameters(new[] { new ReportParameter("anio", anio+"") });
            var pdf = local.Render("PDF");
            return File(pdf, "application/pdf");
        }
    }
}
