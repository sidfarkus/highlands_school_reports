//using Highlands.Model;
using Highlands.StaticModel;
using Highlands.ViewModel;
using Microsoft.Win32;
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
        public GradebookViewModel Gradebook { get; set; }
        GradebookViewModel _gradebook;
        private void OnLogin(object sender, System.EventArgs e)
        {
            Title = "Highlands School";
            loginOverlay.Visibility = System.Windows.Visibility.Hidden;
            _gradebook = studentsControl.LoadGradebook();
            ctrlClasses.Refresh(_gradebook);
            ctrlAttendees.Refresh();
            //ctrlClasses.Visibility = ViewUtils.IsVisible(UserViewModel.CurrentUser.CanEditAttendance);
            //tabAttendance.Visibility = ViewUtils.IsVisible(UserViewModel.CurrentUser.CanEditAttendance);
            tabClasses.Visibility = ViewUtils.IsVisible(UserViewModel.CurrentUser.CanViewGrades);
            tabStudents.Visibility = ViewUtils.IsVisible(UserViewModel.CurrentUser.CanViewGrades);
            btnExport.Visibility = ViewUtils.IsVisible(UserViewModel.CurrentUser.CanImportExport);
            btnImport.Visibility = ViewUtils.IsVisible(UserViewModel.CurrentUser.CanImportExport);
            btnHonorRoll.Visibility = ViewUtils.IsVisible(UserViewModel.CurrentUser.CanViewHonorRoll);
        }

        private void btnImport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var ofd = new OpenFileDialog();
                ofd.DefaultExt = ".csv";
                ofd.Multiselect = false;
                ofd.Filter = "CSV files|*.csv|All Files|*.*";
                if (ofd.ShowDialog() != true)
                    return;
                var results = _gradebook.ImportStudents(ofd.FileName);
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
                var sfd = new SaveFileDialog();
                sfd.DefaultExt = ".csv";
                sfd.Filter = "CSV files|*.csv|All Files|*.*";
                if (sfd.ShowDialog() != true)
                    return;
                _gradebook.ExportStudents(sfd.FileName);
            
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void btnHonorRoll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var mp in MarkingPeriods.Singleton.OrderByDescending(m => m.StartDate))
            {
                //var mp = MarkingPeriod.Current;
                var students = _gradebook.Students.Where(s => s.HonorRoll(mp)).OrderByDescending(s => s.Gpa(mp)).GroupBy(s => s.GradeLevel);
                var outs = new List<string>();
                foreach (var kvp in students.OrderBy(s => Maintenance.GradeLevelNumber(s.Key)))
                {
                    outs.Add("-" + StudentViewModel.FormatGradeLevel(kvp.Key) + "-");
                    foreach(var student in kvp)
                        outs.Add(student.Name + " " + student.Gpa(mp).ToString("0.00"));
                    outs.Add("");
                }
                if (outs.Count() == 0)
                    continue;

                ViewUtils.Mail("", "Honor Roll- " + mp + " " + DateTime.Now.ToString(), string.Join(Environment.NewLine, outs.ToArray()));
                break;
            }
        }
    }
}
