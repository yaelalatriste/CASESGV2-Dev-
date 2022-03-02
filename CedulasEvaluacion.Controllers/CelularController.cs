using CedulasEvaluacion.Entities.MCelular;
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
    public partial class CelularController : Controller
    {
        private readonly IRepositorioCelular vCelular;
        private readonly IRepositorioIncidenciasCelular iCelular;
        private readonly IRepositorioEntregablesCelular eCelular;
        private readonly IRepositorioPerfilCelular vPCelular;
        private readonly IRepositorioFacturas vFacturas;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IRepositorioPerfiles vPerfiles;
        private readonly IHostingEnvironment environment;

        public CelularController(IRepositorioCelular iCelular, IRepositorioIncidenciasCelular ivCelular, IRepositorioEntregablesCelular ieCelular, IRepositorioFacturas iFacturas, 
                                 IRepositorioUsuarios iUsuarios, IRepositorioPerfiles iPerfiles, IRepositorioPerfilCelular viPCelular, IHostingEnvironment environment)
        {
            this.vCelular = iCelular ?? throw new ArgumentNullException(nameof(iCelular));
            this.vPCelular = viPCelular ?? throw new ArgumentNullException(nameof(viPCelular));
            this.iCelular = ivCelular ?? throw new ArgumentNullException(nameof(ivCelular));
            this.eCelular = ieCelular ?? throw new ArgumentNullException(nameof(ieCelular)); 
            this.vFacturas = iFacturas ?? throw new ArgumentNullException(nameof(iFacturas)); 
            this.vUsuarios = iUsuarios ?? throw new ArgumentNullException(nameof(iUsuarios)); 
            this.vPerfiles = iPerfiles ?? throw new ArgumentNullException(nameof(iPerfiles)); 
            this.environment = environment;
        }

        //Metodo Index
        [Route("/telCelular/index")]
        public async Task<IActionResult> Index()
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "ver");
            if (success == 1)
            {
                List<VCedulas> resultado = new List<VCedulas>();
                resultado = await vCelular.GetCedulasCelular();
                return View(resultado);
            }
            return Redirect("/error/denied");
        }

        //Metodo para abrir la vista y generar la nueva Cedula
        [Route("/telCelular/nuevaCedula")]
        [HttpGet]
        public async Task<IActionResult> NuevaCedula(TelefoniaCelular telefoniaCelular)
        {
            telefoniaCelular = new TelefoniaCelular();
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "crear");
            if (success == 1)
            {
                return View(telefoniaCelular);
            }
            return Redirect("/error/denied");
        }

        //inserta la cedula
        [Route("/telCelular/new")]
        public async Task<IActionResult> insertaCedula([FromBody] TelefoniaCelular telefoniaCelular)
        {
            telefoniaCelular.UsuarioId = Convert.ToInt32(User.Claims.ElementAt(0).Value);
            int insert = await vCelular.insertaCedula(telefoniaCelular);
            if (insert != -1)
            {
                return Ok(insert);
            }
            return BadRequest();
        }

        [Route("/telCelular/validaPeriodo/{anio?}/{mes?}")]
        public async Task<IActionResult> validaPeriodo(int anio, string mes)
        {
            int valida = await vCelular.VerificaCedula(anio, mes);
            if (valida != -1)
            {
                return Ok(valida);
            }
            return BadRequest();
        }

        [Route("/telCelular/evaluacion/{id?}")]
        public async Task<IActionResult> Cuestionario(int id)
        {
            TelefoniaCelular telefoniaCelular = await vCelular.CedulaById(id);
            if (telefoniaCelular.Estatus.Equals("Enviado a DAS"))
            {
                return Redirect("/error/cedSend");
            }
            telefoniaCelular.RespuestasEncuesta = await vCelular.obtieneRespuestas(id);
            telefoniaCelular.facturas = await vFacturas.getFacturas(id, telefoniaCelular.ServicioId);
            telefoniaCelular.TotalMontoFactura = vFacturas.obtieneTotalFacturas(telefoniaCelular.facturas);
            return View(telefoniaCelular);
        }

        [Route("/telCelular/incidencias/{id?}")]
        public async Task<IActionResult> IncidenciasCelular(int id)
        {
            TelefoniaCelular telefoniaCelular = await vCelular.CedulaById(id);
            telefoniaCelular.incidenciasCelular = await iCelular.getIncidenciasCelular(id);
            return View(telefoniaCelular);
        }

        [HttpGet]
        [Route("/telCelular/seguimiento/{id}")]
        public async Task<IActionResult> SeguimientoCedula(int id)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "seguimiento");
            if (success == 1)
            {
                TelefoniaCelular telCel = null;
                telCel = await vCelular.CedulaById(id);
                telCel.facturas = await vFacturas.getFacturas(id, telCel.ServicioId);//
                telCel.TotalMontoFactura = vFacturas.obtieneTotalFacturas(telCel.facturas);
                telCel.usuarios = await vUsuarios.getUserById(telCel.UsuarioId);
                telCel.IEntregables = await eCelular.getEntregables(telCel.Id);
                telCel.RespuestasEncuesta = new List<RespuestasEncuesta>();
                telCel.RespuestasEncuesta = await vCelular.obtieneRespuestas(telCel.Id);
                telCel.historialCedulas = new List<HistorialCedulas>();
                telCel.historialCedulas = await vCelular.getHistorialCelular(telCel.Id);
                foreach (var user in telCel.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                telCel.historialEntregables = new List<HistorialEntregables>();
                telCel.historialEntregables = await eCelular.getHistorialEntregables(telCel.Id);
                foreach (var user in telCel.historialEntregables)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                return View(telCel);
            }
            return Redirect("/error/denied");
        }

        [HttpPost]
        [Route("/telCelular/evaluation")]
        public async Task<IActionResult> guardaRespuestas([FromBody] List<RespuestasEncuesta> respuestas)
        {
            int success = await vCelular.GuardaRespuestas(respuestas);
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
        [Route("/telCelular/sendCedula/{cedula?}")]
        public async Task<IActionResult> enviaCedula(int cedula)
        {
            int success = 0;
            success = await vCelular.enviaRespuestas(cedula);
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
        [Route("/telCelular/revision/{id}")]
        public async Task<IActionResult> RevisarCedula(int id)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "revisión");
            if (success == 1)
            {
                TelefoniaCelular telCel = null;
                telCel = await vCelular.CedulaById(id);
                telCel.facturas = await vFacturas.getFacturas(id, telCel.ServicioId);//
                telCel.TotalMontoFactura = vFacturas.obtieneTotalFacturas(telCel.facturas);
                telCel.usuarios = await vUsuarios.getUserById(telCel.UsuarioId);
                telCel.altaEntrega = await iCelular.ListIncidenciasTipoCelular(telCel.Id, "Alta_Equipo");
                telCel.altasentrega = await iCelular.ListIncidenciasTipoCelular(telCel.Id, "Alta");
                telCel.bajaServicio = await iCelular.ListIncidenciasTipoCelular(telCel.Id, "Baja");
                telCel.reactivacion = await iCelular.ListIncidenciasTipoCelular(telCel.Id, "Reactivacion");
                telCel.suspension = await iCelular.ListIncidenciasTipoCelular(telCel.Id, "Suspension");
                telCel.cambioPerfil = await iCelular.ListIncidenciasTipoCelular(telCel.Id,"Perfil");
                telCel.switcheoCard = await iCelular.ListIncidenciasTipoCelular(telCel.Id, "SIM");
                telCel.cambioRegion = await iCelular.ListIncidenciasTipoCelular(telCel.Id, "CambioNumeroRegion");
                telCel.servicioVozDatos = await iCelular.ListIncidenciasTipoCelular(telCel.Id, "VozDatos");
                telCel.diagnostico = await iCelular.ListIncidenciasTipoCelular(telCel.Id, "Diagnostico");
                telCel.reparacion = await iCelular.ListIncidenciasTipoCelular(telCel.Id, "Reparacion");
                telCel.IEntregables = await eCelular.getEntregables(telCel.Id);
                telCel.RespuestasEncuesta = new List<RespuestasEncuesta>();
                telCel.RespuestasEncuesta = await vCelular.obtieneRespuestas(telCel.Id);
                telCel.historialCedulas = new List<HistorialCedulas>();
                telCel.historialCedulas = await vCelular.getHistorialCelular(telCel.Id);
                foreach (var user in telCel.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                return View(telCel);
            }
            return Redirect("/error/denied");
        }

        [HttpPost]
        [Route("telCelular/aprovRechCed")]
        public async Task<IActionResult> aprovacionRechazoCedula([FromBody] TelefoniaCelular telefoniaCelular)
        {
            int success = await vCelular.apruebaRechazaCedula(telefoniaCelular);
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
        [Route("/telCelular/historialCelular")]
        public async Task<IActionResult> historialCelular([FromBody] HistorialCedulas historialCedulas)
        {
            int success = 0;
            success = await vCelular.capturaHistorial(historialCedulas);
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
            return "Telefonia_Celular";
        }

    }
}
