using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class ComisionViaje
    {
        public int ID { get; set; }

        public virtual Comision Comision { get; set; }

        public virtual Viaje Viaje { get; set; }

        public virtual Parada Retirar { get; set; }

        public virtual Parada Entregar { get; set; }

        public bool EntregaRetira { get; set; }

        public Decimal Costo { get; set; }

        public bool Pago { get; set; }
    }
}