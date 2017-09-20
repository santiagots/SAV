﻿using System;
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
using System.Text.RegularExpressions;
using System.Text;

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
            List<Provincia> provincia = db.Provincias.ToList<Provincia>();

            DetailsViajeViewModel detailsViajeViewModel = new DetailsViajeViewModel(viaje, conductor, localidad, provincia);

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
            List<Provincia> provincia = db.Provincias.ToList<Provincia>();

            DetailsViajeViewModel detailsViajeViewModel = new DetailsViajeViewModel(viaje, conductor, localidad, provincia);

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
            List<Provincia> provincias = db.Provincias.ToList<Provincia>();

            viaje.UpDate(detailsViajeViewModel.DatosBasicosViaje, conductores, localidades, provincias);

            db.Entry(viaje).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Search");
        }

        public ActionResult Create()
        {
            var conductores = db.Conductores.ToList<Conductor>();
            var localidades = db.Localidades.ToList<Localidad>();
            var provincias = db.Provincias.ToList<Provincia>();

            var CreateViajeViewModel = new CreateViajeViewModel(conductores, localidades, provincias);

            return View(CreateViajeViewModel);
        }

        [HttpPost]
        public ActionResult Create(CreateViajeViewModel createViajeViewModel)
        {
            var localidades = db.Localidades.ToList<Localidad>();
            var conductores = db.Conductores.ToList<Conductor>();
            var provincias = db.Provincias.ToList<Provincia>();

            var viaje = new Viaje().CreateViajes(createViajeViewModel, conductores, localidades, provincias);

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

            viajesActivos = ViajeHelper.filtrarSerchViajesViewModel(viajes, searchViajeViewModel.SelectOrigen, searchViajeViewModel.SelectDestino, searchViajeViewModel.FechaSalida, searchViajeViewModel.Servicio, searchViajeViewModel.Codigo);

            SearchViaje.ViajesActivos = viajesActivos.ToPagedList<Viaje>(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
            SearchViaje.ViajesFinalizados = viajesFinalizados.ToPagedList<Viaje>(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

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

            return View("Search", SearchViaje);
        }

        public ActionResult ExportViaje(int id)
        {
            var viaje = db.Viajes.Find(id);

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

            List<Pasajeros> pasajeros = ViajeHelper.getPasajeros(viaje.ClienteViaje);

            //List<Comisiones> comisiones = ViajeHelper.getComisiones(viaje.ComisionesViaje);

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

            string name = String.Format("{0}_{1}_{2}_{3}", viajeID, origen, destino, viaje.FechaSalida.ToString("dd-MM_HH:mm"));

            MemoryStream ms = new MemoryStream();

            tamplateWorckbook.Write(ms);

            return File(ms.ToArray(), "application/vnd.ms-excel", name + ".xls");
        }

        public ActionResult ExportViajeCNRT(int id)
        {
            var viaje = db.Viajes.Find(id);
            var configuracion = db.Configuracion.FirstOrDefault();

            if (configuracion == null)
            {
                configuracion = new Configuracion();
            }

            string origen = viaje.Servicio != ViajeTipoServicio.Cerrado ? viaje.Origen.Nombre : viaje.OrigenCerrado;
            string provinciaOrigen = viaje.Servicio != ViajeTipoServicio.Cerrado ? db.Provincias.Where(x => x.Localidad.Any(y => y.ID == viaje.Origen.ID)).First().Codigo : viaje.ProvienciaOrigenCerrado.Codigo;
            string destino = viaje.Servicio != ViajeTipoServicio.Cerrado ? viaje.Destino.Nombre : viaje.DestinoCerrado;
            string provinciaDestino = viaje.Servicio != ViajeTipoServicio.Cerrado ? db.Provincias.Where(x => x.Localidad.Any(y => y.ID == viaje.Destino.ID)).First().Codigo : viaje.ProvienciaDestinoCerrado.Codigo;

            CNRTViaje cnrtViaje = new CNRTViaje()
            {
                fecha_inicio = viaje.FechaSalida.ToString("dd/MM/yyyy HH:mm"),
                fecha_fin = viaje.FechaArribo.ToString("dd/MM/yyyy HH:mm"),
                origen = origen,
                provincia_origen = provinciaOrigen,
                destino = destino,
                provincia_destino = provinciaDestino,
                dominio = viaje.Patente,
                dominio_suplente = viaje.PatenteSuplente,
                tripulante_1_cuit = viaje.Conductor.CUIL != null? viaje.Conductor.CUIL.Replace("-", ""): null,
                tripulante_1_nombre = viaje.Conductor.Nombre,
                tripulante_1_apellido = viaje.Conductor.Apellido,
                tripulante_1_es_chofer = "1",
                contratante_denominacion = configuracion.ContratanteDenominacion,
                contenido_programacion_turistica = configuracion.ContenidoProgramacionTuristica,
                contratante_cuit = configuracion.ContrtanteCuit != null? configuracion.ContrtanteCuit.Replace("-", ""): null,
                contratante_domicilio = (configuracion.ContratanteDomicilio != null)? string.Format("{0} {1}", configuracion.ContratanteDomicilio.Calle, configuracion.ContratanteDomicilio.Numero)  : ""
            };

            List<CNRTPasajero> cnrtPasajeros = new List<CNRTPasajero>();

            int butaca = 1;
            foreach (ClienteViaje clienteViaje in viaje.ClienteViaje)
            {
                cnrtPasajeros.Add(new CNRTPasajero()
                {
                    tipo_documento = "1",
                    numero_documento = clienteViaje.Cliente.DNI.ToString(),
                    nombre = clienteViaje.Cliente.Nombre,
                    apellido = clienteViaje.Cliente.Apellido,
                    sexo = clienteViaje.Cliente.Sexo == Sexo.Femenino? "F": clienteViaje.Cliente.Sexo == Sexo.Masculino? "M": "O",
                    menor = clienteViaje.Cliente.Edad == Edad.Menor? "1": "0",
                    origen = origen,
                    provincia_origen = provinciaOrigen,
                    destino = destino,
                    provincia_destino = provinciaDestino,
                    nacionalidad = clienteViaje.Cliente.Nacionalidad,
                    numero_butaca = butaca,
                    numero_boleto = clienteViaje.ID
                });

                butaca++;
            }

            StringBuilder archivoCVS = new StringBuilder();

            archivoCVS.AppendLine("clase_modalidad");
            archivoCVS.AppendLine("TU");
            archivoCVS.Append(ConvertirCVSHelper.ToCsv<CNRTViaje>(";", new List<CNRTViaje>() { cnrtViaje }));
            archivoCVS.AppendLine("pasajeros_ini");
            archivoCVS.Append(ConvertirCVSHelper.ToCsv<CNRTPasajero>(";", cnrtPasajeros));
            archivoCVS.AppendLine("pasajeros_fin");

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

        public void setAsistenciaPasajeros(int clienteViajeID, int pago, int presente)
        {
            if (clienteViajeID > 0)
            {
                List<ClienteViaje> clienteViaje = db.ClienteViajes.ToList<ClienteViaje>();


                ClienteViaje select = clienteViaje.Where(x => x.ID == clienteViajeID).FirstOrDefault();
                if (select != null)
                {
                    select.Presente = Convert.ToBoolean(presente);
                    select.Pago = Convert.ToBoolean(pago);
                    db.Entry(select).State = EntityState.Modified;
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

        //public ActionResult DetailsPagingComisiones(int? IdViaje, int? pageNumber)
        //{
        //    ViewBag.IdViaje = IdViaje;

        //    IPagedList<Comisiones> Comisiones = ViajeHelper.getComisiones(db.Viajes.Find(IdViaje).ComisionesViaje, pageNumber.Value);

        //    return PartialView("_ComisionesTable", Comisiones);
        //}

        //public ActionResult CierrePagingComisiones(int? IdViaje, int? pageNumber)
        //{
        //    ViewBag.IdViaje = IdViaje;

        //    IPagedList<Comisiones> Comisiones = ViajeHelper.getComisiones(db.Viajes.Find(IdViaje).ComisionesViaje, pageNumber.Value);

        //    return PartialView("_CierreComisionesTable", Comisiones);
        //}

        public ActionResult DetailsPagingPasajeros(int? IdViaje, int? pageNumber)
        {
            ViewBag.IdViaje = IdViaje;

            IPagedList<Pasajeros> pasajeros = ViajeHelper.getPasajeros(db.Viajes.Find(IdViaje).ClienteViaje, pageNumber.Value);

            return PartialView("_PasajerosTable", pasajeros);
        }

        public ActionResult CierrePagingPasajeros(int? IdViaje, int? pageNumber, DateTime time)
        {
            ViewBag.IdViaje = IdViaje;

            IPagedList<Pasajeros> pasajeros = ViajeHelper.getPasajeros(db.Viajes.Find(IdViaje).ClienteViaje, pageNumber.Value);

            return PartialView("_CierrePasajerosTable", pasajeros);
        }

        public ActionResult CierrePagingPasajerosViajeCerrado(int? IdViaje, int? pageNumber, DateTime time)
        {
            ViewBag.IdViaje = IdViaje;

            IPagedList<Pasajeros> pasajeros = ViajeHelper.getPasajeros(db.Viajes.Find(IdViaje).ClienteViaje, pageNumber.Value);

            return PartialView("_CierrePasajerosViajeCerradoTable", pasajeros);
        }

        public ActionResult CierrePagingGastos(int? IdViaje, int? pageNumber)
        {
            ViewBag.IdViaje = IdViaje;

            return PartialView("_CierreGastosTable", db.Viajes.Find(IdViaje).Gastos.ToPagedList(pageNumber.Value, int.Parse(ConfigurationSettings.AppSettings["PageSize"])));
        }

        public ActionResult SearchPagingViajesAbiertos(int? pageNumber, string UpdateTargetId, int? origen, int? destino, string fechaSalida, string servicio, int codigo)
        {
            ViewBag.idOrigen = origen;
            ViewBag.idDestino = destino;
            ViewBag.fechaSalida = fechaSalida;
            ViewBag.servicio = servicio;
            ViewBag.codigo = codigo;

            IPagedList<Viaje> viajesResult;

            viajesResult = ViajeHelper.filtrarSerchViajesViewModel(ViajeHelper.getViajesActivos(db.Viajes.ToList<Viaje>()), origen, destino, fechaSalida, servicio, codigo).ToPagedList<Viaje>(pageNumber.Value, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

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