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
        public string nacionalidad { get; set; }
        public string tripulante { get; set; }
    }
}