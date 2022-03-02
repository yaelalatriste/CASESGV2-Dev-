using CedulasEvaluacion.Entities.MFacturas;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.Models
{
    public partial class ActaEntregaRecepcion
    {
        public int Id { get; set; }
        public string Mes { get; set; }
        public string Folio { get; set; }
        public int Anio { get; set; }
        public string Contrato { get; set; }
        public string Representante { get; set; }
        public string Servicio { get; set; }
        public string Inmueble { get; set; }
        public string InmuebleC { get; set; }
        public string TipoInmueble { get; set; }
        public string Direccion { get; set; }
        public string Estado { get; set; }
        public string Administrador { get; set; }
        public string Reviso { get; set; }
        public string Elaboro { get; set; }
        public string Prestador { get; set; }
        public string FolioFactura { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal Total { get; set; }
        public decimal Cantidad { get; set; }
        public DateTime FechaTimbrado { get; set; }
        public virtual List<Facturas> facturas { get; set; }

    }
}
