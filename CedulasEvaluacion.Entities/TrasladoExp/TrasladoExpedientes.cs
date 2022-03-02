using CedulasEvaluacion.Entities.MFacturas;
using CedulasEvaluacion.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.TrasladoExp
{
    public partial class TrasladoExpedientes
    {
        public int Id { get; set; }
        public int ServicioId { get; set; }
        public int UsuarioId { get; set; }
        public string AreaEvaluada { get; set; }
        public int Maniobras { get; set; }
        public int Levantamiento { get; set; }
        public string Folio { get; set; }
        public string Mes { get; set; }
        public int Anio { get; set; }
        public decimal? Calificacion { get; set; }
        public decimal? PenaCalificacion { get; set; }
        public decimal TotalBajoDemanda { get; set; }
        public string Estatus { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public DateTime? FechaEliminacion { get; set; }

        public virtual List<Facturas> facturas { get; set; }
        public virtual Inmueble inmuebles { get; set; }
        public virtual Usuarios usuarios { get; set; }
        public List<RespuestasEncuesta> RespuestasEncuesta { get; set; }
        public List<IncidenciasTraslado> incidenciasP1 { get; set; }
        public List<IncidenciasTraslado> incidenciasP2 { get; set; }
        public List<IncidenciasTraslado> incidenciasP3 { get; set; }
        public List<IncidenciasTraslado> incidenciasP4 { get; set; }
        public List<IncidenciasTraslado> incidenciasP5 { get; set; }
        public List<IncidenciasTraslado> incidenciasP6 { get; set; }
        public virtual List<Entregables> iEntregables { get; set; }
        public List<HistorialCedulas> historialCedulas { get; set; }
        public List<HistorialEntregables> historialEntregables { get; set; }
        public decimal TotalMontoFactura { get; set; }
    }
}
