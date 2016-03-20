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
        [Display(Name = "Contacto:")]
        [Required(ErrorMessage = "Debe ingresar un Contacto")]
        [RegularExpression("^[A-Za-z ÑÁÉÍÓÚñáéíóú]+$", ErrorMessage = "El Nombre solo debe contener letras")]
        public string Contacto { get; set; }

        [Display(Name = "Teléfono:")]
        [Required(ErrorMessage = "Debe ingresar un Teléfono")]
        [RegularExpression("^(\\d{2,5})\\s(\\d{5,10})$", ErrorMessage = "El Teléfono debe ser en formaro CARACTERISTICA[5] NUMERO[10]")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "Debe ingresar un Costo")]
        [RegularExpression("[0-9]?[0-9]?[0-9]?[0-9](\\,[0-9][0-9])", ErrorMessage = "El Costo debe ser un valor numerico entre 0 y 9999 con 2 digitos decimales")]
        [Display(Name = "Costo:")]
        public string Costo { get; set; }

        [Display(Name = "Pagó:")]
        public bool Pago { get; set; }

        [Display(Name = "Comentario:")]
        public string Comentario { get; set; }

        [Display(Name = "Responsable:")]
        public List<KeyValuePair<int, string>> Responsable { get; set; }

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

        public ComisionViewModel(List<Provincia> provincias, Comision comision)
        {
            Contacto = comision.Contacto;
            Telefono = comision.Telefono;
            Costo = comision.Costo.ToString();
            Pago = comision.Pago;
            Comentario = comision.Comentario;
            Accion = comision.Accion;
            Servicio = comision.Servicio;

            ProvinciaRetirar = provincias.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            ProvinciaEntregar = provincias.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            LocalidadRetirar = new List<KeyValuePair<int, string>>();
            LocalidadEntregar = new List<KeyValuePair<int, string>>();
            ServicioDirectoRetirar = new List<KeyValuePair<int, string>>();
            ServicioDirectoEntregar = new List<KeyValuePair<int, string>>();

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
        }

        public ComisionViewModel(List<Provincia> provincias, List<ComisionResponsable> responsable)
        {
            Responsable = responsable.Select(x => new KeyValuePair<int, string>(x.ID, x.Apellido + ", " + x.Nombre )).ToList();
            ProvinciaRetirar = provincias.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            ProvinciaEntregar = provincias.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            LocalidadRetirar = new List<KeyValuePair<int, string>>();
            LocalidadEntregar = new List<KeyValuePair<int, string>>();
        }

        public ComisionViewModel(List<Provincia> provincias, Viaje viaje, List<ComisionResponsable> responsable)
            : this(provincias, responsable)
        {
            ServicioDirectoRetirar = viaje.Origen.Parada.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            ServicioDirectoRetirar.AddRange(viaje.Destino.Parada.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList());

            ServicioDirectoEntregar = viaje.Origen.Parada.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            ServicioDirectoEntregar.AddRange(viaje.Destino.Parada.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList());
        }

        public ComisionViewModel(List<Provincia> provincias, Viaje viaje, Comision comision, bool SetServicioDirecto, List<ComisionResponsable> responsable)
            : this(provincias, viaje, responsable)
        {
            Contacto = comision.Contacto;
            Telefono = comision.Telefono;
            Costo = comision.Costo.ToString();
            Comentario = comision.Comentario;
            Pago = comision.Pago;
            Accion = comision.Accion;
            Servicio = comision.Servicio;

            if (comision.Responsable != null)
                SelectResponsable = comision.Responsable.ID;

            if(comision.Servicio == ComisionServicio.Puerta)
            {
                if (comision.DomicilioEntregar != null)
                {
                    SelectProvinciaEntregar = comision.DomicilioEntregar.Provincia.ID;
                    LocalidadEntregar = provincias.Where(x=> x.ID == SelectProvinciaEntregar).FirstOrDefault().Localidad.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList<KeyValuePair<int, string>>();
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
            if (comision.Servicio == ComisionServicio.Directo && SetServicioDirecto)
            {
                ComisionViaje comisionViaje = viaje.ComisionesViaje.Where(x => x.Comision.ID == comision.ID).FirstOrDefault<ComisionViaje>();

                if (comisionViaje != null && comisionViaje.Retirar != null)
                    SelectServicioDirectoRetirar = comisionViaje.Retirar.ID;

                if (comisionViaje != null && comisionViaje.Entregar != null)
                    SelectServicioDirectoEntregar = comisionViaje.Entregar.ID;
            }
        }

        public ComisionViaje getComisionViaje(ComisionViewModel comisionViewModel, Viaje viaje, List<Parada> Paradas, Comision comision)
        {
            ComisionViaje comisionViaje = new ComisionViaje();

            comisionViaje.Comision = comision;
            comisionViaje.Viaje = viaje;
            comisionViaje.Pago = comisionViewModel.Pago;
            comisionViaje.Costo = decimal.Parse(comisionViewModel.Costo);

            if (comisionViewModel.Servicio == ComisionServicio.Directo)
            {
                comisionViaje.Entregar = Paradas.Where(x => x.ID ==  comisionViewModel.SelectServicioDirectoEntregar).FirstOrDefault();
                comisionViaje.Retirar = Paradas.Where(x => x.ID == comisionViewModel.SelectServicioDirectoRetirar).FirstOrDefault();
            }
            return comisionViaje;
        }

        public Comision getComision(ComisionViewModel comisionViewModel, List<Provincia> provincias, List<Localidad> localidades)
        {
            Comision comision = new Comision();

            upDateComision(comisionViewModel, provincias, localidades, ref comision);

            return comision;
        }

        public void upDateComision(ComisionViewModel comisionViewModel, List<Provincia> provincias, List<Localidad> localidades, ref Comision comision)
        {
            comision.Contacto = comisionViewModel.Contacto.ToUpper();
            comision.Telefono = comisionViewModel.Telefono;
            comision.Accion = comisionViewModel.Accion;
            comision.Servicio = comisionViewModel.Servicio;
            comision.Costo = decimal.Parse(comisionViewModel.Costo);
            comision.Comentario = comisionViewModel.Comentario;

            if (comisionViewModel.Servicio == ComisionServicio.Puerta)
            {
                comision.DomicilioEntregar = setDomicilio(comisionViewModel.SelectProvinciaEntregar, comisionViewModel.SelectLocalidadEntregar, comisionViewModel.CalleEntregar, comisionViewModel.CalleNumeroEntregar, comisionViewModel.CallePisoEntregar, provincias, localidades);

                comision.DomicilioRetirar = setDomicilio(comisionViewModel.SelectProvinciaRetirar, comisionViewModel.SelectLocalidadRetirar, comisionViewModel.CalleRetirar, comisionViewModel.CalleNumeroRetirar, comisionViewModel.CallePisoRetirar, provincias, localidades);
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