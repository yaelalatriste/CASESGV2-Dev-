using CedulasEvaluacion.Entities.MCelular;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.Vistas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioCelular
    {
        Task<List<VCedulas>> GetCedulasCelular(int user);
        Task<int> insertaCedula(TelefoniaCelular telefoniaCelular);
        Task<int> VerificaCedula(int anio, string mes);
        Task<TelefoniaCelular> CedulaById(int id);
        Task<List<RespuestasEncuesta>> obtieneRespuestas(int id);
        Task<int> GuardaRespuestas(List<RespuestasEncuesta> respuestasEncuestas);
        Task<int> apruebaRechazaCedula(TelefoniaCelular telefoniaCelular);
        Task<int> enviaRespuestas(int cedula);
        Task<int> capturaHistorial(HistorialCedulas historialCedulas);
        Task<List<HistorialCedulas>> getHistorialCelular(object id);
    }
}
