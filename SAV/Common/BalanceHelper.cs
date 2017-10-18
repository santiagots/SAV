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
                        Destino = viaje.Servicio == ViajeTipoServicio.Cerrado? viaje.DestinoCerrado: viaje.Destino.Nombre,
                        HoraArribo = viaje.FechaArribo.ToString("hh:mm"),
                        HoraSalida = viaje.FechaSalida.ToString("hh:mm"),
                        Interno = viaje.Interno,
                        Origen = viaje.Servicio == ViajeTipoServicio.Cerrado? viaje.OrigenCerrado: viaje.Origen.Nombre,
                        Patente = viaje.Patente,
                        Servicio = viaje.Servicio.ToString()};

                List<ItemBalanceViewModel> exportBalanceViewModel = new List<ItemBalanceViewModel>();

                if (viaje.Servicio == ViajeTipoServicio.Cerrado)
                    exportBalanceViewModel.Add(new ItemBalanceViewModel()
                    {
                        Concepto = "Viaje Cerrado",
                        Importe = viaje.CostoCerrado
                    });
                else
                    //Pasajeros
                    exportBalanceViewModel.Add(new ItemBalanceViewModel(viaje));

                //Conductores
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
            var grupoGastos = viajes.SelectMany(x => x.Gastos).GroupBy(y => y.RazonSocial);
            var grupoPasajeros = viajes.GroupBy(x => new { x.Servicio, x.Patente }).ToList();

            foreach (var item in grupoPasajeros)
            {
                
                if (item.First().Servicio == ViajeTipoServicio.Cerrado)
                {
                    int cantidadDeClientes = item.SelectMany(x => x.ClienteViaje).ToList().Count;
                    balanceVeiculoViewModel.Items.Add(new ItemBalanceViewModel()
                    {
                        Concepto = string.Format("Servicio {0} Patente {1} (Total viajes {2} Total pasajeros {3})", item.First().Servicio.ToString(), item.First().Patente.ToString(), item.Count(), cantidadDeClientes),
                        Importe = item.Sum(x => x.CostoCerrado)
                    });
                }
                else
                {
                    int cantidadDeClientes = item.SelectMany(x => x.ClienteViaje).Where(y => y.Presente).ToList().Count;
                    balanceVeiculoViewModel.Items.Add(new ItemBalanceViewModel()
                    {
                        Concepto = string.Format("Servicio {0} (Total viajes {1} Total pasajeros {2})", item.First().Servicio.ToString(), item.Count(), cantidadDeClientes),
                        Importe = item.SelectMany(x => x.ClienteViaje).Sum(y => y.Costo)
                    });
                }
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

        internal static CierreCajaViewModel getBalanceCierreCaja(List<ClienteViaje> clienteViaje, List<Viaje> viajes, List<Comision> comisiones, List<CuentaCorriente> cuentasCorrientes, List<ComisionGasto> comisionGastos, DateTime fecha, DateTime? fechaHasta)
        {
            CierreCajaViewModel balance = new CierreCajaViewModel();

            var grupoConductores = viajes.GroupBy(x => x.Conductor != null ? x.Conductor.ID : -1).ToList();
            var grupoGastos = viajes.SelectMany(x => x.Gastos).GroupBy(y => y.RazonSocial);

            List<Viaje> viajesAuxiliar = new List<Viaje>();

            foreach (ClienteViaje item in clienteViaje)
            {
                Viaje viaje = viajesAuxiliar.FirstOrDefault(x => x.ID == item.Viaje.ID);

                if (viaje != null)
                    viaje.ClienteViaje.Add(item);
                else
                    viajesAuxiliar.Add(
                        new Viaje() {
                            ID = item.Viaje.ID,
                            Servicio = item.Viaje.Servicio,
                            Patente = item.Viaje.Patente,
                            ClienteViaje = new List<ClienteViaje>() }
                        );
            }

            foreach (var viaje in viajesAuxiliar)
            {
                balance.Pasajeros.Add(new ItemBalanceViewModel()
                {
                    Concepto = string.Format("Servicio {0} Patente {1} (Total pasajeros {2})", viaje.Servicio.ToString(), viaje.Patente.ToString(), viaje.ClienteViaje.Count),
                    Importe = viaje.ClienteViaje.Sum(x => x.Costo)
                });
            }

            foreach (var item in grupoConductores)
            {
                if (item.FirstOrDefault().Conductor == null)
                    continue;

                Conductor conductor = item.FirstOrDefault().Conductor;

                balance.Conductores.Add(new ItemBalanceViewModel()
                {
                    Concepto = string.Format("Conductor {0} {1} {2} ({3})", conductor.Apellido, conductor.Nombre, conductor.CUIL, item.Count()),
                    Importe = item.Sum(x => -x.Conductor.ComisionViaje)
                });
            }

            foreach (var item in grupoGastos)
            {
                Gasto gasto = item.FirstOrDefault();

                balance.Gastos.Add(new ItemBalanceViewModel()
                {
                    Concepto = string.Format("Gastos {0} {1} ({2})", gasto.RazonSocial, gasto.CUIT, item.Count()),
                    Importe = item.Sum(x => -decimal.Parse(x.Monto))
                });
            }

            foreach (Comision item in comisiones)
            {
                balance.Comisiones.Add(new ItemBalanceViewModel()
                {
                    Concepto = string.Format("Pago comisión {0}", item.Contacto),
                    Importe = item.Costo
                });
                balance.totalComision += item.Costo;
            }

            foreach (CuentaCorriente item in cuentasCorrientes)
            {
                List<Pago> Pagos = new List<Pago>();
                    if(fechaHasta.HasValue)
                        Pagos = item.Pagos.Where(x => x.Fecha.Date.CompareTo(fecha.Date) >= 0 && x.Fecha.Date.CompareTo(fechaHasta.Value.Date) <= 0).ToList();
                    else
                        Pagos = item.Pagos.Where(x => x.Fecha.Date == fecha.Date).ToList();

                foreach (Pago pago in Pagos)
                {
                    balance.Comisiones.Add(new ItemBalanceViewModel()
                    {
                        Concepto = string.Format("Pago en cuenta {0}", item.RazonSocial),
                        Importe = pago.Monto
                    });
                }
                balance.totalComision += Pagos.Sum(x => x.Monto);
            }

            foreach (ComisionGasto item in comisionGastos)
            {
                balance.Gastos.Add(new ItemBalanceViewModel()
                {
                    Concepto = item.Descripcion,
                    Importe = -item.Monto
                });
            }

            balance.totalPasajeros = balance.Pasajeros.Sum(x => x.Importe);

            balance.totalConductores = balance.Conductores.Sum(x => x.Importe);

            balance.totalComision = balance.Comisiones.Sum(x => x.Importe);

            balance.totalGasto = balance.Gastos.Sum(x => x.Importe);

            balance.total = balance.totalPasajeros + balance.totalConductores + balance.totalComision + balance.totalGasto;

            return balance;
        }

        internal static CierreCajaViewModel getBalanceCierreCaja(List<Viaje> viajes, List<Comision> comisiones, List<CuentaCorriente> cuentasCorrientes, List<ComisionGasto> comisionGastos, DateTime fecha, DateTime fechaHasta)
        {
            throw new NotImplementedException();
        }

        public static BalanceComisionDiarioViewModel getBalanceComision(List<Comision> comisiones, List<CuentaCorriente> cuentasCorrientes, List<ComisionGasto> comisionGasto, DateTime fecha)
        {

            BalanceComisionDiarioViewModel balance = new BalanceComisionDiarioViewModel();

            foreach (Comision item in comisiones)
            {
                    balance.Comisiones.Add(new ItemBalanceComisionViewModel()
                    {
                        Concepto = string.Format("Pago comisión {0}", item.Contacto),
                        Monto = item.Costo
                    });
                balance.totalComision += item.Costo;
            }

            foreach (CuentaCorriente item in cuentasCorrientes)
            {
                List<Pago> Pagos = item.Pagos.Where(x => x.Fecha.Date == fecha.Date).ToList();

                foreach (Pago pago in Pagos)
                {
                    balance.Comisiones.Add(new ItemBalanceComisionViewModel()
                    {
                        Concepto = string.Format("Pago en cuenta {0}", item.RazonSocial),
                        Monto = pago.Monto
                    });
                }
                balance.totalComision += Pagos.Sum(x => x.Monto);
            }

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

        public static BalanceComisionDiarioViewModel getBalanceComision(List<Comision> comisiones, List<CuentaCorriente> cuentasCorrientes, List<ComisionGasto> comisionGasto, DateTime fecha, DateTime fechaHasta)
        {

            BalanceComisionDiarioViewModel balance = new BalanceComisionDiarioViewModel();

            foreach (Comision item in comisiones)
            {
                balance.Comisiones.Add(new ItemBalanceComisionViewModel()
                {
                    Concepto = string.Format("Pago comisión {0}", item.Contacto),
                    Monto = item.Costo
                });
                balance.totalComision += item.Costo;
            }

            foreach (CuentaCorriente item in cuentasCorrientes)
            {
                List<Pago> Pagos = item.Pagos.Where(x => x.Fecha.Date.CompareTo(fecha.Date) >= 0 && x.Fecha.Date.CompareTo(fechaHasta.Date) <= 0).ToList();

                foreach (Pago pago in Pagos)
                {
                    balance.Comisiones.Add(new ItemBalanceComisionViewModel()
                    {
                        Concepto = string.Format("Pago en cuenta {0}", item.RazonSocial),
                        Monto = pago.Monto
                    });
                }
                balance.totalComision += Pagos.Sum(x => x.Monto);
            }

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

        public static List<Viaje> getViajes(List<Viaje> viajes, DateTime fecha)
        {
            return viajes.Where(x => x.FechaArribo.Day == fecha.Day && x.FechaArribo.Month == fecha.Month && x.FechaArribo.Year == fecha.Year && x.Estado == ViajeEstados.Cerrado).ToList<Viaje>();
        }

        public static BalanceVendedorDiarioViewModel getBalanceVendedor(List<Viaje> viajes)
        {
            BalanceVendedorDiarioViewModel balance = new BalanceVendedorDiarioViewModel();

            var grupoCendedores = viajes.SelectMany(x => x.ClienteViaje).GroupBy(y => y.Vendedor);

            foreach (var item in grupoCendedores)
            {
                balance.Items.Add(new ItemBalanceVendedorViewModel()
                {
                    Concepto = string.Format("{0} ({1})", item.First().Vendedor, item.Count()),
                    Monto = item.Where(x=>x.Pago).Sum(x => x.Costo)
                });
                balance.total += item.Where(x => x.Pago).Sum(x => x.Costo);
            }

            return balance;
        }
    }
}