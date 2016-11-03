using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace AddSites2Vegsys
{
    public class DataAccess
    {
		private enum Connections
		{
			DEV_CONN_STRING,
			PROD_CONN_STRING,
			EAST_CONN_STRING,
			DEV_USERDB_CONN_STRING,
			PROD_USERDB_CONN_STRING,
			EAST_USERDB_CONN_STRING
		}

        #region Constants

        private const string ENV_DEV = "DEV";
        private const string ENV_PROD = "PROD";
        private const string ENV_EAST = "EAST";
        public const string ENV_SETTING_KEY = "ENVIRONMENT";

        //private const string DEV_CONN_STRING = "Data Source=CDN101403;Initial Catalog=VegNet;User Id=AppVegUser;Password=VegPwdDB;";
        //private const string DEV_CONN_STRING = "Data Source=informatics;Initial Catalog=VegProd_Test;User Id=developer;Password=sp1d3r5!;";
        //private const string PROD_CONN_STRING = "Data Source=72.29.231.27;Initial Catalog=vegsys_v2_prod;User Id=developer;Password=sp1d3r5!;";
        //private const string EAST_CONN_STRING = "";

        //private const string DEV_USERDB_CONN_STRING = "Data Source=72.29.231.27;Initial Catalog=dbmanager;User Id=developer;Password=sp1d3r5!;";
        //private const string PROD_USERDB_CONN_STRING = "Data Source=72.29.231.27;Initial Catalog=dbmanager;User Id=developer;Password=sp1d3r5!;";
        //private const string EAST_USERDB_CONN_STRING = "";
        public const string STAGE_CONN_STRING = "Data Source=sqlprod3\\sql2008;Initial Catalog=VEGSYS_2014;User Id=developer;Password=sp1d3r5!;";
        public const string PROD_CONN_STRING = "Data Source=sqlprod3\\sql2008;Initial Catalog=vegfield_2014;User Id=developer;Password=sp1d3r5!;";
        public const string DEV_CONN_STRING = "Data Source=IMSDEV3;Initial Catalog=VEGSYS_2014;User Id=developer;Password=sp1d3r5!;";

        #endregion

        #region Properties and Private Members

        private string connectionString = DataAccess.GetConnectionString();
        private SqlConnection dbConn;
        private SqlCommand dbcommand;
        private string sSQL;

        public string ConnectionString
        {
            get { return connectionString; }
        }

        public SqlCommand DBCommand
        {
            get { return dbcommand; }
            set { dbcommand = value; }
        }

        public string SQL
        {
            get { return sSQL; }
            set { sSQL = value; }
        }


        #endregion

        public DataAccess(string DatabaseConnString)
        {
            try
            {
                connectionString = DatabaseConnString;
                dbConn = new SqlConnection(connectionString);
                dbcommand = new SqlCommand();
                dbcommand.Connection = dbConn;
            }
            catch (Exception e)
            {
                throw new Exception("An error has occurred", e.InnerException);
            }
            
        }

		public DataAccess()
		{
			try
			{				
				dbConn = new SqlConnection(DataAccess.GetConnectionString());
				dbcommand = new SqlCommand();
				dbcommand.Connection = dbConn;
			}
			catch (Exception e)
			{
				throw new Exception("An error has occurred", e.InnerException);
			}

		}

        public bool Open()
        {
            dbConn.Open();
            return dbConn.State == ConnectionState.Open;
        }

        public bool Close()
        {
            dbConn.Close();
            return dbConn.State == ConnectionState.Closed;
        }

        public SqlDataReader ExecuteSelectSQL(string sql, bool quietMode, CommandBehavior specialCmdBehavior)
        {
            //the command object should have been initialized in the constructor...
            if (dbcommand == null) throw new ArgumentNullException("The database command object is null");

            try
            {
                SqlDataReader objDataReader;
                dbcommand.CommandText = sql;

                if (dbConn.State != ConnectionState.Open)
                {
                    dbConn.Open();
                }

                objDataReader = dbcommand.ExecuteReader(specialCmdBehavior);
                return objDataReader;

            }
            catch (Exception exc)
            {
                if (!quietMode) { throw new Exception("An error has occured: " + exc.Message, exc.InnerException); }
                return null;
            }
        }

        /// <summary>
        /// Executes the specified select SQL statement and returns the result
        /// as a SQL Server-specific DataReader object.
        /// </summary>
        /// <param name="sql">The SELECT query to execute</param>
        /// <param name="quietMode">Specifies whether to display exceptions, if thrown.</param>
        /// <returns>SqlDataReader containing the records retrieved from the query</returns>
        public SqlDataReader ExecuteSelectSQL(string sql, bool quietMode)
        {
            //the command object should have been initialized in the constructor...
            if (dbcommand == null) throw new ArgumentNullException("The database command object is null");

			try
			{
				SqlDataReader objDataReader;
				dbcommand.CommandText = sql;

				if (dbConn.State != ConnectionState.Open)
				{
					dbConn.Open();
				}

				objDataReader = dbcommand.ExecuteReader(CommandBehavior.CloseConnection);
				return objDataReader;

			}
			catch (Exception exc)
			{
				if (!quietMode) { throw new Exception("An error has occured: " + exc.Message, exc.InnerException); }
				return null;
			}
			//finally
			//{
			//    if (dbConn.State == ConnectionState.Open)
			//        dbConn.Close();
			//}
        }

        /// <summary>
        /// Executes the specified SQL action query (insert, update, delete, etc.)
        /// and returns the number of rows affected.  Closes the database when finished.
        /// </summary>
        /// <param name="sql">The query to execute</param>
        /// <param name="quietMode">Specifies whether to display exceptions, if thrown.</param>
        /// <returns>Integer containing the number of rows affected</returns>
        public int ExecuteInsertOrUpdateSQL(string sql, bool quietMode)
        {
            try
            {
                sSQL = sql;
                dbcommand.CommandText = sSQL;

                if (dbConn.State != ConnectionState.Open)
                {
                    dbConn.Open();
                }

                int intResults = dbcommand.ExecuteNonQuery();
                dbcommand = null;
                dbConn.Close();
                dbConn = null;
                return intResults;
             }
            catch (Exception e)
            {
                if (!quietMode) { throw new Exception("An error has occurred" + e.Message, e.InnerException); }
                return -1;
            }

        }
                      

        /// <summary>
        /// Returns the appropriate connection string based on the environment in the web.config file.
        /// </summary>
        /// <returns></returns>
        public static string GetConnectionString()
        {
            switch (ConfigurationManager.AppSettings[ENV_SETTING_KEY])
            {
                case ENV_DEV:
                    return ConfigurationManager.AppSettings[Enum.GetName(typeof(Connections), Connections.DEV_CONN_STRING)];
                case ENV_PROD:
					return ConfigurationManager.AppSettings[Enum.GetName(typeof(Connections), Connections.PROD_CONN_STRING)];
                case ENV_EAST:
                    return ConfigurationManager.AppSettings[Enum.GetName(typeof(Connections), Connections.EAST_CONN_STRING)];
                default:
                    return "";
            }
        }

        /// <summary>
        /// Returns the appropriate connection string for the user table (dbmanager.dbuserlu)
        /// </summary>
        /// <returns></returns>
        public static string GetUserDBConnectionString()
        {
            switch (ConfigurationManager.AppSettings[ENV_SETTING_KEY])
            {
                case ENV_DEV:
                    return ConfigurationManager.AppSettings[Enum.GetName(typeof(Connections), Connections.DEV_USERDB_CONN_STRING)];
                case ENV_PROD:
                    return ConfigurationManager.AppSettings[Enum.GetName(typeof(Connections), Connections.PROD_USERDB_CONN_STRING)];
                case ENV_EAST:
                    return ConfigurationManager.AppSettings[Enum.GetName(typeof(Connections), Connections.EAST_USERDB_CONN_STRING)];
                default:
                    return "";
            }
        }

        public bool RecordExists(string TableName, string Column, string WhereClause)
        {
            sSQL = "SELECT COUNT(" + Column + ") as RecordCount FROM " + TableName + " " + WhereClause;
            bool blnRetVal = false;
            SqlDataReader objDR = ExecuteSelectSQL(sSQL, true);
            if (objDR == null) return false;
            while (objDR.Read()) 
            {
                if (Convert.ToInt32(objDR["RecordCount"]) > 0) {blnRetVal = true;}
                else {blnRetVal = false;}
            }
            
            return blnRetVal;
        }

        public DataSet GetDataSet(string sql, string tableName)
        {

            SqlConnection conn = new SqlConnection(this.ConnectionString);
            SqlDataAdapter objDA = new SqlDataAdapter();
            DataSet objDataSet = new DataSet();

            objDA.SelectCommand = new SqlCommand(sql,conn);

            try
            {
                objDA.Fill(objDataSet, tableName);
            }
            catch (Exception ex)
            {
                throw new Exception("An error has occurred" + ex.Message, ex.InnerException);
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                    conn = null;                 
                }
            }

            return objDataSet;
            
        }


        internal static string DBNZ(object p)
        {
            if (Convert.IsDBNull(p))
            {
                return "";
            }
            else
            {
                return p.ToString();
            }
        }

        internal static int DBNZInt32(System.Data.SqlClient.SqlDataReader objDR, string columnName)
        {
            if (objDR.IsDBNull(objDR.GetOrdinal(columnName)))
            {
                return default(int);
            }
            else
            {
                return objDR.GetInt32(objDR.GetOrdinal(columnName));
            }
        }

		internal static DateTime? DBNZDateTime(System.Data.SqlClient.SqlDataReader objDR, string columnName)
		{
			if (objDR.IsDBNull(objDR.GetOrdinal(columnName)))
			{
				return null;
			}
			else
			{
				return objDR.GetDateTime(objDR.GetOrdinal(columnName));
			}
		}

		internal static bool DBNZBool(System.Data.SqlClient.SqlDataReader objDR, string columnName)
		{
			if (objDR.IsDBNull(objDR.GetOrdinal(columnName)))
			{
				return default(bool);
			}
			else
			{
				return (bool)objDR.GetSqlBoolean(objDR.GetOrdinal(columnName));
			}
		}

        internal static short DBNZIntSmall(System.Data.SqlClient.SqlDataReader objDR, string columnName)
        {
            if (objDR.IsDBNull(objDR.GetOrdinal(columnName)))
            {
                return default(short);
            }
            else
            {
                return objDR.GetInt16(objDR.GetOrdinal(columnName));
            }
        }

        internal static byte DBNZTinyInt(System.Data.SqlClient.SqlDataReader objDR, string columnName)
        {
          if (objDR.IsDBNull(objDR.GetOrdinal(columnName)))
          {
            return default(byte);
          }
          else
          {
            return objDR.GetByte(objDR.GetOrdinal(columnName));
          }
        }

		internal static byte? DBNZTinyIntNull(System.Data.SqlClient.SqlDataReader objDR, string columnName)
		{
			if (objDR.IsDBNull(objDR.GetOrdinal(columnName)))
			{
				return null;
			}
			else
			{
				return objDR.GetByte(objDR.GetOrdinal(columnName));
			}
		}

        internal static Guid DBNZGuid(System.Data.SqlClient.SqlDataReader objDR, string columnName)
        {
          if (objDR.IsDBNull(objDR.GetOrdinal(columnName)))
          {
            return default(Guid);
          }
          else
          {
            return objDR.GetGuid(objDR.GetOrdinal(columnName));
          }
        }

        public static string GetTextField(string fieldName, string tableName, string whereClause, string DataConnString)
        {
            DataAccess objDA = new DataAccess(DataConnString);
            string strSQL = "SELECT " + fieldName + " AS TextRecord FROM " + tableName;
            string retVal = "";

            //append where clause if specified...
            if (whereClause != null && whereClause != "")
            {
                strSQL += " " + whereClause;
            }

            SqlDataReader objDR = objDA.ExecuteSelectSQL(strSQL, false);
            while (objDR.Read())
            {
                retVal = objDR["TextRecord"].ToString();
            }

            objDR.Close();
            objDR = null;
            objDA.Close();
            objDA = null;

            return retVal;
        }

        public static string GetTextField(string fieldName, string tableName, string whereClause)
        {
            return GetTextField(fieldName, tableName, whereClause, DataAccess.GetConnectionString());
        }

        public static long GetLongField(string fieldName, string tableName, string whereClause)
        {
            DataAccess objDA = new DataAccess(DataAccess.GetConnectionString());
            string strSQL = "SELECT " + fieldName + " FROM " + tableName;
            long retVal = 0;

            //append where clause if specified...
            if (whereClause != null && whereClause != "")
            {
                strSQL += " " + whereClause;
            }

            SqlDataReader objDR = objDA.ExecuteSelectSQL(strSQL, false);
            while (objDR.Read())
            {
                retVal = objDR.GetInt64(objDR.GetOrdinal(fieldName));
            }

            objDR.Close();
            objDR = null;
            objDA.Close();
            objDA = null;

            return retVal;
        }

        public static int GetIntField(string fieldName, string tableName, string whereClause)
        {
            DataAccess objDA = new DataAccess(DataAccess.GetConnectionString());
            string strSQL = "SELECT " + fieldName + " FROM " + tableName;
            int retVal = 0;

            //append where clause if specified...
            if (whereClause != null && whereClause != "")
            {
                strSQL += " " + whereClause;
            }

            SqlDataReader objDR = objDA.ExecuteSelectSQL(strSQL, false);
            while (objDR.Read())
            {
                retVal = objDR.GetInt32(objDR.GetOrdinal(fieldName));
            }

            objDR.Close();
            objDR = null;
            objDA.Close();
            objDA = null;

            return retVal;
        }

        public static short GetSmallIntField(string fieldName, string tableName, string whereClause)
        {
            DataAccess objDA = new DataAccess(DataAccess.GetConnectionString());
            string strSQL = "SELECT " + fieldName + " FROM " + tableName;
            short retVal = 0;

            //append where clause if specified...
            if (whereClause != null && whereClause != "")
            {
                strSQL += " " + whereClause;
            }

            SqlDataReader objDR = objDA.ExecuteSelectSQL(strSQL, false);
            while (objDR.Read())
            {
                retVal = objDR.GetInt16(objDR.GetOrdinal(fieldName));
            }

            objDR.Close();
            objDR = null;
            objDA.Close();
            objDA = null;

            return retVal;
        }

        internal static double DBNZDouble(SqlDataReader objDR, string col)
        {
            if (objDR.IsDBNull(objDR.GetOrdinal(col)))
            {
                return default(double);
            }
            else
            {
                return objDR.GetDouble(objDR.GetOrdinal(col));
            }
        }

		internal static double? DBNZDoubleNull(SqlDataReader objDR, string col)
        {
            if (objDR.IsDBNull(objDR.GetOrdinal(col)))
            {
                return null;
            }
            else
            {
                return objDR.GetDouble(objDR.GetOrdinal(col));
            }
        }

		internal static object SetParameterValue(object p)
		{
			if (p != null)
				return p;
			else
				return System.DBNull.Value;
		}
	}
}
