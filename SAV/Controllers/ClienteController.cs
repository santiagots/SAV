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
using System.Data.Objects;

namespace SAV.Controllers
{
    [Authorize]
    public class ClienteController : Controller
    {
        private SAVContext db = new SAVContext();
        private Object clienteViajesLock= new Object();

        public ActionResult GetLocalidades(int IdProvincia)
        {
            Provincia provincia = db.Provincias.Where(x => x.ID == IdProvincia).FirstOrDefault();

            if (HttpContext.Request.IsAjaxRequest())
                return Json(new SelectList(provincia.Localidad.OrderBy(x => x.Nombre), "ID", "Nombre"), JsonRequestBehavior.AllowGet);

            return View(provincia);
        }

        public ActionResult ImprimirBoleto(string IdViaje, string Servicio, string Apellido, string Nombre, string DNI, string Origen, string Destino, string Fecha)
        {
            ViewBag.IdViaje = IdViaje;
            ViewBag.Servicio = Servicio;
            ViewBag.Apellido = Apellido;
            ViewBag.Nombre = Nombre;
            ViewBag.DNI = DNI;
            ViewBag.Origen = Origen;
            ViewBag.Destino = Destino;
            ViewBag.FechaHora = Fecha;

            return new Rotativa.ViewAsPdf("Boleto");
            //return View("Boleto");
        }

        public ActionResult Search(int? IdViaje)
        {
            if(IdViaje.HasValue)
            {
                ViewBag.IdViaje = IdViaje.Value;

                var viajesFinalizados = db.Viajes.Where(x => x.FechaArribo.CompareTo(EntityFunctions.AddHours(DateTime.Now, 4).Value) < 0 && x.Estado == ViajeEstados.Abierto);

                if (viajesFinalizados.Where(x => x.ID == IdViaje.Value).Any())
                    ViewBag.From = "Cierre";
            }

            SearchClienteViewModel searchClienteViewModel = new SearchClienteViewModel();
            searchClienteViewModel.Clientes = new PagedList<Cliente>(null, 1, 1);

            return View(searchClienteViewModel);
        }

        [HttpPost]
        public ActionResult Search(SearchClienteViewModel searchClienteViewModel, int? IdViaje)
        {
            ViewBag.nombre = searchClienteViewModel.Nombre;
            ViewBag.apellido = searchClienteViewModel.Apellido;
            ViewBag.dni = searchClienteViewModel.DNI;
            ViewBag.telefono = searchClienteViewModel.Telefono;

            if (IdViaje.HasValue)
                ViewBag.IdViaje = IdViaje.Value;

            List<Cliente> cliente = db.Clientes.ToList<Cliente>();

            cliente = ClienteHelper.searchClientes(cliente, searchClienteViewModel.Apellido, searchClienteViewModel.Nombre, searchClienteViewModel.DNI, searchClienteViewModel.Telefono);

            cliente = cliente.OrderBy(x => x.Apellido).ThenBy(x => x.Nombre).ToList<Cliente>();

            searchClienteViewModel.Clientes = cliente.ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return View(searchClienteViewModel);
        }

        public ActionResult Create(int? IdViaje)
        {
            ViewBag.Action = "Create";

            ClienteViewModel clienteViewModel = new ClienteViewModel();
            List<Provincia> provincias = db.Provincias.ToList();

            if (IdViaje.HasValue)
            {
                ViewBag.IdViaje = IdViaje.Value;
                Viaje viaje = db.Viajes.Find(IdViaje);
                clienteViewModel = new ClienteViewModel(provincias, viaje);
                ViewBag.Servicio = viaje.Servicio;
            }
            else
                clienteViewModel = new ClienteViewModel(provincias);

            return View(clienteViewModel);
        }

        [HttpPost]
        public ActionResult Create(ClienteViewModel clienteViewModel, int? idViaje)
        {
            lock(clienteViajesLock)
            {
                List<Provincia> provincias = db.Provincias.ToList<Provincia>();
                List<Localidad> localidades = db.Localidades.ToList<Localidad>();
                List<Parada> paradas = db.Paradas.ToList<Parada>();
                List<FormaPago> formaPagos = db.FormaPago.Where(x => x.Habilitado).ToList();
                Cliente cliente = clienteViewModel.getCliente(clienteViewModel, provincias, localidades);

                if (idViaje.HasValue)
                {
                    Viaje viaje = db.Viajes.Find(idViaje);
                  
                    if (viaje.tieneLugar())
                    {
                        ClienteViaje clienteViaje = clienteViewModel.getClienteViaje(clienteViewModel, viaje, paradas, cliente, localidades, provincias, formaPagos, User.Identity.Name);
                        db.ClienteViajes.Add(clienteViaje);
                        db.Clientes.Add(cliente);
                        db.SaveChanges();

                        var viajesFinalizados = db.Viajes.Where(x => x.FechaArribo.CompareTo(EntityFunctions.AddHours(DateTime.Now, 4).Value) < 0 && x.Estado == ViajeEstados.Abierto);

                        if (viajesFinalizados.Where(x => x.ID == idViaje.Value).Any())
                            return RedirectToAction("Close", "Viaje", new { id = idViaje });
                        else
                            return RedirectToAction("Details", "Viaje", new { id = idViaje });
                    }
                    else
                    {
                        db.Clientes.Add(cliente);
                        db.SaveChanges();

                        return RedirectToAction("Details", new { id = cliente.ID, idViaje = idViaje, error = true });
                    }
                }

                else
                {
                    db.Clientes.Add(cliente);
                    db.SaveChanges();
                    return RedirectToAction("Search");
                }
            }
        }

        public ActionResult Details(int id, int? idViaje, string From,  bool? error)
        {
            ViewBag.Action = "Details";
            ViewBag.From = From;
           
            Cliente cliente = db.Clientes.Find(id);
            ClienteViewModel clienteViewModel = new ClienteViewModel();
            List<Provincia> Provincias = db.Provincias.ToList<Provincia>();
            List<FormaPago> formaPagos = db.FormaPago.Where(x => x.Habilitado).ToList();

            if (error.HasValue)
            {
                ModelState.AddModelError("MaxPlace", "El viaje esta completo no se puede agregar el cliente.");
                ModelState.AddModelError("ClientCreateMaxPlace", "El cliente fue guardado correctamente.");
            }

            if (idViaje.HasValue)
            {
                ViewBag.IdViaje = idViaje;
                Viaje viaje = db.Viajes.Find(idViaje);
                ViewBag.Servicio = viaje.Servicio;
                clienteViewModel = new ClienteViewModel(Provincias, viaje, cliente, formaPagos, User.Identity.Name);
            }
            else
                clienteViewModel = new ClienteViewModel(Provincias, cliente, formaPagos);

            return View("Create", clienteViewModel);
        }

        [HttpPost]
        public ActionResult Details(ClienteViewModel clienteViewModel, int id, int? idViaje)
        {
             lock(clienteViajesLock)
            {
                List<Provincia> provincias = db.Provincias.ToList<Provincia>();
                List<Localidad> localidades = db.Localidades.ToList<Localidad>();
                List<Parada> paradas = db.Paradas.ToList<Parada>();
                Cliente cliente = db.Clientes.Find(id);
                List<FormaPago> formaPagos = db.FormaPago.Where(x => x.Habilitado).ToList();

                clienteViewModel.upDateCliente(clienteViewModel, provincias, localidades, ref cliente);

                if (idViaje.HasValue) //se ingresa a detalle del cliente desde un viaje
                {
                    Viaje viaje = db.Viajes.Find(idViaje.Value);
                    ClienteViaje NewclienteViaje = clienteViewModel.getClienteViaje(clienteViewModel, viaje, paradas, cliente, db.Localidades.ToList<Localidad>(), db.Provincias.ToList<Provincia>(), formaPagos, User.Identity.Name);

                    ClienteViaje clienteViaje = cliente.ClienteViaje.Where(x => x.Viaje != null && x.Viaje.ID == idViaje).FirstOrDefault();

                    if (clienteViaje == null) //el cliente es nuevo para este viaje
                    {
                        if (viaje.tieneLugar())
                        {
                            //agrego la relacion de cliente viaje
                            db.ClienteViajes.Add(NewclienteViaje);
                        }
                        else
                        {
                            db.Entry(cliente).State = EntityState.Modified;
                            db.SaveChanges();

                            return RedirectToAction("Details", new { id = cliente.ID, idViaje = idViaje, error = true });
                        }

                    }
                    else
                    {
                        //actualizo la relacion de cliente viaje
                        clienteViaje.Ascenso = NewclienteViaje.Ascenso;
                        clienteViaje.Descenso = NewclienteViaje.Descenso;
                        clienteViaje.DomicilioAscenso = NewclienteViaje.DomicilioAscenso;
                        clienteViaje.DomicilioDescenso = NewclienteViaje.DomicilioDescenso;
                        clienteViaje.AscensoDomicilioPrincipal = NewclienteViaje.AscensoDomicilioPrincipal;
                        clienteViaje.DescensoDomicilioPrincipal = NewclienteViaje.DescensoDomicilioPrincipal;
                        clienteViaje.DescensoDomicilioOtros = NewclienteViaje.DescensoDomicilioOtros;
                        clienteViaje.Vendedor = User.Identity.Name;
                        if (!clienteViaje.FechaPago.HasValue)
                        {
                            clienteViaje.Costo = NewclienteViaje.Costo;
                            clienteViaje.FormaPago = NewclienteViaje.FormaPago;
                            clienteViaje.Pago = NewclienteViaje.Pago;
                            clienteViaje.VendedorCobro = User.Identity.Name;
                            clienteViaje.FechaPago = DateTime.Now;
                        }
                        db.Entry(clienteViaje).State = EntityState.Modified;
                    }
                }

                db.Entry(cliente).State = EntityState.Modified;
                db.SaveChanges();
            }
            if (idViaje.HasValue) //se ingresa a detalle del cliente desde un viaje
            {
                var viajesFinalizados = db.Viajes.Where(x => x.FechaArribo.CompareTo(EntityFunctions.AddHours(DateTime.Now, 4).Value) < 0 && x.Estado == ViajeEstados.Abierto);

                if (viajesFinalizados.Where(x => x.ID == idViaje.Value).Any())
                    return RedirectToAction("Close", "Viaje", new { id = idViaje });
                else
                    return RedirectToAction("Details", "Viaje", new { id = idViaje });
            }
            else
                return RedirectToAction("Search");
        }

        public ActionResult Delete(int id, int idViaje, string from)
        {
            ClienteViaje clienteViaje = db.ClienteViajes.Where(x => x.Viaje.ID == idViaje && x.Cliente.ID == id).FirstOrDefault();
            
            if (clienteViaje != null)
            {
                db.ClienteViajes.Remove(clienteViaje);
                db.SaveChanges();
            }

            if(!String.IsNullOrEmpty(from) && from == "Close")
                return RedirectToAction("Close", "Viaje", new { id = idViaje });
            else
                return RedirectToAction("Details", "Viaje", new { id = idViaje });
        }

        public ActionResult DeleteCliente(int id, string apellido, string nombre, string dni, string telefono, int? IdViaje)
        {
            Cliente cliente = db.Clientes.Find(id);

            if (cliente != null)
            {
                db.Domicilios.Remove(cliente.DomicilioPrincipal);
                db.Domicilios.Remove(cliente.DomicilioSecundario);

                db.Clientes.Where(x => x.ID == id).FirstOrDefault().ClienteViaje.ToList().ForEach(x => db.ClienteViajes.Remove(x));

                cliente.ClienteViaje.ForEach(x => db.ClienteViajes.Remove(x));

                db.Clientes.Remove(cliente);
                db.SaveChanges();
            }

            SearchClienteViewModel searchClienteViewModel = new SearchClienteViewModel();

            ViewBag.nombre = nombre;
            ViewBag.apellido = apellido;
            ViewBag.dni = dni;
            ViewBag.telefono = telefono;

            if (IdViaje.HasValue)
                ViewBag.IdViaje = IdViaje.Value;

            searchClienteViewModel.Apellido = ViewBag.apellido;
            searchClienteViewModel.Nombre = ViewBag.nombre;
            searchClienteViewModel.DNI = ViewBag.dni;
            searchClienteViewModel.Telefono = ViewBag.telefono;

            List<Cliente> clientes = db.Clientes.ToList<Cliente>();

            clientes = ClienteHelper.searchClientes(clientes, ViewBag.apellido, ViewBag.nombre, ViewBag.dni, ViewBag.telefono);

            clientes = clientes.OrderBy(x => x.Apellido).ThenBy(x => x.Nombre).ToList<Cliente>();

            searchClienteViewModel.Clientes = clientes.ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
           
            return View("Search", searchClienteViewModel);
        }


        public ActionResult SearchPagingClientes(int? IdViaje, int? pageNumber, string apellido, string nombre, string dni, string telefono)
        {
            ViewBag.nombre = nombre;
            ViewBag.apellido = apellido;
            ViewBag.dni = dni;
            ViewBag.telefono = telefono;

            if(IdViaje.HasValue)
                ViewBag.IdViaje = IdViaje.Value;

            List<Cliente> cliente = db.Clientes.ToList<Cliente>();

            cliente = ClienteHelper.searchClientes(cliente, apellido, nombre, dni, telefono);

            cliente = cliente.OrderBy(x => x.Apellido).ThenBy(x => x.Nombre).ToList<Cliente>();

            return PartialView("_ClientesTable", cliente.ToPagedList(pageNumber.Value, int.Parse(ConfigurationSettings.AppSettings["PageSize"])));
        }

        public ActionResult CreatePagingViajes(int id, int pageNumber)
        {
            Cliente cliente = db.Clientes.Find(id);

            IPagedList<ClienteViaje> viajes = cliente.ClienteViaje.Where(x => x.Viaje.Estado == ViajeEstados.Cerrado).ToPagedList<ClienteViaje>(pageNumber, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return PartialView("_ViajesTable", viajes);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}