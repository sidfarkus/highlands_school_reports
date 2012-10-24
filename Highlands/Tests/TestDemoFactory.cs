using Highlands.Model;
using Highlands.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace Highlands.Tests
{
    public class TestDemoFactory
    {
        Gradebook _gradebook;
        public TestDemoFactory()
        {
            _gradebook = DemoFactory.CreateDemo();
        }

        internal void TestAttendance()
        {
           foreach(var student in _gradebook.Student)
           {
               var rows = student.GetAttendanceRows().ToList();
           }
        }

        internal void TestClasses()
        {
            var lines = new List<string>();
            foreach (var student in _gradebook.Student)
            {
                var grades = student.GetGradeRows().GroupBy(g => g.CourseRow.Quarter);
                foreach (var grade in grades)
                {
                    lines.Add(grade.Key + " " + grade.Count());
                }
            }
        }
    }
}
