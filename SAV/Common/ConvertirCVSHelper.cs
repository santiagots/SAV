using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace SAV.Common
{
    public class ConvertirCVSHelper
    {
        public static string ToCsv<T>(string separator, IEnumerable<T> objectlist)
        {
            Type t = typeof(T);
            PropertyInfo[] propertys = t.GetProperties();

            string header = String.Join(separator, propertys.Select(f => f.Name).ToArray());
            

            StringBuilder csvdata = new StringBuilder();
            csvdata.AppendLine(header);

            foreach (var o in objectlist)
                csvdata.AppendLine(ToCsvFields(separator, propertys, o));

            return csvdata.ToString();
        }

        public static string ToCsvFields(string separator, PropertyInfo[] propertys, object o)
        {
            StringBuilder linie = new StringBuilder();

            foreach (var f in propertys)
            {
                if (linie.Length > 0)
                    linie.Append(separator);

                var x = f.GetValue(o);

                if (x != null)
                    linie.Append(x.ToString());
            }

            return linie.ToString();
        }
    }
}