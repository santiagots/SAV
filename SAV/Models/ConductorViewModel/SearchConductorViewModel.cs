using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class SearchConductorViewModel
    {
        [Display(Name = "Apellido:")]
        public string Apellido { get; set; }

        [Display(Name = "Nombre:")]
        public string Nombre { get; set; }

        [Display(Name = "Numero de Documento:")]
        public string DNI { get; set; }

        [Display(Name = "Telefono:")]
        public string Telefono { get; set; }

        public IPagedList<Conductor> Conductores { get; set; }
    }
}