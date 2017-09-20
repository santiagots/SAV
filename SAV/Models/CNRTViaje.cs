using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class CNRTViaje
    {
        public string fecha_inicio { get; set; }
        public string fecha_fin { get; set; }
        public string origen { get; set; }
        public string provincia_origen { get; set; }
        public string destino { get; set; }
        public string provincia_destino { get; set; }
        public string dominio { get; set; }
        public string dominio_suplente { get; set; }
        public string tripulante_1_cuit { get; set; }
        public string tripulante_1_nombre { get; set; }
        public string tripulante_1_apellido { get; set; }
        public string tripulante_1_es_chofer { get; set; }
        public string tripulante_2_cuit { get; set; }
        public string tripulante_2_nombre { get; set; }
        public string tripulante_2_apellido { get; set; }
        public string tripulante_2_es_chofer { get; set; }
        public string tripulante_3_cuit { get; set; }
        public string tripulante_3_nombre { get; set; }
        public string tripulante_3_apellido { get; set; }
        public string tripulante_3_es_chofer { get; set; }
        public string contratante_cuit { get; set; }
        public string contratante_denominacion { get; set; }
        public string contratante_domicilio { get; set; }
        public string contenido_programacion_turistica { get; set; }
        public string observaciones { get; set; }
    }
}