using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MCedula
{
    public partial class FirmantesServicio
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int InmuebleId { get; set; }
        public int ServicioId { get; set; }
        public string Tipo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public DateTime FechaEliminacion { get; set; }
    }
}
