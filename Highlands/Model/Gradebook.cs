using Highlands.StaticModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Highlands.Model
{
    public partial class Gradebook 
    {
        /*
        public void CloneQuarter(MarkingPeriodKey last, MarkingPeriodKey next)
        {
            if (false == MarkingPeriods.Singleton.Any(m => m.Key.Equals(next)))
                MarkingPeriods.Singleton.AddMarkingPeriod(new MarkingPeriod(next, next.ApproximateStartDate, next.ApproximateEndDate, 45));

            var courses = Course.Where(c => c.Quarter == last.ToString());
            foreach (var course in courses)
                Course.AddCourseRow(course.CourseKey + course.Quarter, course.SubjectName, course.Quarter, course.Group, course.Teacher, course.Level);
            
            Save();
        }*/

        public void ImportClassLists(MarkingPeriodKey mpk, List<string> lines)
        {
            foreach (var line in lines)
            {
                var parts = line.Split(",".ToCharArray());
                var studentName = parts[0];
                var student = Student.SingleOrDefault(s => s.Name == studentName);
                if (student == null)
                    continue;
                var courseName = parts[1];
                var quarter = parts[2];
                var teacher = parts[3];
                var group  = parts[4];
                var level = parts[5];
                
            }

            Save();
        }

        public IEnumerable<string> ImportStudents(IEnumerable<string> lines)
        {
            var rv = new List<string>();
            foreach (var line in lines)
            {
                try
                {
                    if (line.StartsWith("Key"))
                        continue;
                    var parts = SplitCsv(line);

                    int i = 0;
                    var key = parts[i++];
                    var name = parts[i++];
                    var dob = DateTime.Parse(parts[i++]);
                    var address1 = parts[i++];
                    var address2 = parts[i++];
                    var gradeLevel = parts[i++];
                    var enrolled = DateTime.Parse(parts[i++]);
                    DateTime withdrawn = ParseNullableDateTime(parts[i++]);

                    var student = Student.FindByKey(key);
                    if (student == null)
                    {
                        student = Student.SingleOrDefault(s => s.Name == name && s.DOB == dob);
                    }
                    if (student == null)
                    {
                        Student.AddStudentRow(MakeStudentKey(name, dob), name, dob, address1, address2, gradeLevel, enrolled, withdrawn);
                        rv.Add("Added student: " + name);
                    }
                    else
                    {
                        bool updated = false;
                        if (!CompareDate(student.DOB, dob))
                        {
                            student.DOB = dob;
                            updated = true;
                        }
                        if (student.AddressLine1 != address1)
                        {
                            student.AddressLine1 = address1;
                            updated = true;
                        }
                        if (student.AddressLine2 != address2)
                        {
                            student.AddressLine2 = address2;
                            updated = true;
                        }
                        if (student.GradeLevel != gradeLevel)
                        {
                            student.GradeLevel = gradeLevel;
                            updated = true;
                        }
                        if (!CompareDate(student.DateEnrolled, enrolled))
                        {
                            student.DateEnrolled = enrolled;
                            updated = true;
                        }
                        if (!CompareDate(student.DateWithdrawn, withdrawn))
                        {
                            student.DateWithdrawn = withdrawn;
                            updated = true;
                        }
                        if (updated)
                            rv.Add("Modified: " + name);
                    }
                }
                catch (Exception)
                {
                    rv.Add("ERROR on student: " + line);
                }
            }

            return rv;
        }
        bool CompareDate(DateTime d1, DateTime d2)
        {
            return (d1.Year == d2.Year && d1.DayOfYear == d2.DayOfYear);
        }

        public static string MakeStudentKey(string name, DateTime dob)
        {
            return name.Replace(" ", "") +dob.ToString("yyyyMMdd");
        }
        private IList<string> SplitCsv(string line)
        {
            var parts = line.Split(",".ToCharArray());
            var rv = new List<string>();
            string superpart = null;
            foreach (var part in parts)
            {
               
                if (part.StartsWith("\""))
                {
                    if (part.EndsWith("\""))
                        rv.Add(part.Trim("\"".ToCharArray()));
                    else
                        superpart = part.Trim("\"".ToCharArray());
                }
                else if (superpart != null)
                {
                    superpart += "," + part.Trim("\"".ToCharArray());
                    if (part.EndsWith("\""))
                    {
                        rv.Add(superpart);
                        superpart = null;
                    }
                }
                else
                    rv.Add(part);
            }
            return rv;
        }

        public List<string> ExportStudents()
        {
            var rv = new List<string>();
            rv.Add("Key,Name,DOB,AddressLine1,AddressLine2,Grade,DateEnrolled,DateWithdrawn");
            foreach (Gradebook.StudentRow student in Student.Rows)
            {
                var withdrawn = "";
                if (student.DateWithdrawn != DateTime.MaxValue)
                    withdrawn = student.DateWithdrawn.ToShortDateString();
                rv.Add(student.Key + "," + student.Name + "," + student.DOB.ToShortDateString() + ",\"" + student.AddressLine1 + "\",\"" + student.AddressLine2 + "\"," + student.GradeLevel + "," + student.DateEnrolled.ToShortDateString() + "," + withdrawn);
            }
            return rv;
        }

        DateTime ParseNullableDateTime(string str)
        {
            DateTime rv = DateTime.MaxValue;
            if (!string.IsNullOrWhiteSpace(str))
                rv = DateTime.Parse(str);
            return rv;
        }
        public static Gradebook Read()
        {
            var rv = new Gradebook();
            try
            {
                rv.ReadXml("gradebook.xml");
                rv.AcceptChanges();
            }
            catch (System.Exception)
            {
                return null;
            }
            return rv;
        }

        
        
        public void Save()
        {
             try
            {
                if (!HasChanges())
                    return;
                var other = new Gradebook();
                other.ReadXml("gradebook.xml");
                if (other.Config.Guid != Config.Guid)
                {
                    if (CheckMerge(other))
                        return;
                }
           }
            catch (Exception exc)
            {
                
            }
            var guid = Guid.NewGuid();
            Config.Guid = guid.ToString();
            WriteXml("gradebook.xml");
            Maintenance.BackupFile("gradebook.xml", CurrentUserName);
            /*Student.WriteXml("studentTable.xml");
            Grade.WriteXml("gradeTable.xml");
            Course.WriteXml("courseTable.xml");
            */
        }

        private bool CheckMerge(Gradebook other)
        {
            // handles may overwrite other changes on same lines
            var changes = GetChanges();
            other.Merge(changes, false);
            other.Save();
            Read();
            
            return true;
        }

        static public User CurrentUser { get; set; }
        static public string CurrentUserName
        {
            get
            {
                string rv = "-";
                if (CurrentUser != null)
                    rv = CurrentUser.Name;
                return rv;
            }
        }
        partial class ConfigDataTable
        {
            public string Guid
            {
                get
                {
                    if (this.Count() < 1)
                        return null;
                    return this[0].Guid;
                }
                set
                {
                   if (this.Count() < 1)
                        AddConfigRow(value, DateTime.Now.ToString(), CurrentUserName);
                    else
                    {
                        this[0].Guid = value;
                        this[0].LastModified = DateTime.Now.ToString();
                        this[0].UserModified = CurrentUserName;
                    }
                }
            }
            public string LastModified
            {
                get
                {
                    if (this.Count() < 1)
                        return null;
                    return this[0].LastModified;
                }
            }
            public string UserModified
            {
                get
                {
                    if (this.Count() < 1)
                        return null;
                    return this[0].UserModified;
                }
            }
        }
        partial class CourseRow
        {
            public MarkingPeriod MarkingPeriod 
            {
                get
                {
                    var key = MarkingPeriodKey.Parse(Quarter);
                    return MarkingPeriods.Singleton.Find(key);
                }
            }
        }

        partial class StudentRow
        {
            internal bool HasTeacher(string teacher)
            {
                // TODO pass in marking period
                return GetGradeRows().Any(c => c.CourseRow.Quarter == MarkingPeriodKey.Current.ToString() && c.CourseRow.Teacher == teacher);
            }

            internal bool HasCourse(CourseRow courseRow, MarkingPeriod mp)
            {
                if (courseRow == null)
                    return true;
 
                return GetGradeRows().Any(c => c.CourseRow.CourseKey == courseRow.CourseKey);
            }

            internal bool AttendedDuring(MarkingPeriod mp)
            {
                DateTime enrolled = DateEnrolled;
                DateTime? withdrawn = null;
                if (!IsDateWithdrawnNull())
                    withdrawn = DateWithdrawn;
                return (enrolled < mp.EndDate && (withdrawn == null || withdrawn > mp.EndDate));
            }
        }

        partial class AttendanceRow
        {
            public override string ToString()
            {
                return StudentRow.Name + " " + Date + " " + State;
            }
        }
    }

}