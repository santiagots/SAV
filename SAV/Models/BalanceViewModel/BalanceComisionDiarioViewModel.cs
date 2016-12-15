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

        [Display(Name = "Clave:")]
        [Required(ErrorMessage = "Debe ingresar una Clave")]
        public string Clave { get; set; }

        public List<ItemBalanceComisionViewModel> Comisiones { get; set; }
        public List<ItemBalanceComisionViewModel> Gastos { get; set; }
        public decimal totalComision { get; set; }
        public decimal totalGasto { get; set; }
        public decimal total { get; set; }

        public BalanceComisionDiarioViewModel()
        {
            Comisiones = new List<ItemBalanceComisionViewModel>();
            Gastos = new List<ItemBalanceComisionViewModel>();
        }
    }

    public class ItemBalanceComisionViewModel
    {
        [Display(Name = "Concepto")]
        public string Concepto { get; set; }

        [Display(Name = "Monto")]
        public decimal Monto { get; set; }

       
    }
}