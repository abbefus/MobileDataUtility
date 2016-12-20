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
        public void UpdateDataGridFromMapping()
        {
            for (int i = 1; i < DataRows.Length; i++)
            {
                ShovelTestPitExtended stp = new ShovelTestPitExtended();
                foreach (string p in ColumnMap.Keys)
                {
                    if (ColumnMap[p] == "Unmapped") continue;
                    PropertyInfo pi = stp.GetType().GetProperty(ColumnMap[p]);
                    pi.SetValue(stp, SqlUtils.ChangeType(DataRows[i][p], pi.PropertyType));
                }
                STPCollection.Add(stp);
            }
            RefreshDataGrid();
        }
        

        public void RefreshDataGrid()
        {
            stp_dg.ItemsSource = null;
            stp_dg.ItemsSource = STPCollection;
        }
    }
}
