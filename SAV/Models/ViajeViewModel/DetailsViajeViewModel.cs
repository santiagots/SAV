using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using PagedList;
using SAV.Common;
using System.Configuration;

namespace SAV.Models
{
    public class DetailsViajeViewModel
    {
        public IPagedList<Pasajeros> Pasajero { get; set; }

        public IPagedList<Gasto> Gastos { get; set; }

        public GastoViajeViewModel NewGastos { get; set; }

        public IPagedList<AdicionalConductor> AdicionalesCoductor { get; set; }

        public AdicionalConductorViewModel AdicionalCoductor { get; set; }

        public ViajeViewModel DatosBasicosViaje { get; set; }

        public DetailsViajeViewModel(): base(){}

        public DetailsViajeViewModel(Viaje viaje, List<Conductor> conductores, bool cierre = false) :this(viaje, cierre)
        {
            if(viaje.Conductor != null)
                DatosBasicosViaje.SelectConductorNombre = viaje.Conductor.ID.ToString();

            DatosBasicosViaje.ConductorNombre = conductores.Select(x => new KeyValuePair<int, string>(x.ID, string.Format("{0} , {1}", x.Apellido, x.Nombre))).ToList();
        }

        public DetailsViajeViewModel(Viaje viaje, List<Conductor> conductores, List<Localidad> localidad, List<Provincia> provincia, List<ModalidadPrestacion> modalidadPrestacion, bool cierre = false)
            : this(viaje, conductores, cierre)
        {
            if (viaje.Servicio != ViajeTipoServicio.Cerrado)
            {
                DatosBasicosViaje.SelectOrigen = viaje.Origen.ID.ToString();
                DatosBasicosViaje.SelectDestino = viaje.Destino.ID.ToString();
            }
            else
            {
                DatosBasicosViaje.OrigenCerrado = viaje.OrigenCerrado;
                DatosBasicosViaje.SelectProvinciaOrigenCerrado = viaje.ProvienciaOrigenCerrado.ID;
                DatosBasicosViaje.DestinoCerrado = viaje.DestinoCerrado;
                DatosBasicosViaje.SelectProvinciaDestinoCerrado = viaje.ProvienciaDestinoCerrado.ID;
                DatosBasicosViaje.CostoCerrado = viaje.CostoCerrado.ToString();
            }

            DatosBasicosViaje.Destino = localidad.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            DatosBasicosViaje.Origen = localidad.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            DatosBasicosViaje.ProvinciaDestinoCerrado = provincia.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            DatosBasicosViaje.ProvinciaOrigenCerrado = provincia.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            DatosBasicosViaje.ModalidadPrestacion = modalidadPrestacion.Select(x => new KeyValuePair<string, string>(x.Codigo, x.Descripcion)).ToList();
            DatosBasicosViaje.SelectModalidadPrestacion = viaje.ModalidadPrestacion?.Codigo;
        }

        public DetailsViajeViewModel(Viaje viaje, List<Conductor> conductores, List<Localidad> localidad, List<Provincia> provincia, List<TipoGasto> tipoGasto, List<TipoAdicionalConductor> tipoAdicional, List<ModalidadPrestacion> modalidadPrestacion, bool cierre = false)
            : this(viaje, conductores, localidad, provincia, modalidadPrestacion, cierre)
        {
            NewGastos = new GastoViajeViewModel();
            NewGastos.TipoGasto = tipoGasto.Where(x=>x.Habilitado).Select(y => new KeyValuePair<int, string>(y.ID, y.Descripcion)).ToList();

            AdicionalCoductor = new AdicionalConductorViewModel();
            AdicionalCoductor.TipoAdicional = tipoAdicional.Where(x => x.Habilitado).Select(y => new KeyValuePair<int, string>(y.ID, y.Descripcion)).ToList();
        }

        public DetailsViajeViewModel(Viaje viaje, bool cierre = false)
        {
            DatosBasicosViaje = new ViajeViewModel();

            DatosBasicosViaje.viajeID = viaje.ID;

            if(cierre)
                Pasajero = ViajeHelper.getPasajerosViajeCerrado(viaje.ClienteViaje, viaje.Asientos, 1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
            else
                Pasajero = ViajeHelper.getPasajerosViajeAbierto(viaje.ClienteViaje, viaje.Asientos, 1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));

            Gastos = viaje.Gastos.ToPagedList<Gasto>(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
            AdicionalesCoductor = viaje.AdicionalConductor.ToPagedList<AdicionalConductor>(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
            DatosBasicosViaje.Asientos = viaje.Asientos;
            DatosBasicosViaje.FechaSalida = viaje.FechaSalida.Date.ToString("dd/MM/yyyy");
            DatosBasicosViaje.HoraSalida = viaje.FechaSalida.ToString("HH:mm");
            DatosBasicosViaje.HoraArribo = viaje.FechaArribo.ToString("HH:mm");
            DatosBasicosViaje.Servicio = viaje.Servicio;
            DatosBasicosViaje.Disponible = viaje.Asientos - viaje.ClienteViaje.Count;
            DatosBasicosViaje.Patente = viaje.Patente;
            DatosBasicosViaje.PatenteSuplente = viaje.PatenteSuplente;
            DatosBasicosViaje.Interno = viaje.Interno;
            DatosBasicosViaje.Estado = viaje.Estado;
        }
    }

    public class Pasajeros
    {
        public int NumeroAsiento { get; set; }
        public int ClienteViajeID { get; set; }
        public int ClienteID { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string DNI { get; set; }
        public string Ascenso { get; set; }
        public string Descenso { get; set; }
        public string Telefono { get; set; }
        public string TelefonoAlternativo { get; set; }
        public string Ausencias { get; set; }
        public bool Pago { get; set; }
        public string Costo { get; set; }
        public bool Presente { get; set; }
        public string Vendedor { get; set; }
    }
}