using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class Comision
    {
        public int ID { get; set; }

        public string Contacto { get; set; }

        public virtual List<ComisionViaje> ComisionViaje { get; set; }

        public virtual Domicilio DomicilioEntregar { get; set; }

        public virtual Domicilio DomicilioRetirar { get; set; }

        public string Telefono { get; set; }

        public decimal Costo { get; set; }

        public string Comentario { get; set; }

        public virtual ComisionResponsable Responsable { get; set; }

        public bool Pago { get; set; }

        public ComisionAccion Accion { get; set; }

        public ComisionServicio Servicio { get; set; }
    }
}