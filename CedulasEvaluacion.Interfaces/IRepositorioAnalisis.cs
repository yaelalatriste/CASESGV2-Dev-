using CedulasEvaluacion.Entities.MAnalisis;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.Vistas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioAnalisis
    {
        Task<List<VCedulas>> GetCedulasAnalisis(int user);
        Task<int> insertaCedula(CedulaAnalisis cedulaAnalisis);
        Task<int> VerificaCedula(int anio, string mes, int inmueble);
        Task<CedulaAnalisis> CedulaById(int id);
        Task<int> GuardaRespuestas(List<RespuestasEncuesta> respuestas);
        Task<List<RespuestasEncuesta>> obtieneRespuestas(int id);
        Task<int> enviaRespuestas(int cedula);
        Task<int> apruebaRechazaCedula(CedulaAnalisis cedulaAnalisis);
        Task<int> capturaHistorial(HistorialCedulas historialCedulas);
        Task<List<HistorialCedulas>> getHistorialAnalisis(object id);
    }
}
