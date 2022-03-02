using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MFacturas
{
    public partial class Facturas
    {
        public int Id { get; set; }
        public int CedulaId { get; set; }
        public int ServicioId { get; set; }
        public string Descripcion { get; set; }
        public IFormFile Xml { get; set; }
        public Comprobante comprobante { get; set; }
        public Emisor emisor { get; set; }
        public List<Concepto> concepto { get; set; }
        public TimbreFiscal timbreFiscal { get; set; }
        public DatosTotales datosTotales { get; set; }
        public DatosExtra datosExtra { get; set; }
        public Traslado traslado { get; set; }
        public Receptor receptor{ get; set; }
        public Retencion retencion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public DateTime FechaEliminacion { get; set; }
    }
}
