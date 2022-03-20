using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.Vistas;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    public class FinancierosController : Controller
    {
        private readonly IRepositorioFinancieros vFinancieros;
        private readonly IRepositorioPerfiles vPerfiles;

        public FinancierosController(IRepositorioFinancieros ivFinancieros, IRepositorioPerfiles ivPerfiles)
        {
            this.vFinancieros = ivFinancieros ?? throw new ArgumentNullException(nameof(ivFinancieros));
            this.vPerfiles = ivPerfiles ?? throw new ArgumentNullException(nameof(ivPerfiles));
        }

        [Route("/financieros/index")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "ver");
            if (success == 1)
            {
                List<Dashboard> resultado = new List<Dashboard>();
                resultado = await vFinancieros.GetCedulasFinancieros();
                return View(resultado);
            }
            return Redirect("/error/denied");
        }

        [Route("/financieros/detalle/{servicio}")]
        [HttpGet]
        public async Task<IActionResult> DetalleServicio(string servicio)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "revisión");
            if (success == 1)
            {
                List<Dashboard> resultado = new List<Dashboard>();
                resultado = await vFinancieros.GetDetalleServicio(servicio);
                return View(resultado);
            }
            return Redirect("/error/denied");
        }

        [Route("/financieros/generaTabla/{servicio}")]
        [HttpGet]
        public async Task<IActionResult> generaFilas(string servicio)
        {
            List<Dashboard> resultado = new List<Dashboard>();
            resultado = await vFinancieros.GetDetalleServicio(servicio);

            var meses = obtieneMeses(resultado);
            var totales = obtieneTotales(resultado,meses);
            var estatus = obtieneEstatus(resultado);
            var columnas = new List<string>();
            var filas = new List<List<string>>();
            double total = 0;

            for (var i=0; i<meses.Count;i++)
            {
                columnas = generaColumnas(estatus);
                for (var j=0;j<resultado.Count;j++)
                {
                    if (meses[i] == resultado[j].Mes)
                    {
                        total = (resultado[j].Total * 100.0)/totales[i];
                        columnas[obtienePosicion(estatus, resultado[j].Estatus)] = 
                            "<td>"+
                                "<div class='row col-lg-12'>" +
                                    "<div class='col-lg-8 mt-2'>" +
                                        "<div class='progress progress-md'>" +
                                            "<div class='progress-bar " + resultado[j].Fondo + "' style = 'width:" + ((resultado[j].Total * 100) / totales[i]) + "%'></div>" +
                                        "</div>" +
                                    "</div>" +
                                    "<div class='col-lg-3'>" +
                                        "<span class='badge " + resultado[j].Fondo + "'>(" + resultado[j].Total + ") " + total.ToString("n2") + "%</span>"+
                                    "</div>" +
                                "</div>" +
                                /*"<div class='row col-lg-12 font-weight-bold'>" +
                                    "Total de Cédulas: " + resultado[j].Total +
                                "</div>" +*/
                            "</td>";
                    }
                }
                filas.Add(columnas);
            }

             return Ok(filas);
        }

        public int obtienePosicion(List<string> estatus, string nEstatus)
        {
            int p = -1;
            for(var i = 0; i < estatus.Count; i++)
            {
                if(estatus.ElementAt(i) == nEstatus)
                {
                    p = i;
                    return p;
                }
            }
            return p;
        }

        public List<string> obtieneMeses(List<Dashboard> dashboards)
        {
            var meses = new List<string>();
            foreach (var dt in dashboards)
            {
                meses.Add(dt.Mes);
            }
            HashSet<string> quitaMeses = new HashSet<string>(meses);
            List<string> lmeses = quitaMeses.ToList();

            return lmeses;
        }

        public List<int> obtieneTotales(List<Dashboard> dashboards, List<string> meses)
        {
            var totales = new List<int>();
            for (var f = 0; f < meses.Count; f++)
            {
                var total = 0;
                for (var c = 0; c < dashboards.Count; c++)
                {
                    if (dashboards[c].Mes == meses[f])
                    {
                        total += dashboards[c].Total;
                    }
                }
                totales.Add(total);
            }

            return totales;
        }

        public List<string> generaColumnas(List<string> estatus)
        {
            var columnas = new List<string>();
            foreach (var dt in estatus)
            {
                columnas.Add("<td></td>");
            }
            
            return columnas;
        }

        public List<string> obtieneEstatus(List<Dashboard> dashboards)
        {
            var estatus = new List<string>();
            foreach (var dt in dashboards)
            {
                estatus.Add(dt.Estatus);
            }
            HashSet<string> quitaEstatus = new HashSet<string>(estatus);
            List<string> lestatus = quitaEstatus.ToList();

            return lestatus;
        }

        private int UserId()
        {
            return Convert.ToInt32(User.Claims.ElementAt(0).Value);
        }

        private string modulo()
        {
            return "Financieros";
        }
    }
}
