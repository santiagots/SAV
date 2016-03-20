using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class Cliente
    {
        public int ID { get; set; }
        
        public string Nombre { get; set; }

        public string Apellido { get; set; }

        public long DNI { get; set; }
        
        public virtual Domicilio DomicilioPrincipal { get; set; }

        public virtual Domicilio DomicilioSecundario { get; set; }

        public string Telefono { get; set; }
        
        public string Email { get; set; }

        public bool Estudiante { get; set; }
        
        public virtual List<ClienteViaje> ClienteViaje { get; set; }

        public string getClienteViaje { get { return string.Format("{0} de {1}", ClienteViaje.Count(x => x.Viaje != null && x.Viaje.Estado == ViajeEstados.Cerrado && !x.Presente), ClienteViaje.Count(x => x.Viaje != null && x.Viaje.Estado == ViajeEstados.Cerrado)); } }
    }
}