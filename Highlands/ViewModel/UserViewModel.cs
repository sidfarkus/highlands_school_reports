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
        internal static ValidationEnum ValidateTeacher(string userName, string unhashedPassword)
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
                return _user.Role == RoleEnum.SuperUser || _user.Role == RoleEnum.ClassroomInstructor;
            }
        }

        public bool CanEdit(string course)
        {
            if (CourseViewModel.ClassroomCourse(course) && CanEditClassroomGrades)
                return true;
            if (CourseViewModel.SpecialCourse(course) && CanEditSpecialClassGrades)
                return true;
            if (CourseViewModel.NonSpecialCourse(course) && CanEditNonSpecialClassGrades)
                return true;
            return false;
        }
        public bool CanApprove(string course)
        {
            if (_user.Role == RoleEnum.SuperUser || _user.Role == RoleEnum.Admin)
                return true;
            return false;
        }
        public bool CanEditSpecialClassGrades
        {
            get
            {
                if (_user == null)
                    return false;
                return _user.Role == RoleEnum.SuperUser || _user.Role == RoleEnum.Admin || _user.Role == RoleEnum.SpecialInstructor || _user.Role == RoleEnum.ClassroomInstructor;
            }
        }

        public bool CanEditClassroomGrades
        {
            get
            {
                if (_user == null)
                    return false;
                return _user.Role == RoleEnum.SuperUser || _user.Role == RoleEnum.Admin || _user.Role == RoleEnum.ClassroomInstructor;
            }
        }

        public bool CanEditNonSpecialClassGrades
        {
            get
            {
                if (_user == null)
                    return false;
                return _user.Role == RoleEnum.SuperUser || _user.Role == RoleEnum.Admin || _user.Role == RoleEnum.ClassroomInstructor || _user.Role == RoleEnum.NonSpecialInstructor;
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
            return ValidationEnum.PasswordChanged;
        }
    }
}
