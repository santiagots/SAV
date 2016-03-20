using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SAV.Models;
using SAV.Common;
using PagedList;
using System.Configuration;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace SAV.Controllers
{
    [Authorize]
    public class ViajeController : Controller
    {
        private SAVContext db = new SAVContext();

        public ActionResult Details(int id = 0)
        {
            Viaje viaje = db.Viajes.Find(id);
            if (viaje == null)
            {
                return HttpNotFound();
            }

            ViewBag.IdViaje = id;

            List<Conductor> conductor = db.Conductores.ToList<Conductor>();

            List<Localidad> localidad = db.Localidades.ToList<Localidad>();

            DetailsViajeViewModel detailsViajeViewModel = new DetailsViajeViewModel(viaje, conductor, localidad);

            return View(detailsViajeViewModel);
        }

        [HttpGet]
        public ActionResult Close(int id = 0)
        {
            Viaje viaje = db.Viajes.Find(id);
            if (viaje == null)
            {
                return HttpNotFound();
            }

            ViewBag.IdViaje = id;

            List<Conductor> conductor = db.Conductores.ToList<Conductor>();

            List<Localidad> localidad = db.Localidades.ToList<Localidad>();

            DetailsViajeViewModel detailsViajeViewModel = new DetailsViajeViewModel(viaje, conductor, localidad);

            return View(detailsViajeViewModel);
        }

        [HttpPost]
        public ActionResult Close(DetailsViajeViewModel detailsViajeViewModel, int idViaje)
        {
            Viaje viaje = db.Viajes.Find(idViaje);
            if (viaje == null)
            {
                return HttpNotFound();
            }

            viaje.Estado = ViajeEstados.Cerrado;

            db.Entry(viaje).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Search");
        }

        [HttpPost]
        public ActionResult Details(DetailsViajeViewModel detailsViajeViewModel, int idViaje)
        {

            Viaje viaje = db.Viajes.Find(idViaje);

            List<Conductor> conductores = db.Conductores.ToList<Conductor>();

            List<Localidad> localidades = db.Localidades.ToList<Localidad>();

            viaje.UpDate(detailsViajeViewModel.DatosBasicosViaje, conductores, localidades);

            db.Entry(viaje).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Search");
        }

        public ActionResult Create()
        {
            var conductores = db.Conductores.ToList<Conductor>();
            var localidades = db.Localidades.ToList<Localidad>();

            var CreateViajeViewModel = new CreateViajeViewModel(conductores, localidades);

            return View(CreateViajeViewModel);
        }

        [HttpPost]
        public ActionResult Create(CreateViajeViewModel createViajeViewModel)
        {
            var localidades = db.Localidades.ToList<Localidad>();

            var conductores = db.Conductores.ToList<Conductor>();

            var viaje = new Viaje().CreateViajes(createViajeViewModel, conductores, localidades);

            foreach (Viaje v in viaje)
                db.Viajes.Add(v);

            db.SaveChanges();

            return RedirectToAction("Search");
        }

        public ActionResult Edit(int id = 0)
        {
            Viaje viaje = db.Viajes.Find(id);
            if (viaje == null)
            {
                return HttpNotFound();
            }
            return View(viaje);
        }

        [HttpPost]
        public ActionResult Edit(Viaje viaje)
        {
            if (ModelState.IsValid)
            {
                db.Entry(viaje).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Search");
            }
            return View(viaje);
        }

        public ActionResult Search()
        {
            var viajes = db.Viajes.ToList<Viaje>();
            var localidades = db.Localidades.ToList<Localidad>();
            var SearchViaje = new SearchViajeViewModel();

            List<Viaje> viajesActivos = ViajeHelper.getViajesActivos(viajes);
            List<Viaje> viajesFinalizados = ViajeHelper.getViajesFinalizados(viajes);

            List<Localidad> destinos = ViajeHelper.getDestinos(localidades);

            SearchViaje.Destino = destinos.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList<KeyValuePair<int, string>>();
            SearchViaje.Origen = destinos.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList<KeyValuePair<int, string>>();

            SearchViaje.ViajesActivos = viajesActivos.OrderBy(x => x.FechaSalida).ToPagedList<Viaje>(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
            SearchViaje.ViajesFinalizados = viajesFinalizados.OrderBy(x => x.FechaSalida).ToPagedList<Viaje>(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return View("Search", SearchViaje);
        }

        [HttpPost]
        public ActionResult Search(SearchViajeViewModel searchViajeViewModel)
        {
            var viajes = db.Viajes.ToList<Viaje>();
            var localidades = db.Localidades.ToList<Localidad>();
            var SearchViaje = new SearchViajeViewModel();

            List<Viaje> viajesActivos = ViajeHelper.getViajesActivos(viajes);
            List<Viaje> viajesFinalizados = ViajeHelper.getViajesFinalizados(viajes);

            List<Localidad> destinos = ViajeHelper.getDestinos(localidades);

            SearchViaje.Destino = destinos.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList<KeyValuePair<int, string>>();
            SearchViaje.Origen = destinos.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList<KeyValuePair<int, string>>();

            viajesActivos = ViajeHelper.filtrarSerchViajesViewModel(viajes, searchViajeViewModel.SelectOrigen, searchViajeViewModel.SelectDestino, searchViajeViewModel.FechaSalida, searchViajeViewModel.Servicio);

            SearchViaje.ViajesActivos = viajesActivos.ToPagedList<Viaje>(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
            SearchViaje.ViajesFinalizados = viajesFinalizados.ToPagedList<Viaje>(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            SearchViaje.SelectDestino = searchViajeViewModel.SelectDestino ?? 0;
            SearchViaje.SelectOrigen = searchViajeViewModel.SelectOrigen ?? 0;
            SearchViaje.FechaSalida = searchViajeViewModel.FechaSalida;
            SearchViaje.Servicio = searchViajeViewModel.Servicio;

            SearchViaje.ViajesActivos.OrderBy(x => x.FechaSalida);
            SearchViaje.ViajesFinalizados.OrderBy(x => x.FechaSalida);

            return View("Search", SearchViaje);
        }

        public ActionResult ExportViaje(int id)
        {
            var viaje = db.Viajes.Find(id);

            string patente = viaje.Patente;
            string interno = viaje.Interno.ToString();
            string viajeID = viaje.ID.ToString();
            string origen = viaje.Servicio != ViajeTipoServicio.Cerrado ? viaje.Origen.Nombre : viaje.OrigenCerrado;
            string destino = viaje.Servicio != ViajeTipoServicio.Cerrado ? viaje.Destino.Nombre : viaje.DestinoCerrado;
            string salida = viaje.FechaSalida.ToString("dd/MM/yyyy HH:mm");
            string arribo = viaje.FechaArribo.ToString("dd/MM/yyyy HH:mm");

            List<Pasajeros> pasajeros = ViajeHelper.getPasajeros(viaje.ClienteViaje);

            List<Comisiones> comisiones = ViajeHelper.getComisiones(viaje.ComisionesViaje);

            if (viaje == null)
            {
                return HttpNotFound();
            }

            string path = System.Web.Hosting.HostingEnvironment.MapPath("~/Template/ViajeTemplate.xls");

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);

            HSSFWorkbook tamplateWorckbook = new HSSFWorkbook(fs, true);

            #region Pasajeros

            ISheet PasajerosSheet = tamplateWorckbook.GetSheet("Pasajeros");

            int PasajerosRow = 5;
            if (pasajeros.Count > 0)
            {
                List<ICell> HeadPasajerosCell = PasajerosSheet.GetRow(1).Cells;
                HeadPasajerosCell[0].SetCellValue(string.Format("Hoja de Viaje - Cod. Viaje: {0} - Patente: {1} - Interno: {2}", viajeID, patente, interno));

                HeadPasajerosCell = PasajerosSheet.GetRow(3).Cells;
                HeadPasajerosCell[0].SetCellValue(string.Format("Origen: {0} - Destino: {1}", origen, destino));
                HeadPasajerosCell[4].SetCellValue(string.Format("Salida: {0} - Arribo: {1}", salida, arribo));

                foreach (Pasajeros pasajero in pasajeros)
                {
                    if (PasajerosRow <= PasajerosSheet.LastRowNum)
                    {
                        List<ICell> pasajerosCell = PasajerosSheet.GetRow(PasajerosRow).Cells;
                        pasajerosCell[1].SetCellValue(pasajero.Apellido + " " + pasajero.Nombre);
                        pasajerosCell[2].SetCellValue(pasajero.DNI);
                        pasajerosCell[3].SetCellValue(pasajero.Ascenso);
                        pasajerosCell[4].SetCellValue(pasajero.Descenso);
                        pasajerosCell[5].SetCellValue(pasajero.Telefono);
                        pasajerosCell[6].SetCellValue(pasajero.Costo);
                        pasajerosCell[7].SetCellValue(pasajero.Pago ? "Si" : "No");
                    }
                    else
                    {
                        List<ICell> pasajerosCell = PasajerosSheet.CopyRow(PasajerosRow - 2, PasajerosRow).Cells;
                        pasajerosCell[0].SetCellValue(PasajerosRow - 4);
                        pasajerosCell[1].SetCellValue(pasajero.Apellido + " " + pasajero.Nombre);
                        pasajerosCell[2].SetCellValue(pasajero.DNI);
                        pasajerosCell[3].SetCellValue(pasajero.Ascenso);
                        pasajerosCell[4].SetCellValue(pasajero.Descenso);
                        pasajerosCell[5].SetCellValue(pasajero.Telefono);
                        pasajerosCell[6].SetCellValue(pasajero.Costo);
                        pasajerosCell[7].SetCellValue(pasajero.Pago ? "Si" : "No");
                    }
                    PasajerosRow++;
                }
            }
            else
            {
                tamplateWorckbook.SetSheetHidden(0, true);
                tamplateWorckbook.SetActiveSheet(1);
            }

            #endregion

            #region comisines

            ISheet ComisionestemplateSheet = tamplateWorckbook.GetSheet("Comisiones");
            Dictionary<string, List<Comisiones>> ComisionesPorResponsable = new Dictionary<string, List<Comisiones>>();
            if (comisiones.Count > 0)
            {
                foreach (Comisiones comision in comisiones)
                {
                    if (!ComisionesPorResponsable.ContainsKey(comision.Responsable))
                    {
                        ComisionesPorResponsable.Add(comision.Responsable, new List<Comisiones>());
                        ((HSSFSheet)ComisionestemplateSheet).CopyTo(tamplateWorckbook, "Comisiones " + comision.Responsable, true, true);
                    }
                    ComisionesPorResponsable[comision.Responsable].Add(comision);
                }

                foreach (var ComisionePorResponsable in ComisionesPorResponsable)
                {
                    ISheet ComisionesSheet = tamplateWorckbook.GetSheet("Comisiones " + ComisionePorResponsable.Key);
                    int ComisionesRow = 5;


                    List<ICell> HeadComisionesCell = ComisionesSheet.GetRow(1).Cells;
                    HeadComisionesCell[0].SetCellValue(string.Format("Hoja de Viaje - Cod. Viaje: {0} - Patente: {1} - Interno: {2}", viajeID, patente, interno));

                    HeadComisionesCell = ComisionesSheet.GetRow(2).Cells;
                    HeadComisionesCell[0].SetCellValue(string.Format("Comisiones: {0}", ComisionePorResponsable.Key));

                    HeadComisionesCell = ComisionesSheet.GetRow(3).Cells;
                    HeadComisionesCell[0].SetCellValue(string.Format("Origen: {0} - Destino: {1}", origen, destino));
                    HeadComisionesCell[4].SetCellValue(string.Format("Salida: {0} - Arribo: {1}", salida, arribo));

                    int ComisionIndex = 0;
                    foreach (Comisiones comision in ComisionePorResponsable.Value)
                    {
                        ComisionIndex++;

                        if (ComisionesSheet.GetRow(ComisionesRow) != null)
                        {
                            SetValueToComisionesCell(ComisionesSheet, ComisionesRow, comision, ComisionIndex);
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

                            SetValueToComisionesCell(ComisionesSheet, ComisionesRow, comision, ComisionIndex);
                        }
                        ComisionesRow += 2;

                    }
                }

            }
            else
            {
                //Muestro la primera hoja por si se oculto
                tamplateWorckbook.SetSheetHidden(0, false);
                tamplateWorckbook.SetActiveSheet(0);
            }

            //elimino la solapa de comisiones que uso como plantilla
            tamplateWorckbook.SetSheetHidden(1, true);


            #endregion

            string name = String.Format("{0}_{1}_{2}_{3}", viajeID, origen, destino, viaje.FechaSalida.ToString("dd-MM_HH:mm"));

            MemoryStream ms = new MemoryStream();

            tamplateWorckbook.Write(ms);

            return File(ms.ToArray(), "application/vnd.ms-excel", name + ".xls");
        }

        private static void SetValueToComisionesCell(ISheet ComisionesSheet, int ComisionesRow, Comisiones comision, int ComisionIndex)
        {
            List<ICell> comisionCell = ComisionesSheet.GetRow(ComisionesRow).Cells;

            comisionCell[0].SetCellValue(ComisionIndex);

            comisionCell[1].SetCellValue(comision.Contacto);
            comisionCell[2].SetCellValue(comision.Accion.ToString());
            if (comision.Servicio == ComisionServicio.Puerta)
            {
                comisionCell[3].SetCellValue(comision.RetirarPuerta);
                comisionCell[4].SetCellValue(comision.EntregarPuerta);
            }
            else
            {
                comisionCell[3].SetCellValue(comision.RetirarDirecto);
                comisionCell[4].SetCellValue(comision.EntregarDirecto);
            }
            comisionCell[5].SetCellValue(comision.Telefono);
            comisionCell[6].SetCellValue(comision.Costo);
            comisionCell[7].SetCellValue(comision.Pago ? "Si" : "No");

            List<ICell> comisionComentarioCell = ComisionesSheet.GetRow(ComisionesRow + 1).Cells;
            comisionComentarioCell[1].SetCellValue(String.Format("Comentario: {0}", comision.Comentaro));
        }

        public ActionResult AddCliente(int idCliente, int idViaje)
        {
            var viaje = db.Viajes.Find(idViaje);
            var cliente = db.Clientes.Find(idCliente);

            if (!viaje.ClienteViaje.Any(x => x.Cliente.ID == idCliente))
            {
                var clienteViaje = new ClienteViaje();

                clienteViaje.Viaje = viaje;
                clienteViaje.Cliente = cliente;

                db.ClienteViajes.Add(clienteViaje);
                db.SaveChanges();
            }
            return View("Details", viaje);
        }

        public ActionResult AddComision(int idComision, int idViaje)
        {
            var viaje = db.Viajes.Find(idViaje);
            var comision = db.Comisiones.Find(idComision);

            if (!viaje.ComisionesViaje.Any(x => x.ID == idComision))
            {
                var clienteViaje = new ComisionViaje();

                clienteViaje.Viaje = viaje;
                clienteViaje.Comision = comision;

                viaje.ComisionesViaje.Add(clienteViaje);
                db.SaveChanges();
            }
            return View("Details", viaje);
        }

        // GET: /Viaje/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Viaje viaje = db.Viajes.Find(id);
            if (viaje == null)
            {
                return HttpNotFound();
            }

            db.ClienteViajes.Where(x => x.Viaje.ID == id).ToList().ForEach(x => db.ClienteViajes.Remove(x));
            db.Viajes.Remove(viaje);
            db.SaveChanges();

            return RedirectToAction("Search");
        }

        public ActionResult DeleteGasto(int id, int idViaje)
        {
            ViewBag.IdViaje = idViaje;
            Viaje viaje = db.Viajes.Find(idViaje);
            Gasto gasto = db.Gastos.Find(id);
            if (gasto == null)
            {
                return HttpNotFound();
            }

            db.Gastos.Remove(gasto);
            db.SaveChanges();

            return PartialView("_CierreGastosTable", viaje.Gastos.ToPagedList<Gasto>(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"])));
        }

        public void setAsistenciaPasajeros(ICollection<Pasajeros> pasajeros)
        {
            if (pasajeros != null)
            {
                List<ClienteViaje> clienteViaje = db.ClienteViajes.ToList<ClienteViaje>();

                foreach (var item in pasajeros)
                {
                    ClienteViaje select = clienteViaje.Where(x => x.ID == item.ClienteViajeID).FirstOrDefault();
                    if (select != null)
                    {
                        if (select.Presente != item.Presente || select.Pago != item.Pago)
                        {
                            select.Presente = item.Presente;
                            select.Pago = item.Pago;
                            db.Entry(select).State = EntityState.Modified;
                        }
                    }
                }
                db.SaveChanges();
            }
        }

        public void setEntregaRetiraComision(ICollection<Comisiones> comisiones)
        {
            if (comisiones != null)
            {
                List<ComisionViaje> comisionViaje = db.ComisionViajes.ToList<ComisionViaje>();

                foreach (var item in comisiones)
                {
                    ComisionViaje select = comisionViaje.Where(x => x.ID == item.ComisionViajeID).FirstOrDefault();
                    if (select != null)
                    {
                        if (select.EntregaRetira != item.EntregaRetira || select.Pago != item.Pago)
                        {
                            select.EntregaRetira = item.EntregaRetira;
                            select.Pago = item.Pago;
                            db.Entry(select).State = EntityState.Modified;
                        }
                    }
                }
                db.SaveChanges();
            }
        }

        public ActionResult AddGasto(string razonSocial, string cuit, string nroTicket, string monto, string comentario, int idViaje)
        {
            Gasto gasto = new Gasto()
            {
                RazonSocial = razonSocial,
                CUIT = cuit,
                NroTicket = long.Parse(nroTicket),
                Monto = monto,
                Comentario = comentario
            };

            ViewBag.IdViaje = idViaje;

            Viaje viaje = db.Viajes.Find(idViaje);
            viaje.Gastos.Add(gasto);

            db.Gastos.Add(gasto);
            db.Entry(viaje).State = EntityState.Modified;
            db.SaveChanges();

            return PartialView("_CierreGastosTable", viaje.Gastos.ToPagedList<Gasto>(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"])));
        }

        public ActionResult DetailsPagingComisiones(int? IdViaje, int? pageNumber)
        {
            ViewBag.IdViaje = IdViaje;

            IPagedList<Comisiones> Comisiones = ViajeHelper.getComisiones(db.Viajes.Find(IdViaje).ComisionesViaje, pageNumber.Value);

            return PartialView("_ComisionesTable", Comisiones);
        }

        public ActionResult CierrePagingComisiones(int? IdViaje, int? pageNumber)
        {
            ViewBag.IdViaje = IdViaje;

            IPagedList<Comisiones> Comisiones = ViajeHelper.getComisiones(db.Viajes.Find(IdViaje).ComisionesViaje, pageNumber.Value);

            return PartialView("_CierreComisionesTable", Comisiones);
        }

        public ActionResult DetailsPagingPasajeros(int? IdViaje, int? pageNumber)
        {
            ViewBag.IdViaje = IdViaje;

            IPagedList<Pasajeros> pasajeros = ViajeHelper.getPasajeros(db.Viajes.Find(IdViaje).ClienteViaje, pageNumber.Value);

            return PartialView("_PasajerosTable", pasajeros);
        }

        public ActionResult CierrePagingPasajeros(int? IdViaje, int? pageNumber)
        {
            ViewBag.IdViaje = IdViaje;

            IPagedList<Pasajeros> pasajeros = ViajeHelper.getPasajeros(db.Viajes.Find(IdViaje).ClienteViaje, pageNumber.Value);

            return PartialView("_CierrePasajerosTable", pasajeros);
        }

        public ActionResult CierrePagingGastos(int? IdViaje, int? pageNumber)
        {
            ViewBag.IdViaje = IdViaje;

            return PartialView("_CierreGastosTable", db.Viajes.Find(IdViaje).Gastos.ToPagedList(pageNumber.Value, int.Parse(ConfigurationSettings.AppSettings["PageSize"])));
        }

        public ActionResult SearchPagingViajesAbiertos(int? pageNumber, string UpdateTargetId, int? origen, int? destino, string fechaSalida, string servicio)
        {
            ViewBag.idOrigen = origen;
            ViewBag.idDestino = destino;
            ViewBag.fechaSalida = fechaSalida;
            ViewBag.servicio = servicio;

            IPagedList<Viaje> viajesResult;

            viajesResult = ViajeHelper.filtrarSerchViajesViewModel(ViajeHelper.getViajesActivos(db.Viajes.ToList<Viaje>()), origen, destino, fechaSalida, servicio).ToPagedList<Viaje>(pageNumber.Value, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return PartialView("_ViajesAbiertosTable", viajesResult);
        }

        public ActionResult SearchPagingViajesCerrados(int? pageNumber)
        {
            IPagedList<Viaje> viajesResult;

            viajesResult = ViajeHelper.getViajesFinalizados(db.Viajes.ToList<Viaje>()).ToPagedList<Viaje>(pageNumber.Value, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return PartialView("_ViajesCerradosTable", viajesResult);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}