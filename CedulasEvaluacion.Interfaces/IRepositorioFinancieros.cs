using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.Vistas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioFinancieros
    {
        Task<List<Dashboard>> GetCedulasFinancieros();
        Task<List<Dashboard>> GetDetalleServicio(string servicio);
    }
}
