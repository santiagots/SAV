using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAV.Models;
using SAV.Common;

namespace SAV.Controllers
{
    [Authorize]
    public class BalanceController : Controller
    {
        private SAVContext db = new SAVContext();

        public ActionResult Balance()
        {
            BalanceViewModel balanceViewModel = new BalanceViewModel();

            return View(balanceViewModel);
        }

        [HttpPost]
        public ActionResult Balance(BalanceViewModel balanceViewModel)
        {
            DateTime fecha = ViajeHelper.getFecha(balanceViewModel.Fecha);

            List<Viaje> viajes = ViajeHelper.getViajesBalance(db.Viajes.ToList<Viaje>(), fecha);

            balanceViewModel = BalanceHelper.getBalance(viajes);

            return View(balanceViewModel);
        }

        public ActionResult ExportBalance(String fechaBusqueda)
        {
            DateTime fecha = ViajeHelper.getFecha(fechaBusqueda);

            List<Viaje> viajes = ViajeHelper.getViajesBalance(db.Viajes.ToList<Viaje>(), fecha);

            BalanceViewModel balanceViewModel = BalanceHelper.getBalance(viajes);

            @ViewBag.fecha = fecha.ToString("dd/MM/yyyy");

            string name = String.Format("Reporte_Dia_{0}", fecha.ToString("dd_MM_yyyy"));

            Response.AddHeader("content-disposition", "attachment;filename=" + name + ".xls");
            Response.ContentType = "application/vnd.ms-excel";
            Response.Charset = "utf-8";

            return View(balanceViewModel);
        }
    }
}
