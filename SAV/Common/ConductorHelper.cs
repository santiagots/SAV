using SAV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAV.Common
{
    public class ConductorHelper
    {
        public static List<Conductor> searchCondictores(List<Conductor> conductor, string apellido, string nombre, string numeroDocumento, string telefono)
        {
            if (!String.IsNullOrEmpty(apellido))
                conductor = conductor.Where(x => x.Apellido.ToUpper().Contains(apellido.ToUpper())).ToList<Conductor>();

            if (!String.IsNullOrEmpty(nombre))
                conductor = conductor.Where(x => x.Nombre.ToUpper().Contains(nombre.ToUpper())).ToList<Conductor>();

            if (!String.IsNullOrEmpty(numeroDocumento))
                conductor = conductor.Where(x => x.CUIL.Contains(numeroDocumento)).ToList<Conductor>();

            if (!String.IsNullOrEmpty(telefono))
                conductor = conductor.Where(x => x.Telefono.ToUpper().Contains(telefono.ToUpper())).ToList<Conductor>();

            return conductor;
        }
    }
}