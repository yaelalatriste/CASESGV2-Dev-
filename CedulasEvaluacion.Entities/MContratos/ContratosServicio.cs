using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MContratos
{
    public partial class ContratosServicio
    {
        public int Id { get; set; }
        public int ServicioId { get; set; }
        public int UsuarioId { get; set; }
        public string NumeroContrato { get; set; }
        public string Empresa { get; set; }
        public string Representante { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public DateTime FechaEliminacion { get; set; }

    }
}
