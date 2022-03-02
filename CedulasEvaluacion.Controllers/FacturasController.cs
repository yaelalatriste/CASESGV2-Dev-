using CedulasEvaluacion.Entities.MFacturas;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    [Authorize]
    public class FacturasController : Controller
    {
        private readonly IRepositorioFacturas vFacturas;
        private readonly IHostingEnvironment environment;

        public FacturasController(IRepositorioFacturas iFacturas, IHostingEnvironment environment)
        {
            this.vFacturas = iFacturas ?? throw new ArgumentNullException(nameof(iFacturas));
            this.environment = environment;
        }

        /************************* Facturas Limpieza *****************************/
        
        //obtenemos todas las facturas en base a la cedula
        [HttpGet]
        [Route("/limpieza/getFacturas/{id?}/{servicio?}")]
        public async Task<IActionResult> obtieneFacturas(int id,int servicio)
        {
            List<Facturas> facturas = await vFacturas.getFacturas(id,servicio);
            decimal total = 0;
            if (facturas != null)
            {
                return Ok(facturas);
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("/limpieza/insertaFactura")]
        public async Task<IActionResult> insertaFacturas([FromForm] Facturas facturas)
        {
            int success = await vFacturas.insertaConceptoFacturas(facturas);

            if (success != -1)
            {
                return Ok(success);
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("limpieza/updateFactura")]
        public async Task<IActionResult> updateFacturas([FromForm] Facturas facturas)
        {
            int success = 0;
            success = await vFacturas.updateConceptoFacturas(facturas);
            if (success != 0)
            {
                return Ok(success);
            }
            return NoContent();
        }

        /*Metodo para eliminar Factura*/
        [HttpDelete]
        [Route("limpieza/deleteFactura/{factura?}")]
        public async Task<IActionResult> deleteFactura(int factura)
        {
            int success = 0;
            success = await vFacturas.deleteFactura(factura);
            if (success != 0)
            {
                return Ok(success);
            }
            return NoContent();
        }

        /************************* Fin Facturas Limpieza *****************************/

    }
}
