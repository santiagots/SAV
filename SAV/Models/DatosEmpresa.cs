using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class DatosEmpresa
    {
        public int ID { get; set; }
        public string RazonSocial { get; set; }
        public string CUIT { get; set; }
        public virtual Domicilio Domicilio { get; set; }
        public string Telefono { get; set; }
        public string ContrtanteCuit { get; set; }
        public string ContratanteDenominacion { get; set; }
        public virtual Domicilio ContratanteDomicilio { get; set; }

        public string aa { get; set; }
    }
}