using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class CreateGastoViewModel
    {
        [Display(Name = "Concepto:")]
        public List<KeyValuePair<int, string>> Concepto { get; set; }

        [Required(ErrorMessage = "Debe ingresar una Concepto")]
        public int? SelectConcepto { get; set; }

        [Display(Name = "Tipo Gasto:")]
        public List<KeyValuePair<int, string>> TipoGasto { get; set; }

        [Required(ErrorMessage = "Debe ingresar una Tipo de Gasto")]
        public int? SelectTipoGasto { get; set; }

        [Display(Name = "Comentario:")]
        public string Comentario { get; set; }

        [Required(ErrorMessage = "Debe ingresar una Monto")]
        [Display(Name = "Monto:")]
        [RegularExpression("[0-9]?[0-9]?[0-9]?[0-9]?[0-9]?[0-9](\\,[0-9][0-9])", ErrorMessage = "El Monto debe ser un valor numerico entre 0 y 999999 con 2 digitos decimales")]
        public string Monto { get; set; }

        [Display(Name = "Usuario Alta:")]
        public string UsuarioAlta { get; set; }

        [Display(Name = "Fecha Alta:")]
        public string FechaAlta { get; set; }
    }
}