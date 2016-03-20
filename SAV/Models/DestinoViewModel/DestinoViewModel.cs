using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class DestinoViewModel
    {
        [Display(Name = "Provincia:")]
        public List<KeyValuePair<int, string>> Provincia { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una Provincia al Domicilio Principal")]
        public int SelectProvincia { get; set; }

        [Display(Name = "Localidad:")]
        public List<KeyValuePair<int, string>> Localidad { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una Localidad al Domicilio Principal")]
        public int SelectLocalidad { get; set; }

        [Display(Name = "Parada:")]
        public List<KeyValuePair<int, string>> Parada { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una Localidad al Domicilio Principal")]
        public int SelectParada { get; set; }

        [Display(Name = "Nueva Parada:")]
        [Required(ErrorMessage = "Debe ingresar un nueva Parada")]
        [RegularExpression("^[A-Za-z ÑÁÉÍÓÚñáéíóú]+$", ErrorMessage = "La nueva parada solo debe contener letras")]
        public string NuevaParada { get; set; }

        public DestinoViewModel(): base()
        {}

        public DestinoViewModel(List<Provincia> provincias)
        {
            Provincia = provincias.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            Localidad = new List<KeyValuePair<int, string>>();
            Parada = new List<KeyValuePair<int, string>>();
        }
    }
}