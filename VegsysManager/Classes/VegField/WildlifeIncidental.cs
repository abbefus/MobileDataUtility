using System;
using System.Data;
using System.Linq;

namespace VegsysManager.Classes
{
    class WildlifeIncidental
    {
        public static WildlifeIncidental Create(DataRow row)
        {
            Type dffType = typeof(WildlifeIncidental);
            string[] columns = dffType.GetProperties().Select(x => x.Name).ToArray();

            WildlifeIncidental wildlifeincidental = new WildlifeIncidental();
            foreach (string column in columns)
            {
                Type t = dffType.GetProperty(column).PropertyType;
                dffType.GetProperty(column).SetValue
                (
                    wildlifeincidental, 
                    SqlCeConversion.CheckDBNull(row[column], t)
                );
            }
            return wildlifeincidental;
        } //creates a WildlifeIncidental from an sqlce or (maybe also) sql database row

        public Int32 ProjectID { get; set; }
        public byte? ProvinceID { get; set; }
        public Int32? SpeciesID { get; set; }
        public Int32? NumberObserved { get; set; }
        public string Age { get; set; }
        public string Sex { get; set; }
        public byte? ObservationTypeID { get; set; }
        public byte? FeatureTypeID { get; set; }
        public DateTime? ObservationDate { get; set; }
        public string Biologist { get; set; }
        public string ObservationID { get; set; }
        public Int32? Easting { get; set; }
        public Int32? Northing { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public Int32? Distance { get; set; }
        public Int32? Bearing { get; set; }
        public Int32? ProjectedEasting { get; set; }
        public Int32? ProjectedNorthing { get; set; }
        public double? ProjectedLongitude { get; set; }
        public double? ProjectedLatitude { get; set; }
        public Int32? UTMZone { get; set; }
        public string Mapsheet { get; set; }
        public string QuarterSection { get; set; }
        public string Section { get; set; }
        public string Township { get; set; }
        public string Range { get; set; }
        public string Meridian { get; set; }
        public string Notes { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string QAedBy { get; set; }
        public DateTime? QADate { get; set; }
        public bool? QAed { get; set; }
        public Guid WLObsGuid { get; set; }
    }
}
