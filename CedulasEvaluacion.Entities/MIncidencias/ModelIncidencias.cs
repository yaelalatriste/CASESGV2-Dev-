﻿using CedulasEvaluacion.Entities.MAgua;
using CedulasEvaluacion.Entities.MAnalisis;
using CedulasEvaluacion.Entities.MCelular;
using CedulasEvaluacion.Entities.MConvencional;
using CedulasEvaluacion.Entities.MFumigacion;
using CedulasEvaluacion.Entities.MMensajeria;
using CedulasEvaluacion.Entities.MMuebles;
using CedulasEvaluacion.Entities.Models;
using CedulasEvaluacion.Entities.MResiduos;
using CedulasEvaluacion.Entities.MTransporte;
using CedulasEvaluacion.Entities.TrasladoExp;
using CedulasEvaluacion.Entities.Vistas;
using System;
using System.Collections.Generic;
using System.Text;

namespace CedulasEvaluacion.Entities.MIncidencias
{
    public partial class ModelIncidencias
    {
        public IncidenciasAgua agua { get; set; }
        public IncidenciasAnalisis analisis { get; set; }
        public IncidenciasCelular celular { get; set; }
        public IncidenciasConvencional convencional { get; set; }
        public IncidenciasFumigacion fumigacion { get; set; }
        public IncidenciasLimpieza limpieza { get; set; }
        public VIncidenciasLimpieza iLimpieza { get; set; }
        public IncidenciasMensajeria mensajeria { get; set; }
        public IncidenciasMuebles muebles { get; set; }
        public IncidenciasResiduos residuos { get; set; }
        public IncidenciasTransporte transporte { get; set; }
        public IncidenciasTraslado traslado { get; set; }
        public VIncidenciasLimpieza iEquipo { get; set; }

    }
}
