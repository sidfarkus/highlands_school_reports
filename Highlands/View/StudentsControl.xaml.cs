﻿using System;
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
using Highlands.ViewModel;
using Highlands.StaticModel;

namespace Highlands
{
    /// <summary>
    /// Interaction logic for StudentsControl.xaml
    /// </summary>
    public partial class StudentsControl : UserControl
    {
        private GradebookViewModel Gradebook { get; set; }

        public StudentsControl()
        {
            InitializeComponent();
            LoadGradebook();
        }

        private void FillCourses(MarkingPeriod mp, string gradeLevel)
        {
            cmbCourse.Items.Clear();
            cmbCourse.Items.Add("All");
            var courses = Gradebook.Courses;
            if (mp != null)
                courses = courses.Where(c => c.Quarter == mp.ToString());
            //if (gradeLevel != "All")
            //    courses = courses.Where(c => c.Level == gradeLevel);
            if (chkMyStudents.IsChecked == true)
                courses = courses.Where(c => c.Teacher == UserViewModel.CurrentUser.Name);

            foreach (var course in courses)
                cmbCourse.Items.Add(course);

            cmbCourse.Text = cmbCourse.Items[0].ToString();
        }

        private void FillStudents(string target, MarkingPeriod mp, string gradeLevel, CourseViewModel course)
        {
            lst.Items.Clear();
            var students = Gradebook.Students;
            if (chkMyStudents.IsChecked == true)
                students = students.Where(s => s.HasTeacher(UserViewModel.CurrentUser));
            if (course != null)
                students = students.Where(s => s.HasCourse(course, mp));

            if (gradeLevel != "All")
                students = students.Where(s => s.GradeLevel == gradeLevel);

            if (mp != null)
                students = students.Where(s => s.AttendedDuring(mp));

            if (!string.IsNullOrWhiteSpace(target))
            {
                target = target.ToUpper();
                students = students.Where(s => s.Name.ToUpper().Contains(target) || s.GradeLevel.ToUpper().Contains(target));
            }
            foreach (var student in students)
                lst.Items.Add(student);

            staCount.Content = "Students: " + students.Count();
        }

        private void LoadGradebook()
        {
            Gradebook = GradebookViewModel.Load();

            chkMyStudents.IsChecked = UserViewModel.CurrentUser.HasStudents;
            chkMyStudents.IsEnabled = UserViewModel.CurrentUser.HasStudents;
            btnGradeAll.IsEnabled = UserViewModel.CurrentUser.HasStudents;

            MarkingPeriod.MarkingPeriods.OrderByDescending(q => q.ApproximateEndDate).ToList().ForEach(q => cmbQuarter.Items.Add(q));
            cmbQuarter.Text = cmbQuarter.Items[1].ToString();

            Maintenance.GradeLevelShorts.ToList().ForEach(g => cmbGradeLevel.Items.Add(g));
            cmbGradeLevel.Text = cmbGradeLevel.Items[0].ToString();

            FillCourses(MarkingPeriod.Current, cmbGradeLevel.Text);
            FillStudents(entStudent.Text, cmbQuarter.SelectedItem as MarkingPeriod, cmbGradeLevel.Text, cmbCourse.SelectedItem as CourseViewModel);
        }


        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            FillStudents(entStudent.Text, cmbQuarter.SelectedItem as MarkingPeriod, cmbGradeLevel.Text, cmbCourse.SelectedItem as CourseViewModel);
        }

        private void lst_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var student = lst.SelectedItem as StudentViewModel;
            if (student == null)
                return;
            var studentWindow = new StudentWindow(student);
            studentWindow.ShowDialog();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void chkMyStudents_Click(object sender, RoutedEventArgs e)
        {
            FillStudents(entStudent.Text, cmbQuarter.SelectedItem as MarkingPeriod, cmbGradeLevel.Text, cmbCourse.SelectedItem as CourseViewModel);
        }

        private void cmbQuarter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillCourses((e.AddedItems[0] as MarkingPeriod), cmbGradeLevel.Text);
            FillStudents(entStudent.Text, cmbQuarter.SelectedItem as MarkingPeriod, cmbGradeLevel.Text, cmbCourse.SelectedItem as CourseViewModel);
        }

        private void cmbCourse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FillStudents(entStudent.Text, cmbQuarter.SelectedItem as MarkingPeriod, cmbGradeLevel.Text, cmbCourse.SelectedItem as CourseViewModel);
        }

        private void cmbGradeLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var gradeLevel = e.AddedItems[0] as string;
            FillCourses(cmbQuarter.SelectedItem as MarkingPeriod, gradeLevel);
            FillStudents(entStudent.Text, cmbQuarter.SelectedItem as MarkingPeriod, gradeLevel, cmbCourse.SelectedItem as CourseViewModel);

        }
    }
}
