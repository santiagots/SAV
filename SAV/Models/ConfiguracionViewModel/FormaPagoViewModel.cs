using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class FormaPagoViewModel
    {
        [Display(Name = "Forma de Pago:")]
        [Required(ErrorMessage = "Debe ingresar una Forma de Pago")]
        public string FormaPago { get; set; }

        public IPagedList<FormaPago> FormaPagos { get; set; }
    }
}