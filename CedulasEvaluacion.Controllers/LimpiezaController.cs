using CASESGCedulasEvaluacion.Entities.Vistas;
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
        private readonly IRepositorioLimpieza vlimpieza;
        private readonly IRepositorioFacturas vFacturas;
        private readonly IRepositorioIncidencias vIncidencias;
        private readonly IRepositorioEntregables vEntregables;
        private readonly IRepositorioInmuebles vInmuebles;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IRepositorioPerfiles vRepositorioPerfiles;
        private readonly IHostingEnvironment environment;

        public LimpiezaController(IRepositorioLimpieza iVLimpieza, IRepositorioInmuebles iVInmueble, IRepositorioIncidencias iIncidencias,
                                  IRepositorioUsuarios iVUsuario, IRepositorioEntregables iEntregables, IHostingEnvironment environment, 
                                  IRepositorioPerfiles iRepositorioPerfiles, IRepositorioFacturas iFacturas)
        {
            this.vlimpieza = iVLimpieza ?? throw new ArgumentNullException(nameof(iVLimpieza));
            this.vFacturas = iFacturas ?? throw new ArgumentNullException(nameof(iFacturas));
            this.vIncidencias = iIncidencias ?? throw new ArgumentNullException(nameof(iIncidencias));
            this.vEntregables = iEntregables ?? throw new ArgumentNullException(nameof(iEntregables));
            this.vInmuebles = iVInmueble ?? throw new ArgumentNullException(nameof(iVInmueble));
            this.vUsuarios = iVUsuario ?? throw new ArgumentNullException(nameof(iVUsuario));
            this.vRepositorioPerfiles = iRepositorioPerfiles ?? throw new ArgumentNullException(nameof(iRepositorioPerfiles));
            this.environment = environment;
        }

        //Metodo que regresa las cedulas aceptadas, guardadas o rechazadas 
        [Route("/limpieza/index")]
        public async Task<IActionResult> Index()
        {
            TempData["Title"] = "\"Cédulas - Servicio Limpieza\"";
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "ver");
            if (success == 1)
            {
                List<VCedulas> resultado = new List<VCedulas>();
                resultado = await vlimpieza.GetCedulasLimpieza(UserId());
                return View(resultado);
            }
            return Redirect("/error/denied");
        }

        //Metodo para abrir la vista y generar la nueva Cedula
        [Route("/limpieza/nuevaCedula")]
        [HttpGet]
        public async Task<IActionResult> NuevaCedula(CedulaLimpieza cedulaLimpieza)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "crear");
            if (success == 1)
            {
                TempData["Title"] = "\"Limpieza en Áreas Comunes y Oficinas\"";
                return View(cedulaLimpieza);
            }
            return Redirect("/error/denied");
        }

        //inserta la cedula
        [Route("/limpieza/new")]
        public async Task<IActionResult> insertaCedula([FromBody]CedulaLimpieza cedulaLimpieza)
        {
            cedulaLimpieza.UsuarioId = Convert.ToInt32(User.Claims.ElementAt(0).Value);
            int insert = await vlimpieza.NuevaCedula(cedulaLimpieza);
            if (insert != -1)
            {
                return Ok(insert);
            }
            return BadRequest();
        }


        [Route("/limpieza/validaPeriodo/{anio?}/{mes?}/{inmueble?}")]
        public async Task<IActionResult> validaPeriodo(int anio, string mes, int inmueble)
        {
            int valida = await vlimpieza.VerificaCedula(anio, mes, inmueble);
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
                CedulaLimpieza cedulaLimpieza = await vlimpieza.CedulaById(id);
                if (cedulaLimpieza.Estatus.Equals("Enviado a DAS"))
                {
                    return Redirect("/error/cedSend");
                }
                cedulaLimpieza.inmuebles = await vInmuebles.inmuebleById(cedulaLimpieza.InmuebleId);
                cedulaLimpieza.RespuestasEncuesta = await vlimpieza.obtieneRespuestas(id);
                cedulaLimpieza.facturas = await vFacturas.getFacturas(id,cedulaLimpieza.ServicioId);
                cedulaLimpieza.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedulaLimpieza.facturas);
                return View(cedulaLimpieza);
            } 
            return Redirect("/error/denied");
        }
        
        //Pendiente de Revisar
        [Route("/limpieza/cedula/{id?}")]
        public async Task<IActionResult> CedulabyId(int id)
        {
            CedulaLimpieza cedulaLimpieza = await vlimpieza.CedulaById(id);
            cedulaLimpieza.inmuebles = await vInmuebles.inmuebleById(cedulaLimpieza.InmuebleId);
            cedulaLimpieza.RespuestasEncuesta = await vlimpieza.obtieneRespuestas(id);
            if(cedulaLimpieza != null)
            {
                return Ok(cedulaLimpieza);
            }
            return BadRequest();
        }

        //Va guardando las respuestas de la evaluacion
        [HttpPost]
        [Route("cedLimpieza/evaluation")]
        public async Task<IActionResult> guardaCuestionario([FromBody] List<RespuestasEncuesta> respuestas) 
        {
            int success = await vlimpieza.GuardaRespuestas(respuestas);
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
                CedulaLimpieza cedLim = null;
                cedLim = await vlimpieza.CedulaById(id);
                cedLim.facturas = await vFacturas.getFacturas(id, cedLim.ServicioId);//
                cedLim.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedLim.facturas);
                cedLim.inmuebles = await vInmuebles.inmuebleById(cedLim.InmuebleId);
                cedLim.capacitacion = await vEntregables.getCapacitacionLimpieza(cedLim.Id);
                cedLim.usuarios = await vUsuarios.getUserById(cedLim.UsuarioId);
                cedLim.incidencias = await vIncidencias.getIncidencias(cedLim.Id);
                cedLim.iEquipos = await vIncidencias.getIncidenciasEquipo(cedLim.Id);
                cedLim.iEntregables = await vEntregables.getEntregables(cedLim.Id);
                cedLim.eAdicionales = await vEntregables.getEntregablesBDLimpieza(cedLim.Id);
                cedLim.TotalBajoDemanda = obtieneTotalBajoDemanda(cedLim.eAdicionales);
                cedLim.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedLim.RespuestasEncuesta = await vlimpieza.obtieneRespuestas(cedLim.Id);
                cedLim.historialCedulas = new List<HistorialCedulas>();
                cedLim.historialCedulas = await vlimpieza.getHistorial(cedLim.Id);
                foreach (var user in cedLim.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                TempData["Title"] = "Cédula de Limpieza con número de folio: ";
                return View(cedLim);
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
                CedulaLimpieza cedLim = null;
                cedLim = await vlimpieza.CedulaById(id);
                cedLim.facturas = await vFacturas.getFacturas(id, cedLim.ServicioId);//
                cedLim.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedLim.facturas);
                cedLim.inmuebles = await vInmuebles.inmuebleById(cedLim.InmuebleId);
                cedLim.usuarios = await vUsuarios.getUserById(cedLim.UsuarioId);
                cedLim.iEntregables = await vEntregables.getEntregables(cedLim.Id);
                cedLim.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedLim.RespuestasEncuesta = await vlimpieza.obtieneRespuestas(cedLim.Id);
                cedLim.historialEntregables = new List<HistorialEntregables>();
                cedLim.historialEntregables = await vEntregables.getHistorialEntregables(cedLim.Id); cedLim = await vlimpieza.CedulaById(id);
                cedLim.facturas = await vFacturas.getFacturas(id, cedLim.ServicioId);//
                cedLim.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedLim.facturas);
                cedLim.inmuebles = await vInmuebles.inmuebleById(cedLim.InmuebleId);
                cedLim.capacitacion = await vEntregables.getCapacitacionLimpieza(cedLim.Id);
                cedLim.usuarios = await vUsuarios.getUserById(cedLim.UsuarioId);
                cedLim.incidencias = await vIncidencias.getIncidencias(cedLim.Id);
                cedLim.iEquipos = await vIncidencias.getIncidenciasEquipo(cedLim.Id);
                cedLim.iEntregables = await vEntregables.getEntregables(cedLim.Id);
                cedLim.eAdicionales = await vEntregables.getEntregablesBDLimpieza(cedLim.Id);
                cedLim.TotalBajoDemanda = obtieneTotalBajoDemanda(cedLim.eAdicionales);
                cedLim.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedLim.RespuestasEncuesta = await vlimpieza.obtieneRespuestas(cedLim.Id);
                cedLim.historialEntregables= new List<HistorialEntregables>();
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
        [Route("/cedLimpieza/sendCedula")]
        public async Task<IActionResult> enviaCedula([FromBody] List<RespuestasEncuesta> respuestas)
        {
            int success = 0;
            success = await vlimpieza.enviaRespuestas(respuestas);
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
        [Route("cedLimpieza/aprovRechCed")]
        public async Task<IActionResult> aprovacionRechazoCedula([FromBody] CedulaLimpieza cedulaLimpieza)
        {
            int success = 0;
            success = await vlimpieza.apruebaRechazaCedula(cedulaLimpieza);
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
        [Route("cedLimpieza/historialLimpieza")]
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
            int excel = await vlimpieza.EliminaCedula(id);
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

    }
}
