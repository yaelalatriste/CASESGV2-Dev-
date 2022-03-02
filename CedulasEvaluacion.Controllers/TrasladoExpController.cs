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
        private readonly IRepositorioTrasladoExp vTraslado;
        private readonly IRepositorioEntregablesTrasladoExp vEntregables;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IRepositorioIncidenciasTraslado iTraslado;
        private readonly IRepositorioPerfiles vPerfiles;
        private readonly IHostingEnvironment environment;
        private readonly IRepositorioFacturas vFacturas;

        public TrasladoExpController(IRepositorioTrasladoExp iTraslado, IRepositorioFacturas iFacturas, IRepositorioEntregablesTrasladoExp iVEntregables, IRepositorioUsuarios iVUsuario,
                                     IRepositorioIncidenciasTraslado ivTraslado, IRepositorioPerfiles iPerfiles, IHostingEnvironment environment)
        {
            this.vTraslado= iTraslado ?? throw new ArgumentNullException(nameof(iTraslado));
            this.vFacturas = iFacturas ?? throw new ArgumentNullException(nameof(iFacturas));
            this.vUsuarios = iVUsuario ?? throw new ArgumentNullException(nameof(iVUsuario));
            this.vPerfiles = iPerfiles ?? throw new ArgumentNullException(nameof(iPerfiles));
            this.iTraslado = ivTraslado ?? throw new ArgumentNullException(nameof(ivTraslado));
            this.vEntregables = iVEntregables ?? throw new ArgumentNullException(nameof(iVEntregables));
        }

        [Route("/trasladoExp/index")]
        public async Task<IActionResult> Index()
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "ver");
            List<VCedulas> resultado = new List<VCedulas>();
            if (success == 1)
            {
                resultado = await vTraslado.getCedulasTraslado();
                return View(resultado);
            }
            return Redirect("/error/denied");
        }

        //Metodo para abrir la vista y generar la nueva Cedula
        [Route("/trasladoExp/nuevaCedula")]
        [HttpGet]
        public async Task<IActionResult> NuevaCedula(TrasladoExpedientes cedulaTraslado)
        {
           int success = await vPerfiles.getPermiso(UserId(), modulo(), "crear");
            cedulaTraslado = new TrasladoExpedientes();
            if (success == 1)
            {
                return View(cedulaTraslado);
            }
            return Redirect("/error/denied");
        }

        [Route("/trasladoExp/validaPeriodo/{anio?}/{mes?}")]
        public async Task<IActionResult> validaPeriodo(int anio, string mes)
        {
            int valida = await vTraslado.VerificaCedula(anio, mes);
            if (valida != -1)
            {
                return Ok(valida);
            }
            return BadRequest();
        }

        //inserta la cedula
        [Route("/trasladoExp/new")]
        public async Task<IActionResult> insertaCedula([FromBody] TrasladoExpedientes trasladoExpedientes)
        {
            trasladoExpedientes.UsuarioId = Convert.ToInt32(User.Claims.ElementAt(0).Value);
            int insert = await vTraslado.insertaCedula(trasladoExpedientes);
            if (insert != -1)
            {
                return Ok(insert);
            }
            return BadRequest();
        }

        [Route("/trasladoExp/evaluacion/{id?}")]
        public async Task<IActionResult> Cuestionario(int id)
        {
            TrasladoExpedientes traslado = await vTraslado.CedulaById(id);
            if (traslado.Estatus.Equals("Enviado a DAS"))
            {
                return Redirect("/error/cedSend");
            }
            traslado.RespuestasEncuesta = await vTraslado.obtieneRespuestas(id);
            traslado.facturas = await vFacturas.getFacturas(id, traslado.ServicioId);
            traslado.TotalMontoFactura = vFacturas.obtieneTotalFacturas(traslado.facturas);
            return View(traslado);
        }

        [HttpPost]
        [Route("/trasladoExp/evaluation")]
        public async Task<IActionResult> guardaRespuestas([FromBody] List<RespuestasEncuesta> respuestas)
        {
            int success = await vTraslado.GuardaRespuestas(respuestas);
            if (success != 0)
            {
                return Ok(success);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("/trasladoExp/sendCedula/{cedula?}")]
        public async Task<IActionResult> enviaCedula(int cedula)
        {
            int success = 0;
            success = await vTraslado.enviaRespuestas(cedula);
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
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "revision");
            if (success == 1)
            {
                TrasladoExpedientes cedMen = null;
                cedMen = await vTraslado.CedulaById(id);
                cedMen.facturas = await vFacturas.getFacturas(id, cedMen.ServicioId);//
                cedMen.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedMen.facturas);
                cedMen.usuarios = await vUsuarios.getUserById(cedMen.UsuarioId);
                cedMen.iEntregables = await vEntregables.getEntregables(cedMen.Id);
                cedMen.incidenciasP1 = await iTraslado.getIncidencias(cedMen.Id,1);
                cedMen.incidenciasP2 = await iTraslado.getIncidencias(cedMen.Id,2);
                cedMen.incidenciasP3 = await iTraslado.getIncidencias(cedMen.Id,3);
                cedMen.incidenciasP4 = await iTraslado.getIncidencias(cedMen.Id,4);
                cedMen.incidenciasP5 = await iTraslado.getIncidencias(cedMen.Id,5);
                cedMen.incidenciasP6 = await iTraslado.getIncidencias(cedMen.Id,6);
                cedMen.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedMen.RespuestasEncuesta = await vTraslado.obtieneRespuestas(cedMen.Id);
                cedMen.historialCedulas = new List<HistorialCedulas>();
                cedMen.historialCedulas = await vTraslado.getHistorialTraslado(cedMen.Id);
                foreach (var user in cedMen.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                return View(cedMen);
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
