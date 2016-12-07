using ArchyManager;
using System;
using System.Data;
using System.Linq;

namespace ArchyManager.Classes.Archy2014
{
    public class ShovelTestPit
    {
        public Int16? ProjectID { get; set; }
        public string Recorder { get; set; }
        public DateTime? SurveyDate { get; set; }
        public string Excavator { get; set; }
        public string PitID { get; set; }
        public float? DatumDistance1 { get; set; }
        public string DatumDirection1 { get; set; }
        public float? DatumDistance2 { get; set; }
        public string DatumDirection2 { get; set; }
        public Int32? PermitNumber { get; set; }
        public string ArtefactsCollected { get; set; }
        public Int32? ArtefactCount { get; set; }
        public string STPNote { get; set; }
        public float? Longitude { get; set; }
        public float? Latitude { get; set; }
        public float? Easting { get; set; }
        public float? Northing { get; set; }
        public byte? UTMZone { get; set; }
        public float? HDOP { get; set; }
        public float? PDOP { get; set; }
        public float? VDOP { get; set; }
        public byte? SatelliteCount { get; set; }
        public string SatelliteFix { get; set; }
        public Guid STPGuid { get; set; }
        public bool? StruckWater { get; set; }
        public float? Elevation { get; set; }
        public Int32? PolygonID { get; set; }
        public Guid ArchSiteGuid { get; set; }
        public string PhotoFrom { get; set; }
        public string PhotoTo { get; set; }
        public string CameraNumber { get; set; }
        public byte? PitToolID { get; set; }
        public Guid DatumGuid { get; set; }
        public float? DatumBearing1 { get; set; }
        public float? DatumBearing2 { get; set; }
    }
}
