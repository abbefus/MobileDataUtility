using ArchyManager.Pages;
using System;
using System.ComponentModel;

namespace ArchyManager.Classes.Archy2014
{
    public class ShovelTestPit : IUploadable
    {
        [Browsable(false)]
        public string DefaultGuid { get { return "STPGuid"; } }
        [Browsable(false)]
        public bool IsUploaded { get; set; }

        public Int16 ProjectID { get; set; }
        public string Recorder { get; set; }
        public DateTime SurveyDate { get; set; }
        public string Excavator { get; set; }
        public string PitID { get; set; }
        public double? DatumDistance1 { get; set; }
        public string DatumDirection1 { get; set; }
        public double? DatumDistance2 { get; set; }
        public string DatumDirection2 { get; set; }
        public Int32? PermitNumber { get; set; }
        public string ArtefactsCollected { get; set; }
        public Int32? ArtefactCount { get; set; }
        public string STPNote { get; set; }
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
        public Guid STPGuid { get; set; }
        public bool? StruckWater { get; set; }
        public double? Elevation { get; set; }
        public Int32? PolygonID { get; set; }
        public Guid ArchSiteGuid { get; set; }
        public string PhotoFrom { get; set; }
        public string PhotoTo { get; set; }
        public string CameraNumber { get; set; }
        public byte? PitToolID { get; set; }
        public Guid DatumGuid { get; set; }
        public double? DatumBearing1 { get; set; }
        public double? DatumBearing2 { get; set; }
    }
}
