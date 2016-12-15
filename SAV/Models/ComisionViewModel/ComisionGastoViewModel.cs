using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class ComisionGastoViewModel
    {
        [Required(ErrorMessage = "Debe ingresar una Descripción")]
        [Display(Name = "Descripción:")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Debe ingresar una Monto")]
        [Display(Name = "Monto:")]
        [RegularExpression("[0-9]?[0-9]?[0-9]?[0-9](\\,[0-9][0-9])", ErrorMessage = "El Monto debe ser un valor numerico entre 0 y 9999 con 2 digitos decimales")]
        public string Monto { get; set; }

        [Display(Name = "Descripción:")]
        public string BuscarDescriptcion { get; set; }

        [Display(Name = "Monto:")]
        public string BuscarMonto { get; set; }

        [Display(Name = "Fecha Alta:")]
        public string BuscarFecha { get; set; }

        public IPagedList<ComisionGasto> Gastos { get; set; }
    }
}