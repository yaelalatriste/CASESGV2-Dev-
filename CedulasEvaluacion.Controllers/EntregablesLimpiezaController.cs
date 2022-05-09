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
    public class EntregablesLimpiezaController : Controller
    {
        private readonly IRepositorioEntregablesCedula vEntregables;
        private readonly IHostingEnvironment environment;

        public EntregablesLimpiezaController(IRepositorioEntregablesCedula iVEntregables, IHostingEnvironment environment)
        {
            this.vEntregables = iVEntregables ?? throw new ArgumentNullException(nameof(iVEntregables));
            this.environment = environment;
        }

        /***************************** Limpieza ****************************/
        /*Metodo para adjuntar los entregables*/
        [HttpPost]
        [Route("/cedLimpieza/adjuntaEntregable")]
        public async Task<IActionResult> adjuntaEntregable([FromForm] Entregables entregables)
        {
            int success = 0;
            success = await vEntregables.adjuntaEntregable(entregables);
            if (success != 0)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        //Obtiene el body de la tabla para los Entregables
        [HttpGet]
        [Route("cedLimpieza/getEntregables/{id?}")]
        public async Task<IActionResult> getEntregablesLimpieza(int id)
        {
            List<Entregables> entregables = null;
            entregables = await vEntregables.getEntregables(id);
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
                    else if (entregable.Tipo.Equals("Cedula_Firmada"))
                    {
                        tipo = "Cédula Firmada";
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

        //obtiene el listado de entregables  
        [HttpGet]
        [Route("/cedLimpieza/getListadoEntregablesLimpieza/{id?}")]
        public async Task<IActionResult> getListadoEntregablesLimpieza(int id)
        {
            List<Entregables> entregables = null;
            entregables = await vEntregables.getEntregables(id);
            if (entregables != null)
            {
                return Ok(entregables);
            }
            return NoContent();
        }

        [HttpPost]
        [Route("/limpieza/entregables/historialEntregable")]
        public async Task<IActionResult> historialLimpieza([FromBody] HistorialEntregables historialEntregables)
        {
            int success = 0;
            success = await vEntregables.capturaHistorial(historialEntregables);
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
        [Route("/limpieza/Entregables/autoRecha")]
        public async Task<IActionResult> aprovacionRechazoCedula([FromBody] Entregables entregables)
        {
            int success = await vEntregables.apruebaRechazaEntregable(entregables);
            if (success != 0)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        //muestra el archivo en la página HTML
        [HttpGet]
        [Route("cedLimpieza/verArchivo/{folio?}/{nombre?}")]
        public IActionResult archivoProyecto(string folio, string nombre)
        {
            string folderName = Directory.GetCurrentDirectory() + "\\Entregables\\" + folio + "\\";
            string webRootPath = environment.ContentRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            string pathArchivo = Path.Combine(newPath, nombre);
            if (nombre.Contains(".pdf")) {
                if (System.IO.File.Exists(pathArchivo))
                {
                    Stream stream = System.IO.File.Open(pathArchivo, FileMode.Open);

                    return File(stream, "application/pdf");
                }
            }
            else
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(pathArchivo);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, nombre);
            }
            return NotFound();
        }

        //Elimina el Archivo
        [HttpPost]
        [Route("cedLimpieza/eliminaArchivo")]
        public async Task<IActionResult> eliminaArchivo([FromBody] Entregables entregable)
        {
            int success = 0;
            success = await vEntregables.eliminaEntregable(entregable);
            if (success != -1)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        //Identifica el entregable para que no se repitan
        [HttpGet]
        [Route("cedLimpieza/buscaEntregable/{id?}/{tipo?}")]
        public async Task<IActionResult> buscaEntregable(int id, string tipo)
        {
            int exists = 0;
            exists = await vEntregables.buscaEntregable(id, tipo);
            if(exists != -1)
            {
                return Ok(exists);
            }
            return BadRequest();
        }



        /***************************** FIN Limpieza ****************************/
    }
}
