using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using PagedList;
using SAV.Common;
using SAV.Models;
using SAV.Models.CuentaCorrienteViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAV.Controllers
{
    public class CuentaCorrienteController : Controller
    {
        private SAVContext db = new SAVContext();

        public ActionResult GetLocalidades(int IdProvincia)
        {
            Provincia provincia = db.Provincias.Where(x => x.ID == IdProvincia).FirstOrDefault();

            if (HttpContext.Request.IsAjaxRequest())
                return Json(new SelectList(provincia.Localidad.OrderBy(x => x.Nombre), "ID", "Nombre"), JsonRequestBehavior.AllowGet);

            return View(provincia);
        }

        public ActionResult Create()
        {
            List<Provincia> provincias = db.Provincias.ToList();
            CuentaCorrienteViewModel cuentaCorrienteViewModel = new CuentaCorrienteViewModel(provincias);

            return View(cuentaCorrienteViewModel);
        }

        [HttpPost]
        public ActionResult Create(CuentaCorrienteViewModel cuentaCorrienteViewModel)
        {
            List<Provincia> provincias = db.Provincias.ToList<Provincia>();
            List<Localidad> localidades = db.Localidades.ToList<Localidad>();

            CuentaCorriente cuentaCorriente = cuentaCorrienteViewModel.getCuentaCorriente(provincias, localidades);

            db.CuentaCorriente.Add(cuentaCorriente);
            db.SaveChanges();

            cuentaCorrienteViewModel = new CuentaCorrienteViewModel(cuentaCorriente, provincias, localidades);

            return View("Edit", cuentaCorrienteViewModel);
        }

        public ActionResult Edit(int id)
        {
            CuentaCorriente cuentaCorriente = db.CuentaCorriente.Find(id);
            List<Provincia> provincias = db.Provincias.ToList<Provincia>();
            List<Localidad> localidades = db.Localidades.ToList<Localidad>();

            CuentaCorrienteViewModel cuentaCorrienteViewModel = new CuentaCorrienteViewModel(cuentaCorriente, provincias, localidades);

            return View("Edit", cuentaCorrienteViewModel);
        }

        [HttpPost]
        public ActionResult Edit(CuentaCorrienteViewModel cuentaCorrienteViewModel)
        {
            List<Provincia> provincias = db.Provincias.ToList<Provincia>();
            List<Localidad> localidades = db.Localidades.ToList<Localidad>();
            CuentaCorriente cuentaCorriente = db.CuentaCorriente.Find(cuentaCorrienteViewModel.ID);

            cuentaCorriente = cuentaCorrienteViewModel.UpDateCuentaCorriente(cuentaCorriente, provincias, localidades);

            db.Entry(cuentaCorriente).State = EntityState.Modified;
            db.SaveChanges();

            cuentaCorrienteViewModel = new CuentaCorrienteViewModel(cuentaCorriente, provincias, localidades);

            return View("Edit", cuentaCorrienteViewModel);
        }


        public ActionResult Delete(int id)
        {
            CuentaCorriente cuentaCorriente = db.CuentaCorriente.Find(id);
            if (cuentaCorriente == null)
            {
                return HttpNotFound();
            }
            db.CuentaCorriente.Remove(cuentaCorriente);
            db.SaveChanges();

            return RedirectToAction("Search");
        }

        public ActionResult DeleteComision(int idComision, int idCuentaCorriente)
        {
            Comision comision = db.Comisiones.Find(idComision);
            if (comision == null)
            {
                return HttpNotFound();
            }
            db.Comisiones.Remove(comision);
            db.SaveChanges();

            return RedirectToAction("Edit", new { id = idCuentaCorriente });
        }

        public ActionResult Search()
        {
            SearchCuentaCorrienteViewModel searchCuentaCorrienteViewModel = new SearchCuentaCorrienteViewModel();

            List<CuentaCorriente> cuentasCorrientes = db.CuentaCorriente.ToList<CuentaCorriente>();

            searchCuentaCorrienteViewModel.CuentaCorriente = cuentasCorrientes.ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return View(searchCuentaCorrienteViewModel);
        }

        [HttpPost]
        public ActionResult Search(SearchCuentaCorrienteViewModel searchCuentaCorrienteViewModel)
        {
            List<CuentaCorriente> cuentasCorrientes = db.CuentaCorriente.ToList<CuentaCorriente>();

            if (searchCuentaCorrienteViewModel.CUIL != null &&  searchCuentaCorrienteViewModel.CUIL != string.Empty)
                cuentasCorrientes = cuentasCorrientes.Where(x => x.CUIL.ToUpper().Contains(searchCuentaCorrienteViewModel.CUIL.ToUpper())).ToList();

            if (searchCuentaCorrienteViewModel.RazonSocial != null && searchCuentaCorrienteViewModel.RazonSocial != string.Empty)
                cuentasCorrientes = cuentasCorrientes.Where(x => x.RazonSocial.ToUpper().Contains(searchCuentaCorrienteViewModel.RazonSocial.ToUpper())).ToList();

            searchCuentaCorrienteViewModel.CuentaCorriente = cuentasCorrientes.ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return View(searchCuentaCorrienteViewModel);
        }

        [HttpPost]
        public ActionResult SearchCuentaCorriente(CuentaCorrienteViewModel cuentaCorrienteViewModel, int id)
        {
            //elimino los errores de carga del formulario 
            ModelState.Clear();

            CuentaCorriente cuentaCorriente = db.CuentaCorriente.Find(id);
            List<Provincia> provincias = db.Provincias.ToList<Provincia>();
            List<Localidad> localidades = db.Localidades.ToList<Localidad>();

            var deuda = CuentaCorrienteHelper.getDeudas(cuentaCorriente.Comisiones);
            var fechaAlta = cuentaCorrienteViewModel.FechaAlta;
            var fechaEnvio = cuentaCorrienteViewModel.FechaEntrega;
            var fechaPago = cuentaCorrienteViewModel.FechaPago;
            var numero = string.IsNullOrEmpty(cuentaCorrienteViewModel.Numero)? (int?)null : int.Parse(cuentaCorrienteViewModel.Numero);

            cuentaCorriente.Comisiones = CuentaCorrienteHelper.searchComisiones(cuentaCorriente.Comisiones, numero, CuentaCorrienteHelper.getFecha(fechaAlta), CuentaCorrienteHelper.getFecha(fechaEnvio), CuentaCorrienteHelper.getFecha(fechaPago));

            cuentaCorrienteViewModel = new CuentaCorrienteViewModel(cuentaCorriente, provincias, localidades);

            cuentaCorrienteViewModel.TotalDeuda = CuentaCorrienteHelper.getTotalDeuda(deuda);
            cuentaCorrienteViewModel.FechaAlta = fechaAlta;
            cuentaCorrienteViewModel.FechaEntrega = fechaEnvio;
            cuentaCorrienteViewModel.FechaPago = fechaPago;
            cuentaCorrienteViewModel.Numero = numero.HasValue? numero.Value.ToString(): null;

            return View("Edit", cuentaCorrienteViewModel);
        }

        public ActionResult Send(int idComision, int idCuentaCorriente)
        {
            SearchCuentaCorrienteViewModel searchCuentaCorrienteViewModel = new SearchCuentaCorrienteViewModel();

            Comision comision = db.Comisiones.Find(idComision);
            comision.FechaEntrega = DateTime.Now.Date;
            db.SaveChanges();

            CuentaCorriente cuentaCorriente = db.CuentaCorriente.Find(idCuentaCorriente);
            List<Provincia> provincias = db.Provincias.ToList<Provincia>();
            List<Localidad> localidades = db.Localidades.ToList<Localidad>();

            CuentaCorrienteViewModel cuentaCorrienteViewModel = new CuentaCorrienteViewModel(cuentaCorriente, provincias, localidades);

            return View("Edit", cuentaCorrienteViewModel);
        }

        [HttpPost]
        public ActionResult pagar(CuentaCorrienteViewModel cuentaCorrienteViewModel, int id)
        {
            //elimino los errores de carga del formulario 
            ModelState.Clear();

            CuentaCorriente cuentaCorriente = db.CuentaCorriente.Find(id);
            List<Provincia> provincias = db.Provincias.ToList<Provincia>();
            List<Localidad> localidades = db.Localidades.ToList<Localidad>();

            List<Comision> deudas = CuentaCorrienteHelper.getDeudas(cuentaCorriente.Comisiones).OrderBy(x => x.FechaAlta).ToList();

            decimal monto = decimal.Parse(cuentaCorrienteViewModel.MontoEntrega);

            TimeZoneInfo tst = TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time");
            DateTime tstTime = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, tst);

            cuentaCorriente.Pagos.Add(new Pago(monto));

            foreach (Comision comision in deudas)
            {
                if (comision.Debe > 0 && monto >= comision.Debe)
                {
                    comision.FechaPago = DateTime.Now;
                    comision.Pago = true;
                    monto -= comision.Debe;
                    comision.Debe = 0;
                    db.Entry(comision).State = EntityState.Modified;
                    continue;
                }
                if (comision.Debe > 0 && monto < comision.Debe)
                {
                    comision.Pago = false;
                    comision.Debe -= monto;
                    monto = 0;
                    db.Entry(comision).State = EntityState.Modified;
                    break;
                }
                if (monto >= comision.Costo)
                {
                    comision.FechaPago = DateTime.Now;
                    comision.Pago = true;
                    monto -= comision.Costo;
                    comision.Debe = 0;
                    db.Entry(comision).State = EntityState.Modified;
                    continue;
                }
                if (monto < comision.Costo)
                {
                    comision.Pago = false;
                    comision.Debe = comision.Costo  - monto;
                    monto = 0;
                    db.Entry(comision).State = EntityState.Modified;
                    break;
                }
            }

            db.SaveChanges();

            cuentaCorrienteViewModel = new CuentaCorrienteViewModel(cuentaCorriente, provincias, localidades);

            return View("Edit", cuentaCorrienteViewModel);
        }

        public ActionResult SearchPagingCuentaCorriente(string razonSocial, string CUIL, int pageNumber)
        {
            List<CuentaCorriente> cuentasCorrientes = db.CuentaCorriente.ToList<CuentaCorriente>();

            if (CUIL != null && CUIL != string.Empty)
                cuentasCorrientes = cuentasCorrientes.Where(x => x.CUIL.ToUpper().Contains(CUIL.ToUpper())).ToList();

            if (razonSocial != null && razonSocial != string.Empty)
                cuentasCorrientes = cuentasCorrientes.Where(x => x.RazonSocial.ToUpper().Contains(razonSocial.ToUpper())).ToList();

            return PartialView("_CuentaCorrienteTable", cuentasCorrientes.ToPagedList(pageNumber, int.Parse(ConfigurationSettings.AppSettings["PageSize"])));
        }

        public ActionResult EditPagingCuentaCorrienteDebe(int id, int? numero, string fechaAlta, string fechaEnvio, string fechaPago, int pageNumber)
        {
            CuentaCorriente cuentaCorriente = db.CuentaCorriente.Find(id);

            if (numero.HasValue || !string.IsNullOrEmpty(fechaAlta) || !string.IsNullOrEmpty(fechaEnvio) || !string.IsNullOrEmpty(fechaPago))
            {
                DateTime? fechaAltaDate;
                if (string.IsNullOrEmpty(fechaAlta))
                    fechaAltaDate = null;
                else
                    fechaAltaDate = DateTime.ParseExact(fechaAlta, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                DateTime? fechaEnvioDate;
                if (string.IsNullOrEmpty(fechaEnvio))
                    fechaEnvioDate = null;
                else
                    fechaEnvioDate = DateTime.ParseExact(fechaEnvio, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                DateTime? fechaEntregaDate;
                if (string.IsNullOrEmpty(fechaPago))
                    fechaEntregaDate = null;
                else
                    fechaEntregaDate = DateTime.ParseExact(fechaPago, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                cuentaCorriente.Comisiones = CuentaCorrienteHelper.searchComisiones(cuentaCorriente.Comisiones, numero, fechaAltaDate, fechaEnvioDate, fechaEntregaDate);
            }
            IPagedList<Comision> Comisiones = CuentaCorrienteHelper.getDeudas(cuentaCorriente.Comisiones).ToPagedList<Comision>(pageNumber, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
            ViewData.Add(new KeyValuePair<string, object>("idCuentaCorriente", id));
            ViewData.Add(new KeyValuePair<string, object>("numero", numero));
            ViewData.Add(new KeyValuePair<string, object>("fechaAlta", fechaAlta));
            ViewData.Add(new KeyValuePair<string, object>("fechaEnvio", fechaEnvio));
            ViewData.Add(new KeyValuePair<string, object>("fechaPago", fechaPago));
            ViewData.Add(new KeyValuePair<string, object>("pageNumber", fechaPago));

            return PartialView("_Debe", Comisiones);
        }


        public ActionResult EditPagingCuentaCorrientePagos(int id, int? numero, string fechaAlta, string fechaEnvio, string fechaPago, int pageNumber)
        {
            CuentaCorriente cuentaCorriente = db.CuentaCorriente.Find(id);

            if (numero.HasValue || !string.IsNullOrEmpty(fechaAlta) || !string.IsNullOrEmpty(fechaEnvio) || !string.IsNullOrEmpty(fechaPago))
            {
                DateTime? fechaAltaDate;
                if (string.IsNullOrEmpty(fechaAlta))
                    fechaAltaDate = null;
                else
                    fechaAltaDate = DateTime.ParseExact(fechaAlta, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                DateTime? fechaEnvioDate;
                if (string.IsNullOrEmpty(fechaEnvio))
                    fechaEnvioDate = null;
                else
                    fechaEnvioDate = DateTime.ParseExact(fechaEnvio, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                DateTime? fechaEntregaDate;
                if (string.IsNullOrEmpty(fechaPago))
                    fechaEntregaDate = null;
                else
                    fechaEntregaDate = DateTime.ParseExact(fechaPago, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);

                cuentaCorriente.Comisiones = CuentaCorrienteHelper.searchComisiones(cuentaCorriente.Comisiones, numero, fechaAltaDate, fechaEnvioDate, fechaEntregaDate);
            }

            IPagedList<Comision> Comisiones = CuentaCorrienteHelper.getPagos(cuentaCorriente.Comisiones).ToPagedList<Comision>(pageNumber, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
            ViewData.Add(new KeyValuePair<string, object>("idCuentaCorriente", id));
            ViewData.Add(new KeyValuePair<string, object>("numero", numero));
            ViewData.Add(new KeyValuePair<string, object>("fechaAlta", fechaAlta));
            ViewData.Add(new KeyValuePair<string, object>("fechaEnvio", fechaEnvio));
            ViewData.Add(new KeyValuePair<string, object>("fechaPago", fechaPago));
            ViewData.Add(new KeyValuePair<string, object>("pageNumber", fechaPago));

            return PartialView("_Pagos", Comisiones);
        }

        public ActionResult Export()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Export(int? id, string fechaDesde, string fechaHasta)
        {
            var cuentaCorriente = db.CuentaCorriente.Find(id);

            DateTime? fDesde = CuentaCorrienteHelper.getFecha(fechaDesde);
            DateTime? fHasta = CuentaCorrienteHelper.getFecha(fechaHasta);

            var comisiones = cuentaCorriente.Comisiones.Where(x => x.FechaAlta.CompareTo(fDesde.Value) >= 0 && x.FechaAlta.CompareTo(fHasta.Value) <= 0).OrderBy(x => x.ID).ToList();

            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/Template/CuentaCorrienteTemplate.xls");

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);

            HSSFWorkbook tamplateWorckbook = new HSSFWorkbook(fs, true);

            ISheet comisionSheet = tamplateWorckbook.GetSheetAt(0);

            List<ICell> cells = comisionSheet.GetRow(4).Cells;
            cells[2].SetCellValue(DateTime.Now.Date.ToString("dd/MM/yyyy"));

            cells = comisionSheet.GetRow(5).Cells;
            cells[2].SetCellValue(cuentaCorriente.RazonSocial);

            cells = comisionSheet.GetRow(6).Cells;
            cells[2].SetCellValue(cuentaCorriente.CUIL);

            cells = comisionSheet.GetRow(7).Cells;
            cells[2].SetCellValue(cuentaCorriente.Domicilio.getDomicilio);

            cells = comisionSheet.GetRow(8).Cells;
            cells[2].SetCellValue(cuentaCorriente.Telefono);

            ICellStyle style = tamplateWorckbook.CreateCellStyle();
            style.BorderBottom = BorderStyle.Medium;
            style.BorderLeft = BorderStyle.Medium;
            style.BorderRight = BorderStyle.Medium;
            style.BorderTop = BorderStyle.Medium;


            int comisionRow = 11;

            foreach (Comision comision in comisiones)
            {
                IRow row = comisionSheet.CreateRow(comisionRow);
                row.CreateCell(0).SetCellValue(comision.ID);
                row.Cells[0].CellStyle = style;
                row.CreateCell(1).SetCellValue(comision.Contacto);
                row.Cells[1].CellStyle = style;
                row.CreateCell(2).SetCellValue(comision.Accion.ToString());
                row.Cells[2].CellStyle = style;
                row.CreateCell(3).SetCellValue(comision.DomicilioRetirar != null? comision.DomicilioRetirar.getDomicilio: "");
                row.Cells[3].CellStyle = style;
                row.CreateCell(4).SetCellValue(comision.DomicilioEntregar != null? comision.DomicilioEntregar.getDomicilio: "");
                row.Cells[4].CellStyle = style;
                row.CreateCell(5).SetCellValue(comision.FechaEntrega.HasValue? comision.FechaEntrega.Value.ToString("dd/MM/yyyy"): "");
                row.Cells[5].CellStyle = style;
                row.CreateCell(6).SetCellValue(comision.Debe > 0? comision.Debe.ToString("c") : comision.Costo.ToString("c"));
                row.Cells[6].CellStyle = style;
                row.CreateCell(7).SetCellValue(comision.Pago ? "Si" : "No");
                row.Cells[7].CellStyle = style;
                comisionRow++;
            }

            IRow rowTotal = comisionSheet.CreateRow(comisionRow);
            rowTotal.CreateCell(0).SetCellValue(string.Format("TOTAL DEUDA {0}", CuentaCorrienteHelper.getTotalDeuda(CuentaCorrienteHelper.getDeudas(comisiones)).ToString("c")));

            style.Alignment = HorizontalAlignment.Right;
            rowTotal.Cells[0].CellStyle = style;

            for (int i = 1; i <= 7; i++)
            {
                rowTotal.CreateCell(i);
                rowTotal.Cells[i].CellStyle = style;
            }

            var cellRange = new CellRangeAddress(comisionRow, comisionRow, 0, 7);
            comisionSheet.AddMergedRegion(cellRange);



            string name = String.Format("{0}_{1}", cuentaCorriente.RazonSocial, DateTime.Now.Date.ToString("dd_MM_yyyy"));

            MemoryStream ms = new MemoryStream();

            tamplateWorckbook.Write(ms);

            return File(ms.ToArray(), "application/vnd.ms-excel", name + ".xls");
        }

    }
}
