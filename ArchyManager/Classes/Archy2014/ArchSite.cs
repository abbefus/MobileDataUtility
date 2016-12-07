using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchyManager.Classes.Archy2014
{
    class ArchSite
    {
        public Guid ArchSiteGuid { get; set; }
        public Int16? ProjectID { get; set; }
        public Int32? PermitID { get; set; }
        public string Recorder { get; set; }
        public string TempNumber { get; set; }
        public string BordenNumber { get; set; }
        public string SiteNumber { get; set; }
        public DateTime? SurveyDate { get; set; }
        public bool? NewSite { get; set; }
        public double? SiteLength { get; set; }
        public string MajorOrientation { get; set; }
        public double? SiteWidth { get; set; }
        public string MinorOrientation { get; set; }
        public double? SiteZMin { get; set; }
        public double? SiteZMax { get; set; }
        public string SiteNote { get; set; }
        public string PhotoFrom { get; set; }
        public string PhotoTo { get; set; }
        public string DimensionDetermination { get; set; }
        public Int16? CulturalStrataMinDepth { get; set; }
        public Int16? CulturalStrataMaxDepth { get; set; }
        public string CulturalStrataComment { get; set; }
        public string DisturbanceTimeframe { get; set; }
        public string DisturbanceCause { get; set; }
        public byte? DisturbancePct { get; set; }
        public string DisturbanceNote { get; set; }
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
        public string CameraNumber { get; set; }
        public string ElevationNote { get; set; }
        public string Location { get; set; }
        public string TargetArea { get; set; }
        public string TestArea { get; set; }
        public double? TestSpacing { get; set; }
        public string Strategy { get; set; }
        public string SterileHorizonDef { get; set; }
        public string Setting { get; set; }
        public string TopographyMajor { get; set; }
        public string TopographyMinor { get; set; }
        public string WaterFeatureType1 { get; set; }
        public double? WF1Distance { get; set; }
        public string WF1Direction { get; set; }
        public string WaterFeatureType2 { get; set; }
        public double? WF2Distance { get; set; }
        public string WF2Direction { get; set; }
        public string WaterFeatureType3 { get; set; }
        public double? WF3Distance { get; set; }
        public string WF3Direction { get; set; }
        public string WaterFeatureType4 { get; set; }
        public double? WF4Distance { get; set; }
        public string WF4Direction { get; set; }
        public string Landform1 { get; set; }
        public string Landform2 { get; set; }
        public string Landform3 { get; set; }
        public string LandformOther { get; set; }
        public string Exposure { get; set; }
        public Int16? SHMinDepth { get; set; }
        public Int16? SHMaxDepth { get; set; }
        public string SterileHorizonType { get; set; }
        public bool? IsInvestigationArea { get; set; }
        public double? TestPitAreaLength { get; set; }
        public double? TestPitAreaWitdh { get; set; }
        public string TestPitAreaOrientation { get; set; }
        public double? TransectSpacing { get; set; }
        public string PotentialNotes { get; set; }
        public string Potential { get; set; }
        public string TerrainInfo1 { get; set; }
        public string TerrainInfo2 { get; set; }
        public string TerrainInfo3 { get; set; }
        public string TerrainNote { get; set; }
        public string Slope { get; set; }
        public string GroundCondition { get; set; }
        public string TerrainFeature1 { get; set; }
        public double? TerrainFeatureLength { get; set; }
        public double? TerrainFeatureWitdh { get; set; }
        public string TerrainFeatureOrientation { get; set; }
        public string Canopy { get; set; }
        public string Understory { get; set; }
        public string GroundCover { get; set; }
        public bool? PotentialRarePlant { get; set; }
        public string SedimentExposure { get; set; }
        public string SoilType2 { get; set; }
        public string SoilType1 { get; set; }
        public string CulturalMaterial { get; set; }
        public string OtherCulturalMaterial { get; set; }
        public string SurfaceNote { get; set; }
        public string FieldDirector { get; set; }
    }
}
