using ArchyManager.Pages;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace ArchyManager.Classes.Archy2014
{
    public class ShovelTestPit : IUploadable
    {
        // outside schema
        [Browsable(false)]
        public string DefaultGuid { get { return "STPGuid"; } }
        [Browsable(false)]
        public bool IsUploaded { get; set; }

        // inside schema
        [Browsable(true)]
        public Int16 ProjectID { get; set; }
        [Browsable(true)]
        public string Recorder { get; set; }
        [Browsable(true)]
        public DateTime SurveyDate { get; set; }
        [Browsable(true)]
        public string Excavator { get; set; }
        [Browsable(true)]
        public string PitID { get; set; }
        [Browsable(true)]
        public double? DatumDistance1 { get; set; }
        [Browsable(true)]
        public string DatumDirection1 { get; set; }
        [Browsable(true)]
        public double? DatumDistance2 { get; set; }
        [Browsable(true)]
        public string DatumDirection2 { get; set; }
        [Browsable(true)]
        public Int32? PermitNumber { get; set; } //actually is PermitID, Matt >=/
        [Browsable(true)]
        public string ArtefactsCollected { get; set; }
        [Browsable(true)]
        public Int32? ArtefactCount { get; set; }
        [Browsable(true)]
        public string STPNote { get; set; }
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
        public double? HDOP { get; set; }
        [Browsable(true)]
        public double? PDOP { get; set; }
        [Browsable(true)]
        public double? VDOP { get; set; }
        [Browsable(true)]
        public byte? SatelliteCount { get; set; }
        [Browsable(true)]
        public string SatelliteFix { get; set; }
        [Browsable(true)]
        public Guid STPGuid { get; set; }
        [Browsable(true)]
        public bool? StruckWater { get; set; }
        [Browsable(true)]
        public double? Elevation { get; set; }
        [Browsable(true)]
        public Int32? PolygonID { get; set; }
        [Browsable(true)]
        public Guid ArchSiteGuid { get; set; }
        [Browsable(true)]
        public string PhotoFrom { get; set; }
        [Browsable(true)]
        public string PhotoTo { get; set; }
        [Browsable(true)]
        public string CameraNumber { get; set; }
        [Browsable(true)]
        public byte? PitToolID { get; set; }
        [Browsable(true)]
        public Guid DatumGuid { get; set; }
        [Browsable(true)]
        public double? DatumBearing1 { get; set; }
        [Browsable(true)]
        public double? DatumBearing2 { get; set; }

        public static void SetBrowsableDefaults(string[] exclusions = null)
        {
            string[] fixedexclusions = new string[] { "DefaultGuid", "IsUploaded" };
            exclusions = exclusions != null ? exclusions.Concat(fixedexclusions).ToArray() : fixedexclusions;
            foreach (PropertyInfo property in typeof(ShovelTestPit).GetProperties())
            {
                if (exclusions.Contains(property.Name))
                {
                    SqlUtils.SetPropertyBrowsable(typeof(ShovelTestPit), property.Name, false);
                }
                else
                {
                    SqlUtils.SetPropertyBrowsable(typeof(ShovelTestPit), property.Name, true);
                }
            }
        }
    }
    public class ShovelTestPitExtended : ShovelTestPit
    {
        // site
        [Browsable(true)]
        public string SiteNumber { get; set; }

        //datum
        [Browsable(true)]
        public string DatumID { get; set; }

        //project
        [Browsable(true)]
        public string ProjectNumber { get; set; }

        //permit
        [Browsable(true)]
        public string Permit { get; set; }

        //lookup
        [Browsable(true)]
        public string PitTool { get; set; }

        new public static void SetBrowsableDefaults(string[] exclusions = null)
        {
            string[] fixedexclusions = new string[] { "DefaultGuid", "IsUploaded" };
            exclusions = exclusions != null ? exclusions.Concat(fixedexclusions).ToArray() : fixedexclusions;
            foreach (PropertyInfo property in typeof(ShovelTestPitExtended).GetProperties())
            {
                if (exclusions.Contains(property.Name))
                {
                    SqlUtils.SetPropertyBrowsable(typeof(ShovelTestPitExtended), property.Name, false);
                }
                else
                {
                    SqlUtils.SetPropertyBrowsable(typeof(ShovelTestPitExtended), property.Name, true);
                }
            }
        }

        public ShovelTestPit ToShovelTestPit()
        {
            return new ShovelTestPit
            {
                ArchSiteGuid = ArchSiteGuid,
                ArtefactsCollected = ArtefactsCollected,
                ArtefactCount = ArtefactCount,
                CameraNumber = CameraNumber,
                DatumBearing1 = DatumBearing1,
                DatumBearing2 = DatumBearing2,
                DatumDirection1 = DatumDirection1,
                DatumDirection2 = DatumDirection2,
                DatumDistance1 = DatumDistance1,
                DatumDistance2 = DatumDistance2,
                DatumGuid = DatumGuid,
                Easting = Easting,
                Elevation = Elevation,
                Excavator = Excavator,
                HDOP = HDOP,
                Latitude = Latitude,
                Longitude = Longitude,
                Northing = Northing,
                PDOP = PDOP,
                PermitNumber = PermitNumber,
                PhotoFrom = PhotoFrom,
                PhotoTo = PhotoTo,
                PitID = PitID,
                PitToolID = PitToolID,
                PolygonID = PolygonID,
                ProjectID = ProjectID,
                Recorder = Recorder,
                SatelliteCount = SatelliteCount,
                SatelliteFix = SatelliteFix,
                STPGuid = STPGuid,
                STPNote = STPNote,
                StruckWater = StruckWater,
                SurveyDate = SurveyDate,
                UTMZone = UTMZone,
                VDOP = VDOP,
                IsUploaded = IsUploaded
            };
        }

    }
    
}
