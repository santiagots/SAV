using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PagedList;

namespace SAV.Models
{
    public class SearchViajeViewModel
    {
        [Display(Name = "Origen:")]
        public List<KeyValuePair<int, string>> Origen { get; set; }

        public int? SelectOrigen { get; set; }

        [Display(Name = "Destino:")]
        public List<KeyValuePair<int, string>> Destino { get; set; }

        [Display(Name = "Código:")]
        public int Codigo { get; set; }

        public int? SelectDestino { get; set; }

        [Display(Name = "Fecha de Salida:")]
        public string FechaSalida { get; set; }

        [Display(Name = "Tipo de Servicio:")]
        public string Servicio { get; set; }

        public IPagedList<Viaje> ViajesActivos { get; set; }

        public IPagedList<Viaje> ViajesFinalizados { get; set; }

        public SearchViajeViewModel(): base()
        { }
    }
}