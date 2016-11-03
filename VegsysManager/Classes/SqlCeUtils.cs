using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VegsysManager.Classes
{
    public static class SqlCeConversion
    {
        public static object CheckDBNull(object obj)
        {
            if (obj.Equals(DBNull.Value)) return null;
            return obj;
        }
    }
}
