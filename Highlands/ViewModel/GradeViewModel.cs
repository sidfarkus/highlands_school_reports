using Highlands.Model;
using Highlands.StaticModel;
using System.Collections.Generic;

namespace Highlands.ViewModel
{
    public class GradeViewModel
    {
        Gradebook.GradeRow _gradeRow;
        public GradeViewModel(Gradebook.GradeRow gradeRow)
        {
            _gradeRow = gradeRow;
        }
        public string Subject
        {
            get
            {
                return _gradeRow.CourseRow.SubjectName;
            }
        }
        public string Level
        {
            get
            {
                return _gradeRow.CourseRow.Level;
            }
        }
        public string Quarter
        {
            get
            {
                return _gradeRow.CourseRow.Quarter;
            }
        }
        public string Group
        {
            get
            {
                return _gradeRow.CourseRow.Group;
            }
        }
        public string LetterGrade
        {
            get
            {
                return _gradeRow.LetterGrade;
            }
            set
            {
                _gradeRow.LetterGrade = value;
            }
        }
        public string SpecialGrade
        {
            get
            {
                return _gradeRow.SpecialGrade;
            }
            set
            {
                _gradeRow.SpecialGrade = value;
            }
        }
        public string Comment
        {
            get
            {
                return _gradeRow.Comment;
            }
            set
            {
                _gradeRow.Comment = value;
            }
        }
        public string Teacher
        {
            get
            {
                return _gradeRow.CourseRow.Teacher;
            }
        }

        public bool IsCurrentForPeriod(MarkingPeriod period)
        {
            return MarkingPeriod.Parse(Quarter).Equals(period);
        }

        public bool ShouldShowOnReportCard(MarkingPeriod period)
        {
            var thisPeriod = MarkingPeriod.Parse(Quarter);
            return thisPeriod.SchoolYear.Item1 == period.SchoolYear.Item1;
        }

        internal void Save(System.Collections.Generic.List<StaticModel.Change> diffs)
        {
            ChangeLog.LogDiffs(diffs);
        }

        internal void Approve()
        {
            var change = new Change(this, "Approval", false.ToString(), true.ToString());
            ChangeLog.LogDiffs(new List<Change>() { change });
            Approval = true;
        }

        public IEnumerable<KeyValuePair<string, string>> GetGradeReportFields(MarkingPeriod period, int rowIndex)
        {
            yield return new KeyValuePair<string, string>(
                 string.Format("Mkg" + MarkingPeriod.Parse(Quarter).Quarter + "-Row{0}", rowIndex),
                 LetterGrade + (!string.IsNullOrEmpty(SpecialGrade) ? "\n" + SpecialGrade : ""));

            if (IsCurrentForPeriod(period))
            {
                yield return new KeyValuePair<string, string>(
                    "SubjectRow" + rowIndex,
                    Subject);
                yield return new KeyValuePair<string, string>(
                    "CommentRow" + rowIndex,
                    Comment);
                yield return new KeyValuePair<string, string>(
                    "TeacherRow" + rowIndex,
                    Teacher);
            }
        }

        public override string ToString()
        {
            return new CourseViewModel(_gradeRow.CourseRow).ToString();
        }

        public bool Approval
        {
            get
            {
                return _gradeRow.Approval;
            }
            set
            {
                _gradeRow.Approval = value;
            }
        }
    }
}
