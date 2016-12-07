using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Windows;

namespace ArchyManager.Classes
{
    class SqlUtils
    {

        //cmd.Parameters.Add("@NewId", SqlDbType.Int).Direction = ParameterDirection.Output;
        //if property name in outputparams, set ParameterDirection.Output
        // loads all data from class into an SqlCommand for use in stored procedure
        public static void AddParametersFrom<T>(T obj, ref SqlCommand dbCommand)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            Type t = obj.GetType();
            PropertyInfo[] properties = t.GetProperties(
                BindingFlags.Public | // Also include public properties
                BindingFlags.Instance // Specify to retrieve non static properties
            );

            foreach (PropertyInfo property in properties)
            {
                object value = t.GetProperty(property.Name).GetValue(obj);
                if (value == null) value = DBNull.Value;
                string parameterName = string.Format("@{0}", property.Name);

                if (dbCommand.Parameters.Contains(parameterName))
                {
                    dbCommand.Parameters[parameterName].Value = value;
                }
                else
                {
                    dbCommand.Parameters.Add(new SqlParameter(parameterName, value));
                }
            }
        }

    }

    public class ArchyConnection
    {
        public const string ARCHY2014_DB = "Archy2014";
        public const string ARCHYPROD_DB = "Archy_Prod";
        public const string ARCHY_CS = "Data Source=sqlprod3\\sql2008;Initial Catalog={0};User Id=developer;Password=sp1d3r5!;";
        public SqlConnection dbConn;
        public SqlCommand dbCommand;
        
        public void ConnectUsing(string db)
        {
            try
            {
                dbConn = new SqlConnection(string.Format(ARCHY_CS,db));
                dbCommand = new SqlCommand();
                dbCommand.Connection = dbConn;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Data.ToString(),
                    "Database Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public bool OpenConnection()
        {
            dbConn.Open();
            return dbConn.State == ConnectionState.Open;
        }
        public bool CloseConnection()
        {
            dbConn.Close();
            return dbConn.State == ConnectionState.Closed;
        }
    }
}
