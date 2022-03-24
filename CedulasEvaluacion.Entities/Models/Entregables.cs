using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.Models
{
    public partial class Entregables
    {
        public int Id { get; set; }
        public int CedulaLimpiezaId { get; set; }
        public int CedulaFumigacionId { get; set; }
        public int CedulaTransporteId { get; set; }
        public int CedulaAnalisisId { get; set; }
        public int CedulaAguaId { get; set; }
        public int CedulaMensajeriaId { get; set; }
        public int CedulaTrasladoId { get; set; }
        public int CedulaCelularId { get; set; }
        public int CedulaConvencionalId { get; set; }
        public int CedulaResiduosId { get; set; }
        public int CedulaMuebleId { get; set; }
        public IFormFile Archivo { get; set; }
        public string NombreArchivo { get; set; }
        public string Estatus { get; set; }
        public string Tipo { get; set; }
        public string Folio { get; set; }
        public int Tamanio { get; set; }
        public int DiasAtraso { get; set; }
        public string Comentarios{ get; set; }
        public DateTime FechaCompromiso { get; set; }
        public DateTime FechaPresentacion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaActualizacion { get; set; }
        public DateTime FechaEliminacion { get; set; }
    }
}
