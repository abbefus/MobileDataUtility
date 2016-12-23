using ArchyManager.Pages;
using System;
using System.ComponentModel;

namespace ArchyManager.Classes.Archy2014
{
    public class Datum : IUploadable
    {
        [Browsable(false)]
        public string DefaultGuid { get { return "DatumGuid"; } }
        [Browsable(false)]
        public bool IsUploaded { get; set; }

        [Browsable(true)]
        public string DatumID { get; set; }
        [Browsable(true)]
        public string FeatureID { get; set; }
        [Browsable(true)]
        public double? Longitude { get; set; }
        [Browsable(true)]
        public double? Latitude { get; set; }
        [Browsable(true)]
        public double? Easting { get; set; }
        [Browsable(true)]
        public double? Northing { get; set; }
        [Browsable(true)]
        public byte? UTMZone { get; set; }
        [Browsable(true)]
        public double? Elevation { get; set; }
        [Browsable(true)]
        public Guid? DatumGuid { get; set; }
        [Browsable(true)]
        public Guid? ArchSiteGuid { get; set; }
        [Browsable(true)]
        public Guid? SourceFeatureGuid { get; set; }
    }
}
