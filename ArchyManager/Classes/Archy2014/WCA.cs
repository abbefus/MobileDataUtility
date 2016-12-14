using ArchyManager.Pages;
using System;
using System.ComponentModel;

namespace ArchyManager.Classes.Archy2014
{
    class WCA : IUploadable
    {
        [Browsable(false)]
        public string DefaultGuid { get { return "WCAGuid"; } }
        [Browsable(false)]
        public bool IsUploaded { get; set; }

        public Int16? ProjectID { get; set; }
        public string Recorder { get; set; }
        public DateTime? SurveyDate { get; set; }
        public Int32? PermitID { get; set; }
        public string WCANumber { get; set; }
        public string AquaticsWCNumber { get; set; }
        public double? Width { get; set; }
        public double? Depth { get; set; }
        public double? BankHeight { get; set; }
        public string Substrate { get; set; }
        public string WCANote { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public double? Easting { get; set; }
        public double? Northing { get; set; }
        public byte? UTMZone { get; set; }
        public double? Elevation { get; set; }
        public double? HDOP { get; set; }
        public double? PDOP { get; set; }
        public double? VDOP { get; set; }
        public byte? SatelliteCount { get; set; }
        public string SatelliteFix { get; set; }
        public string PhotoFrom { get; set; }
        public string PhotoTo { get; set; }
        public string CameraNumber { get; set; }
        public Guid? WCAGuid { get; set; }
        public string Substrate2 { get; set; }
        public string BankCharacter { get; set; }
        public string BankCharacterNote { get; set; }
    }
}
