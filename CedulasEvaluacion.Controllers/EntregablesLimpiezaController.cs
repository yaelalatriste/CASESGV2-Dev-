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
        private readonly IRepositorioAlcancesEntregables vAlcances;
        private readonly IHostingEnvironment environment;

        public EntregablesLimpiezaController(IRepositorioEntregablesCedula iVEntregables, IRepositorioAlcancesEntregables viAlcances, IHostingEnvironment environment)
        {
            this.vEntregables = iVEntregables ?? throw new ArgumentNullException(nameof(iVEntregables));
            this.vAlcances = viAlcances ?? throw new ArgumentNullException(nameof(viAlcances));
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

        [HttpPost]
        [Route("/limpieza/adjuntaAlcance")]
        public async Task<IActionResult> adjuntaAlcance([FromForm] Entregables entregables)
        {
            int success = 0;
            success = await vAlcances.adjuntaEntregable(entregables);
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

        [HttpGet]
        [Route("/limpieza/getAlcances/{id?}")]
        public async Task<IActionResult> getAlcancesEntregables(int id)
        {
            List<Entregables> entregables = null;
            entregables = await vAlcances.getEntregables(id);
            string table = "";
            int i = 0;
            string tipo = "";
            if (entregables != null)
            {
                foreach (var entregable in entregables)
                {
                    i++;
                    if (entregable.Tipo.Equals("AlcanceMO"))
                    {
                        tipo = "Alcance a Memorandum";
                    }
                    else if (entregable.Tipo.Equals("AlcanceAER"))
                    {
                        tipo = "Alcance a Acta Entrega Recepción";
                    }
                    else
                    {
                        tipo = entregable.Tipo;
                    }
                    table += "<tr>" +
                    "<td>" + i + "</td>" +
                    "<td>" + tipo + "</td>" +
                    "<td>" + entregable.NombreArchivo + "</td>" +
                    (entregable.Estatus.Equals("En Revisión") ? ("<td class='text-primary font-weight-bold'>" + entregable.Estatus + "</td>") : (entregable.Estatus.Equals("Autorizado") ? ("<td class='text-success font-weight-bold'>" + entregable.Estatus + "</td>") : ("<td class='text-danger font-weight-bold'>" + entregable.Estatus + "</td>"))) +
                    "<td>" + entregable.FechaCreacion.ToString("dd/MM/yyyy") + "</td>" +
                    "<td class='text-center'>" + (entregable.FechaActualizacion.ToString("dd/MM/yyyy").Equals("01/01/0001") ? "-":entregable.FechaActualizacion.ToString("dd/MM/yyyy")) + "</td>" +
                    "<td>" +
                        "<a href='#' class='text-center mr-2 view_alcance' data-id='" + entregable.Id + "' data-file='" + entregable.NombreArchivo + "' data-tipo ='" + entregable.Tipo + "'>" +
                        "<i class='fas fa-eye text-success'></i></a>" +
                        "<a href='#' class='text-center mr-2 update_alcance' data-id='" + entregable.Id + "' data-coments='" + entregable.Comentarios + "' data-file='" + entregable.NombreArchivo + "'" +
                            "data-tipo='" + entregable.Tipo + "'><i class='fas fa-edit text-primary'></i></a>" +
                        (entregable.Estatus.Equals("En Revisión") ? ("<a href='#' class='text-center mr-2 autorizar_alcance' data-id='" + entregable.Id + "' data-toggle='tooltip' title='Autorizar Alcance' "+
                        " data-tipo = '" + entregable.Tipo + "'><i class='fas fa-check text-success'></i></a>" +
                        "<a href='#' class='text-center mr-2 rechazar_alcance' data-id='" + entregable.Id + "' data-toggle='tooltip' title='Rechazar Alcance' "+
                            " data-tipo = '" + entregable.Tipo + "'><i class='fas fa-times text-danger'></i></a>"):(""))+
                         (entregable.Estatus.Equals("Autorizado") ? ("<a href='#' class='text-center mr-2 rechazar_alcance' data-id='" + entregable.Id + "' data-toggle='tooltip' title='Rechazar Alcance' " +
                            " data-tipo = '" + entregable.Tipo + "'><i class='fas fa-times text-danger'></i></a>") : ("")) +
                         (entregable.Estatus.Equals("Rechazado") ? ("<a href='#' class='text-center mr-2 autorizar_alcance' data-id='" + entregable.Id + "' data-toggle='tooltip' title='Autorizar Alcance' " +
                        " data-tipo = '" + entregable.Tipo + "'><i class='fas fa-check text-success'></i></a>") : (""))+
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
        public async Task<IActionResult> apruebaRechazaEntregable([FromBody] Entregables entregables)
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

        [HttpPost]
        [Route("/limpieza/alcances/autoRecha")]
        public async Task<IActionResult> apruebaRechazaAlcance([FromBody] Entregables entregables)
        {
            int success = await vAlcances.apruebaRechazaAlcance(entregables);
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
