using ArchyManager.Classes;
using ArchyManager.Classes.Archy2014;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
        public STPLookups STPLookups { get; set; }

        public STPDataPage()
        {
            InitializeComponent();
            STPCollection = new ObservableCollection<ShovelTestPitExtended>();
            ColumnMap = new Dictionary<string, string>();
            stp_dg.ItemsSource = STPCollection;
            STPLookups = new STPLookups();
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
                    object input = DataRows[i][kvp.Key];
                    
                    object value;
                    if (SqlUtils.TryChangeType(input, pi.PropertyType, out value))
                    {
                        pi.SetValue(stp, value);
                    }
                    else
                    {
                        Type targetType = SqlUtils.IsNullableType(pi.PropertyType) ? Nullable.GetUnderlyingType(pi.PropertyType) : pi.PropertyType;
                        MessageBox.Show(string.Format("Unable to convert column '{0}' to {1}.\r\nYou will either need to:\r\n\r\n\ta) map this column or\r\n\tb) reformat it in Excel  \r\n\r\nin order to include it in the export.", 
                            kvp.Key,targetType.Name),
                            "Unable to Convert Table Values", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        values = values.Except(Enumerable.Repeat(kvp.Value,1)).ToArray();
                        SqlUtils.SetPropertyBrowsable(typeof(ShovelTestPitExtended), pi.Name, false);
                    }
                }
                STPCollection.Add(stp);
            }
            RefreshDataGrid();
        }
        
        public IEnumerable<T> GetColumnByProperty<T>(string property)
        {
            if (!ColumnMap.Values.Any(x => x == property)) return null;
            KeyValuePair<string,string> kvp = ColumnMap.Where(v => v.Value == property).FirstOrDefault();
            return DataRows.Select(row => (T)SqlUtils.ChangeType(row[kvp.Key],typeof(T)));
        }

        public void RefreshDataGrid()
        {
            stp_dg.ItemsSource = null;
            stp_dg.ItemsSource = STPCollection;
        }
    }
    public class STPLookups
    {
        public ArchSite[] ArchSiteLU { get; set; }
        public Datum[] DatumLU { get; set; }
        public Permit[] PermitLU { get; set; }
        public Project[] ProjectLU { get; set; }
        public LUPitTool[] PitToolLU { get; set; }
        









        public class Permit
        {
            [Browsable(true)]
            public Int16 ProjectID { get; set; }
            [Browsable(true)]
            public Int32 PermitID { get; set; }
            [Browsable(true)]
            public string PermitNumber { get; set; }
            [Browsable(true)]
            public string PermitHolder { get; set; }
            [Browsable(true)]
            public Guid PermitGuid { get; set; }
        }
        public class LUPitTool
        {
            [Browsable(true)]
            public byte PitToolID { get; set; }
            [Browsable(true)]
            public string PitTool { get; set; }
            [Browsable(true)]
            public Guid? PitToolGuid { get; set; }
        }
    }
    
    
}
