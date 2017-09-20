using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class CNRTPasajero
    {
        public string tipo_documento { get; set; }
        public string descripcion_documento { get; set; }
        public string numero_documento { get; set; }
        public string nombre { get; set; }
        public string apellido { get; set; }
        public string sexo { get; set; }
        public string menor { get; set; }
        public string origen { get; set; }
        public string provincia_origen { get; set; }
        public string destino { get; set; }
        public string provincia_destino { get; set; }
        public string nacionalidad { get; set; }
        public int numero_butaca { get; set; }
        public int numero_boleto { get; set; }
    }
}