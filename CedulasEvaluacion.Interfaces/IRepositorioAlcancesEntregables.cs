using CedulasEvaluacion.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioAlcancesEntregables
    {
        Task<List<Entregables>> getEntregables(int cedula);
        Task<int> adjuntaEntregable(Entregables entregables);//adjuntaEntregable
        Task<int> eliminaEntregable(Entregables entregable);
        Task<int> buscaEntregable(int id, string tipo);
        Task<int> capturaHistorial(HistorialEntregables historialEntregables);
        Task<List<HistorialEntregables>> getHistorialEntregables(int id, int servicioId);
        Task<int> apruebaRechazaAlcance(Entregables entregables);
    }
}
