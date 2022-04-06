using CedulasEvaluacion.Entities.MMensajeria;
using CedulasEvaluacion.Entities.Vistas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioReporteCedula
    {
        Task<IEnumerable<VCedulas>> getCedulasMensajeria();
    }
}
