using CedulasEvaluacion.Entities.MPerfiles;
using CedulasEvaluacion.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Services
{
    public class ServicePermisos
    {
        private readonly IRepositorioOperacionesPerfil vPermisos;

        public ServicePermisos(IRepositorioOperacionesPerfil viPermisos)
        {
            this.vPermisos = viPermisos ?? throw new ArgumentNullException(nameof(viPermisos));
        }

        public async Task<PermisosPerfil> GetVModulos(string permiso, string modulo, int usuario)
        {
            PermisosPerfil modulos = await vPermisos.GetPermisoModuloByUser(permiso,modulo,usuario);
            return modulos;
        }
    }
}
