using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class TipoGastoViewModel
    {
        [Display(Name = "Descripción:")]
        [Required(ErrorMessage = "Debe ingresar una Descripción")]
        public string Descripcion { get; set; }

        public IPagedList<TipoGasto> TiposGastos { get; set; }
    }
}