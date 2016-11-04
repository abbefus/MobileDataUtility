using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ABUtils;
using NPOI.XSSF.UserModel;
using System.IO;
using Microsoft.Windows.Controls.Ribbon;
using Microsoft.Win32;
using System.Data.SqlServerCe;
using System.Data;
using VegsysManager.Classes;
using System.Data.SqlClient;

namespace VegsysManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : SecuredWindow
    {
        private string template = @"\\BEFUSA-LT\C$\Users\abefus\Documents\Visual Studio 2015\Projects\VegetationABWETReport\VEGSYS_ABWRET_XLSX\VEGSYS_ABWRET_XLSX\ABWRET-A.xlsm";
        private string outfile = @"C:\TEMP\outfile.xlsm";

        public string VEG_CONN_STRING = "Data Source=sqlprod3\\sql2008;Initial Catalog={0};User Id=developer;Password=sp1d3r5!;";
        public string FIELD_DB = "VegField_2014";
        public string PROD_DB = "VEGSYS_2014";

        SqlConnection dbconn;
        SqlCommand dbcommand;


        public MainWindow()
            : base
            (
                #if DEBUG
                    null, null
                #else
                    @"\\CD1002-F03\GEOMATICS\Utilities\Mobile\Data\Access\VEGSYS\VegsysManager.exe", //fixed folder location
                    "31-DEC-2016"  //expiry date
                #endif
            )
        {
            InitializeComponent();
            grid.DataContext = this;
        }

        private void EditTemplate()
        {
            XSSFWorkbook templateWorkbook;
            using (FileStream fs = new FileStream(template, FileMode.Open, FileAccess.ReadWrite))
            {
                templateWorkbook = new XSSFWorkbook(fs);
                fs.Close();
            }

            XSSFSheet sheet = (XSSFSheet)templateWorkbook.GetSheet("F");
            sheet.GetRow(50).GetCell(3).SetCellValue("Drago");
            sheet.SetColumnWidth(2, 50);
            GC.Collect();
            sheet.ForceFormulaRecalculation = true;


            using (FileStream file = new FileStream(outfile, FileMode.CreateNew, FileAccess.Write))
            {
                templateWorkbook.Write(file);
                file.Close();
            }
        }

        private void exit_btn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void export_btn_Click(object sender, RoutedEventArgs e)
        {
            EditTemplate();
        }
        
        private void manualsync_btn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "SDF Files|*.sdf";
            if (openFileDialog.ShowDialog() == true)
            {
                ConnectUsing(string.Format(VEG_CONN_STRING, FIELD_DB));
                VegFieldDB sdf = VegSDF2DataModel(openFileDialog.FileName);
                SyncVegDB(sdf);
            }
        }

        private void SyncVegDB(VegFieldDB db)
        {
            Dictionary<string, Guid[]> existing = new Dictionary<string, Guid[]>
            {
                { "DataFormF", GetExistingGuids(db.DataFormFs.Select(x => x.DataFormFGuid.ToString()),"DataFormFGuid","v_DataFormF",FIELD_DB) },
                { "DataFormF2", GetExistingGuids(db.DataFormF2s.Select(x => x.DataFormF2Guid.ToString()),"DataFormF2Guid", "v_DataFormF2", FIELD_DB) },
                { "HerbPlotSummary", GetExistingGuids(db.HerbPlotSummaries.Select(x => x.PlotSummaryGuid.ToString()), "PlotSummaryGuid", "V_HerbPlotSummary", FIELD_DB) },
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
            

            //SqlUtils.AddParametersFrom<ShovelTestPit>(stp, dbcommand);

            //OpenConnection();

            //dbcommand.CommandType = CommandType.StoredProcedure;
            //dbcommand.CommandText = "V_SP_AddShovelTestPit";

            //try
            //{
            //    dbcommand.ExecuteNonQuery();
            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show(e.Message + "\n" + e.InnerException,
            //                "Stored Procedure Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            //}


            //CloseConnection();
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
                    .AppendLine(string.Format("WHERE {0} IN ('{1}')",guidname, string.Join("','",sdfguids)));

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
        private bool ConnectUsing(string conn)
        {
            try
            {
                dbconn = new SqlConnection(conn);
                dbcommand = new SqlCommand();
                dbcommand.Connection = dbconn;
                return true;
            }
            catch (Exception e)
            {
                UpdateStatus(StatusType.Failure, e.Message);
                return false;
            }
        }
        private void Disconnect()
        {
            if (dbconn.State == ConnectionState.Open)
            {
                dbconn.Close();
            }
        }
        public bool OpenConnection()
        {
            try
            {
                dbconn.Open();
            }
            catch (Exception e)
            {
                UpdateStatus(StatusType.Failure, e.Message);

            }
            return dbconn.State == ConnectionState.Open;
        }
        public bool CloseConnection()
        {
            dbconn.Close();
            return dbconn.State == ConnectionState.Closed;
        }


        public enum StatusType
        {
            Success,
            Failure
        }
        public void UpdateStatus(StatusType type, string msg)
        {
            status_tb.Foreground = type == StatusType.Failure ?
                new SolidColorBrush(Colors.Red) :
                new SolidColorBrush(Colors.Green);
            Console.WriteLine(msg);
        }

        #region Ribbon

        // switches content in mainframe
        private void Ribbon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
        private void ribbon_Loaded(object sender, RoutedEventArgs e)
        {
            // removes quick action toolbar (styling)
            Grid child = VisualTreeHelper.GetChild((DependencyObject)sender, 0) as Grid;
            if (child != null)
            {
                child.RowDefinitions[0].Height = new GridLength(0);
            }
            Console.SetOut(new ConsolWriter(status_tb));
            UpdateStatus(StatusType.Success, string.Format("This version of VPro Manager will expire in {0} days.", DaysLeft));
        }
        private void RibbonApplicationMenu_Loaded(object sender, RoutedEventArgs e)
        {
            // removes 'recent' column in application menu (styling)
            RibbonApplicationMenu am = sender as RibbonApplicationMenu;
            Grid grid = (am.Template.FindName("MainPaneBorder", am) as Border).Parent as Grid;
            grid.ColumnDefinitions[2].Width = new GridLength(0);
        }


        #endregion
    }
    public class ConsolWriter : TextWriter
    {
        private TextBlock textblock;
        public ConsolWriter(TextBlock textbox)
        {
            textblock = textbox;
        }
        public override void Write(string value)
        {
            textblock.Text = value;
        }
        public override void WriteLine(string value)
        {
            textblock.Text = value;
        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }
}

