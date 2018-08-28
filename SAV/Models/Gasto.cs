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

        public ConceptoGasto Concepto { get; set; }

        public virtual TipoGasto TipoGasto { get; set; }

        public decimal Monto { get; set; }

        public string Comentario { get; set; }

        public DateTime FechaAlta { get; set; }

        public string UsuarioAlta { get; set; }

        public virtual Viaje viaje { get; set; }
    }
}