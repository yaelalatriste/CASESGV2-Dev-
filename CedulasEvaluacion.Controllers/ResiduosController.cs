using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.MResiduos;
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
        private readonly IRepositorioResiduos vResiduos;
        private readonly IRepositorioIncidenciasResiduos iResiduos;
        private readonly IRepositorioEntregablesResiduos eResiduos;
        private readonly IRepositorioFacturas vFacturas;
        private readonly IRepositorioInmuebles vInmuebles;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IRepositorioPerfiles vPerfiles;
        private readonly IHostingEnvironment environment;

        public ResiduosController(IRepositorioResiduos iResiduos, IRepositorioIncidenciasResiduos ivResiduos, IRepositorioEntregablesResiduos ieResiduos, IRepositorioFacturas iFacturas,
                                 IRepositorioUsuarios iUsuarios, IRepositorioPerfiles iPerfiles, IRepositorioInmuebles iVInmueble, IHostingEnvironment environment)
        {
            this.vResiduos = iResiduos ?? throw new ArgumentNullException(nameof(iResiduos));
            this.iResiduos = ivResiduos ?? throw new ArgumentNullException(nameof(ivResiduos));
            this.eResiduos = ieResiduos ?? throw new ArgumentNullException(nameof(ieResiduos));
            this.vInmuebles = iVInmueble ?? throw new ArgumentNullException(nameof(iVInmueble));
            this.vFacturas = iFacturas ?? throw new ArgumentNullException(nameof(iFacturas));
            this.vUsuarios = iUsuarios ?? throw new ArgumentNullException(nameof(iUsuarios));
            this.vPerfiles = iPerfiles ?? throw new ArgumentNullException(nameof(iPerfiles));
            this.environment = environment;
        }

        //Metodo Index
        [Route("/residuos/index")]
        public async Task<IActionResult> Index()
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "ver");
            if (success == 1)
            {
                List<VCedulas> resultado = new List<VCedulas>();
                resultado = await vResiduos.GetCedulasResiduos(UserId());
                return View(resultado);
            }
            return Redirect("/error/denied");
        }

        //Metodo para abrir la vista y generar la nueva Cedula
        [Route("/residuos/nuevaCedula")]
        [HttpGet]
        public async Task<IActionResult> NuevaCedula(Residuos residuos)
        {
            residuos = new Residuos();
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "crear");
            if (success == 1)
            {
                return View(residuos);
            }
            return Redirect("/error/denied");
        }

        //inserta la cedula
        [Route("/residuos/new")]
        public async Task<IActionResult> insertaCedula([FromBody] Residuos residuos)
        {
            residuos.UsuarioId = Convert.ToInt32(User.Claims.ElementAt(0).Value);
            int insert = await vResiduos.insertaCedula(residuos);
            if (insert != -1)
            {
                return Ok(insert);
            }
            return BadRequest();
        }

        [Route("/residuos/validaPeriodo/{anio?}/{mes?}/{inmueble?}")]
        public async Task<IActionResult> validaPeriodo(int anio, string mes,int inmueble)
        {
            int valida = await vResiduos.VerificaCedula(anio, mes,inmueble);
            if (valida != -1)
            {
                return Ok(valida);
            }
            return BadRequest();
        }

        [Route("/residuos/evaluacion/{id?}")]
        public async Task<IActionResult> Cuestionario(int id)
        {
            Residuos residuos = await vResiduos.CedulaById(id);
            if (residuos.Estatus.Equals("Enviado a DAS"))
            {
                return Redirect("/error/cedSend");
            }
            residuos.inmuebles = await vInmuebles.inmuebleById(residuos.InmuebleId);
            residuos.RespuestasEncuesta = await vResiduos.obtieneRespuestas(id);
            residuos.facturas = await vFacturas.getFacturas(id, residuos.ServicioId);
            residuos.TotalMontoFactura = vFacturas.obtieneTotalFacturas(residuos.facturas);
            return View(residuos);
        }

        //Va guardando las respuestas de la evaluacion
        [HttpPost]
        [Route("/residuos/evaluation")]
        public async Task<IActionResult> guardaCuestionario([FromBody] List<RespuestasEncuesta> respuestas)
        {
            int success = await vResiduos.GuardaRespuestas(respuestas);
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
        [Route("/residuos/sendCedula/{cedula?}")]
        public async Task<IActionResult> enviaCedula(int cedula)
        {
            int success = 0;
            success = await vResiduos.enviaRespuestas(cedula);
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
                Residuos residuos= null;
                residuos = await vResiduos.CedulaById(id);
                residuos.facturas = await vFacturas.getFacturas(id, residuos.ServicioId);//
                residuos.TotalMontoFactura = vFacturas.obtieneTotalFacturas(residuos.facturas);
                residuos.inmuebles = await vInmuebles.inmuebleById(residuos.InmuebleId);
                residuos.usuarios = await vUsuarios.getUserById(residuos.UsuarioId);
                residuos.incidenciasManifiesto = await iResiduos.getIncidenciasTipo(residuos.Id,"ManifiestoEntrega");
                residuos.incidenciasResiduos = await iResiduos.getIncidencias(residuos.Id);
                residuos.rEntregables = await eResiduos.getEntregables(residuos.Id);
                residuos.RespuestasEncuesta = new List<RespuestasEncuesta>();
                residuos.RespuestasEncuesta = await vResiduos.obtieneRespuestas(residuos.Id);
                residuos.historialCedulas = new List<HistorialCedulas>();
                residuos.historialCedulas = await vResiduos.getHistorialResiduos(residuos.Id);
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
                Residuos residuos = null;
                residuos = await vResiduos.CedulaById(id);
                residuos.facturas = await vFacturas.getFacturas(id, residuos.ServicioId);//
                residuos.TotalMontoFactura = vFacturas.obtieneTotalFacturas(residuos.facturas);
                residuos.inmuebles = await vInmuebles.inmuebleById(residuos.InmuebleId);
                residuos.usuarios = await vUsuarios.getUserById(residuos.UsuarioId);
                residuos.rEntregables = await eResiduos.getEntregables(residuos.Id);
                residuos.RespuestasEncuesta = new List<RespuestasEncuesta>();
                residuos.RespuestasEncuesta = await vResiduos.obtieneRespuestas(residuos.Id);
                residuos.historialCedulas = new List<HistorialCedulas>();
                residuos.historialCedulas = await vResiduos.getHistorialResiduos(residuos.Id);
                foreach (var user in residuos.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                residuos.historialEntregables = new List<HistorialEntregables>();
                residuos.historialEntregables = await eResiduos.getHistorialEntregables(residuos.Id);
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
        public async Task<IActionResult> aprovacionRechazoCedula([FromBody] Residuos residuos)
        {
            int success = await vResiduos.apruebaRechazaCedula(residuos);
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
            success = await vResiduos.capturaHistorial(historialCedulas);
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
