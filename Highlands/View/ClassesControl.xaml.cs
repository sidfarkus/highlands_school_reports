using Highlands.StaticModel;
using Highlands.ViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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

namespace Highlands
{
    /// <summary>
    /// Interaction logic for ClassesControl.xaml
    /// </summary>
    public partial class ClassesControl : UserControl
    {
        public ClassesControl()
        {
            InitializeComponent();
        }

        private void FillStudents()
        {
            var rv = FillGrades();

            grd.ItemsSource = rv.ToList();
            ICollectionView defaultView = CollectionViewSource.GetDefaultView(grd.ItemsSource);

            if (defaultView != null && defaultView.CanGroup == true)
            {
                defaultView.GroupDescriptions.Clear();
                defaultView.GroupDescriptions.Add(new PropertyGroupDescription("ClassName"));
                //defaultView.GroupDescriptions.Add(new PropertyGroupDescription("Level"));
                //cvTasks.GroupDescriptions.Add(new PropertyGroupDescription("Complete"));
            }
        }

        private Grades FillGrades()
        {
            var rv = new Grades();
            btnSaveReportCards.Visibility = ViewUtils.IsVisible(UserViewModel.CurrentUser.CanExportReportCards);
            var courses = _gradebook.Courses;
            courses = courses.Where(c => c.Teacher == UserViewModel.CurrentUser.Name);
            courses = courses.Where(c => c.Quarter == MarkingPeriodKey.Current.ToString());

            foreach (var course in courses)
            {
                //grd.Items.Add(course);
                foreach (var grade in course.Grades)
                {
                    rv.Add(new StudentByClass(grade));
                    //rv.Add(new dv() { Grade = grade.LetterGrade, Name = grade.StudentName, Stage = grade.ApprovalStage.ToString() });
                }
            }
            return rv;
        }

        // Requires using System.Collections.ObjectModel; 
        public class Grades : ObservableCollection<StudentByClass>
        {
            // Creating the Tasks collection in this way enables data binding from XAML.
        }
        public class StudentByClass
        {
            private GradeViewModel _grade;

            public StudentByClass(GradeViewModel grade)
            {
                 _grade = grade;
            }

            public GradeViewModel Grade
            {
                get
                {
                    return _grade;
                }
            }

            public string StudentName
            {
                get
                {
                    return _grade.StudentName;
                }
            }
            public string ClassName
            {
                get
                {
                    return _grade.Subject + " " + _grade.Level + " " + _grade.Group + " " + _grade.MarkingPeriod;
                }
            }
            public string LetterGrade
            {
                get
                {
                    return _grade.LetterGrade;
                    }
            }
            public string SpecialGrade
            {
                get
                {
                    return _grade.SpecialGrade;
                }
            }
            public string Stage
            {
                get
                {
                    return _grade.ApprovalStage.ToString();
                }
            }
            public string Comment
            {
                get
                {
                    return _grade.Comment;
                }
            }

            internal string ToCsv()
            {
                return StudentName + "," + _grade.Subject + "," + _grade.Level + "," + _grade.Group + "," + _grade.MarkingPeriod + "," + LetterGrade + "," + SpecialGrade + "," + Stage + "," + Comment;
            }

            internal static string CsvHeaders()
            {
                return "Student" + "," + "Subject" + "," + "Level" + "," + "Group" + "," + "Quarter" + "," + "Grade" + "," + "Second" + "," + "Stage" + "," + "Comment";
            }
        }
        public GradebookViewModel _gradebook;

        internal void Refresh(GradebookViewModel gradebook)
        {
            _gradebook = gradebook;

            FillStudents();
        }

        private void OnGradeSaved(object sender, EventArgs e)
        {
            FillStudents();
        }

        private void OnSaveCanceled(object sender, EventArgs e)
        {
            grd.SelectedItem = null;
            hideEditOverlay.Visibility = System.Windows.Visibility.Visible;
        }

        private void SelectionChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (grd.SelectedItem == null)
                return;

            var student = (StudentByClass)grd.SelectedItem;
            gradeEditor.LoadGrade(student.StudentName, student.Grade);
            hideEditOverlay.Visibility = System.Windows.Visibility.Hidden;
            gradeEditor.ShowBackwards = grd.SelectedIndex > 0;
            gradeEditor.ShowForwards = grd.SelectedIndex < grd.Items.Count - 1;
        }

        private void OnNextStudent(object sender, EventArgs e)
        {
            grd.SelectedIndex += 1;
        }

        private void OnPrevStudent(object sender, EventArgs e)
        {
            grd.SelectedIndex -= 1;
        }

        private void OnCreateAllReports(object sender, RoutedEventArgs e)
        {
            if (!UserViewModel.CurrentUser.CanExportReportCards)
            {
                MessageBox.Show("You do not have permission to export report cards!");
                return;
            }

            var dialog = new WPFFolderBrowser.WPFFolderBrowserDialog("Pick a folder to save all the report cards");
            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                var grades = (List<StudentByClass>)grd.ItemsSource;
                var students = grades.Select(g => {
                    var student = _gradebook.Students.FirstOrDefault(s => s.Name.Equals(g.StudentName, StringComparison.OrdinalIgnoreCase));
                    if (!String.IsNullOrEmpty(g.LetterGrade))
                        return student;
                    return null;
                });

                int notExported = 0, exported = 0;
                foreach (var student in students)
                {
                    if (student != null)
                    {
                        var report = new ReportCard(student);
                        var filename = System.IO.Path.Combine(dialog.FileName, report.GetDefaultReportCardFilename(MarkingPeriod.Current.Key) + ".pdf");
                        report.CreateReportCard(filename, MarkingPeriod.Current);
                        exported++;
                    }
                    else
                    {
                        notExported++;
                    }
                }
                if (notExported > 0)
                {
                    MessageBox.Show(notExported + " student(s) could not be exported because they had no grade set!\n\n" + exported + " student(s) were successfully written.");
                }
            }

        }

        private void btnExportGrades_Click(object sender, RoutedEventArgs e)
        {
            var grades = FillGrades();
            try
            {
                var sfd = new SaveFileDialog();
                sfd.DefaultExt = ".csv";
                sfd.Filter = "CSV files|*.csv|All Files|*.*";
                sfd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                sfd.FileName = "Grades-" + UserViewModel.CurrentUser.Name + "-" + DateTime.Now.ToString("yyyyMMdd HHmmss") + ".csv";
                if (sfd.ShowDialog() != true)
                    return;

                var outs = new List<string>();
                outs.Add(StudentByClass.CsvHeaders());
                foreach (var grade in grades)
                    outs.Add(grade.ToCsv());

                File.WriteAllLines(sfd.FileName, outs.ToArray());
                
                Process.Start(sfd.FileName);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }
    }
}
