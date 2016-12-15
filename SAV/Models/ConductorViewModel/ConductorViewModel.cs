using PagedList;
using SAV.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class ConductorViewModel
    {
        [Display(Name = "Nombre:")]
        [Required(ErrorMessage = "Debe ingresar un Nombre")]
        [RegularExpression("^[A-Za-z ÑÁÉÍÓÚñáéíóú]+$", ErrorMessage = "El Nombre solo debe contener letras")]
        public string Nombre { get; set; }

        [Display(Name = "Apellido:")]
        [Required(ErrorMessage = "Debe ingresar un Apellido")]
        [RegularExpression("^[A-Za-z ÑÁÉÍÓÚñáéíóú]+$", ErrorMessage = "El Apellido solo debe contener letras")]
        public string Apellido { get; set; }

        [Display(Name = "CUIL:")]
        [Required(ErrorMessage = "Debe ingresar un CUIL")]
        [RegularExpression("^[0-9]{2}-[0-9]{8}-[0-9]$", ErrorMessage = "El CUIT debe ser en formato [2]-[8]-[1]")]
        public string CUIL { get; set; }

        [Display(Name = "Comision viaje:")]
        [Required(ErrorMessage = "Debe ingresar una Comision viaje")]
        [RegularExpression("[0-9]?[0-9]?[0-9](\\,[0-9][0-9])", ErrorMessage = "La Comision viaje debe ser un valor numerico entre 0 y 999 con 2 digitos decimales")]
        public string ComisionViaje { get; set; }

        [Display(Name = "Comision viaje cerrado:")]
        [Required(ErrorMessage = "Debe ingresar una Comision viaje cerrado")]
        [RegularExpression("[0-9]?[0-9](\\,[0-9][0-9])", ErrorMessage = "Comision viaje cerrado debe ser un valor porcentual entre 0 y 99 con 2 digitos decimales")]
        public string ComisionViajeCerrado { get; set; }

        [Display(Name = "Provincia:")]
        public List<KeyValuePair<int, string>> ProvinciaPrincipal { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una Provincia al Domicilio Principal")]
        public int SelectProvinciaPrincipal { get; set; }

        [Display(Name = "Localidad:")]
        public List<KeyValuePair<int, string>> LocalidadPrincipal { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una Localidad al Domicilio Principal")]
        public int SelectLocalidadPrincipal { get; set; }

        [Display(Name = "Calle:")]
        [Required(ErrorMessage = "Debe ingresar una Calle al Domicilio Principal")]
        [RegularExpression("^[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú]*[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú][A-Za-z0-9 ÑÁÉÍÓÚñáéíóú]*$", ErrorMessage = "La Calle solo debe contener número y letras")]
        public string CallePrincipal { get; set; }

        [Display(Name = "Numero:")]
        [Required(ErrorMessage = "Debe ingresar un Numero al Domicilio Principal")]
        [RegularExpression("^\\d{1,5}$", ErrorMessage = "El Numero debe ser un numero del 1 al 99999")]
        public string CalleNumeroPrincipal { get; set; }

        [Display(Name = "Piso:")]
        [RegularExpression("^[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.]*[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.][A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.]*$", ErrorMessage = "El Piso solo debe contener número y letras")]
        public string CallePisoPrincipal { get; set; }

        [Display(Name = "Teléfono:")]
        [Required(ErrorMessage = "Debe ingresar un Teléfono")]
        [RegularExpression("^(\\d{2,5})\\s(\\d{5,10})$", ErrorMessage = "El Teléfono debe ser en formaro CARACTERISTICA[5] NUMERO[10]")]
        public string Telefono { get; set; }
    
        [Display(Name = "Email:")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "El Email ingresado no tiene un formato valido")]
        public string Email { get; set; }

        public IPagedList Viajes { get; set; }

        public ConductorViewModel():base()
        {}

        public ConductorViewModel(List<Provincia> provincias, Conductor conductor)
        {
            Apellido = conductor.Apellido;
            Nombre = conductor.Nombre;
            Telefono = conductor.Telefono;
            Email = conductor.Email;
            CUIL = conductor.CUIL;
            ComisionViajeCerrado = conductor.ComisionViajeCerrado.ToString();
            ComisionViaje = conductor.ComisionViaje.ToString();

            ProvinciaPrincipal = provincias.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();

            if (conductor.Domicilio != null)
            {
                SelectProvinciaPrincipal = conductor.Domicilio.Provincia.ID;

                LocalidadPrincipal = provincias.Where(x => x.ID == conductor.Domicilio.Provincia.ID).FirstOrDefault().Localidad.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList<KeyValuePair<int, string>>();
                SelectLocalidadPrincipal = conductor.Domicilio.Localidad.ID;

                CallePrincipal = conductor.Domicilio.Calle;
                CalleNumeroPrincipal = conductor.Domicilio.Numero;
                CallePisoPrincipal = conductor.Domicilio.Piso;
            }
            else
                LocalidadPrincipal = new List<KeyValuePair<int, string>>();
        }

        public ConductorViewModel( List<Provincia> provincias)
        {
            ProvinciaPrincipal = provincias.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            LocalidadPrincipal = new List<KeyValuePair<int, string>>();
            Viajes = new List<ClienteViaje>().ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
        }

        public void updateConductor(ConductorViewModel conductorViewModel, List<Provincia> provincias, List<Localidad> localidades, ref Conductor conductor)
        {
            conductor.Apellido = conductorViewModel.Apellido;
            conductor.Nombre = conductorViewModel.Nombre;
            conductor.CUIL = conductorViewModel.CUIL;
            conductor.ComisionViajeCerrado = decimal.Parse(conductorViewModel.ComisionViajeCerrado);
            conductor.ComisionViaje = decimal.Parse(conductorViewModel.ComisionViaje);

            conductor.Domicilio = new Domicilio();
            if (conductorViewModel.SelectProvinciaPrincipal > 0)
                conductor.Domicilio.Provincia = provincias.Where(x => x.ID == conductorViewModel.SelectProvinciaPrincipal).FirstOrDefault();

            if (conductorViewModel.SelectLocalidadPrincipal > 0)
                conductor.Domicilio.Localidad = localidades.Where(x => x.ID == conductorViewModel.SelectLocalidadPrincipal).FirstOrDefault();

            conductor.Domicilio.Calle = conductorViewModel.CallePrincipal;
            conductor.Domicilio.Numero = conductorViewModel.CalleNumeroPrincipal;
            conductor.Domicilio.Piso = conductorViewModel.CallePisoPrincipal;

            conductor.Telefono = conductorViewModel.Telefono;
            conductor.Email = conductorViewModel.Email;
        }
    }
}