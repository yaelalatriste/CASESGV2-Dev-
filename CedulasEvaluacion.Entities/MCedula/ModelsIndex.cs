using CedulasEvaluacion.Entities.Vistas;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MCedula
{
    public class ModelsIndex
    {
        public List<VCedulasEvaluacion> cedulasEstatus { get; set; }
        public List<VCedulasEvaluacion> cedulasMes { get; set; }
        public List<VCedulas> cedulas { get; set; }
        public int ServicioId { get; set; }
        public string Estatus { get; set; }
        public int InmuebleId { get; set; }
        public string Mes { get; set; }
        public string Url { get; set; }
    }
}
