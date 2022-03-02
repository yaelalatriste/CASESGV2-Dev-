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
    public class MensajeriaController:Controller
    {
        private readonly IRepositorioMensajeria vMensajeria;
        private readonly IRepositorioIncidenciasMensajeria viMensajeria;
        private readonly IRepositorioEntregablesMensajeria veMensajeria;
        private readonly IRepositorioFacturas vFacturas;
        private readonly IRepositorioInmuebles vInmuebles;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IRepositorioPerfiles vPerfiles;
        private readonly IHostingEnvironment environment;

        public MensajeriaController(IRepositorioMensajeria iMensajeria, IRepositorioFacturas iFacturas, IRepositorioInmuebles iVInmueble, IRepositorioUsuarios iVUsuario,
                                    IRepositorioIncidenciasMensajeria iiMensajeria, IRepositorioEntregablesMensajeria ivMensajeria,
                                    IRepositorioPerfiles iPerfiles, IHostingEnvironment environment)
        {
            this.vMensajeria = iMensajeria ?? throw new ArgumentNullException(nameof(iMensajeria));
            this.viMensajeria = iiMensajeria ?? throw new ArgumentNullException(nameof(iiMensajeria));
            this.veMensajeria = ivMensajeria ?? throw new ArgumentNullException(nameof(ivMensajeria));
            this.vFacturas = iFacturas ?? throw new ArgumentNullException(nameof(iFacturas));
            this.vInmuebles = iVInmueble ?? throw new ArgumentNullException(nameof(iVInmueble));
            this.vUsuarios = iVUsuario ?? throw new ArgumentNullException(nameof(iVUsuario));
            this.vPerfiles = iPerfiles ?? throw new ArgumentNullException(nameof(iPerfiles));
            this.environment = environment;
        }

        //Metodo Index
        [Route("/mensajeria/index")]
        public async Task<IActionResult> Index()
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "ver");
            if (success == 1)
            {
                TempData["Title"] = "\"Cédulas - Servicio Mensajería\"";
                List<VCedulas> resultado = new List<VCedulas>();
                resultado = await vMensajeria.GetCedulasMensajeria(UserId());
                return View(resultado);
            }
            return Redirect("/error/denied"); 
        }

        //Metodo para abrir la vista y generar la nueva Cedula
        [Route("/mensajeria/nuevaCedula")]
        [HttpGet]
        public async Task<IActionResult> NuevaCedula(CedulaMensajeria cedulaMensajeria)
        {
            cedulaMensajeria = new CedulaMensajeria();
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "crear");
            if (success == 1)
            {
                return View(cedulaMensajeria);
            }
            return Redirect("/error/denied");
        }

        //inserta la cedula
        [Route("/mensajeria/new")]
        public async Task<IActionResult> insertaCedula([FromBody] CedulaMensajeria cedulaMensajeria)
        {
            cedulaMensajeria.UsuarioId = Convert.ToInt32(User.Claims.ElementAt(0).Value);
            int insert = await vMensajeria.insertaCedula(cedulaMensajeria);
            if (insert != -1)
            {
                return Ok(insert);
            }
            return BadRequest();
        }

        [Route("/mensajeria/validaPeriodo/{anio?}/{mes?}/{inmueble?}")]
        public async Task<IActionResult> validaPeriodo(int anio, string mes, int inmueble)
        {
            int valida = await vMensajeria.VerificaCedula(anio, mes, inmueble);
            if (valida != -1)
            {
                return Ok(valida);
            }
            return BadRequest();
        }

        [Route("/mensajeria/evaluacion/{id?}")]
        public async Task<IActionResult> Cuestionario(int id)
        {
            CedulaMensajeria cedulaMensajeria = await vMensajeria.CedulaById(id);
            if (cedulaMensajeria.Estatus.Equals("Enviado a DAS"))
            {
                return Redirect("/error/cedSend");
            }
            cedulaMensajeria.inmuebles = await vInmuebles.inmuebleById(cedulaMensajeria.InmuebleId);
            cedulaMensajeria.RespuestasEncuesta = await vMensajeria.obtieneRespuestas(id);
            cedulaMensajeria.facturas = await vFacturas.getFacturas(id,cedulaMensajeria.ServicioId);
            cedulaMensajeria.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedulaMensajeria.facturas);
            return View(cedulaMensajeria);
        }

        [Route("/mensajeria/incidencias/{id?}")]
        public async Task<IActionResult> IncidenciasMensajeria(int id)
        {
            CedulaMensajeria cedulaMensajeria = await vMensajeria.CedulaById(id);
            cedulaMensajeria.inmuebles = await vInmuebles.inmuebleById(cedulaMensajeria.InmuebleId);
            cedulaMensajeria.incidenciasMensajeria = await viMensajeria.getIncidenciasMensajeria(id);
            return View(cedulaMensajeria);
        }

        [HttpPost]
        [Route("/mensajeria/evaluation")]
        public async Task<IActionResult> guardaRespuestas([FromBody] List<RespuestasEncuesta> respuestas)
        {
            int success = await vMensajeria.GuardaRespuestas(respuestas);
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
        [Route("/mensajeria/sendCedula/{cedula?}")]
        public async Task<IActionResult> enviaCedula(int cedula)
        {
            int success = 0;
            success = await vMensajeria.enviaRespuestas(cedula);
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
        [Route("/mensajeria/revision/{id}")]
        public async Task<IActionResult> RevisarCedula(int id)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "revisión");
            if (success == 1)
            {
            CedulaMensajeria cedMen = null;
            cedMen = await vMensajeria.CedulaById(id);
            cedMen.facturas = await vFacturas.getFacturas(id, cedMen.ServicioId);//
                cedMen.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedMen.facturas);
                cedMen.inmuebles = await vInmuebles.inmuebleById(cedMen.InmuebleId);
                cedMen.usuarios = await vUsuarios.getUserById(cedMen.UsuarioId);
                cedMen.recoleccion = await viMensajeria.getIncidenciasByTipoMensajeria(cedMen.Id,"Recoleccion");
                cedMen.entrega = await viMensajeria.getIncidenciasByTipoMensajeria(cedMen.Id,"Entrega");
                cedMen.acuses = await viMensajeria.getIncidenciasByTipoMensajeria(cedMen.Id,"Acuses");
                cedMen.malEstado = await viMensajeria.getIncidenciasByTipoMensajeria(cedMen.Id,"Mal Estado");
                cedMen.extraviadas = await viMensajeria.getIncidenciasByTipoMensajeria(cedMen.Id,"Extraviadas");
                cedMen.robadas = await viMensajeria.getIncidenciasByTipoMensajeria(cedMen.Id,"Robadas");
                cedMen.iEntregables = await veMensajeria.getEntregables(cedMen.Id);
                cedMen.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedMen.RespuestasEncuesta = await vMensajeria.obtieneRespuestas(cedMen.Id);
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
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "seguimiento");
            if (success == 1)
            {
                CedulaMensajeria cedMen = null;
                cedMen = await vMensajeria.CedulaById(id);
                cedMen.facturas = await vFacturas.getFacturas(id, cedMen.ServicioId);//
                cedMen.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedMen.facturas);
                cedMen.inmuebles = await vInmuebles.inmuebleById(cedMen.InmuebleId);
                cedMen.usuarios = await vUsuarios.getUserById(cedMen.UsuarioId);
                /*cedMen.recoleccion = await viMensajeria.getIncidenciasByTipoMensajeria(cedMen.Id, "Recoleccion");
                cedMen.entrega = await viMensajeria.getIncidenciasByTipoMensajeria(cedMen.Id, "Entrega");
                cedMen.acuses = await viMensajeria.getIncidenciasByTipoMensajeria(cedMen.Id, "Acuses");
                cedMen.malEstado = await viMensajeria.getIncidenciasByTipoMensajeria(cedMen.Id, "Mal Estado");
                cedMen.extraviadas = await viMensajeria.getIncidenciasByTipoMensajeria(cedMen.Id, "Extraviadas");*/
                cedMen.iEntregables = await veMensajeria.getEntregables(cedMen.Id);
                cedMen.RespuestasEncuesta = new List<RespuestasEncuesta>();
                cedMen.RespuestasEncuesta = await vMensajeria.obtieneRespuestas(cedMen.Id);
                cedMen.historialCedulas = new List<HistorialCedulas>();
                cedMen.historialCedulas = await vMensajeria.getHistorialMensajeria(cedMen.Id);
                foreach (var user in cedMen.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                cedMen.historialEntregables = new List<HistorialEntregables>();
                cedMen.historialEntregables = await veMensajeria.getHistorialEntregables(cedMen.Id);
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

    }
}
