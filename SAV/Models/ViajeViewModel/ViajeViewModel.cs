using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class ViajeViewModel
    {
        public int viajeID { get; set; }

        [Display(Name = "Conductor:")]
        public List<KeyValuePair<int, string>> ConductorNombre { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un Conductor")]
        public string SelectConductorNombre { get; set; }

        [Display(Name = "Origen:")]
        public List<KeyValuePair<int, string>> Origen { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un Origen")]
        public string SelectOrigen { get; set; }

        [Display(Name = "Destino:")]
        public List<KeyValuePair<int, string>> Destino { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un Destino")]
        public string SelectDestino { get; set; }

        [Display(Name = "Origen:")]
        [Required(ErrorMessage = "Debe ingresar un Origen")]
        [RegularExpression("^[A-Za-z ÑÁÉÍÓÚñáéíóú]+$", ErrorMessage = "El Origen solo debe contener letras")]
        public string OrigenCerrado { get; set; }

        [Display(Name = "Provincia Origen:")]
        public List<KeyValuePair<int, string>> ProvinciaOrigenCerrado { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un Provincia Origen")]
        public int SelectProvinciaOrigenCerrado { get; set; }

        [Display(Name = "Destino:")]
        [Required(ErrorMessage = "Debe ingresar un Destino")]
        [RegularExpression("^[A-Za-z ÑÁÉÍÓÚñáéíóú]+$", ErrorMessage = "El Destino solo debe contener letras")]
        public string DestinoCerrado { get; set; }

        [Display(Name = "Provincia Destino:")]
        public List<KeyValuePair<int, string>> ProvinciaDestinoCerrado { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un Provincia Destino")]
        public int SelectProvinciaDestinoCerrado { get; set; }

        [Display(Name = "Costo:")]
        [Required(ErrorMessage = "Debe ingresar un Costo")]
        [RegularExpression("[0-9]?[0-9]?[0-9]?[0-9]?[0-9](\\,[0-9][0-9])", ErrorMessage = "El Costo debe ser un valor numerico entre 0 y 99999 con 2 digitos decimales")]
        public string CostoCerrado { get; set; }

        [Required(ErrorMessage = "Debe ingresar una cantidad de Asientos")]
        [RegularExpression("^[1-9][0-9]?$", ErrorMessage = "La cantidad de asientos debe ser un numero del 1 al 99")]
        [Display(Name = "Cantidad de Asientos:")]
        public int Asientos { get; set; }

        [Required(ErrorMessage = "Debe ingresar una Fecha de Salida")]
        [Display(Name = "Fecha de Salida:")]
        public string FechaSalida { get; set; }

        [Required(ErrorMessage = "Debe ingresar una Hora de Salida")]
        [RegularExpression("^(?:0?[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "La hora de salida debe ser en formaro HH:MM 24hs")]
        [Display(Name = "Hora de Salida:")]
        public string HoraSalida { get; set; }

        [Required(ErrorMessage = "Debe ingresar una Hora de Arribo")]
        [RegularExpression("^(?:0?[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$", ErrorMessage = "La hora de arribo debe ser en formaro HH:MM 24hs")]
        [Display(Name = "Hora de Arribo:")]
        public string HoraArribo { get; set; }
    
        [Required(ErrorMessage = "Debe seleccionar una Tipo de Servicio")]
        [Display(Name = "Tipo de Servicio:")]
        public virtual ViajeTipoServicio Servicio { get; set; }

        [Display(Name = "Disponible:")]
        public int Disponible { get; set; }

        [Required(ErrorMessage = "Debe ingresar una Patente")]
        [RegularExpression("(^[A-ZÑ]{3}\\d{3})|(^[A-ZÑ]{2}\\d{3}[A-ZÑ]{2})$", ErrorMessage = "La patente ingresada es invalida debe respetar el formato XXX999 o XX999XX")]
        [Display(Name = "Patente:")]
        public string Patente { get; set; }

        [RegularExpression("(^[A-ZÑ]{3}\\d{3})|(^[A-ZÑ]{2}\\d{3}[A-ZÑ]{2})$", ErrorMessage = "La patente suplente ingresada es invalida debe respetar el formato XXX999 o XX999XX")]
        [Display(Name = "Patente Suplente:")]
        public string PatenteSuplente { get; set; }

        [Required(ErrorMessage = "Debe ingresar una Interno")]
        [RegularExpression("^[1-9][0-9]?[0-9]?$", ErrorMessage = "El interno debe ser un numero del 1 al 999")]
        [Display(Name = "Interno:")]
        public int Interno { get; set; }

        [Display(Name = "Estado:")]
        public ViajeEstados Estado { get; set; }
    }
}