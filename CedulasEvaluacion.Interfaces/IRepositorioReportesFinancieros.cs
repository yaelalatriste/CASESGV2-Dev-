using CedulasEvaluacion.Entities.Reportes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioReportesFinancieros
    {
        Task<List<ReporteCedula>> GetCedulasFinancieros(string mes, int anio);
        Task<List<ReporteCedula>> GetReportePagos(string mes, int anio);
        Task<List<ReporteCedula>> GetReporteServiciosFacturas(int servicio, string mes);
    }
}
