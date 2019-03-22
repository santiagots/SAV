using PagedList;
using SAV.Common;
using SAV.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace SAV.Controllers
{
    public class ConfiguracionController : Controller
    {
        private SAVContext db = new SAVContext();
        public ActionResult Details()
        {
            DatosEmpresa configuracion = db.DatosEmpresa.FirstOrDefault();
            List<Provincia> Provincias = db.Provincias.ToList<Provincia>();
            List<Localidad> Localidades = db.Localidades.ToList<Localidad>();
            DetailsDatosEmpresaViewModel detailsDatosEmpresaViewModel;

            if (configuracion != null)
                detailsDatosEmpresaViewModel = new DetailsDatosEmpresaViewModel(configuracion, Provincias, Localidades);
            else
                detailsDatosEmpresaViewModel = new DetailsDatosEmpresaViewModel(Provincias);

            return View(detailsDatosEmpresaViewModel);
        }

        [HttpPost]
        public ActionResult Details(DetailsDatosEmpresaViewModel detailsDatosEmpresaViewModel)
        {
            DatosEmpresa configuracion = db.DatosEmpresa.FirstOrDefault();
            List<Provincia> Provincias = db.Provincias.ToList<Provincia>();
            List<Localidad> Localidades = db.Localidades.ToList<Localidad>();

            if (configuracion != null)
            {
                configuracion = DetailsDatosEmpresaViewModel.getConfiguracion(detailsDatosEmpresaViewModel, configuracion, Provincias, Localidades);
                db.Entry(configuracion).State = EntityState.Modified;
            }
            else
            {
                configuracion = new DatosEmpresa();
                configuracion.Domicilio = new Domicilio();
                configuracion.ContratanteDomicilio = new Domicilio();
                configuracion = DetailsDatosEmpresaViewModel.getConfiguracion(detailsDatosEmpresaViewModel, configuracion, Provincias, Localidades);
                db.DatosEmpresa.Add(configuracion);
            }
            db.SaveChanges();

            detailsDatosEmpresaViewModel.Provincia = Provincias.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            detailsDatosEmpresaViewModel.ContratanteProvincia = Provincias.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();

            detailsDatosEmpresaViewModel.Localidad = new List<KeyValuePair<int, string>>();
            detailsDatosEmpresaViewModel.ContratanteLocalidad = new List<KeyValuePair<int, string>>();

            return View(detailsDatosEmpresaViewModel);
        }

        public ActionResult TipoGastos()
        {
            TipoGastoViewModel tipoGastoViewModel = new TipoGastoViewModel();
            tipoGastoViewModel.Concepto = EnumHelper.GetEnumList<ConceptoGasto>();
            tipoGastoViewModel.TiposGastos = db.TipoGasto.ToList().ToPagedList<TipoGasto>(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return View(tipoGastoViewModel);
        }

        public ActionResult FormaPago()
        {
            FormaPagoViewModel formaPagoViewModel = new FormaPagoViewModel();

            formaPagoViewModel.FormaPagos = db.FormaPago.ToList().ToPagedList<FormaPago>(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return View(formaPagoViewModel);
        }

        public ActionResult TipoAdicionalesConductor()
        {
            TipoAdicionalConductorViewModel tipoAdicionalConductorViewModel = new TipoAdicionalConductorViewModel();

            tipoAdicionalConductorViewModel.TipoAdicionalConductor = db.TipoAdicionalConductor.ToList().ToPagedList<TipoAdicionalConductor>(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return View(tipoAdicionalConductorViewModel);
        }

        [HttpPost]
        public ActionResult TipoGastos(TipoGastoViewModel tipoGastoViewModel)
        {
            db.TipoGasto.Add(new TipoGasto() { Concepto = (ConceptoGasto)tipoGastoViewModel.SelectConcepto.Value, Descripcion = tipoGastoViewModel.Descripcion, Habilitado = true });
            db.SaveChanges();

            return RedirectToAction("TipoGastos");
        }

        [HttpPost]
        public ActionResult FormaPago(FormaPagoViewModel formaPagoViewModel)
        {
            db.FormaPago.Add(new FormaPago() { Descripcion = formaPagoViewModel.FormaPago, Habilitado = true });
            db.SaveChanges();

            return RedirectToAction("FormaPago");
        }

        [HttpPost]
        public ActionResult TipoAdicionalesConductor(TipoAdicionalConductorViewModel tipoAdicionalConductorViewModel)
        {
            db.TipoAdicionalConductor.Add(new TipoAdicionalConductor() { Descripcion = tipoAdicionalConductorViewModel.Descripcion, Habilitado = true });
            db.SaveChanges();

            tipoAdicionalConductorViewModel.Descripcion = string.Empty;
            tipoAdicionalConductorViewModel.TipoAdicionalConductor = db.TipoAdicionalConductor.ToList().ToPagedList<TipoAdicionalConductor>(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return RedirectToAction("TipoAdicionalesConductor");
        }

        public void setHabilitadoTipoGasto(int TipoGastoID, int Habilitado)
        {
            if (TipoGastoID > 0)
            {
                TipoGasto tipoGasto = db.TipoGasto.Find(TipoGastoID);

                if (tipoGasto != null)
                {
                    tipoGasto.Habilitado = Convert.ToBoolean(Habilitado);
                    db.Entry(tipoGasto).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }

        public void setHabilitadoFormaPago(int FormaPagoID, int Habilitado)
        {
            if (FormaPagoID > 0)
            {
                FormaPago formaPago = db.FormaPago.Find(FormaPagoID);

                if (formaPago != null)
                {
                    formaPago.Habilitado = Convert.ToBoolean(Habilitado);
                    db.Entry(formaPago).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }

        public void setHabilitadoTipoAdicionalConductor(int TipoAdicionalConductorID, int Habilitado)
        {
            if (TipoAdicionalConductorID > 0)
            {
                TipoAdicionalConductor tipoAdicionalConductor = db.TipoAdicionalConductor.Find(TipoAdicionalConductorID);

                if (tipoAdicionalConductor != null)
                {
                    tipoAdicionalConductor.Habilitado = Convert.ToBoolean(Habilitado);
                    db.Entry(tipoAdicionalConductor).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }

        public ActionResult SearchPagingTipoGasto(int? pageNumber)
        {
            IPagedList<TipoGasto> viajesResult = db.TipoGasto.ToList().ToPagedList<TipoGasto>(pageNumber.Value, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return PartialView("_TipoGastosTable", viajesResult);
        }

        public ActionResult SearchPagingFormaPago(int? pageNumber)
        {
            IPagedList<FormaPago> viajesResult = db.FormaPago.ToList().ToPagedList<FormaPago>(pageNumber.Value, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return PartialView("_FormaPagoTable", viajesResult);
        }

        public ActionResult SearchPagingTipoAdicionalConductor(int? pageNumber)
        {
            IPagedList<TipoAdicionalConductor> tipoAdicionalConductor = db.TipoAdicionalConductor.ToList().ToPagedList<TipoAdicionalConductor>(pageNumber.Value, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return PartialView("_TipoAdicionalesConductorTable", tipoAdicionalConductor);
        }
    }
}
