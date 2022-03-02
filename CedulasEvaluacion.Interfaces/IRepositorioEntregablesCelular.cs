using CedulasEvaluacion.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioEntregablesCelular
    {
        Task<List<Entregables>> getEntregables(int cedula);

        Task<int> entregableFactura(Entregables entregables);

        Task<int> eliminaArchivo(Entregables entregable);

        Task<int> buscaEntregable(int id, string tipo);
        Task<List<HistorialEntregables>> getHistorialEntregables(object id);
        Task<int> apruebaRechazaEntregable(Entregables entregables);
        Task<int> capturaHistorial(HistorialEntregables historialEntregables);
    }
}
