using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VegsysManager.Classes
{
    public static class SqlDataReaderExtensions
    {
        public static int SafeGetInt32(this SqlDataReader reader,
                                       string columnName, int defaultValue = 0)
        {
            int ordinal = reader.GetOrdinal(columnName);

            if (!reader.IsDBNull(ordinal))
            {
                return reader.GetInt32(ordinal);
            }
            else
            {
                return defaultValue;
            }
        }
        public static string SafeGetString(this SqlDataReader reader,
                                       string columnName, string defaultValue = "")
        {
            int ordinal = reader.GetOrdinal(columnName);

            if (!reader.IsDBNull(ordinal))
            {
                return reader.GetString(ordinal);
            }
            else
            {
                return defaultValue;
            }
        }
        public static DateTime SafeGetDateTime(this SqlDataReader reader,
                                       string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);

            if (!reader.IsDBNull(ordinal))
            {
                return reader.GetDateTime(ordinal);
            }
            else
            {
                return DateTime.MinValue;
            }
        }
        public static Guid SafeGetGuid(this SqlDataReader reader,
                                       string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);

            if (!reader.IsDBNull(ordinal))
            {
                return reader.GetGuid(ordinal);
            }
            else
            {
                return Guid.Empty;
            }
        }
    }
}
