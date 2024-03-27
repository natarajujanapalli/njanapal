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
    }
}
