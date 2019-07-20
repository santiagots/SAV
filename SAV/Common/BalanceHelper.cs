using SAV.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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

        internal static CierreCajaViewModel getBalanceCierreCaja(List<ClienteViaje> clienteViaje, List<Viaje> viajes, List<Comision> comisiones, List<CuentaCorriente> cuentasCorrientes, List<Gasto> Gastos, List<AdicionalConductor> adiocionales, DateTime fecha, DateTime? fechaHasta)
        {
            CierreCajaViewModel balance = new CierreCajaViewModel();

            //PASAJEROS
            List<IGrouping<Tuple<ViajeTipoServicio, string>, Viaje>> GruposPasajeros = viajes.GroupBy(x => new Tuple<ViajeTipoServicio, string>(x.Servicio, x.Patente)).ToList();

            foreach (IGrouping<Tuple<ViajeTipoServicio, string>, Viaje> grupo in GruposPasajeros)
            {
                balance.Pasajeros.Add(new ItemBalanceViewModel()
                {
                    Concepto = string.Format("Servicio Patente {1} Tipo {0} ", grupo.Key.Item1, grupo.Key.Item2),
                    Importe = grupo.Sum(x =>x.ClienteViaje.Sum(y => y.Costo))
                });
            }

            //CONDUCTORES
            List<IGrouping<Conductor, Viaje>> GrupoCondictores = viajes.GroupBy(x => x.Conductor).ToList();

            foreach (IGrouping<Conductor, Viaje> grupo in GrupoCondictores)
            {
                decimal totalConductor = 0;
                foreach (Viaje viaje in grupo)
                {
                    if (viaje.Servicio == ViajeTipoServicio.Cerrado)
                        totalConductor -= grupo.Key.ComisionViajeCerrado * viaje.ClienteViaje.Sum(x => x.Costo);
                    else
                        totalConductor -= grupo.Key.ComisionViaje;

                    totalConductor -= viaje.AdicionalConductor.Sum(x => x.Monto);
                }

                balance.Conductores.Add(new ItemBalanceViewModel()
                {
                    Concepto = string.Format("Conductor {0} {1} {2}", grupo.Key.Apellido, grupo.Key.Nombre, grupo.Key.CUIL),
                    Importe = totalConductor
                });                
            }

            //COMISIONES
            //Comisiones que no tiene cuenta corriente
            balance.Comisiones.Add(new ItemBalanceViewModel()
            {
                Concepto = "Comisiones sin cuenta corriente",
                Importe = comisiones.Sum(x => x.Costo)
            });

            //Comisiones con cuenta corriente
            foreach (CuentaCorriente cuentasCorriente in cuentasCorrientes)
            {
                List<Pago> pagos = new List<Pago>();
                if (fechaHasta.HasValue)
                    pagos = cuentasCorriente.Pagos.Where(x => x.Fecha.CompareTo(fecha) >= 0 && x.Fecha.CompareTo(fechaHasta.Value) <= 0).ToList();
                    //pagos = cuentasCorriente.Pagos.Where(x => DbFunctions.TruncateTime(x.Fecha).Value.CompareTo(fecha) >= 0 && DbFunctions.TruncateTime(x.Fecha).Value.CompareTo(fechaHasta.Value) <= 0).ToList();
                else
                    pagos = cuentasCorriente.Pagos.Where(x => x.Fecha.Date == fecha.Date).ToList();
                    //pagos = cuentasCorriente.Pagos.Where(x => DbFunctions.TruncateTime(x.Fecha).Value == fecha.Date).ToList();

                if (pagos.Any())
                {
                    balance.Comisiones.Add(new ItemBalanceViewModel()
                    {
                        Concepto = string.Format("Pago en cuenta {0}", cuentasCorriente.RazonSocial),
                        Importe = pagos.Sum(x => x.Monto)
                    });
                }
            }

            //GASTOS
            List<IGrouping<ConceptoGasto, Gasto>> gastos = Gastos.GroupBy(x => x.Concepto).ToList();

            foreach (IGrouping<ConceptoGasto, Gasto> gasto in gastos)
            {
                balance.Gastos.Add(new ItemBalanceViewModel()
                {
                    Concepto = string.Format("Gastos {0}", gasto.Key),
                    Importe = gasto.Sum(x => -x.Monto)
                });
            }

            balance.Pasajeros = balance.Pasajeros.OrderBy(x => x.Concepto).ToList();
            balance.Pasajeros.RemoveAll(x => x.Importe == 0);
            balance.totalPasajeros = balance.Pasajeros.Sum(x => x.Importe);

            balance.Conductores = balance.Conductores.OrderBy(x => x.Concepto).ToList();
            balance.Conductores.RemoveAll(x => x.Importe == 0);
            balance.totalConductores = balance.Conductores.Sum(x => x.Importe);

            balance.Comisiones = balance.Comisiones.OrderBy(x => x.Concepto).ToList();
            balance.Comisiones.RemoveAll(x => x.Importe == 0);
            balance.totalComision = balance.Comisiones.Sum(x => x.Importe);

            balance.Gastos = balance.Gastos.OrderBy(x => x.Concepto).ToList();
            balance.Gastos.RemoveAll(x => x.Importe == 0);
            balance.totalGasto = balance.Gastos.Sum(x => x.Importe);

            balance.total = balance.totalPasajeros + balance.totalConductores + balance.totalComision + balance.totalGasto;

            return balance;
        }

        public static BalanceComisionDiarioViewModel getBalanceComision(List<FormaPago> FormasPago, List<Comision> comisiones, List<CuentaCorriente> CuentasCorriente, List<Gasto> comisionGasto)
        {

            BalanceComisionDiarioViewModel balance = new BalanceComisionDiarioViewModel();

            List<IGrouping<string, Comision>> comisionesPorContacto = comisiones.GroupBy(x => x.Contacto).ToList();
            List<IGrouping<string, CuentaCorriente>> cuentasCorrientePorRazonSocial = CuentasCorriente.GroupBy(x => x.RazonSocial).ToList();
            List<IGrouping<string, Gasto>> gastosPorTipo = comisionGasto.GroupBy(x => x.TipoGasto.Descripcion).ToList();

            decimal Total = 0;
            foreach (FormaPago formaPago in FormasPago)
            {
                decimal Subtotal = 0;
                List<IGrouping<string, Comision>> comisionesPorFechaPorFormaPago = comisionesPorContacto.Where(x => x.Any(y => y.FormaPago?.Descripcion.ToUpper() == formaPago.Descripcion.ToUpper())).ToList();
                foreach (IGrouping<string, Comision> comisionePorFechaPorFormaPago in comisionesPorFechaPorFormaPago)
                {
                    balance.Items.Add(new ItemBalanceComisionViewModel()
                    {
                        Concepto = $"Comisiones {comisionePorFechaPorFormaPago.Key} ({comisionePorFechaPorFormaPago.Count()})",
                        Monto = comisionePorFechaPorFormaPago.Sum(x => x.Costo)
                    });

                    Subtotal += comisionePorFechaPorFormaPago.Sum(x => x.Costo);
                }

                foreach (IGrouping<string, CuentaCorriente> cuentaCorrientePorRazonSocial in cuentasCorrientePorRazonSocial)
                {
                    List<Pago> pagos = cuentaCorrientePorRazonSocial.SelectMany(x => x.Pagos).Where(y => y.FormaPago.Descripcion.ToUpper() == formaPago.Descripcion.ToUpper()).ToList();
                    if (pagos.Count > 0)
                    {
                        balance.Items.Add(new ItemBalanceComisionViewModel()
                        {
                            Concepto = $"CuentaCorriente {cuentaCorrientePorRazonSocial.Key} ({cuentaCorrientePorRazonSocial.Count()})",
                            Monto = pagos.Sum(x => x.Monto)
                        });

                        Subtotal += pagos.Sum(x => x.Monto);
                    }
                }

                if (formaPago.Descripcion.ToUpper() == "Efectivo".ToUpper())
                {
                    foreach (IGrouping<string, Gasto> gastoPorTipo in gastosPorTipo)
                    {
                        balance.Items.Add(new ItemBalanceComisionViewModel()
                        {
                            Concepto = $"Gastos {gastoPorTipo.Key} ({gastoPorTipo.Count()})",
                            Monto = -gastoPorTipo.Sum(x => x.Monto)
                        });

                        Subtotal -= gastoPorTipo.Sum(x => x.Monto);
                    }
                }

                if (Subtotal != 0)
                {
                    balance.Items.Add(new ItemBalanceComisionViewModel()
                    {
                        Concepto = string.Format("Sub Total {0}", formaPago.Descripcion),
                        Monto = Subtotal,
                        SubTotal = true
                    });

                    Total += Subtotal;
                }
            }

            if (balance.Items.Count != 0)
            {
                balance.total = Total;
            }

            return balance;
        }

        //public static BalanceComisionDiarioViewModel getBalanceComision(List<FormaPago> FormasPago, List<Comision> comisiones, List<Pago> CuentasCorrientePagos, List<Gasto> comisionGasto)
        //{

        //    BalanceComisionDiarioViewModel balance = new BalanceComisionDiarioViewModel();

        //    List<IGrouping<FormaPago, Comision>> comisionesPorFormaPago = comisiones.GroupBy(x => x.FormaPago).ToList();
        //    List<IGrouping<FormaPago, Pago>> cuentasCorrientePagosPorFormaPago = CuentasCorrientePagos.GroupBy(x => x.FormaPago).ToList();
        //    List<IGrouping<TipoGasto, Gasto>> gastosPorTipo = comisionGasto.GroupBy(x => x.TipoGasto).ToList();

        //    foreach (FormaPago formaPago in FormasPago)
        //    {
        //        decimal montoTotal = 0;
        //        IGrouping<FormaPago, Comision> comisionesGoup = comisionesPorFormaPago.FirstOrDefault(x => x.Key.ID == formaPago.ID);
        //        if(comisionesGoup != null)
        //            montoTotal += comisionesGoup.Sum(x => x.Costo);

        //        IGrouping<FormaPago, Pago> PagoGoup = cuentasCorrientePagosPorFormaPago.FirstOrDefault(x => x.Key.ID == formaPago.ID);
        //        if (PagoGoup != null)
        //            montoTotal += PagoGoup.Sum(x => x.Monto);

        //        if (montoTotal > 0)
        //        {
        //            balance.Comisiones.Add(new ItemBalanceComisionViewModel()
        //            {
        //                Concepto = $"Total {formaPago.Descripcion} {montoTotal}",
        //                Monto = montoTotal
        //            });
        //        }
        //    }

        //    balance.totalComision = balance.Comisiones.Sum(x => x.Monto);

        //    foreach (IGrouping<TipoGasto, Gasto> gastoPorTipo in gastosPorTipo)
        //    {
        //        balance.Gastos.Add(new ItemBalanceComisionViewModel()
        //        {
        //            Concepto = $"Total Gasto {gastoPorTipo.Key.Descripcion}",
        //            Monto = -gastoPorTipo.Sum(x => x.Monto)
        //        });
        //    }

        //    balance.totalGasto = -comisionGasto.Sum(x => x.Monto);

        //    balance.total = balance.totalComision + balance.totalGasto;

        //    return balance;
        //}

        public static List<Viaje> getViajes(List<Viaje> viajes, DateTime fecha)
        {
            return viajes.Where(x => x.FechaArribo.Day == fecha.Day && x.FechaArribo.Month == fecha.Month && x.FechaArribo.Year == fecha.Year && x.Estado == ViajeEstados.Cerrado).ToList<Viaje>();
        }

        public static BalanceVendedorDiarioViewModel getBalanceVendedor(List<FormaPago> formasPago, List<ClienteViaje> clienteViaje, List<Gasto> Gastos, String[] Rol, String Usuario)
        {
            BalanceVendedorDiarioViewModel balance = new BalanceVendedorDiarioViewModel();
            List<IGrouping<string, ClienteViaje>> gruposCobrosPorVendedor;
            List<IGrouping<string, Gasto>> gruposGastosPorVendedor;
            List<UsuarioViewModel> usuarios = new List<UsuarioViewModel>();

            if (Rol.Contains("Administrador"))
            {
                gruposCobrosPorVendedor = clienteViaje.GroupBy(x => x.VendedorCobro.ToUpper()).ToList();
                gruposGastosPorVendedor = Gastos.GroupBy(x => x.UsuarioAlta.ToUpper()).ToList();
                usuarios = UsuarioHelper.getUsuarios();
            }
            else
            {
                gruposCobrosPorVendedor = clienteViaje.Where(x => x.VendedorCobro.ToUpper() == Usuario).GroupBy(x => x.VendedorCobro.ToUpper()).ToList();
                gruposGastosPorVendedor = Gastos.Where(x => x.UsuarioAlta.ToUpper() == Usuario).GroupBy(x => x.UsuarioAlta.ToUpper()).ToList();
                usuarios.Add(new UsuarioViewModel() { Usuario = Usuario });
            }

            foreach (UsuarioViewModel vendedor in usuarios)
            {
                decimal Total = 0;
                BalanceVendedorViewModel balanceVendedor = new BalanceVendedorViewModel()
                {
                    Concepto = string.Format("Usuario: {0}", vendedor.Usuario.ToUpper()),
                    Items = new List<ItemBalanceVendedorViewModel>()
                };

                foreach (FormaPago formaPago in formasPago)
                {
                    decimal Subtotal = 0;
                    List<IGrouping<string, Gasto>> gastosPorVendedor = new List<IGrouping<string, Gasto>>();
                    if (formaPago.Descripcion.ToUpper() == "Efectivo".ToUpper())
                        gastosPorVendedor = gruposGastosPorVendedor.Where(x => x.Key.ToUpper() == vendedor.Usuario.ToUpper()).ToList();

                    List<IGrouping<string, ClienteViaje>> cobrosPorVendedor = gruposCobrosPorVendedor.Where(x => x.Key.ToUpper() == vendedor.Usuario.ToUpper()).ToList();

                    foreach (IGrouping<string, ClienteViaje> cobroPorVendedor in cobrosPorVendedor)
                    {
                        List<ClienteViaje> cobrosPorFormaPago = cobroPorVendedor.Where(x => x.FormaPago?.Descripcion.ToUpper() == formaPago?.Descripcion.ToUpper()).ToList();

                        foreach (ClienteViaje cobro in cobrosPorFormaPago)
                        {
                            balanceVendedor.Items.Add(new ItemBalanceVendedorViewModel()
                            {
                                Concepto = string.Format("{0} {1} - Cod. {2} Fecha {3}  Serv. {4}", cobro.Cliente.Apellido, cobro.Cliente.Nombre, cobro.Viaje?.ID, cobro.Viaje?.FechaSalida, cobro.Viaje?.Servicio),
                                Monto = cobro.Costo
                            });
                        }

                        Subtotal += cobrosPorFormaPago.Sum(x => x.Costo);
                    }

                    foreach (IGrouping<string, Gasto> gastoPorVendedor in gastosPorVendedor)
                    {
                        foreach (Gasto gasto in gastoPorVendedor)
                        {
                            balanceVendedor.Items.Add(new ItemBalanceVendedorViewModel()
                            {
                                Concepto = string.Format("Gastos {0} {1} {2}", gasto.Concepto, gasto.TipoGasto.Descripcion, gasto.Comentario),
                                Monto = -gasto.Monto
                            });
                        }

                        Subtotal -= gastoPorVendedor.Sum(x => x.Monto);
                    }

                    if (Subtotal != 0)
                    {
                        balanceVendedor.Items.Add(new ItemBalanceVendedorViewModel()
                        {
                            Concepto = string.Format("Sub Total {0}", formaPago.Descripcion),
                            Monto = Subtotal,
                            SubTotal = true
                        });
                    }

                    Total += Subtotal;
                }

                if (balanceVendedor.Items.Count != 0)
                {
                    balanceVendedor.total = Total;
                    balance.BalanceVendedor.Add(balanceVendedor);
                }
            }

            return balance;
        }

        public static BalanceVendedorDiarioViewModel getBalanceVendedor2(List<ClienteViaje> clienteViaje, List<Gasto> Gastos, String[] Rol, String Usuario)
        {
            BalanceVendedorDiarioViewModel balance = new BalanceVendedorDiarioViewModel();
            List<IGrouping<string, ClienteViaje>> gruposCobrosPorVendedor;
            List<IGrouping<string, Gasto>> gruposGastosPorVendedor;
            List<UsuarioViewModel> usuarios = new List<UsuarioViewModel>();

            if (Rol.Contains("Administrador"))
            {
                gruposCobrosPorVendedor = clienteViaje.GroupBy(x => x.VendedorCobro.ToUpper()).ToList();
                gruposGastosPorVendedor = Gastos.GroupBy(x => x.UsuarioAlta.ToUpper()).ToList();
                usuarios = UsuarioHelper.getUsuarios();
            }
            else
            {
                gruposCobrosPorVendedor = clienteViaje.Where(x => x.VendedorCobro.ToUpper() == Usuario).GroupBy(x => x.VendedorCobro.ToUpper()).ToList();
                gruposGastosPorVendedor = Gastos.Where(x => x.UsuarioAlta.ToUpper() == Usuario).GroupBy(x => x.UsuarioAlta.ToUpper()).ToList();
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
                    Concepto = string.Format("Usuario: {0}", vendedor.Usuario.ToUpper()),
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

                        if (pagos.Key?.Descripcion == "Efectivo" && gastoPorVendedor != null)
                        {
                            foreach (var gasto in gastoPorVendedor)
                            {
                                balanceVendedor.Items.Add(new ItemBalanceVendedorViewModel()
                                {
                                    Concepto = string.Format("Gastos {0} {1} {2}", gasto.Concepto, gasto.TipoGasto.Descripcion, gasto.Comentario),
                                    Monto = -gasto.Monto
                                });
                            }

                            decimal monto = (pagos != null ? pagos.Sum(x => x.Costo) : 0) - (gastoPorVendedor != null ? gastoPorVendedor.Sum(x => x.Monto) : 0);
                            balanceVendedor.Items.Add(new ItemBalanceVendedorViewModel()
                            {
                                Concepto = string.Format("Sub Total {0}", pagos.Key != null ? pagos.Key.Descripcion : string.Empty),
                                Monto = monto,
                                SubTotal = true
                            });
                            balanceVendedor.total += monto;
                        }
                        else
                        {
                            decimal monto = (pagos != null ? pagos.Sum(x => x.Costo) : 0);
                            balanceVendedor.Items.Add(new ItemBalanceVendedorViewModel()
                            {
                                Concepto = string.Format("Sub Total {0}", pagos.Key != null ? pagos.Key.Descripcion : string.Empty),
                                Monto = monto,
                                SubTotal = true
                            });
                            balanceVendedor.total += monto;
                        }
                    }
                }

                if (cobroPorVendedor == null  && gastoPorVendedor != null)
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

        public static List<CuentaCorriente> ObtenerPago(List<CuentaCorriente> cuentasCorrientes, DateTime fechaDesde, DateTime fechaHasta)
        {
            List<CuentaCorriente> resultado = new List<CuentaCorriente>();

            foreach (CuentaCorriente cuenta in cuentasCorrientes)
            {
                List<Pago> pagos = cuenta.Pagos.Where(y => y.Fecha.Date.CompareTo(fechaDesde.Date) >= 0 && y.Fecha.Date.CompareTo(fechaHasta.Date) <= 0).ToList();
                if (pagos.Count > 0)
                    resultado.Add(new CuentaCorriente()
                    {
                        RazonSocial = cuenta.RazonSocial,
                        Pagos = pagos
                    });
            }

            return resultado;
        }
    }
}