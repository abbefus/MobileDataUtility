using ABUtils;
using ArchyManager.Pages;
using Microsoft.Windows.Controls.Ribbon;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;

namespace ArchyManager
{

    public partial class ArchyMain : SecuredWindow
    {
        private Dictionary<string, Page> activepages = new Dictionary<string, Page>();

        [DllImport("user32.dll")]
        static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hwnd, int index, int newStyle);

        private const int GWL_STYLE = -16;
        private const int WS_DISABLED = 0x08000000;
        

        public ArchyMain()
            : base 
            (
#if DEBUG
            @"C:\Users\abefus\Documents\Visual Studio 2015\Projects\MobileDataUtility\ArchyManager\bin\Debug\ArchyManager.exe", //fixed folder location
                    "31-JAN-2017"  //expiry date
#elif FINAL
                  @"\\CD1002-F03\GEOMATICS\Utilities\Mobile\Data\Access\AEON\ArchyManager.exe", //fixed folder location
                    "31-JAN-2017"  //expiry date
#elif RELEASE
                  @"C:\Users\abefus\Documents\Visual Studio 2015\Projects\MobileDataUtility\ArchyManager\bin\Release\ArchyManager.exe", //fixed folder location
                    "31-JAN-2017"  //expiry date
#endif
            )
        {
            InitializeComponent();

            foreach (RibbonTab tab in archy_rbn.Items)
            {
                activepages.Add(tab.Name, null);
            }
        }

        private void SetNativeEnabled(bool enabled)
        {
            WindowInteropHelper helper = new WindowInteropHelper(this);
            IntPtr hwnd = helper.Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) &
                ~WS_DISABLED | (enabled ? 0 : WS_DISABLED));
        }


        #region Ribbon

        // need to implement some way to take advantage of navigator 
        private void Ribbon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RibbonTab tab = archy_rbn.SelectedItem as RibbonTab;
            frame.Navigate(activepages[tab.Name]);
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
            Dispatcher.Invoke(() =>
            {
                status_tb.Foreground = type == StatusType.Failure ?
                new SolidColorBrush(Colors.Red) :
                new SolidColorBrush((Color)ColorConverter.ConvertFromString("#b84c00"));
                Console.WriteLine(msg);
            });
        }


        private void exit_btn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void viewsWB_btn_Click(object sender, RoutedEventArgs e)
        {

        }

        
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

    public interface IUploadable
    {
        string DefaultGuid { get; }
        bool IsUploaded { get; set; }
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