using SAV.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SAV.Controllers
{
    public class ConfiguracionController : Controller
    {
        private SAVContext db = new SAVContext();
        public ActionResult Details()
        {
            Configuracion configuracion = db.Configuracion.FirstOrDefault();
            List<Provincia> Provincias = db.Provincias.ToList<Provincia>();
            List<Localidad> Localidades = db.Localidades.ToList<Localidad>();
            DetailsConfiguracionViewModel detailsConfiguracionViewModel;

            if (configuracion != null)
                detailsConfiguracionViewModel = new DetailsConfiguracionViewModel(configuracion, Provincias, Localidades);
            else
                detailsConfiguracionViewModel = new DetailsConfiguracionViewModel(Provincias);

            return View(detailsConfiguracionViewModel);
        }

        [HttpPost]
        public ActionResult Details(DetailsConfiguracionViewModel detailsConfiguracionViewModel)
        {
            Configuracion configuracion = db.Configuracion.FirstOrDefault();
            List<Provincia> Provincias = db.Provincias.ToList<Provincia>();
            List<Localidad> Localidades = db.Localidades.ToList<Localidad>();

            if (configuracion != null)
            {
                configuracion = DetailsConfiguracionViewModel.getConfiguracion(detailsConfiguracionViewModel, configuracion, Provincias, Localidades);
                db.Entry(configuracion).State = EntityState.Modified;
            }
            else
            {
                configuracion = new Configuracion();
                configuracion.Domicilio = new Domicilio();
                configuracion.ContratanteDomicilio = new Domicilio();
                configuracion = DetailsConfiguracionViewModel.getConfiguracion(detailsConfiguracionViewModel, configuracion, Provincias, Localidades);
                db.Configuracion.Add(configuracion);
            }
            db.SaveChanges();

            detailsConfiguracionViewModel.Provincia = Provincias.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            detailsConfiguracionViewModel.ContratanteProvincia = Provincias.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();

            detailsConfiguracionViewModel.Localidad = new List<KeyValuePair<int, string>>();
            detailsConfiguracionViewModel.ContratanteLocalidad = new List<KeyValuePair<int, string>>();

            return View(detailsConfiguracionViewModel);
        }

    }
}
