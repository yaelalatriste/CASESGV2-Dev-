using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Controllers
{
    public class EntregablesResiduosController : Controller
    {
        private readonly IRepositorioEntregablesCedula eResiduos;
        private readonly IHostingEnvironment environment;

        public EntregablesResiduosController(IRepositorioEntregablesCedula evResiduos, IHostingEnvironment environment)
        {
            this.eResiduos = evResiduos ?? throw new ArgumentNullException(nameof(evResiduos));
            this.environment = environment;
        }

        /*Metodo para adjuntar los entregables*/
        [HttpPost]
        [Route("/residuos/adjuntaEntregable")]
        public async Task<IActionResult> adjuntaEntregable([FromForm] Entregables entregables)
        {
            int success = 0;
            success = await eResiduos.adjuntaEntregable(entregables);
            if (success != 0)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
        /*Fin del Metodo para adjuntar los entregables*/

        /*Metodo para obtener las inasistencias*/
        [HttpGet]
        [Route("/residuos/getEntregables/{id?}")]
        public async Task<IActionResult> getEntregablesResiduos(int id)
        {
            List<Entregables> entregables = null;
            entregables = await eResiduos.getEntregables(id);
            string table = "";
            string tipo = "";
            if (entregables != null)
            {
                foreach (var entregable in entregables)
                {
                    if (entregable.Tipo.Equals("ActaER"))
                    {
                        tipo = "Acta Entrega - Recepción";
                    }
                    else if (entregable.Tipo.Equals("SAT"))
                    {
                        tipo = "Validación del SAT";
                    }
                    else if (entregable.Tipo.Equals("NotaCredito"))
                    {
                        tipo = "Nota de Crédito";
                    }
                    else
                    {
                        tipo = entregable.Tipo;
                    }
                    table += "<tr>" +
                    "<td>" + tipo + "</td>" +
                    "<td>" + entregable.NombreArchivo + "</td>" +
                    "<td>" + entregable.FechaCreacion.ToString("yyyy-MM-dd") + "</td>" +
                    "<td>" +
                        "<a href='#' class='text-center mr-2 view_file' data-id='" + entregable.Id + "' data-file='" + entregable.NombreArchivo + "' data-tipo ='" + tipo + "'>" +
                        "<i class='fas fa-eye text-success'></i></a>" +
                        "<a href='#' class='text-center mr-2 update_files' data-id='" + entregable.Id + "' data-coments='" + entregable.Comentarios + "' data-file='" + entregable.NombreArchivo + "'" +
                            "data-tipo='" + entregable.Tipo + "'><i class='fas fa-edit text-primary'></i></a>" +
                        "<a href='#' class='text-center mr-2 delete_files' data-id='" + entregable.Id + "' data-tipo='" + entregable.Tipo + "'><i class='fas fa-times text-danger'></i></a>" +
                    "</td>" +
                    "</tr>";
                }
                return Ok(table);
            }
            return NoContent();
        }
        /*FIN del metodo para obtener las inasistencias*/


        /*Obtiene los tipos de entregables que ya fueron adjuntos*/
        [HttpGet]
        [Route("/residuos/getListadoEntregables/{id?}")]
        public async Task<IActionResult> getListadoEntregablesResiduos(int id)
        {
            List<Entregables> entregables = null;
            entregables = await eResiduos.getEntregables(id);
            if (entregables != null)
            {
                return Ok(entregables);
            }
            return NoContent();
        }

        [HttpGet]
        [Route("/residuos/verArchivo/{folio?}/{nombre?}")]
        public IActionResult archivoProyecto(string folio, string nombre)
        {
            string folderName = Directory.GetCurrentDirectory() + "\\Entregables\\" + folio + "\\";
            string webRootPath = environment.ContentRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            string pathArchivo = Path.Combine(newPath, nombre);

            if (System.IO.File.Exists(pathArchivo))
            {
                Stream stream = System.IO.File.Open(pathArchivo, FileMode.Open);

                return File(stream, "application/pdf");
            }
            return NotFound();
        }

        [HttpPost]
        [Route("/residuos/eliminaArchivo")]
        public async Task<IActionResult> eliminaArchivo([FromBody] Entregables entregable)
        {
            int success = 0;
            success = await eResiduos.eliminaEntregable(entregable);
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
        [Route("/residuos/buscaEntregable/{id?}/{tipo?}")]
        public async Task<IActionResult> buscaEntregable(int id, string tipo)
        {
            int exists = 0;
            exists = await eResiduos.buscaEntregable(id, tipo);
            if (exists != -1)
            {
                return Ok(exists);
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("/residuos/Entregables/autoRecha")]
        public async Task<IActionResult> aprovacionRechazoCedula([FromBody] Entregables entregables)
        {
            int success = await eResiduos.apruebaRechazaEntregable(entregables);
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
        [Route("/residuos/entregables/historialEntregable")]
        public async Task<IActionResult> historialEntregable([FromBody] HistorialEntregables historialEntregables)
        {
            int success = 0;
            success = await eResiduos.capturaHistorial(historialEntregables);
            if (success != 0)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
    }
}
