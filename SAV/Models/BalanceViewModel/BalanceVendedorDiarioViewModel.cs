using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class BalanceVendedorDiarioViewModel
    {
        [Display(Name = "Fecha:")]
        [Required(ErrorMessage = "Debe ingresar una Fecha")]
        public string Fecha { get; set; }

        [Display(Name = "Fecha Hasta:")]
        public string FechaHasta { get; set; }

        [Display(Name = "Clave:")]
        [Required(ErrorMessage = "Debe ingresar una Clave")]
        public string Clave { get; set; }

        public List<BalanceVendedorViewModel> BalanceVendedor { get; set; }

        public BalanceVendedorDiarioViewModel()
        {
            BalanceVendedor = new List<BalanceVendedorViewModel>();
        }
    }

    public class ItemBalanceVendedorViewModel
    {
        public string Concepto { get; set; }
        public decimal Monto { get; set; }
    }

    public class BalanceVendedorViewModel
    {
        public string Concepto { get; set; }

        public decimal total { get; set; }

        public List<ItemBalanceVendedorViewModel> Items { get; set; }
    }
}