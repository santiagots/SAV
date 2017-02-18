using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class CuentaCorriente
    {
        public int ID { get; set; }
        public string RazonSocial { get; set; }

        public string CUIL { get; set; }

        public virtual Domicilio Domicilio { get; set; }
        
        public string Telefono { get; set; }
        
        public string Email { get; set; }

        public virtual List<Comision> Comisiones { get; set; }

        public virtual List<Pago> Pagos { get; set; }

        public decimal Deuda { get { return Comisiones.Where(x => !x.Pago).Sum(y => y.Debe > 0 ? y.Debe : y.Costo); } }
    }
}