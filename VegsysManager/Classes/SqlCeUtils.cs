using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VegsysManager.Classes
{
    public static class SqlCeConversion
    {
        public static object CheckDBNull(object obj, Type t)
        {
            if (obj.Equals(DBNull.Value)) return null;
            return obj;
        }

        public static string FindType<T> (T obj) { return typeof(T).Name; }
    }
}
