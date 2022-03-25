using CASESGCedulasEvaluacion.Entities.Vistas;
using CedulasEvaluacion.Entities.MFinancieros;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.Vistas;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioFinancieros
    {
        Task<List<DashboardFinancieros>> GetCedulasFinancieros();//dashboard Financieros
        Task<List<DashboardFinancieros>> GetDetalleServicio(string servicio);//Detalle del Servicio
        Task<List<Oficio>> GetOficiosFinancieros(string servicio);//obtiene los oficios en base al servicio
        Task<Oficio> GetOficioById(int id);//obtiene el oficio por id de Oficio
        Task<int> insertarNuevoOficio(Oficio oficio);//captura nuevo oficio
        Task<List<DetalleCedula>> GetCedulasTramitePago(int id, string servicio); // Obtiene el Total de Cédulas que están en Trámite de Pago
        Task<List<DetalleCedula>> GetCedulasOficio(int id); // Obtiene las cédulas que ya están dentro de un oficio
        Task<int> insertarCedulasOficio(List<CedulasOficio> cedulas);
    }
}
