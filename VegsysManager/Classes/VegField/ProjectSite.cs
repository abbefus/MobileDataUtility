using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VegsysManager.Classes
{
    class ProjectSite
    {
        public static ProjectSite Create(DataRow row)
        {
            Type dffType = typeof(ProjectSite);
            string[] columns = dffType.GetProperties().Select(x => x.Name).ToArray();

            ProjectSite projectsite = new ProjectSite();
            foreach (string column in columns)
            {
                Type t = dffType.GetProperty(column).PropertyType;
                dffType.GetProperty(column).SetValue
                (
                    projectsite, 
                    SqlCeConversion.CheckDBNull(row[column], t)
                );
            }
            return projectsite;
        } //creates a ProjectSite from an sqlce or (maybe also) sql database row

        public Guid ProjectSiteGuid { get; set; }
        public Guid ProjectGuid { get; set; }
        public Guid SiteGuid { get; set; }
    }
}
