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
