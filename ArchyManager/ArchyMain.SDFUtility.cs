using ArchyManager.Classes;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.Data;
using ArchyManager.Classes.Archy2014;

namespace ArchyManager
{
    public partial class ArchyMain
    {
        private ArchyConnection ac = new ArchyConnection();
        private void openSDF_btn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "SDF Files|*.sdf";
            if (openFileDialog.ShowDialog() == true)
            {
                Archy2014DB db = ReadSDF(openFileDialog.FileName);
            }
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
                db.ArchSites = SDFToDataModel<ArchSite>(conn);
                db.ArchSitePhotologs = SDFToDataModel<ArchSitePhotolog>(conn);
                db.AreaGeometries = SDFToDataModel<AreaGeometry>(conn);
                db.ShovelTestPits = SDFToDataModel<ShovelTestPit>(conn);
                conn.Close();
            }
            return db;
        }

        // creates a datamodel of type T from an SqlCeConnection
        private T[] SDFToDataModel<T>(SqlCeConnection conn)
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
                        Type t = dffType.GetProperty(column).PropertyType;
                        dffType.GetProperty(column).SetValue
                        (
                            obj,
                            SqlCeConversion.CheckDBNull(row[column], t)
                        );
                    }
                    datamodellist.Add(obj);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(string.Format("{0}", e.Message));
                UpdateStatus(StatusType.Failure, "There was an error reading DataFormF\r\n from the compact database.");
            }

            return datamodellist.ToArray();
        }
        
    }
}
