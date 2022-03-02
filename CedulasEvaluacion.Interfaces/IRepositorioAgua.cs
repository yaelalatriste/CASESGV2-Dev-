using CedulasEvaluacion.Entities.MAgua;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.Vistas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioAgua
    {
        Task<List<VCedulas>> GetCedulasAgua(int user);
        Task<int> insertaCedula(CedulaAgua cedulaAgua);
        Task<int> VerificaCedula(int anio, string mes, int inmueble);
        Task<CedulaAgua> CedulaById(int id);
        Task<int> GuardaRespuestas(List<RespuestasEncuesta> respuestas);
        Task<List<RespuestasEncuesta>> obtieneRespuestas(int id);
        Task<int> enviaRespuestas(int cedula);
        Task<int> apruebaRechazaCedula(CedulaAgua cedulaAgua);
        Task<int> capturaHistorial(HistorialCedulas historialCedulas);
        Task<List<HistorialCedulas>> getHistorialAgua(object id);

    }
}
