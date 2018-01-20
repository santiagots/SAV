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
    public class EditViewModel
    {
        public int UsuarioID { get; set; }
        public string RolAnterior { get; set; }

        [Required(ErrorMessage = "Debe ingresar un Usuario")]
        [Display(Name = "Usuario:")]
        public string Usuario { get; set; }

        [Display(Name = "Clave:")]
        public string Clave { get; set; }

        [Display(Name = "Rol:")]
        public List<KeyValuePair<string, string>> Roles { get; set; }

        [Required(ErrorMessage = "Debe ingresar un Rol")]
        public string SelectRol { get; set; }

        public EditViewModel()
        {
            Roles = new List<KeyValuePair<string, string>>();
            foreach (string rol in System.Web.Security.Roles.GetAllRoles())
                Roles.Add(new KeyValuePair<string, string>(rol, rol));
        }
    }
}