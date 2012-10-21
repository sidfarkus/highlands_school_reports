﻿using Highlands.StaticModel;
using Highlands.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
            var rv = new Grades();
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
                    return _grade.Subject + " " + _grade.Level + " " + _grade.Group + " " + _grade.Quarter;
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

        }
        public GradebookViewModel _gradebook;

        internal void Refresh(GradebookViewModel gradebook)
        {
            _gradebook = gradebook;
            FillStudents();
        }
    }
}
