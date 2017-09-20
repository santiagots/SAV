using PagedList;
using SAV.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

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

        public static List<Viaje> getViajesActivos(List<Viaje> viajes)
        {
            return viajes.Where(x => x.FechaArribo.CompareTo(DateTime.Now) >= 0 && x.Estado == ViajeEstados.Abierto).ToList<Viaje>();
        }

        public static List<Viaje> getViajesFinalizados(List<Viaje> viajes)
        {
            return viajes.Where(x => x.FechaArribo.CompareTo(DateTime.Now.AddHours(4)) < 0 && x.Estado == ViajeEstados.Abierto).ToList<Viaje>();
        }

        public static List<Viaje> filtrarSerchViajesViewModel(List<Viaje> viajes, int? IdOrigen, int? IdDestiono, string FechaSalida, string Servicio, int codigo)
        {
            if (IdOrigen.HasValue)
                viajes = viajes.Where(x => x.Origen != null && x.Origen.ID == IdOrigen.Value).ToList<Viaje>();

            if (IdDestiono.HasValue)
                viajes = viajes.Where(x => x.Destino != null && x.Destino.ID == IdDestiono.Value).ToList<Viaje>();

            if (!string.IsNullOrEmpty(FechaSalida))
            {
                DateTime fecha = ViajeHelper.getFecha(FechaSalida);
                viajes = viajes.Where(x => x.FechaSalida.Day == fecha.Day && x.FechaSalida.Month == fecha.Month && x.FechaSalida.Year == fecha.Year).ToList<Viaje>();
            }

            if (!string.IsNullOrEmpty(Servicio))
            {
                viajes = viajes.Where(x => x.Servicio.ToString() == Servicio).ToList<Viaje>();
            }

            if (codigo > 0)
            {
                viajes = viajes.Where(x => x.ID == codigo).ToList<Viaje>();
            }

            return viajes;
        }

        public static List<Localidad> getDestinos(List<Localidad> localidades)
        {
            return localidades.Where(x => x.Parada != null && x.Parada.Count > 0).ToList<Localidad>();
        }

        //public static List<Comisiones> getComisiones(List<ComisionViaje> ComisionesViaje)
        //{
        //    return ComisionesViaje.Select(x => new Comisiones()
        //                                            {
        //                                                ComisionID = x.Comision.ID,
        //                                                ComisionViajeID = x.ID,
        //                                                Responsable = x.Comision.Responsable != null? x.Comision.Responsable.Apellido + " " + x.Comision.Responsable.Nombre: "No definido",
        //                                                Contacto = x.Comision.Contacto,
        //                                                Telefono = x.Comision.Telefono,
        //                                                Accion = x.Comision.Accion,
        //                                                Servicio = x.Comision.Servicio,
        //                                                EntregarPuerta = x.Comision.DomicilioEntregar != null ? x.Comision.DomicilioEntregar.getDomicilio : string.Empty,
        //                                                RetirarPuerta = x.Comision.DomicilioRetirar != null ? x.Comision.DomicilioRetirar.getDomicilio : string.Empty,
        //                                                EntregarDirecto = x.Entregar != null ? x.Entregar.Nombre : string.Empty,
        //                                                RetirarDirecto = x.Retirar != null ? x.Retirar.Nombre : string.Empty,
        //                                                Costo = x.Comision.Costo.ToString(),
        //                                                Comentaro = x.Comision.Comentario,
        //                                                Pago = x.Pago,
        //                                                EntregaRetira = x.EntregaRetira
        //                                            }).ToList<Comisiones>();
        //}

        //public static IPagedList<Comisiones> getComisiones(List<ComisionViaje> ComisionesViaje, int pageNumber)
        //{
        //    return getComisiones(ComisionesViaje).ToPagedList<Comisiones>(pageNumber, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
        //}

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
                Domicilio = x.Cliente.DomicilioPrincipal.getDomicilio,
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
                return clienteViaje.Ascenso.Nombre;
            }
            if (clienteViaje.Viaje.Servicio == ViajeTipoServicio.Directo)
            {
                if (clienteViaje.DomicilioAscenso != null)
                    return clienteViaje.DomicilioAscenso.getDomicilio;
                return clienteViaje.Ascenso.Nombre;
            }
            if (clienteViaje.Viaje.Servicio == ViajeTipoServicio.Cerrado)
                return clienteViaje.Viaje.OrigenCerrado;

            return string.Empty;
        }

        private static string getDescenso(ClienteViaje clienteViaje)
        {
            if (clienteViaje.Viaje.Servicio == ViajeTipoServicio.Puerta)
            {
                if (clienteViaje.DomicilioDescenso != null)
                    return clienteViaje.DomicilioDescenso.getDomicilio;
                return clienteViaje.Descenso.Nombre;
            }

            if (clienteViaje.Viaje.Servicio == ViajeTipoServicio.Directo)
            {
                if (clienteViaje.DomicilioDescenso != null)
                    return clienteViaje.DomicilioDescenso.getDomicilio;
                return clienteViaje.Descenso.Nombre;
            }
            if (clienteViaje.Viaje.Servicio == ViajeTipoServicio.Cerrado)
                return clienteViaje.Viaje.DestinoCerrado;

            return string.Empty;
        }
    }
}