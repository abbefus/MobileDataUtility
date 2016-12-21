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

        public static void ExecuteSP<T>(T row, SqlConnection conn, SPString sp)
        {
            string spcommand = string.Format(sp.Value, row.GetType().Name);
            using (SqlCommand comm = new SqlCommand(spcommand, conn))
            {
                comm.CommandType = CommandType.StoredProcedure;
                AddParametersFrom(row, comm);
                try
                {
                    //comm.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message + "\n" + e.InnerException,
                                string.Format("Stored Procedure Failed - {0}", spcommand), MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        public static SqlParameter ExecuteSP<T>(T row, SqlConnection conn, SPString sp, DbType outputType)
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
                    MessageBox.Show(e.Message + "\n" + e.InnerException,
                                string.Format("Stored Procedure Failed - {0}", spcommand), MessageBoxButton.OK, MessageBoxImage.Error);
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
        private static bool IsNullableType(Type type)
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
}
