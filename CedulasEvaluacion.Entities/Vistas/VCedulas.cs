using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.Vistas
{
    public partial class VCedulas
    {
        public int Id { get; set; }
        public string Abreviacion { get; set; }
        public string Nombre { get; set; }
        public string Destino { get; set; }
        public string Area { get; set; }
        public string Folio { get; set; }
        public string Mes { get; set; }
        public int Anio { get; set; }
        public string NumFactura { get; set; }
        public string Servicio { get; set; }
        public string Estatus { get; set; }

    }
}
