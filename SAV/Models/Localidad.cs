using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class Localidad
    {
        public int ID { get; set; }
        public string Nombre { get; set; }
        public virtual List<Parada> Parada { get; set; }
    }
}