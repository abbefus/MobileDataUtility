using ABSSTools;
using ABUtils;
using Microsoft.Win32;
using Microsoft.Windows.Controls.Ribbon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ArchyManager
{

    public partial class ArchyMain : SecuredWindow
    {
        public const string AEON_CONN_STRING = "Data Source=sqlprod3\\sql2008;Initial Catalog=Archy2014;User Id=developer;Password=sp1d3r5!;";
        private SqlConnection dbConn;
        private SqlCommand dbcommand;
        private string SQL;
        
        public ArchyMain()
        {
            InitializeComponent();
#if DEBUG
            @"C:\Users\abefus\Documents\Visual Studio 2015\Projects\BC1235Tools\PDFToExcel\bin\Debug\PDFToData.exe", //fixed folder location
                    "31-DEC-2016"  //expiry date
#elif FINAL
                  @"\\CD1002-F03\GEOMATICS\Utilities\GIS\PDFToData.exe", //fixed folder location
                    "31-DEC-2016"  //expiry date
#elif RELEASE
                  @"C:\Users\abefus\Documents\Visual Studio 2015\Projects\BC1235Tools\PDFToExcel\bin\Release\PDFToData.exe", //fixed folder location
                    "31-DEC-2016"  //expiry date
#endif
        }

        private ShovelTestPit[] ReadSTPs(DataSet dataset)
        {
            char[] invalidchars = new char[] { ' ', '_', '-' };

            List<ShovelTestPit> stplist = new List<ShovelTestPit>();
            string[] headers = dataset.Tables[0].Rows[0].ItemArray.Where(x => x != DBNull.Value).Cast<string>().ToArray();
            
            for (int i = 1; i < dataset.Tables[0].Rows.Count; i++)
            {
                ShovelTestPit stp = new ShovelTestPit();
                Type targetType = typeof(Object);
                Type sourceType = typeof(Object);
                Type stpType = typeof(ShovelTestPit);
                object value = null;

                for (int j = 0; j < headers.Length; j++)
                {
                    value = dataset.Tables[0].Rows[i].ItemArray[j];
                    sourceType = value.GetType();
                    try
                    {
                        targetType = stp.GetType().GetProperty(headers[j]).PropertyType;
                        stpType.GetProperty(headers[j]).SetValue(stp, value);
                    }
                    catch (ArgumentException e) // cannot convert guid or number value
                    {
                        if (targetType == typeof(Int16?) || targetType == typeof(Int16))
                        {
                            Int16 int16value = Convert.ToInt16(value);
                            stpType.GetProperty(headers[j]).SetValue(stp, int16value);
                        }
                        else if (targetType == typeof(string))
                        {
                            stpType.GetProperty(headers[j]).SetValue(stp, value.ToString());
                        }
                        else if (targetType == typeof(Guid))
                        {
                            string guidstr = value.ToString();
                            Guid guid;
                            if (Guid.TryParse(guidstr, out guid))
                            {
                                stpType.GetProperty(headers[j]).SetValue(stp, guid);
                            }
                        }
                        else
                        {
                            MessageBox.Show(string.Format("{0} ==> {1}: {2}", sourceType, targetType, e.Message), 
                                "Data Conversion Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        
                    }
                    catch (NullReferenceException) // header does not match property in ShovelTest Pit
                    {
                        MessageBox.Show(string.Format("Property {0} doesn't exist in ShovelTestPit", headers[j]),
                                "Header Typo", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Data.ToString(),
                                "Unknown Conversion Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                stplist.Add(stp);
            }
            Console.WriteLine(string.Format("'{0}'",string.Join("','",stplist.Select(x => x.PitID))));
            return stplist.ToArray();


        }
        private void browsefile_btn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ShovelTestPit[] stps = null;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Files|*.xls;*.xlsx";
            if (openFileDialog.ShowDialog() == true)
            {
                DataTableCollection data = ExcelReader.Read(openFileDialog.FileName);
                if (data != null)
                {
                    stps = ReadSTPs(data[0].DataSet);
                }
            }

            if (stps != null)
            {
                //would put this all in one function that took and returned stps
                ArchSite[] archsites = QueryArchSites(stps.Select(x => x.SiteNumber));
                Datum[] datums = QueryDatums(stps.Select(x => x.DatumID));


                
                for (int i = 0; i < stps.Length; i++)
                {
                    ArchSite archsite = archsites.Where(x => x.SiteNumber == stps[i].SiteNumber).FirstOrDefault();
                    if (archsite == null)
                    {
                        stps[i].ArchSiteGuid = Guid.Empty;
                        MessageBox.Show(string.Format("Datum {0} not found.", stps[i].DatumID),
                            "Archsite not found", MessageBoxButton.OK, MessageBoxImage.Warning);
                        continue;
                    }
                    stps[i].ArchSiteGuid = archsite.ArchSiteGuid;
                    stps[i].ProjectID = archsite.ProjectID;
                    if (string.IsNullOrWhiteSpace(stps[i].PermitNumber))
                    {
                        stps[i].PermitID = archsite.PermitID;
                    }

                    Datum datum = datums.Where(x => x.DatumID == stps[i].DatumID && x.ArchSiteGuid == stps[i].ArchSiteGuid).FirstOrDefault();
                    if (datum == null)
                    {
                        stps[i].DatumGuid = Guid.Empty;
                        MessageBox.Show(string.Format("Datum {0} not found.", stps[i].DatumID),
                            "Datum not found", MessageBoxButton.OK, MessageBoxImage.Warning);
                        continue;
                    }
                    stps[i].DatumGuid = datum.DatumGuid;
                    
                    if (!stps[i].DatumBearing1.HasValue || !stps[i].DatumDistance1.HasValue || !datum.Easting.HasValue || !datum.Northing.HasValue )
                    {
                        MessageBox.Show(string.Format("Unable to calculate coordinates for {0}", stps[i].PitID), 
                            "Missing Data", MessageBoxButton.OK, MessageBoxImage.Warning);
                        continue;
                    }
                    double bearing = stps[i].DatumBearing1 ?? 0;
                    double distance = stps[i].DatumDistance1 ?? 0;
                    double easting = datum.Easting ?? 0;
                    double northing = datum.Northing ?? 0;
                    stps[i].Easting = easting + (distance * Math.Sin(rad(90 - bearing)));
                    stps[i].Northing = northing + (distance * Math.Cos(rad(90 - bearing)));

                    if (!datum.UTMZone.HasValue)
                    {
                        MessageBox.Show(string.Format("Unable to calculate latitude and Longitude for {0}", stps[i].PitID),
                            "No UTMZone Specified", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        double longitude = 0;
                        double latitude = 0;
                        LatLongCalc.UTMtoLL(northing, easting, (int?)datum.UTMZone ?? 0, ref longitude, ref latitude);
                        stps[i].Longitude = longitude;
                        stps[i].Latitude = latitude;
                        stps[i].UTMZone = datum.UTMZone;
                    }
                }



                foreach (ShovelTestPit stp in stps.Where(x => x.ArchSiteGuid != Guid.Empty && x.DatumGuid != Guid.Empty))
                {
                    SaveToDatabase(stp);
                }
            }
        }


        private Datum[] QueryDatums(IEnumerable<string> datumids)
        {
            StringBuilder commandSB = new StringBuilder();
            commandSB.AppendLine("SELECT DatumID, Easting, Northing, UTMZone, DatumGuid, ArchSiteGuid")
                    .AppendLine("FROM [Archy2014].[dbo].[Datum]")
                    .AppendLine(string.Format("WHERE DatumID IN ('{0}')", 
                                    string.Join("','", datumids.Distinct())));

            OpenConnection();
            dbcommand.CommandType = CommandType.Text;
            dbcommand.CommandText = commandSB.ToString();
            SqlDataReader rdr = dbcommand.ExecuteReader();

            List<Datum> datumlist = new List<Datum>();
            while (rdr.Read())
            {
                Datum datum = new Datum
                {
                    DatumID = rdr.SafeGetString("DatumID"),
                    DatumGuid = rdr.SafeGetGuid("DatumGuid"),
                    Easting = rdr.GetDouble(rdr.GetOrdinal("Easting")),
                    Northing = rdr.GetDouble(rdr.GetOrdinal("Northing")),
                    UTMZone = rdr.GetByte(rdr.GetOrdinal("UTMZone")),
                    ArchSiteGuid = rdr.SafeGetGuid("ArchSiteGuid")
                };
                datumlist.Add(datum);
            }
            
            CloseConnection();

            return datumlist.ToArray();
        }
        private ArchSite[] QueryArchSites(IEnumerable<string> sitenumbers)
        {
            StringBuilder commandSB = new StringBuilder();
            commandSB.AppendLine("SELECT ArchSiteGuid, ProjectID, PermitID, SiteNumber")
                    .AppendLine("FROM [Archy2014].[dbo].[ArchSite]")
                    .AppendLine(string.Format("WHERE SiteNumber IN ('{0}')", string.Join("','", sitenumbers.Distinct())));

            OpenConnection();
            dbcommand.CommandType = CommandType.Text;
            dbcommand.CommandText = commandSB.ToString();
            SqlDataReader rdr = dbcommand.ExecuteReader();

            List<ArchSite> ialist = new List<ArchSite>();
            while (rdr.Read())
            {
                ArchSite archsite = new ArchSite
                {
                    ArchSiteGuid = rdr.SafeGetGuid("ArchSiteGuid"),
                    ProjectID = rdr.SafeGetInt16("ProjectID"),
                    PermitID = rdr.SafeGetInt32("PermitID"),
                    SiteNumber = rdr.SafeGetString("SiteNumber")
                };
                ialist.Add(archsite);
            }

            CloseConnection();

            return ialist.ToArray();
        }

        public void SaveToDatabase(ShovelTestPit stp)
        {
            AddParametersFrom<ShovelTestPit>(stp, dbcommand);

            OpenConnection();

            dbcommand.CommandType = CommandType.StoredProcedure;
            dbcommand.CommandText = "V_SP_AddShovelTestPit";

            try
            {
                dbcommand.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\n" + e.InnerException,
                            "Stored Procedure Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            

            CloseConnection();
        }

        public static void AddParametersFrom<T>(T obj, SqlCommand dbcommand)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            Type t = obj.GetType();
            PropertyInfo[] properties = t.GetProperties(
                BindingFlags.Public | // Also include public properties
                BindingFlags.Instance // Specify to retrieve non static properties
            );

            foreach (PropertyInfo property in properties)
            {
                object value = t.GetProperty(property.Name).GetValue(obj);
                if (value == null) value = DBNull.Value;
                string parameterName = string.Format("@{0}", property.Name);
                
                if (dbcommand.Parameters.Contains(parameterName))
                {
                    dbcommand.Parameters[parameterName].Value = value;
                }
                else
                {
                    dbcommand.Parameters.Add(new SqlParameter(parameterName, value));
                }
            }
        }

        private void Connect()
        {
            try
            {
                dbConn = new SqlConnection(AEON_CONN_STRING);
                dbcommand = new SqlCommand();
                dbcommand.Connection = dbConn;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Data.ToString(),
                    "Database Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public bool OpenConnection()
        {
            dbConn.Open();
            return dbConn.State == ConnectionState.Open;
        }
        public bool CloseConnection()
        {
            dbConn.Close();
            return dbConn.State == ConnectionState.Closed;
        }
        private static double rad(double degrees)
        {
            return Math.PI * degrees / 180;
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
            UpdateStatus(StatusType.Success, string.Format("This version of ArchyManager will expire in {0} days.", DaysLeft));
        }
        private void RibbonApplicationMenu_Loaded(object sender, RoutedEventArgs e)
        {
            // removes 'recent' column in application menu (styling)
            RibbonApplicationMenu am = sender as RibbonApplicationMenu;
            Grid grid = (am.Template.FindName("MainPaneBorder", am) as Border).Parent as Grid;
            grid.ColumnDefinitions[2].Width = new GridLength(0);
        }


#endregion


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

        private void openSDF_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void exit_btn_Click(object sender, RoutedEventArgs e)
        {

        }
    }

    public class ShovelTestPit
    {
        public Guid ArchSiteGuid { get; set; }
        public string SiteNumber { get; set; }
        public string DatumID { get; set; }
        public Guid DatumGuid { get; set; }
        public string ProjectNumber { get; set; }
        public int? ProjectID { get; set; }
        public int? PermitID { get; set; }
        public string PermitNumber { get; set; }
        public DateTime SurveyDate { get; set; }
        public string PitID { get; set; } // need to ensure does not already exist...
        public string PitTool { get; set; } // need to ensure specified pittool exists
        public double? DatumDistance1 { get; set; }
        public double? DatumBearing1 { get; set; }
        public string DatumDirection1 { get; set; } // calculated
        public string STPNote { get; set; }
        public double? Longitude { get; set; } // calculated
        public double? Latitude { get; set; } // calculated
        public double? Easting { get; set; } // calculated
        public double? Northing { get; set; } // calculated
        public double? Elevation { get; set; }
        public byte? UTMZone { get; set; } // lookup if null
        

        // user also might add...
        public double? DatumDistance2 { get; set; }
        public double? DatumBearing2 { get; set; }
        public string DatumDirection2 { get; set; }
        public string Recorder { get; set; }
        public string Excavator { get; set; }
        public string PhotoFrom { get; set; }
        public string PhotoTo { get; set; }
        public string CameraNumber { get; set; }
        public string ArtefactsCollected { get; set; }
        public int? ArtefactCount { get; set; }
        public bool StruckWater { get; set; }


        // unlikely used
        public double? HDOP { get; set; }
        public double? PDOP { get; set; }
        public double? VDOP { get; set; }
        public byte? SatelliteCount { get; set; }
        public string SatelliteFix { get; set; }
        public int? PolygonID { get; set; }

        //public Guid STPGuid { get; set; }
        //public Guid DatumGuid { get; set; }    
}

    public class Datum
    {
        public string DatumID { get; set; }
        public double? Easting { get; set; }
        public double? Northing { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public byte? UTMZone { get; set; }
        public Guid DatumGuid { get; set; }
        public Guid ArchSiteGuid { get; set; }
    }

    public class ArchSite
    {
        public Guid ArchSiteGuid { get; set; }
        public string SiteNumber { get; set; }
        public int? ProjectID { get; set; }
        public int? PermitID { get; set; }
    }

    public static class ExtensionMethods
    {
        public static string ReplaceAll(this string seed, char[] chars, string replacement = "")
        {
            return chars.Aggregate(seed, (x, c) => x.Replace(c.ToString(), replacement));
        }
    }

    public static class SqlDataReaderExtensions
    {
        public static int SafeGetInt32(this SqlDataReader reader,
                                       string columnName, int defaultValue = 0)
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
        public static short SafeGetInt16(this SqlDataReader reader,
                                       string columnName, short defaultValue = 0)
        {
            int ordinal = reader.GetOrdinal(columnName);

            if (!reader.IsDBNull(ordinal))
            {
                return reader.GetInt16(ordinal);
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
















//        private string UPDATE = @"

//            UPDATE [ShovelTestPit] 
//            SET 
//                [ProjectID] = @ProjectID,
//                [DatumGuid] = @DatumGuid,
//                [Recorder] = @Recorder,
//                [SurveyDate] = @SurveyDate,
//                [Excavator] = @Excavator,
//                [PitID] = @PitID,
//                [DatumDistance1] = @DatumDistance1,
//                [DatumDirection1] = @DatumDirection1,
//                [DatumDistance2] = @DatumDistance2,
//                [DatumDirection2] = @DatumDirection2,
//                [PermitNumber] = @PermitNumber,
//                [ArtefactsCollected] = @ArtefactsCollected,
//                [ArtefactCount] = @ArtefactCount,
//                [STPNote] = @STPNote,
//                [Longitude] = @Longitude,
//                [Latitude] = @Latitude,
//                [Easting] = @Easting,
//                [Northing] = @Northing,
//                [UTMZone] = @UTMZone,
//                [HDOP] = @HDOP,
//                [PDOP] = @PDOP,
//                [VDOP] = @VDOP,
//                [SatelliteCount] = @SatelliteCount,
//                [SatelliteFix] = @SatelliteFix,
//                [StruckWater] = @StruckWater,
//                [Elevation] = @Elevation,
//                [PhotoFrom] = @PhotoFrom,
//                [PhotoTo] = @PhotoTo, 
//                [CameraNumber] = @CameraNumber,
//                [PolygonID] = @PolygonID,
//                [ArchSiteGuid] = @ArchSiteGuid, 
//                [PitToolID] = @PitToolID, 
//                [DatumBearing1] = @DatumBearing1, 
//                [DatumBearing2] = @DatumBearing2 
//            WHERE
//                [STPGuid] = @STPGuid

//";
//        private string INSERT = @"

//            INSERT INTO [ShovelTestPit]
//                ([ProjectID]
//                ,[DatumGuid]
//                ,[Recorder]
//                ,[SurveyDate]
//                ,[Excavator]
//                ,[PitID]
//                ,[DatumDistance1]
//                ,[DatumDirection1]
//                ,[DatumDistance2]
//                ,[DatumDirection2]
//                ,[PermitNumber]
//                ,[ArtefactsCollected]
//                ,[ArtefactCount]
//                ,[STPNote]
//                ,[Longitude]
//                ,[Latitude]
//                ,[Easting]
//                ,[Northing]
//                ,[UTMZone]
//                ,[HDOP]
//                ,[PDOP]
//                ,[VDOP]
//                ,[SatelliteCount]
//                ,[SatelliteFix]
//                ,[STPGuid]
//                ,[StruckWater]
//                ,[Elevation]
//                ,[PhotoFrom]
//                ,[PhotoTo]
//                ,[CameraNumber]
//                ,[PolygonID]
//                ,[ArchSiteGuid]
//                ,[PitToolID]
//                ,[DatumBearing1]
//                ,[DatumBearing2]) 
//            VALUES 
//                (@ProjectID
//                ,@DatumGuid
//                ,@Recorder
//                ,@SurveyDate
//                ,@Excavator
//                ,@PitID
//                ,@DatumDistance1
//                ,@DatumDirection1
//                ,@DatumDistance2
//                ,@DatumDirection2
//                ,@PermitNumber
//                ,@ArtefactsCollected
//                ,@ArtefactCount
//                ,@STPNote
//                ,@Longitude
//                ,@Latitude
//                ,@Easting
//                ,@Northing
//                ,@UTMZone
//                ,@HDOP
//                ,@PDOP
//                ,@VDOP
//                ,@SatelliteCount
//                ,@SatelliteFix
//                ,@STPGuid
//                ,@StruckWater
//                ,@Elevation
//                ,@PhotoFrom
//                ,@PhotoTo
//                ,@CameraNumber
//                ,@PolygonID
//                ,@ArchSiteGuid
//                ,@PitToolID
//                ,@DatumBearing1
//                ,@DatumBearing2)

//";

//        private string SELECT = @"
//            SELECT 
//                [ProjectID],
//                [DatumGuid],
//                [Recorder],
//                [SurveyDate],
//                [Excavator],
//                [PitID],
//                [DatumDistance1],
//                [DatumDirection1],
//                [DatumDistance2],
//                [DatumDirection2],
//                [PermitNumber],
//                [ArtefactsCollected],
//                [ArtefactCount],
//                [STPNote],
//                [Longitude],
//                [Latitude],
//                [Easting],
//                [Northing],
//                [UTMZone],
//                [HDOP],
//                [PDOP],
//                [VDOP],
//                [SatelliteCount],
//                [SatelliteFix],
//                [STPGuid],
//                [StruckWater],
//                [Elevation],
//                [PhotoFrom],
//                [PhotoTo],
//                [CameraNumber],
//                [PolygonID],
//                [ArchSiteGuid],
//                [PitToolID],
//                [DatumBearing1],
//                [DatumBearing2]
//            FROM
//                [ShovelTestPit]
//            WHERE
//                STPGuid = @STPGuid;

//";



//dbcommand.Parameters.Add(new SqlParameter("@ProjectID", stp.ProjectID));
//dbcommand.Parameters.Add(new SqlParameter("@DatumGuid", stp.ProjectID));
//dbcommand.Parameters.Add(new SqlParameter("@Recorder", stp.ProjectID));
//dbcommand.Parameters.Add(new SqlParameter(" @SurveyDate", stp.ProjectID));
//dbcommand.Parameters.Add(new SqlParameter("@Excavator", stp.ProjectID));
//dbcommand.Parameters.Add(new SqlParameter("@PitID", stp.ProjectID));
//dbcommand.Parameters.Add(new SqlParameter("@DatumDistance1", stp.ProjectID));
//dbcommand.Parameters.Add(new SqlParameter("@DatumDirection1", stp.ProjectID));
//dbcommand.Parameters.Add(new SqlParameter("@DatumDistance2", stp.ProjectID));
//dbcommand.Parameters.Add(new SqlParameter("@DatumDirection2", stp.ProjectID));
//dbcommand.Parameters.Add(new SqlParameter("@PermitNumber", stp.ProjectID));
//dbcommand.Parameters.Add(new SqlParameter("@ArtefactsCollected", stp.ProjectID));
//dbcommand.Parameters.Add(new SqlParameter("@ArtefactCount", stp.ProjectID));
//dbcommand.Parameters.Add(new SqlParameter("@STPNote", stp.ProjectID));
//dbcommand.Parameters.Add(new SqlParameter("@Longitude", stp.ProjectID));
//dbcommand.Parameters.Add(new SqlParameter("@Latitude", stp.ProjectID));
//dbcommand.Parameters.Add(new SqlParameter("@Easting", stp.ProjectID));
//dbcommand.Parameters.Add(new SqlParameter("@Northing", stp.ProjectID));
//dbcommand.Parameters.Add(new SqlParameter("@UTMZone", stp.ProjectID));
//dbcommand.Parameters.Add(new SqlParameter("@HDOP", stp.ProjectID));
//dbcommand.Parameters.Add(new SqlParameter("@PDOP", stp.ProjectID));
//dbcommand.Parameters.Add(new SqlParameter("@VDOP", stp.ProjectID));
//dbcommand.Parameters.Add(new SqlParameter("@SatelliteCount", stp.ProjectID));
//dbcommand.Parameters.Add(new SqlParameter("@SatelliteFix", stp.ProjectID));
//dbcommand.Parameters.Add(new SqlParameter("@StruckWater", stp.ProjectID));
//dbcommand.Parameters.Add(new SqlParameter("@Elevation", stp.ProjectID));
//dbcommand.Parameters.Add(new SqlParameter("@PhotoFrom", stp.ProjectID));
//dbcommand.Parameters.Add(new SqlParameter("@PhotoTo", stp.ProjectID));
//dbcommand.Parameters.Add(new SqlParameter("@CameraNumber", stp.ProjectID));
//dbcommand.Parameters.Add(new SqlParameter("@PolygonID", stp.ProjectID));
//dbcommand.Parameters.Add(new SqlParameter("@ArchSiteGuid", stp.ProjectID));
//dbcommand.Parameters.Add(new SqlParameter("@PitToolID", stp.ProjectID));
//dbcommand.Parameters.Add(new SqlParameter("@DatumBearing1", stp.ProjectID));
//dbcommand.Parameters.Add(new SqlParameter("@DatumBearing2", stp.ProjectID));