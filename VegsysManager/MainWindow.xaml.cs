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
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using VEGSYS = VegsysManager.Classes.Vegsys;
using System.Drawing;
using VisualTreeHelper = System.Windows.Media.VisualTreeHelper;
using SolidColorBrush = System.Windows.Media.SolidColorBrush;
using Colors = System.Windows.Media.Colors;
using SS = NPOI.SS.UserModel;
using FileUtils = ABUtils.FileUtils;
using NPOI.XSSF.UserModel.Extensions;

namespace VegsysManager
{
    //TODO:
    //  1. Project logic where user can either select a project from the database or add a new project and select that
    //          Code will add the project, get the id and guid back - then update the other tables with the new project info
    //  2. If only 2 sites are returned by f or s, the sites string will return the query string which, if is more than 2, will be wrong.
    public partial class MainWindow : SecuredWindow
    {
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

        private void WriteABWRETReport(string[] sitenumbers)
        {
            // read template from resources
            XSSFWorkbook template;
            using (MemoryStream memStream = new MemoryStream())
            {
                byte[] xlsm = Properties.Resources.ABWRET_A_WhiteForm_V1_0;
                BinaryFormatter binForm = new BinaryFormatter();
                memStream.Write(xlsm, 0, xlsm.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                template = new XSSFWorkbook(memStream);
                memStream.Close();
            }

            // get site and sitesurvey information from vegsys
            IEnumerable<DataRow> siteRows = QuerySites(sitenumbers);
            int[] siteIDs = siteRows.Select(x => (int)x["SiteID"]).ToArray();
            WhiteFormV1_0 map = new WhiteFormV1_0();

            // format sitesurvey data
            double lat = siteRows.Select(x => x["Lat"]).Cast<double>().Average();
            double lon = Math.Abs(siteRows.Select(x => x["Lon"]).Cast<double>().Average());
            string latitude = string.Format("{0:#.#}N", lat);
            string longitude = string.Format("{0:#.#}W", lon);
            string location = string.Format("{0}, {1}", latitude, longitude);
            DateTime startdate = siteRows.Min(x => (DateTime)x["SurveyDate"]);
            DateTime enddate = siteRows.Max(x => (DateTime)x["SurveyDate"]);
            string date = startdate != enddate ? string.Join(" - ", startdate.ToShortDateString(), enddate.ToShortDateString()) : startdate.ToShortDateString();
            string sites = string.Join(", ", siteRows.Select(x => x["WetlandID"].ToString()));
            string[] investigators = siteRows.Select(x => x["Observers"].ToString()).Distinct().ToArray();
            string investigator = string.Join(", ", investigators);
            if (investigator.Length > 50) investigator = investigators.FirstOrDefault();

            // write tabs
            WriteToS(template, map, siteIDs, sites, location, investigator, siteRows, date);
            int numsites = WriteToF(template, map, siteIDs, sites, location, investigator, siteRows, date);
            WriteToCoverPg(template, map, numsites, sites, location, investigator, date, latitude, longitude);
            


            // save
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel Macro File|*.xlsm";
            sfd.Title = "Save ABWRET Form";
            string filename = string.Format("ABWRET_{0}_{1}", siteRows.Select(x => x["WetlandID"].ToString()).FirstOrDefault(), DateTime.Now.ToString("yyyyMMdd"));
            sfd.FileName = filename;
            if (sfd.ShowDialog() ?? false)
            {
                sfd.FileName = FileUtils.Increment(sfd.FileName);
                using (FileStream file = sfd.OpenFile() as FileStream)
                {
                    template.Write(file);
                    file.Close();
                }
            }
        }

        private void WriteToCoverPg(XSSFWorkbook template, WhiteFormV1_0 map, int numsites, string sites,
                                    string location, string investigator, string date, string latitude, string longitude)
        {
            XSSFCellStyle cellstyle = CreateCellStyle(template, SS.HorizontalAlignment.Center, SS.VerticalAlignment.Center, 0, SS.BorderStyle.Thin);
            // write to "CoverPg" spreadsheet
            XSSFSheet cpsheet = (XSSFSheet)template.GetSheet("CoverPg");
            string sitestring = numsites > 2 ? string.Format("{0} sites", numsites) : sites;

            // WetlandID
            XSSFCell xssfcell = (XSSFCell)cpsheet.GetRow(map.CP_ROW["WetlandID"]).GetCell(map.CP_COLUMN);
            xssfcell.CellStyle = cellstyle;
            XSSFRichTextString richtext = CreateRichText(sitestring, ExcelColors.Purple0, "Arial", 10, true);
            xssfcell.SetCellValue(richtext);

            // Observers
            xssfcell = (XSSFCell)cpsheet.GetRow(map.CP_ROW["Observers"]).GetCell(map.CP_COLUMN);
            xssfcell.CellStyle = cellstyle;
            richtext = CreateRichText(investigator, ExcelColors.Purple0, "Arial", 10, true);
            xssfcell.SetCellValue(richtext);
            if (investigator.Length > 26) xssfcell.Row.HeightInPoints = 30;

            // SurveyDate
            xssfcell = (XSSFCell)cpsheet.GetRow(map.CP_ROW["SurveyDate"]).GetCell(map.CP_COLUMN);
            xssfcell.CellStyle = cellstyle;
            richtext = CreateRichText(date, ExcelColors.Purple0, "Arial", 10, true);
            xssfcell.SetCellValue(richtext);

            // Latitude
            xssfcell = (XSSFCell)cpsheet.GetRow(map.CP_ROW["Lat"]).GetCell(map.CP_COLUMN);
            xssfcell.CellStyle = cellstyle;
            richtext = CreateRichText(latitude, ExcelColors.Purple0, "Arial", 10, true);
            xssfcell.SetCellValue(richtext);

            //Longitude
            xssfcell = (XSSFCell)cpsheet.GetRow(map.CP_ROW["Lon"]).GetCell(map.CP_COLUMN);
            xssfcell.CellStyle = cellstyle;
            richtext = CreateRichText(longitude, ExcelColors.Purple0, "Arial", 10, true);
            xssfcell.SetCellValue(richtext);
        }
        private int WriteToF(XSSFWorkbook template, WhiteFormV1_0 map, int[] siteIDs, string sites, 
                                    string location, string investigator, IEnumerable<DataRow> siteRows, string date)
        {
            
            XSSFSheet fsheet = (XSSFSheet)template.GetSheet("F");
            

            string infostring = WhiteFormV1_0.FormatFInvestigatorInfo(investigator, date);
            SetValue(fsheet, 1, 0, infostring);

            VEGSYS.DataFormF[] dffs = QueryDataFormF(siteIDs);
            // fill out header information
            string sitestring = dffs.Length > 2 ?
                sitestring = WhiteFormV1_0.FormatFSiteInfo(string.Format("Showing data for {0} sites", dffs.Length), location) :
                sitestring = WhiteFormV1_0.FormatFSiteInfo(sites, location);
            SetValue(fsheet, 0, 0, sitestring);

            // fill out matrix
            for (int i = 0; i < dffs.Length; i++)
            {
                foreach (string p in typeof(VEGSYS.DataFormF).GetProperties().Select(x => x.Name))
                {
                    if (p == "DFFID") continue;
                    int row = map.F_ROW[p];
                    int column = map.F_COLUMN_START + i;
                    PropertyInfo pi = dffs[i].GetType().GetProperty(p);

                    if (p == "SiteID")
                    {
                        string value = siteRows.Where(x => (int)x[p] == dffs[i].SiteID).FirstOrDefault()["WetlandID"].ToString();
                        XSSFCell xssfcell = fsheet.GetRow(row).CreateCell(column) as XSSFCell;
                        
                        XSSFFont font = template.CreateFont() as XSSFFont;
                        font.FontName = "Arial Narrow";
                        font.IsBold = true;
                        font.SetColor(new XSSFColor(ExcelColors.Maroon));
                        font.FontHeightInPoints = 11;
                        xssfcell.CellStyle.SetFont(font);
                        xssfcell.CellStyle.Alignment = SS.HorizontalAlignment.Center;
                        xssfcell.CellStyle.VerticalAlignment = SS.VerticalAlignment.Center;
                        xssfcell.CellStyle.BorderBottom = SS.BorderStyle.Thin;
                        xssfcell.CellStyle.BorderLeft = SS.BorderStyle.Thin;
                        xssfcell.CellStyle.BorderRight = SS.BorderStyle.Thin;
                        xssfcell.CellStyle.BorderTop = SS.BorderStyle.Thin;
                        xssfcell.CellStyle.Rotation = 90;

                        xssfcell.SetCellValue(value);

                        fsheet.GetRow(row).HeightInPoints = 120;
                    }
                    else
                    {
                        double value = NullableObjectToDouble(pi.GetValue(dffs[i]));
                        SetValue(fsheet, row, column, value);
                    }
                }
            }
            return dffs.Length;
        }



        private void WriteToS(XSSFWorkbook template, WhiteFormV1_0 map, int[] siteIDs, string sites,
                    string location, string investigator, IEnumerable<DataRow> siteRows, string date)
        {
            string sitestring;
            XSSFSheet ssheet = (XSSFSheet)template.GetSheet("S");

            

            //Investigator:
            string infostring = WhiteFormV1_0.FormatSInvestigatorInfo(investigator);
            SetValue(ssheet, 0, 3, infostring);

            //Date:
            string datestring = WhiteFormV1_0.FormatSDateInfo(date);
            SetValue(ssheet, 0, 4, datestring);

            
            VEGSYS.DataFormS[] dfss = QueryDataFormS(siteIDs);
            //Site Name and Location: (will be wrong if more than 2 sites in string sites and only 2 return in dfss)
            sitestring = dfss.Length > 2 ?
                WhiteFormV1_0.FormatSSiteInfo(string.Format("Showing data for {0} sites", dfss.Length), location) :
                WhiteFormV1_0.FormatSSiteInfo(sites, location);
            SetValue(ssheet, 0, 0, sitestring);

            // fill out matrix
            for (int i = 0; i < dfss.Length; i++)
            {
                foreach (string p in typeof(VEGSYS.DataFormS).GetProperties().Select(x => x.Name))
                {
                    if (p == "DFSID") continue;
                    int row = map.S_ROW[p];
                    int column = map.S_COLUMN_START + i;
                    PropertyInfo pi = dfss[i].GetType().GetProperty(p);

                    if (p == "SiteID")
                    {
                        string value = siteRows.Where(x => (int)x[p] == dfss[i].SiteID).FirstOrDefault()["WetlandID"].ToString();
                        XSSFCell xssfcell = ssheet.GetRow(row).CreateCell(column) as XSSFCell;

                        XSSFFont font = template.CreateFont() as XSSFFont;
                        font.FontName = "Calibri";
                        font.IsBold = true;
                        font.SetColor(new XSSFColor(ExcelColors.Maroon));
                        font.FontHeightInPoints = 12;
                        xssfcell.CellStyle.SetFont(font);
                        xssfcell.CellStyle.Alignment = SS.HorizontalAlignment.Center;
                        xssfcell.CellStyle.VerticalAlignment = SS.VerticalAlignment.Center;
                        xssfcell.CellStyle.BorderBottom = SS.BorderStyle.Thin;
                        xssfcell.CellStyle.BorderLeft = SS.BorderStyle.Thin;
                        xssfcell.CellStyle.BorderRight = SS.BorderStyle.Thin;
                        xssfcell.CellStyle.BorderTop = SS.BorderStyle.Thin;
                        xssfcell.CellStyle.Rotation = 90;

                        xssfcell.SetCellValue(value);

                        ssheet.GetRow(row).HeightInPoints = 120;
                    }
                    else
                    {
                        double value = NullableObjectToDouble(pi.GetValue(dfss[i]));
                        SetValue(ssheet, row, column, value);
                    }
                }
            }
        }


        //Queries
        private IEnumerable<DataRow> QuerySites(string[] sitenumbers)
        {
            string sitenumstring = string.Join("','", sitenumbers);
            StringBuilder sqlSB = new StringBuilder();
            sqlSB.AppendLine("SELECT")
                .AppendLine("CASE WHEN DATALENGTH(RTRIM([VEGSYS_2014].[dbo].[V_Site].WetlandID)) = 0 OR[VEGSYS_2014].[dbo].[V_Site].WetlandID IS NULL")
                    .AppendLine("THEN [VEGSYS_2014].[dbo].[V_Site].SiteNumber")
                    .AppendLine("ELSE [VEGSYS_2014].[dbo].[V_Site].WetlandID")
                .AppendLine("END AS WetlandID,")
                .AppendLine("dbo.V_SiteSurvey.Observers,")
                .AppendLine("MAX(dbo.V_SiteSurvey.SurveyDate) AS SurveyDate,")
                .AppendLine("dbo.V_Site.Lat,")
                .AppendLine("dbo.V_Site.Lon,")
                .AppendLine("dbo.V_Site.SiteID")
            .AppendLine("FROM ")
                .AppendLine("dbo.V_Site INNER JOIN")
                .AppendLine("dbo.V_SiteSurvey ON dbo.V_Site.SiteID = dbo.V_SiteSurvey.SiteID")
            .AppendLine("WHERE")
                .AppendLine(string.Format("dbo.V_Site.SiteNumber IN ('{0}')", sitenumstring))
            .AppendLine("GROUP BY")
                    .AppendLine("[VEGSYS_2014].[dbo].[V_Site].SiteNumber,")
                    .AppendLine("[VEGSYS_2014].[dbo].[V_Site].WetlandID,")
                    .AppendLine("dbo.V_SiteSurvey.Observers,")
                    .AppendLine("dbo.V_Site.Lat,")
                    .AppendLine("dbo.V_Site.Lon,")
                    .AppendLine("dbo.V_Site.SiteID");

            string connect = string.Format(VEG_CONN_STRING, PROD_DB);
            DataTable dt = new DataTable();
            using (SqlDataAdapter sda = new SqlDataAdapter(sqlSB.ToString(), connect))
            {
                sda.Fill(dt);
            }
            return dt.Rows.Cast<DataRow>();
        }
        private VEGSYS.DataFormF[] QueryDataFormF(int[] siteIDs)
        {
            List<VEGSYS.DataFormF> dffs = new List<VEGSYS.DataFormF>();
            string siteIDstring = string.Join(",", siteIDs);
            StringBuilder sqlSB = new StringBuilder();
            sqlSB.AppendLine(string.Format("SELECT {0} FROM [VW_LatestDataFormF]",WhiteFormV1_0.F_COLUMNS))
                .AppendLine(string.Format("WHERE SiteID IN ({0})", siteIDstring));

            string connect = string.Format(VEG_CONN_STRING, PROD_DB);
            DataTable dt = new DataTable();
            using (SqlDataAdapter sda = new SqlDataAdapter(sqlSB.ToString(), connect))
            {
                sda.Fill(dt);
            }
            foreach (DataRow row in dt.Rows)
            {
                dffs.Add(VEGSYS.DataFormF.Create(row));
            }
            return dffs.ToArray();
        }
        private VEGSYS.DataFormS[] QueryDataFormS(int[] siteIDs)
        {
            List<VEGSYS.DataFormS> dfss = new List<VEGSYS.DataFormS>();
            string siteIDstring = string.Join(",", siteIDs);
            StringBuilder sqlSB = new StringBuilder();
            sqlSB.AppendLine("SELECT * FROM [V_DataFormS]")
                .AppendLine(string.Format("WHERE SiteID IN ({0})", siteIDstring));

            string connect = string.Format(VEG_CONN_STRING, PROD_DB);
            DataTable dt = new DataTable();
            using (SqlDataAdapter sda = new SqlDataAdapter(sqlSB.ToString(), connect))
            {
                sda.Fill(dt);
            }
            foreach (DataRow row in dt.Rows)
            {
                dfss.Add(VEGSYS.DataFormS.Create(row));
            }
            return dfss.ToArray();
        }

        //NPOI static functions
        private static XSSFRichTextString CreateRichText(string text, Color color,
            string fontName = "Arial", short fontsize = 10, bool isBold = false, bool isItalic = false)
        {
            XSSFFont font = new XSSFFont();
            font.FontName = fontName;
            font.IsBold = isBold;
            font.IsItalic = isItalic;
            font.SetColor(new XSSFColor(color));
            font.FontHeightInPoints = fontsize;

            
            XSSFRichTextString richtext = new XSSFRichTextString();
            richtext.Append(text, font);
            return richtext;
        }
        private static XSSFCellStyle CreateCellStyle(XSSFWorkbook wb, SS.HorizontalAlignment halign = 0,
            SS.VerticalAlignment valign = 0, short rotation = 0, SS.BorderStyle borderStyle = 0,
            Color? borderColor = null, bool wrapText = true)
        {
            XSSFCellStyle cellstyle = (XSSFCellStyle)wb.CreateCellStyle();
            cellstyle.Rotation = rotation;
            cellstyle.Alignment = halign;
            cellstyle.VerticalAlignment = valign;
            cellstyle.WrapText = wrapText;
            cellstyle.BorderBottom = borderStyle;
            cellstyle.BorderLeft = borderStyle;
            cellstyle.BorderRight = borderStyle;
            cellstyle.BorderTop = borderStyle;
            borderColor = borderColor ?? ExcelColors.Black;
            cellstyle.SetBorderColor(BorderSide.BOTTOM, new XSSFColor((Color)borderColor));
            cellstyle.SetBorderColor(BorderSide.LEFT, new XSSFColor((Color)borderColor));
            cellstyle.SetBorderColor(BorderSide.RIGHT, new XSSFColor((Color)borderColor));
            cellstyle.SetBorderColor(BorderSide.TOP, new XSSFColor((Color)borderColor));
            return cellstyle;
        }
        public static void SetValue(XSSFSheet sheet, int row, int column, double value)
        {
            sheet.GetRow(row).GetCell(column).SetCellValue(value);
        }
        public static void SetValue(XSSFSheet sheet, int row, int column, string value)
        {
            sheet.GetRow(row).GetCell(column).SetCellValue(value);
        }
        public static void SetValue(XSSFSheet sheet, int row, int column, SS.IRichTextString value)
        {
            sheet.GetRow(row).GetCell(column).SetCellValue(value);
        }
        private static double NullableObjectToDouble(object Value)
        {
            if (DBNull.Value.Equals(Value))
                return 0;
            else
                return Convert.ToDouble(Value);
        }



        private void export_btn_Click(object sender, RoutedEventArgs e)
        {
            //open dialog to choose project,
            //populate listview with available sitenumbers
            string[] sitenumbers = new string[]
            {
                "WLc09","WLc11","WLc12","WLc59","WLc14","WLc15","WLc13","WLc16","WLc02","WLc03","WLc04","WLc06","WLc05","WLc63","WLc60","WLc53","WLc53a","WLc71","WLs52a","WLc51","WLc50","WLc070","WLc69","W1000","WLc40","WLc68","WLc10","WLc18","WLc18-2","WLc19","WLc20","WLc65","WLc21","WLc22","WLc27","WLc38-39","D2drainage"
                //"CB16-Vaux-WL001","CB16-Vaux-WL002"
                //"WH2116BS006","WH2116BS007","WH2116BS010","WH2116BS011","WH2116BS020","WH2116BS021","WH2116BS022","WH2116BS024","WH2116BS025","WH2116BS026","WH2116BS027","WH2116BS028","WH2116BS029","WH2116BS030","WH2116BS031","WH2116BS032","WH2116BS033","WH2116JL002","WH2116JL004","WH2116JL005-z1","WH2116JL005-z2","WH2116JL006","WH2116JL007","WH2116JL008","WH2116JL011","WH2116JL014","WH2116JL016","WH2116JL018","WH2116JL020","WH2116JL021","WH2116JL022","WH2116JL024","WH2116JL027","WH2116JL028","WH2116JL029","WH2116JL10abwret"
            };
            WriteABWRETReport(sitenumbers);
        }


        private void manualsync_btn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "SDF Files|*.sdf";
            if (openFileDialog.ShowDialog() == true)
            {
                ConnectUsing(string.Format(VEG_CONN_STRING, FIELD_DB));
                VegFieldDB sdf = VegSDF2DataModel(openFileDialog.FileName);

                //
                string projectnumber = "123511851";
                //will need to specify a project in case more than one
                sdf.ChangeProject(sdf.Projects.FirstOrDefault(), QueryProject(projectnumber));

                SyncVegDB(sdf);
            }
        }


        private void exit_btn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
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
            UpdateStatus(StatusType.Success, string.Format("This version of Vegsys Manager will expire in {0} days.", DaysLeft));
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

    public static class ExcelColors
    {
        public static Color Aqua { get { return Color.FromArgb(0x4bacc6); } }
        public static Color Aqua3 { get { return Color.FromArgb(0x92cddc); } }
        public static Color Aqua4 { get { return Color.FromArgb(0x31869b); } }
        public static Color Black { get { return Color.FromArgb(0x000); } }
        public static Color SkyBlue { get { return Color.FromArgb(0x00b0f0); } }
        public static Color Green { get { return Color.FromArgb(0x00B050); } }
        public static Color Grey { get { return Color.FromArgb(0xf2f2f2); } }
        public static Color Grey2 { get { return Color.FromArgb(0xd9d9d9); } }
        public static Color Maroon { get { return Color.FromArgb(0xc0504d); } }
        public static Color Maroon4 { get { return Color.FromArgb(0x963634); } }
        public static Color Orange { get { return Color.FromArgb(0xf79646); } }
        public static Color Orange1 { get { return Color.FromArgb(0xfde9d9); } }
        public static Color Orange2 { get { return Color.FromArgb(0xfcd5b4); } }
        public static Color Orange4 { get { return Color.FromArgb(0xe26b0a); } }
        public static Color Pink { get { return Color.FromArgb(0xff34ff); } }
        public static Color Purple0 { get { return Color.FromArgb(0x7030a0); } }
        public static Color Red { get { return Color.FromArgb(0xff0000); } }
        public static Color White { get { return Color.FromArgb(0xffffff); } }
        public static Color Yellow0 { get { return Color.FromArgb(0xffff00); } }
        public static Color Yellow1 { get { return Color.FromArgb(0xf6ea00); } }
        public static Color Transparent { get { return Color.Transparent; } }
        public static Color None { get { return Color.Empty; } }
    }
}


//may need to do all this if you want to set column width:
//sheet.SetColumnWidth(2, 50);
//GC.Collect();
//sheet.ForceFormulaRecalculation = true;