using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class AdicionalConductor
    {
        public int ID { get; set; }

        public virtual Viaje Viaje { get; set; }

        public virtual Conductor Conductor { get; set; }

        public virtual TipoAdicionalConductor TipoAdicionalConductor { get; set; }

        public decimal Monto { get; set; }

        public string Comentario { get; set; }

        public DateTime FechaAlta { get; set; }

        public string UsuarioAlta { get; set; }
    }
}