using CASESGCedulasEvaluacion.Entities.Vistas;
using CedulasEvaluacion.Entities.MCedula;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.Vistas;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    [Authorize]
    public class LimpiezaController : Controller
    {
        private readonly IRepositorioEvaluacionServicios vCedula;
        private readonly IRepositorioLimpieza vlimpieza;
        private readonly IRepositorioFacturas vFacturas;
        private readonly IRepositorioIncidencias vIncidencias;
        private readonly IRepositorioEntregables vEntregables;
        private readonly IRepositorioInmuebles vInmuebles;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IRepositorioPerfiles vRepositorioPerfiles;

        public LimpiezaController(IRepositorioEvaluacionServicios viCedula, IRepositorioLimpieza iVLimpieza, IRepositorioInmuebles iVInmueble, IRepositorioIncidencias iIncidencias,
                                  IRepositorioUsuarios iVUsuario, IRepositorioEntregables iEntregables, IRepositorioPerfiles iRepositorioPerfiles, IRepositorioFacturas iFacturas)
        {
            this.vCedula = viCedula ?? throw new ArgumentNullException(nameof(viCedula));
            this.vlimpieza = iVLimpieza ?? throw new ArgumentNullException(nameof(iVLimpieza));
            this.vFacturas = iFacturas ?? throw new ArgumentNullException(nameof(iFacturas));
            this.vIncidencias = iIncidencias ?? throw new ArgumentNullException(nameof(iIncidencias));
            this.vEntregables = iEntregables ?? throw new ArgumentNullException(nameof(iEntregables));
            this.vInmuebles = iVInmueble ?? throw new ArgumentNullException(nameof(iVInmueble));
            this.vUsuarios = iVUsuario ?? throw new ArgumentNullException(nameof(iVUsuario));
            this.vRepositorioPerfiles = iRepositorioPerfiles ?? throw new ArgumentNullException(nameof(iRepositorioPerfiles));
        }

        //Metodo que regresa las cedulas aceptadas, guardadas o rechazadas 
        [Route("/limpieza/index/{servicio?}")]
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
        [Route("/limpieza/nuevaCedula/{servicio}")]
        [HttpGet]
        public async Task<IActionResult> NuevaCedula(int servicio)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "crear");
            if (success == 1)
            {
                TempData["Title"] = "\"Limpieza en Áreas Comunes y Oficinas\"";
                CedulaEvaluacion cedula = new CedulaEvaluacion();
                cedula.ServicioId = servicio;
                return View(cedula);
            }
            return Redirect("/error/denied");
        }

        //inserta la cedula
        [Route("/limpieza/new")]
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


        [Route("/limpieza/validaPeriodo/{servicio}/{anio?}/{mes?}/{inmueble?}")]
        public async Task<IActionResult> validaPeriodo(int servicio, int anio, string mes, int inmueble)
        {
            int valida = await vCedula.VerificaCedula(servicio, anio, mes, inmueble);
            if (valida != -1)
            {
                return Ok(valida);
            }
            return BadRequest();
        }

        [Route("/limpieza/evaluacion/{id?}")]
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

        //Va guardando las respuestas de la evaluacion
        [HttpPost]
        [Route("/limpieza/evaluation")]
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

        [HttpGet]
        [Route("/limpieza/revision/{id}")]
        public async Task<IActionResult> RevisarCedula(int id)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "revisión");
            if (success == 1)
            {
                CedulaEvaluacion cedLim = null;
                cedLim = await vCedula.CedulaById(id);
                cedLim.facturas = await vFacturas.getFacturas(id, cedLim.ServicioId);//
                cedLim.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedLim.facturas);
                cedLim.inmuebles = await vInmuebles.inmuebleById(cedLim.InmuebleId);
                cedLim.usuarios = await vUsuarios.getUserById(cedLim.UsuarioId);
                //Incidencias
                List<VIncidenciasLimpieza> incidencias = await vIncidencias.getIncidencias(cedLim.Id);
                List<VIncidenciasLimpieza> equipo = await vIncidencias.getIncidenciasEquipo(cedLim.Id);
                cedLim.incidencias = new Entities.MIncidencias.ModelsIncidencias();
                cedLim.incidencias.iLimpieza = incidencias;
                cedLim.incidencias.iEquipo = new List<VIncidenciasLimpieza>();
                cedLim.incidencias.iEquipo = equipo;
                cedLim.iEntregables = await vEntregables.getEntregables(cedLim.Id);
                //cedLim.TotalBajoDemanda = obtieneTotalBajoDemanda(cedLim.eAdicionales);
                cedLim.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedLim.RespuestasEncuesta = await vCedula.obtieneRespuestas(cedLim.Id);
                cedLim.historialCedulas = new List<HistorialCedulas>();
                cedLim.historialCedulas = await vlimpieza.getHistorial(cedLim.Id);
                foreach (var user in cedLim.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                TempData["Title"] = "Cédula de Limpieza con número de folio: ";
                if (cedLim.RespuestasEncuesta.Count == 8)
                {
                    return View(cedLim);
                }
                else
                {
                    return Redirect("/limpieza/evaluacion/" + id);
                }
            }
            return Redirect("/error/denied");
        }

        [HttpGet]
        [Route("/limpieza/seguimiento/{id}")]
        public async Task<IActionResult> SeguimientoCedula(int id)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "seguimiento");
            if (success == 1)
            {
                CedulaEvaluacion cedLim = null;
                cedLim = await vCedula.CedulaById(id);
                cedLim.facturas = await vFacturas.getFacturas(id, cedLim.ServicioId);//
                cedLim.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedLim.facturas);
                cedLim.inmuebles = await vInmuebles.inmuebleById(cedLim.InmuebleId);
                cedLim.usuarios = await vUsuarios.getUserById(cedLim.UsuarioId);
                cedLim.iEntregables = await vEntregables.getEntregables(cedLim.Id);
                cedLim.historialEntregables = new List<HistorialEntregables>();
                cedLim.historialEntregables = await vEntregables.getHistorialEntregables(cedLim.Id);
                cedLim.facturas = await vFacturas.getFacturas(id, cedLim.ServicioId);//
                cedLim.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedLim.facturas);
                cedLim.inmuebles = await vInmuebles.inmuebleById(cedLim.InmuebleId);
                cedLim.usuarios = await vUsuarios.getUserById(cedLim.UsuarioId);                
                cedLim.iEntregables = await vEntregables.getEntregables(cedLim.Id);
                cedLim.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedLim.RespuestasEncuesta = await vCedula.obtieneRespuestas(cedLim.Id);
                cedLim.historialEntregables = new List<HistorialEntregables>();
                cedLim.historialEntregables = await vEntregables.getHistorialEntregables(cedLim.Id);
                foreach (var user in cedLim.historialEntregables)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                cedLim.historialCedulas = new List<HistorialCedulas>();
                cedLim.historialCedulas = await vlimpieza.getHistorial(cedLim.Id);
                foreach (var user in cedLim.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }

                return View(cedLim);
            }
            return Redirect("/error/denied");
        }

        //Va guardando las respuestas de la evaluacion
        [HttpPost]
        [Route("/limpieza/sendCedula/{servicio?}/{id?}")]
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

        //Va guardando las respuestas de la evaluacion
        [HttpPost]
        [Route("/limpieza/aprovRechCed")]
        public async Task<IActionResult> aprovacionRechazoCedula([FromBody] CedulaEvaluacion cedulae)
        {
            CedulaLimpieza cedula = new CedulaLimpieza();
            cedula.Id = cedulae.Id;
            cedula.Estatus = cedulae.Estatus;
            cedula.ServicioId = cedulae.ServicioId;
            int success = 0;
            success = await vlimpieza.apruebaRechazaCedula(cedula);
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
        [Route("/limpieza/historialLimpieza")]
        public async Task<IActionResult> historialLimpieza([FromBody] HistorialCedulas historialCedulas)
        {
            int success = 0;
            success = await vlimpieza.capturaHistorial(historialCedulas);
            if (success != 0)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        public decimal obtieneTotalBajoDemanda(List<RespuestasAdicionales> respuestasAdicionales)
        {
            decimal total = 0;
            foreach (var bd in respuestasAdicionales)
            {
                if (bd.Pregunta.Equals("EntregableBD"))
                {
                    total += bd.MontoPenalizacion;
                }
            }
            return total;
        }

        [Route("/limpieza/eliminar/{id?}")]
        public async Task<IActionResult> EliminaCedula(int id)
        {
            int excel = await vCedula.EliminaCedula(id);
            if (excel != -1)
            {
                return Ok(excel);
            }
            return BadRequest();
        }

        /*Datos del Modulo*/
        private int UserId()
        {
            return Convert.ToInt32(User.Claims.ElementAt(0).Value);
        }
        private string modulo()
        {
            return "Limpieza";
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
