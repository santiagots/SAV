using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SAV.Models
{
    public class SearchGastoViewModel
    {
        [Display(Name = "Concepto:")]
        public List<KeyValuePair<int, string>> Concepto { get; set; }

        public int? SelectConcepto { get; set; }

        [Display(Name = "Tipo Gasto:")]
        public List<KeyValuePair<int, string>> TipoGasto { get; set; }

        public int? SelectTipoGasto { get; set; }

        [Display(Name = "Comentario:")]
        public string Comentario { get; set; }

        [Display(Name = "Monto:")]
        public string Monto { get; set; }

        [Display(Name = "Fecha Alta:")]
        public string FechaAlta { get; set; }

        [Display(Name = "Fecha Desde:")]
        public string FechaDesde { get; set; }

        [Display(Name = "Fecha Hasta:")]
        public string FechaHasta { get; set; }

        [Display(Name = "Usuario Alta:")]
        public List<KeyValuePair<string, string>> UsuarioAlta { get; set; }

        public string SelectUsuarioAlta { get; set; }

        public IPagedList<Gasto> Gastos { get; set; }
    }
}