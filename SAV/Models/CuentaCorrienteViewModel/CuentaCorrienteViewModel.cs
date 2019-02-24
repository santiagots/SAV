using PagedList;
using SAV.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SAV.Models.CuentaCorrienteViewModel
{
    public class CuentaCorrienteViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Razón Social/Nombre:")]
        [Required(ErrorMessage = "Debe ingresar un Nombre")]
        [RegularExpression("^[A-Za-z ÑÁÉÍÓÚñáéíóú]+$", ErrorMessage = "El Nombre solo debe contener letras")]
        public string RazonSocial { get; set; }

        [Display(Name = "CUIL/DNI:")]
        [Required(ErrorMessage = "Debe ingresar un CUIL o DNI")]
        public string CUIL { get; set; }

        [Display(Name = "Provincia:")]
        public List<KeyValuePair<int, string>> Provincia { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una Provincia al Domicilio Principal")]
        public int SelectProvincia { get; set; }

        [Display(Name = "Localidad:")]
        public List<KeyValuePair<int, string>> Localidad { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una Localidad al Domicilio Principal")]
        public int SelectLocalidad { get; set; }

        [Display(Name = "Calle:")]
        [Required(ErrorMessage = "Debe ingresar una Calle al Domicilio Principal")]
        [RegularExpression("^[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú]*[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú][A-Za-z0-9 ÑÁÉÍÓÚñáéíóú]*$", ErrorMessage = "La Calle solo debe contener número y letras")]
        public string Calle { get; set; }

        [Display(Name = "Numero:")]
        [Required(ErrorMessage = "Debe ingresar un Numero al Domicilio Principal")]
        [RegularExpression("^\\d{1,5}$", ErrorMessage = "El Numero debe ser un numero del 1 al 99999")]
        public string CalleNumero { get; set; }

        [Display(Name = "Piso:")]
        [RegularExpression("^[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.]*[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.][A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.]*$", ErrorMessage = "El Piso solo debe contener número y letras")]
        public string CallePiso { get; set; }

        [Display(Name = "Teléfono:")]
        [Required(ErrorMessage = "Debe ingresar un Teléfono")]
        [RegularExpression("^(\\d{2,5})\\s(\\d{5,10})$", ErrorMessage = "El Teléfono debe ser en formaro CARACTERISTICA[5] NUMERO[10]")]
        public string Telefono { get; set; }

        [Display(Name = "Email:")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "El Email ingresado no tiene un formato valido")]
        public string Email { get; set; }

        [Display(Name = "Monto:")]
        [Required(ErrorMessage = "Debe ingresar un Monto de entrega.")]
        [RegularExpression("[0-9]?[0-9]?[0-9]?[0-9]?[0-9]?[0-9](\\,[0-9][0-9])", ErrorMessage = "La Entrega debe ser un valor numerico entre 0 y 999999 con 2 digitos decimales")]
        public string MontoEntrega { get; set; }

        [Display(Name = "Forma de Pago:")]
        public List<KeyValuePair<int, string>> FormaPago { get; set; }

        [Required(ErrorMessage = "Debe ingresar una Forma de Pago")]
        public int SelectFormaPago { get; set; }

        [Display(Name = "Numero:")]
        public string Numero { get; set; }

        [Display(Name = "Fecha Alta:")]
        public string FechaAlta { get; set; }

        [Display(Name = "Fecha Entrega:")]
        public string FechaEntrega { get; set; }

        [Display(Name = "Fecha Pago:")]
        public string FechaPago { get; set; }

        public IPagedList ComisionesDebe { get; set; }

        public IPagedList ComisionesPagas { get; set; }

        [Display(Name = "Total Deuda:")]
        public decimal TotalDeuda { get; set; }

        public CuentaCorrienteViewModel()
        {
        }

        public CuentaCorrienteViewModel(List<Provincia> provincias)
        {
            Provincia = provincias.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            Localidad = new List<KeyValuePair<int, string>>();
        }

        public CuentaCorrienteViewModel(List<Provincia> provincias, List<Localidad> localidades)
        {
            Provincia = provincias.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();

            Localidad = localidades.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
        }

        public CuentaCorrienteViewModel(List<Provincia> provincias, List<Comision> comisiones): this(provincias)
        {
            ComisionesDebe = comisiones.ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
            ComisionesPagas = new List<Comision>().ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
        }

        internal CuentaCorriente getCuentaCorriente(List<Provincia> provincias, List<Localidad> localidades)
        {
            CuentaCorriente cuentaCorriente = new CuentaCorriente() {
                CUIL = this.CUIL,
                Domicilio = new Domicilio() {
                    Calle = this.Calle,
                    Provincia = provincias.Where(x => x.ID == this.SelectProvincia).FirstOrDefault(),
                    Localidad = localidades.Where(x => x.ID == this.SelectLocalidad).FirstOrDefault(),
                    Numero = this.CalleNumero,
                    Piso = this.CallePiso },
                Email = this.Email,
                RazonSocial = this.RazonSocial,
                Telefono = this.Telefono
            };

            return cuentaCorriente;
        }

        internal CuentaCorriente UpDateCuentaCorriente(CuentaCorriente cuentaCorriente, List<Provincia> provincias, List<Localidad> localidades)
        {

            cuentaCorriente.CUIL = this.CUIL;
            cuentaCorriente.Domicilio.Calle = this.Calle;
            cuentaCorriente.Domicilio.Provincia = provincias.Where(x => x.ID == this.SelectProvincia).FirstOrDefault();
            cuentaCorriente.Domicilio.Localidad = localidades.Where(x => x.ID == this.SelectLocalidad).FirstOrDefault();
            cuentaCorriente.Domicilio.Numero = this.CalleNumero;
            cuentaCorriente.Domicilio.Piso = this.CallePiso;
            cuentaCorriente.Email = this.Email;
            cuentaCorriente.RazonSocial = this.RazonSocial;
            cuentaCorriente.Telefono = this.Telefono;
        
            return cuentaCorriente;
        }

        internal CuentaCorrienteViewModel(CuentaCorriente cuentaCorriente, List<Provincia> provincias, List<Localidad> localidades, List<FormaPago> formaPagos) :this (provincias, localidades)
        {
            this.ID = cuentaCorriente.ID;
            this.Calle = cuentaCorriente.Domicilio.Calle;
            this.CalleNumero = cuentaCorriente.Domicilio.Numero;
            this.CallePiso = cuentaCorriente.Domicilio.Piso;
            this.CUIL = cuentaCorriente.CUIL;
            this.Email = cuentaCorriente.Email;
            this.RazonSocial = cuentaCorriente.RazonSocial;
            this.SelectLocalidad = cuentaCorriente.Domicilio.Localidad.ID;
            this.SelectProvincia = cuentaCorriente.Domicilio.Provincia.ID;
            this.Telefono = cuentaCorriente.Telefono;

            if (cuentaCorriente.Comisiones == null)
                cuentaCorriente.Comisiones = new List<Comision>();

            List<Comision> comisionesDebe = CuentaCorrienteHelper.getDeudas(cuentaCorriente.Comisiones);
            List<Comision> comisionesPagas = CuentaCorrienteHelper.getPagos(cuentaCorriente.Comisiones);

            this.TotalDeuda = CuentaCorrienteHelper.getTotalDeuda(comisionesDebe);
            this.ComisionesDebe = comisionesDebe.ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
            this.ComisionesPagas = comisionesPagas.ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
            this.FormaPago = formaPagos.Select(x => new KeyValuePair<int, string>(x.ID, x.Descripcion)).ToList();
        }
    }
}