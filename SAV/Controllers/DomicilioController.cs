using PagedList;
using SAV.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SAV.Controllers
{
    public class DomicilioController : Controller
    {
        private SAVContext db = new SAVContext();

        public ActionResult Create(int ClienteId)
        {
            ViewBag.ClienteId = ClienteId;
            List<Provincia> provincias = db.Provincias.ToList();
            DomicilioViewModel domicilioViewModel = new DomicilioViewModel(provincias);

            return View(domicilioViewModel);
        }

        [HttpPost]
        public ActionResult Create(DomicilioViewModel domicilioViewModel, int ClienteId)
        {
            Cliente cliente = db.Clientes.Find(ClienteId);
            if (cliente != null)
            {
                cliente.Domicilios.Add(new Domicilio()
                {
                    Calle = domicilioViewModel.Calle,
                    Comentario = domicilioViewModel.Comentario,
                    Localidad = db.Localidades.Find(domicilioViewModel.SelectLocalidad),
                    Numero = domicilioViewModel.Numero,
                    Piso = domicilioViewModel.Piso,
                    Provincia = db.Provincias.Find(domicilioViewModel.SelectProvincia)
                });

                db.SaveChanges();
            }
            else
            {
                return new HttpNotFoundResult($"No se ha encontrado un cliente con codigo {ClienteId}");
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult Update(int id, int ClienteId)
        {
            ViewBag.ClienteId = ClienteId;

            Domicilio domicilio = db.Domicilios.Find(id);

            List<Provincia> provincias = db.Provincias.ToList();
            DomicilioViewModel domicilioViewModel = new DomicilioViewModel(provincias, domicilio.Provincia.Localidad);

            domicilioViewModel.Calle = domicilio.Calle;
            domicilioViewModel.Comentario = domicilio.Comentario;
            domicilioViewModel.Numero = domicilio.Numero;
            domicilioViewModel.Piso = domicilio.Piso;
            domicilioViewModel.SelectLocalidad = domicilio.Localidad.ID;
            domicilioViewModel.SelectProvincia = domicilio.Provincia.ID;

            return View("create", domicilioViewModel);
        }

        [HttpPost]
        public ActionResult Update(DomicilioViewModel domicilioViewModel, int Id)
        {
            Domicilio domicilio = db.Domicilios.Find(Id);
            if (domicilio != null)
            {
                domicilio.Calle = domicilioViewModel.Calle;
                domicilio.Comentario = domicilioViewModel.Comentario;
                domicilio.Localidad = db.Localidades.Find(domicilioViewModel.SelectLocalidad);
                domicilio.Numero = domicilioViewModel.Numero;
                domicilio.Piso = domicilioViewModel.Piso;
                domicilio.Provincia = db.Provincias.Find(domicilioViewModel.SelectProvincia);

                db.SaveChanges();
            }
            else
            {
                return new HttpNotFoundResult($"No se ha encontrado un domicilio con codigo {Id}");
            }
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        public ActionResult GetLocalidades(int IdProvincia)
        {
            Provincia provincia = db.Provincias.Where(x => x.ID == IdProvincia).FirstOrDefault();

            if (HttpContext.Request.IsAjaxRequest())
                return Json(new SelectList(provincia.Localidad.OrderBy(x => x.Nombre), "ID", "Nombre"), JsonRequestBehavior.AllowGet);

            return View(provincia);
        }

        public JsonResult GetDomicilios(int ClienteId)
        {
            Cliente cliente = db.Clientes.Find(ClienteId);

            if (cliente != null)
                return Json(cliente.Domicilios.Select(x => new KeyValuePair<int, string>(x.ID, x.getDomicilio)).ToList<KeyValuePair<int, string>>(), JsonRequestBehavior.AllowGet);
            else
                return null;
        }

    }
}
