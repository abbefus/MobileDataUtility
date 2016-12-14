using ArchyManager.Pages;
using System;
using System.ComponentModel;

namespace ArchyManager.Classes.Archy2014
{
    class CMT : IUploadable
    {
        [Browsable(false)]
        public string DefaultGuid { get { return "CMTGuid"; } }
        [Browsable(false)]

        public bool IsUploaded { get; set; }
        public Guid CMTGuid { get; set; }
        public string CMTNumber { get; set; }
        public DateTime SurveyDate { get; set; }
        public string Recorder { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public double? Easting { get; set; }
        public double? Northing { get; set; }
        public byte? UTMZone { get; set; }
        public double? HDOP { get; set; }
        public double? PDOP { get; set; }
        public double? VDOP { get; set; }
        public byte? SatelliteCount { get; set; }
        public string SatelliteFix { get; set; }
        public double? Elevation { get; set; }
        public byte? SpeciesID { get; set; }
        public byte? StatusID { get; set; }
        public double? PctSlope { get; set; }
        public double? DBH { get; set; }
        public bool? Core { get; set; }
        public Int16? ScarAge { get; set; }
        public Int16? TreeAge { get; set; }
        public byte? CMTClassID { get; set; }
        public Int16? ProjectID { get; set; }
        public Guid? ArchSiteGuid { get; set; }
        public string CMTNote { get; set; }
        public byte? NTSpeciesID { get; set; }
        public byte? Pre1846ID { get; set; }
    }
}
