using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class ComisionGasto
    {
        public int ID { get; set; }

        public string Descripcion { get; set; }

        public decimal Monto { get; set; }

        public DateTime FechaAlta { get; set; }
    }
}