using ArchyManager.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ArchyManager.Classes.Archy2014
{
    public class ArchSite: IUploadable
    {
        [Browsable(false)]
        public string DefaultGuid { get { return "ArchSiteGuid"; } }
        [Browsable(false)]
        public bool IsUploaded { get; set; }

        [Browsable(true)]
        public Guid ArchSiteGuid { get; set; }
        [Browsable(true)]
        public Int16 ProjectID { get; set; }
        [Browsable(true)]
        public Int32? PermitID { get; set; }
        [Browsable(true)]
        public string Recorder { get; set; }
        [Browsable(true)]
        public string TempNumber { get; set; }
        [Browsable(true)]
        public string BordenNumber { get; set; }
        [Browsable(true)]
        public string SiteNumber { get; set; }
        [Browsable(true)]
        public DateTime? SurveyDate { get; set; }
        [Browsable(true)]
        public bool? NewSite { get; set; }
        [Browsable(true)]
        public double? SiteLength { get; set; }
        [Browsable(true)]
        public string MajorOrientation { get; set; }
        [Browsable(true)]
        public double? SiteWidth { get; set; }
        [Browsable(true)]
        public string MinorOrientation { get; set; }
        [Browsable(true)]
        public double? SiteZMin { get; set; }
        [Browsable(true)]
        public double? SiteZMax { get; set; }
        [Browsable(true)]
        public string SiteNote { get; set; }
        [Browsable(true)]
        public string PhotoFrom { get; set; }
        [Browsable(true)]
        public string PhotoTo { get; set; }
        [Browsable(true)]
        public string DimensionDetermination { get; set; }
        [Browsable(true)]
        public Int16? CulturalStrataMinDepth { get; set; }
        [Browsable(true)]
        public Int16? CulturalStrataMaxDepth { get; set; }
        [Browsable(true)]
        public string CulturalStrataComment { get; set; }
        [Browsable(true)]
        public string DisturbanceTimeframe { get; set; }
        [Browsable(true)]
        public string DisturbanceCause { get; set; }
        [Browsable(true)]
        public byte? DisturbancePct { get; set; }
        [Browsable(true)]
        public string DisturbanceNote { get; set; }
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
        public string CameraNumber { get; set; }
        [Browsable(true)]
        public string ElevationNote { get; set; }
        [Browsable(true)]
        public string Location { get; set; }
        [Browsable(true)]
        public string TargetArea { get; set; }
        [Browsable(true)]
        public string TestArea { get; set; }
        [Browsable(true)]
        public double? TestSpacing { get; set; }
        [Browsable(true)]
        public string Strategy { get; set; }
        [Browsable(true)]
        public string SterileHorizonDef { get; set; }
        [Browsable(true)]
        public string Setting { get; set; }
        [Browsable(true)]
        public string TopographyMajor { get; set; }
        [Browsable(true)]
        public string TopographyMinor { get; set; }
        [Browsable(true)]
        public string WaterFeatureType1 { get; set; }
        [Browsable(true)]
        public double? WF1Distance { get; set; }
        [Browsable(true)]
        public string WF1Direction { get; set; }
        [Browsable(true)]
        public string WaterFeatureType2 { get; set; }
        [Browsable(true)]
        public double? WF2Distance { get; set; }
        [Browsable(true)]
        public string WF2Direction { get; set; }
        [Browsable(true)]
        public string WaterFeatureType3 { get; set; }
        [Browsable(true)]
        public double? WF3Distance { get; set; }
        [Browsable(true)]
        public string WF3Direction { get; set; }
        [Browsable(true)]
        public string WaterFeatureType4 { get; set; }
        [Browsable(true)]
        public double? WF4Distance { get; set; }
        [Browsable(true)]
        public string WF4Direction { get; set; }
        [Browsable(true)]
        public string Landform1 { get; set; }
        [Browsable(true)]
        public string Landform2 { get; set; }
        [Browsable(true)]
        public string Landform3 { get; set; }
        [Browsable(true)]
        public string LandformOther { get; set; }
        [Browsable(true)]
        public string Exposure { get; set; }
        [Browsable(true)]
        public Int16? SHMinDepth { get; set; }
        [Browsable(true)]
        public Int16? SHMaxDepth { get; set; }
        [Browsable(true)]
        public string SterileHorizonType { get; set; }
        [Browsable(true)]
        public bool? IsInvestigationArea { get; set; }
        [Browsable(true)]
        public double? TestPitAreaLength { get; set; }
        [Browsable(true)]
        public double? TestPitAreaWitdh { get; set; }
        [Browsable(true)]
        public string TestPitAreaOrientation { get; set; }
        [Browsable(true)]
        public double? TransectSpacing { get; set; }
        [Browsable(true)]
        public string PotentialNotes { get; set; }
        [Browsable(true)]
        public string Potential { get; set; }
        [Browsable(true)]
        public string TerrainInfo1 { get; set; }
        [Browsable(true)]
        public string TerrainInfo2 { get; set; }
        [Browsable(true)]
        public string TerrainInfo3 { get; set; }
        [Browsable(true)]
        public string TerrainNote { get; set; }
        [Browsable(true)]
        public string Slope { get; set; }
        [Browsable(true)]
        public string GroundCondition { get; set; }
        [Browsable(true)]
        public string TerrainFeature1 { get; set; }
        [Browsable(true)]
        public double? TerrainFeatureLength { get; set; }
        [Browsable(true)]
        public double? TerrainFeatureWitdh { get; set; }
        [Browsable(true)]
        public string TerrainFeatureOrientation { get; set; }
        [Browsable(true)]
        public string Canopy { get; set; }
        [Browsable(true)]
        public string Understory { get; set; }
        [Browsable(true)]
        public string GroundCover { get; set; }
        [Browsable(true)]
        public bool? PotentialRarePlant { get; set; }
        [Browsable(true)]
        public string SedimentExposure { get; set; }
        [Browsable(true)]
        public string SoilType2 { get; set; }
        [Browsable(true)]
        public string SoilType1 { get; set; }
        [Browsable(true)]
        public string CulturalMaterial { get; set; }
        [Browsable(true)]
        public string OtherCulturalMaterial { get; set; }
        [Browsable(true)]
        public string SurfaceNote { get; set; }
        [Browsable(true)]
        public string FieldDirector { get; set; }

        //move this to a class and extend that class
        public static void SetBrowsableDefaults<T>(string[] exclusions = null)
        {
            string[] fixedexclusions = new string[] { "DefaultGuid", "IsUploaded" };
            exclusions = exclusions != null ? exclusions.Concat(fixedexclusions).ToArray() : fixedexclusions;
            foreach (PropertyInfo property in typeof(T).GetProperties())
            {
                if (exclusions.Contains(property.Name))
                {
                    SqlUtils.SetPropertyBrowsable(typeof(T), property.Name, false);
                }
                else
                {
                    SqlUtils.SetPropertyBrowsable(typeof(T), property.Name, true);
                }
            }
        }
    }
}
