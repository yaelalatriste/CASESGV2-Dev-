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
    public class ResiduosController : Controller
    {
        private readonly IRepositorioEvaluacionServicios vCedula;
        private readonly IRepositorioIncidenciasResiduos iResiduos;
        private readonly IRepositorioEntregablesCedula eResiduos;
        private readonly IRepositorioFacturas vFacturas;
        private readonly IRepositorioInmuebles vInmuebles;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IRepositorioPerfiles vPerfiles;

        public ResiduosController(IRepositorioEvaluacionServicios viCedula, IRepositorioIncidenciasResiduos ivResiduos, IRepositorioEntregablesCedula ieResiduos, IRepositorioFacturas iFacturas,
                                 IRepositorioUsuarios iUsuarios, IRepositorioPerfiles iPerfiles, IRepositorioInmuebles iVInmueble)
        {
            this.vCedula = viCedula ?? throw new ArgumentNullException(nameof(viCedula));
            this.iResiduos = ivResiduos ?? throw new ArgumentNullException(nameof(ivResiduos));
            this.eResiduos = ieResiduos ?? throw new ArgumentNullException(nameof(ieResiduos));
            this.vInmuebles = iVInmueble ?? throw new ArgumentNullException(nameof(iVInmueble));
            this.vFacturas = iFacturas ?? throw new ArgumentNullException(nameof(iFacturas));
            this.vUsuarios = iUsuarios ?? throw new ArgumentNullException(nameof(iUsuarios));
            this.vPerfiles = iPerfiles ?? throw new ArgumentNullException(nameof(iPerfiles));
        }

        //Metodo que regresa las cedulas aceptadas, guardadas o rechazadas 
        [Route("/residuos/index/{servicio?}")]
        public async Task<IActionResult> Index(int servicio, [FromQuery(Name = "Estatus")] string Estatus, [FromQuery(Name = "Mes")] string Mes,
            [FromQuery(Name = "Inmueble")] int Inmueble)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "ver");

            if (success == 1)
            {
                ModelsIndex models = new ModelsIndex();
                models.ServicioId = servicio;
                models.Estatus = Estatus;
                models.Mes = Mes;
                models.InmuebleId = Inmueble;
                models.cedulasEstatus = await vCedula.GetCedulasEvaluacionEstatus(servicio, UserId());
                if (models.Estatus != null && !models.Estatus.Equals(""))
                {
                    models.cedulasMes = await vCedula.GetCedulasEvaluacionMes(servicio, UserId(), Estatus);
                }
                if (models.Mes != null && !models.Mes.Equals("") && models.InmuebleId == 0)
                {
                    models.cedulas = await vCedula.GetCedulasEvaluacionServicios(servicio, UserId(), Estatus, Mes, Inmueble);
                }
                if (models.InmuebleId != 0 && (models.Mes == null || models.Mes.Equals("")))
                {
                    models.cedulas = await vCedula.GetCedulasEvaluacionServicios(servicio, UserId(), Estatus, Mes, Inmueble);
                }
                if (models.InmuebleId != 0 && (models.Mes != null && !models.Mes.Equals("")))
                {
                    models.cedulas = await vCedula.GetCedulasEvaluacionServicios(servicio, UserId(), Estatus, Mes, Inmueble);
                }

                return View(models);
            }

            return Redirect("/error/denied");
        }

        //Metodo para abrir la vista y generar la nueva Cedula
        [Route("/residuos/nuevaCedula/{servicio}")]
        [HttpGet]
        public async Task<IActionResult> NuevaCedula(int servicio)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "crear");
            if (success == 1)
            {
                CedulaEvaluacion cedula = new CedulaEvaluacion();
                cedula.ServicioId = servicio;
                return View(cedula);
            }
            return Redirect("/error/denied");
        }

        //inserta la cedula
        [Route("/residuos/new")]
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


        [Route("/residuos/validaPeriodo/{servicio}/{anio?}/{mes?}/{inmueble?}")]
        public async Task<IActionResult> validaPeriodo(int servicio, int anio, string mes, int inmueble)
        {
            int valida = await vCedula.VerificaCedula(servicio, anio, mes, inmueble);
            if (valida != -1)
            {
                return Ok(valida);
            }
            return BadRequest();
        }

        [Route("/residuos/evaluacion/{id?}")]
        public async Task<IActionResult> Cuestionario(int id)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "actualizar");
            if (success == 1)
            {
                CedulaEvaluacion cedula = await vCedula.CedulaById(id);
                if (cedula.Estatus.Equals("Enviado a DAS"))
                {
                    return Redirect("/error/cedSend");
                }
                cedula.URL = Request.QueryString.Value;
                cedula.inmuebles = await vInmuebles.inmuebleById(cedula.InmuebleId);
                cedula.RespuestasEncuesta = await vCedula.obtieneRespuestas(id);
                cedula.facturas = await vFacturas.getFacturas(id, cedula.ServicioId);
                cedula.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedula.facturas);
                return View(cedula);
            }
            return Redirect("/error/denied");
        }

        [HttpPost]
        [Route("/residuos/evaluation")]
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
        [Route("/residuos/sendCedula/{servicio?}/{id?}")]
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
        [Route("/residuos/revision/{id}")]
        public async Task<IActionResult> RevisarCedula(int id)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "revisión");
            if (success == 1)
            {
                CedulaEvaluacion residuos= null;
                residuos = await vCedula.CedulaById(id);
                residuos.URL = Request.QueryString.Value;
                residuos.facturas = await vFacturas.getFacturas(id, residuos.ServicioId);//
                residuos.TotalMontoFactura = vFacturas.obtieneTotalFacturas(residuos.facturas);
                residuos.inmuebles = await vInmuebles.inmuebleById(residuos.InmuebleId);
                residuos.usuarios = await vUsuarios.getUserById(residuos.UsuarioId);
                residuos.incidencias = new Entities.MIncidencias.ModelsIncidencias();
                residuos.incidencias.incidenciasManifiesto = await iResiduos.getIncidenciasTipo(residuos.Id,"ManifiestoEntrega");
                residuos.incidencias.incidenciasResiduos = await iResiduos.getIncidencias(residuos.Id);
                residuos.iEntregables = await eResiduos.getEntregables(residuos.Id);
                residuos.RespuestasEncuesta = new List<RespuestasEncuesta>();
                residuos.RespuestasEncuesta = await vCedula.obtieneRespuestas(residuos.Id);
                residuos.historialCedulas = new List<HistorialCedulas>();
                residuos.historialCedulas = await vCedula.getHistorial(residuos.Id);
                foreach (var user in residuos.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                return View(residuos);
            }
            return Redirect("/error/denied");
        }

        [HttpGet]
        [Route("/residuos/seguimiento/{id}")]
        public async Task<IActionResult> SeguimientoCedula(int id)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "seguimiento");
            if (success == 1)
            {
                CedulaEvaluacion residuos = null;
                residuos = await vCedula.CedulaById(id);
                residuos.URL = Request.QueryString.Value;
                residuos.facturas = await vFacturas.getFacturas(id, residuos.ServicioId);//
                residuos.TotalMontoFactura = vFacturas.obtieneTotalFacturas(residuos.facturas);
                residuos.inmuebles = await vInmuebles.inmuebleById(residuos.InmuebleId);
                residuos.usuarios = await vUsuarios.getUserById(residuos.UsuarioId);
                residuos.iEntregables = await eResiduos.getEntregables(residuos.Id);
                residuos.RespuestasEncuesta = new List<RespuestasEncuesta>();
                residuos.RespuestasEncuesta = await vCedula.obtieneRespuestas(residuos.Id);
                residuos.historialCedulas = new List<HistorialCedulas>();
                residuos.historialCedulas = await vCedula.getHistorial(residuos.Id);
                foreach (var user in residuos.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                residuos.historialEntregables = new List<HistorialEntregables>();
                residuos.historialEntregables = await eResiduos.getHistorialEntregables(residuos.Id,residuos.ServicioId);
                foreach (var user in residuos.historialEntregables)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                return View(residuos);
            }
            return Redirect("/error/denied");
        }

        [HttpPost]
        [Route("/residuos/aprovRechCed")]
        public async Task<IActionResult> aprovacionRechazoCedula([FromBody] CedulaEvaluacion residuos)
        {
            int success = await vCedula.apruebaRechazaCedula(residuos);
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
        [Route("/residuos/historialResiduos")]
        public async Task<IActionResult> historialResiduos([FromBody] HistorialCedulas historialCedulas)
        {
            int success = 0;
            success = await vCedula.capturaHistorial(historialCedulas);
            if (success != 0)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        private int UserId()
        {
            return Convert.ToInt32(User.Claims.ElementAt(0).Value);
        }

        private string modulo()
        {
            return "RPBI";
        }

    }
}
