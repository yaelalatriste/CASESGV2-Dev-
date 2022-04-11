using CedulasEvaluacion.Entities.MContratos;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    public class ContratosServicioController : Controller
    {
        private readonly IRepositorioContratosServicio vContrato;
        private readonly IRepositorioPerfiles vPerfiles;

        public ContratosServicioController(IRepositorioContratosServicio ivContrato, IRepositorioPerfiles ivPerfiles)
        {
            this.vContrato = ivContrato ?? throw new ArgumentNullException(nameof(ivContrato));
            this.vPerfiles = ivPerfiles ?? throw new ArgumentNullException(nameof(ivPerfiles));
        }

        [HttpGet]
        [Route("/contratos/getContratos/{servicio}")]
        public async Task<IActionResult> GetContratosServicios(int servicio)
        {
            List<ContratosServicio> resultado = await vContrato.GetContratosServicios(servicio);
            if (resultado != null)
            {
                return Ok(resultado);
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("/contratos/getContrato/{servicio}")]
        public async Task<IActionResult> GetContratoServicioActivo(int servicio)
        {
            ContratosServicio resultado = await vContrato.GetContratoServicioActivo(servicio);
            if (resultado != null)
            {
                return Ok(resultado);
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("/contratos/insertaContrato")]
        public async Task<IActionResult> InsertaContrato([FromBody] ContratosServicio contratosServicio)
        {
            int insert = await vContrato.InsertaContrato(contratosServicio);
            if (insert != -1)
            {
                return Ok(insert);
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("/contratos/actualizaContrato")]
        public async Task<IActionResult> ActualizaContrato([FromBody] ContratosServicio contratosServicio)
        {
            int insert = await vContrato.ActualizaContrato(contratosServicio);
            if (insert != -1)
            {
                return Ok(insert);
            }
            return BadRequest();
        }
    }
}
