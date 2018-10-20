using SAV.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public enum ComisionAccion { Entregar, Retirar, EntregarRetirar }
    public enum ComisionServicio { Directo, Puerta }

    public class ComisionViewModel
    {
        public int idCuentaCorriente { get; set; }

        [Display(Name = "Contacto:")]
        [Required(ErrorMessage = "Debe ingresar un Contacto")]
        [RegularExpression("^[A-Za-z ÑÁÉÍÓÚñáéíóú]+$", ErrorMessage = "El Nombre solo debe contener letras")]
        public string Contacto { get; set; }

        [Display(Name = "Teléfono:")]
        [Required(ErrorMessage = "Debe ingresar un Teléfono")]
        [RegularExpression("^(\\d{2,5})\\s(\\d{5,10})$", ErrorMessage = "El Teléfono debe ser en formaro CARACTERISTICA[5] NUMERO[10]")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "Debe ingresar un Costo")]
        [RegularExpression("[0-9]?[0-9]?[0-9]?[0-9]?[0-9]?[0-9](\\,[0-9][0-9])", ErrorMessage = "El Costo debe ser un valor numerico entre 0 y 999999 con 2 digitos decimales")]
        [Display(Name = "Costo:")]
        public string Costo { get; set; }

        [Display(Name = "Pagó:")]
        public bool Pago { get; set; }

        [Display(Name = "Comentario:")]
        public string Comentario { get; set; }

        [Display(Name = "Responsable:")]
        public List<KeyValuePair<int, string>> Responsable { get; set; }

        [Display(Name = "Fecha Envio:")]
        public string FechaEnvio { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un Responsable")]
        public int SelectResponsable { get; set; }

        [Display(Name = "Acción:")]
        [Required(ErrorMessage = "Debe seleccionar una Accion")]
        public ComisionAccion Accion { get; set; }

        [Display(Name = "Servicio:")]
        [Required(ErrorMessage = "Debe seleccionar una Servicio")]
        public ComisionServicio Servicio { get; set; }

        [Display(Name = "Provincia:")]
        public List<KeyValuePair<int, string>> ProvinciaEntregar { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una Provincia al Domicilio Principal")]
        public int SelectProvinciaEntregar { get; set; }

        [Display(Name = "Localidad:")]
        public List<KeyValuePair<int, string>> LocalidadEntregar { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una Localidad al Domicilio Principal")]
        public int SelectLocalidadEntregar { get; set; }

        [Display(Name = "Calle:")]
        [Required(ErrorMessage = "Debe ingresar una Calle al Domicilio Principal")]
        [RegularExpression("^[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú]*[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.][A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.]*$", ErrorMessage = "La Calle solo debe contener número y letras")]
        public string CalleEntregar { get; set; }

        [Display(Name = "Numero:")]
        [Required(ErrorMessage = "Debe ingresar un Numero al Domicilio Principal")]
        [RegularExpression("^\\d{1,5}$", ErrorMessage = "El Numero debe ser un numero del 1 al 99999")]
        public string CalleNumeroEntregar { get; set; }

        [Display(Name = "Piso:")]
        [RegularExpression("^[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.]*[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.][A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.]*$", ErrorMessage = "El Piso solo debe contener número y letras")]
        public string CallePisoEntregar { get; set; }

        [Display(Name = "Provincia:")]
        public List<KeyValuePair<int, string>> ProvinciaRetirar { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una Provincia al Domicilio Secundario")]
        public int SelectProvinciaRetirar { get; set; }

        [Display(Name = "Localidad:")]
        public List<KeyValuePair<int, string>> LocalidadRetirar { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una Localidad al Domicilio Secundario")]
        public int SelectLocalidadRetirar { get; set; }

        [Display(Name = "Calle:")]
        [Required(ErrorMessage = "Debe ingresar una Calle al Domicilio Secundario")]
        [RegularExpression("^[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú]*[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.][A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.]*$", ErrorMessage = "La Calle solo debe contener número y letras")]
        public string CalleRetirar { get; set; }

        [Display(Name = "Numero:")]
        [Required(ErrorMessage = "Debe ingresar un Numero al Domicilio Secundario")]
        [RegularExpression("^\\d{1,5}$", ErrorMessage = "El Numero debe ser un numero del 1 al 99999")]
        public string CalleNumeroRetirar { get; set; }

        [Display(Name = "Piso:")]
        [RegularExpression("^[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.]*[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.][A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.]*$", ErrorMessage = "El Piso solo debe contener número y letras")]
        public string CallePisoRetirar { get; set; }

        [Display(Name = "Retirar:")]
        public List<KeyValuePair<int, string>> ServicioDirectoRetirar { get; set; }

        [Required(ErrorMessage = "Debe ingresar un Ascenso")]
        public int SelectServicioDirectoRetirar { get; set; }

        [Display(Name = "Entregar:")]
        public List<KeyValuePair<int, string>> ServicioDirectoEntregar { get; set; }

        [Required(ErrorMessage = "Debe ingresar un Descenso")]
        public int SelectServicioDirectoEntregar { get; set; }

        public ComisionViewModel() : base() { }

        public ComisionViewModel(List<Provincia> provincias, Comision comision, List<ComisionResponsable> responsable, List<Parada> paradas)
        {
            Contacto = comision.Contacto;
            Telefono = comision.Telefono;
            Costo = comision.Costo.ToString();
            Pago = comision.Pago;
            Comentario = comision.Comentario;
            Accion = comision.Accion;
            Servicio = comision.Servicio;
            FechaEnvio = comision.FechaEnvio.HasValue ? comision.FechaEnvio.Value.ToString("dd/MM/yyyy") : string.Empty;
            Responsable = responsable.Select(x => new KeyValuePair<int, string>(x.ID, x.Apellido + ", " + x.Nombre)).ToList();

            if(comision.Responsable != null)
                SelectResponsable = comision.Responsable.ID;

            ProvinciaRetirar = provincias.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            ProvinciaEntregar = provincias.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            LocalidadRetirar = new List<KeyValuePair<int, string>>();
            LocalidadEntregar = new List<KeyValuePair<int, string>>();
            ServicioDirectoRetirar = paradas.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            ServicioDirectoEntregar = paradas.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();

            if (comision.Servicio == ComisionServicio.Puerta)
            {
                if (comision.DomicilioEntregar != null)
                {
                    SelectProvinciaEntregar = comision.DomicilioEntregar.Provincia.ID;
                    LocalidadEntregar = provincias.Where(x => x.ID == SelectProvinciaEntregar).FirstOrDefault().Localidad.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList<KeyValuePair<int, string>>();
                    SelectLocalidadEntregar = comision.DomicilioEntregar.Localidad.ID;
                    CalleEntregar = comision.DomicilioEntregar.Calle;
                    CalleNumeroEntregar = comision.DomicilioEntregar.Numero;
                    CallePisoEntregar = comision.DomicilioEntregar.Piso;
                }
                if (comision.DomicilioRetirar != null)
                {
                    SelectProvinciaRetirar = comision.DomicilioRetirar.Provincia.ID;
                    LocalidadRetirar = provincias.Where(x => x.ID == SelectProvinciaRetirar).FirstOrDefault().Localidad.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList<KeyValuePair<int, string>>();
                    SelectLocalidadRetirar = comision.DomicilioRetirar.Localidad.ID;
                    CalleRetirar = comision.DomicilioRetirar.Calle;
                    CalleNumeroRetirar = comision.DomicilioRetirar.Numero;
                    CallePisoRetirar = comision.DomicilioRetirar.Piso;
                }
            }
            else
            {
                if(comision.Retirar != null)
                    SelectServicioDirectoRetirar = comision.Retirar.ID;
                if (comision.Entregar != null)
                    SelectServicioDirectoEntregar = comision.Entregar.ID;
            }
        }

        public ComisionViewModel(List<Provincia> provincias, List<ComisionResponsable> responsable, List<Parada> paradas)
        {
            Responsable = responsable.Select(x => new KeyValuePair<int, string>(x.ID, x.Apellido + ", " + x.Nombre )).ToList();
            ProvinciaRetirar = provincias.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            ProvinciaEntregar = provincias.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            ServicioDirectoRetirar = paradas.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            ServicioDirectoEntregar = paradas.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            LocalidadRetirar = new List<KeyValuePair<int, string>>();
            LocalidadEntregar = new List<KeyValuePair<int, string>>();
        }

        public Comision getComision(ComisionViewModel comisionViewModel, List<Provincia> provincias, List<Localidad> localidades, List<Parada> parada)
        {
            Comision comision = new Comision();

            upDateComision(comisionViewModel, provincias, localidades, parada, ref comision);
            comision.FechaAlta = DateTime.Now;
            return comision;
        }

        public void upDateComision(ComisionViewModel comisionViewModel, List<Provincia> provincias, List<Localidad> localidades, List<Parada> parada, ref Comision comision)
        {
            comision.Contacto = comisionViewModel.Contacto.ToUpper();
            comision.Telefono = comisionViewModel.Telefono;
            comision.Accion = comisionViewModel.Accion;
            comision.Servicio = comisionViewModel.Servicio;
            comision.Costo = decimal.Parse(comisionViewModel.Costo);
            comision.Comentario = comisionViewModel.Comentario;
            comision.Pago = comisionViewModel.Pago;
            comision.FechaEnvio = ComisionHelper.getFecha(comisionViewModel.FechaEnvio);
            comision.Planificada = comision.FechaEnvio.HasValue;
            
            if (comisionViewModel.Pago)
                comision.FechaPago = DateTime.Now;
            else
                comision.FechaPago = null;

            if (comisionViewModel.Servicio == ComisionServicio.Puerta)
            {
                comision.DomicilioEntregar = setDomicilio(comisionViewModel.SelectProvinciaEntregar, comisionViewModel.SelectLocalidadEntregar, comisionViewModel.CalleEntregar, comisionViewModel.CalleNumeroEntregar, comisionViewModel.CallePisoEntregar, provincias, localidades);

                comision.DomicilioRetirar = setDomicilio(comisionViewModel.SelectProvinciaRetirar, comisionViewModel.SelectLocalidadRetirar, comisionViewModel.CalleRetirar, comisionViewModel.CalleNumeroRetirar, comisionViewModel.CallePisoRetirar, provincias, localidades);
            }
            else
            {
                comision.Entregar = parada.Where(x => x.ID == comisionViewModel.SelectServicioDirectoEntregar).FirstOrDefault();
                comision.Retirar = parada.Where(x => x.ID == comisionViewModel.SelectServicioDirectoRetirar).FirstOrDefault();
            }
        }

        private Domicilio setDomicilio(int provincia, int localidad, string calle, string numero, string piso, List<Provincia> provincias, List<Localidad> localidades)
        {
            if (provincia > 0 && localidad > 0 && !string.IsNullOrEmpty(calle) && !string.IsNullOrEmpty(numero))
            {
                Domicilio domicilio = new Domicilio();
                if (provincia > 0)
                    domicilio.Provincia = provincias.Where(x => x.ID == provincia).FirstOrDefault();

                if (localidad > 0)
                    domicilio.Localidad = localidades.Where(x => x.ID == localidad).FirstOrDefault();

                domicilio.Calle = calle.ToUpper();
                domicilio.Numero = numero;
                domicilio.Piso = piso;

                return domicilio;
            }
            else
                return null;
        }
    }
}