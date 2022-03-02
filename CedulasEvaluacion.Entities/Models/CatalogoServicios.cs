using System;
using System.Collections.Generic;

namespace CedulasEvaluacion.Entities.Models
{
    public partial class CatalogoServicios
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public DateTime? FechaEliminacion { get; set; }

    }
}
