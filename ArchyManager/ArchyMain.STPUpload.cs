using ABSSTools;
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ArchyManager
{
    public partial class ArchyMain
    {
        
        private void openSTP_btn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Files|*.xls;*.xlsx";
            if (openFileDialog.ShowDialog() == true)
            {
                DataTableCollection data = ExcelReader.Read(openFileDialog.FileName);
                if (data != null)
                {
                    STPDataPage dp = new STPDataPage();

                    DataTable table = data[0];
                    string[] headers = table.Rows[0].ItemArray.Cast<string>().ToArray();
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        table.Columns[i].ColumnName = headers[i].ToString();
                    }
                    dp.DataRows = data[0].Rows.Cast<DataRow>().Skip(1).ToArray();
                    
                    // if header is named correctly, 
                    foreach (PropertyInfo property in typeof(ShovelTestPitExtended).GetProperties())
                    {
                        if (headers.Contains(property.Name))
                        {
                            SqlUtils.SetPropertyBrowsable(typeof(ShovelTestPitExtended), property.Name, true);
                            dp.ColumnMap.Add(property.Name, property.Name);
                        }
                        else
                        {
                            SqlUtils.SetPropertyBrowsable(typeof(ShovelTestPitExtended), property.Name, false);
                        }
                    }
                    foreach (string header in headers.Where(x => !dp.ColumnMap.Keys.Contains(x)))
                    {
                        dp.ColumnMap.Add(header, "Unmapped");
                    }

                    dp.UpdateDataGridFromMapping();

                    activepages["field_tab"] = null;
                    activepages["field_tab"] = dp;
                    frame.Navigate(dp);
                }
            }



            //if (stps != null)
            //{
            //    ArchSite2[] archsites = QueryArchSites(stps.Select(x => x.SiteNumber));
            //    Datum2[] datums = QueryDatums(stps.Select(x => x.DatumID));



            //    for (int i = 0; i < stps.Length; i++)
            //    {
            //        ArchSite2 archsite = archsites.Where(x => x.SiteNumber == stps[i].SiteNumber).FirstOrDefault();
            //        if (archsite == null)
            //        {
            //            stps[i].ArchSiteGuid = Guid.Empty;
            //            MessageBox.Show(string.Format("Datum {0} not found.", stps[i].DatumID),
            //                "Archsite not found", MessageBoxButton.OK, MessageBoxImage.Warning);
            //            continue;
            //        }
            //        stps[i].ArchSiteGuid = archsite.ArchSiteGuid;
            //        stps[i].ProjectID = archsite.ProjectID;
            //        if (string.IsNullOrWhiteSpace(stps[i].PermitNumber))
            //        {
            //            stps[i].PermitID = archsite.PermitID;
            //        }

            //        Datum2 datum = datums.Where(x => x.DatumID == stps[i].DatumID && x.ArchSiteGuid == stps[i].ArchSiteGuid).FirstOrDefault();
            //        if (datum == null)
            //        {
            //            stps[i].DatumGuid = Guid.Empty;
            //            MessageBox.Show(string.Format("Datum {0} not found.", stps[i].DatumID),
            //                "Datum not found", MessageBoxButton.OK, MessageBoxImage.Warning);
            //            continue;
            //        }
            //        stps[i].DatumGuid = datum.DatumGuid;

            //        if (!stps[i].DatumBearing1.HasValue || !stps[i].DatumDistance1.HasValue || !datum.Easting.HasValue || !datum.Northing.HasValue)
            //        {
            //            MessageBox.Show(string.Format("Unable to calculate coordinates for {0}", stps[i].PitID),
            //                "Missing Data", MessageBoxButton.OK, MessageBoxImage.Warning);
            //            continue;
            //        }
            //        double bearing = stps[i].DatumBearing1 ?? 0;
            //        double distance = stps[i].DatumDistance1 ?? 0;
            //        double easting = datum.Easting ?? 0;
            //        double northing = datum.Northing ?? 0;
            //        stps[i].Easting = easting + (distance * Math.Sin(rad(90 - bearing)));
            //        stps[i].Northing = northing + (distance * Math.Cos(rad(90 - bearing)));

            //        if (!datum.UTMZone.HasValue)
            //        {
            //            MessageBox.Show(string.Format("Unable to calculate latitude and Longitude for {0}", stps[i].PitID),
            //                "No UTMZone Specified", MessageBoxButton.OK, MessageBoxImage.Warning);
            //        }
            //        else
            //        {
            //            double longitude = 0;
            //            double latitude = 0;
            //            LatLongCalc.UTMtoLL(northing, easting, (int?)datum.UTMZone ?? 0, ref longitude, ref latitude);
            //            stps[i].Longitude = longitude;
            //            stps[i].Latitude = latitude;
            //            stps[i].UTMZone = datum.UTMZone;
            //        }
            //    }



            //    foreach (ShovelTestPit stp in stps.Where(x => x.ArchSiteGuid != Guid.Empty && x.DatumGuid != Guid.Empty))
            //    {
            //        SaveToDatabase(stp);
            //    }
            //}
        }

        private void mapSTP_btn_Click(object sender, RoutedEventArgs e)
        {
            STPDataPage dp = activepages["field_tab"] as STPDataPage;
            MapColumnsDialog dialog = new MapColumnsDialog(dp.ColumnMap,typeof(ShovelTestPitExtended));
            if (dialog.ShowDialog() ?? false)
            {
                dp.ColumnMap = dialog.OMToDictionary();
            }
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

        private Datum2[] QueryDatums(IEnumerable<string> datumids)
        {
            StringBuilder commandSB = new StringBuilder();
            commandSB.AppendLine("SELECT DatumID, Easting, Northing, UTMZone, DatumGuid, ArchSiteGuid")
                    .AppendLine("FROM [Archy2014].[dbo].[Datum]")
                    .AppendLine(string.Format("WHERE DatumID IN ('{0}')",
                                    string.Join("','", datumids.Distinct())));
            List<Datum2> datumlist = null;

            using (SqlConnection conn = SQL2008.ConnectUsing(SQL2008.ARCHY2014_DB))
            {
                using (SqlCommand comm = new SqlCommand(commandSB.ToString(), conn))
                {
                    comm.CommandType = CommandType.Text;
                    SqlDataReader rdr = comm.ExecuteReader();

                    datumlist = new List<Datum2>();
                    while (rdr.Read())
                    {
                        Datum2 datum = new Datum2
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
                }
            };

            return datumlist.ToArray();
        }
        private ArchSite2[] QueryArchSites(IEnumerable<string> sitenumbers)
        {
            StringBuilder commandSB = new StringBuilder();
            commandSB.AppendLine("SELECT ArchSiteGuid, ProjectID, PermitID, SiteNumber")
                    .AppendLine("FROM [Archy2014].[dbo].[ArchSite]")
                    .AppendLine(string.Format("WHERE SiteNumber IN ('{0}')", string.Join("','", sitenumbers.Distinct())));
            List<ArchSite2> ialist = null;

            using (SqlConnection conn = SQL2008.ConnectUsing(SQL2008.ARCHY2014_DB))
            {
                using (SqlCommand comm = new SqlCommand(commandSB.ToString(), conn))
                {
                    comm.CommandType = CommandType.Text;
                    SqlDataReader rdr = comm.ExecuteReader();

                    ialist = new List<ArchSite2>();
                    while (rdr.Read())
                    {
                        ArchSite2 archsite = new ArchSite2
                        {
                            ArchSiteGuid = rdr.SafeGetGuid("ArchSiteGuid"),
                            ProjectID = rdr.SafeGetInt16("ProjectID"),
                            PermitID = rdr.SafeGetInt32("PermitID"),
                            SiteNumber = rdr.SafeGetString("SiteNumber")
                        };
                        ialist.Add(archsite);
                    }
                }
            }
            return ialist.ToArray();
        }


        // saves the STPCollection, as is, to the database
        public void SaveToDatabase()
        {
            // make sure datapage is correct
            STPDataPage dp = activepages["field_tab"] is STPDataPage ? activepages["field_tab"] as STPDataPage : null;
            if (dp == null)
            {
                MessageBox.Show("The ShovelTestPit Data Page cannot be found.", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // set defaults so that all needed attributes are sent to SQL Server
            dp.STPCollection[0].SetBrowsableDefaults();

            // upload all the data
            foreach (ShovelTestPitExtended stp in dp.STPCollection)
            {
                using (SqlConnection conn = SQL2008.ConnectUsing(SQL2008.ARCHY2014_DB))
                {
                    SqlUtils.ExecuteSP(stp.ToShovelTestPit(), conn, SPString.ADDNEW);
                }
            }

            

        }


        private static double rad(double degrees)
        {
            return Math.PI * degrees / 180;
        }




    }

    public class Datum2
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

    public class ArchSite2
    {
        public Guid ArchSiteGuid { get; set; }
        public string SiteNumber { get; set; }
        public int? ProjectID { get; set; }
        public int? PermitID { get; set; }
    }
}
