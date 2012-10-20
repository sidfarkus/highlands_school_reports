using Highlands.Model;
using Highlands.StaticModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Highlands.ViewModel
{
    static public class DemoFactory
    {
        #region Public  Methods
        static public Gradebook CreateDemo()
        {
            var rv = new Gradebook();
            var firstNames = new List<string>(){"Dan", "Kelly", "Allegra", "Ramsey", "Max", "Lexi", "Indy"};
            var lastNames = new List<string>() { "Alpha","Beta","Gamma","Delta","Epsilon","Zeta","Eta","Theta","Iota","Kappa","Lambda","Mu","Nu","Xi","Omicron","Pi","Rho","Sigma","Tau","Upsilon","Phi","Chi","Psi","Omega"};
            var dobStart = DateTime.Now.AddYears(-14);
            var dobEnd = DateTime.Now.AddYears(-5);
            int iCourse = 1000; 
            foreach (var quarter in MarkingPeriod.MarkingPeriods.OrderByDescending(q => q.ToString()))
            {
                foreach(var gradeLevel in Maintenance.GradeLevelShorts)
                    foreach (var subject in Maintenance.Subjects)
                    {
                        var teacher = string.Empty;
                        if (CourseViewModel.ClassroomCourse(subject))
                            teacher = RandString(Maintenance.Users.Where(u => u.Role == RoleEnum.ClassroomInstructor).Select(t => t.Name).ToList());
                        else if (CourseViewModel.NonSpecialCourse(subject))
                            teacher = RandString(Maintenance.Users.Where(u => u.Role == RoleEnum.NonSpecialInstructor).Select(t => t.Name).ToList());
                        else if (CourseViewModel.SpecialCourse(subject))
                            teacher = RandString(Maintenance.Users.Where(u => u.Role == RoleEnum.SpecialInstructor).Select(t => t.Name).ToList());
                        else
                            teacher = null;
                        rv.Course.AddCourseRow((iCourse++).ToString(), subject, quarter.ToString(), RandString(Maintenance.Groups), teacher, Maintenance.GradeLevelNumber(gradeLevel));
                    }
            }
   
            for (int i = 0; i < 100; i++)
            {
                var name = firstNames[i % 7] + " " + lastNames[i % (lastNames.Count)];
                var dob = dobStart.AddDays(i * (dobEnd - dobStart).TotalDays / 100);
                var enrolled = dob.AddYears(5);
                var gradeLevel = AddGradeLevel(Maintenance.GradeLevelShorts[0], (DateTime.Today.Year - enrolled.Year));
                if (gradeLevel == null)
                {
                    //handle k better
                    gradeLevel = Maintenance.GradeLevelShorts.Last();
                }

                var student = rv.Student.AddStudentRow("STU" + i.ToString("0000"), name, dob, i + " Fake St, Bel Air, MD 22222", gradeLevel, enrolled, DateTime.MaxValue);


                foreach (var course in rv.Course.OrderByDescending(c => c.Quarter))
                {
                    var mp = MarkingPeriod.Parse(course.Quarter);
                    var diffYears = MarkingPeriod.Current.EndingSchoolYear - mp.EndingSchoolYear;

                    if (course.Level == Maintenance.GradeLevelNumber(AddGradeLevel(student.GradeLevel, 0 - diffYears)))
                    {

                        if (mp.Equals(MarkingPeriod.Current) && _rnd.Next(0, 2) == 0)
                        {
                            rv.Grade.AddGradeRow(student, course, string.Empty, string.Empty, string.Empty, ApprovalStage.Open.ToString());
                        }
                        else
                        {
                            var stage = ApprovalStage.Office;
                            if (mp.Equals(MarkingPeriod.Current))
                            {
                                if (_rnd.Next(0, 2) == 0)
                                    stage = ApprovalStage.Instructor;
                                else
                                    stage = ApprovalStage.Classroom;
                            }

                            var specialGrade = string.Empty;
                            if (CourseViewModel.HasSpecialGrade(course.SubjectName))
                            {
                                if (course.SubjectName == "Reading")
                                    specialGrade = (int.Parse(course.Level) + _rnd.Next(0, 5)).ToString() + "-" + _rnd.Next(1, 10);
                                else if (course.SubjectName == "Math")
                                    specialGrade = (int.Parse(course.Level) + _rnd.Next(0, 5)).ToString() + "(" + _rnd.Next(1, 2).ToString() + ")";
                                else if (course.SubjectName == "Written Expression")
                                    specialGrade = "Paragraph";
                            }
                            rv.Grade.AddGradeRow(student, course, RandString(Maintenance.LetterGrades, 3), specialGrade, Maintenance.FormatCommentFromList(RandString(Maintenance.Comments)) + " " + (i + @"/100"), stage.ToString());
                        }
                    }
                }
                var quarters = MarkingPeriod.MarkingPeriods.OrderByDescending(q => q.ToString());
                bool withdraw = false;
                if (i % 33 == 0)
                {
                    quarters = MarkingPeriod.MarkingPeriods.OrderBy(q => q.ToString());
                    withdraw = true;
                }

                foreach (var quarter in quarters)
                {
                    if (enrolled > quarter.ApproximateEndDate)
                        continue;
                    foreach (var area in Maintenance.SelfDevelopmentAreas)
                        rv.SelfDevelopment.AddSelfDevelopmentRow(student, area, quarter.ToString(), _rnd.Next(3, 5), "Jekyll");

                    if (_rnd.NextDouble() > .9)
                    {
                        if (withdraw)
                            student.DateWithdrawn = quarter.ApproximateEndDate.AddMonths(-1);
                        else
                            student.DateEnrolled = quarter.ApproximateEndDate.AddMonths(-2);
                        //if (student.DateEnrolled > DateTime.Today)
                        //    student.DateEnrolled = DateTime.Today.AddDays(-1);
                        break;
                    }
                }
            }
            return rv;
        }

        #endregion

        #region Private Methods
        
        static Random _rnd = new Random();
        private static string RandString(IList<string> strs)
        {
            return strs[_rnd.Next(strs.Count())];
        }

        private static string RandString(IList<string> strs, int retries)
        {
            var val = strs.Count();
            for (int i = 0; i < retries; i++)
            {
                int n = _rnd.Next(strs.Count());
                if (n < val)
                    val = n;
            }
            return strs[val];
        }

        private static string AddGradeLevel(string gradeLevel, int offset)
        {
            if (offset == 0)
                return gradeLevel;
            var n = Maintenance.GradeLevelShorts.IndexOf(gradeLevel);
            n += offset;
            if (n < 0)
                return null;
            if (n >= Maintenance.GradeLevelShorts.Count())
                return null;
            return Maintenance.GradeLevelShorts[n];
        }

        #endregion
    }
}
