using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class Pago
    {
        public int ID { get; set; }
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
        public virtual FormaPago FormaPago { get; set; }

        public Pago()
        { }

        public Pago(decimal monto, FormaPago formaPago)
        {
            TimeZoneInfo tst = TimeZoneInfo.FindSystemTimeZoneById("Argentina Standard Time");
            Fecha = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, tst);
            Monto = monto;
            FormaPago = formaPago;
        }
    }
}