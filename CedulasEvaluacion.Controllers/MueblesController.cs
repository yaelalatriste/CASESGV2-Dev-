using CedulasEvaluacion.Entities.MMuebles;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.Vistas;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    public class MueblesController : Controller
    {
        private readonly IRepositorioMuebles vMuebles;
        private readonly IRepositorioIncidenciasMuebles iMuebles;
        private readonly IRepositorioEntregablesMuebles eMuebles;
        private readonly IRepositorioInmuebles vInmuebles;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IRepositorioPerfiles vRepositorioPerfiles;
        private readonly IRepositorioFacturas vFacturas;
        private readonly IHostingEnvironment environment;

        public MueblesController(IRepositorioMuebles iVMuebles, IRepositorioInmuebles iVInmueble, IRepositorioUsuarios iVUsuario,
                                    IRepositorioIncidenciasMuebles iiMuebles, IRepositorioEntregablesMuebles eeMuebles,
                                    IRepositorioPerfiles iRepositorioPerfiles, IRepositorioFacturas iFacturas, IHostingEnvironment environment)
        {
            this.vMuebles = iVMuebles ?? throw new ArgumentNullException(nameof(iVMuebles));
            this.iMuebles = iiMuebles ?? throw new ArgumentNullException(nameof(iiMuebles));
            this.eMuebles = eeMuebles ?? throw new ArgumentNullException(nameof(eeMuebles));
            this.vFacturas = iFacturas ?? throw new ArgumentNullException(nameof(iFacturas));
            this.vInmuebles = iVInmueble ?? throw new ArgumentNullException(nameof(iVInmueble));
            this.vUsuarios = iVUsuario ?? throw new ArgumentNullException(nameof(iVUsuario));
            this.vRepositorioPerfiles = iRepositorioPerfiles ?? throw new ArgumentNullException(nameof(iRepositorioPerfiles));
            this.environment = environment;
        }

        [Route("/muebles/index")]
        public async Task<IActionResult> Index()
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "ver");
            if (success == 1)
            {
                List<VCedulas> resultado = new List<VCedulas>();
                resultado = await vMuebles.GetCedulasMuebles(UserId());
                return View(resultado);
            }
            return Redirect("/error/denied");
        }

        [Route("/muebles/nuevaCedula")]
        public async Task<IActionResult> NuevaCedula(CedulaMuebles cedulaMuebles)
        {
            cedulaMuebles = new CedulaMuebles();
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "crear");
            if (success == 1)
            {
                return View(cedulaMuebles);
            }
            return Redirect("/error/denied");
        }


        [Route("/muebles/new")]
        public async Task<IActionResult> insertaCedula([FromBody] CedulaMuebles cedulaMuebles)
        {
            cedulaMuebles.UsuarioId = Convert.ToInt32(User.Claims.ElementAt(0).Value);
            int insert = await vMuebles.insertaCedula(cedulaMuebles);
            if (insert != -1)
            {
                return Ok(insert);
            }
            return BadRequest();
        }

        [Route("/muebles/evaluacion/{id?}")]
        public async Task<IActionResult> Cuestionario(int id)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "actualizar");
            if (success == 1)
            {
                CedulaMuebles cedulaMuebles = await vMuebles.CedulaById(id);
                if (cedulaMuebles.Estatus.Equals("Enviado a DAS") && (@User.Claims.ElementAt(2).Value).Contains("Evaluador"))
                {
                    return Redirect("/error/cedSend");
                }
                cedulaMuebles.inmuebleOrigen = await vInmuebles.inmuebleById(cedulaMuebles.InmuebleOrigenId);
                cedulaMuebles.inmuebleDestino = await vInmuebles.inmuebleById(cedulaMuebles.InmuebleDestinoId);
                cedulaMuebles.RespuestasEncuesta = await vMuebles.obtieneRespuestas(id);
                cedulaMuebles.facturas = await vFacturas.getFacturas(id, cedulaMuebles.ServicioId);
                cedulaMuebles.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedulaMuebles.facturas);
                return View(cedulaMuebles);
            }
            return Redirect("/error/denied");
        }

        [HttpPost]
        [Route("/muebles/evaluation")]
        public async Task<IActionResult> guardaRespuestas([FromBody] List<RespuestasEncuesta> respuestas)
        {
            int success = await vMuebles.GuardaRespuestas(respuestas);
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
        [Route("/muebles/sendCedula/{cedula?}")]
        public async Task<IActionResult> enviaCedula(int cedula)
        {
            int success = 0;
            success = await vMuebles.enviaRespuestas(cedula);
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
        [Route("/muebles/revision/{id}")]
        public async Task<IActionResult> RevisarCedula(int id)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "revisión");
            if (success == 1)
            {
                CedulaMuebles cedMuebles = null;
                cedMuebles = await vMuebles.CedulaById(id);
                cedMuebles.facturas = await vFacturas.getFacturas(id, cedMuebles.ServicioId);//
                cedMuebles.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedMuebles.facturas);
                cedMuebles.inmuebleOrigen = await vInmuebles.inmuebleById(cedMuebles.InmuebleOrigenId);
                cedMuebles.inmuebleDestino = await vInmuebles.inmuebleById(cedMuebles.InmuebleDestinoId);
                cedMuebles.usuarios = await vUsuarios.getUserById(cedMuebles.UsuarioId);
                cedMuebles.iEntregables = await eMuebles.getEntregables(cedMuebles.Id);
                cedMuebles.incidencias = await iMuebles.GetIncidencias(cedMuebles.Id);
                cedMuebles.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedMuebles.RespuestasEncuesta = await vMuebles.obtieneRespuestas(cedMuebles.Id);
                cedMuebles.historialCedulas = new List<HistorialCedulas>();
                cedMuebles.historialCedulas = await vMuebles.getHistorialMuebles(cedMuebles.Id);
                foreach (var user in cedMuebles.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                if (cedMuebles.RespuestasEncuesta.Count < 8 && cedMuebles.Mes.Equals("Enero")) 
                {
                    return Redirect("/muebles/evaluacion/"+id);   
                }
                else
                {
                    return View(cedMuebles);
                }
            }
            return Redirect("/error/denied");
        }

        [HttpGet]
        [Route("/muebles/seguimiento/{id}")]
        public async Task<IActionResult> SeguimientoCedula(int id)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "seguimiento");
            if (success == 1)
            {
                CedulaMuebles cedMuebles = null;
                cedMuebles = await vMuebles.CedulaById(id);
                cedMuebles.facturas = await vFacturas.getFacturas(id, cedMuebles.ServicioId);//
                cedMuebles.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedMuebles.facturas);
                cedMuebles.inmuebleOrigen = await vInmuebles.inmuebleById(cedMuebles.InmuebleOrigenId);
                cedMuebles.inmuebleDestino = await vInmuebles.inmuebleById(cedMuebles.InmuebleDestinoId);
                cedMuebles.usuarios = await vUsuarios.getUserById(cedMuebles.UsuarioId);
                cedMuebles.iEntregables = await eMuebles.getEntregables(cedMuebles.Id);
                cedMuebles.incidencias = await iMuebles.GetIncidencias(cedMuebles.Id);
                cedMuebles.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedMuebles.RespuestasEncuesta = await vMuebles.obtieneRespuestas(cedMuebles.Id);
                cedMuebles.historialCedulas = new List<HistorialCedulas>();
                cedMuebles.historialCedulas = await vMuebles.getHistorialMuebles(cedMuebles.Id);
                foreach (var user in cedMuebles.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                cedMuebles.historialEntregables = new List<HistorialEntregables>();
                cedMuebles.historialEntregables = await eMuebles.getHistorialEntregables(cedMuebles.Id);
                foreach (var user in cedMuebles.historialEntregables)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                return View(cedMuebles);
            }
            return Redirect("/error/denied");
        }

        [HttpPost]
        [Route("/muebles/aprovRechCed")]
        public async Task<IActionResult> aprovacionRechazoCedula([FromBody] CedulaMuebles cedulaMuebles)
        {
            int success = await vMuebles.apruebaRechazaCedula(cedulaMuebles);
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
        [Route("/muebles/historialMuebles")]
        public async Task<IActionResult> historialMuebles([FromBody] HistorialCedulas historialCedulas)
        {
            int success = 0;
            success = await vMuebles.capturaHistorial(historialCedulas);
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
            return "Bienes_Muebles";
        }
    }
}
