using System;
using System.Collections.Generic;
using System.Linq;
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
using System.IO;

namespace SignedInUsers
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel _viewmodel = null;
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = _viewmodel = new MainWindowViewModel();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            _viewmodel.Go();
        }

        private void MachineListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewmodel != null && sender is ListBox listBox)
            {
                var selectedMachines = listBox.SelectedItems.Cast<string>().ToList();
                _viewmodel.UpdateSelectedMachines(selectedMachines);
            }
        }

        private void StatusBar_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string fileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Log");
            if (Directory.Exists(fileName) == false)
                Directory.CreateDirectory(fileName);

            fileName = System.IO.Path.Combine(fileName, $"Log_{DateTime.Now.ToString("yyyyMMddHHmmss")}.log");

            File.WriteAllText(fileName, _viewmodel.Statuses.ToString());

            System.Diagnostics.Process.Start(fileName);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewmodel != null)
                _viewmodel.Load();
        }

        private void ownerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewmodel != null)
                _viewmodel.SelectAllOwners();
        }

        private void OwnerCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (_viewmodel != null && sender is CheckBox checkBox && checkBox.Content is string ownerName)
            {
                _viewmodel.HandleOwnerSelectionChanged(ownerName, checkBox.IsChecked == true);
            }
            else if (_viewmodel != null)
            {
                _viewmodel.SelectAllOwners();
            }

            if (ownerComboBox != null)
                ownerComboBox.IsDropDownOpen = true;
        }

        private void OwnerSelectAllMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_viewmodel != null)
                _viewmodel.HandleOwnerSelectionChanged("ALL", true);
        }

        private void OwnerDeselectAllMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_viewmodel != null)
                _viewmodel.HandleOwnerSelectionChanged("ALL", false);
        }

        private void ownerComboBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            // Check if the cast was successful and if the ComboBox is not null
            if (comboBox != null)
            {
                // Set IsDropDownOpen to true to open the dropdown
                comboBox.IsDropDownOpen = true;
            }
        }

        private void ComboBox_GotFocus(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            // Check if the cast was successful and if the ComboBox is not null
            if (comboBox != null)
            {
                // Set IsDropDownOpen to true to open the dropdown
                comboBox.IsDropDownOpen = true;
            }
        }
    }
}
