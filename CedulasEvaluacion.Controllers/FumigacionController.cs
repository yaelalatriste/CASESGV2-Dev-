using CedulasEvaluacion.Entities.Vistas;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using CedulasEvaluacion.Entities.MFumigacion;

namespace CedulasEvaluacion.Controllers
{
    [Authorize]
    public class FumigacionController : Controller
    {
        private readonly IRepositorioFumigacion vFumigacion;
        private readonly IRepositorioIncidenciasFumigacion iFumigacion;
        private readonly IRepositorioEntregablesFumigacion eFumigacion;
        private readonly IRepositorioInmuebles vInmuebles;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IRepositorioPerfiles vRepositorioPerfiles;
        private readonly IRepositorioFacturas vFacturas;
        private readonly IHostingEnvironment environment;

        public FumigacionController(IRepositorioFumigacion iVFumigacion, IRepositorioInmuebles iVInmueble, IRepositorioUsuarios iVUsuario,
                                    IRepositorioIncidenciasFumigacion iiFumigacion, IRepositorioEntregablesFumigacion eeFumigacion,
                                    IRepositorioPerfiles iRepositorioPerfiles, IRepositorioFacturas iFacturas, IHostingEnvironment environment)
        {
            this.vFumigacion = iVFumigacion ?? throw new ArgumentNullException(nameof(iVFumigacion));
            this.iFumigacion = iiFumigacion ?? throw new ArgumentNullException(nameof(iiFumigacion));
            this.eFumigacion = eeFumigacion ?? throw new ArgumentNullException(nameof(eeFumigacion));
            this.vFacturas = iFacturas ?? throw new ArgumentNullException(nameof(iFacturas));
            this.vInmuebles = iVInmueble ?? throw new ArgumentNullException(nameof(iVInmueble));
            this.vUsuarios = iVUsuario ?? throw new ArgumentNullException(nameof(iVUsuario));
            this.vRepositorioPerfiles = iRepositorioPerfiles ?? throw new ArgumentNullException(nameof(iRepositorioPerfiles));
            this.environment = environment;
        }

        //Metodo que regresa las cedulas aceptadas, guardadas o rechazadas 
        [Route("/fumigacion/index")]
        public async Task<IActionResult> Index()
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "ver");
            if (success == 1)
            {
                List<VCedulas> resultado = new List<VCedulas>();
                resultado = await vFumigacion.GetCedulasFumigacion(UserId());
                return View(resultado);
            }
            return Redirect("/error/denied");
        }

        [Route("/fumigacion/nuevaCedula")]
        public async Task<IActionResult> NuevaCedula(CedulaFumigacion cedulaFumigacion)
        {
            cedulaFumigacion = new CedulaFumigacion();
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "crear");
            if (success == 1)
            {
                return View(cedulaFumigacion);
            }
            return Redirect("/error/denied");
        }

       [Route("/fumigacion/validaPeriodo/{anio?}/{mes?}/{inmueble?}")]
        public async Task<IActionResult> validaPeriodo(int anio, string mes, int inmueble)
        {
            int valida = await vFumigacion.VerificaCedula(anio, mes, inmueble);
            if (valida != -1)
            {
                return Ok(valida);
            }
            return BadRequest();
        }

        [Route("/fumigacion/new")]
        public async Task<IActionResult> insertaCedula([FromBody] CedulaFumigacion cedulaFumigacion)
        {
            cedulaFumigacion.UsuarioId = Convert.ToInt32(User.Claims.ElementAt(0).Value);
            int insert = await vFumigacion.insertaCedula(cedulaFumigacion);
            if (insert != -1)
            {
                return Ok(insert);
            }
            return BadRequest();
        }

        [Route("/fumigacion/evaluacion/{id?}")]
        public async Task<IActionResult> Cuestionario(int id)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "actualizar");
            if (success == 1)
            {
                CedulaFumigacion cedulaFumigacion = await vFumigacion.CedulaById(id);
                if (cedulaFumigacion.Estatus.Equals("Enviado a DAS"))
                {
                    return Redirect("/error/cedSend");
                }
                cedulaFumigacion.inmuebles = await vInmuebles.inmuebleById(cedulaFumigacion.InmuebleId);
                cedulaFumigacion.RespuestasEncuesta = await vFumigacion.obtieneRespuestas(id);
                cedulaFumigacion.facturas = await vFacturas.getFacturas(id, cedulaFumigacion.ServicioId);
                cedulaFumigacion.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedulaFumigacion.facturas);
                return View(cedulaFumigacion);
            }
            return Redirect("/error/denied");
        }

        [HttpPost]
        [Route("/fumigacion/evaluation")]
        public async Task<IActionResult> guardaRespuestas([FromBody] List<RespuestasEncuesta> respuestas)
        {
            int success = await vFumigacion.GuardaRespuestas(respuestas);
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
        [Route("/fumigacion/sendCedula/{cedula?}")]
        public async Task<IActionResult> enviaCedula(int cedula)
        {
            int success = 0;
            success = await vFumigacion.enviaRespuestas(cedula);
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
        [Route("/fumigacion/revision/{id}")]
        public async Task<IActionResult> RevisarCedula(int id)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "revisión");
            if (success == 1)
            {
                CedulaFumigacion cedFum = null;
                cedFum = await vFumigacion.CedulaById(id);
                cedFum.facturas = await vFacturas.getFacturas(id, cedFum.ServicioId);//
                cedFum.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedFum.facturas);
                cedFum.inmuebles = await vInmuebles.inmuebleById(cedFum.InmuebleId);
                cedFum.usuarios = await vUsuarios.getUserById(cedFum.UsuarioId);
                cedFum.iEntregables = await eFumigacion.getEntregables(cedFum.Id);
                cedFum.incidencias = await iFumigacion.GetIncidencias(cedFum.Id);
                cedFum.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedFum.RespuestasEncuesta = await vFumigacion.obtieneRespuestas(cedFum.Id);
                cedFum.historialCedulas = new List<HistorialCedulas>();
                cedFum.historialCedulas = await vFumigacion.getHistorialFumigacion(cedFum.Id);
                foreach (var user in cedFum.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                return View(cedFum);
            }
            return Redirect("/error/denied");
        }

        [HttpGet]
        [Route("/fumigacion/seguimiento/{id}")]
        public async Task<IActionResult> SeguimientoCedula(int id)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "seguimiento");
            if (success == 1)
            {
                CedulaFumigacion cedFum = null;
                cedFum = await vFumigacion.CedulaById(id);
                cedFum.facturas = await vFacturas.getFacturas(id, cedFum.ServicioId);//
                cedFum.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedFum.facturas);
                cedFum.inmuebles = await vInmuebles.inmuebleById(cedFum.InmuebleId);
                cedFum.usuarios = await vUsuarios.getUserById(cedFum.UsuarioId);
                cedFum.iEntregables = await eFumigacion.getEntregables(cedFum.Id);
                cedFum.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedFum.RespuestasEncuesta = await vFumigacion.obtieneRespuestas(cedFum.Id);
                cedFum.historialCedulas = new List<HistorialCedulas>();
                cedFum.historialCedulas = await vFumigacion.getHistorialFumigacion(cedFum.Id);
                foreach (var user in cedFum.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                cedFum.historialEntregables = new List<HistorialEntregables>();
                cedFum.historialEntregables = await eFumigacion.getHistorialEntregables(cedFum.Id);
                foreach (var user in cedFum.historialEntregables)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                return View(cedFum);
            }
            return Redirect("/error/denied");
        }

        [HttpPost]
        [Route("/fumigacion/aprovRechCed")]
        public async Task<IActionResult> aprovacionRechazoCedula([FromBody] CedulaFumigacion cedulaFumigacion)
        {
            int success = await vFumigacion.apruebaRechazaCedula(cedulaFumigacion);
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
        [Route("/fumigacion/historialFumigacion")]
        public async Task<IActionResult> historialFumigacion([FromBody] HistorialCedulas historialCedulas)
        {
            int success = 0;
            success = await vFumigacion.capturaHistorial(historialCedulas);
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
            return "Fumigación";
        }

    }
}
