using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class BalanceComisionDiarioViewModel
    {
        [Display(Name = "Fecha:")]
        [Required(ErrorMessage = "Debe ingresar una Fecha")]
        public string Fecha { get; set; }

        [Display(Name = "Fecha Hasta:")]
        public string FechaHasta { get; set; }

        public List<ItemBalanceComisionViewModel> Items { get; set; }
        public decimal total { get; set; }

        public BalanceComisionDiarioViewModel()
        {
            Items = new List<ItemBalanceComisionViewModel>();
        }
    }

    public class ItemBalanceComisionViewModel
    {
        [Display(Name = "Concepto")]
        public string Concepto { get; set; }

        [Display(Name = "Monto")]
        public decimal Monto { get; set; }

        public bool SubTotal { get; set; }
    }
}