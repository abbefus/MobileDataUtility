using System;
using System.Data;
using System.Linq;

namespace VegsysManager.Classes
{
    class SiteSurvey
    {
        public static SiteSurvey Create(DataRow row)
        {
            Type dffType = typeof(SiteSurvey);
            string[] columns = dffType.GetProperties().Select(x => x.Name).ToArray();

            SiteSurvey sitesurvey = new SiteSurvey();
            foreach (string column in columns)
            {
                Type t = dffType.GetProperty(column).PropertyType;
                dffType.GetProperty(column).SetValue
                (
                    sitesurvey, 
                    SqlCeConversion.CheckDBNull(row[column],t)
                );
                if (column == "Sketch" && sitesurvey.Sketch == null)
                {
                    sitesurvey.Sketch = new byte[0];
                }
            }
            return sitesurvey;
        } //creates a SiteSurvey from an sqlce or (maybe also) sql database row

        public Guid SiteSurveyGuid { get; set; }
        public Guid ProjectGuid { get; set; }
        public Guid SiteGuid { get; set; }
        public bool? WildlifeObserved { get; set; }
        public DateTime? SurveyDate { get; set; }
        public string SurveyComments { get; set; }
        public Int16? TotalCanopy { get; set; }
        public Int16? TotalSubCanopy { get; set; }
        public Int16? TotalTallShrub { get; set; }
        public Int16? TotalShortShrub { get; set; }
        public bool? MatureTimber { get; set; }
        public Int32? ArborealLichenCover { get; set; }
        public Int32? TerrestrialLichenCover { get; set; }
        public string Observers { get; set; }
        public string StrucStageCode { get; set; }
        public Int32? MoistureID { get; set; }
        public Int32? NutrientID { get; set; }
        public string PermafrostDepth { get; set; }
        public string DepthtoWater { get; set; }        //-----inconsistent capitalization Matt!!!
        public string WtrChemComment { get; set; }
        public double? pH { get; set; }
        public Int16? pHTypeID { get; set; }
        public double? Conductivity { get; set; }
        public double? WtrTemperature { get; set; }
        public Int32? SurveyTypeID { get; set; }
        public string ObservedVegTypeFit { get; set; }
        public Int32? CrownClosurePct { get; set; }
        public Int32? NumofPlot { get; set; }       //-----inconsistent capitalization Matt!!!
        public Int32? LandUseID { get; set; }
        public Int16? WetLdModID { get; set; }
        public Int32? VegID { get; set; }
        public Int32? VegID2 { get; set; }
        public Int32? VegID3 { get; set; }
        public string VegTypeModID { get; set; }
        public Int32? AWIID { get; set; }
        public string WetldSubClsCode { get; set; }
        public Int16? SuccStatID { get; set; }
        public double? Elevation { get; set; }
        public Int32? CommunityTypeID { get; set; }
        public string WetlandClassSystem { get; set; }
        public string WetlandZoneLoc{ get; set; }
        public string WetlandZoneType { get; set; }
        public string WetlandPhase { get; set; }
        public string WetlandClass { get; set; }
        public bool? BryophyteSample { get; set; }
        public string Createdby { get; set; }       //-----inconsistent capitalization Matt!!!
        public DateTime? CreateDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string QCedBy { get; set; }
        public DateTime? QCDate { get; set; }
        public string QCStatus { get; set; }
        public bool? LichenSample { get; set; }
        public double? DepthToSaturation { get; set; }
        public Int32? CowardinSystemID { get; set; }
        public Int32? CowardinSubsystemID { get; set; }
        public Int32? CowardinClassID { get; set; }
        public byte[] Sketch { get; set; }
        public string SurveyName { get; set; }
        public Int32? WetlandCoverType { get; set; }
        public double? Easting { get; set; }
        public double? Northing { get; set; }
        public byte? Zone { get; set; }
        public string Datum { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public Single? HDOP { get; set; }
        public Single? PDOP { get; set; }
        public Single? VDOP { get; set; }
        public byte? Satellites { get; set; }
        public byte? GPSFix { get; set; }
        public bool Transferred { get; set; }
        public byte? AWCSClassID { get; set; }
        public byte? AWCSFormID { get; set; }
        public byte? AWCSTypeID { get; set; }
    }
}
