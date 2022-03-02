using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.Vistas
{
    public partial class VModulosUsuario
    {
        public int Id { get; set; }
        public int PerfilId { get; set; }
        public string Empleado { get; set; }
        public string Modulo { get; set; }
    }
}
