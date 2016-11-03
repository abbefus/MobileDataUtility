using ABSSTools;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace AddSites2Vegsys
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //NOTES:
        //  To add a property:
        //      1. Add to sitemap dictionary with accepted headings
        //      2. Add to type switch in order to have it typed properly
        //      3. Add to map dictionary in GetMapping() so it is mapped to correct column in DataSet


        private Dictionary<string, string[]> sitemap;
        Site[] sites;

        public MainWindow()
        {
            InitializeComponent();
            BuildSiteMap();
        }

        private void BuildSiteMap()
        {
            sitemap = new Dictionary<string, string[]>
            {
                { "SiteNumber", new string[] { "SiteNumber", "Site", "Ident", "ID" } },
                { "ProjectNumber", new string[] { "ProjectNumber" } },
                { "ProjectName" , new string[] { "ProjectName" } },
                { "Latitude", new string[] { "Latitude", "Lat" } },
                { "Longitude", new string[] { "Longitude", "Lon", "Long" } },
                { "Easting", new string[] { "Easting", "UTMX", "X", "East","XProj" } },
                { "Northing", new string[] { "Northing", "UTMY", "Y", "YProj" } },
                { "Comments", new string[] { "Comments", "Comment", "Notes", "Note", "Comm" } },
                { "Elevation", new string[] { "Elevation", "Elev", "UTMZ", "Altitude", "Z" } },
                { "CreateDate", new string[] { "CreateDate", "Date", "DateTime", "LTime", "Time" } },
                { "DatumCode", new string[] { "DatumCode", "Datum", "Projection" } },
                { "Zone", new string[] { "Zone", "UTMZone" } },
                { "ProvinceName", new string[] { "Province", "ProvinceName" } }
            };
        }

        public string SiteID { get; set; }
        public int FieldSourceID { get; set; }

        string vegConnStr = "Data Source=sqlprod3\\sql2008;Initial Catalog=vegfield_2014;User Id=developer;Password=sp1d3r5!;";

        //private bool SaveSite(ref string ErrorMessage)
        //{
        //    if (!IsSiteNumberUnique())
        //    {
        //        ErrorMessage = "<B><BR>There is already a site named " + this.txtSiteNumber.Text + " in this project.</B>";
        //        return false;
        //    }

        //    if (!ValidateLatLong(ref ErrorMessage))
        //    {
        //    }

        //    if (!ValidateEcoRegion(ref ErrorMessage))
        //    {
        //    }

        //    if (!ValidateEasting_and_Northing(ref ErrorMessage))
        //    {
        //    }

        //    if (!CreateNewSite)
        //    {
        //        mobjSite.ID = Convert.ToInt32(SiteID);
        //    }

        //    int mEcoID = SelectedEcoId();
        //    if (mEcoID == 0)
        //    {
        //        mobjSite.EcoID = null;
        //    }
        //    else
        //    {
        //        mobjSite.EcoID = mEcoID;
        //    }


        //    string siteID = "";
        //    VegNET.Classes.User currentUser = (VegNET.Classes.User)Session["CurrentUser"];

        //    if (CreateNewSite)
        //    {
        //        siteID = "";
        //        this.lblCreatedby.Text = currentUser.Username;
        //        this.lblCreatedOn.Text = DateTime.Now.ToString("MMMM dd, yyyy hh:mm");
        //    }
        //    else
        //    {
        //        siteID = SiteID;
        //        this.lblUpdatedBy.Text = currentUser.Username;
        //        this.lblUpdateOn.Text = DateTime.Now.ToString("MMMM dd, yyyy hh:mm");
        //    }

        //    mobjSite.SaveToDatabase(CreateNewSite, currentUser, ref siteID);
        //    SiteID = siteID;
        //    return true;
        //}

        private bool IsSiteNumberUnique(Site site)
        {
            DataAccess da = new DataAccess(DataAccess.GetConnectionString());

            if (da.RecordExists("V_Site S", "S.SiteNumber", "LEFT JOIN V_ProjectSite P ON S.SiteID = P.SiteID WHERE P.FieldSourceID = " + this.FieldSourceID + " and S.SiteNumber like '" + site.SiteNumber + "'"))
            {
                //see if this is a resave -- true if the site id is different
                int checkSiteId = DataAccess.GetIntField("SiteID", "V_Site", "WHERE SiteNumber = '" + site.SiteNumber + "'");
                if (checkSiteId.ToString() == this.SiteID)  //resaving the site so Site Number is unique
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else { return true; }
        }

        private void browsefile_btn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Files|*.xls;*.xlsx";
            if (openFileDialog.ShowDialog() == true)
            {
                DataTableCollection data = ExcelReader.Read(openFileDialog.FileName);
                if (data != null)
                {
                    sites = ReadSites(data[0].DataSet);
                }
            }

            if (sites == null)
            {
                Console.WriteLine("Invalid table format. Import failed. See documentation.");
            }
            else
            {
                InsertSitesInDatabase();
            }
        }
        private Site[] ReadSites(DataSet dataset)
        {
            char[] invalidchars = new char[] { ' ', '_', '-' };

            List<Site> sitelist = new List<Site>();
            string[] headers = dataset.Tables[0].Rows[0].ItemArray.Cast<string>().ToArray();
            for (int i = 0; i < headers.Length; i++)
            {
                headers[i] = headers[i].ReplaceAll(invalidchars).ToLower().Trim();
            }

            Dictionary<string, int> map = GetMapping(headers);

            if (!HasRequiredValues(map.Keys.ToArray())) return null;

            for (int i = 1; i < dataset.Tables[0].Rows.Count; i++)
            {
                Site site = new Site();

                foreach (string property in map.Keys)
                {
                    string strvalue = dataset.Tables[0].Rows[i].ItemArray[map[property]].ToString();
                    switch (property)
                    {
                        case "Latitude":
                        case "Longitude":
                        case "Easting":
                        case "Northing":
                        case "Elevation":
                            double dubvalue;
                            if (double.TryParse(strvalue,out dubvalue))
                            {
                                typeof(Site).GetProperty(property).
                                    SetValue(site, dubvalue);
                            }
                            else
                            {
                                Console.WriteLine("Cannot convert {0} to {1} (Decimal)", strvalue, property);
                            }
                            break;
                        case "Zone":
                            int intvalue;
                            if (int.TryParse(strvalue, out intvalue))
                            {
                                typeof(Site).GetProperty(property).
                                    SetValue(site, intvalue);
                            }
                            else
                            {
                                Console.WriteLine("Cannot convert {0} to {1} (Integer)", strvalue, property);
                            }
                            break;
                        case "CreateDate":
                            DateTime datevalue;
                            if (DateTime.TryParse(strvalue, out datevalue))
                            {
                                typeof(Site).GetProperty(property).
                                    SetValue(site, datevalue);
                            }
                            else
                            {
                                Console.WriteLine("Cannot convert {0} to {1} (Date)", strvalue, property);
                            }
                            break;
                        case "ProvinceName":
                            if (!IsValidProvinceName(strvalue))
                            {
                                Console.WriteLine("{0} is not a valid province name", strvalue);
                                break;
                            }
                            site.ProvinceName = strvalue;
                            break;
                        case "DatumCode":
                            if (!IsValidDatumCode(strvalue))
                            {
                                Console.WriteLine("{0} is not a valid datum code", strvalue);
                                break;
                            }
                            site.DatumCode = strvalue;
                            break;
                        case "SiteNumber":
                        case "ProjectNumber":
                        case "ProjectName":
                        case "Comments":
                        default:
                            try
                            {
                                typeof(Site).GetProperty(property).
                                    SetValue(site, strvalue);
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Cannot convert {0} to {1} (Text)", strvalue, property);
                            }

                            break;
                    }                    
                }

                sitelist.Add(site);
            }
            return sitelist.ToArray();
        }
        
        private void InsertSitesInDatabase()
        {
            foreach (Site site in sites)
            {
                if (!string.IsNullOrEmpty(site.SiteNumber))
                {
                    Site.PopulateSite(site, site.SiteNumber);
                }
            }
        }




        private bool HasRequiredValues(string[] testarray)
        {
            string[] required = new string[] { "SiteNumber", "ProvinceName", "CreateDate" };
            string[] latlong = new string[] { "Latitude", "Longitude" };
            string[] utms = new string[] { "Northing", "Easting", "DatumCode", "Zone" };
            bool valid = !(required.Except(testarray).Any());
            bool latlongvalid = !(latlong.Except(testarray).Any());
            bool utmsvalid = !(utms.Except(testarray).Any());

            return valid && (latlongvalid || utmsvalid);
        }
        private static bool IsValidProvinceName(string province)
        {
            string[] provinces = new string[]
            {
                "Alberta",
                "British Columbia",
                "Manitoba",
                "New Brunswick",
                "Newfoundland",
                "Northwest Territories",
                "Nova Scotia",
                "Ontario",
                "Prince Edward Island",
                "Quebec",
                "Saskatchewan",
                "Yukon",
                "Nunavut"
            };
            return provinces.Contains(province);
        }
        private static bool IsValidDatumCode(string datumcode)
        {
            string[] datumcodes = new string[]
            {
                "NAD27",
                "NAD83",
                "WGS84"
            };
            return datumcodes.Contains(datumcode);
        }

        private Dictionary<string, int> GetMapping(string[] headers)
        {
            Dictionary<string, int> map = new Dictionary<string, int>
            {
                { "SiteNumber", -1 },
                { "ProjectNumber", -1 },
                { "ProjectName" , -1 },
                { "Latitude", -1 },
                { "Longitude", -1 },
                { "Easting", -1 },
                { "Northing", -1 },
                { "Comments", -1 },
                { "Elevation", -1 },
                { "CreateDate", -1 },
                { "DatumCode", -1 },
                { "Zone", -1},
                { "ProvinceName", -1 }
            };

            foreach (string key in map.Keys.ToArray())
            {
                for (int index = 0; index < headers.Length; index++)
                {
                    if (sitemap[key].Select(x => x.ToLower()).Contains(headers[index]))
                    {
                        map[key] = index;
                    }
                }
                if (map[key] == -1)
                {
                    Console.WriteLine("{0} not found in table.", key);
                }
            }
            return map.Where(x => x.Value != -1).ToDictionary(k => k.Key, v => v.Value);
        }
    }
    public static class ExtensionMethods
    {
        public static string ReplaceAll(this string seed, char[] chars, string replacement="")
        {
            return chars.Aggregate(seed, (x, c) => x.Replace(c.ToString(), replacement));
        }
    }
}
