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

        private void testconnection(string file)
        {
            string connection = string.Format("Data Source={0};Max Database Size=4091;Max Buffer Size = 256; Default Lock Escalation =100;", file);
            SqlCeEngine engine = new SqlCeEngine(connection);

            try { engine.Upgrade(); }
            catch { }
            
            using (var conn = new SqlCeConnection(connection))
            {
                StringBuilder sqlbuilder = new StringBuilder();
                sqlbuilder.AppendLine("SELECT *")
                    .AppendLine("FROM v_DataFormF");

                try
                {
                    conn.Open();
                    SqlCeDataAdapter adapter = new SqlCeDataAdapter(sqlbuilder.ToString(), conn);
                    DataTable datatable = new DataTable();
                    adapter.Fill(datatable);

                    List<DataFormF> dataformfs = new List<DataFormF>();

                    foreach (DataRow row in datatable.Rows)
                    {
                        DataFormF dataformf = DataFormF.Create(row);
                        dataformfs.Add(dataformf);
                    }
                }
                catch (SqlCeException)
                {
                    UpdateStatus(StatusType.Failure, "There was an error connecting to the compact database.");
                }
                finally
                {
                    if (conn.State == ConnectionState.Open) conn.Close();
                }
            }
        }

        private void manualsync_btn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "SDF Files|*.sdf";
            if (openFileDialog.ShowDialog() == true)
            {
                testconnection(openFileDialog.FileName);
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
