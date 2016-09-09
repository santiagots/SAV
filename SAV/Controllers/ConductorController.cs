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

namespace SAV.Controllers
{
    [Authorize]
    public class ConductorController : Controller
    {
        private SAVContext db = new SAVContext();

        public ActionResult Create(int? id)
        {
            ConductorViewModel conductorViewModel;
            List<Provincia> provincias = db.Provincias.ToList();

            if (id.HasValue)
            {
                Conductor conductor = db.Conductores.Find(id.Value);
                conductorViewModel = new ConductorViewModel(provincias, conductor);
                conductorViewModel.Viajes = db.Viajes.ToList().Where(x => x.Conductor != null && x.Conductor.ID == id).ToPagedList<Viaje>(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
            }
            else
            {
                conductorViewModel = new ConductorViewModel(provincias);
                conductorViewModel.Viajes = new PagedList<Viaje>(null, 1, 1);
            }

            return View(conductorViewModel);
        }

        [HttpPost]
        public ActionResult Create(ConductorViewModel conductorViewModel, int? id)
        {
            List<Provincia> provincias = db.Provincias.ToList<Provincia>();
            List<Localidad> localidades = db.Localidades.ToList<Localidad>();
            Conductor conductor = new Conductor();

            if (id.HasValue)
            {
                conductor = db.Conductores.Find(id.Value);
                conductorViewModel.updateConductor(conductorViewModel, provincias, localidades, ref conductor);
                db.Entry(conductor).State = EntityState.Modified;
            }
            else
            {
                conductorViewModel.updateConductor(conductorViewModel, provincias, localidades, ref conductor);
                db.Conductores.Add(conductor);
            }
            db.SaveChanges();

            return RedirectToAction("Search");
        }
        
        public ActionResult Search()
        {
            SearchConductorViewModel searchConductorViewModel = new SearchConductorViewModel();

            List<Conductor> conductores = db.Conductores.ToList<Conductor>();

            searchConductorViewModel.Conductores = conductores.ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return View(searchConductorViewModel);
        }

        [HttpPost]
        public ActionResult Search(SearchConductorViewModel searchConductorViewModel)
        {

            ViewBag.nombre = searchConductorViewModel.Nombre;
            ViewBag.apellido = searchConductorViewModel.Apellido;
            ViewBag.dni = searchConductorViewModel.DNI;
            ViewBag.telefono = searchConductorViewModel.Telefono;

            List<Conductor> conductores = db.Conductores.ToList<Conductor>();

            conductores = ConductorHelper.searchCondictores(conductores, searchConductorViewModel.Apellido, searchConductorViewModel.Nombre, searchConductorViewModel.DNI, searchConductorViewModel.Telefono);

            conductores = conductores.OrderBy(x => x.Apellido).ThenBy(x => x.Nombre).ToList<Conductor>();

            searchConductorViewModel.Conductores = conductores.ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return View(searchConductorViewModel);
        }

        public ActionResult SearchPagingConductores(int? pageNumber, string apellido, string nombre, string dni, string telefono)
        {
            ViewBag.nombre = nombre;
            ViewBag.apellido = apellido;
            ViewBag.dni = dni;
            ViewBag.telefono = telefono;

            List<Conductor> conductores = db.Conductores.ToList<Conductor>();

            conductores = ConductorHelper.searchCondictores(conductores, apellido, nombre, dni, telefono);

            conductores = conductores.OrderBy(x => x.Apellido).ThenBy(x => x.Nombre).ToList<Conductor>();

            return PartialView("_ConductoresTable", conductores.ToPagedList(pageNumber.Value, int.Parse(ConfigurationSettings.AppSettings["PageSize"])));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult CreatePagingViajes(int id, int pageNumber)
        {
            Conductor conductor = db.Conductores.Find(id);

            IPagedList<Viaje> viajes = db.Viajes.ToList().Where(x => x.Conductor != null && x.Conductor.ID == id).ToPagedList<Viaje>(pageNumber, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return PartialView("_ViajesTable", viajes);
        }

        public ActionResult Delete(int id = 0)
        {
            Conductor conductor = db.Conductores.Find(id);
            if (conductor == null)
            {
                return HttpNotFound();
            }

            foreach (Viaje viaje in db.Viajes.Where(x => x.Conductor != null && x.Conductor.ID == id).ToList())
            {
                viaje.Conductor = null;
            } 
            db.Conductores.Remove(conductor);
            db.SaveChanges();

            return RedirectToAction("Search");
        }
    }
}