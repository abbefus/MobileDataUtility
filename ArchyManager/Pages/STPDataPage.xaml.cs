using ArchyManager.Classes;
using ArchyManager.Classes.Archy2014;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
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

namespace ArchyManager.Pages
{
    public partial class STPDataPage : Page
    {
        public ObservableCollection<ShovelTestPitExtended> STPCollection { get; set; }
        public Dictionary<string,string> ColumnMap { get; set; }
        public DataRow[] DataRows { get; set; }

        public STPDataPage()
        {
            InitializeComponent();
            STPCollection = new ObservableCollection<ShovelTestPitExtended>();
            ColumnMap = new Dictionary<string, string>();
            stp_dg.ItemsSource = STPCollection;
        }

        // converts spreadsheet values into ShovelTextPitExtended
        public void UpdateDataGridFromMapping(string[] values = null)
        {
            STPCollection.Clear();

            values = values ?? ColumnMap.Values.Where(x => x != "Unmapped").ToArray();
            for (int i = 1; i < DataRows.Length; i++)
            {
                ShovelTestPitExtended stp = new ShovelTestPitExtended();
                foreach (KeyValuePair<string,string> kvp in ColumnMap.Where(x => values.Contains(x.Value)))
                {
                    PropertyInfo pi = stp.GetType().GetProperty(kvp.Value);

                    object value;
                    bool success = SqlUtils.TryChangeType(DataRows[i][kvp.Key], pi.PropertyType, out value);
                    if (success)
                    {
                        pi.SetValue(stp, value);
                    }
                    else
                    {
                        pi.SetValue(stp,SqlUtils.ChangeType(AttemptLookups(pi, DataRows[i][kvp.Key]), pi.PropertyType));
                    }
                    
                }
                STPCollection.Add(stp);
            }
            RefreshDataGrid();
        }
        private object AttemptLookups(PropertyInfo pi, object value)
        {
            switch(pi.Name)
            {
                case "PermitNumber":
                    return value;
                case "PitToolID":
                    return value;
                case "DatumGUID":
                    return value;
                case "ProjectID":
                    return value;
                case "ArchSiteGUID":
                    return value;
                default:
                    return value;
            }
        }
        

        public void RefreshDataGrid()
        {
            stp_dg.ItemsSource = null;
            stp_dg.ItemsSource = STPCollection;
        }
    }
}
