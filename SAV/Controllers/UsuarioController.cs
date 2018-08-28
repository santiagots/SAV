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
    public class UsuarioController : Controller
    {
        private SAVContext db = new SAVContext();

        //
        // GET: /Usuario/

        public ActionResult Create()
        {
            CreateViewModel usuarioViewModel = new CreateViewModel();
            return View(usuarioViewModel);
        }

        [HttpPost]
        public ActionResult Create(CreateViewModel usuarioViewModel)
        {
            bool usuariosExistente = false;
            using (UsersContext usersContexta = new UsersContext())
            {
                usuariosExistente = usersContexta.UserProfiles.Any(x => x.UserName == usuarioViewModel.NewUsuario);
            }

            if (!usuariosExistente)
            {
                WebSecurity.CreateUserAndAccount(usuarioViewModel.NewUsuario, usuarioViewModel.NewClave);
                Roles.AddUserToRole(usuarioViewModel.NewUsuario, usuarioViewModel.SelectRol);
            }
            else
            {
                ModelState.AddModelError("Error", string.Format("El usuario {0} ya existe, por favor ingrese un nuevo usuario.", usuarioViewModel.NewUsuario));
            }

            return View(new CreateViewModel());
        }

        public ActionResult Edit(int id = 0)
        {
            UserProfile usuario;
            EditViewModel editViewModel = new EditViewModel();

            using (UsersContext usersContexta = new UsersContext())
            {
                usuario = usersContexta.UserProfiles.FirstOrDefault(x => x.UserId == id);
            }

            if (usuario != null)
            {
                editViewModel.UsuarioID = usuario.UserId;
                editViewModel.Usuario = usuario.UserName;
                editViewModel.SelectRol = Roles.GetRolesForUser(usuario.UserName).FirstOrDefault();
                editViewModel.RolAnterior = editViewModel.SelectRol;
            }
             
            return View(editViewModel);
        }

        [HttpPost]
        public ActionResult Edit(EditViewModel editViewModel)
        {
            using (UsersContext usersContexta = new UsersContext())
            {
                bool usuariosExistente = usersContexta.UserProfiles.Any(x => x.UserName == editViewModel.Usuario && x.UserId != editViewModel.UsuarioID);

                if (usuariosExistente)
                {
                    ModelState.AddModelError("Error", string.Format("El usuario {0} ya existe, por favor ingrese un nuevo usuario.", editViewModel.Usuario));
                    return View(editViewModel);
                }

                UserProfile usuario = usersContexta.UserProfiles.FirstOrDefault(x => x.UserId == editViewModel.UsuarioID);

                if (usuario != null)
                {
                    if(!string.IsNullOrEmpty(editViewModel.RolAnterior))
                        Roles.RemoveUserFromRole(usuario.UserName, editViewModel.RolAnterior);

                    Roles.AddUserToRole(usuario.UserName, editViewModel.SelectRol);

                    usuario.UserName = editViewModel.Usuario;

                    if (!string.IsNullOrEmpty(editViewModel.Clave))
                    {
                        var token = WebSecurity.GeneratePasswordResetToken(usuario.UserName);
                        var result = WebSecurity.ResetPassword(token, editViewModel.Clave);
                    }
                }
                usersContexta.Entry(usuario).State = EntityState.Modified;
                usersContexta.SaveChanges();
            }

            return View(editViewModel);
        }

        public ActionResult Delete(int id = 0)
        {
            UserProfile usuario;
            using (UsersContext usersContexta = new UsersContext())
            {
                usuario = usersContexta.UserProfiles.FirstOrDefault(x => x.UserId == id);
            }

            if (usuario != null)
            {
                if (Roles.GetRolesForUser(usuario.UserName).Count() > 0)
                    Roles.RemoveUserFromRoles(usuario.UserName, Roles.GetRolesForUser(usuario.UserName));
                ((SimpleMembershipProvider)Membership.Provider).DeleteAccount(usuario.UserName); // deletes record from webpages_Membership table
                ((SimpleMembershipProvider)Membership.Provider).DeleteUser(usuario.UserName, true); // deletes record from UserProfile table
            }

            CreateViewModel usuarioViewModel = new CreateViewModel();
            return View("Create", usuarioViewModel);
        }

        public ActionResult CreatePaging(int? pageNumber)
        {
            CreateViewModel usuarioViewModel = new CreateViewModel();
            IPagedList<UsuarioViewModel> viajesResult = UsuarioHelper.getUsuarios().ToPagedList<UsuarioViewModel>(pageNumber.Value, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            return PartialView("_UsuariosTable", usuarioViewModel);
        }

    }
}
