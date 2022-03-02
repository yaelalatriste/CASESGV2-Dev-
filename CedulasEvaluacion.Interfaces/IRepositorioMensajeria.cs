using CedulasEvaluacion.Entities.MMensajeria;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.Vistas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioMensajeria
    {
        Task<List<VCedulas>> GetCedulasMensajeria(int user);
        Task<int> insertaCedula(CedulaMensajeria cedulaMensajeria);
        Task<int> VerificaCedula(int anio, string mes, int inmueble);
        Task<CedulaMensajeria> CedulaById(int id);
        Task<int> GuardaRespuestas(List<RespuestasEncuesta> respuestas);
        Task<List<RespuestasEncuesta>> obtieneRespuestas(int id);
        Task<int> enviaRespuestas(int cedula);
        Task<int> apruebaRechazaCedula(CedulaMensajeria cedulaMensajeria, int v);
        Task<int> capturaHistorial(HistorialCedulas historialCedulas);
        Task<List<HistorialCedulas>> getHistorialMensajeria(object id);
    }
}
