using CedulasEvaluacion.Entities.MCedula;
using CedulasEvaluacion.Entities.MFacturas;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    [Authorize]
    public class FacturasController : Controller
    {
        private readonly IRepositorioFacturas vFacturas;
        private readonly IRepositorioEvaluacionServicios vCedula;
        private readonly IRepositorioInmuebles vInmuebles;
        private readonly IRepositorioPerfiles vPerfiles;
        private readonly IHostingEnvironment environment;

        public FacturasController(IRepositorioEvaluacionServicios viCedula, IRepositorioFacturas iFacturas, IRepositorioInmuebles viInmueble,
            IRepositorioPerfiles ivPerfiles, IHostingEnvironment environment)
        {
            this.vFacturas = iFacturas ?? throw new ArgumentNullException(nameof(iFacturas));
            this.vCedula = viCedula ?? throw new ArgumentNullException(nameof(viCedula));
            this.vInmuebles = viInmueble ?? throw new ArgumentNullException(nameof(viInmueble));
            this.vPerfiles = ivPerfiles ?? throw new ArgumentNullException(nameof(ivPerfiles));
            this.environment = environment;
        }

        /************************* Modulo Facturas *****************************/
        [HttpGet]
        [Route("/facturas/index")]
        public async Task<IActionResult> Index([FromQuery(Name = "Servicio")] int Servicio, [FromQuery(Name = "Mes")] string Mes, 
            [FromQuery(Name = "Tipo")] string Tipo)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "ver");
            if (success == 1)
            {
                ModelsFacturas resultado = new ModelsFacturas();
                resultado.facturasMes = await vFacturas.getFacturasTipo("Mes");
                resultado.facturasServicio = await vFacturas.getFacturasTipo("Servicio");
                resultado.facturasParciales = await vFacturas.getFacturasTipo("Parciales");
                if (Servicio != 0)
                {
                    if (Mes!= null && Tipo != null)
                    {
                        resultado.detalle = await vFacturas.getDetalleFacturacion(Servicio,Mes,Tipo);
                    }
                    resultado.desgloceServicio = await vFacturas.getDesgloceFacturacion(Servicio);
                }
                return View(resultado);
            }
            return Redirect("/error/denied");
        }
        /*********************** Fin Modulo Facturas ***************************/

        /************************* Facturas *****************************/

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
            CedulaEvaluacion cedula = null;
            if (success == 1)
            {
                return Ok(success);
            }
            else if (success != -1)
            {
                cedula = await vCedula.CedulaById(success);
                cedula.inmuebles = await vInmuebles.inmuebleById(cedula.InmuebleId);
                return Ok(cedula);
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("limpieza/updateFactura")]
        public async Task<IActionResult> updateFacturas([FromForm] Facturas facturas)
        {
            int success = 0;
            success = await vFacturas.updateConceptoFacturas(facturas);
            CedulaEvaluacion cedula = null;
            if (success == 1)
            {
                return Ok(success);
            }
            else if (success != -1)
            {
                cedula = await vCedula.CedulaById(success);
                cedula.inmuebles = await vInmuebles.inmuebleById(cedula.InmuebleId);
                return Ok(cedula);
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

        /*Filtros de Facturas*/
        [HttpGet]
        [Route("/facturas/getFacturasPago/{servicio}")]
        public async Task<IActionResult> getInmueblesPago(int servicio)
        {
            List<Facturas> inmueble = null;
            inmueble = await vFacturas.getFacturasPago(servicio);
            if (inmueble != null)
            {
                return Ok(inmueble);
            }
            return BadRequest();
        }

        private int UserId()
        {
            return Convert.ToInt32(User.Claims.ElementAt(0).Value);
        }

        private string modulo()
        {
            return "Facturación";
        }

    }
}
