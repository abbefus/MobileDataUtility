using ABUtils;
using Microsoft.Windows.Controls.Ribbon;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VProManager
{

    //TODO:
    //  1. Working animation when a query is being run
    //BUGS:
    //  1. When a filter is applied to prod and go back to staging - only selected value is available

    public partial class MainWindow : SecuredWindow
    {
        public const string CONN_STRING = "Persist Security Info=False;Data Source=sqlprod3\\sql2008;Initial Catalog={0};User Id=developer;Password=sp1d3r5!;";
        public const string VPROE_DB = "VPro7";
        public const string STAGING_DB = "VPro7_Field2013";
        private SqlConnection dbConn;
        private SqlCommand dbcommand;
        public const string QUERY_100 = "100 Most Recent Plots";
        public const string QUERY_ALL = "All Plots";

        ObservableCollection<Plot> PlotList { get; set; }

        private bool _stageconn;
        public bool StageConn
        {
            get { return _stageconn; }
            set
            {
                _stageconn = value;
                if (value)
                {
                    Connect2Staging();
                    connect2stage_btn.Label = "Disconnect From Staging";
                    connect2stage_btn.LargeImageSource = new BitmapImage(new Uri("Images/disconnect-icon.png", UriKind.Relative));
                }
                else
                {
                    if (dbConn != null && dbConn.State == ConnectionState.Open)
                    {
                        dbConn.Close();
                    }
                    PlotList.Clear();
                    connect2stage_btn.Label = "Connect to Staging";
                    connect2stage_btn.LargeImageSource = new BitmapImage(new Uri("Images/connect-icon.png", UriKind.Relative));
                    status_tb.Foreground = new SolidColorBrush(Colors.Red);
                    Console.WriteLine("Not connected to anything.");
                }
            }
        }
        private bool _vproeconn;
        public bool VProEConn
        {
            get { return _vproeconn; }
            set
            {
                _vproeconn = value;
                if (value)
                {
                    Connect2VProE();
                    connect2prod_btn.Label = "Disconnect From VProE";
                    connect2prod_btn.LargeImageSource = new BitmapImage(new Uri("Images/disconnect-icon.png", UriKind.Relative));
                }
                else
                {
                    if (dbConn != null && dbConn.State == ConnectionState.Open)
                    {
                        dbConn.Close();
                    }
                    PlotList.Clear();
                    connect2prod_btn.Label = "Connect to VProE";
                    connect2prod_btn.LargeImageSource = new BitmapImage(new Uri("Images/connect-icon.png", UriKind.Relative));
                    UpdateStatus(StatusType.Failure,"Not connected to anything.");
                }
            }
        }
        public enum StatusType
        {
            Success,
            Failure
        }

        public MainWindow() : base
            (
                #if DEBUG
                    null, null
                #else
                    @"\\CD1002-F03\GEOMATICS\Utilities\Mobile\Data\Access\VPRO07E\VProManager.exe", //fixed folder location
                    "31-DEC-2016"  //expiry date
                #endif
            )
        {

            InitializeComponent();
            staging_rg.DataContext = this;
            production_rg.DataContext = this;
            PlotList = new ObservableCollection<Plot>();
            plots_grd.DataContext = PlotList;
        }

        private void Connect2Staging()
        {
            if (!ConnectUsing(string.Format(CONN_STRING, STAGING_DB))) return;
            if (!project_cb.IsEnabled) InitializeFilterDropDown();

            project_cb.SelectedItem = project_cb.Items.Cast<ComboBoxItem>().Where(x => x.Content.ToString() == QUERY_ALL).FirstOrDefault();
            UpdateDropDown(PlotList.Select(x => x.ProjectTitle).ToArray());

        }
        private bool QueryUncopiedPlots()
        {
            PlotList.Clear();

            string project = ((ComboBoxItem)project_cb.SelectedItem).Content.ToString();
            StringBuilder commandSB = new StringBuilder();
            if (project == QUERY_ALL)
            {
                commandSB.AppendLine("SELECT PlotNumber, ProjectTitle, Date, SiteSurveyor, EnvGuid")
                    .AppendLine("FROM [VPro7_Field2013].[dbo].[VW_GetUncopiedPlotInfo]")
                    .AppendLine("ORDER BY ProjectTitle, PlotNumber");
            }
            else
            {
                commandSB.AppendLine("SELECT PlotNumber, ProjectTitle, Date, SiteSurveyor, EnvGuid")
                    .AppendLine("FROM [VPro7_Field2013].[dbo].[VW_GetUncopiedPlotInfo]")
                    .AppendLine(string.Format("WHERE ProjectTitle = '{0}'", project));
            }

            dbcommand.CommandType = CommandType.Text;
            dbcommand.CommandText = commandSB.ToString();

            try
            {
                using (SqlDataReader rdr = dbcommand.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        Plot plot = new Plot
                        {
                            PlotNumber = rdr.SafeGetString("PlotNumber"),
                            ProjectTitle = rdr.SafeGetString("ProjectTitle"),
                            SurveyDate = rdr.SafeGetDateTime("Date"),
                            SiteSurveyor = rdr.SafeGetString("SiteSurveyor"),
                            EnvGuid = rdr.SafeGetGuid("EnvGuid")
                        };
                        PlotList.Add(plot);
                    }
                }
            }
            catch (Exception)
            {
                UpdateStatus(StatusType.Failure, "Query failed. Sorry...");
                return false;
            }
            UpdateStatus(StatusType.Success, string.Format("Showing {0} uncopied plots.", PlotList.Count));
            return true;
        }

        private void Connect2VProE()
        {
            if (!ConnectUsing(string.Format(CONN_STRING,VPROE_DB))) return;   // abort if connection fails

            if (!project_cb.IsEnabled)
            {
                InitializeFilterDropDown();
            }
            UpdateDropDown();
            project_cb.SelectedItem = project_cb.Items.Cast<ComboBoxItem>().Where(x => x.Content.ToString() == QUERY_100).FirstOrDefault();


        }
        private bool QueryVProE()
        {
            PlotList.Clear();

            string project = ((ComboBoxItem)project_cb.SelectedItem).Content.ToString();
            StringBuilder commandSB = new StringBuilder();
            if (project == QUERY_100)
            {
                commandSB.AppendLine("SELECT TOP 100 PlotNumber, ProjectTitle, Date, SiteSurveyor, EnvID")
                    .AppendLine("FROM [VPro7].[dbo].[VW_PlotList]")
                    .AppendLine("ORDER BY Date DESC");
            }
            else
            {
                commandSB.AppendLine("SELECT PlotNumber, ProjectTitle, Date, SiteSurveyor, EnvID")
                    .AppendLine("FROM [VPro7].[dbo].[VW_PlotList]")
                    .AppendLine(string.Format("WHERE ProjectTitle = '{0}'", project));
            }
            dbcommand.CommandText = commandSB.ToString();

            try
            {
                using (SqlDataReader rdr = dbcommand.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        Plot plot = new Plot
                        {
                            PlotNumber = rdr.SafeGetString("PlotNumber"),
                            ProjectTitle = rdr.SafeGetString("ProjectTitle"),
                            SurveyDate = rdr.SafeGetDateTime("Date"),
                            SiteSurveyor = rdr.SafeGetString("SiteSurveyor"),
                            EnvID = rdr.SafeGetInt32("EnvID")
                        };
                        PlotList.Add(plot);
                    }
                }
                UpdateStatus(StatusType.Success, string.Format("Showing {0} plots.", PlotList.Count));
                return true;
            }
            catch (Exception)
            {
                UpdateStatus(StatusType.Failure, "Query failed. Sorry...");
                return false;
            }
            
        }

        private void project_cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!OpenConnection()) return;
            if (!VProEConn && !StageConn)
            {
                CloseConnection();
                return;
            }

            if (dbConn.Database == "VPro7_Field2013")
            {
                QueryUncopiedPlots();
            }
            else
            {
                QueryVProE();
            }
            CloseConnection();

            Console.WriteLine("Returned {0} plots.", PlotList.Count);
        }

        private void InitializeFilterDropDown()
        {
            if (!OpenConnection()) return;
            StringBuilder commandSB = new StringBuilder();
            commandSB = new StringBuilder();
            commandSB.AppendLine("SELECT DISTINCT [ProjectTitle]")
                    .AppendLine("FROM [VPro7].[dbo].[ProjectMetadata]")
                    .AppendLine("ORDER BY ProjectTitle");
            dbcommand.CommandText = commandSB.ToString();
            //TextBlock qblok = new TextBlock { Text = QUERY_100, TextAlignment=TextAlignment.Left };
            project_cb.Items.Add(new ComboBoxItem { Content = QUERY_100, HorizontalContentAlignment = HorizontalAlignment.Left });
            //qblok = new TextBlock { Text = QUERY_ALL, TextAlignment = TextAlignment.Left};
            project_cb.Items.Add(new ComboBoxItem { Content = QUERY_ALL, HorizontalContentAlignment = HorizontalAlignment.Left });
            
            try
            {
                using (SqlDataReader rdr = dbcommand.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        project_cb.Items.Add(new ComboBoxItem
                        {
                            Content = rdr.GetString(rdr.GetOrdinal("ProjectTitle")),
                            HorizontalContentAlignment = HorizontalAlignment.Left,
                        });
                    }
                }
                project_cb.SelectionChanged += project_cb_SelectionChanged; ;
                project_cb.IsEnabled = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message,
                    "Database Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Console.WriteLine("Unable to query projects for dropdown.");
                project_cb.IsEnabled = false;
            }
            CloseConnection();
        }

        private void UpdateDropDown(string[] keeplist = null)
        {
            string limitq = dbConn.Database == VPROE_DB ? QUERY_100: QUERY_ALL;

            if (keeplist == null)
            {
                limitq = dbConn.Database == VPROE_DB ? QUERY_ALL : QUERY_100;
                foreach (ComboBoxItem item in project_cb.Items)
                {
                    item.Visibility = item.Content.ToString() != limitq ? 
                        Visibility.Visible : Visibility.Collapsed;
                }
                return;
            }
            else
            {
                foreach (ComboBoxItem item in project_cb.Items)
                {
                    string value = item.Content.ToString();
                    item.Visibility = keeplist.Contains(value) || value == limitq ?
                        Visibility.Visible : Visibility.Collapsed;
                }
            }
        }

        #region DB Connection
        private bool ConnectUsing(string conn)
        {
            try
            {
                dbConn = new SqlConnection(conn);
                dbcommand = new SqlCommand();
                dbcommand.Connection = dbConn;
                return true;
            }
            catch (Exception e)
            {
                UpdateStatus(StatusType.Failure, e.Message);
                connect2stage_btn.IsChecked = false;
                return false;
            }
        }
        private bool IsConnected()
        {
            if (dbConn.State != ConnectionState.Open)
            {
                UpdateStatus(StatusType.Failure, "Cannot query without an open connection.");
                return false;
            }
            return true;
        }
        public bool OpenConnection()
        {
            try
            {
                dbConn.Open();
            }
            catch (Exception e)
            {
                UpdateStatus(StatusType.Failure, e.Message);

            }
            return dbConn.State == ConnectionState.Open;
        }
        public bool CloseConnection()
        {
            dbConn.Close();
            return dbConn.State == ConnectionState.Closed;
        }
        #endregion


        public void UpdateStatus(StatusType type, string msg)
        {
            status_tb.Foreground = type == StatusType.Failure ? 
                new SolidColorBrush(Colors.Red) : 
                new SolidColorBrush(Colors.Green);
            Console.WriteLine(msg);
        }


        #region WinAPI

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        private const int GWL_STYLE = -16;
        private const int WS_MAXIMIZEBOX = 0x10000;

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            var hwnd = new WindowInteropHelper((Window)sender).Handle;
            var value = GetWindowLong(hwnd, GWL_STYLE);
            SetWindowLong(hwnd, GWL_STYLE, (int)(value & ~WS_MAXIMIZEBOX));
        }

        #endregion

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

        private void exit_btn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (dbConn.State == ConnectionState.Open)
            {
                dbConn.Close();
            }
            Application.Current.Shutdown();
        }

        private void copy2prod_btn_Click(object sender, RoutedEventArgs e)
        {
            string[] plots = plots_grd.SelectedItems.Cast<Plot>().Select(x => x.PlotNumber).ToArray();
            string[] guids = plots_grd.SelectedItems.Cast<Plot>().Select(x => x.EnvGuid.ToString()).ToArray();


            CheckListDialog cld = new CheckListDialog(plots,string.Format("Are you sure you want to copy these {0} plots to VProE?",plots.Length));
            if (cld.ShowDialog() ?? false)
            {
                CopyPlots2Prod(guids);
                PlotList.Clear();
                Connect2Staging();
                Console.WriteLine("Copied {0} items to VProE", plots.Length);
            }
            else
            {
                Console.WriteLine("Chickened out.");
            }
        }
        private void CopyPlots2Prod(string[] guids)
        {
            OpenConnection();

            dbcommand.CommandType = CommandType.StoredProcedure;
            dbcommand.CommandText = "CopyToProd";

            dbcommand.Parameters.Add(new SqlParameter("@EnvGuidToCopy", DBNull.Value));

            foreach (string guid in guids)
            {
                dbcommand.Parameters["@EnvGuidToCopy"].Value = guid;
                try
                {
                    dbcommand.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message + "\n" + e.InnerException,
                                "Stored Procedure Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                Console.WriteLine("Successfully copied {0}", guid);
            }

            CloseConnection();
        }

        private void delete_btn_Click(object sender, RoutedEventArgs e)
        {
            string[] plots = plots_grd.SelectedItems.Cast<Plot>().Select(x => x.PlotNumber).ToArray();
            string[] ids = plots_grd.SelectedItems.Cast<Plot>().Select(x => x.EnvID.ToString()).ToArray();


            CheckListDialog cld = new CheckListDialog(plots, string.Format("Are you sure you want to delete these {0} plots from VProE?", plots.Length));
            if (cld.ShowDialog() ?? false)
            {
                DeletePlotsFromVProE(ids);
                Connect2VProE();
                if (plots.Length == 1)
                {
                    Console.WriteLine("Deleted {0} from VProE", plots.FirstOrDefault());
                }
                else
                {
                    Console.WriteLine("Deleted {0} items from VProE", plots.Length);
                }
            }
            else
            {
                Console.WriteLine("Chickened out.");
            }
        }
        private void DeletePlotsFromVProE(string[] ids)
        {
            OpenConnection();

            dbcommand.CommandType = CommandType.StoredProcedure;
            dbcommand.CommandText = "DeletePlotByID";

            dbcommand.Parameters.Add(new SqlParameter("@EnvID", DBNull.Value));

            foreach (string id in ids)
            {
                dbcommand.Parameters["@EnvID"].Value = id;
                try
                {
                    dbcommand.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message + "\n" + e.InnerException,
                                "Stored Procedure Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            CloseConnection();
        }

        
    }
    public class Plot
    {
        public string PlotNumber { get; set; }
        public string ProjectTitle { get; set; }
        public DateTime SurveyDate { get; set; }
        public string SiteSurveyor { get; set; }
        public Guid EnvGuid { get; set; }
        public int EnvID { get; set; }
    }

    public static class SqlDataReaderExtensions
    {
        public static int SafeGetInt32(this SqlDataReader reader,
                                       string columnName, int defaultValue=0)
        {
            int ordinal = reader.GetOrdinal(columnName);

            if (!reader.IsDBNull(ordinal))
            {
                return reader.GetInt32(ordinal);
            }
            else
            {
                return defaultValue;
            }
        }
        public static string SafeGetString(this SqlDataReader reader,
                                       string columnName, string defaultValue = "")
        {
            int ordinal = reader.GetOrdinal(columnName);

            if (!reader.IsDBNull(ordinal))
            {
                return reader.GetString(ordinal);
            }
            else
            {
                return defaultValue;
            }
        }
        public static DateTime SafeGetDateTime(this SqlDataReader reader,
                                       string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);

            if (!reader.IsDBNull(ordinal))
            {
                return reader.GetDateTime(ordinal);
            }
            else
            {
                return DateTime.MinValue;
            }
        }
        public static Guid SafeGetGuid(this SqlDataReader reader,
                                       string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);

            if (!reader.IsDBNull(ordinal))
            {
                return reader.GetGuid(ordinal);
            }
            else
            {
                return Guid.Empty;
            }
        }
    }
    public static class Pathing
    {
        [DllImport("mpr.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int WNetGetConnection(
            [MarshalAs(UnmanagedType.LPTStr)] string localName,
            [MarshalAs(UnmanagedType.LPTStr)] StringBuilder remoteName,
            ref int length);
        /// <summary>
        /// Given a path, returns the UNC path or the original. (No exceptions
        /// are raised by this function directly). For example, "P:\2008-02-29"
        /// might return: "\\networkserver\Shares\Photos\2008-02-09"
        /// </summary>
        /// <param name="originalPath">The path to convert to a UNC Path</param>
        /// <returns>A UNC path. If a network drive letter is specified, the
        /// drive letter is converted to a UNC or network path. If the 
        /// originalPath cannot be converted, it is returned unchanged.</returns>
        public static string GetUNCPath(string originalPath)
        {
            StringBuilder sb = new StringBuilder(512);
            int size = sb.Capacity;

            // look for the {LETTER}: combination ...
            if (originalPath.Length > 2 && originalPath[1] == ':')
            {
                // don't use char.IsLetter here - as that can be misleading
                // the only valid drive letters are a-z && A-Z.
                char c = originalPath[0];
                if ((c >= 'j' && c <= 'z') || (c >= 'J' && c <= 'Z')) //mapped drives should always be greater or equal to J
                {
                    int error = WNetGetConnection(originalPath.Substring(0, 2),
                        sb, ref size);
                    if (error == 0)
                    {
                        DirectoryInfo dir = new DirectoryInfo(originalPath);

                        string path = Path.GetFullPath(originalPath)
                            .Substring(Path.GetPathRoot(originalPath).Length);
                        return Path.Combine(sb.ToString().TrimEnd(), path);
                    }
                }
            }
            return originalPath;
        }
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

    public class WindowPositionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToDouble(value) > 20 || System.Convert.ToDouble(value) < -200;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("Never Convert Back");
        }

    }
    public class IntToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (int)value > 0;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return true;
        }
    } // for testing bindings
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if ((DateTime)value == DateTime.MinValue)
            {
                return "-- no date --";
            }
            return ((DateTime)value).ToString("dd-MMM-yyyy HH:mm");
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return true;
        }
    }
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidCastException("The target must be a boolean");
            return !(bool)value;
        }
        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

    }// converts false to true and vice versa

}
