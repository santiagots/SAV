using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SAV.Models;
using SAV.Common;
using System.Web.Security;
using System.Data.Entity;

namespace SAV.Controllers
{
    [Authorize]
    public class BalanceController : Controller
    {
        private SAVContext db = new SAVContext();

        public ActionResult CierreCaja()
        {
            CierreCajaViewModel balanceViewModel = new CierreCajaViewModel();

            return View(balanceViewModel);
        }

        [HttpPost]
        public ActionResult CierreCaja(CierreCajaViewModel cierreCajaViewModel)
        {
            DateTime fecha = ViajeHelper.getFecha(cierreCajaViewModel.Fecha);
            if (string.IsNullOrEmpty(cierreCajaViewModel.FechaHasta))
            {
                List<ClienteViaje> ClienteViaje = db.ClienteViajes.Where(x => DbFunctions.TruncateTime(x.FechaPago).Value == fecha.Date && x.Pago).ToList<ClienteViaje>();
                List<Viaje> viajes = BalanceHelper.getViajes(db.Viajes.ToList<Viaje>(), fecha);
                List<Comision> Comisiones = db.Comisiones.Where(x => x.CuentaCorriente == null && x.FechaPago.HasValue && DbFunctions.TruncateTime(x.FechaPago).Value == fecha.Date).ToList();
                List<CuentaCorriente> CuentasCorrientes = db.CuentaCorriente.Where(x => x.Pagos.Any(y => DbFunctions.TruncateTime(y.Fecha).Value == fecha.Date)).ToList();
                List<Gasto> comisionGastos = db.Gastos.Where(x => DbFunctions.TruncateTime(x.FechaAlta).Value == fecha.Date).ToList();
                cierreCajaViewModel = BalanceHelper.getBalanceCierreCaja(ClienteViaje, viajes, Comisiones, CuentasCorrientes, comisionGastos, fecha, null);
            }
            else
            {
                DateTime fechaHasta = ViajeHelper.getFecha(cierreCajaViewModel.FechaHasta);
                List<ClienteViaje> ClienteViaje = db.ClienteViajes.Where(x => DbFunctions.TruncateTime(x.FechaPago).Value.CompareTo(fecha) >= 0 && DbFunctions.TruncateTime(x.FechaPago).Value.CompareTo(fechaHasta) <= 0 && x.Pago).ToList<ClienteViaje>();
                List<Viaje> Viajes = db.Viajes.Where(x => x.FechaArribo.CompareTo(fecha) >= 0 && x.FechaArribo.CompareTo(fechaHasta) <= 0 && x.Estado == ViajeEstados.Cerrado).ToList<Viaje>();
                List<Comision> Comisiones = db.Comisiones.Where(x => x.CuentaCorriente == null && x.FechaPago.HasValue && DbFunctions.TruncateTime(x.FechaPago).Value.CompareTo(fecha) >= 0 && DbFunctions.TruncateTime(x.FechaPago).Value.CompareTo(fechaHasta) <= 0).ToList();
                List<CuentaCorriente> CuentasCorrientes = db.CuentaCorriente.Where(x => x.Pagos.Any(y => DbFunctions.TruncateTime(y.Fecha).Value.CompareTo(fecha.Date) >= 0 && DbFunctions.TruncateTime(y.Fecha).Value.CompareTo(fechaHasta.Date) <= 0)).ToList();
                List<Gasto> comisionGastos = db.Gastos.Where(x => DbFunctions.TruncateTime(x.FechaAlta).Value.CompareTo(fecha) >= 0 && DbFunctions.TruncateTime(x.FechaAlta).Value.CompareTo(fechaHasta) <= 0).ToList<Gasto>();
                cierreCajaViewModel = BalanceHelper.getBalanceCierreCaja(ClienteViaje, Viajes, Comisiones, CuentasCorrientes, comisionGastos, fecha, fechaHasta);
            }

            return View(cierreCajaViewModel);
        }

        public ActionResult Viaje()
        {
            BalanceViajeDiarioViewModel balanceViewModel = new BalanceViajeDiarioViewModel();

            return View(balanceViewModel);
        }

        [HttpPost]
        public ActionResult Viaje(BalanceViajeDiarioViewModel balanceViewModel)
        {
            DateTime fecha = ViajeHelper.getFecha(balanceViewModel.Fecha);

            if (string.IsNullOrEmpty(balanceViewModel.FechaHasta))
            {
                List<Viaje> viajes = BalanceHelper.getViajes(db.Viajes.ToList<Viaje>(), fecha);
                balanceViewModel = BalanceHelper.getBalanceViaje(viajes);
            }
            else
            {
                DateTime fechaHasta = ViajeHelper.getFecha(balanceViewModel.FechaHasta);
                List<Viaje> viajes = db.Viajes.Where(x => x.FechaArribo.CompareTo(fecha) >= 0 && x.FechaArribo.CompareTo(fechaHasta) <= 0 && x.Estado == ViajeEstados.Cerrado).ToList<Viaje>();
                balanceViewModel = BalanceHelper.getBalanceViajeConsolidado(viajes);
            }

            return View(balanceViewModel);
        }

        public ActionResult Vendedor()
        {
            BalanceVendedorDiarioViewModel balanceViewModel = new BalanceVendedorDiarioViewModel();

            return View(balanceViewModel);
        }

        [HttpPost]
        public ActionResult Vendedor(BalanceVendedorDiarioViewModel balanceViewModel)
        {
            DateTime fecha = ViajeHelper.getFecha(balanceViewModel.Fecha);
            List<ClienteViaje> ClienteViaje = new List<Models.ClienteViaje>();
            List<Gasto> Gastos = new List<Gasto>();

            if (string.IsNullOrEmpty(balanceViewModel.FechaHasta))
            {
               ClienteViaje = db.ClienteViajes.Where(x => x.Pago && DbFunctions.TruncateTime(x.FechaPago).Value == fecha.Date).ToList<ClienteViaje>();
               Gastos = db.Gastos.Where(x => x.Concepto == ConceptoGasto.Viaje && DbFunctions.TruncateTime(x.FechaAlta).Value == fecha.Date && x.viaje == null).ToList();
            }
            else
            {
                DateTime fechaHasta = ViajeHelper.getFecha(balanceViewModel.FechaHasta);
                ClienteViaje = db.ClienteViajes.Where(x => x.Pago && DbFunctions.TruncateTime(x.FechaPago).Value.CompareTo(fecha) >= 0 && DbFunctions.TruncateTime(x.FechaPago).Value.CompareTo(fechaHasta) <= 0).ToList<ClienteViaje>();
                Gastos = db.Gastos.Where(x => x.viaje == null && x.Concepto == ConceptoGasto.Viaje && DbFunctions.TruncateTime(x.FechaAlta).Value.CompareTo(fecha) >= 0 && DbFunctions.TruncateTime(x.FechaAlta).Value.CompareTo(fechaHasta) <= 0).ToList<Gasto>();
            }

            balanceViewModel = BalanceHelper.getBalanceVendedor(ClienteViaje, Gastos, Roles.GetRolesForUser(User.Identity.Name), User.Identity.Name);

            return View(balanceViewModel);
        }

        public ActionResult Comision()
        {
            BalanceComisionDiarioViewModel balanceComisionDiarioViewModel = new BalanceComisionDiarioViewModel();

            return View(balanceComisionDiarioViewModel);
        }

        [HttpPost]
        public ActionResult Comision(BalanceComisionDiarioViewModel balanceComisionDiarioViewModel)
        {
            DateTime fecha = ViajeHelper.getFecha(balanceComisionDiarioViewModel.Fecha);
            if (string.IsNullOrEmpty(balanceComisionDiarioViewModel.FechaHasta))
            {
                List<Comision> Comisiones = db.Comisiones.Where(x => x.CuentaCorriente == null && x.FechaPago.HasValue && DbFunctions.TruncateTime(x.FechaPago).Value == fecha.Date).ToList();
                List<CuentaCorriente> CuentasCorrientes = db.CuentaCorriente.Where(x => x.Pagos.Any(y => DbFunctions.TruncateTime(y.Fecha).Value == fecha.Date)).ToList();
                List<Gasto> comisionGastos = db.Gastos.Where(x => x.viaje == null && x.Concepto == ConceptoGasto.Comision && DbFunctions.TruncateTime(x.FechaAlta).Value == fecha.Date).ToList();
                balanceComisionDiarioViewModel = BalanceHelper.getBalanceComision(Comisiones, CuentasCorrientes, comisionGastos, fecha);
            }
            else
            {
                DateTime fechaHasta = ViajeHelper.getFecha(balanceComisionDiarioViewModel.FechaHasta);
                List<Comision> Comisiones = db.Comisiones.Where(x => x.CuentaCorriente == null && x.FechaPago.HasValue && DbFunctions.TruncateTime(x.FechaPago).Value.CompareTo(fecha) >= 0 && DbFunctions.TruncateTime(x.FechaPago).Value.CompareTo(fechaHasta) <= 0).ToList();
                List<CuentaCorriente> CuentasCorrientes = db.CuentaCorriente.Where(x => x.Pagos.Any(y => DbFunctions.TruncateTime(y.Fecha).Value.CompareTo(fecha.Date) >= 0 && DbFunctions.TruncateTime(y.Fecha).Value.CompareTo(fechaHasta.Date) <= 0)).ToList();
                List<Gasto> comisionGastos = db.Gastos.Where(x => x.viaje == null && x.Concepto == ConceptoGasto.Comision && DbFunctions.TruncateTime(x.FechaAlta).Value.CompareTo(fecha) >= 0 && DbFunctions.TruncateTime(x.FechaAlta).Value.CompareTo(fechaHasta) <= 0).ToList<Gasto>();
                balanceComisionDiarioViewModel = BalanceHelper.getBalanceComision(Comisiones, CuentasCorrientes, comisionGastos, fecha, fechaHasta);

            }

            return View(balanceComisionDiarioViewModel);
        }

        public ActionResult ExportViaje(String fechaBusqueda, String fechaHastaBusqueda)
        {
            DateTime fecha = ViajeHelper.getFecha(fechaBusqueda);
            BalanceViajeDiarioViewModel balanceViewModel;
            string name;

            if (string.IsNullOrEmpty(fechaHastaBusqueda))
            {
                List<Viaje> viajes = BalanceHelper.getViajes(db.Viajes.ToList<Viaje>(), fecha);
                balanceViewModel = BalanceHelper.getBalanceViaje(viajes);
                name = String.Format("Reporte_Dia_Viajes{0}", fecha.ToString("dd_MM_yyyy"));
                @ViewBag.titulo = string.Format("Cierre día {0}", fecha.ToString("dd/MM/yyyy"));
            }

            else
            {
                DateTime fechaHasta = ViajeHelper.getFecha(fechaHastaBusqueda);
                List<Viaje> viajes = db.Viajes.Where(x => x.FechaArribo.CompareTo(fecha) >= 0 && x.FechaArribo.CompareTo(fechaHasta) <= 0 && x.Estado == ViajeEstados.Cerrado).ToList<Viaje>();
                balanceViewModel = BalanceHelper.getBalanceViajeConsolidado(viajes);
                name = String.Format("Consolidado_Viajes{0}_{1}", fecha.ToString("dd_MM_yyyy"), fechaHasta.ToString("dd_MM_yyyy"));
                @ViewBag.titulo = string.Format("Consolidado Viajes desde {0} hasta {1}", fecha.ToString("dd/MM/yyyy"), fechaHasta.ToString("dd/MM/yyyy"));
            }

            Response.AddHeader("content-disposition", "attachment;filename=" + name + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            Response.Charset = "utf-8";

            return View(balanceViewModel);
        }

        public ActionResult ExportComisiones(String fechaBusqueda, String fechaHastaBusqueda)
        {
            DateTime fecha = ViajeHelper.getFecha(fechaBusqueda);
            BalanceComisionDiarioViewModel balanceComisionDiarioViewModel = new BalanceComisionDiarioViewModel();
            string name;

            if (string.IsNullOrEmpty(fechaHastaBusqueda))
            {
                List<Comision> Comisiones = db.Comisiones.Where(x => x.CuentaCorriente == null && x.FechaPago.HasValue && DbFunctions.TruncateTime(x.FechaPago).Value == fecha.Date).ToList();
                List<CuentaCorriente> CuentasCorrientes = db.CuentaCorriente.Where(x => x.Pagos.Any(y => DbFunctions.TruncateTime(y.Fecha).Value == fecha.Date)).ToList();
                List<Gasto> comisionGastos = db.Gastos.Where(x => x.viaje == null && x.Concepto == ConceptoGasto.Comision && DbFunctions.TruncateTime(x.FechaAlta).Value == fecha.Date).ToList();
                balanceComisionDiarioViewModel = BalanceHelper.getBalanceComision(Comisiones, CuentasCorrientes, comisionGastos, fecha);

                name = String.Format("Reporte_Comisiones_Dia_{0}", fecha.ToString("dd_MM_yyyy"));
                @ViewBag.titulo = string.Format("Comisiones día {0}", fecha.ToString("dd/MM/yyyy"));
            }
            else
            {
                DateTime fechaHasta = ViajeHelper.getFecha(fechaHastaBusqueda);
                List<Comision> Comisiones = db.Comisiones.Where(x => x.CuentaCorriente == null && x.FechaPago.HasValue && DbFunctions.TruncateTime(x.FechaPago).Value.CompareTo(fecha) >= 0 && DbFunctions.TruncateTime(x.FechaPago).Value.CompareTo(fechaHasta) <= 0).ToList();
                List<CuentaCorriente> CuentasCorrientes = db.CuentaCorriente.Where(x => x.Pagos.Any(y => DbFunctions.TruncateTime(y.Fecha).Value.CompareTo(fecha.Date) >= 0 && DbFunctions.TruncateTime(y.Fecha).Value.CompareTo(fechaHasta.Date) <= 0)).ToList();
                List<Gasto> comisionGastos = db.Gastos.Where(x => x.viaje == null && x.Concepto == ConceptoGasto.Comision && DbFunctions.TruncateTime(x.FechaAlta).Value.CompareTo(fecha) >= 0 && DbFunctions.TruncateTime(x.FechaAlta).Value.CompareTo(fechaHasta) <= 0).ToList<Gasto>();
                balanceComisionDiarioViewModel = BalanceHelper.getBalanceComision(Comisiones, CuentasCorrientes, comisionGastos, fecha, fechaHasta);

                name = String.Format("Reporte_Comisiones_Dia_{0}_{1}", fecha.ToString("dd_MM_yyyy"), fechaHasta.ToString("dd_MM_yyyy"));
                @ViewBag.titulo = string.Format("Consolidado Comisiones desde {0} hasta {1}", fecha.ToString("dd/MM/yyyy"), fechaHasta.ToString("dd/MM/yyyy"));
            }

            @ViewBag.fecha = fecha.ToString("dd/MM/yyyy");

            Response.AddHeader("content-disposition", "attachment;filename=" + name + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            Response.Charset = "utf-8";

            return View(balanceComisionDiarioViewModel);
        }

        public ActionResult ExportCierreCaja(String fechaBusqueda, String fechaHastaBusqueda)
        {
            DateTime fecha = ViajeHelper.getFecha(fechaBusqueda);
            CierreCajaViewModel cierreCajaViewModel = new CierreCajaViewModel();
            string name;

            if (string.IsNullOrEmpty(fechaHastaBusqueda))
            {
                List<ClienteViaje> ClienteViaje = db.ClienteViajes.Where(x => DbFunctions.TruncateTime(x.FechaPago).Value == fecha.Date && x.Pago).ToList<ClienteViaje>();
                List<Viaje> viajes = BalanceHelper.getViajes(db.Viajes.ToList<Viaje>(), fecha);
                List<Comision> Comisiones = db.Comisiones.Where(x => x.CuentaCorriente == null && x.FechaPago.HasValue && DbFunctions.TruncateTime(x.FechaPago).Value == fecha.Date).ToList();
                List<CuentaCorriente> CuentasCorrientes = db.CuentaCorriente.Where(x => x.Pagos.Any(y => DbFunctions.TruncateTime(y.Fecha).Value == fecha.Date)).ToList();
                List<Gasto> comisionGastos = db.Gastos.Where(x => DbFunctions.TruncateTime(x.FechaAlta).Value == fecha.Date).ToList();
                cierreCajaViewModel = BalanceHelper.getBalanceCierreCaja(ClienteViaje, viajes, Comisiones, CuentasCorrientes, comisionGastos, fecha, null);

                name = String.Format("Reporte_Cierre_Caja_{0}", fecha.ToString("dd_MM_yyyy"));
                @ViewBag.titulo = string.Format("Cierre Caja día {0}", fecha.ToString("dd/MM/yyyy"));
            }
            else
            {
                DateTime fechaHasta = ViajeHelper.getFecha(fechaHastaBusqueda);
                List<ClienteViaje> ClienteViaje = db.ClienteViajes.Where(x => DbFunctions.TruncateTime(x.FechaPago).Value.CompareTo(fecha) >= 0 && DbFunctions.TruncateTime(x.FechaPago).Value.CompareTo(fechaHasta) <= 0 && x.Pago).ToList<ClienteViaje>();
                List<Viaje> Viajes = db.Viajes.Where(x => x.FechaArribo.CompareTo(fecha) >= 0 && x.FechaArribo.CompareTo(fechaHasta) <= 0 && x.Estado == ViajeEstados.Cerrado).ToList<Viaje>();
                List<Comision> Comisiones = db.Comisiones.Where(x => x.CuentaCorriente == null && x.FechaPago.HasValue && DbFunctions.TruncateTime(x.FechaPago).Value.CompareTo(fecha) >= 0 && DbFunctions.TruncateTime(x.FechaPago).Value.CompareTo(fechaHasta) <= 0).ToList();
                List<CuentaCorriente> CuentasCorrientes = db.CuentaCorriente.Where(x => x.Pagos.Any(y => DbFunctions.TruncateTime(y.Fecha).Value.CompareTo(fecha.Date) >= 0 && DbFunctions.TruncateTime(y.Fecha).Value.CompareTo(fechaHasta.Date) <= 0)).ToList();
                List<Gasto> comisionGastos = db.Gastos.Where(x => DbFunctions.TruncateTime(x.FechaAlta).Value.CompareTo(fecha) >= 0 && DbFunctions.TruncateTime(x.FechaAlta).Value.CompareTo(fechaHasta) <= 0).ToList<Gasto>();
                cierreCajaViewModel = BalanceHelper.getBalanceCierreCaja(ClienteViaje, Viajes, Comisiones, CuentasCorrientes, comisionGastos, fecha, fechaHasta);

                name = String.Format("Reporte_Cierre_Caja_{0}_{1}", fecha.ToString("dd_MM_yyyy"), fechaHasta.ToString("dd_MM_yyyy"));
                @ViewBag.titulo = string.Format("Consolidado Cierre Caja desde {0} hasta {1}", fecha.ToString("dd/MM/yyyy"), fechaHasta.ToString("dd/MM/yyyy"));
            }

            @ViewBag.fecha = fecha.ToString("dd/MM/yyyy");

            Response.AddHeader("content-disposition", "attachment;filename=" + name + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            Response.Charset = "utf-8";

            return View(cierreCajaViewModel);
        }

        public ActionResult ExportVendedor(String fechaBusqueda, String fechaHastaBusqueda)
        {
            DateTime fecha = ViajeHelper.getFecha(fechaBusqueda);
            BalanceVendedorDiarioViewModel balanceViewModel;
            List<ClienteViaje> ClienteViaje = new List<Models.ClienteViaje>();
            List<Gasto> Gastos = new List<Gasto>();

            string name;

            if (string.IsNullOrEmpty(fechaHastaBusqueda))
            {
                ClienteViaje = db.ClienteViajes.Where(x => x.Pago && DbFunctions.TruncateTime(x.FechaPago).Value == fecha.Date).ToList<ClienteViaje>();
                Gastos = db.Gastos.Where(x => x.Concepto == ConceptoGasto.Viaje && DbFunctions.TruncateTime(x.FechaAlta).Value == fecha.Date && x.viaje == null).ToList();
                name = String.Format("Reporte_Dia_Viajes{0}", fecha.ToString("dd_MM_yyyy"));
                @ViewBag.titulo = string.Format("Cierre día {0}", fecha.ToString("dd/MM/yyyy"));
            }

            else
            {
                DateTime fechaHasta = ViajeHelper.getFecha(fechaHastaBusqueda);
                ClienteViaje = db.ClienteViajes.Where(x => x.Pago && DbFunctions.TruncateTime(x.FechaPago).Value.CompareTo(fecha) >= 0 && DbFunctions.TruncateTime(x.FechaPago).Value.CompareTo(fechaHasta) <= 0).ToList<ClienteViaje>();
                Gastos = db.Gastos.Where(x => x.viaje == null && x.Concepto == ConceptoGasto.Viaje && DbFunctions.TruncateTime(x.FechaAlta).Value.CompareTo(fecha) >= 0 && DbFunctions.TruncateTime(x.FechaAlta).Value.CompareTo(fechaHasta) <= 0).ToList<Gasto>();
                name = String.Format("Consolidado_Vendedores{0}_{1}", fecha.ToString("dd_MM_yyyy"), fechaHasta.ToString("dd_MM_yyyy"));
                @ViewBag.titulo = string.Format("Consolidado Vendedores desde {0} hasta {1}", fecha.ToString("dd/MM/yyyy"), fechaHasta.ToString("dd/MM/yyyy"));
            }

            balanceViewModel = BalanceHelper.getBalanceVendedor(ClienteViaje, Gastos, Roles.GetRolesForUser(User.Identity.Name), User.Identity.Name);

            Response.AddHeader("content-disposition", "attachment;filename=" + name + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            Response.Charset = "utf-8";

            return View(balanceViewModel);
        }
    }
}
