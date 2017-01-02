using ABUtils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ArchyManager.Dialogs
{
    #region MapColumnsDialog
    // Important Note: requires BrowsableAttribute on all properties of dataType
    // Notes:
    //  -uses "Unmapped" as a placeholder for unmapped columns 
    //      -perhaps would be better that these have no value at all
    class MapColumnsDialog : OKCancelDialog
    {       
        public MapColumnsDialog(Dictionary<string,string> currentMapping, Type dataType, string[] exceptions=null)
            : base (string.Format("Map Columns to {0}",dataType.Name))
        {
            ViewModel vm = DictionaryToView(currentMapping, dataType, exceptions);
            DataContext = vm;
            Initialize();
        }

        // creates a viewmodel from an existing mapping dictionary and a dataType
        private ViewModel DictionaryToView(Dictionary<string,string> mapping, Type dataType, string[] exceptions)
        {
            // filter by BrowsableAttribute
            PropertyInfo[] properties = dataType.GetProperties()
                .Where(x => ((BrowsableAttribute)x.GetCustomAttribute(typeof(BrowsableAttribute))).Browsable)
                .ToArray();

            ViewModel vm = new ViewModel { Options = new ObservableCollection<string>(properties.Select(x => x.Name).Where(x => !exceptions.Contains(x))) };
            vm.Options.Insert(0, "Unmapped");

            foreach (KeyValuePair<string,string> kvp in mapping)
            {
                Type datatype = properties.Where(x => x.Name == kvp.Value).Select(x => x.PropertyType).FirstOrDefault();
                vm.Map.Add(new Mapping { Heading = kvp.Key, Selection = kvp.Value, DataType = dataType.Name });
            }
            return vm;
        }

        // returns a <string,string> dictionary of the mapping result
        public Dictionary<string,string> OMToDictionary()
        {
            Dictionary<string, string> output = new Dictionary<string, string>();
            ViewModel vm = DataContext as ViewModel;
            foreach (Mapping mapping in vm.Map)
            {
                output.Add(mapping.Heading, mapping.Selection);
            }
            return output;
        }

        private void Row_Click(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = sender as DataGridRow;
            Console.WriteLine(((Mapping)row.Item).Heading.ToUpperInvariant());
        }

        // sets up datagrid and columns
        private void Initialize()
        {
            // capture mouse clicks on row
            Style style = new Style(typeof(DataGridRow));
            style.Setters.Add(new EventSetter
            {
                Event = PreviewMouseLeftButtonDownEvent,
                Handler = new MouseButtonEventHandler(Row_Click)
            });

            DataGrid dg = new DataGrid
            {
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                AutoGenerateColumns = false,
                CanUserAddRows = false,
                ItemsSource = ((ViewModel)DataContext).Map,
                SelectionMode = DataGridSelectionMode.Single,
                MaxHeight = 500,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                ItemContainerStyle = style
            };

            DataGridTextColumn txtcol = new DataGridTextColumn
            {
                Header = "Heading",
                Binding = new Binding("Heading"),
                IsReadOnly = true,
                MinWidth = 200
            };

            FrameworkElementFactory combfact = new FrameworkElementFactory(typeof(ComboBox));
            combfact.SetBinding(ComboBox.ItemsSourceProperty, new Binding { Source = DataContext, Path = new PropertyPath("Options") });
            combfact.SetBinding(ComboBox.SelectedValueProperty, new Binding
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor,typeof(DataGridRow), 1),
                Path = new PropertyPath("Item.Selection"),
                //Converter = new DataConverter(),
                Mode = BindingMode.TwoWay
            });

            DataTemplate textTemplate = new DataTemplate();
            textTemplate.VisualTree = combfact;

            DataGridTemplateColumn temcol = new DataGridTemplateColumn
            {
                Header = "Options",
                IsReadOnly = true,
                CellTemplate = textTemplate,
                MinWidth = 200,
            };
            dg.Columns.Add(txtcol);
            dg.Columns.Add(temcol);

            stack_pnl.Children.Add(dg);
        }

        // checks for duplicates before closing dialog
        protected override void Okbtn_Click(object sender, RoutedEventArgs e)
        {
            string[] selections = ((ViewModel)DataContext).Map.Select(x => x.Selection).Where(x => x != "Unmapped").ToArray();
            if (selections.Distinct().Count() == selections.Length)
            {
                Console.WriteLine("Mapping Complete.");
                base.Okbtn_Click(sender, e);
            }
            else
            {
                string query = string.Join(" and ", selections.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key));
                MessageBox.Show(string.Format("You cannot map to a property more than once.\r\n{0} was selected more than once.", query),
                    "Duplicate properties selected", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                e.Handled = true;
            }
        }

    }

    public class Mapping : ViewModelBase
    {
        private string _heading;
        public string Heading
        {
            get { return _heading; }
            set { _heading = value; NotifyPropertyChanged(); }
        }

        private string _selection;
        public string Selection
        {
            get { return _selection; }
            set { _selection = value;  NotifyPropertyChanged(); }
        }
        private string _dataType;
        public string DataType
        {
            get { return _dataType; }
            set { _dataType = value; NotifyPropertyChanged(); }
        }
    }
    public class ViewModel : ViewModelBase
    {
        private ObservableCollection<string> _options = new ObservableCollection<string>();
        public ObservableCollection<string> Options
        {
            get { return _options; }
            set { _options = value; NotifyPropertyChanged(); }
        }

        private ObservableCollection<Mapping> _map = new ObservableCollection<Mapping>();
        public ObservableCollection<Mapping> Map
        {
            get { return _map; }
            set { _map = value; NotifyPropertyChanged(); }
        }
    }
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    #endregion

    public class GenericConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    } // for testing bindings
}
