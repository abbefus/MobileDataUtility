using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VegsysManager.Classes
{
    class HerbPlotSummary
    {
        public static HerbPlotSummary Create(DataRow row)
        {
            Type dffType = typeof(HerbPlotSummary);
            string[] columns = dffType.GetProperties().Select(x => x.Name).ToArray();

            HerbPlotSummary herbplotsummary = new HerbPlotSummary();
            foreach (string column in columns)
            {
                Type t = dffType.GetProperty(column).PropertyType;
                dffType.GetProperty(column).SetValue
                (
                    herbplotsummary, 
                    SqlCeConversion.CheckDBNull(row[column],t)
                );
            }
            return herbplotsummary;
        } //creates a dataformf from an sqlce or (maybe also) sql database row

        public Guid PlotSummaryGuid { get; set; }
        public Guid SiteSurveyGuid { get; set; }
        public Int16? PlotSummaryTypeID { get; set; }
        public Single? P1CC { get; set; }
        public Single? P2CC { get; set; }
        public Single? P3CC { get; set; }
        public Single? P4CC { get; set; }
        public Single? P5CC { get; set; }
        public Single? P6CC { get; set; }
        public Single? P7CC { get; set; }
        public Single? P8CC { get; set; }
        public Single? P9CC { get; set; }
        public Single? P10CC { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
