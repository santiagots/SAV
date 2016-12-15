using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class SearchCuentaCorrienteViewModel
    {
        [Display(Name = "Razón Social/Nombre:")]
        public string RazonSocial { get; set; }

        [Display(Name = "CUIL/DNI:")]
        public string CUIL { get; set; }

        public IPagedList<CuentaCorriente> CuentaCorriente { get; set; }
    }
}