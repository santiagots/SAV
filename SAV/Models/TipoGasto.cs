﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class TipoGasto
    {
        public int ID { get; set; }
        public ConceptoGasto Concepto { get; set; }
        public string Descripcion { get; set; }
        public bool Habilitado { get; set; }
    }
}