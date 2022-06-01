using CedulasEvaluacion.Entities.MCedula;
using CedulasEvaluacion.Entities.MMuebles;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.Vistas;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    public class MueblesController : Controller
    {
        private readonly IRepositorioEvaluacionServicios vCedula;
        private readonly IRepositorioMuebles vMuebles;
        private readonly IRepositorioIncidenciasMuebles iMuebles;
        private readonly IRepositorioEntregablesCedula eMuebles;
        private readonly IRepositorioInmuebles vInmuebles;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IRepositorioPerfiles vRepositorioPerfiles;
        private readonly IRepositorioFacturas vFacturas;
        private readonly IHostingEnvironment environment;

        public MueblesController(IRepositorioEvaluacionServicios viCedula, IRepositorioMuebles iVMuebles, IRepositorioInmuebles iVInmueble, IRepositorioUsuarios iVUsuario,
                                    IRepositorioIncidenciasMuebles iiMuebles, IRepositorioEntregablesCedula eeMuebles,
                                    IRepositorioPerfiles iRepositorioPerfiles, IRepositorioFacturas iFacturas, IHostingEnvironment environment)
        {
            this.vCedula = viCedula ?? throw new ArgumentNullException(nameof(viCedula));
            this.vMuebles = iVMuebles ?? throw new ArgumentNullException(nameof(iVMuebles));
            this.iMuebles = iiMuebles ?? throw new ArgumentNullException(nameof(iiMuebles));
            this.eMuebles = eeMuebles ?? throw new ArgumentNullException(nameof(eeMuebles));
            this.vFacturas = iFacturas ?? throw new ArgumentNullException(nameof(iFacturas));
            this.vInmuebles = iVInmueble ?? throw new ArgumentNullException(nameof(iVInmueble));
            this.vUsuarios = iVUsuario ?? throw new ArgumentNullException(nameof(iVUsuario));
            this.vRepositorioPerfiles = iRepositorioPerfiles ?? throw new ArgumentNullException(nameof(iRepositorioPerfiles));
            this.environment = environment;
        }

        //Metodo que regresa las cedulas aceptadas, guardadas o rechazadas 
        [Route("/muebles/index/{servicio?}")]
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
        [Route("/muebles/nuevaCedula/{servicio}")]
        [HttpGet]
        public async Task<IActionResult> NuevaCedula(int servicio)
        {
            //return Redirect("/casesg");
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
        [Route("/muebles/new")]
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

        [Route("/muebles/evaluacion/{id?}")]
        public async Task<IActionResult> Cuestionario(int id)
        {
            //return Redirect("/casesg");
             int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "actualizar");
             if (success == 1)
             {
                 CedulaEvaluacion cedula = await vCedula.CedulaById(id);
                 if (cedula.Estatus.Equals("Enviado a DAS") && isEvaluate() == true)
                 {
                     return Redirect("/error/cedSend");
                 }
                 cedula.URL = Request.QueryString.Value;
                 cedula.inmuebles = await vInmuebles.inmuebleById(cedula.InmuebleId);
                 cedula.inmuebleDestino = await vInmuebles.inmuebleById(cedula.InmuebleDestinoId);
                 cedula.RespuestasEncuesta = await vCedula.obtieneRespuestas(id);
                 cedula.facturas = await vFacturas.getFacturas(id, cedula.ServicioId);
                 cedula.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedula.facturas);
                 return View(cedula);
             }
             return Redirect("/error/denied");
            
        }

        [HttpPost]
        [Route("/muebles/evaluation")]
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
        [Route("/muebles/sendCedula/{servicio?}/{id?}")]
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
        [Route("/muebles/revision/{id}")]
        public async Task<IActionResult> RevisarCedula(int id)
        {
            //return Redirect("/casesg");
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "revisión");
            if (success == 1)
            {
                CedulaEvaluacion cedMuebles = null;
                cedMuebles = await vCedula.CedulaById(id);
                cedMuebles.URL = Request.QueryString.Value;
                cedMuebles.facturas = await vFacturas.getFacturas(id, cedMuebles.ServicioId);//
                cedMuebles.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedMuebles.facturas);
                cedMuebles.inmuebles = await vInmuebles.inmuebleById(cedMuebles.InmuebleId);
                cedMuebles.inmuebleDestino = await vInmuebles.inmuebleById(cedMuebles.InmuebleDestinoId);
                cedMuebles.incidencias = new Entities.MIncidencias.ModelsIncidencias();
                cedMuebles.usuarios = await vUsuarios.getUserById(cedMuebles.UsuarioId);
                cedMuebles.iEntregables = await eMuebles.getEntregables(cedMuebles.Id);
                cedMuebles.incidencias.muebles = await iMuebles.GetIncidencias(cedMuebles.Id);
                cedMuebles.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedMuebles.RespuestasEncuesta = await vCedula.obtieneRespuestas(cedMuebles.Id);
                cedMuebles.historialCedulas = new List<HistorialCedulas>();
                cedMuebles.historialCedulas = await vMuebles.getHistorialMuebles(cedMuebles.Id);
                foreach (var user in cedMuebles.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                if (cedMuebles.RespuestasEncuesta.Count < 8 && cedMuebles.Mes.Equals("Enero")) 
                {
                    return Redirect("/muebles/evaluacion/"+id);   
                }
                else
                {
                    return View(cedMuebles);
                }
            }
            return Redirect("/error/denied");
        }

        [HttpGet]
        [Route("/muebles/seguimiento/{id}")]
        public async Task<IActionResult> SeguimientoCedula(int id)
        {
            //return Redirect("/casesg");
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "seguimiento");
            if (success == 1)
            {
                CedulaEvaluacion cedMuebles = null;
                cedMuebles = await vCedula.CedulaById(id);
                cedMuebles.URL = Request.QueryString.Value;
                cedMuebles.facturas = await vFacturas.getFacturas(id, cedMuebles.ServicioId);//
                cedMuebles.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedMuebles.facturas);
                cedMuebles.inmuebles = await vInmuebles.inmuebleById(cedMuebles.InmuebleId);
                cedMuebles.inmuebleDestino = await vInmuebles.inmuebleById(cedMuebles.InmuebleDestinoId);
                cedMuebles.usuarios = await vUsuarios.getUserById(cedMuebles.UsuarioId);
                cedMuebles.iEntregables = await eMuebles.getEntregables(cedMuebles.Id);
                cedMuebles.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedMuebles.RespuestasEncuesta = await vCedula.obtieneRespuestas(cedMuebles.Id);
                cedMuebles.historialCedulas = new List<HistorialCedulas>();
                cedMuebles.historialCedulas = await vMuebles.getHistorialMuebles(cedMuebles.Id);
                foreach (var user in cedMuebles.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                cedMuebles.historialEntregables = new List<HistorialEntregables>();
                cedMuebles.historialEntregables = await eMuebles.getHistorialEntregables(cedMuebles.Id,cedMuebles.ServicioId);
                foreach (var user in cedMuebles.historialEntregables)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                return View(cedMuebles);
            }
            return Redirect("/error/denied");
        }

        [HttpPost]
        [Route("/muebles/aprovRechCed")]
        public async Task<IActionResult> aprovacionRechazoCedula([FromBody] CedulaMuebles cedulaMuebles)
        {
            int success = await vMuebles.apruebaRechazaCedula(cedulaMuebles);
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
        [Route("/muebles/historialMuebles")]
        public async Task<IActionResult> historialMuebles([FromBody] HistorialCedulas historialCedulas)
        {
            int success = 0;
            success = await vMuebles.capturaHistorial(historialCedulas);
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
            return "Bienes_Muebles";
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
