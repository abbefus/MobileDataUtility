using System;
using System.Data;
using System.Linq;

namespace VegsysManager.Classes
{
    class Site
    {
        public static Site Create(DataRow row)
        {
            Type dffType = typeof(Site);
            string[] columns = dffType.GetProperties().Select(x => x.Name).ToArray();

            Site site = new Site();
            foreach (string column in columns)
            {
                Type t = dffType.GetProperty(column).PropertyType;
                dffType.GetProperty(column).SetValue
                (
                    site, 
                    SqlCeConversion.CheckDBNull(row[column], t)
                );
            }
            return site;
        } //creates a Site from an sqlce or (maybe also) sql database row

        public Guid SiteGUID { get; set; }
        public string SiteNumber { get; set; }
        public string Location { get; set; }
        public double? Easting { get; set; }
        public double? Northing { get; set; }
        public Int32? Zone { get; set; }
        public string DatumCode { get; set; }
        public double? Lon { get; set; }
        public double? Lat { get; set; }
        public double? Slope { get; set; }
        public Int16? SlopePosID { get; set; }
        public double? Aspect { get; set; }
        public Int32? LightLvlID { get; set; }
        public Int16? ProvinceID { get; set; }
        public Int32? EcoID { get; set; }
        public Int32? SettlementRegID { get; set; }
        public string LandOwner { get; set; }
        public Int32? RouteID { get; set; }
        public Int32? SurfaceExpID { get; set; }
        public string LocationDescription { get; set; }
        public string HabitatThreat { get; set; }
        public double? EstimatedForageProduction { get; set; }
        public string WetlandID { get; set; }
        public string WetlandCommunity { get; set; }
        public string Comments { get; set; }
        public bool QAed { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public double? HDOP { get; set; }
        public double? PDOP { get; set; }
        public double? VDOP { get; set; }
        public Int32? Satellites { get; set; }
        public Int32? FixStatus { get; set; }
        public bool? IsParent { get; set; }
        public bool? Transferred { get; set; }
        public double? Elevation { get; set; }
        public double? PctWetlandVisited { get; set; }
        public double? PctAAVisited { get; set; }
    }
}
