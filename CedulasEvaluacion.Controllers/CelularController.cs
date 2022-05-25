using CedulasEvaluacion.Entities.MCedula;
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
        private readonly IRepositorioEvaluacionServicios vCedula;
        private readonly IRepositorioCelular vCelular;
        private readonly IRepositorioIncidenciasCelular iCelular;
        private readonly IRepositorioEntregablesCedula eCelular;
        private readonly IRepositorioPerfilCelular vPCelular;
        private readonly IRepositorioFacturas vFacturas;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IRepositorioPerfiles vPerfiles;
        private readonly IHostingEnvironment environment;

        public CelularController(IRepositorioEvaluacionServicios viCedula, IRepositorioCelular iCelular, IRepositorioIncidenciasCelular ivCelular, IRepositorioEntregablesCedula ieCelular, IRepositorioFacturas iFacturas, 
                                 IRepositorioUsuarios iUsuarios, IRepositorioPerfiles iPerfiles, IRepositorioPerfilCelular viPCelular, IHostingEnvironment environment)
        {
            this.vCedula = viCedula ?? throw new ArgumentNullException(nameof(viCedula));
            this.vCelular = iCelular ?? throw new ArgumentNullException(nameof(iCelular));
            this.vPCelular = viPCelular ?? throw new ArgumentNullException(nameof(viPCelular));
            this.iCelular = ivCelular ?? throw new ArgumentNullException(nameof(ivCelular));
            this.eCelular = ieCelular ?? throw new ArgumentNullException(nameof(ieCelular)); 
            this.vFacturas = iFacturas ?? throw new ArgumentNullException(nameof(iFacturas)); 
            this.vUsuarios = iUsuarios ?? throw new ArgumentNullException(nameof(iUsuarios)); 
            this.vPerfiles = iPerfiles ?? throw new ArgumentNullException(nameof(iPerfiles)); 
            this.environment = environment;
        }

        //Metodo que regresa las cedulas aceptadas, guardadas o rechazadas 
        [Route("/telCelular/index/{servicio?}")]
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
        [Route("/telCelular/nuevaCedula/{servicio}")]
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
        [Route("/telCelular/new")]
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


        [Route("/telCelular/validaPeriodo/{servicio}/{anio?}/{mes?}/{inmueble?}")]
        public async Task<IActionResult> validaPeriodo(int servicio, int anio, string mes, int inmueble)
        {
            int valida = await vCedula.VerificaCedula(servicio, anio, mes, inmueble);
            if (valida != -1)
            {
                return Ok(valida);
            }
            return BadRequest();
        }

        [Route("/telCelular/evaluacion/{id?}")]
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
                cedula.RespuestasEncuesta = await vCedula.obtieneRespuestas(id);
                cedula.facturas = await vFacturas.getFacturas(id, cedula.ServicioId);
                cedula.TotalMontoFactura = vFacturas.obtieneTotalFacturas(cedula.facturas);
                return View(cedula);
            }
            return Redirect("/error/denied");
        }

        [HttpPost]
        [Route("/telCelular/evaluation")]
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
        [Route("/telCelular/sendCedula/{servicio?}/{id?}")]
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

        [Route("/telCelular/incidencias/{id?}")]
        public async Task<IActionResult> IncidenciasCelular(int id)
        {
            CedulaEvaluacion telCel = await vCedula.CedulaById(id);
            telCel.incidencias = new Entities.MIncidencias.ModelsIncidencias();
            telCel.incidencias.celular = await iCelular.getIncidenciasCelular(id);
            return View(telCel);
        }

        [HttpGet]
        [Route("/telCelular/seguimiento/{id}")]
        public async Task<IActionResult> SeguimientoCedula(int id)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "seguimiento");
            if (success == 1)
            {
                CedulaEvaluacion telCel = null;
                telCel = await vCedula.CedulaById(id);
                telCel.facturas = await vFacturas.getFacturas(id, telCel.ServicioId);//
                telCel.TotalMontoFactura = vFacturas.obtieneTotalFacturas(telCel.facturas);
                telCel.usuarios = await vUsuarios.getUserById(telCel.UsuarioId);
                telCel.iEntregables = await eCelular.getEntregables(telCel.Id);
                telCel.RespuestasEncuesta = new List<RespuestasEncuesta>();
                telCel.RespuestasEncuesta = await vCedula.obtieneRespuestas(telCel.Id);
                telCel.historialCedulas = new List<HistorialCedulas>();
                telCel.historialCedulas = await vCelular.getHistorialCelular(telCel.Id);
                foreach (var user in telCel.historialCedulas)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                telCel.historialEntregables = new List<HistorialEntregables>();
                telCel.historialEntregables = await eCelular.getHistorialEntregables(telCel.Id,telCel.ServicioId);
                foreach (var user in telCel.historialEntregables)
                {
                    user.usuarios = await vUsuarios.getUserById(user.UsuarioId);
                }
                return View(telCel);
            }
            return Redirect("/error/denied");
        }

        [HttpGet]
        [Route("/telCelular/revision/{id}")]
        public async Task<IActionResult> RevisarCedula(int id)
        {
            int success = await vPerfiles.getPermiso(UserId(), modulo(), "revisión");
            if (success == 1)
            {
                CedulaEvaluacion telCel = null;
                telCel = await vCedula.CedulaById(id);
                telCel.facturas = await vFacturas.getFacturas(id, telCel.ServicioId);//
                telCel.TotalMontoFactura = vFacturas.obtieneTotalFacturas(telCel.facturas);
                telCel.usuarios = await vUsuarios.getUserById(telCel.UsuarioId);
                telCel.incidencias = new Entities.MIncidencias.ModelsIncidencias();
                telCel.incidencias.altaEntrega = await iCelular.ListIncidenciasTipoCelular(telCel.Id, "Alta_Equipo");
                telCel.incidencias.altasentrega = await iCelular.ListIncidenciasTipoCelular(telCel.Id, "Alta");
                telCel.incidencias.bajaServicio = await iCelular.ListIncidenciasTipoCelular(telCel.Id, "Baja");
                telCel.incidencias.reactivacion = await iCelular.ListIncidenciasTipoCelular(telCel.Id, "Reactivacion");
                telCel.incidencias.suspension = await iCelular.ListIncidenciasTipoCelular(telCel.Id, "Suspension");
                telCel.incidencias.cambioPerfil = await iCelular.ListIncidenciasTipoCelular(telCel.Id,"Perfil");
                telCel.incidencias.switcheoCard = await iCelular.ListIncidenciasTipoCelular(telCel.Id, "SIM");
                telCel.incidencias.cambioRegion = await iCelular.ListIncidenciasTipoCelular(telCel.Id, "CambioNumeroRegion");
                telCel.incidencias.servicioVozDatos = await iCelular.ListIncidenciasTipoCelular(telCel.Id, "VozDatos");
                telCel.incidencias.diagnostico = await iCelular.ListIncidenciasTipoCelular(telCel.Id, "Diagnostico");
                telCel.incidencias.reparacion = await iCelular.ListIncidenciasTipoCelular(telCel.Id, "Reparacion");
                telCel.iEntregables = await eCelular.getEntregables(telCel.Id);
                telCel.RespuestasEncuesta = new List<RespuestasEncuesta>();
                telCel.RespuestasEncuesta = await vCedula.obtieneRespuestas(telCel.Id);
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
