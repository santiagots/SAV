﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class AdicionalConductorViewModel
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Debe ingresar una Tipo de Gasto")]
        [Display(Name = "Tipo Adicional:")]
        public List<KeyValuePair<int, string>> TipoAdicional { get; set; }

        [Required(ErrorMessage = "Debe ingresar una Monto")]
        [RegularExpression("[0-9]?[0-9]?[0-9]?[0-9]?[0-9]?[0-9](\\,[0-9][0-9])", ErrorMessage = "El Monto debe ser un valor numerico entre 0 y 999999 con 2 digitos decimales")]
        public string Monto { get; set; }

        public string Comentario { get; set; }
    }
}