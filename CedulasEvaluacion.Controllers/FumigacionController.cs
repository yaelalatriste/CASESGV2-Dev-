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
using CedulasEvaluacion.Entities.MCedula;

namespace CedulasEvaluacion.Controllers
{
    [Authorize]
    public class FumigacionController : Controller
    {
        private readonly IRepositorioEvaluacionServicios vCedula;
        private readonly IRepositorioFumigacion vFumigacion;
        private readonly IRepositorioIncidenciasFumigacion iFumigacion;
        private readonly IRepositorioEntregablesFumigacion eFumigacion;
        private readonly IRepositorioInmuebles vInmuebles;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IRepositorioPerfiles vRepositorioPerfiles;
        private readonly IRepositorioFacturas vFacturas;

        public FumigacionController(IRepositorioEvaluacionServicios viCedula, IRepositorioFumigacion iVFumigacion, IRepositorioInmuebles iVInmueble, IRepositorioUsuarios iVUsuario,
                                    IRepositorioIncidenciasFumigacion iiFumigacion, IRepositorioEntregablesFumigacion eeFumigacion,
                                    IRepositorioPerfiles iRepositorioPerfiles, IRepositorioFacturas iFacturas)
        {
            this.vCedula = viCedula ?? throw new ArgumentNullException(nameof(viCedula));
            this.vFumigacion = iVFumigacion ?? throw new ArgumentNullException(nameof(iVFumigacion));
            this.iFumigacion = iiFumigacion ?? throw new ArgumentNullException(nameof(iiFumigacion));
            this.eFumigacion = eeFumigacion ?? throw new ArgumentNullException(nameof(eeFumigacion));
            this.vFacturas = iFacturas ?? throw new ArgumentNullException(nameof(iFacturas));
            this.vInmuebles = iVInmueble ?? throw new ArgumentNullException(nameof(iVInmueble));
            this.vUsuarios = iVUsuario ?? throw new ArgumentNullException(nameof(iVUsuario));
            this.vRepositorioPerfiles = iRepositorioPerfiles ?? throw new ArgumentNullException(nameof(iRepositorioPerfiles));
        }

        //Metodo que regresa las cedulas aceptadas, guardadas o rechazadas 
        [Route("/fumigacion/index/{servicio?}")]
        public async Task<IActionResult> Index(int servicio)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "ver");
            if (success == 1)
            {
                List<VCedulas> resultado = new List<VCedulas>();
                resultado = await vCedula.GetCedulasEvaluacion(servicio, UserId());
                return View(resultado);
            }
            return Redirect("/error/denied");
        }

        //Metodo para abrir la vista y generar la nueva Cedula
        [Route("/fumigacion/nuevaCedula/{servicio}")]
        [HttpGet]
        public async Task<IActionResult> NuevaCedula(int servicio)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "crear");
            if (success == 1)
            {
                CedulaEvaluacion cedula = new CedulaEvaluacion();
                cedula.ServicioId = servicio;
                return View(cedula);
            }
            return Redirect("/error/denied");
        }

        //inserta la cedula
        [Route("/fumigacion/new")]
        public async Task<IActionResult> insertaCedula([FromBody] CedulaEvaluacion cedula)
        {
            cedula.UsuarioId = Convert.ToInt32(User.Claims.ElementAt(0).Value);
            int insert = await vCedula.insertaCedula(cedula);
            if (insert != -1)
            {
                return Ok(insert);
            }
            return BadRequest();
        }


        [Route("/fumigacion/validaPeriodo/{servicio}/{anio?}/{mes?}/{inmueble?}")]
        public async Task<IActionResult> validaPeriodo(int servicio, int anio, string mes, int inmueble)
        {
            int valida = await vCedula.VerificaCedula(servicio, anio, mes, inmueble);
            if (valida != -1)
            {
                return Ok(valida);
            }
            return BadRequest();
        }

        [Route("/fumigacion/evaluacion/{id?}")]
        public async Task<IActionResult> Cuestionario(int id)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "actualizar");
            if (success == 1)
            {
                CedulaEvaluacion cedula = await vCedula.CedulaById(id);
                if (cedula.Estatus.Equals("Enviado a DAS") && isEvaluate() == true)
                {
                    return Redirect("/error/cedSend");
                }
                cedula.inmuebles = await vInmuebles.inmuebleById(cedula.InmuebleId);
                cedula.RespuestasEncuesta = await vCedula.obtieneRespuestas(id);
                cedula.facturas = await vFacturas.getFacturas(id, cedula.ServicioId);
                cedula.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedula.facturas);
                return View(cedula);
            }
            return Redirect("/error/denied");
        }

        [HttpPost]
        [Route("/fumigacion/evaluation")]
        public async Task<IActionResult> guardaCuestionario([FromBody] List<RespuestasEncuesta> respuestas)
        {
            int success = await vCedula.GuardaRespuestas(respuestas);
            if (success != 0)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        //Va guardando las respuestas de la evaluacion
        [HttpPost]
        [Route("/fumigacion/sendCedula/{servicio?}/{id?}")]
        public async Task<IActionResult> enviaCedula(int servicio, int id)
        {
            int success = 0;
            success = await vCedula.enviaRespuestas(servicio, id);
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
                CedulaEvaluacion cedFum = null;
                cedFum = await vCedula.CedulaById(id);
                cedFum.facturas = await vFacturas.getFacturas(id, cedFum.ServicioId);//
                cedFum.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedFum.facturas);
                cedFum.inmuebles = await vInmuebles.inmuebleById(cedFum.InmuebleId);
                cedFum.usuarios = await vUsuarios.getUserById(cedFum.UsuarioId);
                cedFum.iEntregables = await eFumigacion.getEntregables(cedFum.Id);
                cedFum.incidencias = new Entities.MIncidencias.ModelsIncidencias();
                cedFum.incidencias.fumigacion = await iFumigacion.GetIncidencias(cedFum.Id);
                cedFum.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedFum.RespuestasEncuesta = await vCedula.obtieneRespuestas(cedFum.Id);
                cedFum.historialCedulas = new List<HistorialCedulas>();
                cedFum.historialCedulas = await vFumigacion.getHistorialFumigacion(cedFum.Id);
                foreach (var user in cedFum.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                if (cedFum.RespuestasEncuesta.Count == 9)
                {
                    return View(cedFum);
                }
                else
                {
                    return Redirect("/fumigacion/evaluacion/" + id);
                }
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
                CedulaEvaluacion cedFum = null;
                cedFum = await vCedula.CedulaById(id);
                cedFum.facturas = await vFacturas.getFacturas(id, cedFum.ServicioId);//
                cedFum.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedFum.facturas);
                cedFum.inmuebles = await vInmuebles.inmuebleById(cedFum.InmuebleId);
                cedFum.usuarios = await vUsuarios.getUserById(cedFum.UsuarioId);
                cedFum.iEntregables = await eFumigacion.getEntregables(cedFum.Id);
                //cedFum.RespuestasEncuesta = new List<RespuestasEncuesta>();
                //cedFum.RespuestasEncuesta = await vCedula.obtieneRespuestas(cedFum.Id);
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

        private bool isEvaluate()
        {
            if ((@User.Claims.ElementAt(2).Value).Contains("Evaluador"))
            {
                return true;
            }
            return false;
        }

    }
}
