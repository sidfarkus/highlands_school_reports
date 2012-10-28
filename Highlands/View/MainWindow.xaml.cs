//using Highlands.Model;
using Highlands.StaticModel;
using Highlands.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            tabClasses.Visibility = ViewUtils.IsVisible(UserViewModel.CurrentUser.HasStudents);
            
            staUser.Content = "Welcome, " + UserViewModel.CurrentUser;
      
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
                Process.Start(sfd.FileName);     
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        private void btnHonorRoll_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.DefaultExt = ".csv";
            sfd.Filter = "CSV files|*.csv|All Files|*.*";
            if (sfd.ShowDialog() != true)
                return; 
            
            var outs = new List<string>();
            foreach (var mp in MarkingPeriods.Singleton.OrderByDescending(m => m.StartDate))
            {
                //var mp = MarkingPeriod.Current;
                var students = _gradebook.Students.Where(s => s.HonorRoll(mp)).OrderByDescending(s => s.Gpa(mp));
                outs.AddRange(AddHonorRollLines(mp, students)); 
                if (outs.Count() == 0)
                    continue;

                outs.Insert(0, "=For Quarter=");
                var year = new MarkingYear(mp);

                outs.Add("=For Year To Date=");
                outs.AddRange(AddHonorRollLines(year, students));


                ViewUtils.WriteAndOpen(sfd.FileName, outs);
                //ViewUtils.Mail("", "Honor Roll " + mp + " " + DateTime.Now, string.Join(Environment.NewLine, outs.ToArray()));

                return;
            }
            MessageBox.Show("No honor roll found.");
        }

        private static IList<string> AddHonorRollLines(Period p, IEnumerable<StudentViewModel> students)
        {
            var outs = new List<string>();
            foreach (var kvp in students.OrderBy(s => Maintenance.GradeLevelNumber(s.GradeLevel)).GroupBy(s => s.GradeLevel))
            {
                outs.Add("-" + StudentViewModel.FormatGradeLevel(kvp.Key) + "-");
                foreach (var student in kvp)
                    outs.Add(student.Name + "," + student.GradeLevel + "," + student.Gpa(p).ToString("0.00"));
            }
            return outs;
        }
    }
}
