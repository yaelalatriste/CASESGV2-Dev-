using CedulasEvaluacion.Entities.MCelular;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.Vistas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioIncidencias
    {
        Task<List<CatalogoIncidencias>> getTiposIncidencias();

        Task<List<CatalogoIncidencias>> getNombresByTipos(string tipo);

        Task<int> guardaIncidencia(IncidenciasLimpieza incidencia);

        Task<List<VIncidenciasLimpieza>> getIncidencias(int cedulaId);

        //Task<List<VIncidenciasLimpieza>> getIncidenciasEquipo(int cedulaId);

        Task<IncidenciasLimpieza> getIncidenciaBeforeUpdate(int id);

        Task<int> updateIncidencia(IncidenciasLimpieza incidencia);

        Task<int> deleteIncidencia(int id);

        /******************** EQUIPOS *********************/
        Task<List<VIncidenciasLimpieza>> getIncidenciasEquipo(int cedulaId);
    }
}
