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

        public virtual Domicilio DomicilioEntregar { get; set; }

        public virtual Domicilio DomicilioRetirar { get; set; }

        public virtual Parada Retirar { get; set; }

        public virtual Parada Entregar { get; set; }

        public string Telefono { get; set; }

        public decimal Costo { get; set; }

        public string Comentario { get; set; }

        public virtual ComisionResponsable Responsable { get; set; }

        public bool Pago { get; set; }

        public ComisionAccion Accion { get; set; }

        public ComisionServicio Servicio { get; set; }

        public bool ParaEnviar { get; set; }

        public bool Enviado { get; set; }

        public DateTime FechaAlta { get; set; }

        public DateTime FechaEnvio { get; set; }

        public DateTime? FechaEntrega { get; set; }

        public DateTime? FechaPago { get; set; }

        public decimal Debe { get; set; }

        public virtual CuentaCorriente CuentaCorriente { get; set; }
}
}