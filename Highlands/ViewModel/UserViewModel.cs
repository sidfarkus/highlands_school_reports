using Highlands.Model;
using Highlands.StaticModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace Highlands.ViewModel
{
    public enum AttendanceStatus
    {
        None,
        Absent,
        Tardy,
        OnTime
    }

    class UserViewModel
    {
        public enum ValidationEnum
        {
            None,
            Passed,
            RequirePasswordReset,
            FailedUserNotFound,
            FailedPassword,
            PasswordRequired,
            PasswordsDoNotMatch,
            PasswordChanged
        }
        internal static ValidationEnum ValidateUser(string userName, string unhashedPassword)
        {
            var user = Maintenance.Users.SingleOrDefault(u => u.Name.ToUpper() == userName.ToUpper());
            if (user == null)
                return ValidationEnum.FailedUserNotFound;
            if (string.IsNullOrWhiteSpace(user.HashedPassword))
                return ValidationEnum.RequirePasswordReset;
            var hashedPassword = User.Hash(user.Name, unhashedPassword);
            if (hashedPassword != user.HashedPassword)
                return ValidationEnum.FailedPassword;
            Gradebook.CurrentUser = user;
            return ValidationEnum.Passed;
        }

        User _user;
        public UserViewModel(User user)
        {
            _user = user;
        }

        public bool IsValid
        {
            get
            {
                return (_user != null);
            }
        }

        public static UserViewModel CurrentUser
        {
            get
            {
                return new UserViewModel(Gradebook.CurrentUser);
            }
        }

        public string Name
        {
            get
            {
                if (_user == null)
                    return "-";
                return _user.Name;
            }
        }

        public string EmailAddress
        {
            get
            {
                if (_user == null)
                    return "-";
                return _user.EmailAddress;
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public bool CanEditStudentInfo
        {
            get
            {
                if (_user == null)
                    return false;
                return _user.Role == RoleEnum.SuperUser || _user.Role == RoleEnum.Admin;
            }
        }

        public bool CanEditAttendance
        {
            get
            {
                if (_user == null)
                    return false;
                return _user.Role == RoleEnum.SuperUser ||
                    _user.Role == RoleEnum.ClassroomInstructor ||
                    _user.Role == RoleEnum.Nurse ||
                    _user.Role == RoleEnum.Admin;
            }
        }

        public bool CanExportReportCards
        {
            get
            {
                if (_user == null)
                    return false;
                return _user.Role == RoleEnum.Admin ||
                    _user.Role == RoleEnum.SuperUser ||
                    _user.Role == RoleEnum.ClassroomInstructor;
            }
        }

        public RightsEnum CanEdit(GradeViewModel grade)
        {
            if (grade.ApprovalStage == ApprovalStage.Office)
                return RightsEnum.StageError;

            if (HasOfficeRights)
                return RightsEnum.Success;

            if (grade.ApprovalStage == ApprovalStage.Classroom)
                return RightsEnum.UserError;

            if (HasClassroomRights)
                return RightsEnum.Success;

            if (grade.ApprovalStage == ApprovalStage.Instructor)
                return RightsEnum.UserError;
    
            if (Name == grade.Teacher)
                return RightsEnum.Success;

            return RightsEnum.UserError;
        }

        public bool HasOfficeRights
        {
            get
            {
                if (_user.Role == RoleEnum.SuperUser || _user.Role == RoleEnum.Admin)
                    return true;
                return false;
            }
        }
        public bool HasClassroomRights
        {
            get
            {
                if (_user == null)
                    return false;
                if (_user.Role == RoleEnum.SuperUser || _user.Role == RoleEnum.Admin || _user.Role == RoleEnum.ClassroomInstructor)
                    return true;
                return false;
            }
        }

        public bool CanEditSelfDevelopmentScores
        {
            get
            {
                if (_user == null)
                    return false;
                return _user.Role == RoleEnum.SuperUser || _user.Role == RoleEnum.Admin || _user.Role == RoleEnum.ClassroomInstructor;
            }
        }

        public bool HasStudents
        {
            get
            {
                if (_user == null)
                    return false;

                return _user.IsTeacher;
            }
        }

        public static IEnumerable<AttendanceStatus> AttendanceStatuses
        {
            get
            {
                yield return AttendanceStatus.Absent;
                yield return AttendanceStatus.Tardy;
                yield return AttendanceStatus.OnTime;
            }
        }

        internal static void Reset()
        {
            Gradebook.CurrentUser = null;
        }

        internal static ValidationEnum SetPassword(string userName, string unhashedPassword1, string unhashedPassword2)
        {
            var user = Maintenance.Users.SingleOrDefault(u => u.Name.ToUpper() == userName.ToUpper());
            if (user == null)
                return ValidationEnum.FailedUserNotFound;
            if (string.IsNullOrWhiteSpace(unhashedPassword1))
                return ValidationEnum.PasswordRequired;
            if (unhashedPassword1 != unhashedPassword2)
                return ValidationEnum.PasswordsDoNotMatch;
            user.HashedPassword = User.Hash(user.Name, unhashedPassword1);
            var change = new Change(user, "password", "old", "new");
            ChangeLog.LogDiff(change);
            Maintenance.SaveUsers();
            ValidateUser(userName, unhashedPassword1);
            return ValidationEnum.PasswordChanged;
        }

        internal RightsEnum CanApprove(GradeViewModel grade)
        {
            if (!grade.IsReadyToApprove)
                return RightsEnum.GradeError;
            if (grade.ApprovalStage == ApprovalStage.Open && CanInstructorApprove(grade.Teacher))
                return RightsEnum.Success;
            else if (grade.ApprovalStage == ApprovalStage.Instructor && HasClassroomRights)
                return RightsEnum.Success;
            else if (grade.ApprovalStage == ApprovalStage.Classroom && HasOfficeRights)
                return RightsEnum.Success;

            return RightsEnum.StageError;
        }

        private bool CanInstructorApprove(string teacherName)
        {
            if (_user.Role == RoleEnum.SuperUser)
                return true;
            if (_user.Name == teacherName)
                return true;
            return false;
        }

        internal RightsEnum CanUnApprove(GradeViewModel grade)
        {
            if (grade.ApprovalStage == ApprovalStage.Office)
                return RightsEnum.StageError;
            if (grade.ApprovalStage == ApprovalStage.None || grade.ApprovalStage == ApprovalStage.Open)
                return RightsEnum.StageError;
            if (grade.ApprovalStage == ApprovalStage.Classroom && HasOfficeRights)
                return RightsEnum.Success;
            if (grade.ApprovalStage == ApprovalStage.Instructor && HasClassroomRights)
                return RightsEnum.Success;

            return RightsEnum.UserError;
        }

        public bool CanViewGrades
        {
            get
            {
                return (_user.Role != RoleEnum.Nurse);
            }
        }

        public bool CanImportExport
        {
            get
            {
                return (_user.Role == RoleEnum.SuperUser || _user.Role == RoleEnum.Admin);
            }
        }


        public bool CanViewHonorRoll
        {
            get
            {
                return (_user.Role != RoleEnum.Nurse);
            }
        }
    }
    public enum RightsEnum
    {
        None,
        Success,
        StageError,
        UserError,
        GradeError
    }
}

