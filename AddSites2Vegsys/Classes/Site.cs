using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace AddSites2Vegsys
{

    public class Site
    {
   
        private int mintID;
        public int ID
        {
            get { return mintID; }
            set { mintID = value; }
        }

        private string mstrSiteNumber;
        public string SiteNumber
        {
            get { return mstrSiteNumber; }
            set { mstrSiteNumber = value; }
        }

        private double? mintEasting;
        public double? Easting
        {
            get { return mintEasting; }
            set { mintEasting = value; }
        }

        private double? mintNorthing;
        public double? Northing
        {
            get { return mintNorthing; }
            set { mintNorthing = value; }
        }

        private double? mintLatitude;
        public double? Latitude
        {
            get { return mintLatitude; }
            set { mintLatitude = value; }
        }//modified from int to double

        private double? mintLongitude;
        public double? Longitude
        {
            get { return mintLongitude; }
            set { mintLongitude = value; }
        }//modified from int to double

        private int? mintZone;
        public int? Zone
        {
            get { return mintZone; }
            set { mintZone = value; }
        }

        private int mstrFieldSourceID;
        public int FieldSourceID
        {
            get { return mstrFieldSourceID; }
            set { mstrFieldSourceID = value; }
        }

        private string mstrProjectNumber;
        public string ProjectNumber
        {
            get { return mstrProjectNumber; }
            set { mstrProjectNumber = value; }
        }

        private string mstrProjectName;
        public string ProjectName
        {
            get { return mstrProjectName; }
            set { mstrProjectName = value; }
        }

        private string mstrComments;
        public string Comments
        {
            get { return mstrComments; }
            set { mstrComments = value; }
        }

        private double? mflAspect;
        public double? Aspect
        {
            get { return mflAspect; }
            set { mflAspect = value; }
        }

        private double? mflEstimatedForageProd;
        public double? EstimatedForageProd
        {
            get { return mflEstimatedForageProd; }
            set { mflEstimatedForageProd = value; }
        }

        private string mstrLocation;
        public string Location
        {
            get { return mstrLocation; }
            set { mstrLocation = value; }
        }

        private string mstrProvinceName;
        public string ProvinceName
        {
            get { return mstrProvinceName; }
            set { mstrProvinceName = value; }
        }

        private double? mfltSlope;
        public double? Slope
        {
            get { return mfltSlope; }
            set { mfltSlope = value; }
        }

        private string mstrRouteName;
        public string RouteName
        {
            get { return mstrRouteName; }
            set { mstrRouteName = value; }
        }

        private string mstrDatumCode;
        public string DatumCode
        {
            get { return mstrDatumCode; }
            set { mstrDatumCode = value; }
        }

        private string mstrLandowner;
        public string Landowner
        {
            get { return mstrLandowner; }
            set { mstrLandowner = value; }
        }

        private string mstrLightLevel;
        public string LightLevel
        {
            get { return mstrLightLevel; }
            set { mstrLightLevel = value; }
        }

        private string mstrSettlementRegion;
        public string SettlementRegion
        {
            get { return mstrSettlementRegion; }
            set { mstrSettlementRegion = value; }
        }

        private string mstrSlopePositionDesc;
        public string SlopePositionDesc
        {
            get { return mstrSlopePositionDesc; }
            set { mstrSlopePositionDesc = value; }
        }

        private string mstrSurfaceExp;
        public string SurfaceExpression
        {
            get { return mstrSurfaceExp; }
            set { mstrSurfaceExp = value; }
        }

        private int? mstrEcoID;
        public int? EcoID
        {
            get { return mstrEcoID; }
            set { mstrEcoID = value; }
        }

        private bool mblnIsLL;
        public bool IsLL
        {
            get { return mblnIsLL; }
            set { mblnIsLL = value; }
        }

        private string sCreatedBy;
        public string CreatedBy
        {
            get { return sCreatedBy; }
            set { sCreatedBy = value; }
        }

        private DateTime sCreateDate;
        public DateTime CreateDate
        {
            get { return sCreateDate; }
            set { sCreateDate = value; }
        }

        private string sUpdatedBy;
        public string UpdatedBy
        {
            get { return sUpdatedBy; }
            set { sUpdatedBy = value; }
        }

        private DateTime sUpdateDate;
        public DateTime UpdateDate
        {
            get { return sUpdateDate; }
            set { sUpdateDate = value; }
        }

        private string sWetlandID;
        public string WetlandID
        {
            get { return sWetlandID; }
            set { sWetlandID = value; }
        }

        private string sWetlandCommunity;
        public string WetlandCommunity
        {
            get { return sWetlandCommunity; }
            set { sWetlandCommunity = value; }
        }

		private int? _ecoLevel1;
		public int? EcoLevel1
		{
			get { return _ecoLevel1; }
			set { _ecoLevel1 = value; }
		}

		private int? _ecoLevel2;
		public int? EcoLevel2
		{
			get { return _ecoLevel2; }
			set { _ecoLevel2 = value; }
		}

		private int? _ecoLevel3;
		public int? EcoLevel3
		{
			get { return _ecoLevel3; }
			set { _ecoLevel3 = value; }
		}

		private int? _provinceId;
		public int? ProvinceId
		{
			get { return _provinceId; }
			set { _provinceId = value; }
		}


        // new properties
        public double? Elevation { get; set; }
        public Guid GUID { get; set; }
        public string ProjectGUID { get; set; }
        

        // adds SiteGUID, ProvinceID, and ProjectGUID to site (and other stuff if site already exists)
        public static void PopulateSite(Site site, string SiteNumber)
        {
            SqlDataReader objDR = GetDBSiteData(SiteNumber);

            while (objDR.Read())
            {
                site.ID = DataAccess.DBNZInt32(objDR, "SiteID");
                site.FieldSourceID = DataAccess.DBNZInt32(objDR, "FieldSourceID");
                site.SiteNumber = DataAccess.DBNZ(objDR["SiteNumber"]);
                site.Easting = DataAccess.DBNZDouble(objDR, "Easting");
                site.Northing = DataAccess.DBNZDouble(objDR, "Northing");
                site.Zone = DataAccess.DBNZInt32(objDR, "Zone");
                site.ProjectNumber = DataAccess.DBNZ(objDR["ProjectNumber"]);
                site.ProjectName = DataAccess.DBNZ(objDR["ProjectName"]);
                site.Comments = DataAccess.DBNZ(objDR["Comments"]);
                site.Aspect = DataAccess.DBNZDouble(objDR, "Aspect");
                site.EstimatedForageProd = DataAccess.DBNZDouble(objDR, "EstimatedForageProduction");
                site.Location = DataAccess.DBNZ(objDR["Location"]);
                site.ProvinceName = DataAccess.DBNZ(objDR["ProvinceName"]);
                site.Slope = DataAccess.DBNZDouble(objDR, "Slope");
                site.RouteName = DataAccess.DBNZ(objDR["RouteName"]);
                site.DatumCode = DataAccess.DBNZ(objDR["DatumCode"]);
                site.Landowner = DataAccess.DBNZ(objDR["Landowner"]);
                site.LightLevel = DataAccess.DBNZ(objDR["LightLevel"]);
                site.SettlementRegion = DataAccess.DBNZ(objDR["SetRegDesc"]);
                site.SlopePositionDesc = DataAccess.DBNZ(objDR["SlopePosDesc"]);
                site.SurfaceExpression = DataAccess.DBNZ(objDR["SurfaceExpID"]);
                site.EcoID = DataAccess.DBNZInt32(objDR, "EcoID");
                site.WetlandID = DataAccess.DBNZ(objDR["WetlandID"]);
                site.WetlandCommunity = DataAccess.DBNZ(objDR["WetlandCommunity"]);
                site.ProvinceName = DataAccess.DBNZ(objDR["ProvinceName"]);
                
                site.CreatedBy = DataAccess.DBNZ(objDR["CreatedBy"]);
                if (objDR["CreateDate"] != DBNull.Value)
                {
                    site.CreateDate = Convert.ToDateTime(DataAccess.DBNZ(objDR["CreateDate"]));
                }

                site.UpdatedBy = DataAccess.DBNZ(objDR["UpdatedBy"]);
                if (objDR["UpdateDate"] != DBNull.Value)
                {
                    site.UpdateDate = Convert.ToDateTime(DataAccess.DBNZ(objDR["UpdateDate"]));
                }

            }
            objDR.Close();
            objDR = null;

            if (string.IsNullOrEmpty(site.ProjectGUID) && !string.IsNullOrEmpty(site.ProjectNumber))
            {
                site.ProjectGUID = GetProjectGUID(site.ProjectNumber);
            }
            if (site.ProvinceId != null && !string.IsNullOrEmpty(site.ProvinceName))
            {
                site.ProvinceId = GetProvinceID(site.ProvinceName);
            }
            if (site.GUID == Guid.Empty)
            {
                site.GUID = NewGUID("SiteGUID");
            }
        }

        public static string GetProjectGUID(string ProjectNumber)
        {
            SqlDataReader objDR = GetDBProjectData(ProjectNumber);
            string GUID = null;

            while (objDR.Read())
            {
                GUID = DataAccess.DBNZ(objDR["ProjectGUID"]);
            }
            objDR.Close();
            objDR = null;
            return GUID;
        }
        public static int? GetProvinceID(string ProvinceName)
        {
            SqlDataReader objDR = GetDBProvinceData(ProvinceName);
            int? GUID = null;

            while (objDR.Read())
            {
                GUID = DataAccess.DBNZInt32(objDR, "ProjectGUID");
            }
            objDR.Close();
            objDR = null;
            return GUID;
        }
        public static Guid NewGUID(string guidColumn)
        {
            SqlDataReader dataReader = GetDBSiteData();
            int guid = dataReader.GetOrdinal(guidColumn);

            while (dataReader.Read())
            {
                if (dataReader.IsDBNull(guid))
                {
                    Console.WriteLine("aw");
                }
            }

            //if (dataReader.IsDBNull(dataReader.GetOrdinal(guidColumn)))
            //{
            //    return default(Guid);
            //}
            //else
            //{
            //    return dataReader.GetGuid(dataReader.GetOrdinal(guidColumn));
            //}
            dataReader.Close();
            dataReader = null;
            return Guid.Empty;
        }

        public static SqlDataReader GetDBSiteData(string SiteNumber="")
        {
            // connect to vegsys
            DataAccess objDA = new DataAccess(DataAccess.PROD_CONN_STRING);

            //build query
            StringBuilder strSQL = new StringBuilder();

            strSQL.AppendLine("SELECT")
                    .AppendLine("dbo.V_Site.SiteID,")
                    .AppendLine("dbo.V_Site.SiteGUID,")
                    .AppendLine("dbo.V_Site.SiteNumber,")
                    .AppendLine("dbo.V_Site.Location,")
                    .AppendLine("dbo.V_Site.Easting,")
                    .AppendLine("dbo.V_Site.Northing,")
                    .AppendLine("dbo.V_Site.Zone,")
                    .AppendLine("dbo.V_Site.Slope,")
                    .AppendLine("dbo.V_Site.SlopePosID,")
                    .AppendLine("dbo.V_Site.Aspect,")
                    .AppendLine("dbo.V_Site.LightLvlID,")
                    .AppendLine("dbo.V_Site.Comments,")
                    .AppendLine("dbo.V_Site.EcoID,")
                    .AppendLine("dbo.V_Site.SettlementRegID,")
                    .AppendLine("dbo.V_Site.RouteID,")
                    .AppendLine("dbo.V_Site.SurfaceExpID,")
                    .AppendLine("dbo.V_Site.DatumCode,")
                    .AppendLine("dbo.V_Site.CreatedBy,")
                    .AppendLine("dbo.V_Site.CreateDate,")
                    .AppendLine("dbo.V_Site.ProvinceID,")
                    .AppendLine("dbo.V_Site.IsLL,")
                    .AppendLine("dbo.V_Site.UpdatedBy,")
                    .AppendLine("dbo.V_Site.UpdateDate,")
                    .AppendLine("dbo.V_Site.LandOwner,")
                    .AppendLine("dbo.V_Site.LocationDescription,")
                    .AppendLine("dbo.V_Site.HabitatThreat,")
                    .AppendLine("dbo.V_Site.EstimatedForageProduction,")
                    .AppendLine("dbo.V_Site.WetlandIDOrig,")
                    .AppendLine("dbo.V_Site.WetlandCommunity,")
                    .AppendLine("dbo.V_Site.QAed,")
                    .AppendLine("dbo.V_Site.Lon,")
                    .AppendLine("dbo.V_Site.Lat,")
                    .AppendLine("dbo.V_Site.WetlandID,")
                    .AppendLine("dbo.V_Site.IsParent,")
                    .AppendLine("dbo.V_Site.Elevation,")
                    .AppendLine("dbo.V_Site.PctWetlandVisited,")
                    .AppendLine("dbo.V_Site.PctAAVisited,")

                    .AppendLine("dbo.V_Project.FieldSourceID,")
                    .AppendLine("dbo.V_Project.ProjectName,")
                    .AppendLine("dbo.V_Project.ProjectNumber")
                .AppendLine("FROM")
                    .AppendLine("dbo.V_ProjectSite INNER JOIN")
                    .AppendLine("dbo.V_Project ON dbo.V_ProjectSite.FieldSourceID = dbo.V_Project.FieldSourceID INNER JOIN")
                    .AppendLine("dbo.V_Site ON dbo.V_ProjectSite.SiteID = dbo.V_Site.SiteID");

            if (!string.IsNullOrWhiteSpace(SiteNumber)) strSQL.Append(string.Format("WHERE V_Site.SiteNumber LIKE '%{0}%'", SiteNumber));


            string result = strSQL.ToString();
            //execute query
            return objDA.ExecuteSelectSQL(strSQL.ToString(), false);
        }
        public static SqlDataReader GetDBProjectData(string ProjectNumber)
        {
            // connect to vegsys
            DataAccess objDA = new DataAccess(DataAccess.STAGE_CONN_STRING);

            //build query
            StringBuilder strSQL = new StringBuilder();

            strSQL.AppendLine("SELECT")
                    .AppendLine("dbo.V_Project.ProjectName,")
                    .AppendLine("dbo.V_Project.ProjectNumber,")
                    .AppendLine("dbo.V_Project.ProjectGUID")
                .AppendLine("FROM")
                    .AppendLine("dbo.V_Project")
                .AppendLine(string.Format("WHERE ProjectNumber = '{0}'", ProjectNumber));

            //execute and return query result
            return objDA.ExecuteSelectSQL(strSQL.ToString(), false);
        }
        public static SqlDataReader GetDBProvinceData(string ProvinceName)
        {
            // connect to vegsys
            DataAccess objDA = new DataAccess(DataAccess.STAGE_CONN_STRING);

            //build query
            StringBuilder strSQL = new StringBuilder();

            strSQL.AppendLine("SELECT")
                    .AppendLine("ProvinceID,")
                    .AppendLine("ProvinceName,")
                    .AppendLine("ProvinceCode")
                .AppendLine("FROM")
                    .AppendLine("dbo.v_ProvinceLU")
                .AppendLine(string.Format("WHERE ProvinceName = '{0}'", ProvinceName));

            //execute and return query result
            return objDA.ExecuteSelectSQL(strSQL.ToString(), false);
        }


        public void SaveToDatabase(bool CreateNew, User currentUser, ref string SiteID)
        {
            DataAccess objDA = new DataAccess(DataAccess.DEV_CONN_STRING);

            objDA.DBCommand.Parameters.Add(new SqlParameter("@FieldSourceID", this.FieldSourceID));
            objDA.DBCommand.Parameters.Add(new SqlParameter("@SiteNumber", this.SiteNumber));
            objDA.DBCommand.Parameters.Add(new SqlParameter("@Location", this.Location));
            objDA.DBCommand.Parameters.Add(new SqlParameter("@DatumCode", this.DatumCode));
            objDA.DBCommand.Parameters.Add(new SqlParameter("@SlopePos", this.SlopePositionDesc));
            objDA.DBCommand.Parameters.Add(new SqlParameter("@LandOwner", this.Landowner));
            objDA.DBCommand.Parameters.Add(new SqlParameter("@Comment", this.Comments));
            objDA.DBCommand.Parameters.Add(new SqlParameter("@ProvinceName", this.ProvinceName));
            objDA.DBCommand.Parameters.Add(new SqlParameter("@RouteName", this.RouteName));
            objDA.DBCommand.Parameters.Add(new SqlParameter("@SettlementRegDesc", this.SettlementRegion));
            objDA.DBCommand.Parameters.Add(new SqlParameter("@SurfaceExpression", this.SurfaceExpression));
            objDA.DBCommand.Parameters.Add(new SqlParameter("@IsLL", this.IsLL));
            objDA.DBCommand.Parameters.Add(new SqlParameter("@WetlandID", this.WetlandID));
            objDA.DBCommand.Parameters.Add(new SqlParameter("@WetlandCommunity", this.WetlandCommunity));

            int? intLightLevelID = GetLightLevelID(this.LightLevel);
            if (intLightLevelID == null)
            {
                objDA.DBCommand.Parameters.Add(new SqlParameter("@LightLvlID", System.DBNull.Value));
            }
            else
            {
                objDA.DBCommand.Parameters.Add(new SqlParameter("@LightLvlID", intLightLevelID));
            }

            if (this.Easting == null)
            {
                objDA.DBCommand.Parameters.Add(new SqlParameter("@Easting", System.DBNull.Value));
            }
            else
            {
                objDA.DBCommand.Parameters.Add(new SqlParameter("@Easting", this.Easting));
            }

            if (this.Northing == null)
            {
                objDA.DBCommand.Parameters.Add(new SqlParameter("@Northing", System.DBNull.Value));
            }
            else
            {
                objDA.DBCommand.Parameters.Add(new SqlParameter("@Northing", this.Northing));
            }

            if (this.Zone == null)
            {
                objDA.DBCommand.Parameters.Add(new SqlParameter("@Zone", System.DBNull.Value));
            }
            else
            {
                objDA.DBCommand.Parameters.Add(new SqlParameter("@Zone", this.Zone));
            }

            if (this.Aspect == null)
            {
                objDA.DBCommand.Parameters.Add(new SqlParameter("@Aspect", System.DBNull.Value));
            }
            else
            {
                objDA.DBCommand.Parameters.Add(new SqlParameter("@Aspect", this.Aspect));
            }

            if (this.Slope == null)
            {
                objDA.DBCommand.Parameters.Add(new SqlParameter("@Slope", System.DBNull.Value));
            }
            else
            {
                objDA.DBCommand.Parameters.Add(new SqlParameter("@Slope", this.Slope));
            }

            if (this.EstimatedForageProd == null)
            {
                objDA.DBCommand.Parameters.Add(new SqlParameter("@EstimatedForageProduction", System.DBNull.Value));
            }
            else
            {
                objDA.DBCommand.Parameters.Add(new SqlParameter("@EstimatedForageProduction", this.EstimatedForageProd));
            }

            if (this.EcoID == null)
            {
                objDA.DBCommand.Parameters.Add(new SqlParameter("@EcoID", System.DBNull.Value));
            }
            else
            {
                objDA.DBCommand.Parameters.Add(new SqlParameter("@EcoID", this.EcoID));
            }

            if (CreateNew)
            {
                
                objDA.DBCommand.Parameters.Add(new SqlParameter("@CreatedBy", currentUser.Username));
                objDA.DBCommand.Parameters.Add(new SqlParameter("@CreateDate", DateTime.Now));

                SqlParameter objSiteID = new SqlParameter();
                objSiteID.ParameterName = "@SiteID";
                objSiteID.Value = 1;
                objSiteID.Direction = ParameterDirection.Output;
                objDA.DBCommand.Parameters.Add(objSiteID);

                objDA.Open();
                objDA.DBCommand.CommandType = CommandType.
                objDA.DBCommand.CommandText = "V_SP_ADDNEWSITE";
                objDA.DBCommand.ExecuteNonQuery();
                SiteID = objDA.DBCommand.Parameters["@SiteID"].Value.ToString();
                objDA.Close();
                objDA = null;

            }

            else
            {
                objDA.DBCommand.Parameters.Add(new SqlParameter("@SiteID", SiteID));

                objDA.Open();
                objDA.DBCommand.CommandType = CommandType.StoredProcedure;
                objDA.DBCommand.CommandText = "V_SP_UPDATESITE";
                objDA.DBCommand.Parameters.Add(new SqlParameter("@UpdatedBy", currentUser.Username));
                objDA.DBCommand.Parameters.Add(new SqlParameter("@UpdateDate", DateTime.Now));
                objDA.DBCommand.ExecuteNonQuery();
            }
            
        }

        protected int? GetLightLevelID(string LightLevel)
        {
            int? id;
            id = (int)DataAccess.GetIntField("LightLvlID", "V_LIGHTLEVELLU", "WHERE LIGHTLEVEL = '" + LightLevel + "'");
            return id;
        }

        public static bool DeleteFromDatabase(string SiteID)
        {
            try
            {
                DataAccess objDA = new DataAccess(DataAccess.GetConnectionString());
                StringBuilder strSQL = new StringBuilder();
                objDA.DBCommand.Parameters.Add("@SiteID", SqlDbType.Int).Value = Convert.ToInt32(SiteID);
                strSQL.Append("DELETE FROM V_PROJECTSITE WHERE SiteID = @SiteID");
                objDA.ExecuteInsertOrUpdateSQL(strSQL.ToString(), false);

                strSQL = new System.Text.StringBuilder();
                strSQL.Append("DELETE FROM V_SITE WHERE SiteID = @SiteID");
                objDA = new DataAccess(DataAccess.GetConnectionString());
                objDA.DBCommand.Parameters.Add("@SiteID", SqlDbType.Int).Value = Convert.ToInt32(SiteID);
                objDA.ExecuteInsertOrUpdateSQL(strSQL.ToString(), false);

            }
            catch (Exception)
            {
                return false;
            }

            return true;

        }

        public static bool ContainsSurveys(string SiteID)
        {
            DataAccess mobjDA = new DataAccess(DataAccess.GetConnectionString());
            bool retVal;
            retVal = mobjDA.RecordExists("V_SITESURVEY", "SiteSurveyID", "WHERE SiteID = " + SiteID);
            mobjDA.Close();
            mobjDA = null;
            return retVal;
        }


    }

}
