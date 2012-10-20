using Highlands.Model;
using Highlands.StaticModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Highlands.Utils;

namespace Highlands.ViewModel
{
    public class StudentViewModel
    {
        Gradebook.StudentRow _studentRow;
        public StudentViewModel(Gradebook.StudentRow studentRow)
        {
            _studentRow = studentRow;  
        }

        [PDFOutputField("CommentRow1")]
        public string Name
        {
            get
            {
                return _studentRow.Name;
            }
        }


        public DateTime DOB
        {
            get
            {
                return _studentRow.DOB;
            }
        }

        public string Address
        {
            get
            {
                return _studentRow.Address;
            }
        }

        public DateTime DateEnrolled 
        {
            get
            {
                return _studentRow.DateEnrolled;
            }
        }

        public string GradeLevel
        {
            get
            {
                return _studentRow.GradeLevel;
            }
        }

        public int Age
        {
            get
            {
                return GetAge(DOB);
            }
        }

        public DateTime? DateWithdrawn
        {
            get
            {
                if (_studentRow.DateWithdrawn == DateTime.MaxValue)
                    return null;
                return _studentRow.DateWithdrawn;
            }
        }

        internal IEnumerable<GradeViewModel> Grades
        {
            get
            {
                var rv = new List<GradeViewModel>();
                var grades = _studentRow.GetGradeRows().ToList();
                grades.ForEach(g => rv.Add(new GradeViewModel(g)));
                return rv;
            }
        }
        internal IEnumerable<SDScoreViewModel> SelfDevelopmentScores
        {
            get
            {
                var rv = new List<SDScoreViewModel>();
                var scores = _studentRow.GetSelfDevelopmentRows().ToList();
                scores.ForEach(s => rv.Add(new SDScoreViewModel(s)));
                return rv;
            }
        }

        public bool IsValid
        {
            get
            {
                return _studentRow != null;
            }
        }
        static public string FormatGradeLevel(string gradeLevel)
        {
            if (gradeLevel == "K")
                return "Kindergarten";
            return gradeLevel + " grade";
        }

        internal bool HasTeacher(UserViewModel user)
        {
            return _studentRow.HasTeacher(user.Name);
        }

        internal bool HasCourse(CourseViewModel course, StaticModel.MarkingPeriod mp)
        {
            return _studentRow.HasCourse(course.CourseRow, mp);
        }

        public void CreateReportCard(string outFilename)
        {
            var outputProperties = typeof(StudentViewModel).GetProperties()
                .Where(p => p.CanRead && p.GetCustomAttributes(typeof(PDFOutputFieldAttribute), false).Any());
            
            PDFWriter.WritePDF(outFilename, outputProperties.ToDictionary(
                p => p.GetCustomAttributes(typeof(PDFOutputFieldAttribute), false).Cast<PDFOutputFieldAttribute>().First().FieldName,
                p => p.GetValue(this).ToString()));
        }

        public override string ToString()
        {
            var rv = Name + ", dob-" + DOB.ToShortDateString() + "(" + Age + "yo), " + FormatGradeLevel(GradeLevel) + ", since:" + DateEnrolled.ToShortDateString();
            if (DateWithdrawn != null)
                rv += ", withdrawn:" + DateWithdrawn.Value.ToShortDateString();
            return rv;
        }

        internal bool AttendedDuring(MarkingPeriod mp)
        {
            return _studentRow.AttendedDuring(mp);
        }

        public static int GetAge(DateTime dob)
        {
            var today = DateTime.Today;
            var years = today.Year - dob.Year;
            if (today.DayOfYear < dob.DayOfYear)
                years -= 1;
            return years;
        }
    }
}
