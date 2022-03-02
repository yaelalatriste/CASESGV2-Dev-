using CedulasEvaluacion.Entities.MFumigacion;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.Vistas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioFumigacion
    {
        Task<List<VCedulas>> GetCedulasFumigacion(int user);
        Task<int> insertaCedula(CedulaFumigacion cedulaFumigacion);
        Task<int> VerificaCedula(int anio, string mes, int inmueble);
        Task<CedulaFumigacion> CedulaById(int id);
        Task<int> GuardaRespuestas(List<RespuestasEncuesta> respuestas);
        Task<List<RespuestasEncuesta>> obtieneRespuestas(int id);
        Task<int> enviaRespuestas(int cedula);
        Task<int> apruebaRechazaCedula(CedulaFumigacion cedulaFumigacion);
        Task<int> capturaHistorial(HistorialCedulas historialCedulas);
        Task<List<HistorialCedulas>> getHistorialFumigacion(object id);
    }
}
