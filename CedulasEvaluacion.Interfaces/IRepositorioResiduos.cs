using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.MResiduos;
using CedulasEvaluacion.Entities.Vistas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioResiduos
    {
        Task<List<VCedulas>> GetCedulasResiduos(int user);
        Task<int> insertaCedula(Residuos residuos);
        Task<int> VerificaCedula(int anio, string mes,int inmueble);
        Task<Residuos> CedulaById(int id);
        Task<List<RespuestasEncuesta>> obtieneRespuestas(int id);
        Task<int> GuardaRespuestas(List<RespuestasEncuesta> respuestas);
        Task<int> enviaRespuestas(int cedula);
        Task<List<HistorialCedulas>> getHistorialResiduos(object id);
        Task<int> apruebaRechazaCedula(Residuos residuos);
        Task<int> capturaHistorial(HistorialCedulas historialCedulas);
    }
}
