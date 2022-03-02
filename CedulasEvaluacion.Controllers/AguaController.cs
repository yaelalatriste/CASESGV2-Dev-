using CedulasEvaluacion.Entities.MAgua;
using CedulasEvaluacion.Entities.Models;
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
    public class AguaController : Controller
    {
        private readonly IRepositorioAgua vAgua;
        private readonly IRepositorioIncidenciasAgua iAgua;
        private readonly IRepositorioEntregablesAgua eAgua;
        private readonly IRepositorioInmuebles vInmuebles;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IRepositorioPerfiles vRepositorioPerfiles;
        private readonly IRepositorioFacturas vFacturas;
        private readonly IHostingEnvironment environment;

        public AguaController(IRepositorioAgua iVAgua, IRepositorioInmuebles iVInmueble, IRepositorioUsuarios iVUsuario,
                                    IRepositorioIncidenciasAgua iiAgua, IRepositorioEntregablesAgua eeAgua,
                                    IRepositorioPerfiles iRepositorioPerfiles, IRepositorioFacturas iFacturas, IHostingEnvironment environment)
        {
            this.vAgua = iVAgua ?? throw new ArgumentNullException(nameof(iVAgua));
            this.iAgua = iiAgua ?? throw new ArgumentNullException(nameof(iiAgua));
            this.eAgua = eeAgua ?? throw new ArgumentNullException(nameof(eeAgua));
            this.vFacturas = iFacturas ?? throw new ArgumentNullException(nameof(iFacturas));
            this.vInmuebles = iVInmueble ?? throw new ArgumentNullException(nameof(iVInmueble));
            this.vUsuarios = iVUsuario ?? throw new ArgumentNullException(nameof(iVUsuario));
            this.vRepositorioPerfiles = iRepositorioPerfiles ?? throw new ArgumentNullException(nameof(iRepositorioPerfiles));
            this.environment = environment;
        }

        //Metodo que regresa las cedulas aceptadas, guardadas o rechazadas 
        [Route("/agua/index")]
        public async Task<IActionResult> Index()
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "ver");
            if (success == 1)
            {
                List<VCedulas> resultado = new List<VCedulas>();
                resultado = await vAgua.GetCedulasAgua(UserId());
                return View(resultado);
            }
            return Redirect("/error/denied");
        }

        [Route("/agua/nuevaCedula")]
        public async Task<IActionResult> NuevaCedula(CedulaAgua cedulaAgua)
        {
            cedulaAgua = new CedulaAgua();
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "crear");
            if (success == 1)
            {
                return View(cedulaAgua);
            }
            return Redirect("/error/denied");
        }

        [Route("/agua/validaPeriodo/{anio?}/{mes?}/{inmueble?}")]
        public async Task<IActionResult> validaPeriodo(int anio, string mes, int inmueble)
        {
            int valida = await vAgua.VerificaCedula(anio, mes, inmueble);
            if (valida != -1)
            {
                return Ok(valida);
            }
            return BadRequest();
        }

        [Route("/agua/new")]
        public async Task<IActionResult> insertaCedula([FromBody] CedulaAgua cedulaAgua)
        {
            cedulaAgua.UsuarioId = Convert.ToInt32(User.Claims.ElementAt(0).Value);
            int insert = await vAgua.insertaCedula(cedulaAgua);
            if (insert != -1)
            {
                return Ok(insert);
            }
            return BadRequest();
        }

        [Route("/agua/evaluacion/{id?}")]
        public async Task<IActionResult> Cuestionario(int id)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "actualizar");
            if (success == 1)
            {
                CedulaAgua cedulaAgua = await vAgua.CedulaById(id);
                if (cedulaAgua.Estatus.Equals("Enviado a DAS"))
                {
                    return Redirect("/error/cedSend");
                }
                cedulaAgua.inmuebles = await vInmuebles.inmuebleById(cedulaAgua.InmuebleId);
                cedulaAgua.RespuestasEncuesta = await vAgua.obtieneRespuestas(id);
                cedulaAgua.facturas = await vFacturas.getFacturas(id, cedulaAgua.ServicioId);
                cedulaAgua.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedulaAgua.facturas);
                return View(cedulaAgua);
            }
            return Redirect("/error/denied");
        }

        [HttpPost]
        [Route("/agua/evaluation")]
        public async Task<IActionResult> guardaRespuestas([FromBody] List<RespuestasEncuesta> respuestas)
        {
            int success = await vAgua.GuardaRespuestas(respuestas);
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
        [Route("/agua/sendCedula/{cedula?}")]
        public async Task<IActionResult> enviaCedula(int cedula)
        {
            int success = 0;
            success = await vAgua.enviaRespuestas(cedula);
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
        [Route("/agua/revision/{id}")]
        public async Task<IActionResult> RevisarCedula(int id)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "revisión");
            if (success == 1)
            {
                CedulaAgua cedAgua = null;
                cedAgua = await vAgua.CedulaById(id);
                cedAgua.facturas = await vFacturas.getFacturas(id, cedAgua.ServicioId);//
                cedAgua.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedAgua.facturas);
                cedAgua.inmuebles = await vInmuebles.inmuebleById(cedAgua.InmuebleId);
                cedAgua.usuarios = await vUsuarios.getUserById(cedAgua.UsuarioId);
                cedAgua.iEntregables = await eAgua.getEntregables(cedAgua.Id);
                cedAgua.incidencias = await iAgua.GetIncidencias(cedAgua.Id);
                cedAgua.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedAgua.RespuestasEncuesta = await vAgua.obtieneRespuestas(cedAgua.Id);
                cedAgua.historialCedulas = new List<HistorialCedulas>();
                cedAgua.historialCedulas = await vAgua.getHistorialAgua(cedAgua.Id);
                foreach (var user in cedAgua.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                return View(cedAgua);
            }
            return Redirect("/error/denied");
        }

        [HttpGet]
        [Route("/agua/seguimiento/{id}")]
        public async Task<IActionResult> SeguimientoCedula(int id)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "seguimiento");
            if (success == 1)
            {
                CedulaAgua cedFum = null;
                cedFum = await vAgua.CedulaById(id);
                cedFum.facturas = await vFacturas.getFacturas(id, cedFum.ServicioId);//
                cedFum.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedFum.facturas);
                cedFum.inmuebles = await vInmuebles.inmuebleById(cedFum.InmuebleId);
                cedFum.usuarios = await vUsuarios.getUserById(cedFum.UsuarioId);
                cedFum.iEntregables = await eAgua.getEntregables(cedFum.Id);
                cedFum.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedFum.RespuestasEncuesta = await vAgua.obtieneRespuestas(cedFum.Id);
                cedFum.historialCedulas = new List<HistorialCedulas>();
                cedFum.historialCedulas = await vAgua.getHistorialAgua(cedFum.Id);
                foreach (var user in cedFum.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                cedFum.historialEntregables = new List<HistorialEntregables>();
                cedFum.historialEntregables = await eAgua.getHistorialEntregables(cedFum.Id);
                foreach (var user in cedFum.historialEntregables)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                return View(cedFum);
            }
            return Redirect("/error/denied");
        }

        [HttpPost]
        [Route("/agua/aprovRechCed")]
        public async Task<IActionResult> aprovacionRechazoCedula([FromBody] CedulaAgua cedulaAgua)
        {
            int success = await vAgua.apruebaRechazaCedula(cedulaAgua);
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
        [Route("/agua/historialAgua")]
        public async Task<IActionResult> historialAgua([FromBody] HistorialCedulas historialCedulas)
        {
            int success = 0;
            success = await vAgua.capturaHistorial(historialCedulas);
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
            return "Agua_Para_Beber";
        }


    }
}
