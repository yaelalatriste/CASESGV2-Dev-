using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.MTransporte;
using CedulasEvaluacion.Entities.Vistas;
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
    public class TransporteController : Controller
    {
        private readonly IRepositorioTransporte vTransporte;
        private readonly IRepositorioIncidenciasTransporte iTransporte;
        private readonly IRepositorioEntregablesTransporte eTransporte;
        private readonly IRepositorioInmuebles vInmuebles;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IRepositorioPerfiles vRepositorioPerfiles;
        private readonly IRepositorioFacturas vFacturas;
        private readonly IHostingEnvironment environment;

        public TransporteController(IRepositorioTransporte iVTransporte, IRepositorioInmuebles iVInmueble, IRepositorioUsuarios iVUsuario,
                                    IRepositorioIncidenciasTransporte iiTransporte, IRepositorioEntregablesTransporte eeTransporte,
                                    IRepositorioPerfiles iRepositorioPerfiles, IRepositorioFacturas iFacturas, IHostingEnvironment environment)
        {
            this.vTransporte = iVTransporte ?? throw new ArgumentNullException(nameof(iVTransporte));
            this.iTransporte = iiTransporte ?? throw new ArgumentNullException(nameof(iiTransporte));
            this.eTransporte = eeTransporte ?? throw new ArgumentNullException(nameof(eeTransporte));
            this.vFacturas = iFacturas ?? throw new ArgumentNullException(nameof(iFacturas));
            this.vInmuebles = iVInmueble ?? throw new ArgumentNullException(nameof(iVInmueble));
            this.vUsuarios = iVUsuario ?? throw new ArgumentNullException(nameof(iVUsuario));
            this.vRepositorioPerfiles = iRepositorioPerfiles ?? throw new ArgumentNullException(nameof(iRepositorioPerfiles));
            this.environment = environment;
        }

        [Route("/transporte/index")]
        public async Task<IActionResult> Index()
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "ver");
            if (success == 1)
            {
                List<VCedulas> resultado = new List<VCedulas>();
                resultado = await vTransporte.GetCedulasTransporte(UserId());
                return View(resultado);
            }
            return Redirect("/error/denied");
        }

        [Route("/transporte/nuevaCedula")]
        public async Task<IActionResult> NuevaCedula(CedulaTransporte cedulaTransporte)
        {
            cedulaTransporte = new CedulaTransporte();
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "crear");
            if (success == 1)
            {
                return View(cedulaTransporte);
            }
            return Redirect("/error/denied");
        }

        [Route("/transporte/validaPeriodo/{anio?}/{mes?}/{inmueble?}")]
        public async Task<IActionResult> validaPeriodo(int anio, string mes, int inmueble)
        {
            int valida = await vTransporte.VerificaCedula(anio, mes, inmueble);
            if (valida != -1)
            {
                return Ok(valida);
            }
            return BadRequest();
        }

        [Route("/transporte/new")]
        public async Task<IActionResult> insertaCedula([FromBody] CedulaTransporte cedulaTransporte)
        {
            cedulaTransporte.UsuarioId = Convert.ToInt32(User.Claims.ElementAt(0).Value);
            int insert = await vTransporte.insertaCedula(cedulaTransporte);
            if (insert != -1)
            {
                return Ok(insert);
            }
            return BadRequest();
        }

        [Route("/transporte/evaluacion/{id?}")]
        public async Task<IActionResult> Cuestionario(int id)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "actualizar");
            if (success == 1)
            {
                CedulaTransporte cedulaTransporte = await vTransporte.CedulaById(id);
                if (cedulaTransporte.Estatus.Equals("Enviado a DAS"))
                {
                    return Redirect("/error/cedSend");
                }
                cedulaTransporte.inmuebles = await vInmuebles.inmuebleById(cedulaTransporte.InmuebleId);
                cedulaTransporte.RespuestasEncuesta = await vTransporte.obtieneRespuestas(id);
                cedulaTransporte.facturas = await vFacturas.getFacturas(id, cedulaTransporte.ServicioId);
                cedulaTransporte.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedulaTransporte.facturas);
                return View(cedulaTransporte);
            }
            return Redirect("/error/denied");
        }

        [HttpPost]
        [Route("/transporte/evaluation")]
        public async Task<IActionResult> guardaRespuestas([FromBody] List<RespuestasEncuesta> respuestas)
        {
            int success = await vTransporte.GuardaRespuestas(respuestas);
            if (success != 0)
            {
                return Ok(success);
            }
            else
            {
                return NotFound();
            }
        }

        //Va guardando las respuestas de la evaluacion
        [HttpPost]
        [Route("/transporte/sendCedula/{cedula?}")]
        public async Task<IActionResult> enviaCedula(int cedula)
        {
            int success = 0;
            success = await vTransporte.enviaRespuestas(cedula);
            if (success != -1)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("/transporte/revision/{id}")]
        public async Task<IActionResult> RevisarCedula(int id)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "revisión");
            if (success == 1)
            {
                CedulaTransporte cedTran = null;
                cedTran = await vTransporte.CedulaById(id);
                cedTran.facturas = await vFacturas.getFacturas(id, cedTran.ServicioId);//
                cedTran.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedTran.facturas);
                cedTran.inmuebles = await vInmuebles.inmuebleById(cedTran.InmuebleId);
                cedTran.usuarios = await vUsuarios.getUserById(cedTran.UsuarioId);
                cedTran.iEntregables = await eTransporte.getEntregables(cedTran.Id);
                cedTran.incidencias = await iTransporte.GetIncidencias(cedTran.Id);
                cedTran.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedTran.RespuestasEncuesta = await vTransporte.obtieneRespuestas(cedTran.Id);
                cedTran.historialCedulas = new List<HistorialCedulas>();
                cedTran.historialCedulas = await vTransporte.getHistorialTransporte(cedTran.Id);
                foreach (var user in cedTran.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                return View(cedTran);
            }
            return Redirect("/error/denied");
        }

        [HttpGet]
        [Route("/transporte/seguimiento/{id}")]
        public async Task<IActionResult> SeguimientoCedula(int id)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "seguimiento");
            if (success == 1)
            {
                CedulaTransporte cedFum = null;
                cedFum = await vTransporte.CedulaById(id);
                cedFum.facturas = await vFacturas.getFacturas(id, cedFum.ServicioId);//
                cedFum.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedFum.facturas);
                cedFum.inmuebles = await vInmuebles.inmuebleById(cedFum.InmuebleId);
                cedFum.usuarios = await vUsuarios.getUserById(cedFum.UsuarioId);
                cedFum.iEntregables = await eTransporte.getEntregables(cedFum.Id);
                cedFum.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedFum.RespuestasEncuesta = await vTransporte.obtieneRespuestas(cedFum.Id);
                cedFum.historialCedulas = new List<HistorialCedulas>();
                cedFum.historialCedulas = await vTransporte.getHistorialTransporte(cedFum.Id);
                foreach (var user in cedFum.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                cedFum.historialEntregables = new List<HistorialEntregables>();
                cedFum.historialEntregables = await eTransporte.getHistorialEntregables(cedFum.Id);
                foreach (var user in cedFum.historialEntregables)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                return View(cedFum);
            }
            return Redirect("/error/denied");
        }

        [HttpPost]
        [Route("/transporte/aprovRechCed")]
        public async Task<IActionResult> aprovacionRechazoCedula([FromBody] CedulaTransporte cedulaTransporte)
        {
            int success = await vTransporte.apruebaRechazaCedula(cedulaTransporte);
            if (success != 0)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("/transporte/historialTransporte")]
        public async Task<IActionResult> historialTransporte([FromBody] HistorialCedulas historialCedulas)
        {
            int success = 0;
            success = await vTransporte.capturaHistorial(historialCedulas);
            if (success != 0)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        /*Datos del Modulo*/
        private int UserId()
        {
            return Convert.ToInt32(User.Claims.ElementAt(0).Value);
        }

        private string modulo()
        {
            return "Transporte_de_Personal";
        }


    }
}
