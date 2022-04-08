using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.Reportes
{
    public partial class ReporteCedula
    {
        public int Id { get; set; }
        public string Inmueble { get; set; }
        public string Folio { get; set; }
        public string Mes { get; set; }
        public int Anio { get; set; }
        public string Administracion { get; set; }
        public string Servicio { get; set; }
        public string Estatus { get; set; }
        public decimal Calificacion { get; set; }
        public string FechaCreacion { get; set; }
        public string Facturas { get; set; }
        public string MontosFacturas { get; set; }
    }
}
