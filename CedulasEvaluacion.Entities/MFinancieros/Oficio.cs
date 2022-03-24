using CedulasEvaluacion.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MFinancieros
{
    public partial class Oficio
    {
        public int Id { get; set; }
        public int ServicioId { get; set; }
        public int Anio { get; set; }
        public int NumeroOficio { get; set; }
        public string Servicio { get; set; }
        public string Estatus { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public DateTime FechaEliminacion { get; set; }
        public List<DetalleCedula> cedulasOficio { get; set; }
        public List<DetalleCedula> detalleCedulas{ get; set; }
    }
}
