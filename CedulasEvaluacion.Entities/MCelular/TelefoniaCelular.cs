using CedulasEvaluacion.Entities.MFacturas;
using CedulasEvaluacion.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MCelular
{
    public partial class TelefoniaCelular
    {
        public int Id { get; set; }
        public int ServicioId { get; set; }
        public int UsuarioId { get; set; }
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

        public virtual Usuarios usuarios { get; set; }
        public virtual List<Entregables> IEntregables { get; set; }
        public virtual List<Facturas> facturas { get; set; }
        public List<RespuestasEncuesta> RespuestasEncuesta { get; set; }
        public List<IncidenciasCelular> incidenciasCelular { get; set; }
        public List<IncidenciasCelular> altaEntrega { get; set; }
        public List<IncidenciasCelular> altasentrega { get; set; }
        public List<IncidenciasCelular> bajaServicio { get; set; }
        public List<IncidenciasCelular> reactivacion { get; set; }
        public List<IncidenciasCelular> suspension { get; set; }
        public List<IncidenciasCelular> cambioPerfil { get; set; }
        public List<IncidenciasCelular> switcheoCard { get; set; }
        public List<IncidenciasCelular> cambioRegion { get; set; }
        public List<IncidenciasCelular> servicioVozDatos { get; set; }
        public List<IncidenciasCelular> diagnostico { get; set; }
        public List<IncidenciasCelular> reparacion { get; set; }
        public List<HistorialCedulas> historialCedulas { get; set; }

        public List<HistorialEntregables> historialEntregables { get; set; }
        public decimal TotalMontoFactura { get; set; }

    }
}
