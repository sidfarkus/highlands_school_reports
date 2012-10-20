using Highlands.Model;
using Highlands.StaticModel;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public bool IsCurrentForPeriod(MarkingPeriodKey period)
        {
            return MarkingPeriodKey.Parse(Quarter).Equals(period);
        }

        public bool ShouldShowOnReportCard(MarkingPeriodKey period)
        {
            var thisPeriod = MarkingPeriodKey.Parse(Quarter);
            return thisPeriod.EndingSchoolYear == period.EndingSchoolYear;
        }

        internal void Save(System.Collections.Generic.List<StaticModel.Change> diffs)
        {
            ChangeLog.LogDiffs(diffs);
        }

        internal void Approve()
        {
            var oldStage = ApprovalStage;
            var newStage = (ApprovalStage)((int)ApprovalStage + 1);
            if (UserViewModel.CurrentUser.HasClassroomRights && newStage == ApprovalStage.Instructor)
                newStage = ApprovalStage.Classroom;
            var change = new Change(this, "Approval", oldStage.ToString(), newStage.ToString());
            ChangeLog.LogDiff(change);
            ApprovalStage = newStage; 
        }

        public IEnumerable<KeyValuePair<string, string>> GetGradeReportFields(MarkingPeriodKey period, int rowIndex)
        {
            yield return new KeyValuePair<string, string>(
                 string.Format(MarkingPeriodKey.Parse(Quarter).Quarter + "Row{0}", rowIndex),
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

        public ApprovalStage ApprovalStage
        {
            get
            {
                return (ApprovalStage) Enum.Parse(typeof(ApprovalStage), _gradeRow.ApprovalStage);
            }
            set
            {
                _gradeRow.ApprovalStage = value.ToString();
            }
        }

        internal bool IsReadyToApprove
        {
            get
            {
                return !string.IsNullOrWhiteSpace(LetterGrade) || !string.IsNullOrWhiteSpace(Comment); 
            }
        }

        internal void UnApprove()
        {
            var oldStage = ApprovalStage;
            var newStage = (ApprovalStage)((int)ApprovalStage - 1);
            var change = new Change(this, "Unapproval", oldStage.ToString(), newStage.ToString());
            ChangeLog.LogDiff(change);
            ApprovalStage = newStage;
            
        }

        internal UserViewModel TeacherVm 
        {
            get
            {
                return new UserViewModel(Maintenance.Teachers.SingleOrDefault(t => t.Name == Teacher));
            }
        }

        internal string ForMail()
        {
            return
                "Student: " + _gradeRow.StudentRow.Name + Environment.NewLine +
                "Course: " + Subject + Environment.NewLine +
                "Quarter: " + Quarter + Environment.NewLine + 
                "Stage: " + ApprovalStage + Environment.NewLine;
        }

        internal string StudentName
        {
            get
            {
                return _gradeRow.StudentRow.Name;
            }
        }

        public IEnumerable<string> ForGrid
        {
            get
            {
                var rv = new List<string>();
                rv.Add(_gradeRow.StudentRow.Name);
                rv.Add(_gradeRow.LetterGrade);
                rv.Add(_gradeRow.SpecialGrade);
                rv.Add(_gradeRow.Comment);
                rv.Add(_gradeRow.ApprovalStage);
                return rv;
            }
        }
    }
}
