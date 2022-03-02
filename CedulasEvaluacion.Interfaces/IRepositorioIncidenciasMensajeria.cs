using CedulasEvaluacion.Entities.MMensajeria;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioIncidenciasMensajeria
    {
        Task<int> IncidenciasExcel(IncidenciasMensajeria incidenciasMensajeria);
        Task<int> IncidenciasMensajeria(IncidenciasMensajeria incidenciasMensajeria);
        Task<int> IncidenciaRobada(IncidenciasMensajeria incidenciasMensajeria);
        Task<List<IncidenciasMensajeria>> getIncidenciasMensajeria(int id);
        Task<List<IncidenciasMensajeria>> getIncidenciasByTipoMensajeria(int id,string tipo);
        Task<int> ActualizaIncidencia(IncidenciasMensajeria incidenciasMensajeria);
        Task<int> ActualizaRobada(IncidenciasMensajeria incidenciasMensajeria);
        Task<int> EliminaIncidencia(int id);
        Task<List<IncidenciasMensajeria>> TotalIncidencias(int id);
        Task<int> EliminaTodaIncidencia(int id,string tipo);
        Task<int> IncidenciasTipo(int id, string tipo);
    }
}
