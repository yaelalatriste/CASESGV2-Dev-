using CedulasEvaluacion.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioCedulasEvaluacion
    {
        Task<List<Usuarios>> GetUsuariosByAdministracion(int user);
        Task<int> GetVerificaFirmantes(string tipo, int user);
    }
}
