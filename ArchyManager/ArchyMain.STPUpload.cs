using ABSSTools;
using ABUtils;
using ArchyManager.Classes;
using ArchyManager.Classes.Archy2014;
using ArchyManager.Dialogs;
using ArchyManager.Pages;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ArchyManager
{
    // Notes: 
    //  -uploaded STPs are expected to be within 10 years of current datetime

    public partial class ArchyMain
    {

        private IEnumerable<string> HideUnhideColumns(IEnumerable<string> headers, Type classType)
        {
            foreach (PropertyInfo property in classType.GetProperties())
            {
                if (headers.Contains(property.Name))
                {
                    SqlUtils.SetPropertyBrowsable(classType, property.Name, true);
                    yield return property.Name;
                }
                else
                {
                    SqlUtils.SetPropertyBrowsable(classType, property.Name, false);
                }
            }
        }

        private void openSTP_btn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Files|*.xls;*.xlsx";
            if (openFileDialog.ShowDialog() == true)
            {
                DataTableCollection data = ExcelReader.Read(openFileDialog.FileName);
                if (data != null)
                {
                    DataTable table = data[0];
                    //populate column names in table (assumes first row is headers)
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        table.Columns[i].ColumnName = table.Rows[0].ItemArray[i].ToString();
                    }

                    STPDataPage dp = new STPDataPage();
                    dp.DataRows = data[0].Rows.Cast<DataRow>().Skip(1)
                        .Where(x => !x.ItemArray.All(y => y == DBNull.Value)).ToArray();

                    // if no data found, tell user before stopping
                    if (dp.DataRows.Length < 1)
                    {
                        MessageBox.Show("No data found in file. Ensure data is contained\r\nin first tab of Excel file.",
                            "No Data Found", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    ClearFrame(FIELD_TAB);

                    // hide columns to simplify datagrid for viewing - exact matches get shown
                    string[] headers = table.Columns.Cast<DataColumn>().Select(x => x.ColumnName).ToArray();
                    string[] matches = HideUnhideColumns(headers, typeof(ShovelTestPitExtended)).ToArray();

                    // populate ColumnMap and create datagrid
                    foreach (string header in headers)
                    {
                        if (matches.Contains(header))
                        {
                            dp.ColumnMap.Add(header, header);
                        }
                        else
                        {
                            dp.ColumnMap.Add(header, "Unmapped");
                        }
                    }
                    dp.UpdateDataGridFromMapping();

                    activepages[FIELD_TAB] = dp;
                    frame.Navigate(dp);
                    UpdateLookups();
                }
            }
        }

        private void uploadSTP_btn_Click(object sender, RoutedEventArgs e)
        {
            UpdateLookups();
            SaveToDatabase();
        }

        // saves STPCollection to the database
        public async void SaveToDatabase()
        {
            STPDataPage dp = activepages[FIELD_TAB] is STPDataPage ? activepages[FIELD_TAB] as STPDataPage : null;
            if (dp == null)
            {
                MessageBox.Show("The ShovelTestPit Data Page cannot be found.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                frame.Content = null;
                return;
            }
            
            List<ShovelTestPit> stps = new List<ShovelTestPit>();
            StringBuilder log = new StringBuilder();

            Action<IProgress<double>> sqlwork = (iprogress) =>
            {
                float tots = dp.STPCollection.Count;
                float count = 0;
                foreach (ShovelTestPitExtended stpex in dp.STPCollection)
                {
                    ShovelTestPit stp = stpex.ToShovelTestPit();
                    log.Append(string.Format("PitID: [ {0} ] => ", stp.PitID));

                    // if any of these fail, this stp is not fit for the database
                    if (dp.STPCollection.Count(x => x.PitID == stp.PitID) > 1)
                    {
                        stp.IsUploadable = false;
                        log.AppendLine("Failed: Duplicate PitID found");
                        continue;
                    }
                    if (stp.SurveyDate != null && (DateTime.Now.Subtract(stp.SurveyDate).Days / 365) > 10)
                    {
                        stp.IsUploadable = false;
                        log.AppendLine("Failed: Invalid SurveyDate");
                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(stp.PitID))
                    {
                        stp.IsUploadable = false;
                        log.AppendLine("Failed: Invalid PitID");
                        continue;
                    }
                    if (!ValidateSTP(ref stp, stpex, dp.STPLookups))
                    {
                        stp.IsUploadable = false;
                        log.AppendLine("Failed: Check ProjectNumber");
                        continue;
                    }
                    if (!ValidateLocation(ref stp, stpex, dp.STPLookups.DatumLU))
                    {
                        stp.IsUploadable = false;
                        ArchSite archsite = dp.STPLookups.ArchSiteLU.Where(site => site.ArchSiteGuid == stp.ArchSiteGuid).FirstOrDefault();
                        string logstring = (archsite != null && archsite.ProjectID != stp.ProjectID) ?
                            "Failed: Mismatch on IA ProjectID" :
                            "Failed: Cannot locate. Check SiteNumber and DatumID";
                        log.AppendLine(logstring);
                        continue;
                    }

                    stp.PitToolID = dp.STPLookups.PitToolLU.Where(x => x.PitTool.ToUpper() == stpex.PitTool.ToUpper()).FirstOrDefault().PitToolID;

                    // try to upload the stp
                    using (SqlConnection conn = SQL2008.ConnectUsing(SQL2008.ARCHY2014_DB))
                    {
                        string sqlmessage;
                        stp.IsUploaded = SqlUtils.ExecuteSP(stp, conn, SPString.ADDNEW, out sqlmessage, false);
                        if (stp.IsUploaded)
                        {
                            log.AppendLine("Successfully uploaded");
                        }
                        else
                        {
                            log.Append("Failed: ").AppendLine(sqlmessage);
                        }
                    }
                    stps.Add(stp);
                    iprogress.Report((++count / tots) * 100);
                }
            };
            Progress<double> progress = new Progress<double>(x => progressbar.Value = x);
            await Task.Run(() =>
            {
                sqlwork(progress);
            });
            progressbar.Value = 0;

            int winners = stps.Count(x => x.IsUploaded);
            int losers = stps.Count(x => !x.IsUploaded);
            if (winners > 0)
            {
                UpdateStatus(StatusType.Success, string.Format("{0} stps uploaded; {1} failed.", winners, losers));
            }
            else
            {
                UpdateStatus(StatusType.Failure, "Complete and utter failure.");
            }

            ScrollableTextDialog dialog = new ScrollableTextDialog(log.ToString(), "Details:", "Upload Complete");
            dialog.ShowDialog();
        }

        // populates ProjectID, attempts ArchSiteGuid and PermitNumber - note uses latest ArchSite if more than one found
        public bool ValidateSTP(ref ShovelTestPit stp, ShovelTestPitExtended stpex, STPLookups stpLU)
        {
            // REQUIRED: CORRECT PROJECTNUMBER
            if (!string.IsNullOrWhiteSpace(stpex.ProjectNumber) && stpLU.ProjectLU != null)
            {
                Project project = stpLU.ProjectLU.Where(x => x.ProjectNumber == stpex.ProjectNumber).FirstOrDefault();
                if (project != default(Project)) stp.ProjectID = project.ProjectID;
            }
            if (stp.ProjectID < 1) return false;
            stpex.ProjectID = stp.ProjectID;

            // OPTIONAL: PERMIT NUMBER
            if (!string.IsNullOrWhiteSpace(stpex.Permit) && stpLU.PermitLU != null)
            {
                STPLookups.Permit permit = stpLU.PermitLU.Where(x => x.PermitNumber == stpex.Permit).FirstOrDefault();
                if (permit != default(STPLookups.Permit)) stp.PermitNumber = permit.PermitID; // and herein lies the inconsistency Matt >=/
            }

            // OPTIONAL: SITENUMBER, HOWEVER IF PROVIDED ARCHSITE PROJECTID MUST MATCH
            if (!string.IsNullOrWhiteSpace(stpex.SiteNumber) && stpLU.ArchSiteLU != null)
            {
                ArchSite archsite = stpLU.ArchSiteLU.Where(x => x.SiteNumber == stpex.SiteNumber && x.ProjectID == stpex.ProjectID)
                                                    .OrderByDescending(x => x.SurveyDate).FirstOrDefault(); // if more than one use the latest
                if (archsite != default(ArchSite))
                {
                    stp.ArchSiteGuid = archsite.ArchSiteGuid;
                    if (stp.PermitNumber < 1)
                    {
                        stp.PermitNumber = archsite.PermitID; // >=/
                    }
                }
            }

            // as long as there is a projectid, this thing is uploadable
            return stp.ProjectID > 0;
        }
        public bool ValidateLocation(ref ShovelTestPit stp, ShovelTestPitExtended stpex, Datum[] datums)
        {
            // use ArchSiteGuid and DatumID to get Datum
            Guid archsiteguid = stp.ArchSiteGuid ?? Guid.Empty;
            Datum datum = datums.Where(x => x.ArchSiteGuid.Equals(archsiteguid) && stpex.DatumID == x.DatumID).FirstOrDefault();
            stp.DatumGuid = datum != null ? datum.DatumGuid : null;

            double longitude = 0;
            double latitude = 0;
            double easting = 0;
            double northing = 0;

            // if lat and long but no utm
            if ((stp.Longitude.HasValue && stp.Latitude.HasValue) && (!stp.Easting.HasValue || !stp.Northing.HasValue || !stp.UTMZone.HasValue))
            {
                stp.UTMZone = (byte)GISConversion.LatLongToUTM((double)stp.Latitude, (double)stp.Longitude, ref easting, ref northing, stp.UTMZone);
                stp.Easting = easting;
                stp.Northing = northing;
            }
            // if utm but no lat long
            else if ((stp.Easting.HasValue && stp.Northing.HasValue && stp.UTMZone.HasValue) && (!stp.Latitude.HasValue || !stp.Longitude.HasValue))
            {
                GISConversion.UTMtoLL((double)stp.Northing, (double)stp.Easting, (int)stp.UTMZone, ref longitude, ref latitude);
                stp.Latitude = latitude;
                stp.Longitude = longitude;
            }
            // if no useable location data
            else if ((!stp.Longitude.HasValue || !stp.Latitude.HasValue) && (!stp.Easting.HasValue || !stp.Northing.HasValue || !stp.UTMZone.HasValue))
            {
                if (!stp.ArchSiteGuid.HasValue || stp.ArchSiteGuid == default(Guid) || 
                    string.IsNullOrWhiteSpace(stpex.DatumID)) return false; // cannot get datum without ArchSiteGuid and DatumID

                

                // convert direction and bearing values
                if (stp.DatumBearing1.HasValue && string.IsNullOrWhiteSpace(stp.DatumDirection1))
                {
                    stp.DatumDirection1 = DirectionFromBearing((double)stp.DatumBearing1);
                }
                else if (!string.IsNullOrWhiteSpace(stp.DatumDirection1))
                {
                    stp.DatumBearing1 = BearingFromDirection(stp.DatumDirection1);
                }
                if (stp.DatumBearing2.HasValue && string.IsNullOrWhiteSpace(stp.DatumDirection2))
                {
                    stp.DatumDirection2 = DirectionFromBearing((double)stp.DatumBearing2);
                }
                else if (!string.IsNullOrWhiteSpace(stp.DatumDirection2))
                {
                    stp.DatumBearing2 = BearingFromDirection(stp.DatumDirection2);
                }

                if ((stp.DatumBearing1.HasValue && stp.DatumDistance1.HasValue) || 
                    (stp.DatumBearing2.HasValue && stp.DatumDistance2.HasValue) && datum != default(Datum))
                {
                    double bearing = stp.DatumBearing1 ?? stp.DatumBearing2 ?? 0;
                    // use the best distance
                    double distance = (bearing == stp.DatumBearing1) ? 
                        stp.DatumDistance1 ?? (double)stp.DatumDistance2 : 
                        stp.DatumDistance2 ?? (double)stp.DatumDistance1;
                    easting = (double)datum.Easting;
                    northing = (double)datum.Northing;

                    stp.Easting = easting + (distance * Math.Sin(rad(90 - bearing)));
                    stp.Northing = northing + (distance * Math.Cos(rad(90 - bearing)));
                    stp.UTMZone = datum.UTMZone ?? 10;

                    GeographicCoordinate gc = GISConversion.UTMXYToLatLon
                    (
                        (double)stp.Easting, (double)stp.Northing, (int)stp.UTMZone,
                        Hemisphere.Northern, GISConversion.NAD83
                    );
                    stp.Longitude = gc.Longitude;
                    stp.Latitude = gc.Latitude;
                }
                else
                {
                    return false;
                }
            }
            // otherwise we good
            return true;
        }

        // can't determine if duplicate pit ids
        private string GetFailureReport(ShovelTestPit stp, STPLookups lus)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Upload failed. ");
            if (stp.ProjectID < 1) sb.Append("Invalid project number. ");
            if ((DateTime.Now.Subtract(stp.SurveyDate).Days / 365) > 10) sb.Append("Invalid survey date. ");
            if (string.IsNullOrWhiteSpace(stp.PitID)) sb.Append("Invalid pit ID. ");
            if (lus.ArchSiteLU.Where(site => site.ArchSiteGuid == stp.ArchSiteGuid).FirstOrDefault().ProjectID != stp.ProjectID)
            {
                sb.Append("Mismatch on IA ProjectID. ");
            }
            if (sb.ToString() == "Upload failed. ") sb.Append("Record may already exist in database.");
            return sb.ToString();
        }

        private void mapSTP_btn_Click(object sender, RoutedEventArgs e)
        {
            STPDataPage dp = activepages["field_tab"] as STPDataPage;
            string[] exceptions = new string[5] { "STPGuid", "ProjectID", "ArchSiteGuid", "PermitNumber", "DatumGuid" };
            MapColumnsDialog dialog = new MapColumnsDialog(dp.ColumnMap,typeof(ShovelTestPitExtended), exceptions);

            if (dialog.ShowDialog() ?? false)
            {
                dp.ColumnMap = dialog.OMToDictionary();
                string[] matches = HideUnhideColumns(dp.ColumnMap.Values.Where(x => x != "Unmapped"), typeof(ShovelTestPitExtended)).ToArray();
                dp.UpdateDataGridFromMapping(matches);
                UpdateLookups();
            }
        }

        private async void UpdateLookups()
        {
            STPDataPage dp = null;

            //handles case that something is running namely this method
            if (activepages["field_tab"] is STPDataPage)
            {
                if (progressbar.Value > 0)
                {
                    dp = activepages["field_tab"] as STPDataPage;
                    if (dp.DataRows.Length > 0)
                    {
                        Utils.DelayAction(5, new Action(UpdateLookups));
                        return;
                    }
                }
            }
            else
            {
                return;
            }
            if (dp == null) dp = activepages["field_tab"] as STPDataPage;
            if (dp == null) return;



            Action<IProgress<double>> sqlwork = (iprogress) =>
            {
                using (SqlConnection conn = SQL2008.ConnectUsing(SQL2008.ARCHY2014_DB))
                {
                    string[] properties = dp.ColumnMap.Values.Distinct().Where(x => x != "Unmapped").ToArray();
                    string sql = "SELECT * FROM [Archy2014].[dbo].[{0}]";
                    iprogress.Report(5);

                    //PitToolLU------------------------------------------------------------------------------------
                    if (dp.STPLookups.PitToolLU == null && properties.Contains("PitTool"))
                    {
                        string query = string.Format(sql, "LUPitTool");
                        dp.STPLookups.PitToolLU = SqlUtils.SQLToDataModel<STPLookups.LUPitTool>(conn, query);
                        if (dp.STPLookups.PitToolLU.Length == 0)
                        {
                            dp.STPLookups.PitToolLU = null;
                            MessageBox.Show("Data in PitTool column does not match anything in database.",
                                "Need Levenshtein?", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    iprogress.Report(20);
                    //PitToolLU------------------------------------------------------------------------------------


                    sql = "SELECT {0} FROM [Archy2014].[dbo].[{1}] WHERE {2} IN {3}";
                    //Project--------------------------------------------------------------------------------------
                    // must have empty ProjectLU and populate ProjectNumber column
                    if ((dp.STPLookups.ProjectLU == null || dp.STPLookups.ProjectLU.Length < 1) 
                            && properties.Contains("ProjectNumber"))
                    {
                        string[] projectNumbers = dp.GetColumnByProperty<string>("ProjectNumber").Distinct().ToArray();
                        iprogress.Report(25);
                        if (projectNumbers != null)
                        {
                            string pnums = SqlUtils.GetSQLArray(projectNumbers);
                            string query = string.Format(sql, "*", "Project", "ProjectNumber", pnums);
                            dp.STPLookups.ProjectLU = SqlUtils.SQLToDataModel<Project>(conn, query);
                            if (dp.STPLookups.ProjectLU.Length == 0)
                            {
                                dp.STPLookups.ProjectLU = null;
                                MessageBox.Show("Data in ProjectNumber column does not match anything in database.", 
                                    "No Match Found",MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    }
                    iprogress.Report(45);
                    //Project--------------------------------------------------------------------------------------

                    //Permit---------------------------------------------------------------------------------------
                    // must have empty PermitLU and populated ProjectLU
                    if ((dp.STPLookups.ProjectLU != null && dp.STPLookups.ProjectLU.Length > 0) 
                        && dp.STPLookups.PermitLU == null)
                    {
                        string pids = SqlUtils.GetSQLArray(dp.STPLookups.ProjectLU
                                                .Select(x => x.ProjectID.ToString()),false);
                        iprogress.Report(50);
                        string query = string.Format(sql, "*", "Permit", "ProjectID", pids);
                        dp.STPLookups.PermitLU = SqlUtils.SQLToDataModel<STPLookups.Permit>(conn, query);
                        if (dp.STPLookups.PermitLU.Length == 0)
                        {
                            dp.STPLookups.PermitLU = null;
                            MessageBox.Show("Data in Permit column does not match anything in database.",
                                "Need Levenshtein?", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    iprogress.Report(65);
                    //Permit---------------------------------------------------------------------------------------

                    #region Get ArchSite
                    //ArchSite-------------------------------------------------------------------------------------
                    // must have empty ArchSiteLU and SiteNumber column
                    if (dp.STPLookups.ArchSiteLU == null && properties.Contains("SiteNumber"))
                    {
                        string[] siteNumbers = dp.GetColumnByProperty<string>("SiteNumber").Distinct().ToArray();
                        iprogress.Report(70);
                        if (siteNumbers != null)
                        {
                            string snums = SqlUtils.GetSQLArray(siteNumbers);
                            string cols = "ArchSiteGuid, SiteNumber, ProjectID, PermitID, SurveyDate";
                            string query = string.Format(sql, cols, "ArchSite", "SiteNumber", snums);
                            dp.STPLookups.ArchSiteLU = SqlUtils.SQLToDataModel<ArchSite>(conn, query);
                            iprogress.Report(75);
                            if (dp.STPLookups.ArchSiteLU.Length == 0)
                            {
                                dp.STPLookups.ProjectLU = null;
                                MessageBox.Show("Data in ProjectNumber column does not match anything in database.",
                                    "No Match Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                            else if (dp.STPLookups.ProjectLU == null)
                            {
                                string projids = SqlUtils.GetSQLArray(dp.STPLookups.ArchSiteLU
                                                .Select(x => x.ProjectID.ToString()));
                                query = string.Format(sql, "*", "Project", "ProjectID", projids);
                                dp.STPLookups.ProjectLU = SqlUtils.SQLToDataModel<Project>(conn, query);
                                
                                // if ProjectLU is null then PermitLU is definitely null
                                query = string.Format(sql, "*", "Permit", "ProjectID", projids);
                                dp.STPLookups.PermitLU = SqlUtils.SQLToDataModel<STPLookups.Permit>(conn, query);
                            }
                        }
                    }
                    iprogress.Report(85);
                    //ArchSite-------------------------------------------------------------------------------------
                    #endregion

                    //Datum----------------------------------------------------------------------------------------
                    // must have empty DatumLU, populated ArchSiteLU, and a DatumID column
                    if (dp.STPLookups.DatumLU == null && properties.Contains("DatumID"))
                    {
                        if (dp.STPLookups.ArchSiteLU == null || dp.STPLookups.ArchSiteLU.Length == 0)
                        {
                            iprogress.Report(100);
                            MessageBox.Show("A unique DatumID cannot be stored in ShovelTestPit if there are no SiteNumbers.",
                                "Unique STP DatumID Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                        else
                        {
                            string guids = SqlUtils.GetSQLArray(dp.STPLookups.ArchSiteLU
                                                .Select(x => x.ArchSiteGuid.ToString()));
                            iprogress.Report(90);
                            string query = string.Format(sql, "*", "Datum", "ArchSiteGuid", guids);
                            dp.STPLookups.DatumLU = SqlUtils.SQLToDataModel<Datum>(conn, query);
                            if (dp.STPLookups.DatumLU.Length == 0)
                            {
                                dp.STPLookups.DatumLU = null;
                                MessageBox.Show("Data in SiteNumber does not match anything in Datum table.",
                                    "No Matches Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    }
                    //Datum----------------------------------------------------------------------------------------
                }
                iprogress.Report(0);
            };

            Progress<double> progress = new Progress<double>(x => progressbar.Value = x);
            await Task.Run(() =>
            {
                sqlwork(progress);
            });
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
            Console.WriteLine(string.Format("'{0}'", string.Join("','", stplist.Select(x => x.PitID))));
            return stplist.ToArray();


        }


        private static string DirectionFromBearing(double bearing)
        {
            if (bearing < 22.5 && bearing > 337.5) return "N";
            if (bearing > 22.5 && bearing < 67.5) return "NE";
            if (bearing > 67.5 && bearing < 112.5) return "E";
            if (bearing > 112.5 && bearing < 157.5) return "SE";
            if (bearing > 157.5 && bearing < 202.5) return "S";
            if (bearing > 202.5 && bearing < 247.5) return "SW";
            if (bearing > 247.5 && bearing < 292.5) return "W";
            if (bearing > 292.5 && bearing < 337.5) return "NW";
            return null;
        }

        private static double? BearingFromDirection(string direction)
        {
            switch (direction.ToUpper())
            {
                case "N":
                    return 0;
                case "NE":
                    return 45;
                case "E":
                    return 90;
                case "SE":
                    return 135;
                case "S":
                    return 180;
                case "SW":
                    return 225;
                case "W":
                    return 270;
                case "NW":
                    return 315;
                default:
                    return null;
            }
        }


        

       


        private static double rad(double degrees)
        {
            return Math.PI * degrees / 180;
        }




    }
    public class IsSTPConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                return value is STPDataPage;
            }
            return false;

        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    } // for testing bindings

}






//string property = dp.ColumnMap[key];

////we know it fails to convert to this property
//PropertyInfo pInfo = typeof(ShovelTestPitExtended).GetProperty(property);
//string[] datas = dp.DataRows.Select(row => row[key].ToString()).Distinct().ToArray();
//Dictionary<string, string> lu = new Dictionary<string, string>();
//string query;

//switch (pInfo.Name)
//{
//// likely entered ProjectName
//case "ProjectNumber":

//    query = "SELECT * FROM [Archy2014].[dbo].[Project] WHERE ProjectID > 0";
//    dp.STPLookups.ProjectLU = SqlUtils.SQLToDataModel<Project>(conn,query);
//    foreach (string data in datas)
//    {
//        KeyValuePair<string, string> kvp = new KeyValuePair<string, string>
//        (
//            data,
//            dp.STPLookups.ProjectLU
//                .Aggregate((x, y) => Utils.LevenshteinDistance.Compute(data, x.ProjectName) <
//                Utils.LevenshteinDistance.Compute(data, y.ProjectName) ? x : y).ProjectName
//        );
//MessageBoxResult mbr = MessageBox.Show(string.Format("Is {0} your intended project name for {1}?", kvp.Value, kvp.Key),
//"Attempting to Align ProjectName", MessageBoxButton.YesNo, MessageBoxImage.Question);
//        if (mbr == MessageBoxResult.Yes)
//        {
//            lu.Add(kvp.Key, kvp.Value);
//        }
//        else
//        {
//            MessageBox.Show(string.Format("Unable to convert to {0}.\r\nYou will need to reformat this column in\r\norder to include it in the export.", pInfo.Name),
//            "Unable to Convert Table Values", MessageBoxButton.OK, MessageBoxImage.Exclamation);
//        }
                                                
//    }
//    foreach (KeyValuePair<string, string> kvp in lu)
//    {
//        foreach (DataRow row in dp.DataRows)
//        {
//            if (row[key].ToString() == kvp.Key) row[key] = kvp.Value;
//        }
//    }
//    break;
//// likely entered PitTool
//case "PitToolID":
//    query = "SELECT * FROM [Archy2014].[dbo].[LUPitTool]";
//    dp.STPLookups.PermitLU = SqlUtils.SQLToDataModel<STPLookups.Permit>(conn, query);
//    break;
//default:
//    MessageBox.Show(string.Format("Unable to convert {0}.\r\nYou will need to reformat this column in\r\norder to include it in the export.", pInfo.Name),
//        "Unable to Convert Table Values", MessageBoxButton.OK, MessageBoxImage.Exclamation);
//    break;
//}