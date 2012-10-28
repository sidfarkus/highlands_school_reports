using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Highlands.StaticModel;

namespace Highlands.ViewModel
{
    public class HonorRollsViewModel : INotifyPropertyChanged
    {
        private IEnumerable<StudentViewModel> allStudents = null;
        public HonorRollsViewModel()
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
                students = GradebookViewModel.Load().Students.ToArray();
                allStudents = students;
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
                Changed("Students");
            }
        }

        private IEnumerable<StudentViewModel> FilterStudents(MarkingPeriodKey currentQuarter)
        {
            if (allStudents == null)
                return Enumerable.Empty<StudentViewModel>();
            return allStudents.Where(s => s.Grades.Any(g => g.MarkingPeriod.Equals(currentQuarter)));
        }

        private IEnumerable<StudentViewModel> HonorRollsStudents(MarkingPeriodKey currentQuarter)
        {
            if (allStudents == null)
                return Enumerable.Empty<StudentViewModel>();
            return allStudents.Where(s => s.Grades.Any(g => g.MarkingPeriod.Equals(currentQuarter)));
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
