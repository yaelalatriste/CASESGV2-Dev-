using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.Login;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CedulasEvaluacion.Entities.Vistas;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioLogin
    {
        Task<int> buscaUsuario(string usuario,string password);        
        Task<DatosUsuario> login(string usuario, string password);
        Task<List<VModulosUsuario>> getModulosByUser(int user);
        Task<List<Dashboard>> totalCedulas(int user);
        Task<List<Dashboard>> CedulasEstatus(int user, string estatus);
        Task<List<VCedulas>> ConcentradoCedulas(int user, string estatus);
    }
}
