using PagedList;
using SAV.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class SearchComisionViewModel
    {
        
        [Display(Name = "Numero:")]
        public string ID { get; set; }

        [Display(Name = "Contacto:")]
        public string Contacto { get; set; }

        [Display(Name = "Servicio:")]
        public string Servicio { get; set; }

        [Display(Name = "Acción:")]
        public string Accion { get; set; }

        [Display(Name = "Fecha Alta:")]
        public string FechaAlta { get; set; }

        [Display(Name = "Fecha Envio:")]
        public string FechaEnvio { get; set; }

        [Display(Name = "Fecha Entrega:")]
        public string FechaEntrega { get; set; }

        [Display(Name = "Fecha Pago:")]
        public string FechaPago { get; set; }

        [Display(Name = "Costo:")]
        public string Costo { get; set; }

        [Display(Name = "Responsable:")]
        public List<KeyValuePair<int, string>> Responsable { get; set; }

        public int? SelectResponsable { get; set; }

        [Display(Name = "Cuenta Corriente:")]
        public List<KeyValuePair<int, string>> CuentaCorriente { get; set; }

        public int? SelectCuentaCorriente { get; set; }

        public IPagedList ComisionesEnvio { get; set; }

        public IPagedList ComisionesPendientes { get; set; }

        public IPagedList ComisionesFinalizadas { get; set; }

        public SearchComisionViewModel()
        {
            ID = string.Empty;

            Contacto = string.Empty;

            Servicio = string.Empty;

            Accion = string.Empty;

            FechaAlta = string.Empty;

            FechaEnvio = string.Empty;

            FechaEntrega = string.Empty;

            FechaPago = string.Empty;

            Costo = string.Empty;
        }

        public SearchComisionViewModel(IQueryable<Comision> Comisiones)
        {
            IQueryable<Comision> comisionesEnvio = ComisionHelper.getEnvios(Comisiones);
            IQueryable<Comision> comisionesPendientes = ComisionHelper.getPendientes(Comisiones);
            IQueryable<Comision> comisionesFinalizadas = ComisionHelper.getFinalizadas(Comisiones);

            this.ComisionesEnvio = comisionesEnvio.ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
            this.ComisionesPendientes = comisionesPendientes.ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
            this.ComisionesFinalizadas = comisionesFinalizadas.ToPagedList(1, int.Parse(ConfigurationSettings.AppSettings["PageSize"]));
        }
    }
}