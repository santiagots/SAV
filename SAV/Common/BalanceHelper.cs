﻿using SAV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

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
                exportBalanceViewModel.AddRange(viaje.Gastos.Where(x=> x.TipoGasto != null).Select(x => new ItemBalanceViewModel(x)).ToList<ItemBalanceViewModel>());

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
            var grupoGastos = viajes.SelectMany(x => x.Gastos).GroupBy(y => y.TipoGasto.Descripcion);
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
                    Concepto = string.Format("Gastos {0} ({1})", item.Key, item.Count()),
                    Importe = item.Sum(x => -x.Monto)
                });
            }

            balanceVeiculoViewModel.total = Math.Round(balanceVeiculoViewModel.Items.Sum(x => x.Importe), 2, MidpointRounding.ToEven);

            return balance;
        }

        internal static CierreCajaViewModel getBalanceCierreCaja(List<ClienteViaje> clienteViaje, List<Viaje> viajes, List<Comision> comisiones, List<CuentaCorriente> cuentasCorrientes, List<Gasto> comisionGastos, DateTime fecha, DateTime? fechaHasta)
        {
            CierreCajaViewModel balance = new CierreCajaViewModel();

            var grupoConductores = viajes.GroupBy(x => x.Conductor != null ? x.Conductor.ID : -1).ToList();
            var grupoGastos = viajes.SelectMany(x => x.Gastos).Where(x => x.TipoGasto != null).GroupBy(y => y.TipoGasto.Descripcion);

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
                    Concepto = string.Format("Gastos {0} ({1})", gasto.TipoGasto.Descripcion, item.Count()),
                    Importe = item.Sum(x => -x.Monto)
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

            foreach (Gasto item in comisionGastos)
            {
                balance.Gastos.Add(new ItemBalanceViewModel()
                {
                    Concepto = item.Comentario,
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

        internal static CierreCajaViewModel getBalanceCierreCaja(List<Viaje> viajes, List<Comision> comisiones, List<CuentaCorriente> cuentasCorrientes, List<Gasto> comisionGastos, DateTime fecha, DateTime fechaHasta)
        {
            throw new NotImplementedException();
        }

        public static BalanceComisionDiarioViewModel getBalanceComision(List<Comision> comisiones, List<CuentaCorriente> cuentasCorrientes, List<Gasto> comisionGasto, DateTime fecha)
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

            foreach (Gasto item in comisionGasto)
            {
                balance.Gastos.Add(new ItemBalanceComisionViewModel()
                {
                    Concepto = string.Format("Gasto {0} {1} {2}", item.Concepto, item.TipoGasto.Descripcion, item.Comentario),
                    Monto = -item.Monto
                });
            }

            balance.totalGasto = -comisionGasto.Sum(x => x.Monto);

            balance.total = balance.totalComision + balance.totalGasto;

            return balance;
        }

        public static BalanceComisionDiarioViewModel getBalanceComision(List<Comision> comisiones, List<CuentaCorriente> cuentasCorrientes, List<Gasto> gastos, DateTime fecha, DateTime fechaHasta)
        {

            BalanceComisionDiarioViewModel balance = new BalanceComisionDiarioViewModel();
            var grupoGastos = gastos.Where(x => x.TipoGasto != null).GroupBy(y => y.TipoGasto.Descripcion);

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

            foreach (var item in grupoGastos)
            {
                Gasto gasto = item.FirstOrDefault();

                balance.Comisiones.Add(new ItemBalanceComisionViewModel()
                {
                    Concepto = string.Format("Gastos {0} ({1})", gasto.TipoGasto.Descripcion, item.Count()),
                    Monto = item.Sum(x => -x.Monto)
                });

                balance.totalGasto = -item.Sum(x => x.Monto);
            }

            balance.total = balance.totalComision + balance.totalGasto;

            return balance;
        }

        public static List<Viaje> getViajes(List<Viaje> viajes, DateTime fecha)
        {
            return viajes.Where(x => x.FechaArribo.Day == fecha.Day && x.FechaArribo.Month == fecha.Month && x.FechaArribo.Year == fecha.Year && x.Estado == ViajeEstados.Cerrado).ToList<Viaje>();
        }

        public static BalanceVendedorDiarioViewModel getBalanceVendedor(List<ClienteViaje> clienteViaje, List<Gasto> Gastos, String[] Rol, String Usuario)
        {
            BalanceVendedorDiarioViewModel balance = new BalanceVendedorDiarioViewModel();
            List<IGrouping<string, ClienteViaje>> gruposCobrosPorVendedor;
            List<IGrouping<string, Gasto>> gruposGastosPorVendedor;
            List<UsuarioViewModel> usuarios = new List<UsuarioViewModel>();

            if (Rol.Contains("Administrador"))
            {
                gruposCobrosPorVendedor = clienteViaje.GroupBy(x => x.VendedorCobro).ToList();
                gruposGastosPorVendedor = Gastos.GroupBy(x => x.UsuarioAlta).ToList();
                usuarios = UsuarioHelper.getUsuarios();
            }
            else
            {
                gruposCobrosPorVendedor = clienteViaje.Where(x => x.VendedorCobro == Usuario).GroupBy(x => x.VendedorCobro).ToList();
                gruposGastosPorVendedor = Gastos.Where(x => x.UsuarioAlta == Usuario).GroupBy(x => x.UsuarioAlta).ToList();
                usuarios.Add(new UsuarioViewModel() { Usuario = Usuario });
            }

            foreach (UsuarioViewModel vendedor in usuarios)
            {
                IGrouping<string, Gasto> gastoPorVendedor = gruposGastosPorVendedor.FirstOrDefault(x => x.Key.ToUpper() == vendedor.Usuario.ToUpper());
                IGrouping<string, ClienteViaje> cobroPorVendedor = gruposCobrosPorVendedor.FirstOrDefault(x => x.Key.ToUpper() == vendedor.Usuario.ToUpper());

                if (gastoPorVendedor == null && cobroPorVendedor == null)
                    continue;

                BalanceVendedorViewModel balanceVendedor = new BalanceVendedorViewModel()
                {
                    Concepto = string.Format("Usuario: {0}", vendedor.Usuario),
                    Items = new List<ItemBalanceVendedorViewModel>()
                };

                if (cobroPorVendedor != null)
                {
                    var Grupospagos = cobroPorVendedor.GroupBy(x => x.FormaPago).ToList();

                    foreach (var pagos in Grupospagos)
                    {
                        
                        foreach (var item in pagos)
                        {
                            balanceVendedor.Items.Add(new ItemBalanceVendedorViewModel()
                            {
                                Concepto = string.Format("{0} {1} - Cod. {2} Fecha {3}  Serv. {4}", item.Cliente.Apellido, item.Cliente.Nombre, item.Viaje.ID, item.Viaje.FechaSalida, item.Viaje.Servicio),
                                Monto = item.Costo
                            });
                        }

                        if (pagos.Key.Descripcion == "Efectivo" && gastoPorVendedor != null)
                        {
                            foreach (var gasto in gastoPorVendedor)
                            {
                                balanceVendedor.Items.Add(new ItemBalanceVendedorViewModel()
                                {
                                    Concepto = string.Format("Gastos {0} {1} {2}", gasto.Concepto, gasto.TipoGasto.Descripcion, gasto.Comentario),
                                    Monto = -gasto.Monto
                                });
                            }
                        }

                        balanceVendedor.Items.Add(new ItemBalanceVendedorViewModel() {
                            Concepto = string.Format("Sub Total {0}", pagos.Key != null ? pagos.Key.Descripcion : string.Empty),
                            Monto = pagos.Sum(x => x.Costo) - gastoPorVendedor.Sum(x => x.Monto),
                            SubTotal = true
                            });
                        balanceVendedor.total += pagos.Sum(x => x.Costo) - gastoPorVendedor.Sum(x => x.Monto);
                    }
                }

                if (!cobroPorVendedor.Any(x=> x.FormaPago.Descripcion == "Efectivo") && gastoPorVendedor != null)
                {
                    foreach (var gasto in gastoPorVendedor)
                    {
                        balanceVendedor.Items.Add(new ItemBalanceVendedorViewModel()
                        {
                            Concepto = string.Format("Gastos {0} {1} {2}", gasto.Concepto, gasto.TipoGasto.Descripcion, gasto.Comentario),
                            Monto = -gasto.Monto
                        });
                    }

                    balanceVendedor.Items.Add(new ItemBalanceVendedorViewModel()
                    {
                        Concepto = string.Format("Sub Total Gastos"),
                        Monto = -gastoPorVendedor.Sum(x => x.Monto),
                        SubTotal = true
                    });
                    balanceVendedor.total -= gastoPorVendedor.Sum(x => x.Monto);
                }
                balance.BalanceVendedor.Add(balanceVendedor);
            }

            return balance;
        }
    }
}