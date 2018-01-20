using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class CierreCajaViewModel
    {
        [Display(Name = "Fecha:")]
        [Required(ErrorMessage = "Debe ingresar una Fecha")]
        public string Fecha { get; set; }

        [Display(Name = "Fecha Hasta:")]
        public string FechaHasta { get; set; }

        public List<ItemBalanceViewModel> Pasajeros { get; set; }
        public List<ItemBalanceViewModel> Conductores { get; set; }
        public List<ItemBalanceViewModel> Comisiones { get; set; }
        public List<ItemBalanceViewModel> Gastos { get; set; }
        public decimal totalPasajeros { get; set; }
        public decimal totalConductores { get; set; }
        public decimal totalComision { get; set; }
        public decimal totalGasto { get; set; }
        public decimal total { get; set; }

        public CierreCajaViewModel()
        {
            Pasajeros = new List<ItemBalanceViewModel>();
            Conductores = new List<ItemBalanceViewModel>();
            Comisiones = new List<ItemBalanceViewModel>();
            Gastos = new List<ItemBalanceViewModel>();
        }
    }
}