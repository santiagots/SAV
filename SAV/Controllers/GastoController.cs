using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using PagedList;
using SAV.Common;
using SAV.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SAV.Controllers
{
    public class GastoController : Controller
    {
        private SAVContext db = new SAVContext();

        public ActionResult Create()
        {

            List<TipoGasto> tipoGastos = db.TipoGasto.ToList();
            List<Gasto> comisionGasto = db.Gastos.ToList<Gasto>();

            CreateGastoViewModel searchGastoViewModel = new CreateGastoViewModel();

            searchGastoViewModel.TipoGasto = new List<KeyValuePair<int, string>>();
            searchGastoViewModel.Concepto = EnumHelper.GetEnumList<ConceptoGasto>();
            searchGastoViewModel.UsuarioAlta = User.Identity.Name.ToUpper();
            searchGastoViewModel.FechaAlta = DateTime.Now.ToString("dd/MM/yyyy");

            return View(searchGastoViewModel);
        }

        [HttpPost]
        public ActionResult Create(CreateGastoViewModel searchGastoViewModel)
        {
            try
            {
                Gasto gasto = new Gasto()
                {
                    Concepto =  (ConceptoGasto) searchGastoViewModel.SelectConcepto,
                    Comentario = searchGastoViewModel.Comentario,
                    FechaAlta = DateTime.Now,
                    Monto = decimal.Parse(searchGastoViewModel.Monto),
                    TipoGasto = db.TipoGasto.Where(x => x.ID == searchGastoViewModel.SelectTipoGasto).FirstOrDefault(),
                    UsuarioAlta = searchGastoViewModel.UsuarioAlta
                };

                db.Gastos.Add(gasto);
                db.SaveChanges();


                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpNotFoundResult($"Se a generado un error al guardar el Gasto. {ex.Message}");
            }
        }

        public ActionResult Search()
        {
            List<TipoGasto> tipoGastos = db.TipoGasto.ToList();
            List<Gasto> comisionGasto = db.Gastos.ToList<Gasto>();

            SearchGastoViewModel searchGastoViewModel = new SearchGastoViewModel();

            searchGastoViewModel.Concepto = EnumHelper.GetEnumList<ConceptoGasto>();
            searchGastoViewModel.TipoGasto = new List<KeyValuePair<int, string>>();
            searchGastoViewModel.UsuarioAlta = UsuarioHelper.getUsuarios().Select(x => new KeyValuePair<string, string>(x.Usuario.ToUpper(), x.Usuario.ToUpper())).ToList();
            searchGastoViewModel.Gastos = comisionGasto.OrderByDescending(x => x.ID).ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return View(searchGastoViewModel);
        }

        [HttpPost]
        public ActionResult Search(SearchGastoViewModel searchGastoViewModel)
        {
            this.ViewData["descripcion"] = searchGastoViewModel.Comentario;
            this.ViewData["fechaAlta"] = searchGastoViewModel.FechaAlta;
            this.ViewData["fechaDesde"] = searchGastoViewModel.FechaDesde;
            this.ViewData["fechaHasta"] = searchGastoViewModel.FechaHasta;
            this.ViewData["monto"] = searchGastoViewModel.Monto;
            this.ViewData["usuarioAlta"] = searchGastoViewModel.SelectUsuarioAlta;
            this.ViewData["tipoGasto"] = searchGastoViewModel.SelectTipoGasto;
            this.ViewData["concepto"] = searchGastoViewModel.SelectConcepto;

            //elimino los errores de carga del formulario 
            ModelState.Clear();

            IQueryable<Gasto> gastoQueryable = db.Gastos.AsQueryable<Gasto>();
            List<Gasto> gasto = GastoHelper.searchComisionGasto(gastoQueryable, searchGastoViewModel.Comentario, ComisionHelper.getFecha(searchGastoViewModel.FechaAlta), ComisionHelper.getFecha(searchGastoViewModel.FechaDesde), ComisionHelper.getFecha(searchGastoViewModel.FechaHasta), ComisionHelper.getMonto(searchGastoViewModel.Monto), searchGastoViewModel.SelectTipoGasto, searchGastoViewModel.SelectUsuarioAlta, searchGastoViewModel.SelectConcepto);
            List<TipoGasto> tipoGastos = db.TipoGasto.ToList();
            List<Gasto> comisionGasto = db.Gastos.ToList<Gasto>();

            searchGastoViewModel.Concepto = EnumHelper.GetEnumList<ConceptoGasto>();
            if(searchGastoViewModel.SelectConcepto.HasValue)
                searchGastoViewModel.TipoGasto = tipoGastos.Where(x => x.Habilitado && x.Concepto == (ConceptoGasto)searchGastoViewModel.SelectConcepto).Select(y => new KeyValuePair<int, string>(y.ID, y.Descripcion)).ToList();
            else
                searchGastoViewModel.TipoGasto = new List<KeyValuePair<int, string>>();
            searchGastoViewModel.UsuarioAlta = UsuarioHelper.getUsuarios().Select(x => new KeyValuePair<string, string>(x.Usuario.ToUpper(), x.Usuario.ToUpper())).ToList();
            searchGastoViewModel.Gastos = gasto.OrderByDescending(x => x.ID).ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return View(searchGastoViewModel);
        }

        [HttpPost]
        public ActionResult Export(SearchGastoViewModel searchGastoViewModel)
        {
            this.ViewData["descripcion"] = searchGastoViewModel.Comentario;
            this.ViewData["fechaAlta"] = searchGastoViewModel.FechaAlta;
            this.ViewData["fechaDesde"] = searchGastoViewModel.FechaDesde;
            this.ViewData["fechaHasta"] = searchGastoViewModel.FechaHasta;
            this.ViewData["monto"] = searchGastoViewModel.Monto;
            this.ViewData["usuarioAlta"] = searchGastoViewModel.SelectUsuarioAlta;
            this.ViewData["tipoGasto"] = searchGastoViewModel.SelectTipoGasto;
            this.ViewData["concepto"] = searchGastoViewModel.SelectConcepto;

            IQueryable<Gasto> gastoQueryable = db.Gastos.AsQueryable<Gasto>();
            List<Gasto> gastos = GastoHelper.searchComisionGasto(gastoQueryable, searchGastoViewModel.Comentario, ComisionHelper.getFecha(searchGastoViewModel.FechaAlta), ComisionHelper.getFecha(searchGastoViewModel.FechaDesde), ComisionHelper.getFecha(searchGastoViewModel.FechaHasta), ComisionHelper.getMonto(searchGastoViewModel.Monto), searchGastoViewModel.SelectTipoGasto, searchGastoViewModel.SelectUsuarioAlta, searchGastoViewModel.SelectConcepto);

            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/Template/GastoTemplate.xls");

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);

            HSSFWorkbook tamplateWorckbook = new HSSFWorkbook(fs, true);

            ISheet gastosSheet = tamplateWorckbook.GetSheetAt(0);

            ICellStyle style = tamplateWorckbook.CreateCellStyle();
            style.BorderBottom = BorderStyle.Medium;
            style.BorderLeft = BorderStyle.Medium;
            style.BorderRight = BorderStyle.Medium;
            style.BorderTop = BorderStyle.Medium;

            int gastoRow = 3;

            foreach (Gasto gasto in gastos)
            {
                IRow row = gastosSheet.CreateRow(gastoRow);
                row.CreateCell(0).SetCellValue(gasto.FechaAlta.ToString("dd/MM/yy"));
                row.Cells[0].CellStyle = style;
                row.CreateCell(1).SetCellValue(gasto.ID);
                row.Cells[1].CellStyle = style;
                row.CreateCell(2).SetCellValue(gasto.Concepto.ToString());
                row.Cells[2].CellStyle = style;
                row.CreateCell(3).SetCellValue(gasto.TipoGasto.Descripcion);
                row.Cells[3].CellStyle = style;
                row.CreateCell(4).SetCellValue(gasto.Comentario);
                row.Cells[4].CellStyle = style;
                row.CreateCell(5).SetCellValue(gasto.Monto.ToString("C"));
                row.Cells[5].CellStyle = style;
                gastoRow++;
            }

            IRow rowTotal = gastosSheet.CreateRow(gastoRow);
            rowTotal.CreateCell(5).SetCellValue(gastos.Sum(x => x.Monto).ToString("C"));

            MemoryStream ms = new MemoryStream();

            tamplateWorckbook.Write(ms);

            return File(ms.ToArray(), "application/vnd.ms-excel", "Gastos.xls");
        }

        public ActionResult DeleteGasto(int id)
        {
            Gasto gasto = db.Gastos.Find(id);

            if (gasto != null)
            {
                db.Gastos.Remove(gasto);
                db.SaveChanges();
            }
            return RedirectToAction("Search");
        }

        public ActionResult GastosPaging(string comentario, string fechaAlta, string fechaDesde, string fechaHasta, string monto, int? tipoGasto, string usuarioAlta, int? concepto, int pageNumber)
        {
            IQueryable<Gasto> gastoQueryable = db.Gastos.AsQueryable<Gasto>();

            List<Gasto> gasto = GastoHelper.searchComisionGasto(gastoQueryable, comentario, ComisionHelper.getFecha(fechaAlta), ComisionHelper.getFecha(fechaDesde), ComisionHelper.getFecha(fechaHasta), ComisionHelper.getMonto(monto), tipoGasto, usuarioAlta.ToUpper(), concepto);

            IPagedList<Gasto> comisionesPagos = gasto.OrderByDescending(x => x.ID).ToPagedList<Gasto>(pageNumber, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            this.ViewData["descripcion"] = comentario;
            this.ViewData["fechaAlta"] = fechaAlta;
            this.ViewData["fechaDesde"] = fechaDesde;
            this.ViewData["fechaHasta"] = fechaHasta;
            this.ViewData["monto"] = monto;
            this.ViewData["usuarioAlta"] = usuarioAlta.ToUpper();
            this.ViewData["tipoGasto"] = tipoGasto;
            this.ViewData["concepto"] = concepto;

            return PartialView("_GastosTable", comisionesPagos);
        }

        public ActionResult GetGastosPorConcepto(int concepto)
        {
            List<TipoGasto> tipoGasto = db.TipoGasto.Where(x => x.Habilitado && x.Concepto == (ConceptoGasto) concepto).ToList();

            if (HttpContext.Request.IsAjaxRequest())
                return Json(new SelectList(tipoGasto.OrderBy(x => x.Descripcion), "ID", "Descripcion"), JsonRequestBehavior.AllowGet);

            return View(tipoGasto);
        }

    }
}
