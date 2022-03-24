using CedulasEvaluacion.Entities.MCatalogoServicios;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioCatalogoServicios
    {
        //Catalogo de Servicios
        Task<List<CatalogoServicios>> GetCatalogoServicios();
    }
}
