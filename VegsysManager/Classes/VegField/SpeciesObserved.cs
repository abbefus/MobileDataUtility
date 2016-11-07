using System;
using System.Data;
using System.Linq;

namespace VegsysManager.Classes
{
    class SpeciesObserved
    {
        public static SpeciesObserved Create(DataRow row)
        {
            Type dffType = typeof(SpeciesObserved);
            string[] columns = dffType.GetProperties().Select(x => x.Name).ToArray();

            SpeciesObserved speciesobs = new SpeciesObserved();
            foreach (string column in columns)
            {
                Type t = dffType.GetProperty(column).PropertyType;
                dffType.GetProperty(column).SetValue
                (
                    speciesobs, 
                    SqlCeConversion.CheckDBNull(row[column],t)
                );
            }
            return speciesobs;
        } //creates a SpeciesObserved from an sqlce or (maybe also) sql database row

        public Guid ObservationGuid { get; set; }
        public Guid SiteSurveyGuid { get; set; }
        public Single? DBHCanopy { get; set; }
        public Int16? AgeCanopy { get; set; }
        public string StrucStageCode { get; set; }
        public Single? DBHSubCanopy { get; set; }
        public Int16? AgeSubCanopy { get; set; }
        public string SubStrucStageCode { get; set; }
        public Int16? CCID { get; set; }
        public Single? PctCoverTotal { get; set; }
        public Single? PctCoverCanopy { get; set; }
        public Single? HeightCanopy { get; set; }
        public Single? PctCoverSubCanopy { get; set; }
        public Single? HeightSubCanopy { get; set; }
        public Single? PctCover1_5mto5m { get; set; }
        public Single? Height1_5mto5m { get; set; }
        public Single? PctCoverLT1_5m { get; set; }
        public Single? HeightLT1_5m { get; set; }
        public Single? P1HPrcntCover { get; set; }
        public Single? P2HPrcntCover { get; set; }
        public Single? P3HPrcntCover { get; set; }
        public Single? P4HPrcntCover { get; set; }
        public Single? P5HPrcntCover { get; set; }
        public Single? P6HPrcntCover { get; set; }
        public Single? P7HPrcntCover { get; set; }
        public Single? P8HPrcntCover { get; set; }
        public Single? P9HPrcntCover { get; set; }
        public Single? P10HPrcntCover { get; set; }
        public bool? SpecimenCollect { get; set; }
        public bool? GroundCoverFlag { get; set; }
        public bool? TreeShrubFlag { get; set; }
        public bool? AnBRegenFlag { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string RPStatus { get; set; }
        public Single? DensityDistribution { get; set; }
        public Int32? tsn { get; set; }
    }
}
