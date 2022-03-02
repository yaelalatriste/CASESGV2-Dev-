using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.MTransporte;
using CedulasEvaluacion.Entities.Vistas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioTransporte
    {
        Task<List<VCedulas>> GetCedulasTransporte(int user);
        Task<int> insertaCedula(CedulaTransporte cedulaTransporte);
        Task<int> VerificaCedula(int anio, string mes, int inmueble);
        Task<CedulaTransporte> CedulaById(int id);
        Task<int> GuardaRespuestas(List<RespuestasEncuesta> respuestas);
        Task<List<RespuestasEncuesta>> obtieneRespuestas(int id);
        Task<int> enviaRespuestas(int cedula);
        Task<int> apruebaRechazaCedula(CedulaTransporte cedulaTransporte);
        Task<int> capturaHistorial(HistorialCedulas historialCedulas);
        Task<List<HistorialCedulas>> getHistorialTransporte(object id);
    }
}
