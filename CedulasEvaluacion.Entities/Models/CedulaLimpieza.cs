using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.MFacturas;
using CedulasEvaluacion.Entities.Vistas;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.Models
{
    public partial class CedulaLimpieza
    {
        public int Id { get; set; }
        public int ServicioId { get; set; }
        public int InmuebleId { get; set; }
        public int UsuarioId{ get; set; }
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

        public virtual Inmueble inmuebles { get; set; }
        public virtual Usuarios usuarios { get; set; }
        public virtual List<Facturas> facturas { get; set; }
        public virtual List<VIncidenciasLimpieza> incidencias { get; set; }
        public virtual List<VIncidenciasLimpieza> iEquipos { get; set; }
        public virtual List<Entregables> iEntregables { get; set; }
        public virtual List<RespuestasAdicionales> eAdicionales { get; set; }
        public virtual RespuestasAdicionales capacitacion { get; set; }
        public List<RespuestasEncuesta> RespuestasEncuesta { get; set; }
        public List<HistorialCedulas> historialCedulas { get; set; }
        public List<HistorialEntregables> historialEntregables{ get; set; }
        public decimal TotalMontoFactura { get; set; }
    }
}
