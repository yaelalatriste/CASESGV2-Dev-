using CedulasEvaluacion.Entities.MCatalogoServicios;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    public class CatalogoServiciosController:Controller
    {
        private readonly IRepositorioCatalogoServicios vCatalogo;
        private readonly IRepositorioPerfiles vPerfiles;

        public CatalogoServiciosController(IRepositorioCatalogoServicios ivCatalogo, IRepositorioPerfiles ivPerfiles)
        {
            this.vCatalogo = ivCatalogo ?? throw new ArgumentNullException(nameof(ivCatalogo));
            this.vPerfiles = ivPerfiles ?? throw new ArgumentNullException(nameof(ivPerfiles));
        }

        [HttpGet]
        [Route("/catalogo/servicios")]
        public async Task<IActionResult> GetCatalogoServicios()
        {
            List<CatalogoServicios> resultado = await vCatalogo.GetCatalogoServicios();
            if (resultado != null) {
                return Ok(resultado);
            }
            return BadRequest();
        }
    }
}
