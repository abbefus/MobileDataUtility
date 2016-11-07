using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VegsysManager.Classes
{
    class VegFieldDB
    {
        public string[] GetFieldnames()
        {
            List<string> fieldnames = new List<string>();
            foreach (PropertyInfo property in GetType().GetProperties())
            {
                if (property.PropertyType.IsArray)
                {
                    fieldnames.Add(property.PropertyType.GetElementType().Name);
                }
            }
            return fieldnames.ToArray();
        }
        public void ChangeProject(Project current, Project newproject)
        {
            for (int i = 0; i < ProjectSites.Length; i++)
            {
                if (ProjectSites[i].ProjectGuid == current.ProjectGUID)
                {
                    ProjectSites[i].ProjectGuid = newproject.ProjectGUID;
                }
            }
            for (int i = 0; i < SiteSurveys.Length; i++)
            {
                if (SiteSurveys[i].ProjectGuid == current.ProjectGUID)
                {
                    SiteSurveys[i].ProjectGuid = newproject.ProjectGUID;
                }
            }
            for (int i = 0; i < WildlifeIncidentals.Length; i++)
            {
                if (WildlifeIncidentals[i].ProjectID == current.FieldSourceID)
                {
                    WildlifeIncidentals[i].ProjectID = newproject.FieldSourceID;
                }
            }
            if (Projects.Where(x => x.ProjectGUID == newproject.ProjectGUID).Count() == 0)
            {
                List<Project> projects = Projects.ToList();
                projects.Add(newproject);
                Projects = projects.ToArray();
            }
        }

        public DataFormF[] DataFormFs { get; set; }
        public DataFormF2[] DataFormF2s { get; set; }
        public HerbPlotSummary[] HerbPlotSummaries { get; set; }
        public Project[] Projects { get; set; }
        public ProjectSite[] ProjectSites { get; set; }
        public Site[] Sites { get; set; }
        public SiteLSD[] SiteLSDs { get; set; }
        public SiteSurvey[] SiteSurveys { get; set; }
        public SoilObs[] SoilObss { get; set; }
        public SpeciesObserved[] SpeciesObserveds { get; set; }
        public WetlandHydrology[] WetlandHydrologies { get; set; }
        public WildlifeIncidental[] WildlifeIncidentals { get; set; }

    }
    public interface IDBTable
    {
        IDBTable Create(DataRow row);
    }
}
