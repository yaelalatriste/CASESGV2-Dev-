using CedulasEvaluacion.Entities.Vistas;
using CedulasEvaluacion.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioLimpieza
    {
        Task<List<VCedulas>> GetCedulasLimpieza(int user);//Devolvemos las cedulas en base al Tipo

        //Creamos la nueva Cedula
        Task<int> NuevaCedula(CedulaLimpieza cedLimpieza);
        Task<int> VerificaCedula(int anio, string mes, int inmueble);
        //Obtenemos la Cedula para Modificar 
        Task<CedulaLimpieza> CedulaById(int id);
        Task<int> GuardaRespuestas(List<RespuestasEncuesta> respuestasEncuestas);
        Task<List<RespuestasEncuesta>> obtieneRespuestas(int id);
        Task<int> enviaRespuestas(List<RespuestasEncuesta> respuestasEncuestas);
        Task<int> apruebaRechazaCedula(CedulaLimpieza cedulaLimpieza);
        Task<int> capturaHistorial(HistorialCedulas historialCedulas);
        Task<List<HistorialCedulas>> getHistorial(int cedula);
        Task<int> EliminaCedula(int id);
    }
}
