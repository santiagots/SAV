using SAV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAV.Common
{
    public class ClienteHelper
    {
        public static List<Cliente> searchClientes(List<Cliente> cliente, string apellido, string nombre, string numeroDocumento, string telefono)
        {
            if (!String.IsNullOrEmpty(apellido))
                cliente = cliente.Where(x => x.Apellido != null && x.Apellido.ToUpper().Contains(apellido.ToUpper())).ToList<Cliente>();

            if (!String.IsNullOrEmpty(nombre))
                cliente = cliente.Where(x => x.Nombre != null && x.Nombre.ToUpper().Contains(nombre.ToUpper())).ToList<Cliente>();

            if (!String.IsNullOrEmpty(numeroDocumento))
                cliente = cliente.Where(x => x.NumeroDocumento != null && x.NumeroDocumento.ToString().Contains(numeroDocumento)).ToList<Cliente>();

            if (!String.IsNullOrEmpty(telefono))
                cliente = cliente.Where(x => x.Telefono != null && x.Telefono.ToUpper().Contains(telefono.ToUpper())).ToList<Cliente>();

            return cliente;
        }
    }
}