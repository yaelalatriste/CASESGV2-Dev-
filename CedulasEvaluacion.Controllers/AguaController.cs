using CedulasEvaluacion.Entities.MAgua;
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
        private readonly IRepositorioEntregablesCedula eAgua;
        private readonly IRepositorioInmuebles vInmuebles;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IRepositorioPerfiles vRepositorioPerfiles;
        private readonly IRepositorioFacturas vFacturas;

        public AguaController(IRepositorioEvaluacionServicios viCedula, IRepositorioInmuebles iVInmueble, IRepositorioUsuarios iVUsuario,IRepositorioIncidenciasAgua iiAgua, 
                              IRepositorioEntregablesCedula eeAgua, IRepositorioPerfiles iRepositorioPerfiles, IRepositorioFacturas iFacturas)
        {
            this.vCedula = viCedula ?? throw new ArgumentNullException(nameof(viCedula));
            this.iAgua = iiAgua ?? throw new ArgumentNullException(nameof(iiAgua));
            this.eAgua = eeAgua ?? throw new ArgumentNullException(nameof(eeAgua));
            this.vFacturas = iFacturas ?? throw new ArgumentNullException(nameof(iFacturas));
            this.vInmuebles = iVInmueble ?? throw new ArgumentNullException(nameof(iVInmueble));
            this.vUsuarios = iVUsuario ?? throw new ArgumentNullException(nameof(iVUsuario));
            this.vRepositorioPerfiles = iRepositorioPerfiles ?? throw new ArgumentNullException(nameof(iRepositorioPerfiles));
        }

        [Route("/agua/index/{servicio?}")]
        public async Task<IActionResult> Index(int servicio, [FromQuery(Name = "Estatus")] string Estatus, [FromQuery(Name = "Mes")] string Mes,
            [FromQuery(Name = "Inmueble")] int Inmueble)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "ver");

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
                cedAgua.URL = Request.QueryString.Value;
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
                cedAgua.historialCedulas = await vCedula.getHistorial(cedAgua.Id);
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
                CedulaEvaluacion cedAgua = null;
                cedAgua = await vCedula.CedulaById(id);
                cedAgua.URL = Request.QueryString.Value;
                cedAgua.facturas = await vFacturas.getFacturas(id, cedAgua.ServicioId);//
                cedAgua.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedAgua.facturas);
                cedAgua.inmuebles = await vInmuebles.inmuebleById(cedAgua.InmuebleId);
                cedAgua.usuarios = await vUsuarios.getUserById(cedAgua.UsuarioId);
                cedAgua.iEntregables = await eAgua.getEntregables(cedAgua.Id);
                cedAgua.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedAgua.RespuestasEncuesta = await vCedula.obtieneRespuestas(cedAgua.Id);
                cedAgua.historialCedulas = new List<HistorialCedulas>();
                cedAgua.historialCedulas = await vCedula.getHistorial(cedAgua.Id);
                foreach (var user in cedAgua.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                cedAgua.historialEntregables = new List<HistorialEntregables>();
                cedAgua.historialEntregables = await eAgua.getHistorialEntregables(cedAgua.Id, cedAgua.ServicioId);
                foreach (var user in cedAgua.historialEntregables)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                return View(cedAgua);
            }
            return Redirect("/error/denied");
        }

        [HttpPost]
        [Route("/agua/aprovRechCed")]
        public async Task<IActionResult> aprovacionRechazoCedula([FromBody] CedulaEvaluacion cedulaAgua)
        {
            int success = await vCedula.apruebaRechazaCedula(cedulaAgua);
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
