using Highlands.Model;
using Highlands.StaticModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Highlands.ViewModel
{
    class UserViewModel
    {
        internal static bool ValidateTeacher(string userName, string password)
        {
            var user = Maintenance.Users.SingleOrDefault(u => u.Name.ToUpper() == userName.ToUpper());
            if (user == null)
                return false;

            Gradebook.CurrentUser = user;
            return true;
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
    }
}
