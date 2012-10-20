using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Highlands.StaticModel
{
    public enum RoleEnum
    {
        None,
        SuperUser,
        Admin,
        Nurse,
        ClassroomInstructor,
        NonSpecialInstructor,
        SpecialInstructor
    }

    public class User
    {
        static public IList<User> ParseUserStrings(IList<string> userStrings)
        {
            if (userStrings == null)
                return null;
            var rv = new List<User>();
            foreach (var userString in userStrings)
            {
                rv.Add(new User(userString));
            }
            return rv;
        }

        internal static IList<string> ComposeUserStrings(IList<User> users)
        {
            var rv = new List<string>();
            foreach (var user in users)
            {
                rv.Add(user.Name + "," + user.Role);
            }
            return rv;
        }

        public User(string userInfo)
        {
            var parts = userInfo.Split(",".ToCharArray());
            Name = parts[0];
            Role = (RoleEnum)Enum.Parse(typeof(RoleEnum), parts[1]);
        }

        public User(string name, RoleEnum role)
        {
            // TODO: Complete member initialization
            Name = name;
            Role = role;
        }

        public bool IsTeacher
        {
            get
            {
                return (Role == RoleEnum.ClassroomInstructor || Role == RoleEnum.NonSpecialInstructor || Role == RoleEnum.SpecialInstructor);
            }
        }
        public override string ToString()
        {
            return Name + "(" + Role + ")";
        }
        public string Name { get; set; }
        public RoleEnum Role { get; set; }

    }

}
