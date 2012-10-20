using Highlands.StaticModel;
using Highlands.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Highlands
{
    /// <summary>
    /// Interaction logic for StudentWindow.xaml
    /// </summary>
    public partial class StudentWindow : Window
    {
        StudentViewModel _student;
        public StudentWindow(StudentViewModel student)
        {
            InitializeComponent();
            _student = student;

            Title = _student.Name;
            var currentUser = UserViewModel.CurrentUser;
            
            chkMyClasses.IsChecked = currentUser.HasStudents;
            chkMyClasses.IsEnabled = currentUser.HasStudents;
            gridStudentInfo.IsEnabled = currentUser.CanEditStudentInfo;

            _sdScores = new ObservableCollection<SDScoreViewModel>();
            _grades = new ObservableCollection<GradeViewModel>();
            RefreshMarks();

            entName.Text = _student.Name;
            dtpDob.SelectedDate = _student.DOB;
            entAddress.Text = _student.Address;
            dtpEnrolled.SelectedDate = _student.DateEnrolled;
           if (_student.DateWithdrawn.HasValue)
                dtpWithdrawn.SelectedDate = _student.DateWithdrawn.Value;
            chkWithdrawn.IsChecked = _student.DateWithdrawn.HasValue;
            Maintenance.GradeLevelShorts.ForEach(o => cmbGradeLevel.Items.Add(o));
            cmbGradeLevel.Text = _student.GradeLevel;

            dgvGrades.ItemsSource = Grades;

            dgvSelfDevelopment.ItemsSource = SDScores;

            MarkingPeriod.MarkingPeriods.ForEach(o => cmbMarkingPeriod.Items.Add(o));
            cmbMarkingPeriod.Text = MarkingPeriod.Current.ToString();
            
            //dgv.Items.Add(new List<string>() { "A", "B", "c" });
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            dtpWithdrawn.IsEnabled = chkWithdrawn.IsChecked == true;
            if (chkWithdrawn.IsChecked == true && _student.DateWithdrawn == null)
                dtpWithdrawn.SelectedDate = DateTime.Today;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
        ObservableCollection<GradeViewModel> _grades;
        public ObservableCollection<GradeViewModel> Grades
        {
            get
            {
                if (_grades == null)
                {
                    RefreshMarks();
                }
                return _grades;
            }
        }
        ObservableCollection<SDScoreViewModel> _sdScores;
        public ObservableCollection<SDScoreViewModel> SDScores
        {
            get
            {
                if (_sdScores == null)
                {
                    RefreshMarks();
                }
                return _sdScores;
            }
        }
        
        
        enum ShowRange
        {
            All,
            ThisYear,
            ThisQuarter
        }

        public string TeacherName
        {
            get
            {
                //var ds = _student.Table.DataSet as Gradebook;
                return UserViewModel.CurrentUser.Name;
            }
        }
        private void RefreshMarks()
        {
            if (_student == null || !_student.IsValid)
                return;

            var subsetGrades = _student.Grades;
            var subsetSDScores = _student.SelfDevelopmentScores;

            if (chkMyClasses.IsChecked == true)
            {
                subsetGrades = subsetGrades.Where(g => g.Teacher == TeacherName);
            }
            if (_range == ShowRange.ThisQuarter)
            {
                subsetGrades = subsetGrades.Where(g => g.Quarter == MarkingPeriod.Current.ToString());
                subsetSDScores = subsetSDScores.Where(g => g.Quarter == MarkingPeriod.Current.ToString());
            }
            else if (_range == ShowRange.ThisYear)
            {
                subsetGrades = subsetGrades.Where(g => g.Quarter.Contains("2012"));
                subsetSDScores = subsetSDScores.Where(g => g.Quarter.Contains("2012"));
            }

            _grades.Clear();
            subsetGrades.ToList().ForEach(g => _grades.Add(g));
            _sdScores.Clear();
            subsetSDScores.ToList().ForEach(s => _sdScores.Add(s));
        }

        ShowRange _range = ShowRange.All;
        private void radThisQuarter_Click(object sender, RoutedEventArgs e)
        {
            _range = ShowRange.ThisQuarter;
            RefreshMarks();
        }

        private void radAll_Click(object sender, RoutedEventArgs e)
        {
            _range = ShowRange.All;
            RefreshMarks();
        }

        private void radThisYear_Click(object sender, RoutedEventArgs e)
        {
            _range = ShowRange.ThisYear;
            RefreshMarks();
        }

        private void chkMyClasses_Click(object sender, RoutedEventArgs e)
        {
            RefreshMarks();
        }

        private void dgvSelfDevelopment_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
        }

        private void btnEditGrade_Click(object sender, RoutedEventArgs e)
        {
            var context = (sender as Button).DataContext;
            var grade = context as GradeViewModel;
            if (!UserViewModel.CurrentUser.CanEdit(grade.Subject))
            {
                MessageBox.Show("User can not edit this grade.");
                return;
            }
            var window = new EditGrade(_student.Name, grade);
            window.ShowDialog();
            RefreshMarks();
        }


        private void dptDob_Changed(object sender, SelectionChangedEventArgs e)
        {
            var dob = dtpDob.SelectedDate.Value;
            staAge.Content = StudentViewModel.GetAge(dob).ToString("0") + " years old";

        }

        private void btnApprove_Click(object sender, RoutedEventArgs e)
        {
            var context = (sender as Button).DataContext;
            var grade = context as GradeViewModel;
            if (!UserViewModel.CurrentUser.CanApprove(grade.Subject))
            {
                MessageBox.Show("User can not approve/lock this grade.");
                return;
            }
            if (MessageBoxResult.Yes != MessageBox.Show("Are you sure you want to lock " + grade + "?", "Approval", MessageBoxButton.YesNo))
                return;

            grade.Approve();
            dgvGrades.ItemsSource = Grades;
        }

        private void OnGenerateReportCard(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog()
            {
                AddExtension = true,
                Filter = "PDF Files (*.pdf)|*.pdf"
            };
            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                try
                {
                    _student.CreateReportCard(dialog.FileName, MarkingPeriod.Current);
                }
                catch (System.IO.IOException)
                {
                    MessageBox.Show("Error writing the report card! Check to ensure you have permission to write to this folder and that the file is not currently in use.");
                }
            }
        }

     }
 }
