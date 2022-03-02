using CedulasEvaluacion.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioOperacionesPerfil
    {
        Task<int> insertarOperacionesPerfil(OperacionesPerfil operacionesPerfil);
        Task<List<OperacionesPerfil>> getOperacionesByPerfil(int perfil);
        Task<int> eliminaOpPerfil(int perfil);
    }
}
