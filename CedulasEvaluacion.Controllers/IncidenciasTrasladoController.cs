using CedulasEvaluacion.Entities.TrasladoExp;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    public class IncidenciasTrasladoController : Controller
    {
        private readonly IRepositorioIncidenciasTraslado iTraslado;
        private readonly IHostingEnvironment environment;

        public IncidenciasTrasladoController(IRepositorioIncidenciasTraslado ivTraslado, IHostingEnvironment environment)
        {
            this.iTraslado = ivTraslado ?? throw new ArgumentNullException(nameof(ivTraslado));
        }

        [Route("/trasladoExp/inserta/incidencia")]
        public async Task<IActionResult> IncidenciasTraslado([FromBody] IncidenciasTraslado incidenciasTraslado)
        {
            int success = await iTraslado.InsertaIncidencia(incidenciasTraslado);
            if (success != -1)
            {
                return Ok(success);
            }
            return BadRequest();
        }

        [Route("/trasladoExp/actualiza/incidencia")]
        public async Task<IActionResult> ActualizaIncidencia([FromBody] IncidenciasTraslado incidenciasTraslado)
        {
            int success = await iTraslado.ActualizaIncidencia(incidenciasTraslado);
            if (success != -1)
            {
                return Ok(success);
            }
            return BadRequest();
        }

        [Route("/trasladoExp/getIncidencias")]
        public async Task<IActionResult> getIncidencias(int id)
        {
            List<IncidenciasTraslado> incidencias = await iTraslado.getIncidencias(id);
            if (incidencias != null)
            {
                return Ok(incidencias);
            }
            return BadRequest();
        }

        [Route("/trasladoExp/getIncidenciasTable/{id?}")]
        public async Task<IActionResult> getIncidenciasTable(int id)
        {
            List<IncidenciasTraslado> incidencias = await iTraslado.getIncidencias(id);
            string table = "";
            string coments = "";
            int i = 0;
            if (incidencias != null)
            {
                var com = incidencias[0].Comentarios.Split("|");
                for (var m = 0; m < com.Length; m++)
                {
                    coments += com + "<br />";
                    break;
                }

                foreach (var res in incidencias)
                {
                    i++;
                    table += 
                        "<tr>" +
                            "<td>" + i + "</td>" +
                            "<td>" + res.Pregunta + "</td>" +
                            "<td>" + res.FechaIncumplida.ToString("dd/MM/yyyy") + "</td>" +
                            "<td>" + coments + "</td>" +
                            "<td>" +
                                "<a href='#' class='text-primary mr-3 update_incumplimiento' data-fecha='"+res.FechaIncumplida.ToString("yyyy-MM-dd") + "' " +
                                    "data-id='"+res.Id+ "' data-pregunta='" + res.Pregunta + "'><i class='fas fa-pencil'></i></a>" +
                                "<a href='#' class='text-danger delete_incumplimiento' data-id='" + res.Id + "' data-pregunta='" + res.Pregunta + "'><i class='fas fa-times'></i></a>" +
                            "</td>" +
                        "</tr>";
                }
                return Ok(table);
            }
            return BadRequest();
        }

        [Route("/trasladoExp/eliminaIncumplimiento/{id?}")]
        public async Task<IActionResult> EliminaIncumplimiento(int id)
        {
            int success = await iTraslado.EliminaIncumplimiento(id);
            if (success != -1)
            {
                return Ok(success);
            }
            return BadRequest();
        }

    }
}
