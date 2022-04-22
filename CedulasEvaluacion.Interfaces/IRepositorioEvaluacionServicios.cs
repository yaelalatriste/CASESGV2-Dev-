using System.Collections.Generic;
using System;
using System.Text;
using CedulasEvaluacion.Entities.Vistas;
using System.Threading.Tasks;
using CedulasEvaluacion.Entities.MCedula;
using CedulasEvaluacion.Entities.Models;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioEvaluacionServicios
    {
        Task<int> VerificaCedula(int servicio, int anio, string mes, int inmueble);
        Task<List<VCedulas>> GetCedulasEvaluacion(int servicio, int user);
        Task<int> insertaCedula(CedulaEvaluacion cedula);
        Task<string> GetFolioCedula(int servicio);
        Task<CedulaEvaluacion> CedulaById(int id);
        Task<int> GuardaRespuestas(List<RespuestasEncuesta> respuestasEncuestas);
        Task<List<RespuestasEncuesta>> obtieneRespuestas(int id);
        Task<int> enviaRespuestas(int servicio, int cedula);
        Task<int> apruebaRechazaCedula(CedulaEvaluacion cedula);
        Task<int> capturaHistorial(HistorialCedulas historialCedulas);
        Task<List<HistorialCedulas>> getHistorial(object id);
        Task<int> EliminaCedula(int id);
    }
}
