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
    public class ClienteViewModel
    {
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
        [RegularExpression("^(?!\\.?$)\\d{0,8}?$", ErrorMessage = "El DNI solo debe contener numeros sin separado de miles")]
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

        #region domicilio principal
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

        [RegularExpression("^[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.]*[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.][A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.]*$", ErrorMessage = "La Calle solo debe contener número y letras")]
        public string CallePrincipal { get; set; }

        [Display(Name = "Numero:")]
        [Required(ErrorMessage = "Debe ingresar un Numero al Domicilio Principal")]
        [RegularExpression("^\\d{1,5}$", ErrorMessage = "El Numero debe ser un numero del 1 al 99999")]
        public string CalleNumeroPrincipal { get; set; }

        [Display(Name = "Piso:")]
        [RegularExpression("^[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.]*[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.][A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.]*$", ErrorMessage = "El Piso solo debe contener número y letras")]
        public string CallePisoPrincipal { get; set; }
        #endregion

        #region domicilio otros
        [Display(Name = "Provincia:")]
        public List<KeyValuePair<int, string>> ProvinciaOtros { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una Provincia al Domicilio Otros")]
        public int SelectProvinciaOtros { get; set; }

        [Display(Name = "Localidad:")]
        public List<KeyValuePair<int, string>> LocalidadOtros { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una Localidad al Domicilio Otros")]
        public int SelectLocalidadOtros { get; set; }

        [Display(Name = "Calle:")]
        [Required(ErrorMessage = "Debe ingresar una Calle al Domicilio Otros")]
        [RegularExpression("^[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.]*[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.][A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.]*$", ErrorMessage = "La Calle solo debe contener número y letras")]
        public string CalleOtros { get; set; }

        [Display(Name = "Numero:")]
        [Required(ErrorMessage = "Debe ingresar un Numero al Domicilio Otros")]
        [RegularExpression("^\\d{1,5}$", ErrorMessage = "El Numero debe ser un numero del 1 al 99999")]
        public string CalleNumeroOtros { get; set; }

        [Display(Name = "Piso:")]
        [RegularExpression("^[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.]*[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.][A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.]*$", ErrorMessage = "El Piso solo debe contener número y letras")]
        public string CallePisoOtros { get; set; }
        #endregion

        #region domicilio secundario
        [Display(Name = "Provincia:")]
        public List<KeyValuePair<int, string>> ProvinciaSecundaria { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una Provincia al Domicilio Secundario")]
        public int SelectProvinciaSecundaria { get; set; }

        [Display(Name = "Localidad:")]
        public List<KeyValuePair<int, string>> LocalidadSecundaria { get; set; }

        [Required(ErrorMessage = "Debe seleccionar una Localidad al Domicilio Secundario")]
        public int SelectLocalidadSecundaria { get; set; }

        [Display(Name = "Calle:")]
        [Required(ErrorMessage = "Debe ingresar una Calle al Domicilio Secundario")]
        [RegularExpression("^[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.]*[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.][A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.]*$", ErrorMessage = "La Calle solo debe contener número y letras")]
        public string CalleSecundaria { get; set; }

        [Display(Name = "Numero:")]
        [Required(ErrorMessage = "Debe ingresar un Numero al Domicilio Secundario")]
        [RegularExpression("^\\d{1,5}$", ErrorMessage = "El Numero debe ser un numero del 1 al 99999")]
        public string CalleNumeroSecundaria { get; set; }

        [Display(Name = "Piso:")]
        [RegularExpression("^[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.]*[A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.][A-Za-z0-9 ÑÁÉÍÓÚñáéíóú.]*$", ErrorMessage = "El Piso solo debe contener número y letras")]
        public string CallePisoSecundaria { get; set; }
        #endregion

        [Display(Name = "Teléfono:")]
        [Required(ErrorMessage = "Debe ingresar un Teléfono")]
        [RegularExpression("^(\\d{2,5})\\s(\\d{5,10})$", ErrorMessage = "El Teléfono debe ser en formaro CARACTERISTICA[5] NUMERO[10]")]
        public string Telefono { get; set; }
    
        [Display(Name = "Email:")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "El Email ingresado no tiene un formato valido")]
        public string Email { get; set; }

        public string FechaSalida { get; set; }

        [Display(Name = "Vendedor:")]
        public string Vendedor { get; set; }

        [Display(Name = "Origen:")]
        public string Origen { get; set; }

        [Display(Name = "Destino:")]
        public string Destino { get; set; }

        [Display(Name = "Ascenso:")]
        public List<KeyValuePair<int, string>> ServicioPuertaAscenso { get; set; }

        [Required(ErrorMessage = "Debe ingresar un Ascenso")]
        public int SelectServicioPuertaAscenso { get; set; }

        [Display(Name = "Descenso:")]
        public List<KeyValuePair<int, string>> ServicioPuertaDescenso { get; set; }

        [Required(ErrorMessage = "Debe ingresar un Descenso")]
        public int SelectServicioPuertaDescenso { get; set; }

        [Display(Name = "Ascenso:")]
        public List<KeyValuePair<int, string>> ServicioDirectoAscenso { get; set; }

        [Required(ErrorMessage = "Debe ingresar un Ascenso")]
        public int SelectServicioDirectoAscenso { get; set; }

        [Display(Name = "Descenso:")]
        public List<KeyValuePair<int, string>> ServicioDirectoDescenso { get; set; }
        
        [Required(ErrorMessage = "Debe ingresar un Descenso")]
        public int SelectServicioDirectoDescenso { get; set; }

        [Display(Name = "Pagó:")]
        public bool Pago { get; set; }

        [Display(Name = "Estudiante:")]
        public bool Estudiante { get; set; }

        public IPagedList Viajes { get; set; }

        [Display(Name = "Ascenso Domicilio Principal:")]
        public bool AscensoDomicilioPrincipal { get; set; }

        [Display(Name = "Descenso Domicilio Principal:")]
        public bool DescensoDomicilioPrincipal { get; set; }

        [Display(Name = "Descenso Domicilio Otros:")]
        public bool DescensoDomicilioOtros { get; set; }

        [Required(ErrorMessage = "Debe ingresar una Costo")]
        [Display(Name = "Costo del Pasaje:")]
        [RegularExpression("[0-9]?[0-9]?[0-9](\\,[0-9][0-9])", ErrorMessage = "El Costo debe ser un valor numerico entre 0 y 999 con 2 digitos decimales")]
        public string Costo { get; set; }

        public ClienteViewModel():base()
        {}

        public ClienteViewModel( List<Provincia> provincias, Viaje viaje): this(provincias)
        {
            if (viaje.Servicio != ViajeTipoServicio.Cerrado)
            {
                Origen = viaje.Origen.Nombre;
                Destino = viaje.Destino.Nombre;

                ServicioPuertaAscenso = new List<KeyValuePair<int, string>>();
                ServicioPuertaAscenso.Add(new KeyValuePair<int, string>(1, "Domicilio Principal"));
                ServicioPuertaAscenso.Add(new KeyValuePair<int, string>(2, "Domicilio Secunadrio"));

                ServicioPuertaDescenso = new List<KeyValuePair<int, string>>();
                ServicioPuertaDescenso.Add(new KeyValuePair<int, string>(1, "Domicilio Principal"));
                ServicioPuertaDescenso.Add(new KeyValuePair<int, string>(2, "Domicilio Secunadrio"));

                ServicioDirectoAscenso = viaje.Origen.Parada.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
                ServicioDirectoDescenso = viaje.Destino.Parada.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            }
            else
            {
                Origen = viaje.OrigenCerrado;
                Destino = viaje.DestinoCerrado;
            }

            FechaSalida = viaje.FechaSalida.ToString("dd/MM/yyyy HH:mm");
        }

        public ClienteViewModel(List<Provincia> provincias)
        {
            ProvinciaPrincipal = provincias.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            ProvinciaSecundaria = provincias.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            ProvinciaOtros = provincias.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();

            LocalidadPrincipal = new List<KeyValuePair<int, string>>();
            LocalidadSecundaria = new List<KeyValuePair<int, string>>();
            LocalidadOtros = new List<KeyValuePair<int, string>>();
            Viajes = new List<ClienteViaje>().ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            Nacionalidad = "Argentino";
        }

        public ClienteViewModel(List<Provincia> provincias, Cliente cliente)
        {
            Apellido = cliente.Apellido;
            Nombre = cliente.Nombre;
            Telefono = cliente.Telefono;
            Email = cliente.Email;
            DNI = cliente.DNI.ToString();
            Estudiante = cliente.Estudiante;
            Sexo = cliente.Sexo;
            Edad = cliente.Edad;
            Nacionalidad = cliente.Nacionalidad;

            if (cliente.ClienteViaje == null)
                Viajes = new List<ClienteViaje>().ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
            else
                Viajes = cliente.ClienteViaje.Where(x => x.Viaje != null && x.Viaje.Estado == ViajeEstados.Cerrado).ToPagedList<ClienteViaje>(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            ProvinciaPrincipal = provincias.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();

            if (cliente.DomicilioPrincipal != null)
            {
                if (cliente.DomicilioPrincipal.Provincia != null)
                {
                    SelectProvinciaPrincipal = cliente.DomicilioPrincipal.Provincia.ID;

                    LocalidadPrincipal = provincias.Where(x => x.ID == cliente.DomicilioPrincipal.Provincia.ID).FirstOrDefault().Localidad.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList<KeyValuePair<int, string>>();

                    if(cliente.DomicilioPrincipal.Localidad != null)
                        SelectLocalidadPrincipal = cliente.DomicilioPrincipal.Localidad.ID;
                }
                else
                {
                    LocalidadPrincipal = new List<KeyValuePair<int, string>>();
                }

                CallePrincipal = cliente.DomicilioPrincipal.Calle;
                CalleNumeroPrincipal = cliente.DomicilioPrincipal.Numero;
                CallePisoPrincipal = cliente.DomicilioPrincipal.Piso;
            }
            else
                LocalidadPrincipal = new List<KeyValuePair<int, string>>();

            ProvinciaSecundaria = provincias.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();

            if (cliente.DomicilioSecundario != null && cliente.DomicilioSecundario.Provincia != null)
            {
                SelectProvinciaSecundaria = cliente.DomicilioSecundario.Provincia.ID;

                LocalidadSecundaria = provincias.Where(x => x.ID == cliente.DomicilioSecundario.Provincia.ID).FirstOrDefault().Localidad.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList<KeyValuePair<int, string>>();
                SelectLocalidadSecundaria = cliente.DomicilioSecundario.Localidad != null ? cliente.DomicilioSecundario.Localidad.ID : 0;

                CalleSecundaria = cliente.DomicilioSecundario.Calle != null ? cliente.DomicilioSecundario.Calle : string.Empty;
                CalleNumeroSecundaria = cliente.DomicilioSecundario.Numero != null ? cliente.DomicilioSecundario.Numero : string.Empty;
                CallePisoSecundaria = cliente.DomicilioSecundario.Piso != null ? cliente.DomicilioSecundario.Piso : string.Empty;
            }
            else
                LocalidadSecundaria = new List<KeyValuePair<int, string>>();

            ProvinciaOtros = provincias.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            LocalidadOtros = new List<KeyValuePair<int, string>>();
        }

        public ClienteViewModel(List<Provincia> provincias, Viaje viaje, Cliente cliente, String vendedor):this(provincias, cliente)
        {
            FechaSalida = viaje.FechaSalida.ToString("dd/MM/yyyy HH:mm");

            ServicioPuertaAscenso = new List<KeyValuePair<int, string>>();
            ServicioPuertaAscenso.Add(new KeyValuePair<int, string>(1, "Domicilio Principal"));
            ServicioPuertaAscenso.Add(new KeyValuePair<int, string>(2, "Domicilio Secunadrio"));

            ServicioPuertaDescenso = new List<KeyValuePair<int, string>>();
            ServicioPuertaDescenso.Add(new KeyValuePair<int, string>(1, "Domicilio Principal"));
            ServicioPuertaDescenso.Add(new KeyValuePair<int, string>(2, "Domicilio Secunadrio"));

            this.Vendedor = vendedor;

            if (viaje.Servicio == ViajeTipoServicio.Cerrado)
            {
                Origen = viaje.OrigenCerrado;
                Destino = viaje.DestinoCerrado;
            }
            else
            {
                ServicioDirectoAscenso = viaje.Origen.Parada.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
                ServicioDirectoDescenso = viaje.Destino.Parada.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();

                Origen = viaje.Origen.Nombre;
                Destino = viaje.Destino.Nombre;
            }

            if (viaje.ClienteViaje != null && viaje.ClienteViaje.Count > 0)
            {
                ClienteViaje clienteViaje = viaje.ClienteViaje.Where(x => x.Cliente.ID == cliente.ID).FirstOrDefault();

                if (clienteViaje != null)
                {
                    Pago = clienteViaje.Pago;
                    Costo = clienteViaje.Costo.ToString();
                    AscensoDomicilioPrincipal = clienteViaje.AscensoDomicilioPrincipal;
                    DescensoDomicilioPrincipal = clienteViaje.DescensoDomicilioPrincipal;
                    DescensoDomicilioOtros = clienteViaje.DescensoDomicilioOtros;
                    Vendedor = clienteViaje.Vendedor;

                    if (DescensoDomicilioOtros)
                    {
                        SelectProvinciaOtros = clienteViaje.DomicilioDescenso.Provincia.ID;

                        LocalidadOtros = provincias.Where(x => x.ID == clienteViaje.DomicilioDescenso.Provincia.ID).FirstOrDefault().Localidad.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList<KeyValuePair<int, string>>();
                        SelectLocalidadOtros = clienteViaje.DomicilioDescenso.Localidad.ID;

                        CalleOtros = clienteViaje.DomicilioDescenso.Calle;
                        CalleNumeroOtros = clienteViaje.DomicilioDescenso.Numero;
                        CallePisoOtros = clienteViaje.DomicilioDescenso.Piso;
                    }

                    if (viaje.Servicio == ViajeTipoServicio.Puerta)
                    {
                        if (cliente.DomicilioPrincipal.ID == clienteViaje.DomicilioAscenso.ID)
                        {
                            SelectServicioPuertaAscenso = 1;
                            SelectServicioPuertaDescenso = 2;
                        }
                        else
                        {
                            SelectServicioPuertaAscenso = 2;
                            SelectServicioPuertaDescenso = 1;
                        }
                    }
                    if (viaje.Servicio == ViajeTipoServicio.Directo)
                    {
                        Origen = viaje.Origen.Nombre;
                        Destino = viaje.Destino.Nombre;

                        SelectServicioDirectoAscenso = clienteViaje.Ascenso != null ? clienteViaje.Ascenso.ID : 0;
                        SelectServicioDirectoDescenso = clienteViaje.Descenso != null ? clienteViaje.Descenso.ID : 0;
                    }
                }
            }
        }

        public ClienteViaje getClienteViaje(ClienteViewModel clienteViewModel, Viaje viaje, List<Parada> Paradas, Cliente cliente, List<Localidad> localidades, List<Provincia> provincias, String Vendedor)
        {
            ClienteViaje clienteViaje = new ClienteViaje();

            clienteViaje.Cliente = cliente;
            clienteViaje.Viaje = viaje;
            clienteViaje.Pago = clienteViewModel.Pago;
            if (clienteViewModel.Pago)
                clienteViaje.FechaPago = DateTime.Now;
            else
                clienteViaje.FechaPago = null;
            clienteViaje.AscensoDomicilioPrincipal = clienteViewModel.AscensoDomicilioPrincipal;
            clienteViaje.DescensoDomicilioPrincipal = clienteViewModel.DescensoDomicilioPrincipal;
            clienteViaje.DescensoDomicilioOtros = clienteViewModel.DescensoDomicilioOtros;
            clienteViaje.Costo = Convert.ToDecimal(clienteViewModel.Costo);
            clienteViaje.Vendedor = Vendedor;

            if (viaje.Servicio == ViajeTipoServicio.Puerta)
            {
                clienteViaje.DomicilioAscenso = clienteViewModel.SelectServicioPuertaAscenso == 1 ? cliente.DomicilioPrincipal : cliente.DomicilioSecundario;
                clienteViaje.DomicilioDescenso = clienteViewModel.SelectServicioPuertaDescenso == 1 ? cliente.DomicilioPrincipal : cliente.DomicilioSecundario;
            }
            if (viaje.Servicio == ViajeTipoServicio.Directo)
            {
                if (clienteViewModel.AscensoDomicilioPrincipal)
                {
                    clienteViaje.DomicilioAscenso = cliente.DomicilioPrincipal;
                    clienteViaje.Ascenso = null;
                }
                else
                {
                    clienteViaje.Ascenso = Paradas.Where(x => x.ID == clienteViewModel.SelectServicioDirectoAscenso).FirstOrDefault();
                    clienteViaje.DomicilioAscenso = null;
                }

                if (clienteViewModel.DescensoDomicilioPrincipal)
                {
                    clienteViaje.DomicilioDescenso = cliente.DomicilioPrincipal;
                    clienteViaje.Descenso = null;
                }
                else if (clienteViewModel.DescensoDomicilioOtros)
                {
                    clienteViaje.DomicilioDescenso = new Domicilio() {
                        Calle = clienteViewModel.CalleOtros,
                        Localidad = localidades.Where(x => x.ID == clienteViewModel.SelectLocalidadOtros).FirstOrDefault(),
                        Numero = clienteViewModel.CalleNumeroOtros,
                        Piso = clienteViewModel.CallePisoOtros,
                        Provincia = provincias.Where(x => x.ID == clienteViewModel.SelectProvinciaOtros).FirstOrDefault() };

                    clienteViaje.Descenso = null;
                }
                else
                {
                    clienteViaje.Descenso = Paradas.Where(x => x.ID == clienteViewModel.SelectServicioDirectoDescenso).FirstOrDefault();
                    clienteViaje.DomicilioDescenso = null;
                }
            }
            return clienteViaje;
        }

        public Cliente getCliente(ClienteViewModel clienteViewModel, List<Provincia> provincias, List<Localidad> localidades)
        {
            Cliente cliente = new Cliente();

            upDateCliente(clienteViewModel, provincias, localidades, ref cliente);

            return cliente;
        }

        public void upDateCliente(ClienteViewModel clienteViewModel, List<Provincia> provincias, List<Localidad> localidades, ref Cliente cliente)
        {
            cliente.Apellido = clienteViewModel.Apellido.ToUpper();
            cliente.Nombre = clienteViewModel.Nombre.ToUpper();
            cliente.DNI = long.Parse(clienteViewModel.DNI);

            if(cliente.DomicilioPrincipal == null)
                cliente.DomicilioPrincipal = new Domicilio();

            if (clienteViewModel.SelectProvinciaPrincipal > 0)
                cliente.DomicilioPrincipal.Provincia = provincias.Where(x => x.ID == clienteViewModel.SelectProvinciaPrincipal).FirstOrDefault();

            if (clienteViewModel.SelectLocalidadPrincipal > 0)
                cliente.DomicilioPrincipal.Localidad = localidades.Where(x => x.ID == clienteViewModel.SelectLocalidadPrincipal).FirstOrDefault();

            if (!String.IsNullOrEmpty(clienteViewModel.CallePrincipal))
                cliente.DomicilioPrincipal.Calle = clienteViewModel.CallePrincipal.ToUpper(); ;

            if (!String.IsNullOrEmpty(clienteViewModel.CalleNumeroPrincipal))
            cliente.DomicilioPrincipal.Numero = clienteViewModel.CalleNumeroPrincipal;

            if (!String.IsNullOrEmpty(clienteViewModel.CallePisoPrincipal))
            cliente.DomicilioPrincipal.Piso = clienteViewModel.CallePisoPrincipal;

            if(cliente.DomicilioSecundario == null)
                cliente.DomicilioSecundario = new Domicilio();

            if (clienteViewModel.SelectProvinciaSecundaria > 0)
                cliente.DomicilioSecundario.Provincia = provincias.Where(x => x.ID == clienteViewModel.SelectProvinciaSecundaria).FirstOrDefault();

            if (clienteViewModel.SelectLocalidadSecundaria > 0)
                cliente.DomicilioSecundario.Localidad = localidades.Where(x => x.ID == clienteViewModel.SelectLocalidadSecundaria).FirstOrDefault();

            if(!String.IsNullOrEmpty(clienteViewModel.CalleSecundaria))
                cliente.DomicilioSecundario.Calle = clienteViewModel.CalleSecundaria.ToUpper();

            if (!String.IsNullOrEmpty(clienteViewModel.CalleNumeroSecundaria))
            cliente.DomicilioSecundario.Numero = clienteViewModel.CalleNumeroSecundaria;

            if (!String.IsNullOrEmpty(clienteViewModel.CallePisoSecundaria))
            cliente.DomicilioSecundario.Piso = clienteViewModel.CallePisoSecundaria;

            cliente.Telefono = clienteViewModel.Telefono;
            cliente.Email = clienteViewModel.Email;
            cliente.Edad = clienteViewModel.Edad.Value;
            cliente.Sexo = clienteViewModel.Sexo.Value;
            cliente.Nacionalidad = clienteViewModel.Nacionalidad;
            cliente.Estudiante = clienteViewModel.Estudiante;
        }

        public void fillListas(List<Provincia> provincias, Viaje viaje)
        {
            if (viaje.Servicio != ViajeTipoServicio.Cerrado)
            {
                Origen = viaje.Origen.Nombre;
                Destino = viaje.Destino.Nombre;

                ServicioPuertaAscenso = new List<KeyValuePair<int, string>>();
                ServicioPuertaAscenso.Add(new KeyValuePair<int, string>(1, "Domicilio Principal"));
                ServicioPuertaAscenso.Add(new KeyValuePair<int, string>(2, "Domicilio Secunadrio"));

                ServicioPuertaDescenso = new List<KeyValuePair<int, string>>();
                ServicioPuertaDescenso.Add(new KeyValuePair<int, string>(1, "Domicilio Principal"));
                ServicioPuertaDescenso.Add(new KeyValuePair<int, string>(2, "Domicilio Secunadrio"));

                ServicioDirectoAscenso = viaje.Origen.Parada.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
                ServicioDirectoDescenso = viaje.Destino.Parada.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            }

            ProvinciaPrincipal = provincias.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            ProvinciaSecundaria = provincias.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            LocalidadPrincipal = new List<KeyValuePair<int, string>>();
            LocalidadSecundaria = new List<KeyValuePair<int, string>>();

            Viajes = new List<ClienteViaje>().ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
        }
    }
}