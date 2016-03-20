using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class SearchComisionViewModel
    {
        [Display(Name = "Contacto:")]
        public string Contacto { get; set; }

        [Display(Name = "Telefono:")]
        public string Telefono { get; set; }

        [Display(Name = "Servicio:")]
        public string Servicio { get; set; }

        [Display(Name = "Acción:")]
        public string Accion { get; set; }

        public IPagedList<Comision> Comisiones { get; set; }
    }
}