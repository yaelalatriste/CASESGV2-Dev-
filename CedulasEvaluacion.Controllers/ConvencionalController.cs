using CedulasEvaluacion.Entities.MConvencional;
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
    public class ConvencionalController : Controller
    {
        private readonly IRepositorioConvencional vConvencional;
        private readonly IRepositorioIncidenciasConvencional iConvencional;
        private readonly IRepositorioEntregablesConvencional eConvencional;
        private readonly IRepositorioFacturas vFacturas;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IRepositorioPerfiles vPerfiles;
        private readonly IHostingEnvironment environment;

        public ConvencionalController(IRepositorioConvencional iConvencional, IRepositorioIncidenciasConvencional ivConvencional, IRepositorioEntregablesConvencional ieConvencional, 
                                      IRepositorioFacturas iFacturas, IRepositorioUsuarios iUsuarios, IRepositorioPerfiles iPerfiles, IHostingEnvironment environment)
        {
            this.vConvencional = iConvencional ?? throw new ArgumentNullException(nameof(iConvencional));
            this.iConvencional = ivConvencional ?? throw new ArgumentNullException(nameof(ivConvencional));
            this.eConvencional = ieConvencional ?? throw new ArgumentNullException(nameof(ieConvencional));
            this.vFacturas = iFacturas ?? throw new ArgumentNullException(nameof(iFacturas));
            this.vUsuarios = iUsuarios ?? throw new ArgumentNullException(nameof(iUsuarios));
            this.vPerfiles = iPerfiles ?? throw new ArgumentNullException(nameof(iPerfiles));
            this.environment = environment;
        }

        //Metodo Index
        [Route("/telConvencional/index")]
        public async Task<IActionResult> Index()
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "ver");
            if (success == 1)
            {
                List<VCedulas> resultado = new List<VCedulas>();
                resultado = await vConvencional.GetCedulasConvencional(UserId());
                return View(resultado);
            }
            return Redirect("/error/denied");
        }

        //Metodo para abrir la vista y generar la nueva Cedula
        [Route("/telConvencional/nuevaCedula")]
        [HttpGet]
        public async Task<IActionResult> NuevaCedula(TelefoniaConvencional telefoniaConvencional)
        {
            telefoniaConvencional = new TelefoniaConvencional();
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "crear");
            if (success == 1)
            {
                return View(telefoniaConvencional);
            }
            return Redirect("/error/denied");
        }

        //inserta la cedula
        [Route("/telConvencional/new")]
        public async Task<IActionResult> insertaCedula([FromBody] TelefoniaConvencional telefoniaConvencional)
        {
            telefoniaConvencional.UsuarioId = Convert.ToInt32(User.Claims.ElementAt(0).Value);
            int insert = await vConvencional.insertaCedula(telefoniaConvencional);
            if (insert != -1)
            {
                return Ok(insert);
            }
            return BadRequest();
        }

        [Route("/telConvencional/validaPeriodo/{anio?}/{mes?}")]
        public async Task<IActionResult> validaPeriodo(int anio, string mes)
        {
            int valida = await vConvencional.VerificaCedula(anio, mes);
            if (valida != -1)
            {
                return Ok(valida);
            }
            return BadRequest();
        }

        [Route("/telConvencional/evaluacion/{id?}")]
        public async Task<IActionResult> Cuestionario(int id)
        {
            TelefoniaConvencional telefoniaConvencional = await vConvencional.CedulaById(id);
            if (telefoniaConvencional.Estatus.Equals("Enviado a DAS"))
            {
                return Redirect("/error/cedSend");
            }
            telefoniaConvencional.RespuestasEncuesta = await vConvencional.obtieneRespuestas(id);
            telefoniaConvencional.facturas = await vFacturas.getFacturas(id, telefoniaConvencional.ServicioId);
            telefoniaConvencional.TotalMontoFactura = vFacturas.obtieneTotalFacturas(telefoniaConvencional.facturas);
            return View(telefoniaConvencional);
        }

        [Route("/telConvencional/incidencias/{id?}")]
        public async Task<IActionResult> IncidenciasConvencional(int id)
        {
            TelefoniaConvencional telefoniaConvencional = await vConvencional.CedulaById(id);
            telefoniaConvencional.incidenciasConvencional = await iConvencional.getIncidenciasConvencional(id);
            return View(telefoniaConvencional);
        }

        [HttpPost]
        [Route("/telConvencional/evaluation")]
        public async Task<IActionResult> guardaRespuestas([FromBody] List<RespuestasEncuesta> respuestas)
        {
            int success = await vConvencional.GuardaRespuestas(respuestas);
            if (success != 0)
            {
                return Ok(success);
            }
            else
            {
                return NotFound();
            }
        }

        //Va guardando las respuestas de la evaluacion
        [HttpPost]
        [Route("/telConvencional/sendCedula/{cedula?}")]
        public async Task<IActionResult> enviaCedula(int cedula)
        {
            int success = 0;
            success = await vConvencional.enviaRespuestas(cedula);
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
        [Route("/telConvencional/revision/{id}")]
        public async Task<IActionResult> RevisarCedula(int id)
        {
             int success = await vPerfiles.getPermiso(UserId(), modulo(), "revision");
             if (success == 1)
             {
                TelefoniaConvencional telCel = null;
                telCel = await vConvencional.CedulaById(id);
                telCel.facturas = await vFacturas.getFacturas(id, telCel.ServicioId);//
                telCel.TotalMontoFactura = vFacturas.obtieneTotalFacturas(telCel.facturas);
                telCel.usuarios = await vUsuarios.getUserById(telCel.UsuarioId);
                telCel.contratacion = await iConvencional.ListIncidenciasTipoConvencional(telCel.Id, "contratacion_instalacion");
                telCel.cableado = await iConvencional.ListIncidenciasTipoConvencional(telCel.Id, "cableado");
                telCel.entregaAparato = await iConvencional.ListIncidenciasTipoConvencional(telCel.Id, "entregaAparato");
                telCel.cambioDomicilio = await iConvencional.ListIncidenciasTipoConvencional(telCel.Id, "cambioDomicilio");
                telCel.reubicacion = await iConvencional.ListIncidenciasTipoConvencional(telCel.Id, "reubicacion");
                telCel.identificador = await iConvencional.ListIncidenciasTipoConvencional(telCel.Id, "identificadorLlamadas");
                telCel.instalaciónTroncal = await iConvencional.ListIncidenciasTipoConvencional(telCel.Id, "troncales");
                telCel.contratacionInternet = await iConvencional.ListIncidenciasTipoConvencional(telCel.Id, "internet");
                telCel.habilitacionServicios = await iConvencional.ListIncidenciasTipoConvencional(telCel.Id, "serviciosTelefonia");
                telCel.cancelacionServicios = await iConvencional.ListIncidenciasTipoConvencional(telCel.Id, "cancelacion");
                telCel.reporteFallas = await iConvencional.ListIncidenciasTipoConvencional(telCel.Id, "reportesFallas");
                telCel.iEntregables = await eConvencional.getEntregables(telCel.Id);
                telCel.RespuestasEncuesta = new List<RespuestasEncuesta>();
                telCel.RespuestasEncuesta = await vConvencional.obtieneRespuestas(telCel.Id);
                telCel.historialCedulas = new List<HistorialCedulas>();
                telCel.historialCedulas = await vConvencional.getHistorialConvencional(telCel.Id);
                foreach (var user in telCel.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                return View(telCel);
            }
            return Redirect("/error/denied");
        }

        [HttpGet]
        [Route("/telConvencional/seguimiento/{id}")]
        public async Task<IActionResult> SeguimientoCedula(int id)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "seguimiento");
            if (success == 1)
            {
                TelefoniaConvencional telCel = null;
                telCel = await vConvencional.CedulaById(id);
                telCel.facturas = await vFacturas.getFacturas(id, telCel.ServicioId);//
                telCel.TotalMontoFactura = vFacturas.obtieneTotalFacturas(telCel.facturas);
                telCel.usuarios = await vUsuarios.getUserById(telCel.UsuarioId);
                telCel.iEntregables = await eConvencional.getEntregables(telCel.Id);
                telCel.RespuestasEncuesta = new List<RespuestasEncuesta>();
                telCel.RespuestasEncuesta = await vConvencional.obtieneRespuestas(telCel.Id);
                telCel.historialCedulas = new List<HistorialCedulas>();
                telCel.historialCedulas = await vConvencional.getHistorialConvencional(telCel.Id);
                foreach (var user in telCel.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                telCel.historialEntregables = new List<HistorialEntregables>();
                telCel.historialEntregables = await eConvencional.getHistorialEntregables(telCel.Id);
                foreach (var user in telCel.historialEntregables)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                return View(telCel);
            }
            return Redirect("/error/denied");
        }

        [HttpPost]
        [Route("/telConvencional/aprovRechCed")]
        public async Task<IActionResult> aprovacionRechazoCedula([FromBody] TelefoniaConvencional telefoniaConvencional)
        {
            int success = await vConvencional.apruebaRechazaCedula(telefoniaConvencional);
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
        [Route("/telConvencional/historialConvencional")]
        public async Task<IActionResult> historialConvencional([FromBody] HistorialCedulas historialCedulas)
        {
            int success = 0;
            success = await vConvencional.capturaHistorial(historialCedulas);
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
            return "Telefonia_Convencional";
        }
    }
}
