using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;

namespace ArchyManager.Classes
{
    static class SqlUtils
    {
        // Note: Trim is required for properties in tables for which Matt gave a column the same name as the table >=/
        public static T[] SQLToDataModel<T>(SqlConnection conn, string query = null)
        {
            Type dffType = typeof(T);

            string sql = query ?? string.Format("SELECT * FROM {0}", dffType.Name);
            List<T> datamodellist = new List<T>();

            try
            {
                DataTable datatable = new DataTable();
                using (SqlDataAdapter adapter = new SqlDataAdapter(sql, conn))
                {
                    adapter.Fill(datatable);
                }

                foreach (DataRow row in datatable.Rows)
                {
                    string[] available = row.Table.Columns.Cast<DataColumn>().Select(x => x.ColumnName.TrimEnd('_')).ToArray(); //trim
                    string[] columns = dffType.GetProperties().Select(x => x.Name).Where(x => available.Contains(x)).ToArray();

                    T obj = (T)Activator.CreateInstance(dffType);
                    foreach (string column in columns)
                    {
                        PropertyInfo p = dffType.GetProperty(column);

                        BrowsableAttribute ba = p.GetCustomAttribute(typeof(BrowsableAttribute), false) as BrowsableAttribute;
                        if (ba != null)
                        {
                            if (ba.Browsable == false) continue;
                        }
                        string col = column.TrimEnd('_');  //trim
                        object value = CheckDBNull(row[col]);
                        p.SetValue
                        (
                            obj,
                            value
                        );
                    }
                    datamodellist.Add(obj);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{0}", e.Message));
                throw new Exception(string.Format("There was an error reading {0} from the sql server.", dffType.Name));
            }

            return datamodellist.ToArray();
        }


        //cmd.Parameters.Add("@NewId", SqlDbType.Int).Direction = ParameterDirection.Output;
        //if property name in outputparams, set ParameterDirection.Output
        // loads all data from class into an SqlCommand for use in stored procedure
        public static void AddParametersFrom<T>(T obj, SqlCommand comm)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            Type t = obj.GetType();
            PropertyInfo[] properties = t.GetProperties(
                BindingFlags.Public | // Also include public properties
                BindingFlags.Instance // Specify to retrieve non static properties
            );
            PropertyDescriptorCollection pds = TypeDescriptor.GetProperties(t);

            foreach (PropertyInfo property in properties)
            {
                // this allows browseable flag to determine whether property is sent as a parameter
                BrowsableAttribute ba = property.GetCustomAttribute(typeof(BrowsableAttribute), false) as BrowsableAttribute;
                if (ba != null)
                {
                    if (ba.Browsable == false) continue;
                }

                object value = t.GetProperty(property.Name).GetValue(obj);
                if (value == null) value = DBNull.Value;
                string parameterName = string.Format("@{0}", property.Name);

                if (comm.Parameters.Contains(parameterName))
                {
                    comm.Parameters[parameterName].Value = value;
                }
                else
                {
                    comm.Parameters.Add(new SqlParameter(parameterName, value));
                }
            }
        }
        public static SqlParameter AddParametersFrom<T>(T obj, SqlCommand comm, DbType dbType)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            Type t = obj.GetType();
            PropertyInfo[] properties = t.GetProperties(
                BindingFlags.Public | // Also include public properties
                BindingFlags.Instance // Specify to retrieve non static properties
            );
            
            foreach (PropertyInfo property in properties)
            {
                BrowsableAttribute ba = property.GetCustomAttribute(typeof(BrowsableAttribute), false) as BrowsableAttribute;
                if (ba != null)
                {
                    if (ba.Browsable == false) continue;
                }



                object value = t.GetProperty(property.Name).GetValue(obj);
                if (value == null) value = DBNull.Value;
                string parameterName = string.Format("@{0}", property.Name);

                comm.Parameters.Add(new SqlParameter(parameterName, value));
            }
            SqlParameter rv = comm.CreateParameter();
            rv.ParameterName = "@rv";
            rv.Direction = ParameterDirection.Output;
            rv.DbType = dbType;
            comm.Parameters.Add(rv);
            return rv;
        }

        public static bool ExecuteSP<T>(T row, SqlConnection conn, SPString sp, out string sqlmessage, bool showMessage=true)
        {
            string spcommand = string.Format(sp.Value, row.GetType().Name);
            using (SqlCommand comm = new SqlCommand(spcommand, conn))
            {
                comm.CommandType = CommandType.StoredProcedure;
                AddParametersFrom(row, comm);
                try
                {
                    comm.ExecuteNonQuery();
                    sqlmessage = "Successfully executed stored procedure.";
                    return true;
                }
                catch (SqlException e)
                {
                    if (showMessage)
                    {
                        MessageBox.Show(e.Message,
                                string.Format("Stored Procedure Failed - {0}", spcommand), 
                                MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    sqlmessage = e.Message;
                    return false;
                }
            }
        }
//        catch (StoredProcException spEx)
//        {
//            switch (spEx.ReturnValue)
//            {
//                case 6:
//                    UserMessageException umEx = new UserMessageException(spEx.Message);
//                    throw umEx;
//            }
//}
public static SqlParameter ExecuteSP<T>(T row, SqlConnection conn, SPString sp, DbType outputType, bool showMessage = true)
        {
            string spcommand = string.Format(sp.Value, row.GetType().Name);
            SqlParameter rv;

            using (SqlCommand comm = new SqlCommand(spcommand, conn))
            {
                comm.CommandType = CommandType.StoredProcedure;
                rv = AddParametersFrom(row, comm, outputType);
                try
                {
                    //comm.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    if (showMessage)
                    {
                        MessageBox.Show(e.Message + "\n" + e.InnerException,
                                string.Format("Stored Procedure Failed - {0}", spcommand), MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            return rv;
        }


        public static object ChangeType(object obj, Type type)
        {
            Type targetType = IsNullableType(type) ? Nullable.GetUnderlyingType(type) : type;
            try
            {
                return Convert.ChangeType(obj, targetType);
            }
            catch
            {
                return Activator.CreateInstance(targetType);
            }
            
        }
        public static bool TryChangeType<T>(object obj, Type type, out T value)
        {
            Type targetType = IsNullableType(type) ? Nullable.GetUnderlyingType(type) : type;
            try
            {
                value = (T)Convert.ChangeType(obj, targetType);
                return true;
            }
            catch
            {
                value = default(T);
                return false;
            }

        }
        public static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }

        public static bool SetPropertyBrowsable(Type classtype, string columnname, bool browseable = true)
        {
            try
            {
                PropertyDescriptor pd = TypeDescriptor.GetProperties(classtype)[columnname];
                if (pd.IsBrowsable == browseable) return true;
                BrowsableAttribute att = pd.Attributes.Cast<Attribute>().Where(x => x.GetType() == typeof(BrowsableAttribute)).FirstOrDefault() as BrowsableAttribute;
                FieldInfo fi = typeof(BrowsableAttribute).GetField("browsable", BindingFlags.Instance | BindingFlags.NonPublic);
                fi.SetValue(att, browseable);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static object CheckDBNull(object obj)
        {
            if (obj.Equals(DBNull.Value)) return null;
            return obj;
        }

        public static string GetSQLArray(IEnumerable<object> array, bool asString = true)
        {
            if (asString)
            {
                return string.Format("('{0}')", string.Join("','", array.Distinct()));
            }
            else
            {
                return string.Format("({0})", string.Join(",", array.Distinct()));
            }
        }
    }

    public class SPString
    {
        private SPString(string value) { Value = value; }
        public string Value { get; set; }
        public static SPString INSERT { get { return new SPString("SP_Insert{0}"); } }
        public static SPString ADDNEW { get { return new SPString("SP_AddNew{0}"); } }
    }

    public static class SQL2008
    {
        public const string ARCHY2014_DB = "Archy2014";
        public const string ARCHYPROD_DB = "Archy_Prod";
        public const string ARCHY_CS = "Data Source=sqlprod3\\sql2008;Initial Catalog={0};User Id=developer;Password=sp1d3r5!;";
        
        // only use this to connect or risk a crash
        public static SqlConnection ConnectUsing(string db)
        {
            SqlConnection conn = new SqlConnection(string.Format(ARCHY_CS, db));
            conn.Open();
            return conn;
        }

    }


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
        public static short SafeGetInt16(this SqlDataReader reader,
                                       string columnName, short defaultValue = 0)
        {
            int ordinal = reader.GetOrdinal(columnName);

            if (!reader.IsDBNull(ordinal))
            {
                return reader.GetInt16(ordinal);
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
