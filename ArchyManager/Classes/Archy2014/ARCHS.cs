using ArchyManager.Pages;
using System;
using System.ComponentModel;

namespace ArchyManager.Classes.Archy2014
{
    class ARCHS : IUploadable
    {
        [Browsable(false)]
        public string DefaultGuid { get { return "ARCHSGuid"; } }
        [Browsable(false)]
        public bool IsUploaded { get; set; }

        public Int16 ProjectID { get; set; }
        public Int32 PermitID { get; set; }
        public DateTime SurveyDate { get; set; }
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
        public Guid ARCHSGuid { get; set; }
        public float? Elevation { get; set; }
        public string SiteNumber { get; set; }
        public string Potential { get; set; }
        public string PotentialArea { get; set; }
        public string AquaticsCrossingNumber { get; set; }
        public string PotentialNotes { get; set; }
        public string Slope { get; set; }
        public string GroundCondition { get; set; }
        public string TerrainFeature1 { get; set; }
        public string TerrainFeature2 { get; set; }
        public string TerrainNote { get; set; }
        public string LithicMaterial { get; set; }
        public string SurfaceType1 { get; set; }
        public string SurfaceType2 { get; set; }
        public string SurfaceNote { get; set; }
        public string Recorder { get; set; }
        public byte? TestingPercent { get; set; }
        public string PhotoFrom { get; set; }
        public string PhotoTo { get; set; }
        public string CameraNumber { get; set; }
        public string SoilType2 { get; set; }
        public string SoilType1 { get; set; }
        public float? TransectSpacing { get; set; }
        public string Exposure { get; set; }
        public string DisturbanceTimeframe { get; set; }
        public string DisturbanceCause { get; set; }
        public byte? DisturbancePct { get; set; }
        public string DisturbanceNote { get; set; }
        public string DisturbanceExposure { get; set; }
        public string CulturalMaterial { get; set; }
        public string OtherCulturalMaterial { get; set; }
        public string Canopy { get; set; }
        public string Understory { get; set; }
        public string GroundCover { get; set; }
        public bool PotentialRarePlant { get; set; }
        public string TerrainInfo1 { get; set; }
        public string TerrainInfo2 { get; set; }
        public string TerrainInfo3 { get; set; }
        public float? TerrainFeatureLength { get; set; }
        public float? TerrainFeatureWitdh { get; set; }
        public string TerrainFeatureOrientation { get; set; }
        public string SoilExposure { get; set; }
        public string PotentialToolstone { get; set; }
        public string ToolstoneNote { get; set; }
        public string TreeStrata { get; set; }
        public string TreeStrataAge { get; set; }
        public string ShrubStrata { get; set; }
        public string HerbaceousStrata { get; set; }
        public string VegNote { get; set; }
        public string TreeStrataDesc { get; set; }
    }
}
