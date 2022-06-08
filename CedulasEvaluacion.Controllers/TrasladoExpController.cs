using CedulasEvaluacion.Entities.MCedula;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.TrasladoExp;
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
    public class TrasladoExpController : Controller
    {
        private readonly IRepositorioEvaluacionServicios vCedula;
        private readonly IRepositorioTrasladoExp vTraslado;
        private readonly IRepositorioEntregablesCedula vEntregables;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IRepositorioInmuebles vInmuebles;
        private readonly IRepositorioIncidenciasTraslado iTraslado;
        private readonly IRepositorioPerfiles vPerfiles;
        private readonly IHostingEnvironment environment;
        private readonly IRepositorioFacturas vFacturas;

        public TrasladoExpController(IRepositorioEvaluacionServicios viCedula, IRepositorioTrasladoExp iTraslado, 
                                     IRepositorioFacturas iFacturas, IRepositorioEntregablesCedula iVEntregables, 
                                     IRepositorioUsuarios iVUsuario, IRepositorioInmuebles iVInmueble,
                                     IRepositorioIncidenciasTraslado ivTraslado, IRepositorioPerfiles iPerfiles, IHostingEnvironment environment)
        {
            this.vCedula = viCedula ?? throw new ArgumentNullException(nameof(viCedula));
            this.vTraslado= iTraslado ?? throw new ArgumentNullException(nameof(iTraslado));
            this.vFacturas = iFacturas ?? throw new ArgumentNullException(nameof(iFacturas));
            this.vInmuebles = iVInmueble ?? throw new ArgumentNullException(nameof(iVInmueble));
            this.vUsuarios = iVUsuario ?? throw new ArgumentNullException(nameof(iVUsuario));
            this.vPerfiles = iPerfiles ?? throw new ArgumentNullException(nameof(iPerfiles));
            this.iTraslado = ivTraslado ?? throw new ArgumentNullException(nameof(ivTraslado));
            this.vEntregables = iVEntregables ?? throw new ArgumentNullException(nameof(iVEntregables));
        }

        [Route("/trasladoExp/index/{servicio?}")]
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
        [Route("/trasladoExp/nuevaCedula/{servicio}")]
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
        [Route("/trasladoExp/new")]
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


        [Route("/trasladoExp/validaPeriodo/{servicio}/{anio?}/{mes?}/{inmueble?}")]
        public async Task<IActionResult> validaPeriodo(int servicio, int anio, string mes, int inmueble)
        {
            int valida = await vCedula.VerificaCedula(servicio, anio, mes, inmueble);
            if (valida != -1)
            {
                return Ok(valida);
            }
            return BadRequest();
        }

        [Route("/trasladoExp/evaluacion/{id?}")]
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
        [Route("/trasladoExp/evaluation")]
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
        [Route("/trasladoExp/sendCedula/{servicio?}/{id?}")]
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
        [Route("/trasladoExp/revision/{id}")]
        public async Task<IActionResult> RevisarCedula(int id)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "revisión");
            if (success == 1)
            {
                CedulaEvaluacion cedula = null;
                cedula = await vCedula.CedulaById(id);
                cedula.URL = Request.QueryString.Value;
                cedula.inmuebles = await vInmuebles.inmuebleById(cedula.InmuebleId);
                cedula.facturas = await vFacturas.getFacturas(id, cedula.ServicioId);//
                cedula.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedula.facturas);
                cedula.usuarios = await vUsuarios.getUserById(cedula.UsuarioId);
                cedula.iEntregables = await vEntregables.getEntregables(cedula.Id);
                cedula.incidencias = new Entities.MIncidencias.ModelsIncidencias();
                cedula.incidencias.traslado = await iTraslado.getIncidencias(cedula.Id);
                cedula.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedula.RespuestasEncuesta = await vCedula.obtieneRespuestas(cedula.Id);
                cedula.historialCedulas = new List<HistorialCedulas>();
                cedula.historialCedulas = await vTraslado.getHistorialTraslado(cedula.Id);
                foreach (var user in cedula.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                return View(cedula);
            }
            return Redirect("/error/denied");
        }

        [HttpGet]
        [Route("/trasladoExp/seguimiento/{id}")]
        public async Task<IActionResult> SeguimientoCedula(int id)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "seguimiento");
            if (success == 1)
            {
                CedulaEvaluacion cedula = null;
                cedula = await vCedula.CedulaById(id);
                cedula.URL = Request.QueryString.Value;
                cedula.inmuebles = await vInmuebles.inmuebleById(cedula.InmuebleId);
                cedula.facturas = await vFacturas.getFacturas(id, cedula.ServicioId);//
                cedula.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedula.facturas);
                cedula.usuarios = await vUsuarios.getUserById(cedula.UsuarioId);
                cedula.iEntregables = await vEntregables.getEntregables(cedula.Id);
                cedula.historialCedulas = new List<HistorialCedulas>();
                cedula.historialCedulas = await vTraslado.getHistorialTraslado(cedula.Id);
                foreach (var user in cedula.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                cedula.historialEntregables = new List<HistorialEntregables>();
                cedula.historialEntregables = await vEntregables.getHistorialEntregables(cedula.Id, cedula.ServicioId);
                foreach (var user in cedula.historialEntregables)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                return View(cedula);
            }
            return Redirect("/error/denied");
        }

        [HttpPost]
        [Route("/trasladoExp/historialTraslado")]
        public async Task<IActionResult> historialTraslado([FromBody] HistorialCedulas historialCedulas)
        {
            int success = 0;
            success = await vTraslado.capturaHistorial(historialCedulas);
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
        [Route("trasladoExp/aprovRechCed")]
        public async Task<IActionResult> aprovacionRechazoCedula([FromBody] TrasladoExpedientes trasladoExpedientes)
        {
            int success = await vTraslado.apruebaRechazaCedula(trasladoExpedientes);
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
            return "Traslado_de_Expedientes";
        }
    }
}
