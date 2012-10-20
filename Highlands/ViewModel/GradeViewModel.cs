using Highlands.Model;
using Highlands.StaticModel;
using System;
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

        internal void Save(System.Collections.Generic.List<StaticModel.Change> diffs)
        {
            ChangeLog.LogDiffs(diffs);
        }

        internal void Approve()
        {
            var change = new Change(this, "Approval", false.ToString(), true.ToString());
            ChangeLog.LogDiffs(new List<Change>() { change });
            ApprovalStage = (ApprovalStage) ((int)ApprovalStage + 1);
            if (UserViewModel.CurrentUser.HasClassroomRights && ApprovalStage == Model.ApprovalStage.Instructor)
                ApprovalStage = Model.ApprovalStage.Classroom;
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

        public bool IsReadyToApprove
        {
            get
            {
                return !string.IsNullOrWhiteSpace(LetterGrade) || !string.IsNullOrWhiteSpace(Comment); 
            }
        }
    }
}
