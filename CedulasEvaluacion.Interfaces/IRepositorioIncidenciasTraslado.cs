using CedulasEvaluacion.Entities.TrasladoExp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioIncidenciasTraslado
    {
        Task<int> IncidenciasTraslado(IncidenciasTraslado incidenciasTraslado);
        Task<List<IncidenciasTraslado>> getIncidencias(int id,int pregunta);
        Task<int> EliminaIncumplimiento(int id);
    }
}
