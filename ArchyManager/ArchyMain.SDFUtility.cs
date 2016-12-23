using ABUtils;
using ArchyManager.Classes;
using ArchyManager.Classes.Archy2014;
using ArchyManager.Pages;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ArchyManager
{
    public partial class ArchyMain
    {
        private ArchyRegion region = ArchyRegion.BC; // maybe store on datapage for gc

        private async void openSDF_btn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "SDF Files|*.sdf";
            if (openFileDialog.ShowDialog() == true)
            {
                field_tab.IsEnabled = false;
                Archy2014DB db = ReadSDF(openFileDialog.FileName);
                if (db == null) return;

                activepages["field_tab"] = null;
                activepages["field_tab"] = new SDFDataPage { DataContext = db };
                frame.Navigate(activepages["field_tab"]);

                double totes = typeof(Archy2014DB).GetProperties()
                    .Select(x => (Array)typeof(Archy2014DB).GetProperty(x.Name).GetValue(db))
                    .Where(x => x != null)
                    .Sum(x => x.Length);
                Progress<double> progress = new Progress<double>(x => progressbar.Value = x);
                Action<IProgress<double>> sqlwork = (iprogress) =>
                {
                    using (SqlConnection conn = SQL2008.ConnectUsing(SQL2008.ARCHY2014_DB))
                    {
                        PropertyInfo[] pis = typeof(Archy2014DB).GetProperties();
                        double count = 0;
                        foreach (PropertyInfo p in pis)
                        {
                            UpdateStatus(StatusType.Success, string.Format("Polling exisiting records: {0}", p.Name));
                            IUploadable[] cs = p.GetValue(db) as IUploadable[];
                            if (cs != null)
                            {
                                foreach (IUploadable c in cs)
                                {
                                    c.IsUploaded = CheckForExistingGuid(c, conn);
                                    iprogress.Report((++count / totes) * 100);
                                }
                            }
                        }
                    }
                };

                await Task.Run(() =>
                {
                    sqlwork(progress);
                });

                RefreshDataGrids();
                uploadSDF_btn.IsEnabled = CheckForAnyUploads(db);
                field_tab.IsEnabled = true;
            }
            UpdateStatus(StatusType.Success, "SDF read complete.");
            progressbar.Value = 0;
            
        }
        private bool CheckForAnyUploads(Archy2014DB db)
        {
            IEnumerable<IUploadable[]> pros = typeof(Archy2014DB).GetProperties()
                    .Select(x => (IUploadable[])typeof(Archy2014DB).GetProperty(x.Name).GetValue(db))
                    .Where(x => x != null);
            return pros.Any(x => !x.Any(y => y.IsUploaded));
        }
        private bool CheckForExistingGuid(IUploadable obj, SqlConnection conn)
        {
            string table = obj.GetType().Name;
            string guid = obj.GetType().GetProperty(obj.DefaultGuid).GetValue(obj).ToString();
            StringBuilder commandSB = new StringBuilder();
            commandSB.AppendLine(string.Format("SELECT {0}", obj.DefaultGuid))
                    .AppendLine(string.Format("FROM [Archy2014].[dbo].[{0}]", table))
                    .AppendLine(string.Format("WHERE {0} = '{1}'", obj.DefaultGuid, guid));
            object sqlguid = null;
            using (SqlCommand comm = conn.CreateCommand())
            {
                comm.CommandType = CommandType.Text;
                comm.CommandText = commandSB.ToString();
                sqlguid = comm.ExecuteScalar();
            }

            return sqlguid != null;
        }

        //Binding would probably be cleaner and wouldn't reset the scrollbar
        private void RefreshDataGrids()
        {
        TabControl tc = ((SDFDataPage)activepages["field_tab"]).data_tac;
        foreach (TabItem ti in tc.Items)
        {
            DataGrid dg = (DataGrid)ti.Content;
            dg.Items.Refresh();
            dg.ItemContainerGenerator.StatusChanged += (s, x) =>
            {
                for (int i = 0; i < dg.Items.Count; i++)
                {
                    if (((ItemContainerGenerator)s).Status == GeneratorStatus.ContainersGenerated)
                    {
                        IUploadable iup = (IUploadable)dg.ItemContainerGenerator.Items.ElementAt(i);
                        if (iup != null)
                        {
                            DataGridRow row = (DataGridRow)dg.ItemContainerGenerator.ContainerFromItem(iup);
                            if (row == null) continue;
                            if (!iup.IsUploaded)
                            {
                                row.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF7f0000"));
                            }
                            else
                            {
                                row.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFff960d"));
                            }
                        }

                    }
                }
            };
        }
        }

        private async void uploadSDF_btn_Click(object sender, RoutedEventArgs e)
        {
            Archy2014DB db = ((SDFDataPage)activepages["field_tab"]).DataContext as Archy2014DB;
            StringBuilder log = new StringBuilder();

            Progress<double> progress = new Progress<double>(x => progressbar.Value = x);
            double totes = typeof(Archy2014DB).GetProperties()
                    .Select(x => (IUploadable[])typeof(Archy2014DB).GetProperty(x.Name).GetValue(db))
                    .Where(x => x != null)
                    .Sum(x => x.Where(m => !m.IsUploaded).Count());
            Action<IProgress<double>> sqlwork = (iprogress) =>
            {
                using (SqlConnection conn = SQL2008.ConnectUsing(SQL2008.ARCHY2014_DB))
                {
                    PropertyInfo[] pis = typeof(Archy2014DB).GetProperties();
                    double count = 0;
                    foreach (PropertyInfo p in pis)
                    {
                        IUploadable[] table = ((IUploadable[])p.GetValue(db));
                        if (table != null)
                        {
                            IEnumerable<IUploadable> uprows = table.Where(x => !x.IsUploaded);
                            
                            UpdateStatus(StatusType.Success, string.Format("Uploading {0} to {1}", uprows.Count(), p.Name));
                            if (uprows != null)
                            {
                                foreach (IUploadable row in uprows)
                                {
                                    iprogress.Report((++count / totes) * 100);
                                    SqlParameter rv = SqlUtils.ExecuteSP(row, conn, SPString.INSERT, DbType.Boolean);
                                    try
                                    {
                                        log.AppendLine(rv.Value != null && (bool)rv.Value ?
                                            string.Format("Success: {0} => {1}", row.GetType().GetProperty(row.DefaultGuid).GetValue(row), row.GetType().Name) :
                                            string.Format("Failed: {0} => {1}", row.GetType().GetProperty(row.DefaultGuid).GetValue(row), row.GetType().Name));
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                }
            };

            SetNativeEnabled(false);
            await Task.Run(() =>
            {
                sqlwork(progress);
            });
            SetNativeEnabled(true);
            progressbar.Value = 0;

            RefreshDataGrids();
            uploadSDF_btn.IsEnabled = CheckForAnyUploads(db);

            ScrollableTextDialog dialog = new ScrollableTextDialog(log.ToString(), "Details:", "Upload Complete");
            dialog.ShowDialog();
        }

        private Archy2014DB ReadSDF(string file)
        {
            string connection = string.Format("Data Source='{0}';Max Database Size=4091;Max Buffer Size=256;Default Lock Escalation=100;Password='J3ll-0'", file);
            SqlCeEngine engine = new SqlCeEngine(connection);

            Archy2014DB db = new Archy2014DB();

            // upgrade sdf
            try { engine.Upgrade(); }
            catch {} // if sdf already upgraded, do nothing

            using (var conn = new SqlCeConnection(connection))
            {
                conn.Open();
                try
                {
                    db.Projects = SDFToDataModel<Project>(conn);
                }
                catch (Exception e)
                {
                    UpdateStatus(StatusType.Failure, e.Message);
                }
                try
                {
                    region = (ArchyRegion)Enum.Parse(typeof(ArchyRegion), db.Projects.FirstOrDefault().Province);
                }
                catch (Exception e)
                {
                    MessageBox.Show(string.Format("Unable to parse province from the project.\r\n{0}", e.Message));
                    UpdateStatus(StatusType.Failure, "Unable to parse province from the project.");
                    return null;
                }

                // verify this by comparing tables between NB and BC sdf - if they are the same then simplify this
                if (region == ArchyRegion.BC)
                {
                    try
                    {
                        db.ArchSites = SDFToDataModel<ArchSite>(conn);
                        db.ArchSitePhotologs = SDFToDataModel<ArchSitePhotolog>(conn);
                        db.AreaGeometries = SDFToDataModel<AreaGeometry>(conn);
                        db.CMTs = SDFToDataModel<CMT>(conn);
                        db.CMTMarks = SDFToDataModel<CMTMark>(conn);
                        db.CrewDefinitions = SDFToDataModel<CrewDefinition>(conn);
                        db.Datums = SDFToDataModel<Datum>(conn);
                        db.HSFs = SDFToDataModel<HSF>(conn);
                        db.ProfileDescriptors = SDFToDataModel<ProfileDescriptor>(conn);
                        db.Projects = SDFToDataModel<Project>(conn);
                        db.ShovelTestPits = SDFToDataModel<ShovelTestPit>(conn);
                        db.STPProfiles = SDFToDataModel<STPProfile>(conn);
                        db.TestPitCrews = SDFToDataModel<TestPitCrew>(conn);
                        db.Tracklogs = SDFToDataModel<TrackLog>(conn);
                    }
                    catch (Exception e)
                    {
                        UpdateStatus(StatusType.Failure, e.Message);
                    }
                }
                else if (region == ArchyRegion.NB)
                {
                    try
                    {
                        db.ARCHSs = SDFToDataModel<ARCHS>(conn);
                        db.ArchSites = SDFToDataModel<ArchSite>(conn);
                        db.ArchSitePhotologs = SDFToDataModel<ArchSitePhotolog>(conn);
                        db.AreaGeometries = SDFToDataModel<AreaGeometry>(conn);
                        db.CMTs = SDFToDataModel<CMT>(conn);
                        db.CMTMarks = SDFToDataModel<CMTMark>(conn);
                        db.CrewDefinitions = SDFToDataModel<CrewDefinition>(conn);
                        db.Datums = SDFToDataModel<Datum>(conn);
                        db.HSFs = SDFToDataModel<HSF>(conn);
                        db.ProfileDescriptors = SDFToDataModel<ProfileDescriptor>(conn);
                        db.Projects = SDFToDataModel<Project>(conn);
                        db.ShovelTestPits = SDFToDataModel<ShovelTestPit>(conn);
                        db.STPProfiles = SDFToDataModel<STPProfile>(conn);
                        db.TestPitCrews = SDFToDataModel<TestPitCrew>(conn);
                        db.Tracklogs = SDFToDataModel<TrackLog>(conn);
                        db.WCAs = SDFToDataModel<WCA>(conn);
                    }
                    catch (Exception e)
                    {
                        UpdateStatus(StatusType.Failure, e.Message);
                    }

                }
                
                conn.Close();
            }
            return db;
        }

        // creates a datamodel of type T from an SqlCeConnection
        private static T[] SDFToDataModel<T>(SqlCeConnection conn)
        {
            Type dffType = typeof(T);

            string sql = string.Format("SELECT * FROM {0}", dffType.Name);
            List<T> datamodellist = new List<T>();

            try
            {
                DataTable datatable = new DataTable();
                using (SqlCeDataAdapter adapter = new SqlCeDataAdapter(sql, conn))
                {
                    adapter.Fill(datatable);
                }

                foreach (DataRow row in datatable.Rows)
                {
                    string[] columns = dffType.GetProperties().Select(x => x.Name).ToArray();

                    T obj = (T)Activator.CreateInstance(dffType);
                    foreach (string column in columns)
                    {
                        PropertyInfo p = dffType.GetProperty(column);

                        // properties unbrowsable properties
                        BrowsableAttribute ba = p.GetCustomAttribute(typeof(BrowsableAttribute), false) as BrowsableAttribute;
                        if (ba != null)
                        {
                            if (ba.Browsable == false) continue;
                        }

                        p.SetValue
                        (
                            obj,
                            SqlCeConversion.CheckDBNull(row[column.TrimEnd('_')])
                            // the trim is required for tables in which Matt gave the column the same name as a table >=/
                        );
                    }
                    datamodellist.Add(obj);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{0}", e.Message));
                throw new Exception(string.Format("There was an error reading {0} from the compact database.", dffType.Name));
            }

            return datamodellist.ToArray();
        }
        
    }
}
