using SAV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace SAV.Common
{
    public class UsuarioHelper
    {
        public static List<UsuarioViewModel> getUsuarios()
        {
            List<UserProfile> usersProfile = new List<UserProfile>();
            List<UsuarioViewModel> listaUsuario = new List<UsuarioViewModel>();

            using (UsersContext usersContexta = new UsersContext())
            {
                usersProfile = usersContexta.UserProfiles.ToList();
            }

            foreach (UserProfile usuario in usersProfile)
                listaUsuario.Add(new UsuarioViewModel() { Usuario = usuario.UserName, IdUsuario = usuario.UserId, Rol = string.Join(", ", System.Web.Security.Roles.GetRolesForUser(usuario.UserName)) });

            return listaUsuario;
        }
    }
}