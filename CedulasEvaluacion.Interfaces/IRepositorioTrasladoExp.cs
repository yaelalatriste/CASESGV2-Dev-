using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.TrasladoExp;
using CedulasEvaluacion.Entities.Vistas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioTrasladoExp
    {
        Task<List<VCedulas>> getCedulasTraslado();
        Task<int> VerificaCedula(int anio, string mes);
        Task<int> insertaCedula(TrasladoExpedientes trasladoExpedientes);
        Task<TrasladoExpedientes> CedulaById(int id);
        Task<List<RespuestasEncuesta>> obtieneRespuestas(int id);
        Task<int> GuardaRespuestas(List<RespuestasEncuesta> respuestas);
        Task<int> enviaRespuestas(int cedula);
        Task<int> capturaHistorial(HistorialCedulas historialCedulas);
        Task<int> apruebaRechazaCedula(TrasladoExpedientes trasladoExpedientes);
        Task<List<HistorialCedulas>> getHistorialTraslado(object id);
    }
}
