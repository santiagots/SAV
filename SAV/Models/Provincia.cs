using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class Provincia
    {
        public int ID { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Codigo { get; set; }
        [Required]
        public virtual List<Localidad> Localidad { get; set; }
    }
}