using NPOI.SS.UserModel;
using SAV.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SAV.Common
{
    public class ComisionHelper
    {
        public static IQueryable<Comision> searchComisiones(IQueryable<Comision> comisiones, string ID, string Contacto, string Servicio, string Accion, DateTime? FechaAlta, DateTime? FechaEnvio, DateTime? FechaEntrega, DateTime? FechaPago, string Costo, int? IdResponsable, int? IdCuentaCorriente)
        {
            if (!String.IsNullOrEmpty(ID))
                comisiones = comisiones.Where(x => x.ID == int.Parse(ID));

            if (!String.IsNullOrEmpty(Contacto))
                comisiones = comisiones.Where(x => x.Contacto.ToUpper().Contains(Contacto.ToUpper()));

            if (!String.IsNullOrEmpty(Servicio))
                comisiones = comisiones.Where(x => x.Servicio.ToString() == Servicio);

            if (!String.IsNullOrEmpty(Accion))
                comisiones = comisiones.Where(x => x.Accion.ToString() == Accion);

            if (FechaAlta.HasValue)
                comisiones = comisiones.Where(x => DbFunctions.TruncateTime(x.FechaAlta).Value == FechaAlta.Value);

            if (FechaEnvio.HasValue)
                comisiones = comisiones.Where(x => DbFunctions.TruncateTime(x.FechaEnvio) == FechaEnvio.Value);

            if (FechaEntrega.HasValue)
                comisiones = comisiones.Where(x => x.FechaEntrega.HasValue && DbFunctions.TruncateTime(x.FechaEntrega.Value) == FechaEntrega.Value);

            if (FechaPago.HasValue)
                comisiones = comisiones.Where(x => x.FechaPago.HasValue && DbFunctions.TruncateTime(x.FechaPago.Value) == FechaPago.Value);

            if (!String.IsNullOrEmpty(Costo))
                comisiones = comisiones.Where(x => x.Costo == Decimal.Parse(Costo));

            if (IdResponsable.HasValue)
                comisiones = comisiones.Where(x => x.Responsable != null && x.Responsable.ID == IdResponsable.Value);

            if (IdCuentaCorriente.HasValue)
                comisiones = comisiones.Where(x => x.CuentaCorriente != null && x.CuentaCorriente.ID == IdCuentaCorriente.Value);

            return comisiones;
        }

        public static DateTime? getFecha(string fecha)
        {
            if (String.IsNullOrEmpty(fecha))
                return null;

            string fechaSalida = string.Format("{0}", fecha);
            return DateTime.ParseExact(fechaSalida, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        }

        public static decimal getMonto(string monto)
        {
            if (String.IsNullOrEmpty(monto))
                return 0;

            return decimal.Parse(monto);
        }

        public static IQueryable<Comision> getEnvios(IQueryable<Comision> comisiones)
        {
            return comisiones.Where(x => !x.Enviado).OrderByDescending(x => x.FechaAlta);
        }

        public static IQueryable<Comision> getPendientes(IQueryable<Comision> comisiones)
        {
            return comisiones.Where(x => x.Enviado && (!x.FechaEntrega.HasValue || !x.FechaPago.HasValue)).OrderByDescending(x => x.FechaAlta);
        }

        public static IQueryable<Comision> getFinalizadas(IQueryable<Comision> comisiones)
        {
            return comisiones.Where(x => x.FechaEntrega.HasValue && x.FechaPago.HasValue).OrderByDescending(x => x.FechaAlta);
        }

        public static void SetValueToComisionesCell(ISheet ComisionesSheet, int ComisionesRow, Comision comision, int ComisionIndex)
        {
            List<ICell> comisionCell = ComisionesSheet.GetRow(ComisionesRow).Cells;

            comisionCell[0].SetCellValue(ComisionIndex);

            comisionCell[1].SetCellValue(comision.Contacto);
            comisionCell[2].SetCellValue(comision.Accion.ToString());
            if (comision.Servicio == ComisionServicio.Puerta)
            {
                if(comision.DomicilioRetirar != null)
                    comisionCell[3].SetCellValue(comision.DomicilioRetirar.getDomicilio);
                if(comision.DomicilioEntregar != null)
                    comisionCell[4].SetCellValue(comision.DomicilioEntregar.getDomicilio);
            }
            else
            {
                if (comision.Retirar != null)
                    comisionCell[3].SetCellValue(comision.Retirar.Nombre);
                if (comision.Entregar != null)
                    comisionCell[4].SetCellValue(comision.Entregar.Nombre);
            }
            comisionCell[5].SetCellValue(comision.Telefono);
            comisionCell[6].SetCellValue(comision.Costo.ToString("c"));
            comisionCell[7].SetCellValue(comision.Pago ? "Si" : "No");

            List<ICell> comisionComentarioCell = ComisionesSheet.GetRow(ComisionesRow + 1).Cells;
            comisionComentarioCell[1].SetCellValue(String.Format("Comentario: {0}", comision.Comentario));
        }

        public static IList<Comision> AutoCheck(IQueryable<Comision> Comisiones)
        {
            Comisiones = ComisionHelper.getEnvios(Comisiones);
            Comisiones = Comisiones.Where(x => !x.ParaEnviar);

            DateTime fecha = DateHelper.getLocal();
            
            DateTime proximoLunes = DateHelper.GetNextWeekday(DayOfWeek.Monday, DateTime.Now);
            DateTime proximoSabado = DateHelper.previoustWeekday(DayOfWeek.Saturday, proximoLunes);
            List<Comision> comisionesChecked = new List<Comision>();

            foreach (Comision comision in Comisiones)
            { 
                bool @checked = false;
                //si es viernes y la fecha de entrega es entrega Sabado y Lunes marco la comision para enviar
                if (fecha.DayOfWeek == DayOfWeek.Friday && comision.FechaEnvio.Date >= proximoSabado.Date && comision.FechaEnvio.Date <= proximoLunes.Date)
                {
                    @checked = true;
                    comision.ParaEnviar = true;
                }

                //si no es viernes y la fecha de entrega es mañana marco la comision para enviar
                if (fecha.DayOfWeek != DayOfWeek.Friday && comision.FechaEnvio.AddDays(-1).Date == fecha.Date)
                {
                    @checked = true;
                    comision.ParaEnviar = true;
                }

                //si la fecha es hoy marco la comision para enviar. Esto no deberia pasar!!!
                if (comision.FechaEnvio == fecha.Date)
                {
                    @checked = true;
                    comision.ParaEnviar = true;
                }

                if (@checked)
                    comisionesChecked.Add(comision);
            }

            return comisionesChecked;
        }
    }
}