using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using SAV.Common;

namespace SAV.Models
{
    public class CreateViajeViewModel
    {
        public int viajeID { get; set; }

        public ViajeViewModel DatosBasicosViaje { get; set; }

        [Display(Name = "Lunes:")]
        public bool Lunes { get; set; }

        [Display(Name = "Martes:")]
        public bool Martes { get; set; }

        [Display(Name = "Miercoles:")]
        public bool Miercoles { get; set; }

        [Display(Name = "Jueves:")]
        public bool Jueves { get; set; }

        [Display(Name = "Viernes:")]
        public bool Viernes { get; set; }

        [Display(Name = "Sabado:")]
        public bool Sabado { get; set; }

        [Display(Name = "Domingo:")]
        public bool Domingo { get; set; }

        [Display(Name = "Fecha Fin Repetición:")]
        public string FechaRepeticionFin { get; set; }

        public CreateViajeViewModel() : base()
        { }

        public CreateViajeViewModel(List<Conductor> conductores)
        {
            DatosBasicosViaje = new ViajeViewModel();

            DatosBasicosViaje.ConductorNombre = conductores.Select(x => new KeyValuePair<int, string>(x.ID, string.Format("{0} , {1}", x.Apellido, x.Nombre))).ToList();
        }

        public CreateViajeViewModel(List<Conductor> conductores, List<Localidad> destinos, List<Provincia> provincias, List<ModalidadPrestacion> modalidadPrestacion) : this(conductores)
        {
            DatosBasicosViaje.ModalidadPrestacion = modalidadPrestacion.Select(x => new KeyValuePair<string, string>(x.Codigo, x.Descripcion)).ToList();

            DatosBasicosViaje.Destino = destinos.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            DatosBasicosViaje.Origen = destinos.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();

            DatosBasicosViaje.ProvinciaOrigenCerrado = provincias.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
            DatosBasicosViaje.ProvinciaDestinoCerrado = provincias.Select(x => new KeyValuePair<int, string>(x.ID, x.Nombre)).ToList();
        }
    }
}