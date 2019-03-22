using PagedList;
using SAV.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class ClienteViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Nombre:")]
        [Required(ErrorMessage = "Debe ingresar un Nombre")]
        [RegularExpression("^[A-Za-z ÑÁÉÍÓÚñáéíóú]+$", ErrorMessage = "El Nombre solo debe contener letras")]
        public string Nombre { get; set; }

        [Display(Name = "Apellido:")]
        [Required(ErrorMessage = "Debe ingresar un Apellido")]
        [RegularExpression("^[A-Za-z ÑÁÉÍÓÚñáéíóú]+$", ErrorMessage = "El Apellido solo debe contener letras")]
        public string Apellido { get; set; }

        [Display(Name = "DNI:")]
        [Required(ErrorMessage = "Debe ingresar un DNI")]
        [RegularExpression("^(?!^0+$)[a-zA-Z0-9]{6,10}$", ErrorMessage = "El DNI o Pasaporte solo debe contener numeros y letras de entre 6 y 10 caracteres")]
        public string DNI { get; set; }

        [Display(Name = "Edad:")]
        [Required(ErrorMessage = "Debe ingresar una Edad")]
        public Edad? Edad { get; set; }

        [Display(Name = "Sexo:")]
        [Required(ErrorMessage = "Debe ingresar un Sexo")]
        public Sexo? Sexo { get; set; }

        [Display(Name = "Nacionalidad:")]
        [Required(ErrorMessage = "Debe ingresar una Nacionalidad")]
        public string Nacionalidad { get; set; }

        public IPagedList Domicilios { get; set; }

        [Display(Name = "Teléfono:")]
        [Required(ErrorMessage = "Debe ingresar un Teléfono")]
        [RegularExpression("^(\\d{2,5})\\s(\\d{5,10})$", ErrorMessage = "El Teléfono debe ser en formaro CARACTERISTICA[5] NUMERO[10]")]
        public string Telefono { get; set; }

        [Display(Name = "Teléfono Alternativo:")]
        [RegularExpression("^(\\d{2,5})\\s(\\d{5,10})$", ErrorMessage = "El Teléfono alternativo debe ser en formaro CARACTERISTICA[5] NUMERO[10]")]
        public string TelefonoAlternativo { get; set; }

        [Display(Name = "Email:")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "El Email ingresado no tiene un formato valido")]
        public string Email { get; set; }

        public string FechaSalida { get; set; }

        [Display(Name = "Vendedor Alta:")]
        public string VendedorAlta { get; set; }

        [Display(Name = "Vendedor Cobro:")]
        public string VendedorCobro { get; set; }

        [Display(Name = "Origen:")]
        public string Origen { get; set; }

        [Display(Name = "Destino:")]
        public string Destino { get; set; }

        [Display(Name = "Domicilio Ascenso:")]
        public List<KeyValuePair<int, string>> DomicilioAscenso { get; set; }

        [Required(ErrorMessage = "Debe ingresar un Domicilio Ascenso")]
        public int SelectDomicilioAscenso { get; set; }

        [Display(Name = "Domicilio Descenso:")]
        public List<KeyValuePair<int, string>> DomicilioDescenso { get; set; }

        [Required(ErrorMessage = "Debe ingresar un Domicilio Descenso")]
        public int SelectDomicilioDescenso { get; set; }

        [Display(Name = "Parada Ascenso:")]
        public List<KeyValuePair<int, string>> ParadaAscenso { get; set; }

        [Required(ErrorMessage = "Debe ingresar una Parada Ascenso")]
        public int SelectParadaAscenso { get; set; }

        [Display(Name = "Parada Descenso:")]
        public List<KeyValuePair<int, string>> ParadaDescenso { get; set; }

        [Required(ErrorMessage = "Debe ingresar una Parada Descenso")]
        public int SelectParadaDescenso { get; set; }

        [Display(Name = "Pagó:")]
        public bool Pago { get; set; }

        [Display(Name = "Estudiante:")]
        public bool Estudiante { get; set; }

        public IPagedList Viajes { get; set; }

        public IList<ClienteViaje> ViajesEnElMismoDia { get; set; }

        [Required(ErrorMessage = "Debe ingresar una Costo")]
        [Display(Name = "Costo del Pasaje:")]
        [RegularExpression("[0-9]?[0-9]?[0-9]?[0-9]?[0-9](\\,[0-9][0-9])", ErrorMessage = "El Costo debe ser un valor numerico entre 0 y 99999 con 2 digitos decimales")]
        public string Costo { get; set; }

        [Display(Name = "Forma de Pago:")]
        public List<KeyValuePair<int, string>> FormaPago { get; set; }

        [Required(ErrorMessage = "Debe ingresar una Forma de Pago")]
        public int SelectFormaPago { get; set; }

        public ClienteViewModel() : base()
        {
            Nacionalidad = "Argentino";
            Domicilios = new List<Domicilio>().ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
            Viajes = new List<ClienteViaje>().ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
        }

        public ClienteViewModel(List<Provincia> provincias, Viaje viaje, List<FormaPago> formaPagos, string vendedor)
        {
            DomicilioAscenso = new List<KeyValuePair<int, string>>();
            DomicilioDescenso = new List<KeyValuePair<int, string>>();

            if (viaje.Servicio != ViajeTipoServicio.Cerrado)
            {
                Origen = viaje.Origen.Nombre;
                Destino = viaje.Destino.Nombre;

                ParadaAscenso = viaje.Origen.Parada.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
                ParadaDescenso = viaje.Destino.Parada.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            }
            else
            {
                Origen = viaje.OrigenCerrado;
                Destino = viaje.DestinoCerrado;

                ParadaAscenso = new List<KeyValuePair<int, string>>();
                ParadaAscenso.Add(new KeyValuePair<int, string>(0, viaje.OrigenCerrado));

                ParadaDescenso = new List<KeyValuePair<int, string>>();
                ParadaAscenso.Add(new KeyValuePair<int, string>(0, viaje.DestinoCerrado));
            }

            FechaSalida = viaje.FechaSalida.ToString("dd/MM/yyyy HH:mm");
            Nacionalidad = "Argentino";
            Domicilios = new List<Domicilio>().ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
            Viajes = new List<ClienteViaje>().ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
            FormaPago = formaPagos.Select(x => new KeyValuePair<int, string>(x.ID, x.Descripcion)).ToList();
            VendedorAlta = vendedor;
        }

        public ClienteViewModel(List<Provincia> provincias, Cliente cliente, List<FormaPago> formaPagos)
        {
            Id = cliente.ID;
            Apellido = cliente.Apellido;
            Nombre = cliente.Nombre;
            Telefono = cliente.Telefono;
            TelefonoAlternativo = cliente.TelefonoAlternativo;
            Email = cliente.Email;
            DNI = cliente.DNI.ToString();
            Estudiante = cliente.Estudiante;
            Sexo = cliente.Sexo;
            Edad = cliente.Edad;
            Nacionalidad = cliente.Nacionalidad;
            FormaPago = formaPagos.Select(x => new KeyValuePair<int, string>(x.ID, x.Descripcion)).ToList();
            Domicilios = cliente.Domicilios.ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            if (cliente.ClienteViaje == null)
                Viajes = new List<ClienteViaje>().ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
            else
                Viajes = cliente.ClienteViaje.Where(x => x.Viaje != null && x.Viaje.Estado == ViajeEstados.Cerrado).ToPagedList<ClienteViaje>(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
        }

        public ClienteViewModel(List<Provincia> provincias, Viaje viaje, Cliente cliente, List<ClienteViaje> viajesDelDia, List<FormaPago> formaPagos, String vendedor) : this(provincias, cliente, formaPagos)
        {
            FechaSalida = viaje.FechaSalida.ToString("dd/MM/yyyy HH:mm");

            DomicilioAscenso = new List<KeyValuePair<int, string>>();
            DomicilioAscenso.AddRange(cliente.Domicilios.Select(x => new KeyValuePair<int, string>(x.ID, x.getDomicilio)).ToList<KeyValuePair<int, string>>());

            DomicilioDescenso = new List<KeyValuePair<int, string>>();
            DomicilioDescenso.AddRange(cliente.Domicilios.Select(x => new KeyValuePair<int, string>(x.ID, x.getDomicilio)).ToList<KeyValuePair<int, string>>());

            this.VendedorAlta = vendedor;

            if (viaje.Servicio == ViajeTipoServicio.Cerrado)
            {
                ParadaAscenso = new List<KeyValuePair<int, string>>() { new KeyValuePair<int, string>(1, viaje.OrigenCerrado) };
                ParadaDescenso = new List<KeyValuePair<int, string>>() { new KeyValuePair<int, string>(1, viaje.DestinoCerrado) };

                Origen = viaje.OrigenCerrado;
                Destino = viaje.DestinoCerrado;
            }
            else
            {
                ParadaAscenso = viaje.Origen.Parada.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
                ParadaDescenso = viaje.Destino.Parada.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();

                Origen = viaje.Origen.Nombre;
                Destino = viaje.Destino.Nombre;
            }

            ViajesEnElMismoDia = viajesDelDia;

            if (viaje.ClienteViaje != null && viaje.ClienteViaje.Count > 0)
            {
                ClienteViaje clienteViaje = viaje.ClienteViaje.Where(x => x.Cliente.ID == cliente.ID).FirstOrDefault();

                if (clienteViaje != null)
                {
                    Pago = clienteViaje.Pago;
                    Costo = clienteViaje.Costo.ToString();
                    SelectFormaPago = clienteViaje.FormaPago != null? clienteViaje.FormaPago.ID : 0;
                    VendedorAlta = clienteViaje.Vendedor;
                    VendedorCobro = clienteViaje.VendedorCobro;
                    SelectDomicilioAscenso = clienteViaje.DomicilioAscenso != null ? clienteViaje.DomicilioAscenso.ID : 0;
                    SelectDomicilioDescenso = clienteViaje.DomicilioDescenso != null ? clienteViaje.DomicilioDescenso.ID : 0;

                    if (viaje.Servicio == ViajeTipoServicio.Cerrado)
                    {
                        SelectParadaAscenso = SelectDomicilioAscenso == 0 ? 1 : 0;
                        SelectParadaDescenso = SelectDomicilioDescenso == 0 ? 1 : 0;
                    }
                    else
                    {
                        SelectParadaAscenso = clienteViaje.Ascenso != null ? clienteViaje.Ascenso.ID : 0;
                        SelectParadaDescenso = clienteViaje.Descenso != null ? clienteViaje.Descenso.ID : 0;
                    }
                }
            }
        }

        public ClienteViaje getClienteViaje(ClienteViewModel clienteViewModel, Viaje viaje, List<Parada> Paradas, Cliente cliente, List<FormaPago> formaPagos, String Vendedor, Configuracion Configuracion, int? NumeroAsiento)
        {
            ClienteViaje clienteViaje = new ClienteViaje();

            if (Configuracion.UtilizarNumeroPasajes)
            {
                if (NumeroAsiento.HasValue)
                {
                    clienteViaje.NumeroAsiento = NumeroAsiento.Value;
                }
                else
                {
                    for (int i = 1; i <= viaje.Asientos; i++)
                    {
                        if (!viaje.ClienteViaje.Any(x => x.NumeroAsiento == i))
                        {
                            clienteViaje.NumeroAsiento = i;
                            break;
                        }
                    }
                }
            }
            
            clienteViaje.Cliente = cliente;
            clienteViaje.Viaje = viaje;
            clienteViaje.Pago = clienteViewModel.Pago;
            if (clienteViewModel.Pago)
            {
                clienteViaje.FechaPago = DateTime.Now;
                clienteViaje.VendedorCobro = Vendedor;
            }
            else
                clienteViaje.FechaPago = null;

            clienteViaje.Costo = Convert.ToDecimal(clienteViewModel.Costo);
            clienteViaje.FormaPago = formaPagos.FirstOrDefault(x => x.ID == clienteViewModel.SelectFormaPago);
            clienteViaje.Vendedor = Vendedor;
            clienteViaje.DomicilioAscenso = cliente.Domicilios.FirstOrDefault(x => x.ID == clienteViewModel.SelectDomicilioAscenso);
            clienteViaje.DomicilioDescenso = cliente.Domicilios.FirstOrDefault(x => x.ID == clienteViewModel.SelectDomicilioDescenso);
            clienteViaje.Ascenso = Paradas.FirstOrDefault(x => x.ID == clienteViewModel.SelectParadaAscenso);
            clienteViaje.Descenso = Paradas.FirstOrDefault(x => x.ID == clienteViewModel.SelectParadaDescenso);
            clienteViaje.Registro = new List<RegistroViaje>();

            return clienteViaje;
        }
    }
}