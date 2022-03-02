using CedulasEvaluacion.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CASESGCedulasEvaluacion.Entities.Vistas
{
    public partial class DetalleCedula
    {
        public List<ArchivosCedula> archivosCedulas { get; set; }

        public int Id { get; set; }
        public string Servicio { get; set; }
        public string NoContrato { get; set; }
        public string Inmueble { get; set; }
        public string Folio { get; set; }
        public string Mes { get; set; }
        public int Anio { get; set; }
        public string NumFactura{ get; set; }
        public decimal MontoFactura { get; set; }
        public decimal Calificacion { get; set; }
        public string Estatus{ get; set; }

    }
}
