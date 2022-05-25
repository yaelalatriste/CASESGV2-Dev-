using CedulasEvaluacion.Entities.MCedula;
using CedulasEvaluacion.Entities.MMensajeria;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.Vistas;
using CedulasEvaluacion.Interfaces;
using CedulasEvaluacion.Repositories;
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
    public class MensajeriaController : Controller
    {
        private readonly IRepositorioEvaluacionServicios vCedula;
        private readonly IRepositorioMensajeria vMensajeria;
        private readonly IRepositorioIncidenciasMensajeria viMensajeria;
        private readonly IRepositorioEntregablesCedula veMensajeria;
        private readonly IRepositorioFacturas vFacturas;
        private readonly IRepositorioInmuebles vInmuebles;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IRepositorioPerfiles vPerfiles;

        public MensajeriaController(IRepositorioEvaluacionServicios viCedula, IRepositorioMensajeria iMensajeria, IRepositorioFacturas iFacturas, IRepositorioInmuebles iVInmueble, IRepositorioUsuarios iVUsuario,
                                    IRepositorioIncidenciasMensajeria iiMensajeria, IRepositorioEntregablesCedula ivMensajeria,
                                    IRepositorioPerfiles iPerfiles)
        {
            this.vCedula = viCedula ?? throw new ArgumentNullException(nameof(viCedula));
            this.vMensajeria = iMensajeria ?? throw new ArgumentNullException(nameof(iMensajeria));
            this.viMensajeria = iiMensajeria ?? throw new ArgumentNullException(nameof(iiMensajeria));
            this.veMensajeria = ivMensajeria ?? throw new ArgumentNullException(nameof(ivMensajeria));
            this.vFacturas = iFacturas ?? throw new ArgumentNullException(nameof(iFacturas));
            this.vInmuebles = iVInmueble ?? throw new ArgumentNullException(nameof(iVInmueble));
            this.vUsuarios = iVUsuario ?? throw new ArgumentNullException(nameof(iVUsuario));
            this.vPerfiles = iPerfiles ?? throw new ArgumentNullException(nameof(iPerfiles));

        }

        //Metodo que regresa las cedulas aceptadas, guardadas o rechazadas 
        [Route("/mensajeria/index/{servicio?}")]
        public async Task<IActionResult> Index(int servicio, [FromQuery(Name = "Estatus")] string Estatus, [FromQuery(Name = "Mes")] string Mes)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "ver");
            if (success == 1)
            {
                ModelsIndex models = new ModelsIndex();
                models.ServicioId = servicio;
                models.Estatus = Estatus;
                models.Mes = Mes;
                models.cedulasEstatus = await vCedula.GetCedulasEvaluacionEstatus(servicio, UserId());
                if (models.Estatus != null && !models.Estatus.Equals(""))
                {
                    models.cedulasMes = await vCedula.GetCedulasEvaluacionMes(servicio, UserId(), Estatus);
                }
                if (models.Mes != null && !models.Mes.Equals(""))
                {
                    models.cedulas = await vCedula.GetCedulasEvaluacionServicios(servicio, UserId(), Estatus, Mes);
                }
                return View(models);
            }
            
            return Redirect("/error/denied");
        }

        //Metodo para abrir la vista y generar la nueva Cedula
        [Route("/mensajeria/nuevaCedula/{servicio}")]
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
        [Route("/mensajeria/new")]
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


        [Route("/mensajeria/validaPeriodo/{servicio}/{anio?}/{mes?}/{inmueble?}")]
        public async Task<IActionResult> validaPeriodo(int servicio, int anio, string mes, int inmueble)
        {
            int valida = await vCedula.VerificaCedula(servicio, anio, mes, inmueble);
            if (valida != -1)
            {
                return Ok(valida);
            }
            return BadRequest();
        }

        [Route("/mensajeria/evaluacion/{id?}")]
        public async Task<IActionResult> Cuestionario(int id)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "actualizar");
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
        [Route("/mensajeria/evaluation")]
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
        [Route("/mensajeria/sendCedula/{servicio?}/{id?}")]
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

        [Route("/mensajeria/incidencias/{id?}")]
        public async Task<IActionResult> IncidenciasMensajeria(int id)
        {
            CedulaEvaluacion cedulaMensajeria = await vCedula.CedulaById(id);
            cedulaMensajeria.inmuebles = await vInmuebles.inmuebleById(cedulaMensajeria.InmuebleId);
            cedulaMensajeria.incidencias = new Entities.MIncidencias.ModelsIncidencias();
            cedulaMensajeria.incidencias.mensajeria = await viMensajeria.getIncidenciasMensajeria(id);
            return View(cedulaMensajeria);
        }

        [HttpGet]
        [Route("/mensajeria/revision/{id}")]
        public async Task<IActionResult> RevisarCedula(int id)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "revisión");
            if (success == 1)
            {
                CedulaEvaluacion cedMen = null;
                cedMen = await vCedula.CedulaById(id);
                cedMen.facturas = await vFacturas.getFacturas(id, cedMen.ServicioId);//
                cedMen.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedMen.facturas);
                cedMen.inmuebles = await vInmuebles.inmuebleById(cedMen.InmuebleId);
                cedMen.usuarios = await vUsuarios.getUserById(cedMen.UsuarioId);
                cedMen.incidencias = new Entities.MIncidencias.ModelsIncidencias();
                cedMen.incidencias.recoleccion = await viMensajeria.getIncidenciasByTipoMensajeria(cedMen.Id, "Recoleccion");
                cedMen.incidencias.entrega = await viMensajeria.getIncidenciasByTipoMensajeria(cedMen.Id, "Entrega");
                cedMen.incidencias.acuses = await viMensajeria.getIncidenciasByTipoMensajeria(cedMen.Id, "Acuses");
                cedMen.incidencias.malEstado = await viMensajeria.getIncidenciasByTipoMensajeria(cedMen.Id, "Mal Estado");
                cedMen.incidencias.extraviadas = await viMensajeria.getIncidenciasByTipoMensajeria(cedMen.Id, "Extraviadas");
                cedMen.incidencias.robadas = await viMensajeria.getIncidenciasByTipoMensajeria(cedMen.Id, "Robadas");
                cedMen.iEntregables = await veMensajeria.getEntregables(cedMen.Id);
                cedMen.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedMen.RespuestasEncuesta = await vCedula.obtieneRespuestas(cedMen.Id);
                cedMen.historialCedulas = new List<HistorialCedulas>();
                cedMen.historialCedulas = await vMensajeria.getHistorialMensajeria(cedMen.Id);
                foreach (var user in cedMen.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                return View(cedMen);
            }
            return Redirect("/error/denied");
        }

        [HttpGet]
        [Route("/mensajeria/seguimiento/{id}")]
        public async Task<IActionResult> SeguimientoCedula(int id)
        {
            CedulaEvaluacion cedMen = null;
            cedMen = await vCedula.CedulaById(id);
            cedMen.inmuebles = await vInmuebles.inmuebleById(cedMen.InmuebleId);
            if (cedMen.inmuebles.Tipo == 1)
            {
                return Redirect("/mensajeria/seguimiento/local/" + id);
            }
            else
            {
                return Redirect("/mensajeria/seguimiento/foraneo/" + id);
            }
        }

        [HttpGet]
        [Route("/mensajeria/seguimiento/local/{id}")]
        public async Task<IActionResult> SeguimientoCedulaCAE(int id)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "seguimiento");
            if (success == 1)
            {
                CedulaEvaluacion cedMen = null;
                cedMen = await vCedula.CedulaById(id);
                cedMen.facturas = await vFacturas.getFacturas(id, cedMen.ServicioId);//
                cedMen.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedMen.facturas);
                cedMen.inmuebles = await vInmuebles.inmuebleById(cedMen.InmuebleId);
                cedMen.usuarios = await vUsuarios.getUserById(cedMen.UsuarioId);
                cedMen.iEntregables = await veMensajeria.getEntregables(cedMen.Id);
                cedMen.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedMen.RespuestasEncuesta = await vCedula.obtieneRespuestas(cedMen.Id);
                cedMen.historialCedulas = new List<HistorialCedulas>();
                cedMen.historialCedulas = await vMensajeria.getHistorialMensajeria(cedMen.Id);
                foreach (var user in cedMen.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                cedMen.historialEntregables = new List<HistorialEntregables>();
                cedMen.historialEntregables = await veMensajeria.getHistorialEntregables(cedMen.Id,cedMen.ServicioId);
                foreach (var user in cedMen.historialEntregables)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                return View(cedMen);
            }
            return Redirect("/error/denied");
        }

        [HttpGet]
        [Route("/mensajeria/seguimiento/foraneo/{id}")]
        public async Task<IActionResult> SeguimientoCedulaCAR(int id)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "seguimiento");
            if (success == 1)
            {
                CedulaEvaluacion cedMen = null;
                cedMen = await vCedula.CedulaById(id);
                cedMen.facturas = await vFacturas.getFacturas(id, cedMen.ServicioId);//
                cedMen.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedMen.facturas);
                cedMen.inmuebles = await vInmuebles.inmuebleById(cedMen.InmuebleId);
                cedMen.usuarios = await vUsuarios.getUserById(cedMen.UsuarioId);
                cedMen.iEntregables = await veMensajeria.getEntregables(cedMen.Id);
                cedMen.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedMen.RespuestasEncuesta = await vCedula.obtieneRespuestas(cedMen.Id);
                cedMen.historialCedulas = new List<HistorialCedulas>();
                cedMen.historialCedulas = await vMensajeria.getHistorialMensajeria(cedMen.Id);
                foreach (var user in cedMen.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                cedMen.historialEntregables = new List<HistorialEntregables>();
                cedMen.historialEntregables = await veMensajeria.getHistorialEntregables(cedMen.Id, cedMen.ServicioId);
                foreach (var user in cedMen.historialEntregables)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                return View(cedMen);
            }
            return Redirect("/error/denied");
        }

        [HttpPost]
        [Route("mensajeria/aprovRechCed")]
        public async Task<IActionResult> aprovacionRechazoCedula([FromBody] CedulaMensajeria cedulaMensajeria)
        {
            int success = await vMensajeria.apruebaRechazaCedula(cedulaMensajeria, 0);
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
        [Route("/mensajeria/historialMensajeria")]
        public async Task<IActionResult> historialMensajeria([FromBody] HistorialCedulas historialCedulas)
        {
            int success = 0;
            success = await vMensajeria.capturaHistorial(historialCedulas);
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
            return "Mensajeria";
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
