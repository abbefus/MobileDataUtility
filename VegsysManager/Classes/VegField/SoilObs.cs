using System;
using System.Data;
using System.Linq;

namespace VegsysManager.Classes
{
    class SoilObs
    {
        public static SoilObs Create(DataRow row)
        {
            Type dffType = typeof(SoilObs);
            string[] columns = dffType.GetProperties().Select(x => x.Name).ToArray();

            SoilObs soilobs = new SoilObs();
            foreach (string column in columns)
            {
                Type t = dffType.GetProperty(column).PropertyType;
                dffType.GetProperty(column).SetValue
                (
                    soilobs, 
                    SqlCeConversion.CheckDBNull(row[column],t)
                );
            }
            return soilobs;
        } //creates a SoilObs from an sqlce or (maybe also) sql database row

        public string Horizon { get; set; }
        public string HorizonSuffix1 { get; set; }
        public string HorizonSuffix2 { get; set; }
        public string HorizonSuffix3 { get; set; }
        public string Depth { get; set; }
        public string MatrixColor { get; set; }
        public string MatrixHue { get; set; }
        public string MatrixChroma { get; set; }
        public string MatrixPct { get; set; }
        public string RedoxColor { get; set; }
        public string RedoxHue { get; set; }
        public string RedoxChroma { get; set; }
        public string RedoxPct { get; set; }
        public string Texture { get; set; }
        public Guid SiteSurveyGUID { get; set; }        //-----inconsistent capitalization Matt!!!
        public Guid SoilObsGUID { get; set; }           //-----inconsistent capitalization Matt!!!
    }
}
