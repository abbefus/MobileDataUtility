using System;
using System.Data;
using System.Linq;

namespace VegsysManager.Classes
{
    class Project
    {
        public static Project Create(DataRow row)
        {
            Type dffType = typeof(Project);
            string[] columns = dffType.GetProperties().Select(x => x.Name).ToArray();

            Project project = new Project();
            foreach (string column in columns)
            {
                dffType.GetProperty(column).SetValue
                (
                    project, 
                    SqlCeConversion.CheckDBNull(row[column])
                );
            }
            return project;
        } //creates a Project from an sqlce or (maybe also) sql database row

        public Int32 FieldSourceID { get; set; }
        public Int32? ClientID { get; set; }
        public string ProjectName { get; set; }
        public string ProjectNumber { get; set; }
        public string ProjDesc { get; set; }
        public Int16? OwnerID { get; set; }
        public Int16? PlotShapeID { get; set; }
        public Int16? PlotSize_ForbGrass { get; set; }
        public Int16? PlotSize_MossLichen { get; set; }
        public Int16? PlotSize_Shrub { get; set; }
        public Int16? PlotSize_Tree { get; set; }
        public string projComments { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public Guid ProjectGUID { get; set; }
    }
}
