﻿using System;
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
        [Display(Name = "Clave:")]
        [Required(ErrorMessage = "Debe ingresar una Clave")]
        public string Clave { get; set; }
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

    public class ItemBalanceViewModel
    {
        public int IdViaje { get; set; }

        [Display(Name = "Concepto")]
        public string Concepto { get; set; }

        [Display(Name = "Importe")]
        public decimal Importe { get; set; }

        public ItemBalanceViewModel()
        {
        }

        public ItemBalanceViewModel(Viaje viaje)
        {
            IdViaje = viaje.ID;
            if(viaje.Servicio != ViajeTipoServicio.Cerrado)
                Concepto = String.Format("Pasajeros ({0})", getTotalPasajerosViajaron(viaje));
            else
                Concepto = String.Format("Pasajeros ({0})", getTotalPasajerosViajaron(viaje));
            Importe = getImporte(viaje);
        }

        public ItemBalanceViewModel(Viaje viaje, Conductor conductor)
        {
            Concepto = String.Format("Conductor {0} {1} ({2})", conductor.Apellido, conductor.Nombre, conductor.CUIL);

            if (viaje.Servicio == ViajeTipoServicio.Cerrado)
                Importe = Math.Round(-(getImporte(viaje) * (conductor.ComisionViajeCerrado / 100)), 2, MidpointRounding.ToEven);
            else
                Importe = Math.Round(-conductor.ComisionViaje, 2, MidpointRounding.ToEven); ;
        }

        public ItemBalanceViewModel(Gasto gasto)
        {
            Concepto = String.Format("Gasto {0} {1}", gasto.RazonSocial, gasto.CUIT);
            Importe = -decimal.Parse(gasto.Monto);
        }

        private decimal getImporte(Viaje viaje)
        {
            return Math.Round((from item in viaje.ClienteViaje
                    where item.Pago
                    select item.Costo).Sum(), 2 , MidpointRounding.ToEven);
        }

        private int getTotalPasajerosViajaron(Viaje viaje)
        {
            return viaje.ClienteViaje.Count(x => x.Pago);
        }
    }
}