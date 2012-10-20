using Highlands.StaticModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Highlands.Model
{
    public class Book
    {
        public static Book Read()
        {
            var rv = new Book();
            rv.Students = Students.Read();
            
            return rv;
        }
        public Students Students { get; private set; }
        public List<Grade> Grades { get; private set; }
        public SelfDevelopments SelfDevelopments { get; private set; }
        public List<Course> Course { get; private set; }
    }

    public class Students : List<Student>
    {
        internal static Students Read()
        {
            var lines = Maintenance.ReadArrayFromFile("studentTable");
            if (lines == null)
                return null;
            var rv = new Students();
            foreach (var line in lines)
                rv.Add(Student.ReadLine(line));

            return rv;
        }
    }

    public class Student
    {
        public string Key 
        {
            get
            {
                return Name + DOB.ToString("yyyyMMddd");
            }
        }
        public string Name { get; set; }
        public DateTime DOB { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string GradeLevel { get; set; }
        public DateTime DateEnrolled { get; set; }
        public DateTime? DateWithdrawn { get; set; }
        public List<Grade> Grades 
        {
            get
            {
                throw new Exception();
            }
        }

        internal static Student ReadLine(string line)
        {
            var parts = line.Split(",".ToCharArray());
            var rv = new Student();
            rv.Name = parts[0];
            rv.DOB = DateTime.Parse(parts[1]);
            rv.AddressLine1 = parts[2];
            rv.AddressLine2 = parts[3];
            rv.GradeLevel = parts[4];
            rv.DateEnrolled = DateTime.Parse(parts[5]);
            rv.DateWithdrawn = DateTime.Parse(parts[6]);
            return rv;
         }
    }

    public class Grade
    {
        public string StudentKey { get; set; }
        public string CourseKey { get; set; }
        public string LetterGrade { get; set; }
        public string SpecialGrade { get; set; }
        public string Comment { get; set; }
        public ApprovalStage Stage { get; set; }
    }

    public class SelfDevelopments : List<SelfDevelopment>
    {
    }

    public class SelfDevelopment
    {
        public string StudentKey { get; set; }
        public string Area { get; set; }
        public MarkingPeriod Quarter { get; set; }
        public string Score { get; set; }
        public User Teacher { get; set; }
    }

    public class Course
    {
        public string Key
        {
            get
            {
                return Subject + Quarter.ToString() + Group + Level + Teacher;
            }
        }
        public string Subject { get; set; }
        public MarkingPeriod Quarter { get; set; }
        public string Group { get; set; }
        public string Teacher { get; set; }
        public int Level { get; set; }
    }

}
