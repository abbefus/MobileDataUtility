using System;
using System.Data;
using System.Linq;

namespace VegsysManager.Classes
{
    class SiteLSD
    {
        public static SiteLSD Create(DataRow row)
        {
            Type dffType = typeof(SiteLSD);
            string[] columns = dffType.GetProperties().Select(x => x.Name).ToArray();

            SiteLSD sitelsd = new SiteLSD();
            foreach (string column in columns)
            {
                Type t = dffType.GetProperty(column).PropertyType;
                dffType.GetProperty(column).SetValue
                (
                    sitelsd, 
                    SqlCeConversion.CheckDBNull(row[column], t)
                );
            }
            return sitelsd;
        } //creates a SiteLSD from an sqlce or (maybe also) sql database row

        public string QtrSection { get; set; }
        public Int16 Section { get; set; }
        public Int16 Township { get; set; }
        public Int16 Range { get; set; }
        public string Meridian { get; set; }
        public Guid SiteGUID { get; set; }
        public Guid SiteLSDGUID { get; set; }
    }
}
