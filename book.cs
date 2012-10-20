using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Highlands.Model
{
    class book
    {
        List<student> students = new List<student>();//create student classs
        List<grade> grades = new List<grade>();
        List<course> courses = new List<course>();
        List<selfdevscore> selfdevscores = new List<selfdevscore>();

        public static book Read()
        {
            var rv = new book();
            try
            {
                rv.ReadStudents("students.csv");
                rv.ReadGrades("grades.csv");
                rv.ReadCourses("courses.csv");
                rv.ReadSelfDevScores("selfdevscore.csv");
            }
            catch (System.Exception)
            {
                return null;
            }
            return rv;
        }

        public void ReadStudents(string file)
        {
            //fill the lists
            IEnumerable<string> lines = File.ReadLines(file);
            char[] separator = new Char[] { ','};
            foreach (string line in lines)
            {
                string[] fields = line.Split(separator);
                student astudent = new student();
                astudent.Name = fields[0];
                astudent.GradeLevel = fields[1];
              //  astudent.DOB = DateTime(fields[2]);
                astudent.DateWithdrawn = fields[3];
                astudent.DateEnrolled = fields[4];
                astudent.Address = fields[5];
            }

        }

        public void ReadGrades(string file)
        {
            //fill the lists
            IEnumerable<string> lines = File.ReadLines(file);
            char[] separator = new Char[] { ',' };
            foreach (string line in lines)
            {
                string[] fields = line.Split(separator);
                grade agrade = new grade();
                agrade.Approval = fields[0];
                agrade.Comment = fields[1];                
                agrade.CourseKey = fields[2];
                agrade.LetterGrade = fields[3];
                agrade.SpecialGrade = fields[4];
                agrade.StudentKey = fields[5];                
            }

        }

        public void ReadCourses(string file)
        {
            //fill the lists
            IEnumerable<string> lines = File.ReadLines(file);
            char[] separator = new Char[] { ',' };
            foreach (string line in lines)
            {
                string[] fields = line.Split(separator);
                course acourse = new course();
                acourse.CourseKey = fields[0];
                acourse.Group = fields[1];                
                acourse.Level = fields[2];
                acourse.SubjectName = fields[3];
                acourse.Teacher = fields[4];
                acourse.Quarter = fields[5];
            }

        }

        public void ReadSelfDevScores(string file)
        {
            //fill the lists
            IEnumerable<string> lines = File.ReadLines(file);
            char[] separator = new Char[] { ',' };
            foreach (string line in lines)
            {
                string[] fields = line.Split(separator);
                selfdevscore ascore = new selfdevscore();
                ascore.Area = fields[0];
                ascore.Quarter = fields[1];                
                ascore.Score = fields[2];
                ascore.StudentKey = fields[3];
                ascore.Teacher = fields[4];
            }

        }
        
    }
}
