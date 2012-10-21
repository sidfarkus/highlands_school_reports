//using Highlands.Model;
using Highlands.StaticModel;
using Highlands.ViewModel;
using System;
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
            try
            {
                var results = _gradebook.ImportStudents("importStudents.csv");
                if (results.Count() == 0)
                {
                    MessageBox.Show("No changes");
                    return;
                }

                string message;
                if (results.Count() > 30)
                    message = "More than " + results.Count() + " rows changed";
                else
                    message = string.Join(Environment.NewLine, results.ToArray());
                
                var result = MessageBox.Show(message + Environment.NewLine + "Save results and close?", "Import Complete", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    _gradebook.Save();
                    Close();
                }
                else
                    Close();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _gradebook.ExportStudents("exportStudents.csv");
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }
    }
}
