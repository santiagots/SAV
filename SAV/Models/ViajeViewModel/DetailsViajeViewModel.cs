﻿using System;
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

        public Gasto  NewGastos { get; set; }

        public ViajeViewModel DatosBasicosViaje { get; set; }

        public DetailsViajeViewModel(): base(){}

        public DetailsViajeViewModel(Viaje viaje, List<Conductor> conductores):this(viaje)
        {
            if(viaje.Conductor != null)
                DatosBasicosViaje.SelectConductorNombre = viaje.Conductor.ID.ToString();

            DatosBasicosViaje.ConductorNombre = conductores.Select(x => new KeyValuePair<int, string>(x.ID, string.Format("{0} , {1}", x.Apellido, x.Nombre))).ToList();
        }

        public DetailsViajeViewModel(Viaje viaje, List<Conductor> conductores, List<Localidad> localidad)
            : this(viaje, conductores)
        {
            if (viaje.Servicio != ViajeTipoServicio.Cerrado)
            {
                DatosBasicosViaje.SelectOrigen = viaje.Origen.ID.ToString();
                DatosBasicosViaje.SelectDestino = viaje.Destino.ID.ToString();
            }
            else
            {
                DatosBasicosViaje.OrigenCerrado = viaje.OrigenCerrado;
                DatosBasicosViaje.DestinoCerrado = viaje.DestinoCerrado;
            }

            DatosBasicosViaje.Destino = localidad.Where(x => x.Parada != null && x.Parada.Count > 0).Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            DatosBasicosViaje.Origen = localidad.Where(x => x.Parada != null && x.Parada.Count > 0).Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
        }

         public DetailsViajeViewModel(Viaje viaje)
        {
            DatosBasicosViaje = new ViajeViewModel();

            DatosBasicosViaje.viajeID = viaje.ID;

            Pasajero = ViajeHelper.getPasajeros(viaje.ClienteViaje, 1);

            Gastos = viaje.Gastos.ToPagedList<Gasto>(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
            DatosBasicosViaje.Asientos = viaje.Asientos;
            DatosBasicosViaje.FechaSalida = viaje.FechaSalida.Date.ToString("dd/MM/yyyy");
            DatosBasicosViaje.HoraSalida = viaje.FechaSalida.ToString("HH:mm");
            DatosBasicosViaje.HoraArribo = viaje.FechaArribo.ToString("HH:mm");
            DatosBasicosViaje.Servicio = viaje.Servicio;
            DatosBasicosViaje.Disponible = viaje.Asientos - viaje.ClienteViaje.Count;
            DatosBasicosViaje.Patente = viaje.Patente;
            DatosBasicosViaje.Interno = viaje.Interno;
        }
    }

    public class Pasajeros
    {
        public int ClienteViajeID { get; set; }
        public int ClienteID { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public long DNI { get; set; }
        public string Domicilio { get; set; }
        public string Ascenso { get; set; }
        public string Descenso { get; set; }
        public string Telefono { get; set; }
        public string Ausencias { get; set; }
        public bool Pago { get; set; }
        public string Costo { get; set; }
        public bool Presente { get; set; }
    }
}