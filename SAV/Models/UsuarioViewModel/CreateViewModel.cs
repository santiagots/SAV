using PagedList;
using SAV.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class CreateViewModel
    {
        [Required(ErrorMessage = "Debe ingresar un Usuario")]
        [Display(Name = "Usuario:")]
        public string NewUsuario { get; set; }

        [Required(ErrorMessage = "Debe ingresar una Clave")]
        [Display(Name = "Clave:")]
        public string NewClave { get; set; }

        [Display(Name = "Rol:")]
        public List<KeyValuePair<string, string>> Roles { get; set; }

        [Required(ErrorMessage = "Debe ingresar un Rol")]
        public string SelectRol { get; set; }

        public IPagedList<UsuarioViewModel> Usuarios { get; set; }

        public CreateViewModel()
        {
            Usuarios = UsuarioHelper.getUsuarios().ToPagedList<UsuarioViewModel>(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            Roles = new List<KeyValuePair<string, string>>();
            foreach (string rol in System.Web.Security.Roles.GetAllRoles())
                Roles.Add(new KeyValuePair<string, string>(rol, rol));
        }
    }
}