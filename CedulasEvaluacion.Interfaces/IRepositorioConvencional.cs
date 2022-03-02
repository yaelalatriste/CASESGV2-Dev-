using CedulasEvaluacion.Entities.MConvencional;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.Vistas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioConvencional
    {
        Task<List<VCedulas>> GetCedulasConvencional();
        Task<int> insertaCedula(TelefoniaConvencional telefoniaConvencional);
        Task<int> VerificaCedula(int anio, string mes);
        Task<List<RespuestasEncuesta>> obtieneRespuestas(int id);
        Task<TelefoniaConvencional> CedulaById(int id);
        Task<int> GuardaRespuestas(List<RespuestasEncuesta> respuestas);
        Task<int> enviaRespuestas(int cedula);
        Task<int> apruebaRechazaCedula(TelefoniaConvencional telefoniaConvencional);
        Task<int> capturaHistorial(HistorialCedulas historialCedulas);
        Task<List<HistorialCedulas>> getHistorialConvencional(object id);
    }
}
