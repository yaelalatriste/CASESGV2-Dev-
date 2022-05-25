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
    public class AccionesController : Controller
    {
        private readonly IRepositorioEntregablesCedula vEntregables;
        private readonly IHostingEnvironment environment;

        public AccionesController(IRepositorioEntregablesCedula eiEntregables, IHostingEnvironment environment)
        {
            this.vEntregables = eiEntregables ?? throw new ArgumentNullException(nameof(eiEntregables));
            this.environment = environment;
        }

        [HttpGet]
        [Route("/view/entregable/{folio?}/{nombre?}")]
        public IActionResult verProyecto(string folio, string nombre)
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

        [HttpGet]
        [Route("/view/actadeRobo/{folio?}/{nombre?}")]
        public IActionResult verActadeRobo(string folio, string nombre)
        {
            string folderName = Directory.GetCurrentDirectory() + "\\Entregables\\" + folio + "\\Actas de Robo";
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

        /*Flujo para los estatus*/
        [HttpGet]
        [Route("/entregables/flujo/cae/{cedula?}/{estatus?}")]
        public async Task<IActionResult> GetFlujoCedulaCAE(int cedula, string estatus)
        {
            int exists = 0;
            exists = await vEntregables.GetFlujoCedulaCAE(cedula, estatus);
            if (exists != -3)
            {
                return Ok(exists);
            }
            return BadRequest();
        }
        
        [HttpGet]
        [Route("/entregables/flujo/car/{cedula?}/{estatus?}")]
        public async Task<IActionResult> GetFlujoCedulaCAR(int cedula, string estatus)
        {
            int exists = 0;
            exists = await vEntregables.GetFlujoCedulaCAR(cedula, estatus);
            if (exists != -3)
            {
                return Ok(exists);
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("/entregables/validaCedula/{cedula?}")]
        public async Task<IActionResult> validaCedulaDAS(int cedula)
        {
            int valida = 0;
            valida = await vEntregables.validaCedulaDAS(cedula);
            if (valida != -1)
            {
                return Ok(valida);
            }
            return BadRequest();
        }
        /*Flujo para los estatus*/
    }
}
