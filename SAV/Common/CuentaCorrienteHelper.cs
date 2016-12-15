using SAV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAV.Common
{
    public class CuentaCorrienteHelper
    {
        public static List<Comision> searchComisiones(List<Comision> comisiones, int? numero, DateTime? fechaAlta, DateTime? fechaEntrega, DateTime? fechaPago)
        {
            if (numero.HasValue)
                comisiones = comisiones.Where(x => x.ID == numero.Value).ToList();

            if (fechaAlta.HasValue)
                comisiones = comisiones.Where(x => x.FechaAlta.Date == fechaAlta.Value.Date).ToList();

            if (fechaEntrega.HasValue)
                comisiones = comisiones.Where(x => x.FechaEntrega.HasValue ? x.FechaEntrega.Value.Date == fechaEntrega.Value.Date : false).ToList();

            if (fechaPago.HasValue)
                comisiones = comisiones.Where(x => x.FechaPago.HasValue? x.FechaPago.Value.Date == fechaPago.Value.Date : false).ToList();

            return comisiones;
        }

        public static DateTime? getFecha(string fecha)
        {
            if (String.IsNullOrEmpty(fecha))
                return null;

            string fechaSalida = string.Format("{0}", fecha);
            return DateTime.ParseExact(fechaSalida, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
        }

        public static List<Comision> getDeudas(List<Comision> comisiones)
        {
            return comisiones.Where(x => !x.Pago || x.Debe > 0).OrderBy(x => x.FechaAlta).ToList();
        }

        public static List<Comision> getPagos(List<Comision> comisiones)
        {
            return comisiones.Where(x => x.Pago && x.Debe == 0).OrderBy(x => x.FechaAlta).ToList();
        }

        public static decimal getTotalDeuda(List<Comision> comisiones)
        {
            return comisiones.Sum(x => x.Debe > 0 ? x.Debe : x.Costo);
        }
        
    }
}