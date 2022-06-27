using CedulasEvaluacion.Entities.MFacturas;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioFacturas
    {
        Task<Facturas> insertaFacturas(Facturas facturas);
        Task<int> insertaConceptoFacturas(Facturas facturas);
        Task<Facturas> updateFacturas(Facturas facturas);
        Task<int> updateConceptoFacturas(Facturas facturas);
        Task<List<Facturas>> getFacturas(int cedula,int servicio);
        Task<int> deleteFactura(int factura);
        decimal obtieneTotalFacturas(List<Facturas> facturas);
        Task<List<Facturas>> getFacturasPago(int servicio);
    }
}
