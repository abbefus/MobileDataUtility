using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VegsysManager.Classes
{
    class SqlUtils
    {
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
}
