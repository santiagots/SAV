using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class DomicilioViewModel
    {
        [Display(Name = "Provincia:")]
        public List<KeyValuePair<int, string>> Provincia { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una Provincia al Domicilio")]
        public int SelectProvincia { get; set; }

        [Display(Name = "Localidad:")]
        public List<KeyValuePair<int, string>> Localidad { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una Localidad al Domicilio")]
        public int SelectLocalidad { get; set; }

        [Display(Name = "Calle:")]
        [Required(ErrorMessage = "Debe ingresar una Calle al Domicilio")]

        [RegularExpression("^[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.]*[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.][A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.]*$", ErrorMessage = "La Calle solo debe contener número y letras")]
        public string Calle { get; set; }

        [Display(Name = "Numero:")]
        [Required(ErrorMessage = "Debe ingresar un Numero al Domicilio")]
        [RegularExpression("^\\d{1,5}$", ErrorMessage = "El Numero debe ser un numero del 1 al 99999")]
        public string Numero { get; set; }

        [Display(Name = "Piso:")]
        [RegularExpression("^[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.]*[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.][A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.]*$", ErrorMessage = "El Piso solo debe contener número y letras")]
        public string Piso { get; set; }

        public string Comentario { get; set; }

        public DomicilioViewModel()
        {
        }


        public DomicilioViewModel(List<Provincia> provincias)
        {
            Provincia = provincias.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            Localidad = new List<KeyValuePair<int, string>>();
        }

        public DomicilioViewModel(List<Provincia> provincias, List<Localidad> localidades)
        {
            Provincia = provincias.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            Localidad = localidades.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
        }
    }
}