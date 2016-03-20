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
                return Json(new SelectList(provincia.Localidad, "ID", "Nombre"), JsonRequestBehavior.AllowGet);

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

        public ActionResult Delete(int id, int idViaje)
        {
            ComisionViaje comisionViaje = db.ComisionViajes.Where(x => x.Viaje.ID == idViaje && x.Comision.ID == id).FirstOrDefault();

            if (comisionViaje != null)
            {
                db.ComisionViajes.Remove(comisionViaje);
                db.SaveChanges();
            }

            return RedirectToAction("Details", "Viaje", new { id = idViaje });
        }

        public ActionResult Create(int? IdViaje)
        {
            ViewBag.Action = "Create";

            List<ComisionResponsable> comisionResponsable = db.ComisionResponsable.ToList<ComisionResponsable>();
            List<Provincia> provincia = db.Provincias.ToList<Provincia>();
            ComisionViewModel comisionViewModel = null;

            if (IdViaje.HasValue)
            {
                ViewBag.IdViaje = IdViaje;
                Viaje viaje = db.Viajes.Find(IdViaje);
                comisionViewModel = new ComisionViewModel(provincia, viaje, comisionResponsable);
            }
            else
            {
                comisionViewModel = new ComisionViewModel(provincia, comisionResponsable);
                comisionViewModel.ServicioDirectoRetirar = new List<KeyValuePair<int,string>>();
                comisionViewModel.ServicioDirectoEntregar = new List<KeyValuePair<int, string>>(); 
            }

            return View(comisionViewModel);
        }

        [HttpPost]
        public ActionResult Create(ComisionViewModel comisionViewModel, int? idViaje)
        {
            List<Provincia> provincias = db.Provincias.ToList<Provincia>();
            List<Localidad> localidades = db.Localidades.ToList<Localidad>();
            List<Parada> paradas = db.Paradas.ToList<Parada>();
            Comision comision = comisionViewModel.getComision(comisionViewModel, provincias, localidades);
            comision.Responsable = db.ComisionResponsable.Find(comisionViewModel.SelectResponsable);

            if (idViaje.HasValue)
            {
                Viaje viaje = db.Viajes.Find(idViaje);
                ComisionViaje comisionViaje = comisionViewModel.getComisionViaje(comisionViewModel, viaje, paradas, comision);
                db.ComisionViajes.Add(comisionViaje);
            }

            db.Comisiones.Add(comision);
            db.SaveChanges();

            if (idViaje.HasValue)
                return RedirectToAction("Details", "Viaje", new { id = idViaje });
            else
                return RedirectToAction("Search");
        }

        public ActionResult Details(int id, int? idViaje, bool? fromViaje )
        {
            ViewBag.Action = "Details";

            List<ComisionResponsable> comisionResponsable = db.ComisionResponsable.ToList<ComisionResponsable>();
            Comision comision = db.Comisiones.Find(id);
            ComisionViewModel comisionViewModel = new ComisionViewModel();
            List<Provincia> Provincias = db.Provincias.ToList<Provincia>();

            if (idViaje.HasValue)
            {
                ViewBag.IdViaje = idViaje;
                Viaje viaje = db.Viajes.Find(idViaje);
                comisionViewModel = new ComisionViewModel(Provincias, viaje, comision, fromViaje.HasValue, comisionResponsable);
            }
            else
            {
                comisionViewModel = new ComisionViewModel(Provincias, comision);
            }

            return View("Create", comisionViewModel);
        }

        [HttpPost]
        public ActionResult Details(ComisionViewModel comisionViewModel, int id, int? idViaje)
        {
            List<Provincia> provincias = db.Provincias.ToList<Provincia>();
            List<Localidad> localidades = db.Localidades.ToList<Localidad>();
            List<Parada> paradas = db.Paradas.ToList<Parada>();
            Comision comisiones = db.Comisiones.Find(id);

            comisionViewModel.upDateComision(comisionViewModel, provincias, localidades, ref comisiones);
            comisiones.Responsable = db.ComisionResponsable.Find(comisionViewModel.SelectResponsable);

            if (idViaje.HasValue) //se ingresa a detalle de la comision desde un viaje
            {
                Viaje viaje = db.Viajes.Find(idViaje.Value);
                ComisionViaje newComisionViaje = comisionViewModel.getComisionViaje(comisionViewModel, viaje, paradas, comisiones);

                ComisionViaje clienteViaje = comisiones.ComisionViaje.Where(x => x.Viaje != null && x.Viaje.ID == idViaje).FirstOrDefault();

                if (clienteViaje == null) //el cliente es nuevo para este viaje
                {
                    //agrego la relacion de cliente viaje
                    db.ComisionViajes.Add(newComisionViaje);
                }
                else
                {
                    //actualizo la relacion de cliente viaje
                    clienteViaje.Entregar = newComisionViaje.Entregar;
                    clienteViaje.Retirar = newComisionViaje.Retirar;
                    clienteViaje.Pago = newComisionViaje.Pago;

                    db.Entry(clienteViaje).State = EntityState.Modified;
                }
            }

            db.Entry(comisiones).State = EntityState.Modified;
            db.SaveChanges();

            if (idViaje.HasValue) //se ingresa a detalle del cliente desde un viaje
                return RedirectToAction("Details", "Viaje", new { id = idViaje });
            else
                return RedirectToAction("Search");
        }

        public ActionResult Search(int? IdViaje)
        {
            if (IdViaje.HasValue)
                ViewBag.IdViaje = IdViaje.Value;

            SearchComisionViewModel searchComisionViewModel = new SearchComisionViewModel();
            searchComisionViewModel.Comisiones = new PagedList<Comision>(null, 1, 1);

            return View(searchComisionViewModel);
        }

        [HttpPost]
        public ActionResult Search(SearchComisionViewModel searchComisionViewModel, int? IdViaje)
        {
            ViewBag.Contacto = searchComisionViewModel.Contacto;
            ViewBag.Telefono = searchComisionViewModel.Telefono;
            ViewBag.Accion = searchComisionViewModel.Accion;
            ViewBag.Servicio = searchComisionViewModel.Servicio;

            if (IdViaje.HasValue)
                ViewBag.IdViaje = IdViaje.Value;

            List<Comision> comisiones = db.Comisiones.ToList<Comision>();

            if (!String.IsNullOrEmpty(searchComisionViewModel.Contacto))
                comisiones = comisiones.Where(x => x.Contacto.Contains(searchComisionViewModel.Contacto)).ToList<Comision>();

            if (!String.IsNullOrEmpty(searchComisionViewModel.Telefono))
                comisiones = comisiones.Where(x => x.Telefono.Contains(searchComisionViewModel.Telefono)).ToList<Comision>();

            if (!String.IsNullOrEmpty(searchComisionViewModel.Accion))
                comisiones = comisiones.Where(x => x.Accion.ToString() == searchComisionViewModel.Accion).ToList<Comision>();

            if (!String.IsNullOrEmpty(searchComisionViewModel.Servicio))
                comisiones = comisiones.Where(x => x.Servicio.ToString() == searchComisionViewModel.Servicio).ToList<Comision>();

            searchComisionViewModel.Comisiones = comisiones.ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return View(searchComisionViewModel);
        }

        public ActionResult SearchPagingComision(int? IdViaje, int? pageNumber, string Contacto, string Telefono, string Accion, string Servicio)
        {
            ViewBag.Contacto = Contacto;
            ViewBag.Telefono = Telefono;
            ViewBag.Accion = Accion;
            ViewBag.Servicio = Servicio;

            if (IdViaje.HasValue)
                ViewBag.IdViaje = IdViaje.Value;

            List<Comision> comisiones = db.Comisiones.ToList<Comision>();

            if (!String.IsNullOrEmpty(Contacto))
                comisiones = comisiones.Where(x => x.Contacto.Contains(Contacto)).ToList<Comision>();

            if (!String.IsNullOrEmpty(Telefono))
                comisiones = comisiones.Where(x => x.Telefono.Contains(Telefono)).ToList<Comision>();

            if (!String.IsNullOrEmpty(Accion))
                comisiones = comisiones.Where(x => x.Accion.ToString() == Accion).ToList<Comision>();

            if (!String.IsNullOrEmpty(Servicio))
                comisiones = comisiones.Where(x => x.Servicio.ToString() == Servicio).ToList<Comision>();

            return PartialView("_ComisionTable", comisiones.ToPagedList(pageNumber.Value, int.Parse(ConfigurationSettings.AppSettings["PageSize"])));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}