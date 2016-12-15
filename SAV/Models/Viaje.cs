using SAV.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public enum ViajeEstados { Abierto, Cerrado }
    public enum ViajeTipoServicio { Puerta, Directo, Cerrado }

    public class Viaje
    {
        public int ID { get; set; }
        public virtual List<ClienteViaje> ClienteViaje { get; set; }
        public virtual List<Gasto> Gastos { get; set; }
        public virtual Conductor Conductor { get; set; }
        public virtual Localidad Origen { get; set; }
        public virtual Localidad Destino { get; set; }
        public String OrigenCerrado { get; set; }
        public String DestinoCerrado { get; set; }
        public int Asientos { get; set; }
        public DateTime FechaSalida { get; set; }
        public DateTime FechaArribo { get; set; }
        public virtual ViajeEstados Estado { get; set; }
        public virtual ViajeTipoServicio Servicio { get; set; }
        public virtual string Patente { get; set; }
        public virtual int Interno { get; set; }

        public Viaje(): base()
        { }

        public Viaje(DateTime fechaSalida, DateTime fechaArribo, ViajeViewModel viajeViewModel, List<Conductor> conductores, List<Localidad> localidades)
        {
            Set(fechaSalida, fechaArribo, viajeViewModel, conductores, localidades);
        }

        public List<Viaje> CreateViajes(CreateViajeViewModel createViajeViewModel, List<Conductor> conductores, List<Localidad> localidades)
        {
            List<Viaje> viajes = new List<Viaje>();

            FechaSalida = ViajeHelper.getFecha(createViajeViewModel.DatosBasicosViaje.FechaSalida, createViajeViewModel.DatosBasicosViaje.HoraSalida);
            FechaArribo = ViajeHelper.getFecha(createViajeViewModel.DatosBasicosViaje.FechaSalida, createViajeViewModel.DatosBasicosViaje.HoraArribo);

            if (FechaArribo < FechaSalida)
                FechaArribo = FechaArribo.AddDays(1);

            viajes.Add(new Viaje(FechaSalida, FechaArribo, createViajeViewModel.DatosBasicosViaje, conductores, localidades));

            if (String.IsNullOrEmpty(createViajeViewModel.FechaRepeticionFin))
                return viajes;

            DateTime repeticionHasta = ViajeHelper.getFecha(createViajeViewModel.FechaRepeticionFin, "23:59");

            TimeSpan diff = repeticionHasta - FechaSalida;
            int days = diff.Days;

            for (var i = 1; i <= days; i++)
            {
                var repeticionSalida = FechaSalida.AddDays(i);
                var repeticionArribo = FechaArribo.AddDays(i);

                switch (repeticionSalida.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        if(createViajeViewModel.Lunes)
                            viajes.Add(new Viaje(repeticionSalida, repeticionArribo, createViajeViewModel.DatosBasicosViaje, conductores, localidades));
                        break;
                    case DayOfWeek.Tuesday:
                        if(createViajeViewModel.Martes)
                            viajes.Add(new Viaje(repeticionSalida, repeticionArribo, createViajeViewModel.DatosBasicosViaje, conductores, localidades));
                        break;
                    case DayOfWeek.Wednesday:
                        if(createViajeViewModel.Miercoles)
                            viajes.Add(new Viaje(repeticionSalida, repeticionArribo, createViajeViewModel.DatosBasicosViaje, conductores, localidades));
                        break;
                    case DayOfWeek.Thursday:
                        if (createViajeViewModel.Jueves)
                            viajes.Add(new Viaje(repeticionSalida, repeticionArribo, createViajeViewModel.DatosBasicosViaje, conductores, localidades));
                        break;
                    case DayOfWeek.Friday:
                        if(createViajeViewModel.Viernes)
                            viajes.Add(new Viaje(repeticionSalida, repeticionArribo, createViajeViewModel.DatosBasicosViaje, conductores, localidades));
                        break;
                    case DayOfWeek.Saturday:
                        if (createViajeViewModel.Sabado)
                            viajes.Add(new Viaje(repeticionSalida, repeticionArribo, createViajeViewModel.DatosBasicosViaje, conductores, localidades));
                        break;
                    case DayOfWeek.Sunday:
                        if (createViajeViewModel.Domingo)
                            viajes.Add(new Viaje(repeticionSalida, repeticionArribo, createViajeViewModel.DatosBasicosViaje, conductores, localidades));
                        break;
                }
            }
            return viajes;
        }

        public void UpDate(ViajeViewModel viajeViewModel, List<Conductor> conductores, List<Localidad> localidades)
        {
            List<Viaje> viajes = new List<Viaje>();

            FechaSalida = ViajeHelper.getFecha(viajeViewModel.FechaSalida, viajeViewModel.HoraSalida);
            FechaArribo = ViajeHelper.getFecha(viajeViewModel.FechaSalida, viajeViewModel.HoraArribo);

            if (FechaArribo < FechaSalida)
                FechaArribo = FechaArribo.AddDays(1);

            Set(FechaSalida, FechaArribo, viajeViewModel, conductores, localidades);
        }

        public void Set(DateTime fechaSalida, DateTime fechaArribo, ViajeViewModel viajeViewModel, List<Conductor> conductores, List<Localidad> localidades)
        {
            Conductor = conductores.Where(x => x.ID == int.Parse(viajeViewModel.SelectConductorNombre)).FirstOrDefault();

            if (viajeViewModel.Servicio != ViajeTipoServicio.Cerrado)
            {
                Origen = localidades.Where(x => x.ID == int.Parse(viajeViewModel.SelectOrigen)).FirstOrDefault();
                Destino = localidades.Where(x => x.ID == int.Parse(viajeViewModel.SelectDestino)).FirstOrDefault();
                OrigenCerrado = string.Empty;
                DestinoCerrado = string.Empty;
            }
            else
            {
                Origen = null;
                Destino = null;
                OrigenCerrado = viajeViewModel.OrigenCerrado;
                DestinoCerrado = viajeViewModel.DestinoCerrado;
            }
            Asientos = viajeViewModel.Asientos;
            FechaSalida = fechaSalida;
            FechaArribo = fechaArribo;
            Servicio = viajeViewModel.Servicio;
            Estado = ViajeEstados.Abierto;
            Patente = viajeViewModel.Patente.ToUpper();
            Interno = viajeViewModel.Interno;
        }

        public bool tieneLugar()
        {
            return (Asientos - ClienteViaje.Count) > 0;
        }
    }
}