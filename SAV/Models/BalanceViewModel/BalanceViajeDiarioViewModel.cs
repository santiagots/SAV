using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class BalanceViajeDiarioViewModel
    {
        [Display(Name = "Fecha:")]
        [Required(ErrorMessage = "Debe ingresar una Fecha")]
        public string Fecha { get; set; }
        [Display(Name = "Fecha Hasta:")]
        public string FechaHasta { get; set; }
        public List<BalanceVeiculoViewModel> Veiculos { get; set; }
        public BalanceViajeDiarioViewModel()
        {
            Veiculos = new List<BalanceVeiculoViewModel>();
        }
    }

    public class BalanceVeiculoViewModel
    {
        public int Id { get; set; }
        public string Origen { get; set; }
        public string Destino { get; set; }
        public string HoraSalida { get; set; }
        public string HoraArribo { get; set; }
        public string Servicio { get; set; }
        public string Patente { get; set; }
        public int Interno { get; set; }
        public decimal total { get; set; }

        public List<ItemBalanceViewModel> Items { get; set; }

        public BalanceVeiculoViewModel()
        {
            Items = new List<ItemBalanceViewModel>();
        }
    }
}