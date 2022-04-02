using CedulasEvaluacion.Entities.MContratos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioEntregablesContrato
    {
        Task<List<EntregablesContrato>> GetEntregablesCS(int contrato);
        Task<List<EntregablesContrato>> GetEntregableCsById(int id);
        Task<int> InsertaContrato(EntregablesContrato entregable);
    }
}
