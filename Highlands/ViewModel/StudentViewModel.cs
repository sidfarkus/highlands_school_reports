using Highlands.Model;
using Highlands.StaticModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Highlands.Utils;
using System.Diagnostics;

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

        public int DaysAttendance(Period p, AttendanceStatus status)
        {
            return _studentRow.GetAttendanceRows().Count(a => a.Date >= p.StartDate && a.Date <= p.EndDate && a.State == status.ToString());
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

        public AttendanceMiniVm GetAttendanceForDay(DateTime date)
        {

            var rv = new AttendanceMiniVm();
            rv.Date = date;
            rv.Status = AttendanceStatus.OnTime;
            var attendanceRow = _studentRow.GetAttendanceRows().SingleOrDefault(a => a.StudentKey == _studentRow.Key && a.Date == date);
            if (attendanceRow != null)
                rv.Status = (AttendanceStatus) Enum.Parse(typeof(AttendanceStatus), attendanceRow.State);
            return rv;
        }

        public void SetAttendanceForDay(AttendanceStatus status)
        {
            var date = CurrentDayForAttendance;
            var attendanceRow = _studentRow.GetAttendanceRows().SingleOrDefault(a => a.StudentKey == _studentRow.Key && a.Date == date);
            var ds = (_studentRow.Table.DataSet as Gradebook);
            var attendanceTable = ds.Attendance;
            if (attendanceRow == null)
            {
                if (status != AttendanceStatus.OnTime)  // don't bother saving it there are on time
                    attendanceTable.AddAttendanceRow(_studentRow, date, status.ToString());
            }
            else
                attendanceRow.State = status.ToString();
            return;
        }
        public DateTime CurrentDayForAttendance { get; set; } 

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
        public class AttendanceMiniVm
        {
            public AttendanceStatus Status { get; set; }
            public DateTime Date { get; set; }
            public override string ToString()
            {
                return Status.ToString();
            }
        }

        AttendanceStatus _attendanceStatus;
        public AttendanceStatus AttendenceForDay
        {
            get
            {
                return _attendanceStatus;
            }
            set
            {
                _attendanceStatus = value;
                SetAttendanceForDay(_attendanceStatus);
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

        internal double Gpa(Period p)
        {
             var grades = _studentRow.GetGradeRows().Where(g => p.ContainsQuarter(g.CourseRow.MarkingPeriod));
            return grades.Average(g => Maintenance.GradePoint(g.LetterGrade));
        }

        internal bool HonorRoll(Period p)
        {
            var grades = _studentRow.GetGradeRows().Where(g => g.CourseRow.MarkingPeriod == p);
            if (grades.Any(g => Maintenance.GradePoint(g.LetterGrade) < 1.0))
                return false;
            if (grades.Count() == 0)
                return false;
            var avg = grades.Average(g => Maintenance.GradePoint(g.LetterGrade));
            var lines = grades.Select(g => g.LetterGrade).ToList();
            if (avg >= 2.999)
                return true;
            return false;
        }

        internal IEnumerable<StudentReport> StudentReports()
        {
            var rv = new List<StudentReport>();
            var gradesByQuarter = _studentRow.GetGradeRows().GroupBy(g => g.CourseRow.Quarter);
            foreach (var kvp in gradesByQuarter)
            {
                var i = new StudentReport();
                var mp = MarkingPeriods.Singleton.Find(MarkingPeriodKey.Parse(kvp.Key));
                i.Quarter = mp.ToString();
                i.NumberOfClasses = kvp.Count();
                i.Gpa = Gpa(mp);
                i.Stage = (ApprovalStage) kvp.Max(g => (int) Enum.Parse(typeof(ApprovalStage), g.ApprovalStage));
                i.HonorRoll = HonorRoll(mp);
                i.DaysAbsent = DaysAttendance(mp, AttendanceStatus.Absent);
                i.DaysTardy = DaysAttendance(mp, AttendanceStatus.Tardy);
                i.DaysPresent = mp.DaysIn - i.DaysAbsent - i.DaysTardy;
                rv.Add(i);
            }
            return rv;
        }
    }

}
