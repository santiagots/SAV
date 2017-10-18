﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class ClienteViaje
    {
        public int ID { get; set; }

        public virtual  Cliente Cliente { get; set; }

        public virtual Viaje Viaje { get; set; }

        public bool Pago { get; set; }

        public DateTime? FechaPago { get; set; }

        public virtual Parada Ascenso { get; set; }

        public virtual Parada Descenso { get; set; }

        public virtual Domicilio DomicilioAscenso { get; set; }

        public virtual Domicilio DomicilioDescenso { get; set; }

        public decimal Costo { get; set; }

        public bool AscensoDomicilioPrincipal { get; set; }

        public bool DescensoDomicilioPrincipal { get; set; }

        public bool DescensoDomicilioOtros { get; set; }

        public bool Presente { get; set; }

        public string Vendedor { get; set; }
    }
}