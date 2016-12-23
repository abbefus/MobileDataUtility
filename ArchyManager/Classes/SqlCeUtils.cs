using System;

namespace ArchyManager.Classes
{
    public static class SqlCeConversion
    {
        public static object CheckDBNull(object obj)
        {
            if (obj.Equals(DBNull.Value)) return null;
            return obj;
        }

        public static string FindType<T> (T obj) { return typeof(T).Name; }
    }
}
