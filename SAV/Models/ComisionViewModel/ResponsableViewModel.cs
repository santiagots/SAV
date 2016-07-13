using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class ResponsableViewModel
    {
        public  IPagedList<ComisionResponsable> Responsables { get; set; }

        [Display(Name = "Nombre:")]
        [Required(ErrorMessage = "Debe ingresar un Nombre")]
        [RegularExpression("^[A-Za-z ÑÁÉÍÓÚñáéíóú]+$", ErrorMessage = "El Nombre solo debe contener letras")]
        public string NuevoNombre { get; set; }

        [Display(Name = "Apellido:")]
        [Required(ErrorMessage = "Debe ingresar un Nombre")]
        [RegularExpression("^[A-Za-z ÑÁÉÍÓÚñáéíóú]+$", ErrorMessage = "El Apellido solo debe contener letras")]
        public string NuevoApellido { get; set; }
    }
}