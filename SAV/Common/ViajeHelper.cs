using PagedList;
using SAV.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace SAV.Common
{
    public static class ViajeHelper
    {

        public static DateTime getFecha(string fecha, string hora)
        {
            hora = hora.PadLeft(5, '0');

            string fechaSalida = string.Format("{0} {1}", fecha, hora);

            return DateTime.ParseExact(fechaSalida, "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
        }

        public static DateTime getFecha(string fecha)
        {
            string fechaSalida = string.Format("{0}", fecha);

            return DateTime.ParseExact(fechaSalida, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        }

        public static IQueryable<Viaje> filtrarSerchViajesViewModel(IQueryable<Viaje> viajes, int? IdOrigen, int? IdDestiono, string FechaSalida, string Servicio, int? codigo, string nombrePasajero, string estadoViaje)
        {
            if (IdOrigen.HasValue)
                viajes = viajes.Where(x => x.Origen != null && x.Origen.ID == IdOrigen.Value);

            if (IdDestiono.HasValue)
                viajes = viajes.Where(x => x.Destino != null && x.Destino.ID == IdDestiono.Value);

            if (!string.IsNullOrEmpty(FechaSalida))
            {
                DateTime fecha = ViajeHelper.getFecha(FechaSalida);
                viajes = viajes.Where(x => x.FechaSalida.Day == fecha.Day && x.FechaSalida.Month == fecha.Month && x.FechaSalida.Year == fecha.Year);
            }

            if (!string.IsNullOrEmpty(Servicio))
            {
                ViajeTipoServicio servicio = (ViajeTipoServicio)Enum.Parse(typeof(ViajeTipoServicio), Servicio);
                viajes = viajes.Where(x => x.Servicio == servicio);
            }

            if (codigo.HasValue && codigo.Value > 0)
            {
                viajes = viajes.Where(x => x.ID == codigo.Value);
            }

            if (!string.IsNullOrEmpty(nombrePasajero))
            {
                viajes = viajes.Where(x => x.ClienteViaje.Any(y => y.Cliente.Nombre.ToUpper().Contains(nombrePasajero.ToUpper()) || y.Cliente.Apellido.ToUpper().Contains(nombrePasajero.ToUpper())));
            }

            if (!string.IsNullOrEmpty(estadoViaje))
            {
                ViajeEstados estado = (ViajeEstados)Enum.Parse(typeof(ViajeEstados), estadoViaje);
                viajes = viajes.Where(x => x.Estado == estado);
            }

            return viajes;
        }

        public static List<Pasajeros> getPasajeros(List<ClienteViaje> ClienteViaje)
        {
            return ClienteViaje.Select(x => new Pasajeros()
            {
                ClienteViajeID = x.ID,
                ClienteID = x.Cliente.ID,
                Nombre = x.Cliente.Nombre,
                Apellido = x.Cliente.Apellido,
                DNI = x.Cliente.DNI,
                Telefono = x.Cliente.Telefono,
                TelefonoAlternativo = x.Cliente.TelefonoAlternativo,
                Ascenso = getAscenso(x),
                Descenso = getDescenso(x),
                Ausencias = x.Cliente.getClienteViaje,
                Pago = x.Pago,
                Costo = x.Costo.ToString(),
                Presente = x.Presente,
                Vendedor = x.Vendedor
            }).ToList<Pasajeros>();
        }

        public static IPagedList<Pasajeros> getPasajeros(List<ClienteViaje> ClienteViaje, int pageNumber)
        {
            return getPasajeros(ClienteViaje).ToPagedList<Pasajeros>(pageNumber, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
        }

        private static string getAscenso(ClienteViaje clienteViaje)
        {
            if (clienteViaje.Viaje.Servicio == ViajeTipoServicio.Puerta)
            {
                if (clienteViaje.DomicilioAscenso != null)
                    return clienteViaje.DomicilioAscenso.getDomicilio;
                return clienteViaje.Ascenso?.Nombre;
            }
            if (clienteViaje.Viaje.Servicio == ViajeTipoServicio.Directo)
            {
                if (clienteViaje.DomicilioAscenso != null)
                    return clienteViaje.DomicilioAscenso.getDomicilio;
                return clienteViaje.Ascenso?.Nombre;
            }
            if (clienteViaje.Viaje.Servicio == ViajeTipoServicio.Cerrado)
            {
                if (clienteViaje.DomicilioAscenso != null)
                    return clienteViaje.DomicilioAscenso.getDomicilio;
                return clienteViaje.Viaje.OrigenCerrado;
            }
             
            return string.Empty;
        }

        private static string getDescenso(ClienteViaje clienteViaje)
        {
            if (clienteViaje.Viaje.Servicio == ViajeTipoServicio.Puerta)
            {
                if (clienteViaje.DomicilioDescenso != null)
                    return clienteViaje.DomicilioDescenso.getDomicilio;
                return clienteViaje.Descenso?.Nombre;
            }

            if (clienteViaje.Viaje.Servicio == ViajeTipoServicio.Directo)
            {
                if (clienteViaje.DomicilioDescenso != null)
                    return clienteViaje.DomicilioDescenso.getDomicilio;
                return clienteViaje.Descenso?.Nombre;
            }
            if (clienteViaje.Viaje.Servicio == ViajeTipoServicio.Cerrado)
            {
                if (clienteViaje.DomicilioDescenso != null)
                    return clienteViaje.DomicilioDescenso.getDomicilio;
                return clienteViaje.Viaje.DestinoCerrado;
            }
            return string.Empty;
        }
    }
}