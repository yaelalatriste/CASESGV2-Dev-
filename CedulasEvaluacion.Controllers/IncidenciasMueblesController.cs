using CedulasEvaluacion.Entities.MMuebles;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    public partial class IncidenciasMueblesController : Controller
    {
        private readonly IRepositorioIncidenciasMuebles iMuebles;
        private readonly IHostingEnvironment environment;

        public IncidenciasMueblesController(IRepositorioIncidenciasMuebles ivMuebles, IHostingEnvironment environment)
        {
            this.iMuebles = ivMuebles?? throw new ArgumentNullException(nameof(ivMuebles));
        }

        [Route("/muebles/getIncidencias/{id?}")]
        public async Task<IActionResult> getIncidencias(int id)
        {
            List<IncidenciasMuebles> incidencias = await iMuebles.GetIncidencias(id);
            if (incidencias != null)
            {
                return Ok(incidencias);
            }
            return BadRequest();
        }

        [Route("/muebles/tablaIncidencias/{id?}/{pregunta?}")]
        public async Task<IActionResult> generaTablaincidencias(int id, int pregunta)
        {
            string tbody = "";
            List<IncidenciasMuebles> incidencias = await iMuebles.GetIncidenciasPregunta(id, pregunta);
            if (incidencias != null)
            {
                int i = 0;
                foreach (var inc in incidencias)
                {
                    tbody +=
                     "<tr>" +
                         "<td>" + (i + 1) + "</td>" +
                         "<td>" + inc.Tipo + "</td>" +
                         "<td>" + inc.FechaSolicitud + "</td>" +
                         "<td>" + inc.FechaRespuesta + "</td>" +
                         "<td>" + inc.Comentarios + "</td>" +
                         "<td>" +
                             "<a href='#' class='text-center mr-2 update_incidencia' data-id='" + inc.Id + "' data-tipo='" + inc.Tipo + "' data-fechareal='" + inc.FechaRespuesta.ToString("yyyy-MM-ddTHH:mm") + "'" +
                             " data-fechaprog='" + inc.FechaSolicitud.ToString("yyyy-MM-ddTHH:mm") + "' data-coment='" + inc.Comentarios + "'>" +
                                 "<i class='fas fa-edit text-primary'></i>" +
                             "</a>" +
                             "<a href='#' class='text-center mr-2 delete_incidencia' data-id='" + inc.Id + "'><i class='fas fa-times text-danger'></i></a>" +
                         "</td>" +
                     "</tr>";
                }
                return Ok(tbody);
            }
            return BadRequest();
        }

        [Route("/muebles/getIncidencias/{id?}/{pregunta}")]
        public async Task<IActionResult> getIncidenciasPregunta(int id, int pregunta)
        {
            List<IncidenciasMuebles> incidencias = await iMuebles.GetIncidenciasPregunta(id,pregunta);
            if (incidencias != null)
            {
                return Ok(incidencias);
            }
            return BadRequest();
        }

        [Route("/muebles/inserta/incidencia")]
        public async Task<IActionResult> insertaIncidencia([FromBody] IncidenciasMuebles incidenciasMuebles)
        {
            int success = await iMuebles.IncidenciasMuebles(incidenciasMuebles);
            if (success != -1)
            {
                return Ok(success);
            }
            return BadRequest();
        }

        [Route("/muebles/actualiza/incidencia")]
        public async Task<IActionResult> ActualizaIncidencia([FromBody] IncidenciasMuebles incidenciasMuebles)
        {
            int success = await iMuebles.ActualizaIncidencia(incidenciasMuebles);
            if (success != -1)
            {
                return Ok(success);
            }
            return BadRequest();
        }
    }
}
