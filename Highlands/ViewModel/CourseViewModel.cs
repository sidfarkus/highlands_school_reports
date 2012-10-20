using Highlands.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Highlands.ViewModel
{
    public class CourseViewModel
    {
        public CourseViewModel(Gradebook.CourseRow courseRow)
        {
            CourseRow = courseRow;
        }
        public Gradebook.CourseRow CourseRow { get; set; }

        public string Quarter
        {
            get
            {
                return CourseRow.Quarter;
            }
        }

        public string Level
        {
            get
            {
                return CourseRow.Level;
            }
        }

        public string Teacher
        {
            get
            {
                return CourseRow.Teacher;
            }
        }

        public override string ToString()
        {
            return CourseRow.SubjectName + "-" + Level + ", " + Teacher + ", " + Quarter + " " + CourseRow.Group;
        }

        internal static bool HasSpecialGrade(string subject)
        {
            return (subject == "Math"|| subject == "Reading"|| subject == "Written Expression") ;
        }

        // "Reading", "Phonics/Spelling", "Written Expression", "Math", "Science", "Social Studies", "Mind That's Mine", "Art", "Music", "Health and Fitness"
        internal static bool SpecialCourse(string subject)
        {
            return (subject == "Art" || subject == "Music" || subject == "Health and Fitness");
        }
        internal static bool NonSpecialCourse(string subject)
        {
            return (subject == "Reading" || subject == "Phonics/Spelling" || subject == "Written Expression" || subject == "Math");
        }
        internal static bool ClassroomCourse(string subject)
        {
            return (subject == "Science" || subject == "Social Studies" || subject == "Mind That's Mine");
        }


    }
}
