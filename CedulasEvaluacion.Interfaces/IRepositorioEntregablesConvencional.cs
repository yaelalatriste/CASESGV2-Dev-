using CedulasEvaluacion.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioEntregablesConvencional
    {
        Task<int> entregableFactura(Entregables entregables);
        Task<List<Entregables>> getEntregables(int id);
        Task<int> eliminaArchivo(Entregables entregable);
        Task<int> buscaEntregable(int id, string tipo);
    }
}
