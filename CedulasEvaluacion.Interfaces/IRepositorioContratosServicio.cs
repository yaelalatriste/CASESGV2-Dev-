using CedulasEvaluacion.Entities.MContratos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioContratosServicio
    {
        Task<List<ContratosServicio>> GetContratosServicios(int servicio);
        Task<ContratosServicio> GetContratoServicioActivo(int servicio);
        Task<ContratosServicio> GetContratoServicioById(int id);
        Task<int> InsertaContrato(ContratosServicio contratosServicio);
    }
}
