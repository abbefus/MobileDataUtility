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
        public static void AddParametersFrom<T>(T obj, SqlCommand dbcommand)
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

                if (dbcommand.Parameters.Contains(parameterName))
                {
                    dbcommand.Parameters[parameterName].Value = value;
                }
                else
                {
                    dbcommand.Parameters.Add(new SqlParameter(parameterName, value));
                }
            }
        }

        //public void SaveToDatabase(ShovelTestPit stp)
        //{
        //    AddParametersFrom<ShovelTestPit>(stp, dbcommand);

        //    OpenConnection();

        //    dbcommand.CommandType = CommandType.StoredProcedure;
        //    dbcommand.CommandText = "V_SP_AddShovelTestPit";

        //    try
        //    {
        //        dbcommand.ExecuteNonQuery();
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show(e.Message + "\n" + e.InnerException,
        //                    "Stored Procedure Failed", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }


        //    CloseConnection();
        //}
    }

    public class ArchyConnection
    {
        public const string ARCHY2014_DB = "Archy2014";
        public const string ARCHY_PROD = "Archy_Prod";
        public const string ARCHY_CS = "Data Source=sqlprod3\\sql2008;Initial Catalog={0};User Id=developer;Password=sp1d3r5!;";
        public SqlConnection dbConn;
        public SqlCommand dbCommand;
        
        private void Connect(string db)
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
