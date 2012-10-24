using Highlands.StaticModel;
using Highlands.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Highlands.ViewModel
{
    public class ReportCard
    {
        private StudentViewModel _student;
        
        public ReportCard(StudentViewModel student)
        {
            _student = student;
        }
        public void CreateReportCard(string outFilename, MarkingPeriodKey period)
        {
            var currentSubjects = _student.Grades.GroupBy(g => g.Subject)
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

            if (outFilename == null)
            {
                var tempDir = Path.GetTempPath();
                Directory.CreateDirectory(tempDir);
                string tempFileName = Path.Combine(tempDir, GetDefaultReportCardFilename(period) + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf");
                PDFWriter.WritePDF(tempFileName, outputProperties.ToDictionary(p => p.Key, p => p.Value));
                Process.Start(tempFileName);
            }
            else
                PDFWriter.WritePDF(outFilename, outputProperties.ToDictionary(p => p.Key, p => p.Value));
        }

        private IEnumerable<KeyValuePair<string, string>> GetSelfDevFields(MarkingPeriodKey period)
        {
            var sdAreas = Maintenance.SelfDevelopmentAreas;
            var o = _student.SelfDevelopmentScores.Where(score => MarkingPeriodKey.Parse(score.Quarter).Equals(period));
            var p = o.Select((s, i) => new[] { 
                    new KeyValuePair<string, string>("SD" + (i + 1), sdAreas[i]),
                    new KeyValuePair<string, string>("SDValue" + (i == 10 ? 12 : i + 1), s.Score.ToString())
                }).SelectMany(x => x);
            return p;
        }

        private IEnumerable<KeyValuePair<string, string>> GetReportLevelFields(MarkingPeriodKey period)
        {
            var thisQuarter = MarkingPeriods.Singleton.Find(period);
            var thisYear = new MarkingYear(thisQuarter);
            // TODO absence in Demo
            // TODONE need year to date attendance
            // TODO need to show grades for 1-4 when showing 4th quarter
            // TODO need to show not show quarters in drop down for past
            var absentThisQuarter = _student.DaysAttendance(thisQuarter, AttendanceStatus.Absent);
            int absentThisYear = _student.DaysAttendance(thisYear, AttendanceStatus.Absent);
     
            var tardyThisQuarter = _student.DaysAttendance(thisQuarter, AttendanceStatus.Tardy);
            int tardyThisYear = _student.DaysAttendance(thisYear, AttendanceStatus.Tardy);

            return new[] { 
                new KeyValuePair<string, string>(
                    "StudentAddress",
                    _student.AddressLine1 + "\n" + _student.AddressLine2),
                new KeyValuePair<string, string>(
                    "SchoolYear",
                    String.Format("{0:MMM dd, yyyy} - {1:MMM dd, yyyy}", thisYear.StartDate, thisYear.EndDate)),
                new KeyValuePair<string, string>(
                    "SchoolYearAttendance",
                    String.Format("{0} - {1}", thisYear.StartDate.Year, thisYear.EndDate.Year)),
                new KeyValuePair<string, string>(
                    "QTR1",
                    thisQuarter.DaysIn.ToString()),
                new KeyValuePair<string, string>(
                    "YR1",
                    thisYear.DaysIn.ToString()),
                new KeyValuePair<string, string>(
                    "QTR2",
                    absentThisQuarter.ToString()),
                new KeyValuePair<string, string>(
                    "YR2",
                    absentThisYear.ToString()),
                new KeyValuePair<string, string>(
                    "QTR3",
                    tardyThisQuarter.ToString()),
                new KeyValuePair<string, string>(
                    "YR3",
                    tardyThisYear.ToString()),
                new KeyValuePair<string, string>(
                    "MarkingPeriod",
                    period.QuarterString),
                new KeyValuePair<string, string>(
                    "QtrEnding",
                    thisQuarter != null ? thisQuarter.EndDate.ToShortDateString() : "N/A")
            };
        }

        public string GetDefaultReportCardFilename(MarkingPeriodKey period)
        {
            return String.Format("{0}_Q{1}_{2}_Report_Card", _student.Name, period.Quarter, period.EndingSchoolYear);
        }

        private IEnumerable<KeyValuePair<string, string>> GetPDFFields<T>(T obj)
        {
            var outputProperties = typeof(T).GetProperties()
                .Where(p => p.CanRead && p.GetCustomAttributes(typeof(PDFOutputFieldAttribute), false).Any());

            foreach (var prop in outputProperties)
            {
                var val = prop.GetValue(obj, null) ?? "";
                if (val is DateTime)
                    val = ((DateTime)val).ToShortDateString();
                yield return new KeyValuePair<string, string>(
                    prop.GetCustomAttributes(typeof(PDFOutputFieldAttribute), false).Cast<PDFOutputFieldAttribute>().First().FieldName,
                    val.ToString());
            }
        }
    }
}
