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
            _rnd = new Random(1116);
            var rv = new Gradebook();
            var firstNames = new List<string>(){"Dan", "Kelly", "Allegra", "Ramsey", "Max", "Lexi", "Indy"};
            var lastNames = new List<string>() { "Alpha","Beta","Gamma","Delta","Epsilon","Zeta","Eta","Theta","Iota","Kappa","Lambda","Mu","Nu","Xi","Omicron","Pi","Rho","Sigma","Tau","Upsilon","Phi","Chi","Psi","Omega"};
            var dobStart = DateTime.Now.AddYears(-14);
            var dobEnd = DateTime.Now.AddYears(-5);
            int iCourse = 1000;

            var homeroomTeachers = new Dictionary<string, string>();
            int iClassroom = 0;
            foreach (var gradeLevel in Maintenance.GradeLevelShorts)
            {
                homeroomTeachers.Add(gradeLevel,Maintenance.Users.Where(u => u.Role == RoleEnum.ClassroomInstructor).Select(t => t.Name).ToList()[iClassroom++]);
            }
            foreach (var quarter in MarkingPeriods.Singleton.Where(q => q.EndDate <= MarkingPeriod.Current.EndDate).OrderByDescending(q => q.ToString()))
            {
                foreach (var gradeLevel in Maintenance.GradeLevelShorts)
                {
                    foreach (var subject in Maintenance.Subjects)
                    {
                        var teacher = string.Empty;
                        if (CourseViewModel.ClassroomCourse(subject))
                            teacher = homeroomTeachers[gradeLevel];
                        else if (CourseViewModel.SmallGroupCourse(subject))
                            teacher = RandString(Maintenance.Users.Where(u => u.Role == RoleEnum.SmallGroupInstructor).Select(t => t.Name).ToList());
                        else if (CourseViewModel.SpecialCourse(subject))
                            teacher = RandString(Maintenance.Users.Where(u => u.Role == RoleEnum.SpecialInstructor).Select(t => t.Name).ToList());
                        else
                            teacher = null;

                        rv.Course.AddCourseRow((iCourse++).ToString(), subject, quarter.ToString(), RandString(Maintenance.Groups), teacher, Maintenance.GradeLevelNumber(gradeLevel));
                    }
                }
            }
   
            for (int iStudent = 0; iStudent < 100; iStudent++)
            {
                var name = firstNames[iStudent % 7] + " " + lastNames[iStudent % (lastNames.Count)];
                var dob = dobStart.AddDays(iStudent * (dobEnd - dobStart).TotalDays / 100);
                var enrolled = dob.AddYears(5);
                var gradeLevel = AddGradeLevel(Maintenance.GradeLevelShorts[0], (DateTime.Today.Year - enrolled.Year));
                if (gradeLevel == null)
                {
                    //handle k better
                    gradeLevel = Maintenance.GradeLevelShorts.Last();
                }

                var student = rv.Student.AddStudentRow(Gradebook.MakeStudentKey(name, dob), name, dob, iStudent + " Fake St", "Bel Air, MD 22222", gradeLevel, enrolled, DateTime.MaxValue);

                foreach (var course in rv.Course.OrderByDescending(c => c.Quarter))
                {
                    var mpk = MarkingPeriodKey.Parse(course.Quarter);
                    var mp = MarkingPeriods.Singleton.Find(mpk);
                    var diffYears = MarkingPeriodKey.Current.EndingSchoolYear - mpk.EndingSchoolYear;

                    if (course.Level == Maintenance.GradeLevelNumber(AddGradeLevel(student.GradeLevel, 0 - diffYears)))
                    {
                        if (_rnd.NextDouble() < .1)
                            rv.Attendance.AddAttendanceRow(student, mp.StartDate.AddDays(_rnd.Next(0, 30)), AttendanceStatus.Absent.ToString());
                        if (_rnd.NextDouble() < .1)
                            rv.Attendance.AddAttendanceRow(student, mp.StartDate.AddDays(_rnd.Next(0, 30)), AttendanceStatus.Tardy.ToString());

                        if (mpk.Equals(MarkingPeriodKey.Current) && _rnd.Next(0, 2) == 0)
                        {
                            rv.Grade.AddGradeRow(student, course, string.Empty, string.Empty, string.Empty, ApprovalStage.Open.ToString());
                        }
                        else
                        {
                            var stage = ApprovalStage.Office;
                            if (mpk.Equals(MarkingPeriodKey.Current))
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
                            rv.Grade.AddGradeRow(student, course, RandString(Maintenance.LetterGrades.Keys.ToList(), 3), specialGrade, Maintenance.FormatCommentFromList(RandString(Maintenance.Comments)) + " " + (iStudent + @"/100"), stage.ToString());
                        }
                    }
                }
                var quarters = MarkingPeriods.Singleton.OrderByDescending(q => q.ToString());
                bool withdraw = false;
                if (iStudent % 33 == 0)
                {
                    quarters = MarkingPeriods.Singleton.OrderBy(q => q.ToString());
                    withdraw = true;
                }

                foreach (var quarter in quarters)
                {
                    if (enrolled > quarter.EndDate)
                        continue;
                    foreach (var area in Maintenance.SelfDevelopmentAreas)
                        rv.SelfDevelopment.AddSelfDevelopmentRow(student, area, quarter.ToString(), _rnd.Next(3, 5), "Jekyll");

                    if (_rnd.NextDouble() > .9)
                    {
                        if (withdraw)
                            student.DateWithdrawn = quarter.EndDate.AddMonths(-1);
                        else
                            student.DateEnrolled = quarter.EndDate.AddMonths(-2);
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

        static public DateTime ApproximateStartDate(MarkingPeriodKey mpk)
        {
            if (mpk.Quarter == 1)
                return new DateTime(mpk.EndingSchoolYear - 1, 9, 1);
            else if (mpk.Quarter == 2)
                return new DateTime(mpk.EndingSchoolYear - 1, 11, 11);
            else if (mpk.Quarter == 3)
                return new DateTime(mpk.EndingSchoolYear, 1, 21);
            else //if (Quarter == 4)
                return new DateTime(mpk.EndingSchoolYear, 4, 2);
        }

        static public DateTime ApproximateEndDate(MarkingPeriodKey mpk)
        {
            if (mpk.Quarter == 1)
                return new DateTime(mpk.EndingSchoolYear - 1, 11, 10);
            else if (mpk.Quarter == 2)
                return new DateTime(mpk.EndingSchoolYear, 1, 20);
            else if (mpk.Quarter == 3)
                return new DateTime(mpk.EndingSchoolYear, 4, 1);
            else //if (Quarter == 4)
                return new DateTime(mpk.EndingSchoolYear, 6, 12);
        }

        internal static MarkingPeriods DemoMarkingPeriod()
        {
            var rv = new MarkingPeriods();
            for (int year = 2011; year <= 2013; year++)
            {
                for (int quarter = 1; quarter <= 4; quarter++)
                {
                    var key = new MarkingPeriodKey(quarter, year);
                    var mp = new MarkingPeriod(key, ApproximateStartDate(key), ApproximateEndDate(key), 45);
                    rv.Add(mp);
                }
            }
            return rv;
        }

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
