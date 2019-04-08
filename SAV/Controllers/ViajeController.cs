    using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using SAV.Models;
using SAV.Common;
using PagedList;
using System.Configuration;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Text;
using SAV.Filters;

namespace SAV.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class ViajeController : Controller
    {
        private SAVContext db = new SAVContext();

        public ActionResult Details(int? IdClienteViajePago, int id = 0)
        {
            Viaje viaje = db.Viajes.Find(id);
            if (viaje == null)
            {
                return HttpNotFound();
            }

            ViewBag.IdViaje = id;

            if(IdClienteViajePago.HasValue)
                ViewBag.IdClienteViajePago = IdClienteViajePago.Value;

            List<Conductor> conductor = db.Conductores.ToList<Conductor>();
            List<Localidad> destinos = db.Localidades.Where(x => x.Parada.Any()).ToList<Localidad>();
            List<Provincia> provincia = db.Provincias.ToList<Provincia>();
            List<ModalidadPrestacion> modalidadPrestacion = db.ModalidadPrestacion.ToList<ModalidadPrestacion>();
            Configuracion configuracion = db.Configuracion.First();

            DetailsViajeViewModel detailsViajeViewModel = new DetailsViajeViewModel(viaje, conductor, destinos, provincia, modalidadPrestacion, configuracion);

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
            List<Localidad> destinos = db.Localidades.Where(x => x.Parada.Any()).ToList<Localidad>();
            List<Provincia> provincia = db.Provincias.ToList<Provincia>();
            List<TipoGasto> TipoGasto = db.TipoGasto.Where(x => x.Concepto == ConceptoGasto.Viaje).ToList<TipoGasto>();
            List<TipoAdicionalConductor> TipoAdicional = db.TipoAdicionalConductor.ToList<TipoAdicionalConductor>();
            List<ModalidadPrestacion> modalidadPrestacion = db.ModalidadPrestacion.ToList<ModalidadPrestacion>();
            Configuracion configuracion = db.Configuracion.First();

            DetailsViajeViewModel detailsViajeViewModel = new DetailsViajeViewModel(viaje, conductor, destinos, provincia, TipoGasto, TipoAdicional, modalidadPrestacion, configuracion, true);

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
            List<Localidad> destinos = db.Localidades.Where(x => x.Parada.Any()).ToList<Localidad>();
            List<Provincia> provincias = db.Provincias.ToList<Provincia>();
            List<ModalidadPrestacion> modalidadPrestacion = db.ModalidadPrestacion.ToList<ModalidadPrestacion>();

            viaje.UpDate(detailsViajeViewModel.DatosBasicosViaje, conductores, destinos, provincias, modalidadPrestacion);

            db.Entry(viaje).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Search");
        }

        public ActionResult Create()
        {
            var conductores = db.Conductores.ToList<Conductor>();
            var destinos = db.Localidades.Where(x => x.Parada.Any()).ToList<Localidad>();
            var provincias = db.Provincias.ToList<Provincia>();
            var modalidadPrestacion = db.ModalidadPrestacion.ToList<ModalidadPrestacion>();

            var CreateViajeViewModel = new CreateViajeViewModel(conductores, destinos, provincias, modalidadPrestacion);

            return View(CreateViajeViewModel);
        }

        [HttpPost]
        public ActionResult Create(CreateViajeViewModel createViajeViewModel)
        {
            var destinos = db.Localidades.Where(x => x.Parada.Any()).ToList<Localidad>();
            var conductores = db.Conductores.ToList<Conductor>();
            var provincias = db.Provincias.ToList<Provincia>();
            var modalidadPrestacion = db.ModalidadPrestacion.ToList<ModalidadPrestacion>();

            var viaje = new Viaje().CreateViajes(createViajeViewModel, conductores, destinos, provincias, modalidadPrestacion);

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
            DateTime horaLocal = DateHelper.getLocal();
            var viajesActivos = db.Viajes.Where(x => x.FechaArribo.CompareTo(horaLocal) >= 0 && x.Estado == ViajeEstados.Abierto);
            var viajesFinalizados = db.Viajes.Where(x =>  x.FechaArribo.CompareTo(horaLocal) < 0 && x.Estado == ViajeEstados.Abierto);
            var destinos = db.Localidades.Where(x => x.Parada.Any()).ToList<Localidad>();
            var SearchViaje = new SearchViajeViewModel();

            SearchViaje.Destino = destinos.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList<KeyValuePair<int, string>>();
            SearchViaje.Origen = destinos.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList<KeyValuePair<int, string>>();

            SearchViaje.ViajesActivos = viajesActivos.OrderBy(x => x.FechaSalida).ToPagedList<Viaje>(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
            SearchViaje.ViajesFinalizados = viajesFinalizados.OrderBy(x => x.FechaSalida).ToPagedList<Viaje>(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return View("Search", SearchViaje);
        }

        [HttpPost]
        public ActionResult Search(SearchViajeViewModel searchViajeViewModel)
        {
            var SearchViaje = new SearchViajeViewModel();

            var destinos = db.Localidades.Where(x => x.Parada.Any()).ToList<Localidad>();

            SearchViaje.Destino = destinos.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList<KeyValuePair<int, string>>();
            SearchViaje.Origen = destinos.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList<KeyValuePair<int, string>>();

            var viajesActivos = ViajeHelper.filtrarSerchViajesViewModel(db.Viajes, searchViajeViewModel.SelectOrigen, searchViajeViewModel.SelectDestino, searchViajeViewModel.FechaSalida, searchViajeViewModel.Servicio, searchViajeViewModel.Codigo, searchViajeViewModel.Cliente, searchViajeViewModel.Estado);
            var viajesFinalizados = db.Viajes.Where(x => x.FechaArribo.CompareTo(DbFunctions.AddHours(DateTime.Now, 4).Value) < 0 && x.Estado == ViajeEstados.Abierto);

            SearchViaje.ViajesActivos = viajesActivos.OrderBy(x => x.FechaSalida).ToPagedList<Viaje>(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
            SearchViaje.ViajesFinalizados = viajesFinalizados.OrderBy(x => x.FechaSalida).ToPagedList<Viaje>(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            SearchViaje.SelectDestino = searchViajeViewModel.SelectDestino ?? 0;
            SearchViaje.SelectOrigen = searchViajeViewModel.SelectOrigen ?? 0;
            SearchViaje.FechaSalida = searchViajeViewModel.FechaSalida;
            SearchViaje.Servicio = searchViajeViewModel.Servicio;

            SearchViaje.ViajesActivos.OrderBy(x => x.FechaSalida);
            SearchViaje.ViajesFinalizados.OrderBy(x => x.FechaSalida);

            ViewBag.idOrigen = searchViajeViewModel.Origen;
            ViewBag.idDestino = searchViajeViewModel.SelectDestino;
            ViewBag.fechaSalida = searchViajeViewModel.FechaSalida;
            ViewBag.servicio = searchViajeViewModel.Servicio;
            ViewBag.codigo = searchViajeViewModel.Codigo;
            ViewBag.nombrePasajero = searchViajeViewModel.Cliente;
            ViewBag.estadoViaje = searchViajeViewModel.Estado;

            return View("Search", SearchViaje);
        }

        public ActionResult ExportViaje(int id)
        {
            var viaje = db.Viajes.Find(id);
            Configuracion configuracion = db.Configuracion.First();

            string conductor;
            if (viaje.Conductor != null)
                conductor = string.Format("{0} {1} ({2})", viaje.Conductor.Apellido, viaje.Conductor.Nombre, viaje.Conductor.CUIL);
            else
                conductor = "Conductor no asignado";
            string patente = viaje.Patente;
            string patenteSuplente = viaje.PatenteSuplente;
            string interno = viaje.Interno.ToString();
            string viajeID = viaje.ID.ToString();
            string origen = viaje.Servicio != ViajeTipoServicio.Cerrado ? viaje.Origen.Nombre : viaje.OrigenCerrado;
            string destino = viaje.Servicio != ViajeTipoServicio.Cerrado ? viaje.Destino.Nombre : viaje.DestinoCerrado;
            string salida = viaje.FechaSalida.ToString("dd/MM/yyyy HH:mm");
            string arribo = viaje.FechaArribo.ToString("dd/MM/yyyy HH:mm");

            List<Pasajeros> pasajeros = ViajeHelper.getPasajerosViajeAbierto(viaje.ClienteViaje, configuracion, viaje.Asientos);

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
                HeadPasajerosCell[0].SetCellValue(string.Format("Hoja de Viaje - Cod. Viaje: {0} - Patente: {1} - Suplente: {2} - Interno: {3} - Conductor: {4}", viajeID, patente, patenteSuplente, interno, conductor));

                HeadPasajerosCell = PasajerosSheet.GetRow(3).Cells;
                HeadPasajerosCell[0].SetCellValue(string.Format("Origen: {0} - Destino: {1}", origen, destino));
                HeadPasajerosCell[4].SetCellValue(string.Format("Salida: {0} - Arribo: {1}", salida, arribo));

                foreach (Pasajeros pasajero in pasajeros)
                {
                    if (PasajerosRow <= PasajerosSheet.LastRowNum)
                    {
                        List<ICell> pasajerosCell = PasajerosSheet.GetRow(PasajerosRow).Cells;
                        pasajerosCell[1].SetCellValue(pasajero.Apellido + " " + pasajero.Nombre);
                        pasajerosCell[2].SetCellValue(pasajero.NumeroDocumento);
                        pasajerosCell[3].SetCellValue(pasajero.Ascenso);
                        pasajerosCell[4].SetCellValue(pasajero.Descenso);
                        pasajerosCell[5].SetCellValue(string.IsNullOrEmpty(pasajero.TelefonoAlternativo)? pasajero.Telefono : $"{pasajero.Telefono}\n{pasajero.TelefonoAlternativo}");
                        pasajerosCell[6].SetCellValue(pasajero.Costo);
                        pasajerosCell[7].SetCellValue(pasajero.Pago ? "Si" : "No");
                    }
                    else
                    {
                        List<ICell> pasajerosCell = PasajerosSheet.CopyRow(PasajerosRow - 2, PasajerosRow).Cells;
                        pasajerosCell[0].SetCellValue(PasajerosRow - 4);
                        pasajerosCell[1].SetCellValue(pasajero.Apellido + " " + pasajero.Nombre);
                        pasajerosCell[2].SetCellValue(pasajero.NumeroDocumento);
                        pasajerosCell[3].SetCellValue(pasajero.Ascenso);
                        pasajerosCell[4].SetCellValue(pasajero.Descenso);
                        pasajerosCell[5].SetCellValue(string.IsNullOrEmpty(pasajero.TelefonoAlternativo) ? pasajero.Telefono : $"{pasajero.Telefono}\n{pasajero.TelefonoAlternativo}");
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

            string name = String.Format("{0}_{1}_{2}_{3}", viajeID, origen, destino, viaje.FechaSalida.ToString("dd-MM_HH:mm"));

            MemoryStream ms = new MemoryStream();

            tamplateWorckbook.Write(ms);

            return File(ms.ToArray(), "application/vnd.ms-excel", name + ".xls");
        }

        public ActionResult ExportViajeCNRT(int id)
        {
            var viaje = db.Viajes.Find(id);
            Configuracion configuracion = db.Configuracion.First();

            string origen = viaje.Servicio != ViajeTipoServicio.Cerrado ? viaje.Origen.Nombre : viaje.OrigenCerrado;
            //string provinciaOrigen = viaje.Servicio != ViajeTipoServicio.Cerrado ? db.Provincias.Where(x => x.Localidad.Any(y => y.ID == viaje.Origen.ID)).First().Codigo : viaje.ProvienciaOrigenCerrado.Codigo;
            string destino = viaje.Servicio != ViajeTipoServicio.Cerrado ? viaje.Destino.Nombre : viaje.DestinoCerrado;
            //string provinciaDestino = viaje.Servicio != ViajeTipoServicio.Cerrado ? db.Provincias.Where(x => x.Localidad.Any(y => y.ID == viaje.Destino.ID)).First().Codigo : viaje.ProvienciaDestinoCerrado.Codigo;

            List<CNRTPasajero> cnrtPasajeros = new List<CNRTPasajero>();
            int i = 0;

            foreach (ClienteViaje clienteViaje in viaje.ClienteViaje.OrderBy(x => x.NumeroAsiento))
            {
                i++;
                cnrtPasajeros.Add(new CNRTPasajero()
                {
                    apellido = clienteViaje.Cliente.Apellido,
                    nombre = clienteViaje.Cliente.Nombre,
                    tipo_documento = clienteViaje.Cliente.TipoDocumento.ToString(),
                    numero_documento = clienteViaje.Cliente.NumeroDocumento.ToString(),
                    sexo = clienteViaje.Cliente.Sexo == Sexo.Femenino ? "F" : clienteViaje.Cliente.Sexo == Sexo.Masculino ? "M" : "O",
                    menor = clienteViaje.Cliente.Edad == Edad.Menor ? "1" : "0",
                    nacionalidad = clienteViaje.Cliente.Nacionalidad,
                    tripulante = "0"
                });
            }

            StringBuilder archivoCVS = new StringBuilder();

            archivoCVS.Append(ConvertirCVSHelper.ToCsv<CNRTPasajero>(";", cnrtPasajeros));

            string name = String.Format("CNRT{0}_{1}_{2}_{3}", viaje.ID, origen, destino, viaje.FechaSalida.ToString("dd-MM_HH:mm"));

            return File(Encoding.UTF8.GetBytes(archivoCVS.ToString()), "text/csv", name + ".csv");
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

        public ActionResult Delete(int id = 0)
        {
            Viaje viaje = db.Viajes.Find(id);
            if (viaje == null)
            {
                return HttpNotFound();
            }
            viaje.ClienteViaje.Clear();
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

        public ActionResult DeleteAdicionalConductor(int id, int idViaje)
        {
            ViewBag.IdViaje = idViaje;
            Viaje viaje = db.Viajes.Find(idViaje);
            AdicionalConductor adicionalConductor = db.AdicionalConductor.Find(id);
            if (adicionalConductor == null)
            {
                return HttpNotFound();
            }

            db.AdicionalConductor.Remove(adicionalConductor);
            db.SaveChanges();

            return PartialView("_CierreGastosTable", viaje.Gastos.ToPagedList<Gasto>(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"])));
        }

        public void setAsistenciaPasajeros(int clienteViajeID, int pago, int presente)
        {
            if (clienteViajeID > 0)
            {
                ClienteViaje clienteViaje = db.ClienteViajes.Find(clienteViajeID);

                if (clienteViaje != null)
                {
                    RegistroViaje registroPrensente = RegistroViajeHelper.GetRegistroPresente(User.Identity.Name.ToUpper(), clienteViaje, clienteViaje.Presente, Convert.ToBoolean(presente));
                    if(registroPrensente != null)
                        db.RegistroViaje.Add(registroPrensente);

                    clienteViaje.Presente = Convert.ToBoolean(presente);

                    if (clienteViaje.Pago != Convert.ToBoolean(pago) && clienteViaje.Viaje.Estado == ViajeEstados.Abierto)
                    {
                        RegistroViaje registroPago = RegistroViajeHelper.GetRegistroPago(User.Identity.Name.ToUpper(), clienteViaje, clienteViaje.Pago, Convert.ToBoolean(pago));
                        if (registroPago != null)
                            db.RegistroViaje.Add(registroPago);

                        clienteViaje.FormaPago = Convert.ToBoolean(pago)? db.FormaPago.Where(x => x.Descripcion.ToUpper() == "EFECTIVO").FirstOrDefault() : null;
                        clienteViaje.Pago = Convert.ToBoolean(pago);
                        clienteViaje.FechaPago = Convert.ToBoolean(pago) ? DateHelper.getLocal() : (DateTime?) null;
                        clienteViaje.VendedorCobro = Convert.ToBoolean(pago) ? User.Identity.Name.ToUpper() : "";
                    }
                    db.Entry(clienteViaje).State = EntityState.Modified;
                }

                db.SaveChanges();
            }
        }

        public void Actualizar(string patente, string PatenteSuplente, string interno, int idViaje)
        {
            ViewBag.IdViaje = idViaje;

            Viaje viaje = db.Viajes.Find(idViaje);
            viaje.Interno = int.Parse(interno);
            viaje.Patente = patente;
            viaje.PatenteSuplente = PatenteSuplente;

            db.Entry(viaje).State = EntityState.Modified;
            db.SaveChanges();
        }

        public ActionResult AddAdicionalConductor(int idTipoAdicional, decimal monto, string comentario, int idViaje)
        {
            Viaje viaje = db.Viajes.Find(idViaje);

            AdicionalConductor adicionalConductor = new AdicionalConductor()
            {
                Conductor = viaje.Conductor,
                Viaje = viaje,
                Monto = monto,
                Comentario = comentario,
                FechaAlta = DateHelper.getLocal(),
                UsuarioAlta = User.Identity.Name.ToUpper(),
                TipoAdicionalConductor = db.TipoAdicionalConductor.Find(idTipoAdicional)
            };

            ViewBag.IdViaje = idViaje;

            db.AdicionalConductor.Add(adicionalConductor);
            db.SaveChanges();

            return PartialView("_CierreAdicionalConductor", viaje.AdicionalConductor.ToPagedList<AdicionalConductor>(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"])));
        }

        public ActionResult AddGasto(int idTipoGasto, decimal monto, string comentario, int idViaje)
        {

            TipoGasto tipoGasto = db.TipoGasto.Find(idTipoGasto);

            Gasto gasto = new Gasto()
            {
                Concepto = ConceptoGasto.Viaje,
                TipoGasto = tipoGasto,
                Monto = monto,
                Comentario = comentario,
                FechaAlta = DateHelper.getLocal(),
                UsuarioAlta = User.Identity.Name.ToUpper()
            };

            ViewBag.IdViaje = idViaje;

            Viaje viaje = db.Viajes.Find(idViaje);
            viaje.Gastos.Add(gasto);

            db.Gastos.Add(gasto);
            db.Entry(viaje).State = EntityState.Modified;
            db.SaveChanges();

            return PartialView("_CierreGastosTable", viaje.Gastos.ToPagedList<Gasto>(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"])));
        }

        public ActionResult DetailsPagingPasajeros(int? IdViaje, int? pageNumber)
        {
            ViewBag.IdViaje = IdViaje;
            Viaje viaje = db.Viajes.Find(IdViaje);
            Configuracion configuracion = db.Configuracion.First();

            IPagedList<Pasajeros> pasajeros = ViajeHelper.getPasajerosViajeAbierto(viaje.ClienteViaje, viaje.Asientos, pageNumber.Value, int.Parse(ConfigurationSettings.AppSettings["PageSize"]), configuracion);

            return PartialView("_PasajerosTable", pasajeros);
        }

        public ActionResult CierrePagingPasajeros(int? IdViaje, int? pageNumber, DateTime time)
        {
            ViewBag.IdViaje = IdViaje;
            Viaje viaje = db.Viajes.Find(IdViaje);

            IPagedList<Pasajeros> pasajeros = ViajeHelper.getPasajerosViajeCerrado(viaje.ClienteViaje, viaje.Asientos, pageNumber.Value, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return PartialView("_CierrePasajerosTable", pasajeros);
        }

        public ActionResult CierrePagingPasajerosViajeCerrado(int? IdViaje, int? pageNumber, DateTime time)
        {
            ViewBag.IdViaje = IdViaje;
            Viaje viaje = db.Viajes.Find(IdViaje);

            IPagedList<Pasajeros> pasajeros = ViajeHelper.getPasajerosViajeCerrado(viaje.ClienteViaje, viaje.Asientos, pageNumber.Value, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return PartialView("_CierrePasajerosViajeCerradoTable", pasajeros);
        }

        public ActionResult CierrePagingAdicionalConductor(int? IdViaje, int? pageNumber)
        {
            ViewBag.IdViaje = IdViaje;

            Viaje viaje = db.Viajes.Find(IdViaje);

            return PartialView("_CierreGastosTable", db.AdicionalConductor.Where(x => x.Viaje.ID == viaje.ID && x.Conductor.ID == viaje.Conductor.ID).ToPagedList(pageNumber.Value, int.Parse(ConfigurationSettings.AppSettings["PageSize"])));
        }

        public ActionResult CierrePagingGastos(int? IdViaje, int? pageNumber)
        {
            ViewBag.IdViaje = IdViaje;

            return PartialView("_CierreGastosTable", db.Viajes.Find(IdViaje).Gastos.ToPagedList(pageNumber.Value, int.Parse(ConfigurationSettings.AppSettings["PageSize"])));
        }

        public ActionResult SearchPagingViajesAbiertos(int? pageNumber, int? origen, int? destino, string fechaSalida, string servicio, int? codigo, string nombrePasajero, string estadoViaje)
        {
            ViewBag.idOrigen = origen;
            ViewBag.idDestino = destino;
            ViewBag.fechaSalida = fechaSalida;
            ViewBag.servicio = servicio;
            ViewBag.codigo = codigo;
            ViewBag.nombrePasajero = nombrePasajero;
            ViewBag.estadoViaje = estadoViaje;

            IPagedList<Viaje> viajesResult;

            var viajesActivos = db.Viajes.Where(x => x.FechaArribo.CompareTo(DateTime.Now) >= 0 && x.Estado == ViajeEstados.Abierto);

            viajesResult = ViajeHelper.filtrarSerchViajesViewModel(viajesActivos, origen, destino, fechaSalida, servicio, codigo, nombrePasajero, estadoViaje).OrderBy(x => x.FechaSalida).ToPagedList<Viaje>(pageNumber.Value, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return PartialView("_ViajesAbiertosTable", viajesResult);
        }

        public ActionResult SearchPagingViajesCerrados(int? pageNumber)
        {
            IPagedList<Viaje> viajesResult;

            var viajesFinalizados = db.Viajes.Where(x => x.FechaArribo.CompareTo(DbFunctions.AddHours(DateTime.Now, 4).Value) < 0 && x.Estado == ViajeEstados.Abierto);
            viajesResult = viajesFinalizados.OrderBy(x => x.FechaSalida).ToPagedList<Viaje>(pageNumber.Value, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return PartialView("_ViajesCerradosTable", viajesResult);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}