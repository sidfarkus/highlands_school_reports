using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Highlands.StaticModel;

namespace Highlands.ViewModel
{
    public class AttendenceViewModel : INotifyPropertyChanged
    {
        private IEnumerable<StudentViewModel> allStudents = null;
        public AttendenceViewModel()
        {
            if (!quarters.Any())
            {
                quarters = MarkingPeriods.Singleton.Select(p => p.Key)
                    .OrderByDescending(p => p.EndingSchoolYear)
                    .ThenBy(p => p.Quarter)
                    .ToList();
                CurrentQuarter = quarters.FirstOrDefault();
            }
            if (!students.Any())
            {
                Students = GradebookViewModel.Load().Students.ToArray();
                allStudents = students;
            }
        }

        public bool CanEdit
        {
            get
            {
                return UserViewModel.CurrentUser.CanEditAttendance;
            }
        }

        private DateTime currentDay = DateTime.Now;
        public DateTime CurrentDay
        {
            get
            {
                return currentDay;
            }
            set
            {
                currentDay = value;
                Changed("CurrentDay");
            }
        }

        private DateTime quarterStart = DateTime.MinValue;
        public DateTime QuarterStart
        {
            get
            {
                return quarterStart;
            }
            set
            {
                quarterStart = value;
                Changed("QuarterStart");
            }
        }

        private DateTime quarterEnd = DateTime.Now;
        public DateTime QuarterEnd
        {
            get
            {
                return quarterEnd;
            }
            set
            {
                quarterEnd = value;
                Changed("QuarterEnd");
            }
        }

        private IEnumerable<MarkingPeriodKey> quarters = Enumerable.Empty<MarkingPeriodKey>();
        public IEnumerable<MarkingPeriodKey> Quarters
        {
            get
            {
                return quarters;
            }
            set
            {
                quarters = value;
                Changed("Quarters");
            }
        }

        private MarkingPeriodKey currentQuarter = new MarkingPeriodKey(0, 0);
        public MarkingPeriodKey CurrentQuarter
        {
            get
            {
                return currentQuarter;
            }
            set
            {
                currentQuarter = value;
                Students = FilterStudents(currentQuarter);

                var period = MarkingPeriods.Singleton.Find(p => p.Key.Equals(currentQuarter));
                QuarterEnd = period.EndDate;
                QuarterStart = period.StartDate;
                if (CurrentDay > QuarterEnd || CurrentDay < QuarterStart)
                    CurrentDay = QuarterEnd;

                Changed("CurrentQuarter");
            }
        }


        private IEnumerable<StudentViewModel> students = Enumerable.Empty<StudentViewModel>();
        public IEnumerable<StudentViewModel> Students
        {
            get
            {
                return students;
            }
            set
            {
                students = value;
                foreach (var student in students)
                    student.AttendenceForDay = student.GetAttendanceForDay(CurrentDay).Status;
                
                Changed("Students");
            }
        }

        private IEnumerable<StudentViewModel> FilterStudents(MarkingPeriodKey currentQuarter)
        {
            if (allStudents == null)
                return Enumerable.Empty<StudentViewModel>();
            return allStudents.Where(s => s.Grades.Any(g => MarkingPeriodKey.Parse(g.Quarter).Equals(currentQuarter)));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Changed(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }
}
