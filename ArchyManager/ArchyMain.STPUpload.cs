using ABSSTools;
using ArchyManager.Classes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ArchyManager
{
    public partial class ArchyMain
    {
        public const string AEON_CONN_STRING = "Data Source=sqlprod3\\sql2008;Initial Catalog=Archy2014;User Id=developer;Password=sp1d3r5!;";
        private SqlConnection dbConn;
        private SqlCommand dbcommand;
        private string SQL;

        private ShovelTestPit2[] ReadSTPs(DataSet dataset)
        {
            char[] invalidchars = new char[] { ' ', '_', '-' };

            List<ShovelTestPit2> stplist = new List<ShovelTestPit2>();
            string[] headers = dataset.Tables[0].Rows[0].ItemArray.Where(x => x != DBNull.Value).Cast<string>().ToArray();

            for (int i = 1; i < dataset.Tables[0].Rows.Count; i++)
            {
                ShovelTestPit2 stp = new ShovelTestPit2();
                Type targetType = typeof(Object);
                Type sourceType = typeof(Object);
                Type stpType = typeof(ShovelTestPit2);
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
        private void browsefile_btn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ShovelTestPit2[] stps = null;

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
                ArchSite2[] archsites = QueryArchSites(stps.Select(x => x.SiteNumber));
                Datum2[] datums = QueryDatums(stps.Select(x => x.DatumID));



                for (int i = 0; i < stps.Length; i++)
                {
                    ArchSite2 archsite = archsites.Where(x => x.SiteNumber == stps[i].SiteNumber).FirstOrDefault();
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

                    Datum2 datum = datums.Where(x => x.DatumID == stps[i].DatumID && x.ArchSiteGuid == stps[i].ArchSiteGuid).FirstOrDefault();
                    if (datum == null)
                    {
                        stps[i].DatumGuid = Guid.Empty;
                        MessageBox.Show(string.Format("Datum {0} not found.", stps[i].DatumID),
                            "Datum not found", MessageBoxButton.OK, MessageBoxImage.Warning);
                        continue;
                    }
                    stps[i].DatumGuid = datum.DatumGuid;

                    if (!stps[i].DatumBearing1.HasValue || !stps[i].DatumDistance1.HasValue || !datum.Easting.HasValue || !datum.Northing.HasValue)
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



                foreach (ShovelTestPit2 stp in stps.Where(x => x.ArchSiteGuid != Guid.Empty && x.DatumGuid != Guid.Empty))
                {
                    SaveToDatabase(stp);
                }
            }
        }


        private Datum2[] QueryDatums(IEnumerable<string> datumids)
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

            List<Datum2> datumlist = new List<Datum2>();
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

            CloseConnection();

            return datumlist.ToArray();
        }
        private ArchSite2[] QueryArchSites(IEnumerable<string> sitenumbers)
        {
            StringBuilder commandSB = new StringBuilder();
            commandSB.AppendLine("SELECT ArchSiteGuid, ProjectID, PermitID, SiteNumber")
                    .AppendLine("FROM [Archy2014].[dbo].[ArchSite]")
                    .AppendLine(string.Format("WHERE SiteNumber IN ('{0}')", string.Join("','", sitenumbers.Distinct())));

            OpenConnection();
            dbcommand.CommandType = CommandType.Text;
            dbcommand.CommandText = commandSB.ToString();
            SqlDataReader rdr = dbcommand.ExecuteReader();

            List<ArchSite2> ialist = new List<ArchSite2>();
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

            CloseConnection();

            return ialist.ToArray();
        }

        public void SaveToDatabase(ShovelTestPit2 stp)
        {
            SqlUtils.AddParametersFrom(stp, ref dbcommand);

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




    }
    public class ShovelTestPit2
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
