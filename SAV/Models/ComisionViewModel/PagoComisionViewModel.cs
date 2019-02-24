using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class PagoComisionViewModel
    {
        [Required(ErrorMessage = "Debe ingresar un Costo")]
        [RegularExpression("[0-9]?[0-9]?[0-9]?[0-9]?[0-9]?[0-9](\\,[0-9][0-9])", ErrorMessage = "El Costo debe ser un valor numerico entre 0 y 999999 con 2 digitos decimales")]
        [Display(Name = "Costo:")]
        public string Pago { get; set; }

        [Display(Name = "Forma de Pago:")]
        public List<KeyValuePair<int, string>> FormaPago { get; set; }

        [Required(ErrorMessage = "Debe ingresar una Forma de Pago")]
        public int SelectFormaPago { get; set; }

        public PagoComisionViewModel(Comision comision, List<FormaPago> formaPagos)
        {
            Pago = comision.Costo.ToString();
            FormaPago = formaPagos.Select(x => new KeyValuePair<int, string>(x.ID, x.Descripcion)).ToList();
        }
    }
}