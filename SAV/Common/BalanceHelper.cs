using SAV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAV.Common
{
    public class BalanceHelper
    {
        public static List<ItemBalanceViewModel> getBalance(List<Viaje> viajes)
        {
            List<ItemBalanceViewModel> exportBalanceViewModel = new List<ItemBalanceViewModel>();

            //Pasajeros
            exportBalanceViewModel.AddRange(viajes.Select(x => new ItemBalanceViewModel(x)).ToList<ItemBalanceViewModel>());

            //Comiciones
            foreach (Viaje viaje in viajes)
                exportBalanceViewModel.AddRange(viaje.ComisionesViaje.Select(x => new ItemBalanceViewModel(viaje, x)).ToList<ItemBalanceViewModel>());

            //Consuctores
            exportBalanceViewModel.AddRange(viajes.Select(x => new ItemBalanceViewModel(x, x.Conductor)).ToList<ItemBalanceViewModel>());

            //Gastos
            foreach (Viaje viaje in viajes)
                exportBalanceViewModel.AddRange(viaje.Gastos.Select(x => new ItemBalanceViewModel(viaje, x)).ToList<ItemBalanceViewModel>());

            return exportBalanceViewModel.OrderBy(x => x.IdViaje).ToList<ItemBalanceViewModel>();
        }
    }
}