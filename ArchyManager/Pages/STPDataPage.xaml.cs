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
using System.Windows;
using System.Windows.Controls;

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

            for (int i = 0; i < DataRows.Length; i++)
            {
                ShovelTestPitExtended stp = new ShovelTestPitExtended() { IsUploadable = true, IsUploaded = false };
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
                        Type targetType = SqlUtils.IsNullableType(pi.PropertyType) ? 
                            Nullable.GetUnderlyingType(pi.PropertyType) : 
                            pi.PropertyType;
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("Unable to convert column [ {0} ] to {1}.")
                            .AppendLine("You will either need to:")
                            .AppendLine("\ta) map this column or")
                            .AppendLine("\tb) reformat it in Excel");

                        MessageBox.Show(string.Format(sb.ToString(),kvp.Key,targetType.Name),
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
            [DefaultValue("Shovel")]
            public string PitTool { get; set; }
            [Browsable(true)]
            public Guid? PitToolGuid { get; set; }
        }
    }
    
    
}
