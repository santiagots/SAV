using SAV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAV.Common
{
    public class BalanceHelper
    {
        public static BalanceViajeDiarioViewModel getBalanceViaje(List<Viaje> viajes)
        {

            BalanceViajeDiarioViewModel balance = new BalanceViajeDiarioViewModel();

            foreach (Viaje viaje in viajes)
            {
                BalanceVeiculoViewModel balanceVeiculoViewModel = new BalanceVeiculoViewModel()
                        {Id = viaje.ID,
                        Destino = viaje.Destino.Nombre,
                        HoraArribo = viaje.FechaArribo.ToString("hh:mm"),
                        HoraSalida = viaje.FechaSalida.ToString("hh:mm"),
                        Interno = viaje.Interno,
                        Origen = viaje.Origen.Nombre,
                        Patente = viaje.Patente,
                        Servicio = viaje.Servicio.ToString()};

                List<ItemBalanceViewModel> exportBalanceViewModel = new List<ItemBalanceViewModel>();

                //Pasajeros
                exportBalanceViewModel.Add(new ItemBalanceViewModel(viaje));

                //Consuctores
                if(viaje.Conductor != null)
                    exportBalanceViewModel.Add(new ItemBalanceViewModel(viaje, viaje.Conductor));

                //Gastos
                exportBalanceViewModel.AddRange(viaje.Gastos.Select(x => new ItemBalanceViewModel(x)).ToList<ItemBalanceViewModel>());

                balanceVeiculoViewModel.Items = exportBalanceViewModel;

                balanceVeiculoViewModel.total = Math.Round(balanceVeiculoViewModel.Items.Sum(x => x.Importe), 2, MidpointRounding.ToEven);

                balance.Veiculos.Add(balanceVeiculoViewModel);
            }

            return balance;
        }

        public static BalanceViajeDiarioViewModel getBalanceViajeConsolidado(List<Viaje> viajes)
        {

            BalanceViajeDiarioViewModel balance = new BalanceViajeDiarioViewModel();

            BalanceVeiculoViewModel balanceVeiculoViewModel = new BalanceVeiculoViewModel();
            balance.Veiculos.Add(balanceVeiculoViewModel);

            var grupoConductores = viajes.GroupBy(x => x.Conductor != null? x.Conductor.ID:-1).ToList();
            var grupoGastos = viajes.SelectMany(x => x.Gastos).GroupBy(y => y.Comentario);
            var grupoPasajeros = viajes.SelectMany(x => x.ClienteViaje).GroupBy(y => y.Viaje.Servicio);

            foreach (var item in grupoPasajeros)
            {
                balanceVeiculoViewModel.Items.Add(new ItemBalanceViewModel()
                {
                    Concepto = string.Format("{0} ({1})", item.First().Viaje.Servicio.ToString(), grupoPasajeros.Count()),
                    Importe = item.Sum(x => x.Costo)
                });
            }

            foreach (var item in grupoConductores)
            {
                if (item.FirstOrDefault().Conductor == null)
                    continue;

                Conductor conductor = item.FirstOrDefault().Conductor;

                 balanceVeiculoViewModel.Items.Add(new ItemBalanceViewModel() {
                     Concepto = string.Format("Conductor {0} {1} {2} ({3})", conductor.Apellido, conductor.Nombre, conductor.CUIL, item.Count()),
                     Importe = item.Sum(x => -x.Conductor.ComisionViaje)
                });
            }

            foreach (var item in grupoGastos)
            {
                Gasto gasto = item.FirstOrDefault();

                balanceVeiculoViewModel.Items.Add(new ItemBalanceViewModel()
                {
                    Concepto = string.Format("Gastos {0} {1} ({2})", gasto.RazonSocial, gasto.CUIT, item.Count()),
                    Importe = item.Sum(x => -decimal.Parse(x.Monto))
                });
            }

            balanceVeiculoViewModel.total = Math.Round(balanceVeiculoViewModel.Items.Sum(x => x.Importe), 2, MidpointRounding.ToEven);

            return balance;
        }

        public static BalanceComisionDiarioViewModel getBalanceComision(List<Comision> comision, List<ComisionGasto> comisionGasto)
        {

            BalanceComisionDiarioViewModel balance = new BalanceComisionDiarioViewModel();

            foreach (Comision item in comision)
            {
                balance.Comisiones.Add(new ItemBalanceComisionViewModel()
                {
                    Concepto = item.Contacto,
                    Monto = item.Costo
                });
            }

            balance.totalComision = comision.Sum(x => x.Costo);

            foreach (ComisionGasto item in comisionGasto)
            {
                balance.Gastos.Add(new ItemBalanceComisionViewModel()
                {
                    Concepto = item.Descripcion,
                    Monto = -item.Monto
                });
            }

            balance.totalGasto = -comisionGasto.Sum(x => x.Monto);

            balance.total = balance.totalComision + balance.totalGasto;

            return balance;
        }

        internal static List<Comision> getComisiones(List<Comision> list, DateTime fecha)
        {
            return list.Where(x => x.FechaPago.HasValue && x.FechaPago.Value.Date == fecha.Date).ToList();
        }

        internal static List<ComisionGasto> getComisionesGastos(List<ComisionGasto> list, DateTime fecha)
        {
            return list.Where(x => x.FechaAlta.Date == fecha.Date).ToList();
        }

        public static List<Viaje> getViajes(List<Viaje> viajes, DateTime fecha)
        {
            return viajes.Where(x => x.FechaArribo.Day == fecha.Day && x.FechaArribo.Month == fecha.Month && x.FechaArribo.Year == fecha.Year && x.Estado == ViajeEstados.Cerrado).ToList<Viaje>();
        }
    }
}