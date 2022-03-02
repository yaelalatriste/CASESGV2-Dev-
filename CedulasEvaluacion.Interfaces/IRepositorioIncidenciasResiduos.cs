using CedulasEvaluacion.Entities.MResiduos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioIncidenciasResiduos
    {
        Task<List<IncidenciasResiduos>> getIncidencias(int id);
        Task<List<IncidenciasResiduos>> getIncidenciasTipo(int id, string tipo);
        Task<int> insertaIncidencia(IncidenciasResiduos incidenciasResiduos);
        Task<int> ActualizaIncidencia(IncidenciasResiduos incidenciasResiduos);
        Task<int> EliminaIncidencias(int id);
        Task<int> EliminaTodaIncidencia(int id, string tipo);
    }
}
