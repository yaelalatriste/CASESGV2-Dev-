using CedulasEvaluacion.Entities.MIncidencias;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.Vistas;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    public class IncidenciasController : Controller
    {
        private readonly IRepositorioIncidencias vIncidencias;
        private readonly IRepositorioInmuebles vInmuebles;
        private readonly IRepositorioUsuarios vUsuarios;
        private readonly IHostingEnvironment environment;

        public IncidenciasController(IRepositorioIncidencias iIncidencias, IRepositorioInmuebles iVInmueble, IRepositorioUsuarios iVUsuario,
                                 IHostingEnvironment environment)
        {
            this.vIncidencias = iIncidencias ?? throw new ArgumentNullException(nameof(iIncidencias));
            this.vInmuebles = iVInmueble ?? throw new ArgumentNullException(nameof(iVInmueble));
            this.vUsuarios = iVUsuario ?? throw new ArgumentNullException(nameof(iVUsuario));
            this.environment = environment;
        }


        /*INCIDENCIAS LIMPIEZA*/

        [HttpGet]
        //[Route("cedLimpieza/getTiposIncidencia")]
        [Route("/limpieza/getTiposIncidencia")]
        public async Task<ActionResult<List<CatalogoIncidencias>>> getTipos()
        {
            List<CatalogoIncidencias> resp = null;
            resp = await vIncidencias.getTiposIncidencias();
            return Ok(resp);
        }

        /*Metodo que obtiene las Áreas en base al tipo de incidencia*/
        [HttpGet]
        [Route("/limpieza/getTiposNombre")]
        public async Task<ActionResult<List<CatalogoIncidencias>>> getNombreByTipo([FromQuery] string tipo)
        {
            List<CatalogoIncidencias> nombres = null;
            nombres = await vIncidencias.getNombresByTipos(tipo);
            return Ok(nombres);
        }
        /*FIN Metodo que obtiene las Áreas en base al tipo de incidencia*/

        /*Metodo que guarda las incidencias*/
        [HttpPost]
        [Route("/limpieza/guardaIncidencias")]
        public async Task<IActionResult> guardaIncidencias([FromBody] IncidenciasLimpieza incidencia)
        {
            int success = 0;
            if (incidencia.Id == 0)
            {
                success = await vIncidencias.guardaIncidencia(incidencia);
            }
            else
            {
                success = await vIncidencias.updateIncidencia(incidencia);
            }

            if (success != 0)
            {
                return Ok();
            }

            return NoContent();
        }

        /*Metodo para obtener la incidencia antes de actualizar*/
        [HttpGet]
        [Route("/limpieza/getIncidencia/{id}")]
        public async Task<IActionResult> getIncidenciaBeforeUpdate(int id)
        {
            IncidenciasLimpieza incidencia = null;
            incidencia = await vIncidencias.getIncidenciaBeforeUpdate(id);
            if (incidencia != null)
            {
                return Ok(incidencia);
            }

            return NoContent();
        }
        /*FIN Metodo para obtener la incidencia antes de actualizar*/

        /*Metodo para obtener las incidencias*/
        [HttpGet]
        [Route("/limpieza/getIncidencias/{id}")]
        public async Task<IActionResult> getIncidencias(int id)
        {
            List<VIncidenciasLimpieza> success = null;
            success = await vIncidencias.getIncidencias(id);
            string table = "";
            if (success != null)
            {
                int i = 0;
                foreach (var tb in success)
                {
                    i++;
                    table += "<tr>" +
                        "<td>" + (i)+ "</td>" +
                        "<td>" + tb.FechaIncidencia.ToShortDateString() + "</td>" +
                        "<td>" + tb.Tipo + "</td>" +
                        "<td>" + tb.Nombre + "</td>" +
                        "<td>" + tb.Comentarios + "</td>" +
                        "<td>" +
                        "<a href='#' class='text-center mr-2 update_incidencia' data-id='" + tb.Id + "'><i class='fas fa-edit text-primary'></i></a>" +
                        "<a href='#' class='text-center mr-2 delete_incidencia' data-id='" + tb.Id + "'><i class='fas fa-times text-danger'></i></a>" +
                        "</td>" +
                        "</tr>";
                }
                return Ok(table);
            }

            return NoContent();
        }
        /*FIN Metodo para obtener las incidencias*/

        /*Metodo para eliminar TODAS las incidencias*/
        [HttpGet]
        [Route("/limpieza/eliminarIncidencias/{cedulaId}")]
        public async Task<IActionResult> eliminaIncidencias(int cedulaId)
        {
            List<VIncidenciasLimpieza> success = null;
            success = await vIncidencias.getIncidencias(cedulaId);
            if (success != null)
            {
                foreach (var tipo in success)
                {
                    if (!tipo.Tipo.Equals("Equipo"))
                    {
                        await vIncidencias.deleteIncidencia(tipo.Id);
                    }
                }
                return Ok();
            }

            return NoContent();
        }
        /*FIN del Metodo para eliminar TODAS las incidencias*/

        [Route("/limpieza/incidencia/eliminar/{id?}")]
        public async Task<IActionResult> EliminaIncidencia(int id)
        {
            int success = await vIncidencias.deleteIncidencia(id);
            if (success != -1)
            {
                return Ok(success);
            }
            return BadRequest();
        }


        /**********************Incidencias de EQUIPO******************************/

        /*Metodo para obtener las incidencias de Equipo*/
        [HttpGet]
        [Route("/limpieza/getIncidenciasEquipo/{id}")]
        public async Task<IActionResult> getIncidenciasEquipo(int id)
        {
            List<VIncidenciasLimpieza> success = null;
            success = await vIncidencias.getIncidenciasEquipo(id);
            string table = "";
            if (success != null)
            {
                foreach (var tb in success)
                {
                    table += "<tr>" +
                        "<td>" + tb.Id + "</td>" +
                        "<td>" + tb.FechaIncidencia.ToShortDateString() + "</td>" +
                        "<td>" + tb.Tipo + "</td>" +
                        "<td>" + tb.Nombre + "</td>" +
                        "<td>" + tb.Comentarios + "</td>" +
                        "<td>" +
                        "<a href='#' class='text-center mr-2 update_incidenciaEq' data-id='" + tb.Id + "'><i class='fas fa-edit text-primary'></i></a>" +
                        "<a href='#' class='text-center mr-2 delete_incidencia' data-id='" + tb.Id + "'><i class='fas fa-times text-danger'></i></a>" +
                        "</td>" +
                        "</tr>";
                }
                return Ok(table);
            }

            return NoContent();
        }
        /*FIN Metodo para obtener las incidencias*/

        /*Metodo para eliminar las incidencias de Equipo*/
        [HttpGet]
        [Route("/limpieza/eliminaIncidenciasEquipo/{cedulaId}")]
        public async Task<IActionResult> eliminaIncidenciasEquipo(int cedulaId)
        {
            List<VIncidenciasLimpieza> success = null;
            success = await vIncidencias.getIncidenciasEquipo(cedulaId);
            if (success != null)
            {
                foreach (var tipo in success)
                {
                    if (tipo.Tipo.Equals("Equipo"))
                    {
                        await vIncidencias.deleteIncidencia(tipo.Id);
                    }
                }
                return Ok();
            }

            return NoContent();
        }
        /*FIN Metodo para obtener las incidencias*/

        /*FIN INCIDENCIAS LIMPIEZA*/
    }
}
