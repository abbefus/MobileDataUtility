using ArchyManager.Pages;
using System;
using System.ComponentModel;

namespace ArchyManager.Classes.Archy2014
{
    class Datum : IUploadable
    {
        [Browsable(false)]
        public string DefaultGuid { get { return "DatumGuid"; } }
        [Browsable(false)]
        public bool IsUploaded { get; set; }

        public string DatumID { get; set; }
        public string FeatureID { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public double? Easting { get; set; }
        public double? Northing { get; set; }
        public byte? UTMZone { get; set; }
        public double? Elevation { get; set; }
        public Guid? DatumGuid { get; set; }
        public Guid? ArchSiteGuid { get; set; }
        public Guid? SourceFeatureGuid { get; set; }
    }
}
