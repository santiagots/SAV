using SAV.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAV.Controllers
{
    [Authorize]
    public class DestinosController : Controller
    {
        private SAVContext db = new SAVContext();

        public ActionResult GetLocalidades(int IdProvincia)
        {
            Provincia provincia = db.Provincias.Where(x => x.ID == IdProvincia).FirstOrDefault();

            if (HttpContext.Request.IsAjaxRequest())
                return Json(new SelectList(provincia.Localidad.OrderBy(x => x.Nombre), "ID", "Nombre"), JsonRequestBehavior.AllowGet);

            return View(provincia);
        }

        public ActionResult GetParadas(int IdLocalidad)
        {
            Localidad localidades = db.Localidades.Find(IdLocalidad);

            if (HttpContext.Request.IsAjaxRequest())
                return Json(new SelectList(localidades.Parada, "ID", "Nombre"), JsonRequestBehavior.AllowGet);

            return View(localidades);
        }

        public ActionResult ModifyDestino(int IdLocalidad, int IdParada, string NombreParada)
        {
            Parada parada = db.Paradas.Find(IdParada);
            parada.Nombre = NombreParada;

            db.Entry(parada).State = EntityState.Modified;
            db.SaveChanges();

            Localidad localidades = db.Localidades.Find(IdLocalidad);

            if (HttpContext.Request.IsAjaxRequest())
                return Json(new SelectList(localidades.Parada, "ID", "Nombre"), JsonRequestBehavior.AllowGet);

            return View(localidades);
        }

        public ActionResult AddDestino(int IdLocalidad, string NombreParada)
        {
            Localidad localidades = db.Localidades.Find(IdLocalidad);
            Parada parada = new Parada() { Nombre = NombreParada };

            localidades.Parada.Add(parada);

            db.Paradas.Add(parada);
            db.Entry(localidades).State = EntityState.Modified;

            db.SaveChanges();

            if (HttpContext.Request.IsAjaxRequest())
                return Json(new SelectList(localidades.Parada, "ID", "Nombre"), JsonRequestBehavior.AllowGet);

            return View(localidades);
        }

        public ActionResult RemoveDestino(int IdLocalidad, int IdParada)
        {
            Parada parada = db.Paradas.Find(IdParada);

            db.ClienteViajes.Where(x => x.Ascenso.ID == IdParada || x.Descenso.ID == IdParada).ToList().ForEach(x => db.ClienteViajes.Remove(x));
            db.Paradas.Remove(parada);
            db.SaveChanges();

            Localidad localidades = db.Localidades.Find(IdLocalidad);

            if (HttpContext.Request.IsAjaxRequest())
                return Json(new SelectList(localidades.Parada, "ID", "Nombre"), JsonRequestBehavior.AllowGet);

            return View(localidades);
        }

        public ActionResult Create()
        {
            List<Provincia> provincia = db.Provincias.ToList<Provincia>();

            DestinoViewModel destinoViewModel = new DestinoViewModel(provincia);

            return View(destinoViewModel);
        }

    }
}
