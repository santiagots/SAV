using SAV.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SAV.Common
{
    public class GastoHelper
    {
        public static List<Gasto> searchComisionGasto(IQueryable<Gasto> comisionGasto, string Descripcion, DateTime? FechaAlta, DateTime? FechaDesde, DateTime? FechaHasta, decimal Monto, int? TipoGasto, string UsuarioAlta, int? Concepto)
        {
            if (!String.IsNullOrEmpty(Descripcion))
                comisionGasto = comisionGasto.Where(x => x.Comentario.Contains(Descripcion));

            if (FechaAlta.HasValue)
                comisionGasto = comisionGasto.Where(x => DbFunctions.TruncateTime(x.FechaAlta).Value == FechaAlta.Value);

            if (FechaDesde.HasValue)
                comisionGasto = comisionGasto.Where(x => DbFunctions.TruncateTime(x.FechaAlta).Value >= FechaDesde.Value);

            if (FechaHasta.HasValue)
                comisionGasto = comisionGasto.Where(x => DbFunctions.TruncateTime(x.FechaAlta).Value <= FechaHasta.Value);

            if (Monto > 0)
                comisionGasto = comisionGasto.Where(x => x.Monto == Monto);

            if (TipoGasto.HasValue)
                comisionGasto = comisionGasto.Where(x => x.TipoGasto.ID == TipoGasto.Value);

            if (Concepto.HasValue)
                comisionGasto = comisionGasto.Where(x => x.Concepto == (ConceptoGasto)Concepto.Value);

            if (!String.IsNullOrEmpty(UsuarioAlta))
                comisionGasto = comisionGasto.Where(x => x.UsuarioAlta.ToUpper() == UsuarioAlta.ToUpper());

            return comisionGasto.ToList();
        }
    }
}