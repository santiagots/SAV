using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAV.Models;
using PagedList;
using System.Configuration;
using SAV.Common;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace SAV.Controllers
{
    [Authorize]
    public class ComisionController : Controller
    {
        private SAVContext db = new SAVContext();

        public ActionResult GetLocalidades(int IdProvincia)
        {
            Provincia provincia = db.Provincias.Where(x => x.ID == IdProvincia).FirstOrDefault();

            if (HttpContext.Request.IsAjaxRequest())
                return Json(new SelectList(provincia.Localidad.OrderBy(x => x.Nombre), "ID", "Nombre"), JsonRequestBehavior.AllowGet);

            return View(provincia);
        }

        [HttpPost]
        public ActionResult ResponsableAdd(string apellido, string nombre)
        {
            ComisionResponsable comisionResponsable = new ComisionResponsable();

            comisionResponsable.Nombre = nombre.ToUpper();
            comisionResponsable.Apellido = apellido.ToUpper();

            db.ComisionResponsable.Add(comisionResponsable);
            db.SaveChanges();

            ResponsableViewModel responsableViewModel = new ResponsableViewModel();

            responsableViewModel.Responsables = db.ComisionResponsable.ToList().ToPagedList<ComisionResponsable>(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return View("Responsable", responsableViewModel);
        }

        [HttpPost]
        public ActionResult ResponsableDelete(int id)
        {
            ComisionResponsable comisionResponsable = db.ComisionResponsable.Find(id);

            List<Comision> comisiones = db.Comisiones.Where(x => x.Responsable.ID == id).ToList();

            foreach (Comision comision in comisiones)
            {
                comision.Responsable = null;
                db.Entry(comision).State = EntityState.Modified;
            }

            db.Entry(comisionResponsable).State = EntityState.Deleted;
            db.SaveChanges();

            ResponsableViewModel responsableViewModel = new ResponsableViewModel();

            responsableViewModel.Responsables = db.ComisionResponsable.ToList().ToPagedList<ComisionResponsable>(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return View("Responsable", responsableViewModel);
        }

        public ActionResult ResponsableAdd()
        {
            ResponsableViewModel responsableViewModel = new ResponsableViewModel();

            responsableViewModel.Responsables = db.ComisionResponsable.ToList().ToPagedList<ComisionResponsable>(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return View("Responsable", responsableViewModel);
        }

        public ActionResult SearchPagingResponsable(int? pageNumber)
        {
            IPagedList<ComisionResponsable> result = db.ComisionResponsable.ToList().ToPagedList<ComisionResponsable>(pageNumber.Value, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return PartialView("_ResponsableTable", result);
        }

        public ActionResult Delete(int id)
        {
            Comision comision = db.Comisiones.Find(id);
            db.Comisiones.Remove(comision);
            db.SaveChanges();

            return RedirectToAction("Search");
        }

        public ActionResult DeleteGasto(int id)
        {
            ComisionGasto ComisionGasto = db.ComisionGastos.Find(id);
            db.ComisionGastos.Remove(ComisionGasto);
            db.SaveChanges();

            return RedirectToAction("Spending");
        }

        public ActionResult Create(int? idCuentaCorriente)
        {

            List<ComisionResponsable> comisionResponsable = db.ComisionResponsable.ToList<ComisionResponsable>();
            List<Provincia> provincia = db.Provincias.ToList<Provincia>();
            List<Parada> paradas = db.Paradas.ToList<Parada>();

            ComisionViewModel comisionViewModel = new ComisionViewModel(provincia, comisionResponsable, paradas);

            comisionViewModel.idCuentaCorriente = idCuentaCorriente.HasValue? idCuentaCorriente.Value: -1;

            return View(comisionViewModel);
        }

        public ActionResult Clone(int idComision, int? idCuentaCorriente)
        {
            ViewBag.Action = "Create";
            List<ComisionResponsable> comisionResponsable = db.ComisionResponsable.ToList<ComisionResponsable>();
            List<Provincia> provincia = db.Provincias.ToList<Provincia>();
            List<Parada> paradas = db.Paradas.ToList<Parada>();
            Comision comision = db.Comisiones.Find(idComision);

            ComisionViewModel comisionViewModel = new ComisionViewModel(provincia, comision, comisionResponsable, paradas);

            if (idCuentaCorriente.HasValue)
                comisionViewModel.idCuentaCorriente = idCuentaCorriente.Value;

            return View("Create", comisionViewModel);
        }

        [HttpPost]
        public ActionResult Create(ComisionViewModel comisionViewModel)
        {
            List<Provincia> provincias = db.Provincias.ToList<Provincia>();
            List<Localidad> localidades = db.Localidades.ToList<Localidad>();
            List<Parada> paradas = db.Paradas.ToList<Parada>();

            Comision comision = comisionViewModel.getComision(comisionViewModel, provincias, localidades, paradas);
            comision.Responsable = db.ComisionResponsable.Find(comisionViewModel.SelectResponsable);

            if (comisionViewModel.idCuentaCorriente > 0)
            {
                CuentaCorriente cuentaCorriente = db.CuentaCorriente.Find(comisionViewModel.idCuentaCorriente);
                cuentaCorriente.Comisiones.Add(comision);
                db.SaveChanges();
                return RedirectToAction("Edit", "CuentaCorriente", new { id = comisionViewModel.idCuentaCorriente });
            }
            else
            {
                db.Comisiones.Add(comision);
                db.SaveChanges();
                return RedirectToAction("Search");
            }
        }

        public ActionResult Details(int id, int? idCuentaCorriente)
        {
            ViewBag.Action = "Details";

            List<ComisionResponsable> comisionResponsable = db.ComisionResponsable.ToList<ComisionResponsable>();
            Comision comision = db.Comisiones.Find(id);
            ComisionViewModel comisionViewModel = new ComisionViewModel();
            List<Provincia> Provincias = db.Provincias.ToList<Provincia>();
            List<Parada> paradas = db.Paradas.ToList<Parada>();

            comisionViewModel = new ComisionViewModel(Provincias, comision, comisionResponsable, paradas);
            comisionViewModel.idCuentaCorriente = idCuentaCorriente.HasValue ? idCuentaCorriente.Value : -1;
            return View("Create", comisionViewModel);
        }

        [HttpPost]
        public ActionResult Details(ComisionViewModel comisionViewModel, int id)
        {
            List<Provincia> provincias = db.Provincias.ToList<Provincia>();
            List<Localidad> localidades = db.Localidades.ToList<Localidad>();
            List<Parada> paradas = db.Paradas.ToList<Parada>();
            Comision comisiones = db.Comisiones.Find(id);

            comisionViewModel.upDateComision(comisionViewModel, provincias, localidades, paradas, ref comisiones);
            comisiones.Responsable = db.ComisionResponsable.Find(comisionViewModel.SelectResponsable);

            db.Entry(comisiones).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Search");
        }

        public ActionResult Search()
        {
            SearchComisionViewModel searchComisionViewModel = new SearchComisionViewModel();

            List<Comision> comisiones = db.Comisiones.ToList<Comision>();

            List<ComisionResponsable> responsables = db.ComisionResponsable.ToList();
            List<CuentaCorriente> cuentaCorriente = db.CuentaCorriente.ToList();

            searchComisionViewModel.Responsable = responsables.Select(x => new KeyValuePair<int, string>(x.ID, string.Format("{0} {1}", x.Apellido, x.Nombre))).ToList();
            searchComisionViewModel.CuentaCorriente = cuentaCorriente.Select(x => new KeyValuePair<int, string>(x.ID, x.RazonSocial)).ToList();
            searchComisionViewModel.ComisionesEnvio = ComisionHelper.getEnvios(comisiones).OrderByDescending(x => x.ID).ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
            searchComisionViewModel.ComisionesPendientes = ComisionHelper.getPendientes(comisiones).OrderByDescending(x => x.ID).ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
            searchComisionViewModel.ComisionesFinalizadas = ComisionHelper.getFinalizadas(comisiones).OrderByDescending(x => x.ID).ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return View(searchComisionViewModel);
        }

        [HttpPost]
        public ActionResult Search(SearchComisionViewModel searchComisionViewModel)
        {
            List<Comision> comisiones = db.Comisiones.ToList<Comision>();

            comisiones = ComisionHelper.searchComisiones(comisiones, searchComisionViewModel.ID, searchComisionViewModel.Contacto, searchComisionViewModel.Servicio, searchComisionViewModel.Accion, ComisionHelper.getFecha(searchComisionViewModel.FechaAlta), ComisionHelper.getFecha(searchComisionViewModel.FechaEnvio), ComisionHelper.getFecha(searchComisionViewModel.FechaEntrega), ComisionHelper.getFecha(searchComisionViewModel.FechaPago), searchComisionViewModel.Costo, searchComisionViewModel.SelectResponsable, searchComisionViewModel.SelectCuentaCorriente);

            List<ComisionResponsable> responsables = db.ComisionResponsable.ToList();
            List<CuentaCorriente> cuentaCorriente = db.CuentaCorriente.ToList();

            searchComisionViewModel.Responsable = responsables.Select(x => new KeyValuePair<int, string>(x.ID, string.Format("{0} {1}", x.Apellido, x.Nombre))).ToList();
            searchComisionViewModel.CuentaCorriente = cuentaCorriente.Select(x => new KeyValuePair<int, string>(x.ID, x.RazonSocial)).ToList();
            searchComisionViewModel.ComisionesEnvio = ComisionHelper.getEnvios(comisiones).OrderByDescending(x => x.ID).ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
            searchComisionViewModel.ComisionesPendientes = ComisionHelper.getPendientes(comisiones).OrderByDescending(x => x.ID).ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
            searchComisionViewModel.ComisionesFinalizadas = ComisionHelper.getFinalizadas(comisiones).OrderByDescending(x => x.ID).ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return View(searchComisionViewModel);
        }

        [HttpPost]
        public ActionResult SearchComisionGasto(ComisionGastoViewModel comisionGastoViewModel)
        {
            //elimino los errores de carga del formulario 
            ModelState.Clear();

            List<ComisionGasto> comisionGasto = db.ComisionGastos.ToList<ComisionGasto>();

            comisionGasto = ComisionHelper.searchComisionGasto(comisionGasto, comisionGastoViewModel.BuscarDescriptcion, ComisionHelper.getFecha(comisionGastoViewModel.BuscarFecha), ComisionHelper.getMonto(comisionGastoViewModel.BuscarMonto));

            comisionGastoViewModel.Gastos = comisionGasto.OrderByDescending(x => x.ID).ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return View("Spending", comisionGastoViewModel);
        }
        

        public ActionResult Send(int ID, string Numero, string Contacto, string Servicio, string Accion, string FechaAlta, string FechaEnvio, string FechaEntrega, string FechaPago, string Costo, int? IdResponsable, int? IdCuentaCorriente)
        {
            SearchCuentaCorrienteViewModel searchCuentaCorrienteViewModel = new SearchCuentaCorrienteViewModel();

            Comision comision = db.Comisiones.Find(ID);
            comision.FechaEntrega = DateTime.Now.Date;
            db.SaveChanges();

            List<Comision> comisiones = db.Comisiones.ToList();

            comisiones = ComisionHelper.searchComisiones(comisiones, Numero, Contacto, Servicio, Accion, ComisionHelper.getFecha(FechaAlta), ComisionHelper.getFecha(FechaEnvio), ComisionHelper.getFecha(FechaEntrega), ComisionHelper.getFecha(FechaPago), Costo, IdResponsable, IdCuentaCorriente);

            SearchComisionViewModel searchComisionViewModel = new SearchComisionViewModel();
            searchComisionViewModel.Contacto = Contacto;
            searchComisionViewModel.Servicio = Servicio;
            searchComisionViewModel.Accion = Accion;
            searchComisionViewModel.ID = Numero;
            searchComisionViewModel.FechaAlta = FechaAlta;
            searchComisionViewModel.FechaEntrega = FechaEntrega;
            searchComisionViewModel.FechaEnvio = FechaEnvio;
            searchComisionViewModel.FechaPago = FechaPago;

            searchComisionViewModel.ComisionesEnvio = ComisionHelper.getEnvios(comisiones).OrderByDescending(x => x.ID).ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
            searchComisionViewModel.ComisionesPendientes = ComisionHelper.getPendientes(comisiones).OrderByDescending(x => x.ID).ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
            searchComisionViewModel.ComisionesFinalizadas = ComisionHelper.getFinalizadas(comisiones).OrderByDescending(x => x.ID).ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            List<ComisionResponsable> responsables = db.ComisionResponsable.ToList();
            List<CuentaCorriente> cuentaCorriente = db.CuentaCorriente.ToList();

            searchComisionViewModel.Responsable = responsables.Select(x => new KeyValuePair<int, string>(x.ID, string.Format("{0} {1}", x.Apellido, x.Nombre))).ToList();
            searchComisionViewModel.CuentaCorriente = cuentaCorriente.Select(x => new KeyValuePair<int, string>(x.ID, x.RazonSocial)).ToList();

            return View("Search", searchComisionViewModel);
        }

        public ActionResult Pay(int ID, string Numero, string Contacto, string Servicio, string Accion, string FechaAlta, string FechaEnvio, string FechaEntrega, string FechaPago, string Costo, int? IdResponsable, int? IdCuentaCorriente)
        {
            SearchCuentaCorrienteViewModel searchCuentaCorrienteViewModel = new SearchCuentaCorrienteViewModel();

            Comision comision = db.Comisiones.Find(ID);
            comision.Pago = true;
            comision.FechaPago = DateTime.Now.Date;
            db.SaveChanges();

            List<Comision> comisiones = db.Comisiones.ToList();

            comisiones = ComisionHelper.searchComisiones(comisiones, Numero, Contacto, Servicio, Accion, ComisionHelper.getFecha(FechaAlta), ComisionHelper.getFecha(FechaEnvio), ComisionHelper.getFecha(FechaEntrega), ComisionHelper.getFecha(FechaPago), Costo, IdResponsable, IdCuentaCorriente);

            SearchComisionViewModel searchComisionViewModel = new SearchComisionViewModel();
            searchComisionViewModel.Contacto = Contacto;
            searchComisionViewModel.Servicio = Servicio;
            searchComisionViewModel.Accion = Accion;
            searchComisionViewModel.ID = Numero;
            searchComisionViewModel.FechaAlta = FechaAlta;
            searchComisionViewModel.FechaEntrega = FechaEntrega;
            searchComisionViewModel.FechaEnvio = FechaEnvio;
            searchComisionViewModel.FechaPago = FechaPago;

            searchComisionViewModel.ComisionesEnvio = ComisionHelper.getEnvios(comisiones).OrderByDescending(x => x.ID).ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
            searchComisionViewModel.ComisionesPendientes = ComisionHelper.getPendientes(comisiones).OrderByDescending(x => x.ID).ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
            searchComisionViewModel.ComisionesFinalizadas = ComisionHelper.getFinalizadas(comisiones).OrderByDescending(x => x.ID).ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            List<ComisionResponsable> responsables = db.ComisionResponsable.ToList();
            List<CuentaCorriente> cuentaCorriente = db.CuentaCorriente.ToList();

            searchComisionViewModel.Responsable = responsables.Select(x => new KeyValuePair<int, string>(x.ID, string.Format("{0} {1}", x.Apellido, x.Nombre))).ToList();
            searchComisionViewModel.CuentaCorriente = cuentaCorriente.Select(x => new KeyValuePair<int, string>(x.ID, x.RazonSocial)).ToList();

            return View("Search", searchComisionViewModel);
        }

        public ActionResult Spending()
        {
            ComisionGastoViewModel comisionGastoViewModel = new ComisionGastoViewModel();

            List<ComisionGasto> comisiones = db.ComisionGastos.ToList<ComisionGasto>();

            comisionGastoViewModel.Gastos = comisiones.OrderByDescending(x => x.ID).ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return View(comisionGastoViewModel);
        }

        [HttpPost]
        public ActionResult Spending(ComisionGastoViewModel comisionGastoViewModel)
        {
            ComisionGasto comisionGasto = new ComisionGasto();

            comisionGasto.Descripcion = comisionGastoViewModel.Descripcion;
            comisionGasto.Monto = decimal.Parse(comisionGastoViewModel.Monto);
            comisionGasto.FechaAlta = DateTime.Now.Date;

            db.ComisionGastos.Add(comisionGasto);
            db.SaveChanges();

            List<ComisionGasto> gastos = db.ComisionGastos.ToList<ComisionGasto>();
            comisionGastoViewModel.Gastos = gastos.OrderByDescending(x => x.ID).ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return View(comisionGastoViewModel);
        }

        public ActionResult GenerarPlanilla()
        {
            List<Comision> comisiones = db.Comisiones.Where(x => x.Enviar).ToList();

            foreach (var item in comisiones)
            {
                item.FechaEnvio = DateTime.Now;
                item.Enviar = false;
                db.Entry(item).State = EntityState.Modified;
            }

            db.SaveChanges();

            MemoryStream ms = GenerarExcel(comisiones);
            string name = String.Format("Comisiones_{0}", DateTime.Now.ToString("dd-MM-yy"));
            return File(ms.ToArray(), "application/vnd.ms-excel", name + ".xls");
        }

        private static MemoryStream GenerarExcel(List<Comision> comisiones)
        {
            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/Template/ComisionTemplate.xls");

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);

            HSSFWorkbook tamplateWorckbook = new HSSFWorkbook(fs, true);

            ISheet ComisionestemplateSheet = tamplateWorckbook.GetSheet("Comisiones");
            Dictionary<string, List<Comision>> ComisionesPorResponsable = new Dictionary<string, List<Comision>>();
            if (comisiones.Count > 0)
            {
                foreach (Comision comision in comisiones)
                {
                    if (!ComisionesPorResponsable.ContainsKey(comision.Responsable.Apellido + ", " + comision.Responsable.Nombre))
                    {
                        ComisionesPorResponsable.Add(comision.Responsable.Apellido + ", " + comision.Responsable.Nombre, new List<Comision>());
                        ((HSSFSheet)ComisionestemplateSheet).CopyTo(tamplateWorckbook, "Comisiones " + comision.Responsable.Apellido + ", " + comision.Responsable.Nombre, true, true);
                    }
                    ComisionesPorResponsable[comision.Responsable.Apellido + ", " + comision.Responsable.Nombre].Add(comision);
                }

                foreach (var ComisionePorResponsable in ComisionesPorResponsable)
                {
                    ISheet ComisionesSheet = tamplateWorckbook.GetSheet("Comisiones " + ComisionePorResponsable.Key);
                    int ComisionesRow = 5;


                    List<ICell> HeadComisionesCell = ComisionesSheet.GetRow(2).Cells;

                    HeadComisionesCell[0].SetCellValue(string.Format("Comisiones: {0}", ComisionePorResponsable.Key));

                    HeadComisionesCell = ComisionesSheet.GetRow(3).Cells;

                    int ComisionIndex = 0;
                    foreach (Comision comision in ComisionePorResponsable.Value)
                    {
                        ComisionIndex++;

                        if (ComisionesSheet.GetRow(ComisionesRow) != null)
                        {
                            ComisionHelper.SetValueToComisionesCell(ComisionesSheet, ComisionesRow, comision, ComisionIndex);
                        }
                        else
                        {
                            IRow RowTemplate1;
                            IRow RowTemplate2;
                            //para cebrar tomo diferentes rows como template
                            if (ComisionIndex % 2 == 0)
                            {
                                RowTemplate1 = ComisionesSheet.GetRow(7);
                                RowTemplate2 = ComisionesSheet.GetRow(8);
                            }
                            else
                            {
                                RowTemplate1 = ComisionesSheet.GetRow(5);
                                RowTemplate2 = ComisionesSheet.GetRow(6);
                            }

                            RowTemplate1.CopyRowTo(ComisionesRow);
                            RowTemplate2.CopyRowTo(ComisionesRow + 1);

                            ComisionHelper.SetValueToComisionesCell(ComisionesSheet, ComisionesRow, comision, ComisionIndex);
                        }
                        ComisionesRow += 2;

                    }
                }

            }

            //elimino la solapa de comisiones que uso como plantilla
            tamplateWorckbook.SetSheetHidden(0, true);

            MemoryStream ms = new MemoryStream();
            tamplateWorckbook.Write(ms);

            return ms;
        }

        public ActionResult SearchPagingComisionEnvios(string Numero, string Contacto, string Servicio, string Accion, string FechaAlta, string FechaEnvio, string FechaEntrega, string FechaPago, string Costo, int? IdResponsable, int? IdCuentaCorriente, int pageNumber)
        {
            List<Comision> comisiones = db.Comisiones.ToList();

            comisiones = ComisionHelper.searchComisiones(comisiones, Numero, Contacto, Servicio, Accion, ComisionHelper.getFecha(FechaAlta), ComisionHelper.getFecha(FechaEnvio), ComisionHelper.getFecha(FechaEntrega), ComisionHelper.getFecha(FechaPago), Costo, IdResponsable, IdCuentaCorriente);

            IPagedList<Comision> comisionesDebe = ComisionHelper.getEnvios(comisiones).OrderByDescending(x => x.ID).ToPagedList<Comision>(pageNumber, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
            ViewData.Add(new KeyValuePair<string, object>("Numero", Numero));
            ViewData.Add(new KeyValuePair<string, object>("Contacto", Contacto));
            ViewData.Add(new KeyValuePair<string, object>("Servicio", Servicio));
            ViewData.Add(new KeyValuePair<string, object>("Accion", Accion));
            ViewData.Add(new KeyValuePair<string, object>("FechaAlta", FechaAlta));
            ViewData.Add(new KeyValuePair<string, object>("FechaEnvio", FechaEnvio));
            ViewData.Add(new KeyValuePair<string, object>("FechaEntrega", FechaEntrega));
            ViewData.Add(new KeyValuePair<string, object>("FechaPago", FechaPago));
            ViewData.Add(new KeyValuePair<string, object>("Costo", Costo));
            ViewData.Add(new KeyValuePair<string, object>("IdResponsable", IdResponsable.HasValue ? IdResponsable : 0));
            ViewData.Add(new KeyValuePair<string, object>("IdCuentaCorriente", IdCuentaCorriente.HasValue ? IdCuentaCorriente : 0));

            return PartialView("_Envio", comisionesDebe);
        }

        public ActionResult SearchPagingComisionEnProgreso(string Numero, string Contacto, string Servicio, string Accion, string FechaAlta, string FechaEnvio, string FechaEntrega, string FechaPago, string Costo, int? IdResponsable, int? IdCuentaCorriente, int pageNumber)
        {
            List<Comision> comisiones = db.Comisiones.ToList();

            comisiones = ComisionHelper.searchComisiones(comisiones, Numero, Contacto, Servicio, Accion, ComisionHelper.getFecha(FechaAlta), ComisionHelper.getFecha(FechaEnvio), ComisionHelper.getFecha(FechaEntrega), ComisionHelper.getFecha(FechaPago), Costo, IdResponsable, IdCuentaCorriente);

            IPagedList<Comision> comisionesDebe = ComisionHelper.getPendientes(comisiones).OrderByDescending(x => x.ID).ToPagedList<Comision>(pageNumber, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
            ViewData.Add(new KeyValuePair<string, object>("Numero", Numero));
            ViewData.Add(new KeyValuePair<string, object>("Contacto", Contacto));
            ViewData.Add(new KeyValuePair<string, object>("Servicio", Servicio));
            ViewData.Add(new KeyValuePair<string, object>("Accion", Accion));
            ViewData.Add(new KeyValuePair<string, object>("FechaAlta", FechaAlta));
            ViewData.Add(new KeyValuePair<string, object>("FechaEnvio", FechaEnvio));
            ViewData.Add(new KeyValuePair<string, object>("FechaEntrega", FechaEntrega));
            ViewData.Add(new KeyValuePair<string, object>("FechaPago", FechaPago));
            ViewData.Add(new KeyValuePair<string, object>("Costo", Costo));
            ViewData.Add(new KeyValuePair<string, object>("IdResponsable", IdResponsable));
            ViewData.Add(new KeyValuePair<string, object>("IdCuentaCorriente", IdCuentaCorriente));

            return PartialView("_EnProgreso", comisionesDebe);
        }


        public ActionResult SearchPagingComisionFinalizadas(string Numero, string Contacto, string Servicio, string Accion, string FechaAlta, string FechaEnvio, string FechaEntrega, string FechaPago, string Costo, int? IdResponsable, int? IdCuentaCorriente, int pageNumber)
        {
            List<Comision> comisiones = db.Comisiones.ToList();

            comisiones = ComisionHelper.searchComisiones(comisiones, Numero, Contacto, Servicio, Accion, ComisionHelper.getFecha(FechaAlta), ComisionHelper.getFecha(FechaEnvio), ComisionHelper.getFecha(FechaEntrega), ComisionHelper.getFecha(FechaPago), Costo, IdResponsable, IdCuentaCorriente);
            
            IPagedList<Comision> comisionesPagos = ComisionHelper.getFinalizadas(comisiones).OrderByDescending(x => x.ID).ToPagedList<Comision>(pageNumber, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
            ViewData.Add(new KeyValuePair<string, object>("Numero", Numero));
            ViewData.Add(new KeyValuePair<string, object>("Contacto", Contacto));
            ViewData.Add(new KeyValuePair<string, object>("Servicio", Servicio));
            ViewData.Add(new KeyValuePair<string, object>("Accion", Accion));
            ViewData.Add(new KeyValuePair<string, object>("FechaAlta", FechaAlta));
            ViewData.Add(new KeyValuePair<string, object>("FechaEnvio", FechaEnvio));
            ViewData.Add(new KeyValuePair<string, object>("FechaEntrega", FechaEntrega));
            ViewData.Add(new KeyValuePair<string, object>("FechaPago", FechaPago));
            ViewData.Add(new KeyValuePair<string, object>("Costo", Costo));
            ViewData.Add(new KeyValuePair<string, object>("IdResponsable", IdResponsable));
            ViewData.Add(new KeyValuePair<string, object>("IdCuentaCorriente", IdCuentaCorriente));

            return PartialView("_Finalizadas", comisionesPagos);
        }

        public ActionResult SpendingPagingComision(string descripcion, string fechaAlta , string monto , int pageNumber)
        {
            List<ComisionGasto> comisionGastos = db.ComisionGastos.ToList();

            comisionGastos = ComisionHelper.searchComisionGasto(comisionGastos, descripcion, ComisionHelper.getFecha(fechaAlta), ComisionHelper.getMonto(monto));

            IPagedList<ComisionGasto> comisionesPagos = comisionGastos.OrderByDescending(x => x.ID).ToPagedList<ComisionGasto>(pageNumber, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            ViewData.Add(new KeyValuePair<string, object>("descripcion", descripcion));
            ViewData.Add(new KeyValuePair<string, object>("fechaAlta", fechaAlta));
            ViewData.Add(new KeyValuePair<string, object>("monto", monto));

            return PartialView("_SpendingTable", comisionesPagos);
        }

        public void setEnvio(int idComision, int enviar)
        {
            if (idComision > 0)
            {
                Comision comision = db.Comisiones.Find(idComision);
                comision.Enviar = Convert.ToBoolean(enviar);

                db.Entry(comision).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}