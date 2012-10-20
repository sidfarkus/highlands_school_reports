using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
        SmallGroupInstructor,
        SpecialInstructor
    }

    public class User
    {
        static public IList<User> ParseUserStrings(IList<string> userStrings)
        {
            try
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
            catch (Exception)
            {
                return null;
            }
        }

        internal static IList<string> ComposeUserStrings(IList<User> users)
        {
            var rv = new List<string>();
            foreach (var user in users)
            {
                rv.Add(user.Name + "," + user.Role + "," + user.EmailAddress + "," + user.HashedPassword);
            }
            return rv;
        }

        /// <summary>
        /// For username and password
        /// </summary>
        /// <param name="unhashed1">gets uppered</param>
        /// <param name="unhashed2"></param>
        /// <returns></returns>
        public static string Hash(string unhashed1, string unhashed2)
        {
            //create our SHA1 provider
            var sha = new SHA1CryptoServiceProvider();
            var unhashed = unhashed1.ToUpper() + unhashed2;
            string hashedValue = string.Empty;
            //hash the data
            byte[] hashedData = sha.ComputeHash(Encoding.Unicode.GetBytes(unhashed));

            //loop through each byte in the byte array
            foreach (byte b in hashedData)
            {
                //convert each byte and append
                hashedValue += String.Format("{0,2:X2}", b);
            }

            //return the hashed value
            return hashedValue;
        }
        public User(string userInfo)
        {
            var parts = userInfo.Split(",".ToCharArray());
            Name = parts[0];
            Role = (RoleEnum)Enum.Parse(typeof(RoleEnum), parts[1]);
            EmailAddress = parts[2];
            HashedPassword = parts[3];
        }

        public User(string name, RoleEnum role, string emailAddress, string hashedPassword)
        {
            Name = name;
            Role = role;
            EmailAddress = emailAddress;
            HashedPassword = hashedPassword;
        }

        public bool IsTeacher
        {
            get
            {
                return (Role == RoleEnum.ClassroomInstructor || Role == RoleEnum.SmallGroupInstructor || Role == RoleEnum.SpecialInstructor);
            }
        }
        public override string ToString()
        {
            return Name + "(" + Role + ")";
        }
        
        public string Name { get; set; }
        public RoleEnum Role { get; set; }
        public string EmailAddress { get; set; }
        public string HashedPassword { get; set; }
    }

}
