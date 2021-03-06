using CedulasEvaluacion.Entities.MFacturas;
using CedulasEvaluacion.Entities.MIncidencias;
using CedulasEvaluacion.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MCedula
{
    public partial class CedulaEvaluacion
    {
        public int Id { get; set; }
        public int ServicioId { get; set; }
        public int InmuebleId { get; set; }
        public int Maniobras { get; set; }
        public int InmuebleDestinoId { get; set; }
        public int UsuarioId { get; set; }
        public string Folio { get; set; }
        public string Mes { get; set; }
        public int Anio { get; set; }
        public decimal? Calificacion { get; set; }
        public decimal? PenaCalificacion { get; set; }
        public decimal TotalBajoDemanda { get; set; }
        public string Estatus { get; set; }
        public string URL { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public DateTime? FechaEliminacion { get; set; }
        public decimal TotalMontoFactura { get; set; }

        public virtual Inmueble inmuebles { get; set; }
        public virtual Inmueble inmuebleDestino { get; set; }
        public virtual Usuarios usuarios { get; set; }
        public virtual List<Facturas> facturas { get; set; }
        public virtual List<Entregables> iEntregables { get; set; }
        public virtual List<Entregables> iAlcances { get; set; }
        public List<RespuestasEncuesta> RespuestasEncuesta { get; set; }
        public List<HistorialCedulas> historialCedulas { get; set; }
        public List<HistorialEntregables> historialEntregables { get; set; }
        public ModelsIncidencias incidencia { get; set; }
        public ModelsIncidencias incidencias { get; set; }
    }
}
