using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using SAV.Models;
using PagedList;
using System.Configuration;
using SAV.Common;
using System.Data.Entity.Core.Objects;

namespace SAV.Controllers
{
    [Authorize]
    public class ClienteController : Controller
    {
        private SAVContext db = new SAVContext();
        private Object clienteViajesLock = new Object();

        public ActionResult ImprimirBoleto(int IdClienteViaje)
        {
            var clienteViaje = db.ClienteViajes.Find(IdClienteViaje);

            ViewBag.IdViaje = clienteViaje.Viaje.ID;
            ViewBag.Servicio = clienteViaje.Viaje.Servicio == ViajeTipoServicio.Directo? "Turismo": clienteViaje.Viaje.Servicio.ToString();
            ViewBag.Apellido = clienteViaje.Cliente.Apellido;
            ViewBag.Nombre = clienteViaje.Cliente.Nombre;
            ViewBag.DNI = clienteViaje.Cliente.DNI;
            ViewBag.Origen = clienteViaje.Viaje.Servicio == ViajeTipoServicio.Cerrado? clienteViaje.Viaje.OrigenCerrado : clienteViaje.Viaje.Origen.Nombre;
            ViewBag.Destino = clienteViaje.Viaje.Servicio == ViajeTipoServicio.Cerrado ? clienteViaje.Viaje.DestinoCerrado : clienteViaje.Viaje.Destino.Nombre;
            ViewBag.FechaPago = clienteViaje.FechaPago.HasValue ? clienteViaje.FechaPago.Value.ToString("dd/MM/yyyy") : "SIN PAGO";
            ViewBag.FechaHora = clienteViaje.Viaje.FechaSalida.ToString("dd/MM/yyyy HH:mm:ss");

            return new Rotativa.ViewAsPdf("Boleto");
            //return View("Boleto");
        }

        public ActionResult Search(int? IdViaje, int? NumeroAsiento)
        {
            if (IdViaje.HasValue)
            {
                ViewBag.IdViaje = IdViaje.Value;
                ViewBag.NumeroAsiento = NumeroAsiento;

                var viajesFinalizados = db.Viajes.Where(x => x.FechaArribo.CompareTo(DbFunctions.AddHours(DateTime.Now, 4).Value) < 0 && x.Estado == ViajeEstados.Abierto);

                if (viajesFinalizados.Where(x => x.ID == IdViaje.Value).Any())
                    ViewBag.From = "Cierre";
            }

            SearchClienteViewModel searchClienteViewModel = new SearchClienteViewModel();
            searchClienteViewModel.Clientes = new PagedList<Cliente>(null, 1, 1);

            return View(searchClienteViewModel);
        }

        [HttpPost]
        public ActionResult Search(SearchClienteViewModel searchClienteViewModel, int? IdViaje, int? NumeroAsiento)
        {
            ViewBag.nombre = searchClienteViewModel.Nombre;
            ViewBag.apellido = searchClienteViewModel.Apellido;
            ViewBag.dni = searchClienteViewModel.DNI;
            ViewBag.telefono = searchClienteViewModel.Telefono;

            if (IdViaje.HasValue)
            {
                ViewBag.IdViaje = IdViaje.Value;
                ViewBag.NumeroAsiento = NumeroAsiento;
            }

            List<Cliente> cliente = db.Clientes.ToList<Cliente>();

            cliente = ClienteHelper.searchClientes(cliente, searchClienteViewModel.Apellido, searchClienteViewModel.Nombre, searchClienteViewModel.DNI, searchClienteViewModel.Telefono);

            cliente = cliente.OrderBy(x => x.Apellido).ThenBy(x => x.Nombre).ToList<Cliente>();

            searchClienteViewModel.Clientes = cliente.ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return View(searchClienteViewModel);
        }

        public ActionResult Create(int? id, int? IdViaje, int? NumeroAsiento)
        {
            ClienteViewModel clienteViewModel = new ClienteViewModel();

            //si no hay ID quiere decir que es un cliete nuevo por lo que tengo que forzar la creacion en la DB de uno para poder vincularle los domicilios
            if (!id.HasValue)
            {
                Cliente cliente = new Cliente();
                cliente = db.Clientes.FirstOrDefault(x => x.Nombre == null && x.Apellido == null && x.DNI == null && DbFunctions.DiffDays(x.FechaCreacion, DateTime.Now).Value > 5);
                if (cliente == null)
                {
                    cliente = new Cliente() { FechaCreacion = DateTime.Now };
                    db.Clientes.Add(cliente);
                    db.SaveChanges();
                }
                else
                {
                    cliente.FechaCreacion = DateTime.Now;
                    cliente.Domicilios.Clear();
                    db.SaveChanges();
                }
                return RedirectToAction("Create", new { id = cliente.ID, IdViaje = IdViaje, NumeroAsiento = NumeroAsiento });
            }
            else
            {
                clienteViewModel.Id = id.Value;
            }

            ViewBag.Action = "Create";

            if (IdViaje.HasValue)
            {
                List<FormaPago> formaPagos = db.FormaPago.Where(x => x.Habilitado).ToList();
                ViewBag.IdViaje = IdViaje.Value;
                ViewBag.NumeroAsiento = NumeroAsiento;
                Viaje viaje = db.Viajes.Find(IdViaje);
                List<Provincia> provincias = db.Provincias.ToList();

                clienteViewModel = new ClienteViewModel(provincias, viaje, formaPagos, User.Identity.Name.ToUpper());
                ViewBag.Servicio = viaje.Servicio;
            }

            return View(clienteViewModel);
        }

        [HttpPost]
        public ActionResult Create(ClienteViewModel clienteViewModel, int? idViaje, int? NumeroAsiento)
        {
            lock (clienteViajesLock)
            {
                List<Parada> paradas = db.Paradas.ToList<Parada>();
                List<FormaPago> formaPagos = db.FormaPago.Where(x => x.Habilitado).ToList();
                Cliente cliente = db.Clientes.Find(clienteViewModel.Id);

                cliente.Apellido = clienteViewModel.Apellido;
                cliente.DNI = clienteViewModel.DNI;
                cliente.Edad = clienteViewModel.Edad.Value;
                cliente.Email = clienteViewModel.Email;
                cliente.Estudiante = clienteViewModel.Estudiante;
                cliente.Nacionalidad = clienteViewModel.Nacionalidad;
                cliente.Nombre = clienteViewModel.Nombre;
                cliente.Sexo = clienteViewModel.Sexo.Value;
                cliente.Telefono = clienteViewModel.Telefono;
                cliente.TelefonoAlternativo = clienteViewModel.TelefonoAlternativo;

                if (idViaje.HasValue)
                {
                    Viaje viaje = db.Viajes.Find(idViaje);

                    if (viaje.tieneLugar())
                    {
                        ClienteViaje clienteViaje = clienteViewModel.getClienteViaje(clienteViewModel, viaje, paradas, cliente, formaPagos, User.Identity.Name.ToUpper(), NumeroAsiento);
                        cliente.ClienteViaje.Add(clienteViaje);

                        RegistroViaje registroViaje = RegistroViajeHelper.GetRegistro(User.Identity.Name.ToUpper(), null, clienteViaje);
                            if(registroViaje != null)
                                clienteViaje.Registro.Add(registroViaje);

                        db.SaveChanges();

                        DateTime fechaActual = DateHelper.getLocal();
                        var viajesFinalizados = db.Viajes.Where(x => x.FechaArribo.CompareTo(fechaActual) < 0 && x.Estado == ViajeEstados.Abierto);

                        if (viajesFinalizados.Where(x => x.ID == idViaje.Value).Any())
                            return RedirectToAction("Close", "Viaje", new { id = idViaje });
                        else
                            if (clienteViaje.Pago)
                            return RedirectToAction("Details", "Viaje", new { id = idViaje, IdClienteViajePago = clienteViaje.ID });
                        else
                            return RedirectToAction("Details", "Viaje", new { id = idViaje });
                    }
                    else
                    {
                        db.SaveChanges();
                        return RedirectToAction("Details", new { id = cliente.ID, idViaje = idViaje, error = true });
                    }
                }

                else
                {
                    db.SaveChanges();
                    return RedirectToAction("Search");
                }
            }
        }

        public ActionResult Details(int id, int? idViaje, string From, bool? error, int? NumeroAsiento)
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
                ViewBag.NumeroAsiento = NumeroAsiento;
                Viaje viaje = db.Viajes.Find(idViaje);
                ViewBag.Servicio = viaje.Servicio;
                List<ClienteViaje> viajesDelDia = db.ClienteViajes.Where(x => x.Cliente.ID == cliente.ID && x.Viaje.ID != viaje.ID && EntityFunctions.TruncateTime(x.Viaje.FechaSalida) == EntityFunctions.TruncateTime(viaje.FechaSalida)).ToList();
                clienteViewModel = new ClienteViewModel(Provincias, viaje, cliente, viajesDelDia, formaPagos, User.Identity.Name.ToUpper());
            }
            else
                clienteViewModel = new ClienteViewModel(Provincias, cliente, formaPagos);

            return View("Create", clienteViewModel);
        }

        [HttpPost]
        public ActionResult Details(ClienteViewModel clienteViewModel, int id, int? idViaje, int? NumeroAsiento)
        {

            List<Parada> paradas = db.Paradas.ToList<Parada>();
            List<FormaPago> formaPagos = db.FormaPago.Where(x => x.Habilitado).ToList();

            Cliente cliente = db.Clientes.Find(id);
            cliente.Apellido = clienteViewModel.Apellido;
            cliente.DNI = clienteViewModel.DNI;
            cliente.Edad = clienteViewModel.Edad.Value;
            cliente.Email = clienteViewModel.Email;
            cliente.Estudiante = clienteViewModel.Estudiante;
            cliente.Nacionalidad = clienteViewModel.Nacionalidad;
            cliente.Nombre = clienteViewModel.Nombre;
            cliente.Sexo = clienteViewModel.Sexo.Value;
            cliente.Telefono = clienteViewModel.Telefono;
            cliente.TelefonoAlternativo = clienteViewModel.TelefonoAlternativo;

            if (idViaje.HasValue) //se ingresa a detalle del cliente desde un viaje
            {
                Viaje viaje = db.Viajes.Find(idViaje.Value);
                ClienteViaje newClienteViaje = clienteViewModel.getClienteViaje(clienteViewModel, viaje, paradas, cliente, formaPagos, User.Identity.Name.ToUpper(), NumeroAsiento);

                ClienteViaje clienteViaje = cliente.ClienteViaje.Where(x => x.Viaje != null && x.Viaje.ID == idViaje).FirstOrDefault();

                if (clienteViaje == null) //el cliente es nuevo para este viaje
                {
                    if (viaje.tieneLugar())
                    {
                        //agrego la relacion de cliente viaje
                        db.ClienteViajes.Add(newClienteViaje);

                        RegistroViaje registroViaje = RegistroViajeHelper.GetRegistro(User.Identity.Name.ToUpper(), null, newClienteViaje);
                        if (registroViaje != null)
                            newClienteViaje.Registro.Add(registroViaje);

                    }
                    else
                    {
                        db.Entry(cliente).State = EntityState.Modified;

                        db.SaveChanges();

                        return RedirectToAction("Details", new { id = cliente.ID, idViaje = idViaje, error = true, NumeroAsiento = NumeroAsiento});
                    }

                }
                else
                {
                    RegistroViaje registroViaje = RegistroViajeHelper.GetRegistro(User.Identity.Name.ToUpper(), clienteViaje, newClienteViaje);
                    if(registroViaje != null)
                        clienteViaje.Registro.Add(registroViaje);

                    //actualizo la relacion de cliente viaje
                    clienteViaje.Ascenso = newClienteViaje.Ascenso;
                    clienteViaje.Descenso = newClienteViaje.Descenso;
                    clienteViaje.DomicilioAscenso = newClienteViaje.DomicilioAscenso;
                    clienteViaje.DomicilioDescenso = newClienteViaje.DomicilioDescenso;
                    clienteViaje.Vendedor = User.Identity.Name.ToUpper();

                    //Actualizo el costo solo si es mayor a 0, puede ser 0 porque el costo se deshabilita cuando el viaje esta pago!!!
                    if (newClienteViaje.Costo > 0)
                        clienteViaje.Costo = newClienteViaje.Costo;

                    clienteViaje.FormaPago = newClienteViaje.FormaPago;
                    if (clienteViewModel.Pago && !clienteViaje.Pago)
                    {
                        clienteViaje.Pago = clienteViewModel.Pago;
                        clienteViaje.VendedorCobro = User.Identity.Name.ToUpper();
                        clienteViaje.FechaPago = DateTime.Now;
                    }
                    db.Entry(clienteViaje).State = EntityState.Modified;
                }

                db.Entry(cliente).State = EntityState.Modified;
                db.SaveChanges();

                var viajesFinalizados = db.Viajes.Where(x => x.FechaArribo.CompareTo(EntityFunctions.AddHours(DateTime.Now, 4).Value) < 0 && x.Estado == ViajeEstados.Abierto);

                if (viajesFinalizados.Where(x => x.ID == idViaje.Value).Any())
                    return RedirectToAction("Close", "Viaje", new { id = idViaje });
                else
                {
                    clienteViaje = clienteViaje == null ? newClienteViaje : clienteViaje;
                    if (clienteViaje.Pago)
                        return RedirectToAction("Details", "Viaje", new { id = idViaje, IdClienteViajePago = clienteViaje.ID });
                    else
                        return RedirectToAction("Details", "Viaje", new { id = idViaje });
                }

            }
            else
                return RedirectToAction("Search");
        }

        public ActionResult Delete(int id, int idViaje, string from)
        {
            ClienteViaje clienteViaje = db.ClienteViajes.Where(x => x.Viaje.ID == idViaje && x.Cliente.ID == id).FirstOrDefault();

            if (clienteViaje != null)
            {
                clienteViaje.Registro.Clear();
                db.ClienteViajes.Remove(clienteViaje);
                db.SaveChanges();
            }

            if (!String.IsNullOrEmpty(from) && from == "Close")
                return RedirectToAction("Close", "Viaje", new { id = idViaje });
            else
                return RedirectToAction("Details", "Viaje", new { id = idViaje });
        }

        public ActionResult DeleteCliente(int id, string apellido, string nombre, string dni, string telefono, int? IdViaje)
        {
            Cliente cliente = db.Clientes.Find(id);

            if (cliente != null)
            {
                db.Domicilios.RemoveRange(cliente.Domicilios);
                db.ClienteViajes.RemoveRange(cliente.ClienteViaje);
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

        public ActionResult DeleteDomicilio(int id, int ClienteId)
        {
            Domicilio domicilio = db.Domicilios.Find(id);
            Cliente cliente = db.Clientes.Find(ClienteId);
            cliente.Domicilios.Remove(domicilio);

            db.SaveChanges();

            ViewData["ClienteId"] = ClienteId;

            IPagedList<Domicilio> domicilios = cliente.Domicilios.ToPagedList<Domicilio>(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return PartialView("_DomiciliosTable", domicilios);
        }

        public ActionResult PagingRegistro(int? pageNumber, int idCliente, int idViaje)
        {
            ClienteViaje clienteViaje = db.Clientes.Find(idCliente).ClienteViaje.Where(x => x.Viaje != null && x.Viaje.ID == idViaje).FirstOrDefault();
            if(clienteViaje != null)
                return PartialView("_Registro", clienteViaje.Registro.ToPagedList(pageNumber.Value, int.Parse(ConfigurationSettings.AppSettings["PageSize"])));
            else
                return PartialView("_Registro", new List<RegistroViaje>().ToPagedList(pageNumber.Value, int.Parse(ConfigurationSettings.AppSettings["PageSize"])));
        }

        public ActionResult SearchPagingClientes(int? IdViaje, int? NumeroAsiento, int? pageNumber, string apellido, string nombre, string dni, string telefono)
        {
            ViewBag.nombre = nombre;
            ViewBag.apellido = apellido;
            ViewBag.dni = dni;
            ViewBag.telefono = telefono;
            ViewBag.NumeroAsiento = NumeroAsiento;

            if (IdViaje.HasValue)
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

        public ActionResult PagingDomicilios(int id, int pageNumber)
        {
            Cliente cliente = db.Clientes.Find(id);
            ViewBag.ClienteId = id;

            IPagedList<Domicilio> viajes = cliente.Domicilios.ToPagedList<Domicilio>(pageNumber, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return PartialView("_DomiciliosTable", viajes);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}