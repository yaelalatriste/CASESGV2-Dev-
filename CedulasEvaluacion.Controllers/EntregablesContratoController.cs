using CedulasEvaluacion.Entities.MContratos;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    public class EntregablesContratoController : Controller
    {
        private readonly IRepositorioEntregablesContrato eContrato;
        private readonly IHostingEnvironment environment;

        public EntregablesContratoController(IRepositorioEntregablesContrato evContrato, IHostingEnvironment environment)
        {
            this.eContrato = evContrato ?? throw new ArgumentNullException(nameof(evContrato));
            this.environment = environment;
        }
        /*Metodo para adjuntar los entregables*/
        [HttpPost]
        [Route("/contrato/insertaObligacion")]
        public async Task<IActionResult> insertaObligacion([FromForm] EntregablesContrato entregables)
        {
            int success = 0;
            success = await eContrato.InsertaContrato(entregables);
            if (success != 0)
            {
                return Ok(success);
            }
            else
            {
                return BadRequest();
            }
        }
        /*Fin del Metodo para adjuntar los entregables*/
    }
}
