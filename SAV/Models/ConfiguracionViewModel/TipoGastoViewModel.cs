﻿using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class TipoGastoViewModel
    {
        [Display(Name = "Concepto:")]
        public List<KeyValuePair<int, string>> Concepto { get; set; }

        [Required(ErrorMessage = "Debe ingresar una Concepto")]
        public int? SelectConcepto { get; set; }

        [Display(Name = "Descripción:")]
        [Required(ErrorMessage = "Debe ingresar una Descripción")]
        public string Descripcion { get; set; }

        public IPagedList<TipoGasto> TiposGastos { get; set; }
    }
}