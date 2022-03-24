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
        private readonly IRepositorioEntregables vEntregables;
        private readonly IHostingEnvironment environment;

        public EntregablesLimpiezaController(IRepositorioEntregables iVEntregables, IHostingEnvironment environment)
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
            success = await vEntregables.entregableFactura(entregables);
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
        /*FIN del metodo para obtener las inasistencias*/

        /*Obtiene los tipos de entregables que ya fueron adjuntos*/
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

        [HttpPost]
        [Route("cedLimpieza/eliminaArchivo")]
        public async Task<IActionResult> eliminaArchivo([FromBody] Entregables entregable)
        {
            int success = 0;
            success = await vEntregables.eliminaArchivo(entregable);
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

        /***************************** Limpieza ADICIONALES****************************/
        [HttpPost]
        [Route("/limpieza/insertaCapacitacion")]
        public async Task<IActionResult> insertaCapacitacion([FromBody]RespuestasAdicionales respuestasAdicionales)
        {
            int success = 0;
            success = await vEntregables.insertaCapacitacion(respuestasAdicionales);
            if (success != -1)
            {
                return Ok(success);
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("/cedLimpieza/adjuntaEntregableBD")]
        public async Task<IActionResult> adjuntaEntregableBD([FromForm] RespuestasAdicionales respuestasAdicionales)
        {
            int success = 0;
            success = await vEntregables.insertaEntregablesBD(respuestasAdicionales);
            if (success != -1)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("cedLimpieza/getEntregablesBD/{id?}")]
        public async Task<IActionResult> getEntregablesBDLimpieza(int id)
         {
            List<RespuestasAdicionales> entregables = null;
            entregables = await vEntregables.getEntregablesBDLimpieza(id);
            string table = "";
            if (entregables != null)
            {
                foreach (var entregable in entregables)
                {
                    table += "<tr>" +
                    "<td>" + entregable.NombreArchivo + "</td>" +
                    "<td>" + entregable.FechaEntrega.ToString("dd-MM-yyyy") + "</td>" +
                    "<td>" + entregable.FechaLimite.ToString("dd-MM-yyyy") + "</td>" +
                    "<td>" + (entregable.Penalizable == true ?"Si":"No") + "</td>" +
                    "<td>" + String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0:C}", entregable.MontoPenalizacion) + "</td>" +
                    "<td>" +
                        "<a href='#' class='text-center mr-2 view_file' data-id='" + entregable.Id + "' data-file='" + entregable.NombreArchivo +"'>" +
                        "<i class='fas fa-eye text-success'></i></a>" +
                        "<a href='#' class='text-center mr-2 update_files' data-id='" + entregable.Id + "' data-coments='" + entregable.Comentarios + "' " +
                                     "data-prioridad ='" + entregable.Prioridad + "' " +
                                     "data-file='" + entregable.NombreArchivo + "' data-entrega = '"+ entregable.FechaEntrega.ToString("yyyy-MM-dd") + 
                                     "' data-limite='"+ entregable.FechaLimite.ToString("yyyy-MM-dd") + "'><i class='fas fa-edit text-primary'></i></a>" +
                        "<a href='#' class='text-center mr-2 delete_files' data-id='" + entregable.Id + "'><i class='fas fa-times text-danger'></i></a>" +
                    "</td>" +
                    "</tr>";
                }
                return Ok(table);
            }
            return NoContent();
        }

        [HttpGet]
        [Route("cedLimpieza/getCapacitacionLimpieza/{id?}")]
        public async Task<IActionResult> getCapacitacionLimpieza(int id)
        {
            RespuestasAdicionales capacitacion = null;
            capacitacion = await vEntregables.getCapacitacionLimpieza(id);

            if (capacitacion != null)
            {
                return Ok(capacitacion);
            }
            return NoContent();
        }

        [HttpPost]
        [Route("cedLimpieza/eliminaEntregableBD")]
        public async Task<IActionResult> eliminaEntregableBD([FromBody] RespuestasAdicionales entregable)
        {
            int success = 0;
            success = await vEntregables.eliminaEntregableBD(entregable);
            if (success != -1)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("cedLimpieza/eliminaTodoEntregableBD")]
        public async Task<IActionResult> eliminaTodoEntregableBD([FromBody] RespuestasAdicionales entregable)
        {
            string folio = entregable.Folio;
            List<RespuestasAdicionales> entregables = null;
            int success = 0;

            entregables = await vEntregables.getEntregablesBDLimpieza(entregable.Id);
            foreach (var remove in entregables)
            {
                remove.Folio = folio;
                //success = await vEntregables.eliminaEntregableBD(remove);
                if (success == -1)
                {
                    return NotFound();
                }
            }
            success = await vEntregables.calculaCalificacionEntregableBD(entregable.Id);
            return Ok();
        }


        /***************************** FIN Limpieza ADICIONALES****************************/
    }
}
