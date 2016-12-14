using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Windows;

namespace ArchyManager.Classes
{
    class SqlUtils
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
                if (property.GetCustomAttributes(false).Length == 1) continue;

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
                if (property.GetCustomAttributes(false).Length == 1) continue;
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
                    comm.ExecuteNonQuery();
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
                    comm.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message + "\n" + e.InnerException,
                                string.Format("Stored Procedure Failed - {0}", spcommand), MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return rv;
        }
    }

    public class SPString
    {
        private SPString(string value) { Value = value; }
        public string Value { get; set; }
        public static SPString INSERT { get { return new SPString("SP_Insert{0}"); } }
        public static SPString UPDATE { get { return new SPString("SP_Update{0}"); } }
    }

    public class SQL2008
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
