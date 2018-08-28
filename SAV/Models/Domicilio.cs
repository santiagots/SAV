using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class Domicilio
    {
        public int ID { get; set; }

        public virtual Provincia Provincia { get; set; }

        public virtual Localidad Localidad { get; set; }

        public String Calle { get; set; }

        public String Numero { get; set; }

        public String Piso { get; set; }

        public String Comentario { get; set; }

        public string getDomicilio { get { return string.Format("{0} {1} {2} ({3}) {4}", Calle ?? "", Numero ?? "", Piso ?? "", Localidad != null? Localidad.Nombre ?? "" : "", Comentario != null ? string.Concat("[", Comentario, "]") : ""); } }
    }
}