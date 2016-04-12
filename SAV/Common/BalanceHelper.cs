using SAV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAV.Common
{
    public class BalanceHelper
    {
        public static BalanceViewModel getBalance(List<Viaje> viajes)
        {

            BalanceViewModel balance = new BalanceViewModel();

            foreach (Viaje viaje in viajes)
            {
                BalanceVeiculoViewModel balanceVeiculoViewModel = new BalanceVeiculoViewModel()
                        {Destino = viaje.Destino.Nombre,
                        HoraArribo = viaje.FechaArribo.ToString("hh:mm"),
                        HoraSalida = viaje.FechaSalida.ToString("hh:mm"),
                        Interno = viaje.Interno,
                        Origen = viaje.Origen.Nombre,
                        Patente = viaje.Patente,
                        Servicio = viaje.Servicio.ToString()};

                List<ItemBalanceViewModel> exportBalanceViewModel = new List<ItemBalanceViewModel>();

                //Pasajeros
                exportBalanceViewModel.Add(new ItemBalanceViewModel(viaje));

                //Comiciones
                exportBalanceViewModel.AddRange(viaje.ComisionesViaje.Select(x => new ItemBalanceViewModel(viaje, x)).ToList<ItemBalanceViewModel>());

                //Consuctores
                exportBalanceViewModel.Add(new ItemBalanceViewModel(viaje, viaje.Conductor));

                //Gastos
                exportBalanceViewModel.AddRange(viaje.Gastos.Select(x => new ItemBalanceViewModel(viaje, x)).ToList<ItemBalanceViewModel>());

                balanceVeiculoViewModel.Items = exportBalanceViewModel;

                balanceVeiculoViewModel.total = Math.Round(balanceVeiculoViewModel.Items.Sum(x => x.Importe), 2, MidpointRounding.ToEven);

                balance.Veiculos.Add(balanceVeiculoViewModel);
            }

            return balance;
        }
    }
}