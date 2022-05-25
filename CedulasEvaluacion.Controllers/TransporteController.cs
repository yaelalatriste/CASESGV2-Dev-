using CedulasEvaluacion.Entities.MCedula;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.MTransporte;
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
    public class TransporteController : Controller
    {
        private readonly IRepositorioEvaluacionServicios vCedula;
        private readonly IRepositorioTransporte vTransporte;
        private readonly IRepositorioIncidenciasTransporte iTransporte;
        private readonly IRepositorioEntregablesCedula eTransporte;
        private readonly IRepositorioInmuebles vInmuebles;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IRepositorioPerfiles vRepositorioPerfiles;
        private readonly IRepositorioFacturas vFacturas;

        public TransporteController(IRepositorioEvaluacionServicios viCedula, IRepositorioTransporte iVTransporte, IRepositorioInmuebles iVInmueble, IRepositorioUsuarios iVUsuario,
                                    IRepositorioIncidenciasTransporte iiTransporte, IRepositorioEntregablesCedula eeTransporte,
                                    IRepositorioPerfiles iRepositorioPerfiles, IRepositorioFacturas iFacturas)
        {
            this.vCedula = viCedula ?? throw new ArgumentNullException(nameof(viCedula));
            this.vTransporte = iVTransporte ?? throw new ArgumentNullException(nameof(iVTransporte));
            this.iTransporte = iiTransporte ?? throw new ArgumentNullException(nameof(iiTransporte));
            this.eTransporte = eeTransporte ?? throw new ArgumentNullException(nameof(eeTransporte));
            this.vFacturas = iFacturas ?? throw new ArgumentNullException(nameof(iFacturas));
            this.vInmuebles = iVInmueble ?? throw new ArgumentNullException(nameof(iVInmueble));
            this.vUsuarios = iVUsuario ?? throw new ArgumentNullException(nameof(iVUsuario));
            this.vRepositorioPerfiles = iRepositorioPerfiles ?? throw new ArgumentNullException(nameof(iRepositorioPerfiles));
        }

        //Metodo que regresa las cedulas aceptadas, guardadas o rechazadas 
        [Route("/transporte/index/{servicio?}")]
        public async Task<IActionResult> Index(int servicio, [FromQuery(Name = "Estatus")] string Estatus, [FromQuery(Name = "Mes")] string Mes)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "ver");
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
        [Route("/transporte/nuevaCedula/{servicio}")]
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
        [Route("/transporte/new")]
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


        [Route("/transporte/validaPeriodo/{servicio}/{anio?}/{mes?}/{inmueble?}")]
        public async Task<IActionResult> validaPeriodo(int servicio, int anio, string mes, int inmueble)
        {
            int valida = await vCedula.VerificaCedula(servicio, anio, mes, inmueble);
            if (valida != -1)
            {
                return Ok(valida);
            }
            return BadRequest();
        }

        [Route("/transporte/evaluacion/{id?}")]
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
        [Route("/transporte/evaluation")]
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
        [Route("/transporte/sendCedula/{servicio?}/{id?}")]
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
        [Route("/transporte/revision/{id}")]
        public async Task<IActionResult> RevisarCedula(int id)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "revisión");
            if (success == 1)
            {
                CedulaEvaluacion cedTran = null;
                cedTran = await vCedula.CedulaById(id);
                cedTran.facturas = await vFacturas.getFacturas(id, cedTran.ServicioId);//
                cedTran.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedTran.facturas);
                cedTran.inmuebles = await vInmuebles.inmuebleById(cedTran.InmuebleId);
                cedTran.usuarios = await vUsuarios.getUserById(cedTran.UsuarioId);
                cedTran.iEntregables = await eTransporte.getEntregables(cedTran.Id);
                cedTran.incidencias = new Entities.MIncidencias.ModelsIncidencias();
                cedTran.incidencias.transporte = await iTransporte.GetIncidencias(cedTran.Id);
                cedTran.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedTran.RespuestasEncuesta = await vCedula.obtieneRespuestas(cedTran.Id);
                cedTran.historialCedulas = new List<HistorialCedulas>();
                cedTran.historialCedulas = await vTransporte.getHistorialTransporte(cedTran.Id);
                foreach (var user in cedTran.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                return View(cedTran);
            }
            return Redirect("/error/denied");
        }

        [HttpGet]
        [Route("/transporte/seguimiento/{id}")]
        public async Task<IActionResult> SeguimientoCedula(int id)
        {
            int success = await vRepositorioPerfiles.getPermiso(UserId(), modulo(), "seguimiento");
            if (success == 1)
            {
                CedulaEvaluacion cedTran = null;
                cedTran = await vCedula.CedulaById(id);
                cedTran.facturas = await vFacturas.getFacturas(id, cedTran.ServicioId);//
                cedTran.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedTran.facturas);
                cedTran.inmuebles = await vInmuebles.inmuebleById(cedTran.InmuebleId);
                cedTran.usuarios = await vUsuarios.getUserById(cedTran.UsuarioId);
                cedTran.iEntregables = await eTransporte.getEntregables(cedTran.Id);
                cedTran.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedTran.RespuestasEncuesta = await vCedula.obtieneRespuestas(cedTran.Id);
                cedTran.historialCedulas = new List<HistorialCedulas>();
                cedTran.historialCedulas = await vTransporte.getHistorialTransporte(cedTran.Id);
                foreach (var user in cedTran.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                cedTran.historialEntregables = new List<HistorialEntregables>();
                cedTran.historialEntregables = await eTransporte.getHistorialEntregables(cedTran.Id,cedTran.ServicioId);
                foreach (var user in cedTran.historialEntregables)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                return View(cedTran);
            }
            return Redirect("/error/denied");
        }

        [HttpPost]
        [Route("/transporte/aprovRechCed")]
        public async Task<IActionResult> aprovacionRechazoCedula([FromBody] CedulaTransporte cedulaTransporte)
        {
            int success = await vTransporte.apruebaRechazaCedula(cedulaTransporte);
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
        [Route("/transporte/historialTransporte")]
        public async Task<IActionResult> historialTransporte([FromBody] HistorialCedulas historialCedulas)
        {
            int success = 0;
            success = await vTransporte.capturaHistorial(historialCedulas);
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
            return "Transporte_de_Personal";
        }
    }
}
