using CedulasEvaluacion.Entities.Models;
using System;
using System.Collections.Generic;

namespace CedulasEvaluacion.Entities.Models
{
    public partial class RespuestasEncuesta
    {
        public int Id { get; set; }
        public int CedulaLimpiezaId { get; set; }
        public int CedulaMensajeriaId { get; set; }
        public int CedulaTrasladoId { get; set; }
        public int CedulaTransporteId { get; set; }
        public int CedulaFumigacionId { get; set; }
        public int CedulaAnalisisId { get; set; }
        public int CedulaAguaId { get; set; }
        public int CedulaCelularId { get; set; }
        public int CedulaResiduosId { get; set; }
        public int CedulaMuebleId { get; set; }
        public int CedulaConvencionalId { get; set; }
        public int Pregunta { get; set; }
        public bool Respuesta { get; set; }
        public string Detalles { get; set; }
        public bool? Penalizable { get; set; }
        public decimal MontoPenalizacion { get; set; }
    }
}
