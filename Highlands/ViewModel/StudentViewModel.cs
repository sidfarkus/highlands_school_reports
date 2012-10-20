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

        [PDFOutputField("StudentName")]
        public string Name
        {
            get
            {
                return _studentRow.Name;
            }
        }

        [PDFOutputField("DOB")]
        public DateTime DOB
        {
            get
            {
                return _studentRow.DOB;
            }
        }

        [PDFOutputField("StudentAddress")]
        public string Address
        {
            get
            {
                return _studentRow.Address;
            }
        }

        [PDFOutputField("DateEnrolled")]
        public DateTime DateEnrolled
        {
            get
            {
                return _studentRow.DateEnrolled;
            }
        }

        [PDFOutputField("Grade")]
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

        [PDFOutputField("DateWithdrawn")]
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

        public void CreateReportCard(string outFilename, MarkingPeriodKey period)
        {
            var currentSubjects = Grades.GroupBy(g => g.Subject)
                .Where(subject => subject.Any(subGrade => subGrade.IsCurrentForPeriod(period)))
                .OrderBy(subject => subject.Key);
            var gradeReportFields = currentSubjects.Select((subject, i) => {
                var thisYearsGrades = subject.Where(subGrade => subGrade.ShouldShowOnReportCard(period))
                    .Select(subGrade => subGrade.GetGradeReportFields(period, i + 1));
                return thisYearsGrades.SelectMany(x => x);
            }).SelectMany(x => x);
            var outputProperties = gradeReportFields.Concat(GetPDFFields(this))
                .Concat(GetReportLevelFields(period))
                .Concat(GetSelfDevFields(period));

            PDFWriter.WritePDF(outFilename, outputProperties.ToDictionary(p => p.Key, p => p.Value));
        }

        private IEnumerable<KeyValuePair<string, string>> GetSelfDevFields(MarkingPeriodKey period)
        {
            var sdAreas = Maintenance.SelfDevelopmentAreas;
            return SelfDevelopmentScores.Where(score => MarkingPeriodKey.Parse(score.Quarter).Equals(period))
                .Select((s, i) => new[] { 
                    new KeyValuePair<string, string>("SD" + i + 1, sdAreas[i]),
                    new KeyValuePair<string, string>("SDValue" + i + 1, s.Score.ToString())
                }).SelectMany(x => x);
        }

        private IEnumerable<KeyValuePair<string, string>> GetReportLevelFields(MarkingPeriodKey period)
        {
            yield return new KeyValuePair<string, string>(
                "SchoolYear",
                String.Format("{0} - {1}", period.ApproximateStartDate.ToString("MMM, yyyy"), period.ApproximateStartDate.ToString("MMM, yyyy")));

        }

        private IEnumerable<KeyValuePair<string, string>> GetPDFFields<T>(T obj)
        {
            var outputProperties = typeof(T).GetProperties()
                .Where(p => p.CanRead && p.GetCustomAttributes(typeof(PDFOutputFieldAttribute), false).Any());

            foreach (var prop in outputProperties)
            {
                var val = prop.GetValue(obj) ?? "";
                if (val is DateTime)
                    val = ((DateTime) val).ToShortDateString();
                yield return new KeyValuePair<string, string>(
                    prop.GetCustomAttributes(typeof(PDFOutputFieldAttribute), false).Cast<PDFOutputFieldAttribute>().First().FieldName,
                    val.ToString());
            }
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
