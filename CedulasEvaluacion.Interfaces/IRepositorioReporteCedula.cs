using CedulasEvaluacion.Entities.MMensajeria;
using CedulasEvaluacion.Entities.Reportes;
using CedulasEvaluacion.Entities.Vistas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioReporteCedula
    {
        Task<IEnumerable<ReporteCedula>> getReporteMensajeria(int id);
        Task<IEnumerable<ReporteFinancieros>> getReporteFinancierosLimpieza();
    }
}
