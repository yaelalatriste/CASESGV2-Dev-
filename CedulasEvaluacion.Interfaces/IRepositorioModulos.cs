﻿using CedulasEvaluacion.Entities.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CedulasEvaluacion.Interfaces
{
    public interface IRepositorioModulos
    {
        Task<List<Modulos>> getModulos();
    }
}
