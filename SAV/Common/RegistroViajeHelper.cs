using SAV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SAV.Common
{
    public class RegistroViajeHelper
    {
        public static RegistroViaje GetRegistro(string usuario, ClienteViaje anterior, ClienteViaje nuevo)
        {
            RegistroViaje registroViaje = new RegistroViaje() { Fecha = DateHelper.getLocal(), Usuario = usuario};

            if (anterior == null)
            {
                registroViaje.Accion += "Alta de usuario al viaje.<br>";
                registroViaje.Accion += string.Format("Costo {0}.<br>", nuevo.Costo);
                registroViaje.Accion += string.Format("Accenso {0}{1}.<br>", nuevo.Ascenso?.Nombre, nuevo.DomicilioAscenso?.getDomicilio);
                registroViaje.Accion += string.Format("Descenso {0}{1}.<br>", nuevo.Descenso?.Nombre, nuevo.DomicilioDescenso?.getDomicilio);
                if (nuevo.FormaPago?.ID != null)
                    registroViaje.Accion += string.Format("Forma de Pago {0}.<br>", nuevo.FormaPago?.Descripcion);
                if (nuevo.Pago)
                    registroViaje.Accion += "Se registra Pago.<br>";
                return registroViaje;
            }
            
            if (anterior.Costo != nuevo.Costo && nuevo.Costo > 0)
                registroViaje.Accion = string.Format("Se modificó el Costo de {0} a {1}.<br>", anterior.Costo, nuevo.Costo);

            if ((anterior.Ascenso?.ID != nuevo.Ascenso?.ID) || (anterior.DomicilioAscenso?.ID != nuevo.DomicilioAscenso?.ID))
                registroViaje.Accion += string.Format("Se modificó el Acceso de {0}{1} a {2}{3}.<br>", anterior.Ascenso?.Nombre, anterior.DomicilioAscenso?.getDomicilio, nuevo.Ascenso?.Nombre, nuevo.DomicilioAscenso?.getDomicilio);

            if ((anterior.Descenso?.ID != nuevo.Descenso?.ID) || (anterior.DomicilioDescenso?.ID != nuevo.DomicilioDescenso?.ID))
                registroViaje.Accion += string.Format("Se modificó el Descenso de {0}{1} a {2}{3}.<br>", anterior.Descenso?.Nombre, anterior.DomicilioDescenso?.getDomicilio, nuevo.Descenso?.Nombre, nuevo.DomicilioDescenso?.getDomicilio);

            if (nuevo.Pago)
                registroViaje.Accion += "Se registra Pago.<br>";

            if (anterior.FormaPago?.ID != nuevo.FormaPago?.ID)
            {
                if(anterior.FormaPago == null)
                    registroViaje.Accion += string.Format("Se registra Forma de Pago {0}.<br>", nuevo.FormaPago?.Descripcion);
                else
                    registroViaje.Accion += string.Format("Se modificó la Forma de Pago de {0} a {1}.<br>", anterior.FormaPago?.Descripcion, nuevo.FormaPago?.Descripcion);
            }

            if (string.IsNullOrEmpty(registroViaje.Accion))
                return null;

            return registroViaje;
        }

        public static RegistroViaje GetRegistroPresente(string usuario, ClienteViaje ClienteViaje, bool PresenteAnterior, bool PresenteNuevo)
        {
            RegistroViaje registroViaje = null;

            if (PresenteAnterior != PresenteNuevo)
            {
                registroViaje = new RegistroViaje() { Fecha = DateHelper.getLocal(), Usuario = usuario};

                if(PresenteNuevo)
                    registroViaje.Accion += "Se registra Presente.<br>";
                else
                    registroViaje.Accion += "Se quita Presente.<br>";
            }

            return registroViaje;
        }

        public static RegistroViaje GetRegistroPago(string usuario, ClienteViaje ClienteViaje, bool PagoAnterior, bool PagoNuevo)
        {
            RegistroViaje registroViaje = null;

            if (PagoAnterior != PagoNuevo)
            {
                registroViaje = new RegistroViaje() { Fecha = DateHelper.getLocal(), Usuario = usuario};

                if (PagoNuevo)
                    registroViaje.Accion += "Se registra Pago.<br>";
                else
                    registroViaje.Accion += "Se quita Pago.<br>";
            }

            return registroViaje;
        }
    }
}