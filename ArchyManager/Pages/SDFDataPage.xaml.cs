using ArchyManager.Classes.Archy2014;
using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using WpfControlLibrary.Extension;

namespace ArchyManager.Pages
{

    public partial class SDFDataPage : Page
    {
        public SDFDataPage()
        {
            InitializeComponent();
            DataContextChanged += SDFDataPage_DataContextChanged;
        }

        private void SDFDataPage_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            data_tac.Items.Clear();
            CreateTabs();
        }

        private void CreateTabs()
        {
            Type t = typeof(Archy2014DB);
            PropertyInfo[] pis = t.GetProperties().Where(x => t.GetProperty(x.Name).GetValue(DataContext) != null).ToArray();

            foreach (PropertyInfo pi in pis)
            {
                if (((ICollection)pi.GetValue(DataContext)).Count < 1) continue;

                Type p = pi.GetType();

                // trying to bind the foreground color to the Uploaded property of an IUploadable
                // Style dgstyle = new Style(typeof(DataGridRow));
                // dgstyle.

                DataGrid dg = new DataGrid
                {
                    Margin = new Thickness(-6),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    AutoGenerateColumns = true,
                    IsReadOnly = true,
                    CanUserAddRows = false,
                    ItemsSource = (ICollection)pi.GetValue(DataContext),
                    EnableColumnVirtualization = true,
                    EnableRowVirtualization = true,
                    MaxWidth = 2560,
                    MaxHeight = 1600
                };
                ScrollViewer tmp = ABUtils.Utils.GetVisualChild<ScrollViewer>(dg);
                if (tmp != null) tmp.CanContentScroll = false;
                DataGridBehavior.SetUseBrowsableAttributeOnColumn(dg, true);

                TabItem tab = new TabItem
                {
                    Header = pi.Name,
                    Content = dg,
                };
                data_tac.Items.Add(tab);
            }
            Console.WriteLine("Found data in {0} tables", data_tac.Items.Count);
        }

    }
    public interface IUploadable
    {
        string DefaultGuid { get; }
        bool IsUploaded { get; set; }
    }
}
