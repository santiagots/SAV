using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class Gasto
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Debe ingresar un Razon Social")]
        [RegularExpression("^[A-Za-z ÑÁÉÍÓÚñáéíóú]+$", ErrorMessage = "La Razon Social solo debe contener letras")]
        public string RazonSocial { get; set; }

        [Required(ErrorMessage = "Debe ingresar un CUIT")]
        [RegularExpression("^[0-9]{2}-[0-9]{8}-[0-9]$", ErrorMessage = "El CUIT debe ser en formato [2]-[8]-[1]")]
        public string CUIT { get; set; }

        [Required(ErrorMessage = "Debe ingresar un Numero de Ticket")]
        [RegularExpression("^\\d{1,10}$", ErrorMessage = "El Numero de Ticket debe ser un numero de hasta 10 digitos")]
        public long NroTicket { get; set; }

        [Required(ErrorMessage = "Debe ingresar una Monto")]
        [RegularExpression("[0-9]?[0-9]?[0-9]?[0-9](\\,[0-9][0-9])", ErrorMessage = "El Monto debe ser un valor numerico entre 0 y 9999 con 2 digitos decimales")]
        public string Monto { get; set; }

        public string Comentario { get; set; }
    }
}