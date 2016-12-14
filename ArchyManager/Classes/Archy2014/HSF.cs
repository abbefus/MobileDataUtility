using ArchyManager.Pages;
using System;
using System.ComponentModel;

namespace ArchyManager.Classes.Archy2014
{
    class HSF : IUploadable
    {
        [Browsable(false)]
        public string DefaultGuid { get { return "HSFGuid"; } }
        [Browsable(false)]

        public bool IsUploaded { get; set; }
        public Int16 ProjectID { get; set; }
        public Int32? PermitID { get; set; }
        public string Recorder { get; set; }
        public string BordenNumber { get; set; }
        public string FeatureNumber { get; set; }
        public DateTime? SurveyDate { get; set; }
        public string FeaturePortion { get; set; }
        public string Location { get; set; }
        public double? DatumDistance { get; set; }
        public string DatumDirection { get; set; }
        public double? FeatureLength { get; set; }
        public string MajorOrientation { get; set; }
        public double? FeatureWidth { get; set; }
        public string MinorOrientation { get; set; }
        public double? FeatureZ { get; set; }
        public double? FeatureDiameter { get; set; }
        public string Shape { get; set; }
        public string PreservationState { get; set; }
        public string FeatureNote { get; set; }
        public byte? ClassID { get; set; }
        public byte? TypeID { get; set; }
        public byte? SubtypeID { get; set; }
        public Int16? DescriptorID { get; set; }
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
        public string PhotoFrom { get; set; }
        public string PhotoTo { get; set; }
        public string CameraNumber { get; set; }
        public Guid? HSFGuid { get; set; }
        public Guid? ArchSiteGuid { get; set; }
        public string OtherDetail { get; set; }
        public Guid? DatumGuid { get; set; }
        public bool? IsSiteCharacterization { get; set; }
        public string TempNumber { get; set; }
    }
}
