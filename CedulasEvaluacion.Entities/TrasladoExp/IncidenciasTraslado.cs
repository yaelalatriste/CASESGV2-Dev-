using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.TrasladoExp
{
    public partial class IncidenciasTraslado
    {
        public int Id { get; set; }
        public int CedulaTrasladoId { get; set; }
        public int Pregunta { get; set; }
        public DateTime FechaIncumplida { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public DateTime FechaEliminacion { get; set; }
    }
}
