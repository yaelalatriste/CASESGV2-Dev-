using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MFacturas
{
    public partial class DesgloceServicio
    {
        public int Id { get; set; }
        public int Anio { get; set; }
        public string Mes { get; set; }
        public string Tipo { get; set; }
        public string Folio { get; set; }
        public string Estatus { get; set; }
        public string Empresa { get; set; }
        public string EstatusFactura { get; set; }
        public string Serie { get; set; }
        public string Fondo { get; set; }
        public string FolioFactura { get; set; }
        public string Total{ get; set; }
        public string MontoTotal{ get; set; }
        public string TotalPagado { get; set; }
        public string PendientePago { get; set; }
        public string FacturasDGPPT { get; set; }
        public string FacturasProceso { get; set; }
    }
}
