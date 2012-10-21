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
    public partial class StudentWindow : UserControl
    {
        StudentViewModel _student;

        ShowRange _range = ShowRange.All;

        public StudentWindow()
        {
            InitializeComponent();
        }
        
        public StudentWindow(StudentViewModel student) : this()
        {
            LoadStudent(student);
        }

        public void ClearStudent()
        {
            noStudentOverlay.Visibility = System.Windows.Visibility.Visible;
        }

        public void LoadStudent(StudentViewModel student)
        {
            _student = student;
            nameHeader.Text = student.Name;
            noStudentOverlay.Visibility = System.Windows.Visibility.Hidden;
            var currentUser = UserViewModel.CurrentUser;

            chkMyClasses.IsChecked = currentUser.HasStudents;
            chkMyClasses.IsEnabled = currentUser.HasStudents;
            gridStudentInfo.IsEnabled = currentUser.CanEditStudentInfo;

            _sdScores = new ObservableCollection<SDScoreViewModel>();
            _grades = new ObservableCollection<GradeViewModel>();
            RefreshMarks();

            entName.Text = _student.Name;
            dtpDob.SelectedDate = _student.DOB;
            entAddress.Text = _student.AddressLine1 + "," + student.AddressLine2;
            dtpEnrolled.SelectedDate = _student.DateEnrolled;
            if (_student.DateWithdrawn.HasValue)
                dtpWithdrawn.SelectedDate = _student.DateWithdrawn.Value;
            chkWithdrawn.IsChecked = _student.DateWithdrawn.HasValue;
            Maintenance.GradeLevelShorts.ForEach(o => cmbGradeLevel.Items.Add(o));
            cmbGradeLevel.Text = _student.GradeLevel;

            dgvGrades.ItemsSource = Grades;
            dgvSelfDevelopment.ItemsSource = SDScores;
            
            var periods = new HashSet<string>();
            cmbMarkingPeriod.ItemsSource = student.Grades.Select(g => MarkingPeriodKey.Parse(g.Quarter))
                .Where(period => { 
                    var hadItem = periods.Contains(period.ToString());
                    if (!hadItem) periods.Add(period.ToString());
                    return !hadItem;
                })
                .OrderByDescending(period => period.ApproximateEndDate)
                .ThenBy(period => period.Quarter);
            if (cmbMarkingPeriod.Items.Count > 0)
            {
                cmbMarkingPeriod.SelectedIndex = 0;
                genReport.IsEnabled = true;
            }
            else
            {
                genReport.IsEnabled = false;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            dtpWithdrawn.IsEnabled = chkWithdrawn.IsChecked == true;
            if (chkWithdrawn.IsChecked == true && _student.DateWithdrawn == null)
                dtpWithdrawn.SelectedDate = DateTime.Today;
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
                subsetGrades = subsetGrades.Where(g => g.Quarter == MarkingPeriodKey.Current.ToString());
                subsetSDScores = subsetSDScores.Where(g => g.Quarter == MarkingPeriodKey.Current.ToString());
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

        private void btnEditGrade_Click(object sender, RoutedEventArgs e)
        {
            var grade = dgvGrades.SelectedValue as GradeViewModel;
            var rights = UserViewModel.CurrentUser.CanEdit(grade);
            if (RightsEnum.Success != UserViewModel.CurrentUser.CanEdit(grade))
            {
                MessageBox.Show("User can not edit this grade at this stage.");
                return;
            }
            var window = new EditGradeWindow(_student.Name, grade);
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
            var grade = dgvGrades.SelectedValue as GradeViewModel;
            var result = UserViewModel.CurrentUser.CanApprove(grade);
            if (RightsEnum.Success != result)
            {
                if (RightsEnum.GradeError == result)
                    MessageBox.Show("Grade is not ready to be approved/locked.");
                else
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
            var period = (MarkingPeriodKey) cmbMarkingPeriod.SelectedItem;
            var dialog = new Microsoft.Win32.SaveFileDialog()
            {
                AddExtension = true,
                Filter = "PDF Files (*.pdf)|*.pdf",
                FileName = String.Format("{0}_Q{1}_{2}_Report_Card.pdf", _student.Name, period.Quarter, period.EndingSchoolYear)
            };
            var result = dialog.ShowDialog();
            if (result.HasValue && result.Value)
            {
                try
                {
                    _student.CreateReportCard(dialog.FileName, period);
                }
                catch (System.IO.IOException)
                {
                    MessageBox.Show("Error writing the report card! Check to ensure you have permission to write to this folder and that the file is not currently in use.");
                }
            }
        }

        private void btnUnApprove_Click(object sender, RoutedEventArgs e)
        {
            var grade = dgvGrades.SelectedValue as GradeViewModel;
            var result = UserViewModel.CurrentUser.CanUnApprove(grade);
            if (RightsEnum.Success != result)
            {
                if (RightsEnum.GradeError == result)
                    MessageBox.Show("Grade is not ready to be approved/locked.");
                else
                    MessageBox.Show("User can not approve/lock this grade.");
                return;
            }
            if (MessageBoxResult.Yes != MessageBox.Show("Are you sure you want to lock " + grade + "?", "Approval", MessageBoxButton.YesNo))
                return;

            grade.UnApprove();
            dgvGrades.ItemsSource = Grades;
        }

        private void btnEmail_Click(object sender, RoutedEventArgs e)
        {
            var context = (sender as Button).DataContext;
            var grade = context as GradeViewModel;
            var teacher = grade.TeacherVm;
            ViewUtils.Mail(teacher.EmailAddress, "Highlands School Report for " + grade.StudentName, Environment.NewLine + Environment.NewLine + grade.ForMail());
        }

        private void OnSaveUser(object sender, RoutedEventArgs e)
        {
            _student.Save();
        }

        private void SaveSDScore(object sender, RoutedEventArgs e)
        {
            var sd = (SDScoreViewModel) dgvSelfDevelopment.SelectedValue;
            var scoreBox = (TextBox) LogicalTreeHelper.FindLogicalNode(((Button)sender).Parent, "scoreBox");
            var score = 0;
            if (Int32.TryParse(scoreBox.Text, out score))
            {
                sd.Score = score;
                _student.Save();
                RefreshMarks();
            }
            scoreBox.Text = "";
        }

     }
 }
