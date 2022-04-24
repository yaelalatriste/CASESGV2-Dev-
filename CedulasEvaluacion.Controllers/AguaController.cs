﻿using CedulasEvaluacion.Entities.MAgua;
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
    public class AguaController : Controller
    {
        private readonly IRepositorioEvaluacionServicios vCedula;
        private readonly IRepositorioAgua vAgua;
        private readonly IRepositorioIncidenciasAgua iAgua;
        private readonly IRepositorioEntregablesAgua eAgua;
        private readonly IRepositorioInmuebles vInmuebles;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IRepositorioPerfiles vRepositorioPerfiles;
        private readonly IRepositorioFacturas vFacturas;
        private readonly IHostingEnvironment environment;

        public AguaController(IRepositorioEvaluacionServicios viCedula, IRepositorioAgua iVAgua, IRepositorioInmuebles iVInmueble, IRepositorioUsuarios iVUsuario,
                                    IRepositorioIncidenciasAgua iiAgua, IRepositorioEntregablesAgua eeAgua,
                                    IRepositorioPerfiles iRepositorioPerfiles, IRepositorioFacturas iFacturas, IHostingEnvironment environment)
        {
            this.vCedula = viCedula ?? throw new ArgumentNullException(nameof(viCedula));
            this.vAgua = iVAgua ?? throw new ArgumentNullException(nameof(iVAgua));
            this.iAgua = iiAgua ?? throw new ArgumentNullException(nameof(iiAgua));
            this.eAgua = eeAgua ?? throw new ArgumentNullException(nameof(eeAgua));
            this.vFacturas = iFacturas ?? throw new ArgumentNullException(nameof(iFacturas));
            this.vInmuebles = iVInmueble ?? throw new ArgumentNullException(nameof(iVInmueble));
            this.vUsuarios = iVUsuario ?? throw new ArgumentNullException(nameof(iVUsuario));
            this.vRepositorioPerfiles = iRepositorioPerfiles ?? throw new ArgumentNullException(nameof(iRepositorioPerfiles));
            this.environment = environment;
        }

        [Route("/agua/index/{servicio?}")]
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
        [Route("/agua/nuevaCedula/{servicio}")]
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
        [Route("/agua/new")]
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


        [Route("/agua/validaPeriodo/{servicio}/{anio?}/{mes?}/{inmueble?}")]
        public async Task<IActionResult> validaPeriodo(int servicio, int anio, string mes, int inmueble)
        {
            int valida = await vCedula.VerificaCedula(servicio, anio, mes, inmueble);
            if (valida != -1)
            {
                return Ok(valida);
            }
            return BadRequest();
        }

        [Route("/agua/evaluacion/{id?}")]
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
        [Route("/agua/evaluation")]
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
        [Route("/agua/sendCedula/{servicio?}/{id?}")]
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
        [Route("/agua/revision/{id}")]
        public async Task<IActionResult> RevisarCedula(int id)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "revisión");
            if (success == 1)
            {
                CedulaEvaluacion cedAgua = null;
                cedAgua = await vCedula.CedulaById(id);
                cedAgua.facturas = await vFacturas.getFacturas(id, cedAgua.ServicioId);//
                cedAgua.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedAgua.facturas);
                cedAgua.inmuebles = await vInmuebles.inmuebleById(cedAgua.InmuebleId);
                cedAgua.usuarios = await vUsuarios.getUserById(cedAgua.UsuarioId);
                cedAgua.iEntregables = await eAgua.getEntregables(cedAgua.Id);
                cedAgua.incidencias = new Entities.MIncidencias.ModelsIncidencias();
                cedAgua.incidencias.agua = await iAgua.GetIncidencias(cedAgua.Id);
                cedAgua.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedAgua.RespuestasEncuesta = await vCedula.obtieneRespuestas(cedAgua.Id);
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
                CedulaEvaluacion cedFum = null;
                cedFum = await vCedula.CedulaById(id);
                cedFum.facturas = await vFacturas.getFacturas(id, cedFum.ServicioId);//
                cedFum.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedFum.facturas);
                cedFum.inmuebles = await vInmuebles.inmuebleById(cedFum.InmuebleId);
                cedFum.usuarios = await vUsuarios.getUserById(cedFum.UsuarioId);
                cedFum.iEntregables = await eAgua.getEntregables(cedFum.Id);
                cedFum.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedFum.RespuestasEncuesta = await vCedula.obtieneRespuestas(cedFum.Id);
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
