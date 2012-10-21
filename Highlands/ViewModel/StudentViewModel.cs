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
            set
            {
                _studentRow.Name = value;
            }
        }

        [PDFOutputField("DOB")]
        public DateTime DOB
        {
            get
            {
                return _studentRow.DOB;
            }
            set
            {
                _studentRow.DOB = value;
            }
        }

        public string AddressLine1
        {
            get
            {
                return _studentRow.AddressLine1;
            }
            set
            {
                _studentRow.AddressLine1 = value;
            }
        }

        public string AddressLine2
        {
            get
            {
                return _studentRow.AddressLine2;
            }
            set
            {
                _studentRow.AddressLine2 = value;
            }
        }

        [PDFOutputField("DateEnrolled")]
        public DateTime DateEnrolled
        {
            get
            {
                return _studentRow.DateEnrolled;
            }
            set
            {
                _studentRow.DateEnrolled = value;
            }
        }

        [PDFOutputField("Grade")]
        public string GradeLevel
        {
            get
            {
                return _studentRow.GradeLevel;
            }
            set
            {
                _studentRow.GradeLevel = value;
            }
        }

        public int Age
        {
            get
            {
                return GetAge(DOB);
            }
        }

        public AttendanceStatus GetAttendanceForDay(DateTime date)
        {
            var rv = AttendanceStatus.NotEntered;
            var attendanceRow = _studentRow.GetAttendanceRows().SingleOrDefault(a => a.StudentKey == _studentRow.Key && a.Date == date);
            if (attendanceRow != null)
                rv = (AttendanceStatus) Enum.Parse(typeof(AttendanceStatus), attendanceRow.State);
            return rv;
        }

        public void SetAttendanceForDay(DateTime date, string quarter, AttendanceStatus status)
        {
            var attendanceRow = _studentRow.GetAttendanceRows().SingleOrDefault(a => a.StudentKey == _studentRow.Key && a.Date == date);
            var ds = (_studentRow.Table.DataSet as Gradebook);
            var attendanceTable = ds.Attendance;
            if (attendanceRow != null)
                attendanceTable.AddAttendanceRow(_studentRow, quarter, date, status.ToString());
            else
                attendanceRow.State = status.ToString();
            return;
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
            set
            {
                _studentRow.DateWithdrawn = value ?? DateTime.MaxValue;
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

        public AttendanceStatus AttendenceForDay
        {
            get;
            set;
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

        public void Save()
        {
            ((Gradebook)_studentRow.Table.DataSet).Save();
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
                .OrderBy(subject => Maintenance.GetSubjectIndex(subject.Key));
            var gradeReportFields = currentSubjects.Select((subject, i) =>
            {
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
            var o = SelfDevelopmentScores.Where(score => MarkingPeriodKey.Parse(score.Quarter).Equals(period));
            var p = o.Select((s, i) => new[] { 
                    new KeyValuePair<string, string>("SD" + (i + 1), sdAreas[i]),
                    new KeyValuePair<string, string>("SDValue" + (i == 10 ? 12 : i + 1), s.Score.ToString())
                }).SelectMany(x => x);
            return p;
        }

        private IEnumerable<KeyValuePair<string, string>> GetReportLevelFields(MarkingPeriodKey period)
        {
            var thisQuarter = MarkingPeriods.Singleton.Find(p => p.Key.Equals(period));
            var thisYear = MarkingPeriods.Singleton.Where(p => {
                return p.Key.EndingSchoolYear == period.EndingSchoolYear &&
                       p.Key.Quarter <= period.Quarter;
            }).Select(p => p.DaysInQuarter).Sum();
            var yearStart = MarkingPeriods.Singleton.Where(p => p.Key.EndingSchoolYear == period.EndingSchoolYear && p.Key.Quarter == 1)
                .Select(x => x.StartDate)
                .FirstOrDefault();

            return new[] { 
                new KeyValuePair<string, string>(
                    "StudentAddress",
                    AddressLine1 + "\n" + AddressLine2),
                new KeyValuePair<string, string>(
                    "SchoolYear",
                    String.Format("{0:MMM dd, yyyy} - {1:MMM dd, yyyy}", yearStart, thisQuarter.EndDate)),
                new KeyValuePair<string, string>(
                    "SchoolYearAttendance",
                    String.Format("{0} - {1}", thisQuarter.SchoolYear.Item1, thisQuarter.SchoolYear.Item2)),
                new KeyValuePair<string, string>(
                    "QTR1",
                    thisQuarter.DaysInQuarter.ToString()),
                new KeyValuePair<string, string>(
                    "YR1",
                    thisYear.ToString()),
                new KeyValuePair<string, string>(
                    "QTR2",
                    "AbsentQ"),
                new KeyValuePair<string, string>(
                    "YR2",
                    "AbsentY"),
                new KeyValuePair<string, string>(
                    "QTR3",
                    "TardyQ"),
                new KeyValuePair<string, string>(
                    "YR3",
                    "TardyY"),
                new KeyValuePair<string, string>(
                    "MarkingPeriod",
                    period.QuarterString),
                new KeyValuePair<string, string>(
                    "QtrEnding",
                    thisQuarter != null ? thisQuarter.EndDate.ToShortDateString() : "N/A")
            };
        }

        private IEnumerable<KeyValuePair<string, string>> GetPDFFields<T>(T obj)
        {
            var outputProperties = typeof(T).GetProperties()
                .Where(p => p.CanRead && p.GetCustomAttributes(typeof(PDFOutputFieldAttribute), false).Any());

            foreach (var prop in outputProperties)
            {
                var val = prop.GetValue(obj) ?? "";
                if (val is DateTime)
                    val = ((DateTime)val).ToShortDateString();
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

        public string GetDefaultReportCardFilename(MarkingPeriodKey period)
        {
            return String.Format("{0}_Q{1}_{2}_Report_Card.pdf", Name, period.Quarter, period.EndingSchoolYear);
        }
    }
}
