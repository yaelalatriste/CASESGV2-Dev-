using CedulasEvaluacion.Entities.MAnalisis;
using CedulasEvaluacion.Entities.MCedula;
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
    public class AnalisisController : Controller
    {
        private readonly IRepositorioEvaluacionServicios vCedula;
        private readonly IRepositorioAnalisis vAnalisis;
        private readonly IRepositorioIncidenciasAnalisis iAnalisis;
        private readonly IRepositorioEntregablesCedula eAnalisis;
        private readonly IRepositorioInmuebles vInmuebles;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IRepositorioPerfiles vRepositorioPerfiles;
        private readonly IRepositorioFacturas vFacturas;
        private readonly IHostingEnvironment environment;

        public AnalisisController(IRepositorioEvaluacionServicios viCedula, IRepositorioAnalisis iVAnalisis, IRepositorioInmuebles iVInmueble, IRepositorioUsuarios iVUsuario,
                                    IRepositorioIncidenciasAnalisis iiAnalisis, IRepositorioEntregablesCedula eeAnalisis,
                                    IRepositorioPerfiles iRepositorioPerfiles, IRepositorioFacturas iFacturas, IHostingEnvironment environment)
        {
            this.vCedula = viCedula ?? throw new ArgumentNullException(nameof(viCedula));
            this.vAnalisis = iVAnalisis ?? throw new ArgumentNullException(nameof(iVAnalisis));
            this.iAnalisis = iiAnalisis ?? throw new ArgumentNullException(nameof(iiAnalisis));
            this.eAnalisis = eeAnalisis ?? throw new ArgumentNullException(nameof(eeAnalisis));
            this.vFacturas = iFacturas ?? throw new ArgumentNullException(nameof(iFacturas));
            this.vInmuebles = iVInmueble ?? throw new ArgumentNullException(nameof(iVInmueble));
            this.vUsuarios = iVUsuario ?? throw new ArgumentNullException(nameof(iVUsuario));
            this.vRepositorioPerfiles = iRepositorioPerfiles ?? throw new ArgumentNullException(nameof(iRepositorioPerfiles));
            this.environment = environment;
        }

        [Route("/analisis/index/{servicio?}")]
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
        [Route("/analisis/nuevaCedula/{servicio}")]
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
        [Route("/analisis/new")]
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


        [Route("/analisis/validaPeriodo/{servicio}/{anio?}/{mes?}/{inmueble?}")]
        public async Task<IActionResult> validaPeriodo(int servicio, int anio, string mes, int inmueble)
        {
            int valida = await vCedula.VerificaCedula(servicio, anio, mes, inmueble);
            if (valida != -1)
            {
                return Ok(valida);
            }
            return BadRequest();
        }

        [Route("/analisis/evaluacion/{id?}")]
        public async Task<IActionResult> Cuestionario(int id)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "actualizar");
            if (success == 1)
            {
                CedulaEvaluacion cedula = await vCedula.CedulaById(id);
                if (cedula.Estatus.Equals("Enviado a DAS"))
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
        [Route("/analisis/evaluation")]
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
        [Route("/analisis/sendCedula/{servicio?}/{id?}")]
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
        [Route("/analisis/revision/{id}")]
        public async Task<IActionResult> RevisarCedula(int id)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "revisión");
            if (success == 1)
            {
                CedulaEvaluacion cedAna = null;
                cedAna = await vCedula.CedulaById(id);
                cedAna.facturas = await vFacturas.getFacturas(id, cedAna.ServicioId);//
                cedAna.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedAna.facturas);
                cedAna.inmuebles = await vInmuebles.inmuebleById(cedAna.InmuebleId);
                cedAna.usuarios = await vUsuarios.getUserById(cedAna.UsuarioId);
                cedAna.iEntregables = await eAnalisis.getEntregables(cedAna.Id);
                cedAna.incidencias = new Entities.MIncidencias.ModelsIncidencias();
                cedAna.incidencias.analisis = await iAnalisis.GetIncidencias(cedAna.Id);
                cedAna.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedAna.RespuestasEncuesta = await vCedula.obtieneRespuestas(cedAna.Id);
                cedAna.historialCedulas = new List<HistorialCedulas>();
                cedAna.historialCedulas = await vAnalisis.getHistorialAnalisis(cedAna.Id);
                foreach (var user in cedAna.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                return View(cedAna);
            }
            return Redirect("/error/denied");
        }

        [HttpGet]
        [Route("/analisis/seguimiento/{id}")]
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
                cedFum.iEntregables = await eAnalisis.getEntregables(cedFum.Id);
                cedFum.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedFum.RespuestasEncuesta = await vCedula.obtieneRespuestas(cedFum.Id);
                cedFum.historialCedulas = new List<HistorialCedulas>();
                cedFum.historialCedulas = await vAnalisis.getHistorialAnalisis(cedFum.Id);
                foreach (var user in cedFum.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                cedFum.historialEntregables = new List<HistorialEntregables>();
                cedFum.historialEntregables = await eAnalisis.getHistorialEntregables(cedFum.Id, cedFum.ServicioId);
                foreach (var user in cedFum.historialEntregables)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                return View(cedFum);
            }
            return Redirect("/error/denied");
        }

        [HttpPost]
        [Route("/analisis/aprovRechCed")]
        public async Task<IActionResult> aprovacionRechazoCedula([FromBody] CedulaAnalisis cedulaAnalisis)
        {
            int success = await vAnalisis.apruebaRechazaCedula(cedulaAnalisis);
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
        [Route("/analisis/historialAnalisis")]
        public async Task<IActionResult> historialAnalisis([FromBody] HistorialCedulas historialCedulas)
        {
            int success = 0;
            success = await vAnalisis.capturaHistorial(historialCedulas);
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
            return "Análisis_Microbiológicos";
        }
    }
}
