using CedulasEvaluacion.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioEntregablesTrasladoExp
    {
        Task<List<Entregables>> getEntregables(int cedula);

        Task<int> entregableFactura(Entregables entregables);

        Task<int> eliminaArchivo(Entregables entregable);

        Task<int> buscaEntregable(int id, string tipo);
    }
}
