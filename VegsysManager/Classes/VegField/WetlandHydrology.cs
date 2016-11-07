using System;
using System.Data;
using System.Linq;

namespace VegsysManager.Classes
{
    class WetlandHydrology
    {
        public static WetlandHydrology Create(DataRow row)
        {
            Type dffType = typeof(WetlandHydrology);
            string[] columns = dffType.GetProperties().Select(x => x.Name).ToArray();

            WetlandHydrology wetlandhydro = new WetlandHydrology();
            foreach (string column in columns)
            {
                Type t = dffType.GetProperty(column).PropertyType;
                dffType.GetProperty(column).SetValue
                (
                    wetlandhydro, 
                    SqlCeConversion.CheckDBNull(row[column],t)
                );
            }
            return wetlandhydro;
        } //creates a WetlandHydrology from an sqlce or (maybe also) sql database row

        public Guid SiteSurveyGuid { get; set; }
        public Guid HydroGuid { get; set; }
        public bool? Inundated { get; set; }
        public bool? Saturated { get; set; }
        public bool? WaterMark { get; set; }
        public bool? DriftLine { get; set; }
        public bool? SedimentDeposit { get; set; }
        public bool? AlgalMat { get; set; }
        public bool? SaltCrust { get; set; }
        public bool? InundatedOnAerials { get; set; }
        public bool? SparseVeg { get; set; }
        public bool? WaterStained { get; set; }
        public bool? IronDeposit { get; set; }
        public bool? TrueAquaticPlant { get; set; }
        public bool? NoPrimary { get; set; }
        public bool? OxidizedRoot { get; set; }
        public bool? SurfaceSoilCrack { get; set; }
        public bool? LocalSoilSurvey { get; set; }
        public bool? FAC { get; set; }
        public bool? DrainagePattern { get; set; }
        public bool? ThinMuck { get; set; }
        public bool? FrostHeave { get; set; }
        public bool? NoSecondary { get; set; }
        public bool? AquaticInvertShells { get; set; }
    }
}
