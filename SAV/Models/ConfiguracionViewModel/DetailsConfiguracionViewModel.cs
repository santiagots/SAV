using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class DetailsDatosEmpresaViewModel
    {
        [Display(Name = "Razon Social:")]
        [Required(ErrorMessage = "Debe ingresar una Razon Social")]
        public string RazonSocial { get; set; }

        [Display(Name = "CUIT:")]
        [Required(ErrorMessage = "Debe ingresar un CUIT")]
        [RegularExpression("^[0-9]{2}-[0-9]{8}-[0-9]$", ErrorMessage = "El CUIT debe ser en formato [2]-[8]-[1]")]
        public string CUIT { get; set; }

        #region domicilio
        [Display(Name = "Provincia:")]
        public List<KeyValuePair<int, string>> Provincia { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una Provincia")]
        public int SelectProvincia { get; set; }

        [Display(Name = "Localidad:")]
        public List<KeyValuePair<int, string>> Localidad { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una Localidad")]
        public int SelectLocalidad { get; set; }

        [Display(Name = "Calle:")]
        [Required(ErrorMessage = "Debe ingresar una Calle")]

        [RegularExpression("^[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.]*[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.][A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.]*$", ErrorMessage = "La Calle solo debe contener número y letras")]
        public string Calle { get; set; }

        [Display(Name = "Numero:")]
        [Required(ErrorMessage = "Debe ingresar un Numero al Domicilio")]
        [RegularExpression("^\\d{1,5}$", ErrorMessage = "El Numero debe ser un numero del 1 al 99999")]
        public string CalleNumero { get; set; }

        [Display(Name = "Piso:")]
        [RegularExpression("^[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.]*[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.][A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.]*$", ErrorMessage = "El Piso solo debe contener número y letras")]
        public string CallePiso { get; set; }
        #endregion

        [Display(Name = "Teléfono:")]
        [Required(ErrorMessage = "Debe ingresar un Teléfono")]
        [RegularExpression("^(\\d{2,5})\\s(\\d{5,10})$", ErrorMessage = "El Teléfono debe ser en formaro CARACTERISTICA[5] NUMERO[10]")]
        public string Telefono { get; set; }

        [Display(Name = "CUIT:")]
        [Required(ErrorMessage = "Debe ingresar un CUIT")]
        [RegularExpression("^[0-9]{2}-[0-9]{8}-[0-9]$", ErrorMessage = "El CUIT debe ser en formato [2]-[8]-[1]")]
        public string ContrtanteCuit { get; set; }

        [Display(Name = "Denominación:")]
        [Required(ErrorMessage = "Debe ingresar un Denominación")]
        public string ContratanteDenominacion { get; set; }

        #region domicilio contratante
        [Display(Name = "Provincia:")]
        public List<KeyValuePair<int, string>> ContratanteProvincia { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una Provincia")]
        public int ContratanteSelectProvincia { get; set; }

        [Display(Name = "Localidad:")]
        public List<KeyValuePair<int, string>> ContratanteLocalidad { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una Localidad")]
        public int ContratanteSelectLocalidad { get; set; }

        [Display(Name = "Calle:")]
        [Required(ErrorMessage = "Debe ingresar una Calle")]

        [RegularExpression("^[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.]*[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.][A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.]*$", ErrorMessage = "La Calle solo debe contener número y letras")]
        public string ContratanteCalle { get; set; }

        [Display(Name = "Numero:")]
        [Required(ErrorMessage = "Debe ingresar un Numero al Domicilio")]
        [RegularExpression("^\\d{1,5}$", ErrorMessage = "El Numero debe ser un numero del 1 al 99999")]
        public string ContratanteCalleNumero { get; set; }

        [Display(Name = "Piso:")]
        [RegularExpression("^[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.]*[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.][A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.]*$", ErrorMessage = "El Piso solo debe contener número y letras")]
        public string ContratanteCallePiso { get; set; }
        #endregion

        [Display(Name = "Programacion Turistica:")]
        [Required(ErrorMessage = "Debe ingresar un Programacion Turistica")]
        public string ProgramacionTuristica { get; set; }

        public DetailsDatosEmpresaViewModel():base()
        { }

        public DetailsDatosEmpresaViewModel(DatosEmpresa configuracion, List<Provincia> provincias, List<Localidad> Localidades) : this(provincias, Localidades)
        {
            RazonSocial = configuracion.RazonSocial;
            CUIT = configuracion.CUIT;
            SelectProvincia = configuracion.Domicilio.Provincia.ID;
            SelectLocalidad = configuracion.Domicilio.Localidad.ID;
            Calle = configuracion.Domicilio.Calle;
            CalleNumero = configuracion.Domicilio.Numero;
            CallePiso = configuracion.Domicilio.Piso;
            Telefono = configuracion.Telefono;

            ContrtanteCuit = configuracion.ContrtanteCuit;
            ContratanteDenominacion = configuracion.ContratanteDenominacion;
            ContratanteSelectProvincia = configuracion.ContratanteDomicilio.Provincia.ID;
            ContratanteSelectLocalidad = configuracion.ContratanteDomicilio.Localidad.ID;
            ContratanteCalle = configuracion.ContratanteDomicilio.Calle;
            ContratanteCalleNumero = configuracion.ContratanteDomicilio.Numero;
            ContratanteCallePiso = configuracion.ContratanteDomicilio.Piso;
        }

        public DetailsDatosEmpresaViewModel(List<Provincia> provincias, List<Localidad> Localidades)
        {
            Provincia = provincias.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            ContratanteProvincia = provincias.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();

            Localidad = Localidades.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            ContratanteLocalidad = Localidades.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
        }

        public DetailsDatosEmpresaViewModel(List<Provincia> provincias)
        {
            Provincia = provincias.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            ContratanteProvincia = provincias.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();

            Localidad = new List<KeyValuePair<int, string>>();
            ContratanteLocalidad = new List<KeyValuePair<int, string>>();
        }

        public static DatosEmpresa getConfiguracion(DetailsDatosEmpresaViewModel detailsDatosEmpresaViewModel, DatosEmpresa configuracion, List<Provincia> Provincias, List<Localidad> Localidades)
        {
            configuracion.ContratanteDenominacion = detailsDatosEmpresaViewModel.ContratanteDenominacion;
            configuracion.ContratanteDomicilio.Calle = detailsDatosEmpresaViewModel.ContratanteCalle;
            configuracion.ContratanteDomicilio.Localidad = Localidades.Where(x => x.ID == detailsDatosEmpresaViewModel.ContratanteSelectLocalidad).FirstOrDefault();
            configuracion.ContratanteDomicilio.Numero = detailsDatosEmpresaViewModel.ContratanteCalleNumero;
            configuracion.ContratanteDomicilio.Piso = detailsDatosEmpresaViewModel.ContratanteCallePiso;
            configuracion.ContratanteDomicilio.Provincia = Provincias.Where(x => x.ID == detailsDatosEmpresaViewModel.ContratanteSelectProvincia).FirstOrDefault();
            configuracion.ContrtanteCuit = detailsDatosEmpresaViewModel.ContrtanteCuit;
            configuracion.CUIT = detailsDatosEmpresaViewModel.CUIT;
            configuracion.Domicilio.Calle = detailsDatosEmpresaViewModel.Calle;
            configuracion.Domicilio.Localidad = Localidades.Where(x => x.ID == detailsDatosEmpresaViewModel.SelectLocalidad).FirstOrDefault();
            configuracion.Domicilio.Numero = detailsDatosEmpresaViewModel.CalleNumero;
            configuracion.Domicilio.Piso = detailsDatosEmpresaViewModel.CallePiso;
            configuracion.Domicilio.Provincia = Provincias.Where(x => x.ID == detailsDatosEmpresaViewModel.SelectProvincia).FirstOrDefault();
            configuracion.RazonSocial = detailsDatosEmpresaViewModel.RazonSocial;
            configuracion.Telefono = detailsDatosEmpresaViewModel.Telefono;

            return configuracion;
        }
    }
}