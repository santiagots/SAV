using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class RegistroViaje
    {
        public int ID { get; set; }

        public DateTime Fecha { get; set; }

        public string Usuario { get; set; }

        public string Accion { get; set; }
    }
}