﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MFacturas
{
    public partial class DesgloceServicio
    {
        public int Id { get; set; }
        public string Mes { get; set; }
        public string Fondo { get; set; }
        public string Tipo { get; set; }
        public string Folio { get; set; }
        public string Estatus { get; set; }
        public string Empresa { get; set; }
        public string EstatusFactura { get; set; }
        public string Serie { get; set; }
        public string FolioFactura { get; set; }
        public int FacturasPendientes{ get; set; }
        public string TotalPendiente { get; set; }
        public int FacturasPagadas{ get; set; }
        public string TotalPagado{ get; set; }
        public int FacturasDGPPT { get; set; }
        public string TotalDGPPT{ get; set; }
        public int TotalFacturas { get; set; }
        public decimal TotalFinal { get; set; }
        
    }
}
