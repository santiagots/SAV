using NPOI.SS.UserModel;
using SAV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAV.Common
{
    public class ComisionHelper
    {
        public static List<Comision> searchComisiones(List<Comision> comisiones, string ID, string Contacto, string Servicio, string Accion, DateTime? FechaAlta, DateTime? FechaEnvio, DateTime? FechaEntrega, DateTime? FechaPago, string Costo, int? IdResponsable, int? IdCuentaCorriente)
        {
            if (!String.IsNullOrEmpty(ID))
                comisiones = comisiones.Where(x => x.ID == int.Parse(ID)).ToList<Comision>();

            if (!String.IsNullOrEmpty(Contacto))
                comisiones = comisiones.Where(x => x.Contacto.ToUpper().Contains(Contacto.ToUpper())).ToList<Comision>();

            if (!String.IsNullOrEmpty(Servicio))
                comisiones = comisiones.Where(x => x.Servicio.ToString() == Servicio).ToList<Comision>();

            if (!String.IsNullOrEmpty(Accion))
                comisiones = comisiones.Where(x => x.Accion.ToString() == Accion).ToList<Comision>();

            if (FechaAlta.HasValue)
                comisiones = comisiones.Where(x => x.FechaAlta.Date == FechaAlta.Value.Date).ToList<Comision>();

            if (FechaEnvio.HasValue)
                comisiones = comisiones.Where(x => x.FechaEnvio.HasValue && x.FechaEnvio.Value.Date == FechaEnvio.Value.Date).ToList<Comision>();

            if (FechaEntrega.HasValue)
                comisiones = comisiones.Where(x => x.FechaEntrega.HasValue && x.FechaEntrega.Value.Date == FechaEntrega.Value.Date).ToList<Comision>();

            if (FechaPago.HasValue)
                comisiones = comisiones.Where(x => x.FechaPago.HasValue && x.FechaPago.Value.Date == FechaPago.Value.Date).ToList<Comision>();

            if (!String.IsNullOrEmpty(Costo))
                comisiones = comisiones.Where(x => x.Costo == Decimal.Parse(Costo)).ToList<Comision>();

            if (IdResponsable.HasValue)
                comisiones = comisiones.Where(x => x.Responsable != null && x.Responsable.ID == IdResponsable.Value).ToList<Comision>();

            if (IdCuentaCorriente.HasValue)
                comisiones = comisiones.Where(x => x.CuentaCorriente != null && x.CuentaCorriente.ID == IdCuentaCorriente.Value).ToList<Comision>();

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

        public static List<ComisionGasto> searchComisionGasto(List<ComisionGasto> comisionGasto, string Descripcion, DateTime? FechaAlta, decimal Monto)
        {
            if (!String.IsNullOrEmpty(Descripcion))
                comisionGasto = comisionGasto.Where(x => x.Descripcion.Contains(Descripcion)).ToList<ComisionGasto>();

            if (FechaAlta.HasValue)
                comisionGasto = comisionGasto.Where(x => x.FechaAlta.Date == FechaAlta.Value.Date).ToList<ComisionGasto>();

            if (Monto > 0)
                comisionGasto = comisionGasto.Where(x => x.Monto == Monto).ToList<ComisionGasto>();

            return comisionGasto;
        }

        public static List<Comision> getEnvios(List<Comision> comisiones)
        {
            return comisiones.Where(x => !x.FechaEnvio.HasValue).OrderBy(x => x.FechaAlta).ToList();
        }

        public static List<Comision> getPendientes(List<Comision> comisiones)
        {
            return comisiones.Where(x => x.FechaEnvio.HasValue && (!x.Pago || x.Debe > 0 || !x.FechaEntrega.HasValue)).OrderBy(x => x.FechaAlta).ToList();
        }

        public static List<Comision> getFinalizadas(List<Comision> comisiones)
        {
            return comisiones.Where(x => x.FechaEnvio.HasValue && x.Pago && x.Debe == 0 && x.FechaEntrega.HasValue).OrderBy(x => x.FechaAlta).ToList();
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
    }
}