using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAV.Models;
using SAV.Common;
using System.Configuration;

namespace SAV.Controllers
{
    [Authorize]
    public class BalanceController : Controller
    {
        private SAVContext db = new SAVContext();

        public ActionResult Viaje()
        {
            BalanceViajeDiarioViewModel balanceViewModel = new BalanceViajeDiarioViewModel();

            return View(balanceViewModel);
        }

        [HttpPost]
        public ActionResult Viaje(BalanceViajeDiarioViewModel balanceViewModel)
        {
            if (!balanceViewModel.Clave.Equals(ConfigurationManager.AppSettings["ClaveReporte"]))
            {
                ModelState.AddModelError("", "La clave ingresada es incorrecta.");
                return View(balanceViewModel);
            }
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

        public ActionResult Comision()
        {
            BalanceComisionDiarioViewModel balanceComisionDiarioViewModel = new BalanceComisionDiarioViewModel();

            return View(balanceComisionDiarioViewModel);
        }

        [HttpPost]
        public ActionResult Comision(BalanceComisionDiarioViewModel balanceComisionDiarioViewModel)
        {
            if (!balanceComisionDiarioViewModel.Clave.Equals(ConfigurationManager.AppSettings["ClaveReporte"]))
            {
                ModelState.AddModelError("", "La clave ingresada es incorrecta.");
                return View(balanceComisionDiarioViewModel);
            }
            DateTime fecha = ViajeHelper.getFecha(balanceComisionDiarioViewModel.Fecha);
            if (string.IsNullOrEmpty(balanceComisionDiarioViewModel.FechaHasta))
            {
                List<Comision> comision = BalanceHelper.getComisiones(db.Comisiones.ToList<Comision>(), fecha);
                List<ComisionGasto> comisionGastos = BalanceHelper.getComisionesGastos(db.ComisionGastos.ToList<ComisionGasto>(), fecha);
                balanceComisionDiarioViewModel = BalanceHelper.getBalanceComision(comision, comisionGastos);
            }
            else
            {
                DateTime fechaHasta = ViajeHelper.getFecha(balanceComisionDiarioViewModel.FechaHasta);
                List<Comision> comision = db.Comisiones.Where(x => x.FechaPago.HasValue && x.FechaPago.Value.CompareTo(fecha) >= 0 && x.FechaPago.Value.CompareTo(fechaHasta) <=0).ToList<Comision>();
                List<ComisionGasto> comisionGastos = db.ComisionGastos.Where(x => x.FechaAlta.CompareTo(fecha) >= 0 && x.FechaAlta.CompareTo(fechaHasta) <= 0).ToList<ComisionGasto>();
                balanceComisionDiarioViewModel = BalanceHelper.getBalanceComision(comision, comisionGastos);

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
                List<Comision> comision = BalanceHelper.getComisiones(db.Comisiones.ToList<Comision>(), fecha);
                List<ComisionGasto> comisionGastos = BalanceHelper.getComisionesGastos(db.ComisionGastos.ToList<ComisionGasto>(), fecha);
                balanceComisionDiarioViewModel = BalanceHelper.getBalanceComision(comision, comisionGastos);

                name = String.Format("Reporte_Comisiones_Dia_{0}", fecha.ToString("dd_MM_yyyy"));
                @ViewBag.titulo = string.Format("Comisiones día {0}", fecha.ToString("dd/MM/yyyy"));
            }
            else
            {
                DateTime fechaHasta = ViajeHelper.getFecha(fechaHastaBusqueda);
                List<Comision> comision = db.Comisiones.Where(x => x.FechaPago.HasValue && x.FechaPago.Value.CompareTo(fecha) >= 0 && x.FechaPago.Value.CompareTo(fechaHasta) <= 0).ToList<Comision>();
                List<ComisionGasto> comisionGastos = db.ComisionGastos.Where(x => x.FechaAlta.CompareTo(fecha) >= 0 && x.FechaAlta.CompareTo(fechaHasta) <= 0).ToList<ComisionGasto>();
                balanceComisionDiarioViewModel = BalanceHelper.getBalanceComision(comision, comisionGastos);

                name = String.Format("Reporte_Comisiones_Dia_{0}_{1}", fecha.ToString("dd_MM_yyyy"), fechaHasta.ToString("dd_MM_yyyy"));
                @ViewBag.titulo = string.Format("Consolidado Comisiones desde {0} hasta {1}", fecha.ToString("dd/MM/yyyy"), fechaHasta.ToString("dd/MM/yyyy"));
            }

            @ViewBag.fecha = fecha.ToString("dd/MM/yyyy");

            Response.AddHeader("content-disposition", "attachment;filename=" + name + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            Response.Charset = "utf-8";

            return View(balanceComisionDiarioViewModel);
        }
    }
}
