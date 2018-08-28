using PagedList;
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
            DatosEmpresa configuracion = db.Configuracion.FirstOrDefault();
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
            DatosEmpresa configuracion = db.Configuracion.FirstOrDefault();
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
                db.Configuracion.Add(configuracion);
            }
            db.SaveChanges();

            detailsDatosEmpresaViewModel.Provincia = Provincias.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            detailsDatosEmpresaViewModel.ContratanteProvincia = Provincias.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();

            detailsDatosEmpresaViewModel.Localidad = new List<KeyValuePair<int, string>>();
            detailsDatosEmpresaViewModel.ContratanteLocalidad = new List<KeyValuePair<int, string>>();

            return View(detailsDatosEmpresaViewModel);
        }

        //public ActionResult Usuarios(int? idUsuario)
        //{
            
        //    CreateViewModel usuarioViewModel = new CreateViewModel();

        //    if (idUsuario.HasValue)
        //    {
        //        using (UsersContext usersContexta = new UsersContext())
        //        {
        //            UserProfile userProfile = usersContexta.UserProfiles.FirstOrDefault(x => x.UserId == idUsuario.Value);
        //            if (userProfile != null)
        //                usuarioViewModel.NewUsuario = userProfile.UserName;
        //        }

        //        string[] rolesUsuario = Roles.GetRolesForUser(usuarioViewModel.NewUsuario);
        //        if (rolesUsuario.Length > 0)
        //            usuarioViewModel.SelectRol = rolesUsuario[0];
        //    }
        //    return View("Usuarios", usuarioViewModel);
        //}

        //[HttpPost]
        //public ActionResult AltaUsuarios(CreateViewModel usuarioViewModel)
        //{
        //    bool usuariosExistente = false;
        //    using (UsersContext usersContexta = new UsersContext())
        //    {
        //        usuariosExistente = usersContexta.UserProfiles.Any(x => x.UserName == usuarioViewModel.NewUsuario);
        //    }

        //    if (!usuariosExistente)
        //    {
        //        WebSecurity.CreateUserAndAccount(usuarioViewModel.NewUsuario, usuarioViewModel.NewClave);
        //        Roles.AddUserToRole(usuarioViewModel.NewUsuario, usuarioViewModel.SelectRol);
        //    }
        //    else
        //    { 
        //        ModelState.AddModelError("Error", string.Format("El usuario {0} ya existe, por favor ingrese un nuevo usuario.", usuarioViewModel.NewUsuario));
        //        CreateViewModel newUsuarioViewModel = new CreateViewModel();
        //        usuarioViewModel.Roles = newUsuarioViewModel.Roles;
        //        usuarioViewModel.Usuarios = newUsuarioViewModel.Usuarios;
        //    }
        //    return View("Usuarios", usuarioViewModel);
        //}

        //[HttpPost]
        //public ActionResult ModificarUsuario(CreateViewModel usuarioViewModel)
        //{
        //    UserProfile userProfile;
        //    using (UsersContext usersContexta = new UsersContext())
        //    {
        //        userProfile = usersContexta.UserProfiles.FirstOrDefault(x => x.UserName == usuarioViewModel.NewUsuario);


        //    if(userProfile != null)
        //    {
        //            if (!string.IsNullOrEmpty(usuarioViewModel.NewClave))
        //            {
        //                string token = WebSecurity.GeneratePasswordResetToken(usuarioViewModel.NewUsuario);
        //                WebSecurity.ResetPassword(token, usuarioViewModel.NewClave);

        //                Roles.RemoveUserFromRoles(usuarioViewModel.NewUsuario, Roles.GetRolesForUser(usuarioViewModel.NewUsuario));
        //                Roles.AddUserToRole(usuarioViewModel.NewUsuario, usuarioViewModel.SelectRol);
        //            }
        //            else
        //            { 
        //                userProfile.UserName = usuarioViewModel.NewUsuario;
        //                usersContexta.SaveChanges();
        //            }
        //        }
        //    else
        //    { 
        //        ModelState.AddModelError("Error", string.Format("El usuario {0} no existe, no se ha podido modificar el usuario.", usuarioViewModel.NewUsuario));
        //    }
        //    }
        //    return View("Usuarios", new CreateViewModel());
        //}

        public ActionResult TipoGastos()
        {
            TipoGastoViewModel tipoGastoViewModel = new TipoGastoViewModel();

            tipoGastoViewModel.TiposGastos = db.TipoGasto.ToList().ToPagedList<TipoGasto>(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return View(tipoGastoViewModel);
        }

        public ActionResult FormaPago()
        {
            FormaPagoViewModel formaPagoViewModel = new FormaPagoViewModel();

            formaPagoViewModel.FormaPagos = db.FormaPago.ToList().ToPagedList<FormaPago>(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return View(formaPagoViewModel);
        }

        [HttpPost]
        public ActionResult TipoGastos(TipoGastoViewModel tipoGastoViewModel)
        {
            db.TipoGasto.Add(new TipoGasto() { Descripcion = tipoGastoViewModel.Descripcion, Habilitado = true });
            db.SaveChanges();

            tipoGastoViewModel.Descripcion = string.Empty;
            tipoGastoViewModel.TiposGastos = db.TipoGasto.ToList().ToPagedList<TipoGasto>(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return View(tipoGastoViewModel);
        }

        [HttpPost]
        public ActionResult FormaPago(FormaPagoViewModel formaPagoViewModel)
        {
            db.FormaPago.Add(new FormaPago() { Descripcion = formaPagoViewModel.FormaPago, Habilitado = true });
            db.SaveChanges();

            formaPagoViewModel.FormaPago = string.Empty;
            formaPagoViewModel.FormaPagos = db.FormaPago.ToList().ToPagedList<FormaPago>(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return View(formaPagoViewModel);
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
    }
}
