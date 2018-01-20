using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class Gasto
    {
        public int ID { get; set; }

        public virtual TipoGasto TipoGasto { get; set; }

        public string Monto { get; set; }

        public string Comentario { get; set; }
    }
}