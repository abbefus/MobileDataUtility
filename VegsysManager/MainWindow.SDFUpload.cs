using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VegsysManager.Classes;

namespace VegsysManager
{
    public partial class MainWindow
    {
        private Project QueryProject(string projectnumber)
        {
            StringBuilder commandSB = new StringBuilder();
            commandSB.AppendLine("SELECT")
                    .AppendLine("FieldSourceID,")
                    .AppendLine("ClientID,")
                    .AppendLine("ProjectName,")
                    .AppendLine("ProjectNumber,")
                    .AppendLine("ProjDesc,")
                    .AppendLine("OwnerID,")
                    .AppendLine("PlotShapeID,")
                    .AppendLine("PlotSize_ForbGrass,")
                    .AppendLine("PlotSize_MossLichen,")
                    .AppendLine("PlotSize_Shrub,")
                    .AppendLine("PlotSize_Tree,")
                    .AppendLine("projComments,")
                    .AppendLine("CreatedBy,")
                    .AppendLine("CreateDate,")
                    .AppendLine("UpdatedBy,")
                    .AppendLine("UpdateDate,")
                    .AppendLine("ProjectGUID")
                .AppendLine("FROM [VegField_2014].[dbo].[V_Project]")
                .AppendLine(string.Format("WHERE ProjectNumber = '{0}'", projectnumber));

            OpenConnection();
            dbcommand.CommandType = CommandType.Text;
            dbcommand.CommandText = commandSB.ToString();
            Project project = new Project { ProjectNumber = projectnumber };

            using (SqlDataReader rdr = dbcommand.ExecuteReader())
            {
                while (rdr.Read())
                {
                    project.FieldSourceID = rdr.SafeGetInt32("FieldSourceID");
                    project.ClientID = rdr.SafeGetInt32("ClientID");
                    project.CreateDate = rdr.SafeGetDateTime("CreateDate");
                    project.CreatedBy = rdr.SafeGetString("CreatedBy");
                    project.OwnerID = rdr.SafeGetInt16("OwnerID");
                    project.PlotShapeID = rdr.SafeGetInt16("PlotShapeID");
                    project.PlotSize_ForbGrass = rdr.SafeGetInt16("PlotSize_ForbGrass");
                    project.PlotSize_MossLichen = rdr.SafeGetInt16("PlotSize_MossLichen");
                    project.PlotSize_Shrub = rdr.SafeGetInt16("PlotSize_Shrub");
                    project.PlotSize_Tree = rdr.SafeGetInt16("PlotSize_Tree");
                    project.projComments = rdr.SafeGetString("projComments");
                    project.ProjDesc = rdr.SafeGetString("ProjDesc");
                    project.ProjectGUID = rdr.SafeGetGuid("ProjectGUID");
                    project.ProjectName = rdr.SafeGetString("ProjectName");
                    project.UpdateDate = rdr.SafeGetDateTime("UpdateDate");
                    project.UpdatedBy = rdr.SafeGetString("UpdatedBy");
                }
            }


            CloseConnection();

            return project;
        }

        private void SyncVegDB(VegFieldDB db)
        {
            Dictionary<string, Guid[]> existing = new Dictionary<string, Guid[]>
            {
                { "DataFormF", GetExistingGuids(db.DataFormFs.Select(x => x.DataFormFGuid.ToString()),"DataFormFGuid","v_DataFormF",FIELD_DB) },
                { "DataFormF2", GetExistingGuids(db.DataFormF2s.Select(x => x.DataFormF2Guid.ToString()),"DataFormF2Guid", "v_DataFormF2", FIELD_DB) },
                { "HerbPlotSummary", GetExistingGuids(db.HerbPlotSummaries.Select(x => x.PlotSummaryGuid.ToString()), "PlotSummaryGuid", "V_HerbPlotSummary", FIELD_DB) },
                { "Project", GetExistingGuids(db.Projects.Select(x => x.ProjectGUID.ToString()), "ProjectGUID", "V_Project", FIELD_DB) },
                { "ProjectSite", GetExistingGuids(db.ProjectSites.Select(x => x.ProjectSiteGuid.ToString()), "ProjectSiteGuid", "V_ProjectSite", FIELD_DB) },
                { "Site", GetExistingGuids(db.Sites.Select(x => x.SiteGUID.ToString()), "SiteGUID", "V_Site", FIELD_DB) },
                { "SiteLSD", GetExistingGuids(db.SiteLSDs.Select(x => x.SiteLSDGUID.ToString()), "SiteLSDGUID", "V_SiteLSD", FIELD_DB) },
                { "SiteSurvey", GetExistingGuids(db.SiteSurveys.Select(x => x.SiteSurveyGuid.ToString()), "SiteSurveyGuid", "V_SiteSurvey", FIELD_DB) },
                { "SoilObs", GetExistingGuids(db.SoilObss.Select(x => x.SoilObsGUID.ToString()), "SoilObsGUID", "V_SoilObs", FIELD_DB) },
                { "SpeciesObserved", GetExistingGuids(db.SpeciesObserveds.Select(x => x.ObservationGuid.ToString()), "ObservationGuid", "V_SpeciesObserved", FIELD_DB) },
                { "WetlandHydrology", GetExistingGuids(db.WetlandHydrologies.Select(x => x.HydroGuid.ToString()), "HydroGuid", "V_WetlandHydrology", FIELD_DB) },
                { "WildlifeIncidental", GetExistingGuids(db.WildlifeIncidentals.Select(x => x.WLObsGuid.ToString()), "WLObsGuid", "W_WildlifeIncidental", FIELD_DB) }
            };
            VegFieldDB datainserts = new VegFieldDB
            {
                DataFormFs = db.DataFormFs.Where(x => !existing["DataFormF"].Contains(x.DataFormFGuid)).ToArray(),
                DataFormF2s = db.DataFormF2s.Where(x => !existing["DataFormF2"].Contains(x.DataFormF2Guid)).ToArray(),
                HerbPlotSummaries = db.HerbPlotSummaries.Where(x => !existing["HerbPlotSummary"].Contains(x.PlotSummaryGuid)).ToArray(),
                Projects = db.Projects.Where(x => !existing["Project"].Contains(x.ProjectGUID)).ToArray(),
                ProjectSites = db.ProjectSites.Where(x => !existing["ProjectSite"].Contains(x.ProjectSiteGuid)).ToArray(),
                Sites = db.Sites.Where(x => !existing["Site"].Contains(x.SiteGUID)).ToArray(),
                SiteLSDs = db.SiteLSDs.Where(x => !existing["SiteLSD"].Contains(x.SiteLSDGUID)).ToArray(),
                SiteSurveys = db.SiteSurveys.Where(x => !existing["SiteSurvey"].Contains(x.SiteSurveyGuid)).ToArray(),
                SoilObss = db.SoilObss.Where(x => !existing["SoilObs"].Contains(x.SoilObsGUID)).ToArray(),
                SpeciesObserveds = db.SpeciesObserveds.Where(x => !existing["SpeciesObserved"].Contains(x.ObservationGuid)).ToArray(),
                WetlandHydrologies = db.WetlandHydrologies.Where(x => !existing["WetlandHydrology"].Contains(x.HydroGuid)).ToArray(),
                WildlifeIncidentals = db.WildlifeIncidentals.Where(x => !existing["WildlifeIncidental"].Contains(x.WLObsGuid)).ToArray()
            };
            UploadToDatabase(datainserts);
        }

        private void UploadToDatabase(VegFieldDB data)
        {
            OpenConnection();
            if (dbcommand.Parameters.Count > 0) dbcommand.Parameters.Clear();
            dbcommand.CommandType = CommandType.StoredProcedure;

            foreach (DataFormF row in data.DataFormFs)
            {
                ExecuteStoredProcedure(row, dbcommand);
            }
            foreach (DataFormF2 row in data.DataFormF2s)
            {
                ExecuteStoredProcedure(row, dbcommand);
            }
            foreach (HerbPlotSummary row in data.HerbPlotSummaries)
            {
                ExecuteStoredProcedure(row, dbcommand);
            }
            //foreach (Project row in data.Projects)
            //{
            //    ExecuteStoredProcedure(row, dbcommand);  // this will go somewhere else
            //}
            foreach (ProjectSite row in data.ProjectSites)
            {
                ExecuteStoredProcedure(row, dbcommand);
            }
            foreach (Site row in data.Sites)
            {
                ExecuteStoredProcedure(row, dbcommand);
            }
            foreach (SiteLSD row in data.SiteLSDs)
            {
                ExecuteStoredProcedure(row, dbcommand);
            }
            foreach (SiteSurvey row in data.SiteSurveys)
            {
                ExecuteStoredProcedure(row, dbcommand);
            }
            foreach (SoilObs row in data.SoilObss)
            {
                ExecuteStoredProcedure(row, dbcommand);
            }
            foreach (SpeciesObserved row in data.SpeciesObserveds)
            {
                ExecuteStoredProcedure(row, dbcommand);
            }
            foreach (WetlandHydrology row in data.WetlandHydrologies)
            {
                ExecuteStoredProcedure(row, dbcommand);
            }
            foreach (WildlifeIncidental row in data.WildlifeIncidentals)
            {
                ExecuteStoredProcedure(row, dbcommand);
            }

            CloseConnection();
        }

        private static void ExecuteStoredProcedure<T>(T row, SqlCommand command)
        {
            if (command.Connection.State != ConnectionState.Open)
            {
                MessageBox.Show("ConnectionState must be open.",
                            "No Connection", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SqlUtils.AddParametersFrom(row, command);
            string sp = string.Format("SP_Insert{0}", row.GetType().Name);
            command.CommandText = sp;
            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.InnerException,
                            string.Format("Stored Procedure Failed - {0}", sp), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            command.Parameters.Clear();
        }

        //TODO:
        //  1. Try statement around conn.Open()
        private VegFieldDB VegSDF2DataModel(string file)
        {
            string connection = string.Format("Data Source={0};Max Database Size=4091;Max Buffer Size = 256; Default Lock Escalation =100;", file);
            SqlCeEngine engine = new SqlCeEngine(connection);

            VegFieldDB db = new VegFieldDB();
            try { engine.Upgrade(); }
            catch { }

            using (var conn = new SqlCeConnection(connection))
            {
                conn.Open();
                db.DataFormFs = SDF2DataFormF(conn);
                db.DataFormF2s = SDF2DataFormF2(conn);
                db.HerbPlotSummaries = SDF2HerbPlotSummary(conn);
                db.Projects = SDF2Project(conn);
                db.ProjectSites = SDF2ProjectSite(conn);
                db.SiteLSDs = SDF2SiteLSD(conn);
                db.Sites = SDF2Site(conn);
                db.SiteSurveys = SDF2SiteSurvey(conn);
                db.SoilObss = SDF2SoilObs(conn);
                db.SpeciesObserveds = SDF2SpeciesObserved(conn);
                db.WetlandHydrologies = SDF2WetlandHydrology(conn);
                db.WildlifeIncidentals = SDF2WildlifeIncidental(conn);
                if (conn.State == ConnectionState.Open) conn.Close();
            }
            return db;
        }

        //TODO:
        //  1. Maybe use interface IDBTable to turn all of these into one function
        private DataFormF[] SDF2DataFormF(SqlCeConnection conn)
        {
            string sql = "SELECT * FROM v_DataFormF";
            List<DataFormF> dataformfs = new List<DataFormF>();

            try
            {
                DataTable datatable = new DataTable();
                using (SqlCeDataAdapter adapter = new SqlCeDataAdapter(sql, conn))
                {
                    adapter.Fill(datatable);
                }

                foreach (DataRow row in datatable.Rows)
                {
                    DataFormF dataformf = DataFormF.Create(row);
                    dataformfs.Add(dataformf);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{0}", e.Message));
                UpdateStatus(StatusType.Failure, "There was an error reading DataFormF\r\n from the compact database.");
            }

            return dataformfs.ToArray();
        }
        private DataFormF2[] SDF2DataFormF2(SqlCeConnection conn)
        {
            string sql = "SELECT * FROM v_DataFormF2";
            List<DataFormF2> dataformf2s = new List<DataFormF2>();

            try
            {
                DataTable datatable = new DataTable();
                using (SqlCeDataAdapter adapter = new SqlCeDataAdapter(sql, conn))
                {
                    adapter.Fill(datatable);
                }

                foreach (DataRow row in datatable.Rows)
                {
                    dataformf2s.Add(DataFormF2.Create(row));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{0}", e.Message));
                UpdateStatus(StatusType.Failure, "There was an error reading DataFormF2\r\n from the compact database.");
            }

            return dataformf2s.ToArray();
        }
        private HerbPlotSummary[] SDF2HerbPlotSummary(SqlCeConnection conn)
        {
            string sql = "SELECT * FROM V_HerbPlotSummary";
            List<HerbPlotSummary> herbplots = new List<HerbPlotSummary>();

            try
            {
                DataTable datatable = new DataTable();
                using (SqlCeDataAdapter adapter = new SqlCeDataAdapter(sql, conn))
                {
                    adapter.Fill(datatable);
                }

                foreach (DataRow row in datatable.Rows)
                {
                    herbplots.Add(HerbPlotSummary.Create(row));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{0}", e.Message));
                UpdateStatus(StatusType.Failure, "There was an error reading HerbPlotSummary\r\n from the compact database.");
            }

            return herbplots.ToArray();
        }
        private Project[] SDF2Project(SqlCeConnection conn)
        {
            string sql = "SELECT * FROM V_Project";
            List<Project> projects = new List<Project>();

            try
            {
                DataTable datatable = new DataTable();
                using (SqlCeDataAdapter adapter = new SqlCeDataAdapter(sql, conn))
                {
                    adapter.Fill(datatable);
                }

                foreach (DataRow row in datatable.Rows)
                {
                    projects.Add(Project.Create(row));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{0}", e.Message));
                UpdateStatus(StatusType.Failure, "There was an error reading Project\r\n from the compact database.");
            }

            return projects.ToArray();
        }
        private ProjectSite[] SDF2ProjectSite(SqlCeConnection conn)
        {
            string sql = "SELECT * FROM V_ProjectSite";
            List<ProjectSite> projectsites = new List<ProjectSite>();

            try
            {
                DataTable datatable = new DataTable();
                using (SqlCeDataAdapter adapter = new SqlCeDataAdapter(sql, conn))
                {
                    adapter.Fill(datatable);
                }

                foreach (DataRow row in datatable.Rows)
                {
                    projectsites.Add(ProjectSite.Create(row));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{0}", e.Message));
                UpdateStatus(StatusType.Failure, "There was an error reading ProjectSite\r\n from the compact database.");
            }

            return projectsites.ToArray();
        }
        private Site[] SDF2Site(SqlCeConnection conn)
        {
            string sql = "SELECT * FROM V_Site";
            List<Site> sites = new List<Site>();

            try
            {
                DataTable datatable = new DataTable();
                using (SqlCeDataAdapter adapter = new SqlCeDataAdapter(sql, conn))
                {
                    adapter.Fill(datatable);
                }

                foreach (DataRow row in datatable.Rows)
                {
                    sites.Add(Site.Create(row));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{0}", e.Message));
                UpdateStatus(StatusType.Failure, "There was an error reading Site\r\n from the compact database.");
            }

            return sites.ToArray();
        }
        private SiteLSD[] SDF2SiteLSD(SqlCeConnection conn)
        {
            string sql = "SELECT * FROM V_SiteLSD";
            List<SiteLSD> sitelsd = new List<SiteLSD>();

            try
            {
                DataTable datatable = new DataTable();
                using (SqlCeDataAdapter adapter = new SqlCeDataAdapter(sql, conn))
                {
                    adapter.Fill(datatable);
                }

                foreach (DataRow row in datatable.Rows)
                {
                    sitelsd.Add(SiteLSD.Create(row));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{0}", e.Message));
                UpdateStatus(StatusType.Failure, "There was an error reading SiteLSD\r\n from the compact database.");
            }

            return sitelsd.ToArray();
        }
        private SiteSurvey[] SDF2SiteSurvey(SqlCeConnection conn)
        {
            string sql = "SELECT * FROM V_SiteSurvey";
            List<SiteSurvey> sitesurvey = new List<SiteSurvey>();

            try
            {
                DataTable datatable = new DataTable();
                using (SqlCeDataAdapter adapter = new SqlCeDataAdapter(sql, conn))
                {
                    adapter.Fill(datatable);
                }

                foreach (DataRow row in datatable.Rows)
                {
                    sitesurvey.Add(SiteSurvey.Create(row));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{0}", e.Message));
                UpdateStatus(StatusType.Failure, "There was an error reading SiteSurvey\r\n from the compact database.");
            }

            return sitesurvey.ToArray();
        }
        private SoilObs[] SDF2SoilObs(SqlCeConnection conn)
        {
            string sql = "SELECT * FROM V_SoilObs";
            List<SoilObs> soilobss = new List<SoilObs>();

            try
            {
                DataTable datatable = new DataTable();
                using (SqlCeDataAdapter adapter = new SqlCeDataAdapter(sql, conn))
                {
                    adapter.Fill(datatable);
                }

                foreach (DataRow row in datatable.Rows)
                {
                    soilobss.Add(SoilObs.Create(row));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{0}", e.Message));
                UpdateStatus(StatusType.Failure, "There was an error reading SoilObs\r\n from the compact database.");
            }

            return soilobss.ToArray();
        }
        private SpeciesObserved[] SDF2SpeciesObserved(SqlCeConnection conn)
        {
            string sql = "SELECT * FROM V_SpeciesObserved";
            List<SpeciesObserved> speciesobs = new List<SpeciesObserved>();

            try
            {
                DataTable datatable = new DataTable();
                using (SqlCeDataAdapter adapter = new SqlCeDataAdapter(sql, conn))
                {
                    adapter.Fill(datatable);
                }

                foreach (DataRow row in datatable.Rows)
                {
                    speciesobs.Add(SpeciesObserved.Create(row));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{0}", e.Message));
                UpdateStatus(StatusType.Failure, "There was an error reading SpeciesObserved\r\n from the compact database.");
            }

            return speciesobs.ToArray();
        }
        private WetlandHydrology[] SDF2WetlandHydrology(SqlCeConnection conn)
        {
            string sql = "SELECT * FROM V_WetlandHydrology";
            List<WetlandHydrology> wetlandhydro = new List<WetlandHydrology>();

            try
            {
                DataTable datatable = new DataTable();
                using (SqlCeDataAdapter adapter = new SqlCeDataAdapter(sql, conn))
                {
                    adapter.Fill(datatable);
                }

                foreach (DataRow row in datatable.Rows)
                {
                    wetlandhydro.Add(WetlandHydrology.Create(row));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{0}", e.Message));
                UpdateStatus(StatusType.Failure, "There was an error reading WetlandHydrology\r\n from the compact database.");
            }

            return wetlandhydro.ToArray();
        }
        private WildlifeIncidental[] SDF2WildlifeIncidental(SqlCeConnection conn)
        {
            string sql = "SELECT * FROM W_WildlifeIncidental";
            List<WildlifeIncidental> wildlifeincidental = new List<WildlifeIncidental>();

            try
            {
                DataTable datatable = new DataTable();
                using (SqlCeDataAdapter adapter = new SqlCeDataAdapter(sql, conn))
                {
                    adapter.Fill(datatable);
                }

                foreach (DataRow row in datatable.Rows)
                {
                    wildlifeincidental.Add(WildlifeIncidental.Create(row));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{0}", e.Message));
                UpdateStatus(StatusType.Failure, "There was an error reading WildlifeIncidental\r\n from the compact database.");
            }

            return wildlifeincidental.ToArray();
        }


        private Guid[] GetExistingGuids(IEnumerable<string> sdfguids, string guidname, string table, string db)
        {
            StringBuilder commandSB = new StringBuilder();
            commandSB.AppendLine(string.Format("SELECT {0}", guidname))
                    .AppendLine(string.Format("FROM [{0}].[dbo].[{1}]", db, table))
                    .AppendLine(string.Format("WHERE {0} IN ('{1}')", guidname, string.Join("','", sdfguids)));

            OpenConnection();
            dbcommand.CommandType = CommandType.Text;
            dbcommand.CommandText = commandSB.ToString();


            List<Guid> guids = new List<Guid>();
            using (SqlDataReader rdr = dbcommand.ExecuteReader())
            {
                while (rdr.Read())
                {
                    Guid guid = rdr.SafeGetGuid(guidname);
                    guids.Add(guid);
                }
            }

            CloseConnection();

            return guids.ToArray();
        }
    }
}
