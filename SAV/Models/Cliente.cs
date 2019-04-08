using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public enum TipoDocumento { Dni, Pasaporte }
    public enum Sexo { Femenino, Masculino }
    public enum Edad { Mayor, Menor }
    public class Cliente
    {
        public int ID { get; set; }
        
        public string Nombre { get; set; }

        public string Apellido { get; set; }

        public TipoDocumento TipoDocumento { get; set; }

        public string NumeroDocumento { get; set; }

        public virtual Sexo Sexo { get; set; }

        public virtual Edad Edad { get; set; }

        public string Nacionalidad { get; set; }

        public virtual List<Domicilio> Domicilios { get; set; }

        public string Telefono { get; set; }

        public string TelefonoAlternativo { get; set; }

        public string Email { get; set; }

        public bool Estudiante { get; set; }

        public DateTime FechaCreacion { get; set; }

        public virtual List<ClienteViaje> ClienteViaje { get; set; }

        public string getClienteViaje { get { return string.Format("{0} de {1}", ClienteViaje.Count(x => x.Viaje != null && x.Viaje.Estado == ViajeEstados.Cerrado && !x.Presente), ClienteViaje.Count(x => x.Viaje != null && x.Viaje.Estado == ViajeEstados.Cerrado)); } }

        public Cliente()
        {
            Domicilios = new List<Domicilio>();
        }
    }
}