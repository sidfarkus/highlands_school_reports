using Highlands.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Highlands.ViewModel
{
    public class GradebookViewModel
    {
        Gradebook _gradebook;
        public GradebookViewModel(Gradebook gradebook)
        {
            _gradebook = gradebook;
        }
        internal IEnumerable<StudentViewModel> Students
        {
            get
            {
                var rv = new List<StudentViewModel>();

                var students = _gradebook.Student.ToList();
                students.ForEach(s => rv.Add(new StudentViewModel(s)));
                return rv;
            }
        }

        internal IEnumerable<CourseViewModel> Courses
        {
            get
            {
                var rv = new List<CourseViewModel>();

                var courses = _gradebook.Course.ToList();
                courses.ForEach(c => rv.Add(new CourseViewModel(c)));
                return rv;
            }
        }

        internal static GradebookViewModel Load()
        {
            var gradebook = Gradebook.Read();
            if (gradebook == null)
                gradebook = DemoFactory.CreateDemo();
            return new GradebookViewModel(gradebook);
        }

        internal void Reset()
        {
            _gradebook = null;
            UserViewModel.Reset();
        }
    }
}
