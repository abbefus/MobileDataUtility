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

namespace VegsysManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : SecuredWindow
    {
        private string template = @"\\BEFUSA-LT\C$\Users\abefus\Documents\Visual Studio 2015\Projects\VegetationABWETReport\VEGSYS_ABWRET_XLSX\VEGSYS_ABWRET_XLSX\ABWRET-A.xlsm";
        private string outfile = @"C:\TEMP\outfile.xlsm";

        public bool VegConn { get; set; }

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

        private void VegSDF2DataModel(string file)
        {
            string connection = string.Format("Data Source={0};Max Database Size=4091;Max Buffer Size = 256; Default Lock Escalation =100;", file);
            SqlCeEngine engine = new SqlCeEngine(connection);
            VegFieldDB db = new VegFieldDB();

            try { engine.Upgrade(); }
            catch { }
            
            using (var conn = new SqlCeConnection(connection))
            {
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
            }
        }
        
        private DataFormF[] SDF2DataFormF(SqlCeConnection conn)
        {
            string sql = "SELECT * FROM v_DataFormF";
            List<DataFormF> dataformfs = new List<DataFormF>();

            try
            {
                conn.Open();
                SqlCeDataAdapter adapter = new SqlCeDataAdapter(sql, conn);
                DataTable datatable = new DataTable();
                adapter.Fill(datatable);

                foreach (DataRow row in datatable.Rows)
                {
                    DataFormF dataformf = DataFormF.Create(row);
                    dataformfs.Add(dataformf);
                }
            }
            catch (SqlCeException sqle)
            {
                MessageBox.Show(string.Format("{0}\r\n{1}", sqle.Message,sqle.Source));
                UpdateStatus(StatusType.Failure, "There was an error connecting to the compact database.");
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{0}", e.Message));
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }

            return dataformfs.ToArray();
        }
        private DataFormF2[] SDF2DataFormF2(SqlCeConnection conn)
        {
            string sql = "SELECT * FROM v_DataFormF2";
            List<DataFormF2> dataformf2s = new List<DataFormF2>();

            try
            {
                conn.Open();
                SqlCeDataAdapter adapter = new SqlCeDataAdapter(sql, conn);
                DataTable datatable = new DataTable();
                adapter.Fill(datatable);

                foreach (DataRow row in datatable.Rows)
                {
                    dataformf2s.Add(DataFormF2.Create(row));
                }
            }
            catch (SqlCeException sqle)
            {
                MessageBox.Show(string.Format("{0}\r\n{1}", sqle.Message, sqle.Source));
                UpdateStatus(StatusType.Failure, "There was an error connecting to the compact database.");
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{0}", e.Message));
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }

            return dataformf2s.ToArray();
        }
        private HerbPlotSummary[] SDF2HerbPlotSummary(SqlCeConnection conn)
        {
            string sql = "SELECT * FROM V_HerbPlotSummary";
            List<HerbPlotSummary> herbplots = new List<HerbPlotSummary>();

            try
            {
                conn.Open();
                SqlCeDataAdapter adapter = new SqlCeDataAdapter(sql, conn);
                DataTable datatable = new DataTable();
                adapter.Fill(datatable);

                foreach (DataRow row in datatable.Rows)
                {
                    herbplots.Add(HerbPlotSummary.Create(row));
                }
            }
            catch (SqlCeException sqle)
            {
                MessageBox.Show(string.Format("{0}\r\n{1}", sqle.Message, sqle.Source));
                UpdateStatus(StatusType.Failure, "There was an error connecting to the compact database.");
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{0}", e.Message));
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }

            return herbplots.ToArray();
        }
        private Project[] SDF2Project(SqlCeConnection conn)
        {
            string sql = "SELECT * FROM V_Project";
            List<Project> projects = new List<Project>();

            try
            {
                conn.Open();
                SqlCeDataAdapter adapter = new SqlCeDataAdapter(sql, conn);
                DataTable datatable = new DataTable();
                adapter.Fill(datatable);

                foreach (DataRow row in datatable.Rows)
                {
                    projects.Add(Project.Create(row));
                }
            }
            catch (SqlCeException sqle)
            {
                MessageBox.Show(string.Format("{0}\r\n{1}", sqle.Message, sqle.Source));
                UpdateStatus(StatusType.Failure, "There was an error connecting to the compact database.");
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{0}", e.Message));
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }

            return projects.ToArray();
        }
        private ProjectSite[] SDF2ProjectSite(SqlCeConnection conn)
        {
            string sql = "SELECT * FROM V_ProjectSite";
            List<ProjectSite> projectsites = new List<ProjectSite>();

            try
            {
                conn.Open();
                SqlCeDataAdapter adapter = new SqlCeDataAdapter(sql, conn);
                DataTable datatable = new DataTable();
                adapter.Fill(datatable);

                foreach (DataRow row in datatable.Rows)
                {
                    projectsites.Add(ProjectSite.Create(row));
                }
            }
            catch (SqlCeException sqle)
            {
                MessageBox.Show(string.Format("{0}\r\n{1}", sqle.Message, sqle.Source));
                UpdateStatus(StatusType.Failure, "There was an error connecting to the compact database.");
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{0}", e.Message));
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }

            return projectsites.ToArray();
        }
        private Site[] SDF2Site(SqlCeConnection conn)
        {
            string sql = "SELECT * FROM V_Site";
            List<Site> sites = new List<Site>();

            try
            {
                conn.Open();
                SqlCeDataAdapter adapter = new SqlCeDataAdapter(sql, conn);
                DataTable datatable = new DataTable();
                adapter.Fill(datatable);

                foreach (DataRow row in datatable.Rows)
                {
                    sites.Add(Site.Create(row));
                }
            }
            catch (SqlCeException sqle)
            {
                MessageBox.Show(string.Format("{0}\r\n{1}", sqle.Message, sqle.Source));
                UpdateStatus(StatusType.Failure, "There was an error connecting to the compact database.");
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{0}", e.Message));
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }

            return sites.ToArray();
        }
        private SiteLSD[] SDF2SiteLSD(SqlCeConnection conn)
        {
            string sql = "SELECT * FROM V_SiteLSD";
            List<SiteLSD> sitelsd = new List<SiteLSD>();

            try
            {
                conn.Open();
                SqlCeDataAdapter adapter = new SqlCeDataAdapter(sql, conn);
                DataTable datatable = new DataTable();
                adapter.Fill(datatable);

                foreach (DataRow row in datatable.Rows)
                {
                    sitelsd.Add(SiteLSD.Create(row));
                }
            }
            catch (SqlCeException sqle)
            {
                MessageBox.Show(string.Format("{0}\r\n{1}", sqle.Message, sqle.Source));
                UpdateStatus(StatusType.Failure, "There was an error connecting to the compact database.");
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{0}", e.Message));
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }

            return sitelsd.ToArray();
        }
        private SiteSurvey[] SDF2SiteSurvey(SqlCeConnection conn)
        {
            string sql = "SELECT * FROM V_SiteSurvey";
            List<SiteSurvey> sitesurvey = new List<SiteSurvey>();

            try
            {
                conn.Open();
                SqlCeDataAdapter adapter = new SqlCeDataAdapter(sql, conn);
                DataTable datatable = new DataTable();
                adapter.Fill(datatable);

                foreach (DataRow row in datatable.Rows)
                {
                    sitesurvey.Add(SiteSurvey.Create(row));
                }
            }
            catch (SqlCeException sqle)
            {
                MessageBox.Show(string.Format("{0}\r\n{1}", sqle.Message, sqle.Source));
                UpdateStatus(StatusType.Failure, "There was an error connecting to the compact database.");
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{0}", e.Message));
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }

            return sitesurvey.ToArray();
        }
        private SoilObs[] SDF2SoilObs(SqlCeConnection conn)
        {
            string sql = "SELECT * FROM V_SoilObs";
            List<SoilObs> soilobss = new List<SoilObs>();

            try
            {
                conn.Open();
                SqlCeDataAdapter adapter = new SqlCeDataAdapter(sql, conn);
                DataTable datatable = new DataTable();
                adapter.Fill(datatable);

                foreach (DataRow row in datatable.Rows)
                {
                    soilobss.Add(SoilObs.Create(row));
                }
            }
            catch (SqlCeException sqle)
            {
                MessageBox.Show(string.Format("{0}\r\n{1}", sqle.Message, sqle.Source));
                UpdateStatus(StatusType.Failure, "There was an error connecting to the compact database.");
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{0}", e.Message));
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }

            return soilobss.ToArray();
        }
        private SpeciesObserved[] SDF2SpeciesObserved(SqlCeConnection conn)
        {
            string sql = "SELECT * FROM V_SpeciesObserved";
            List<SpeciesObserved> speciesobs = new List<SpeciesObserved>();

            try
            {
                conn.Open();
                SqlCeDataAdapter adapter = new SqlCeDataAdapter(sql, conn);
                DataTable datatable = new DataTable();
                adapter.Fill(datatable);

                foreach (DataRow row in datatable.Rows)
                {
                    speciesobs.Add(SpeciesObserved.Create(row));
                }
            }
            catch (SqlCeException sqle)
            {
                MessageBox.Show(string.Format("{0}\r\n{1}", sqle.Message, sqle.Source));
                UpdateStatus(StatusType.Failure, "There was an error connecting to the compact database.");
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{0}", e.Message));
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }

            return speciesobs.ToArray();
        }
        private WetlandHydrology[] SDF2WetlandHydrology(SqlCeConnection conn)
        {
            string sql = "SELECT * FROM V_WetlandHydrology";
            List<WetlandHydrology> wetlandhydro = new List<WetlandHydrology>();

            try
            {
                conn.Open();
                SqlCeDataAdapter adapter = new SqlCeDataAdapter(sql, conn);
                DataTable datatable = new DataTable();
                adapter.Fill(datatable);

                foreach (DataRow row in datatable.Rows)
                {
                    wetlandhydro.Add(WetlandHydrology.Create(row));
                }
            }
            catch (SqlCeException sqle)
            {
                MessageBox.Show(string.Format("{0}\r\n{1}", sqle.Message, sqle.Source));
                UpdateStatus(StatusType.Failure, "There was an error connecting to the compact database.");
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{0}", e.Message));
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }

            return wetlandhydro.ToArray();
        }
        private WildlifeIncidental[] SDF2WildlifeIncidental(SqlCeConnection conn)
        {
            string sql = "SELECT * FROM W_WildlifeIncidental";
            List<WildlifeIncidental> wildlifeincidental = new List<WildlifeIncidental>();

            try
            {
                conn.Open();
                SqlCeDataAdapter adapter = new SqlCeDataAdapter(sql, conn);
                DataTable datatable = new DataTable();
                adapter.Fill(datatable);

                foreach (DataRow row in datatable.Rows)
                {
                    wildlifeincidental.Add(WildlifeIncidental.Create(row));
                }
            }
            catch (SqlCeException sqle)
            {
                MessageBox.Show(string.Format("{0}\r\n{1}", sqle.Message, sqle.Source));
                UpdateStatus(StatusType.Failure, "There was an error connecting to the compact database.");
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{0}", e.Message));
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }

            return wildlifeincidental.ToArray();
        }


        private void manualsync_btn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "SDF Files|*.sdf";
            if (openFileDialog.ShowDialog() == true)
            {
                VegSDF2DataModel(openFileDialog.FileName);
            }
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

//Replication code
//using System.Data.SqlServerCe;

//SqlCeReplication repl = new SqlCeReplication();

//repl.InternetUrl = @"http://aspxprod3/FDMSHub/sqlcesa35.dll";
//repl.InternetLogin = @"abefus";
//repl.InternetPassword = @"<...>";
//repl.Publisher = @"sqlprod3\sql2008";
//repl.PublisherDatabase = @"VegField_2014";
//repl.PublisherSecurityMode = SecurityType.NTAuthentication;
//repl.Publication = @"VINES_07";
//repl.Subscriber = @"VINES_07";
//repl.SubscriberConnectionString = @"Data Source=""C:\Users\abefus\Documents\SDFTemp\VINES_695.sdf"";Max Database Size=128;Default Lock Escalation =100;";
//try
//{
//   repl.AddSubscription(AddOption.ExistingDatabase);
//   repl.Synchronize();
//}
//catch (SqlCeException e)
//{
//   MessageBox.Show(e.ToString());
//}
