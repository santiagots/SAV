using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class Conductor
    {
        public int ID { get; set; }

        public string Nombre { get; set; }

        public string Apellido { get; set; }

        public string CUIL { get; set; }

        public virtual Domicilio Domicilio { get; set; }

        public string Telefono { get; set; }

        public string Email { get; set; }

        public IList<Viaje> Viaje { get; set; }

        public decimal ComisionViaje { get; set; }

        public decimal ComisionViajeCerrado { get; set; }
    }
}