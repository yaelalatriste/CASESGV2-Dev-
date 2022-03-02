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

        [Route("/trasladoExp/incidencia")]
        public async Task<IActionResult> IncidenciasTraslado([FromBody] IncidenciasTraslado incidenciasTraslado)
        {
            int success = await iTraslado.IncidenciasTraslado(incidenciasTraslado);
            if (success != -1)
            {
                return Ok(success);
            }
            return BadRequest();
        }

        [Route("/trasladoExp/getIncidencias/{id?}/{pregunta?}")]
        public async Task<IActionResult> getIncidencias(int id,int pregunta)
        {
            List<IncidenciasTraslado> incidencias = await iTraslado.getIncidencias(id,pregunta);
            if (incidencias != null)
            {
                return Ok(incidencias);
            }
            return BadRequest();
        }

        [Route("/trasladoExp/getIncidenciasTable/{id?}/{pregunta?}")]
        public async Task<IActionResult> getIncidenciasTable(int id,int pregunta)
        {
            List<IncidenciasTraslado> incidencias = await iTraslado.getIncidencias(id,pregunta);
            string table = "";
            int i = 0;
            if (incidencias != null)
            {
                foreach (var res in incidencias)
                {
                    i++;
                    table += 
                        "<tr>" +
                            "<td>" + i + "</td>" +
                            "<td>" + res.Pregunta + "</td>" +
                            "<td>" + res.FechaIncumplida.ToString("dd/MM/yyyy") + "</td>" +
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
