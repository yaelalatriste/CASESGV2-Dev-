using CedulasEvaluacion.Entities.MMuebles;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.Vistas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioMuebles
    {
        Task<List<VCedulas>> GetCedulasMuebles(int user);
        Task<int> insertaCedula(CedulaMuebles cedulaMuebles);
        Task<int> VerificaCedula(int anio, string mes, int inmueble);
        Task<CedulaMuebles> CedulaById(int id);
        Task<int> GuardaRespuestas(List<RespuestasEncuesta> respuestas);
        Task<List<RespuestasEncuesta>> obtieneRespuestas(int id);
        Task<int> enviaRespuestas(int cedula);
        Task<int> apruebaRechazaCedula(CedulaMuebles cedulaMuebles);
        Task<int> capturaHistorial(HistorialCedulas historialCedulas);
        Task<List<HistorialCedulas>> getHistorialMuebles(object id);
    }
}
