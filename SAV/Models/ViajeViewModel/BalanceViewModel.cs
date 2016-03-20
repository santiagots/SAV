using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class BalanceViewModel
    {
        [Display(Name = "Fecha:")]
        [Required(ErrorMessage = "Debe ingresar una Fecha")]
        public string Fecha { get; set; }
        public List<ItemBalanceViewModel> Items { get; set; }

        public BalanceViewModel()
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

        public ItemBalanceViewModel(Viaje viaje)
        {
            IdViaje = viaje.ID;
            if(viaje.Servicio != ViajeTipoServicio.Cerrado)
                Concepto = String.Format("[{0}]Pasajeros - {1} - {2}", viaje.ID, viaje.Origen.Nombre, viaje.Destino.Nombre);
            else
                Concepto = String.Format("[{0}]Pasajeros - {1} - {2}", viaje.ID, viaje.OrigenCerrado, viaje.DestinoCerrado);
            Importe = getImporte(viaje);
        }

        public ItemBalanceViewModel(Viaje viaje, ComisionViaje comisionViaje)
        {
            IdViaje = viaje.ID;
            Concepto = String.Format("[{0}]Com. {1} - {2} - {3}", viaje.ID, comisionViaje.Comision.Contacto, viaje.Origen.Nombre, viaje.Destino.Nombre);
            Importe = comisionViaje.Comision.Costo;
        }

        public ItemBalanceViewModel(Viaje viaje, Conductor conductor)
        {
            IdViaje = viaje.ID;
            Concepto = String.Format("[{0}]Conductor {1} {2}", viaje.ID, conductor.Apellido, conductor.Nombre);

            if (viaje.Servicio == ViajeTipoServicio.Cerrado)
                Importe = Math.Round(-(getImporte(viaje) * (conductor.ComisionViajeCerrado / 100)), 2, MidpointRounding.ToEven);
            else
                Importe = Math.Round(-conductor.ComisionViaje, 2, MidpointRounding.ToEven); ;
        }

        public ItemBalanceViewModel(Viaje viaje, Gasto gasto)
        {
            IdViaje = viaje.ID;
            Concepto = String.Format("[{0}]Gasto {1} {2}", viaje.ID, gasto.RazonSocial, gasto.CUIT);
            Importe = -decimal.Parse(gasto.Monto);
        }

        private decimal getImporte(Viaje viaje)
        {
            return Math.Round((from item in viaje.ClienteViaje
                    where item.Pago
                    select item.Costo).Sum(), 2 , MidpointRounding.ToEven);
        }

    }
}