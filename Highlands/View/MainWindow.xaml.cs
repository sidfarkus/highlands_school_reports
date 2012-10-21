//using Highlands.Model;
using Highlands.StaticModel;
using Highlands.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Highlands
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            GradebookViewModel.Test();
       
            // clear gradebook
            Title = "Welcome, " + UserViewModel.CurrentUser;
            loginWindow.Reset();
            loginOverlay.Visibility = System.Windows.Visibility.Visible;
        }
        GradebookViewModel _gradebook;
        private void OnLogin(object sender, System.EventArgs e)
        {
            Title = "Highlands School";
            loginOverlay.Visibility = System.Windows.Visibility.Hidden;
            _gradebook = studentsControl.LoadGradebook();
            ctrlClasses.Refresh(_gradebook);
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            _gradebook.ImportStudents("importStudents.csv");
            
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            _gradebook.ExportStudents("exportStudents.csv");
        }
    }
}
