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
        public void CloneQuarter(MarkingPeriodKey last, MarkingPeriodKey next)
        {
            if (false == MarkingPeriods.Singleton.Any(m => m.Key.Equals(next)))
                MarkingPeriods.Singleton.AddMarkingPeriod(new MarkingPeriod(next, next.ApproximateStartDate, next.ApproximateEndDate, 45));

            var courses = Course.Where(c => c.Quarter == last.ToString());
            foreach (var course in courses)
                Course.AddCourseRow(course.CourseKey + course.Quarter, course.SubjectName, course.Quarter, course.Group, course.Teacher, course.Level);
            
            Save();
        }

        /*
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
        */
 
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
            this.WriteXml("gradebook.xml");
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
                    var name = "-";
                    if (CurrentUser != null)
                        name = CurrentUser.Name;
                    if (this.Count() < 1)
                        AddConfigRow(value, DateTime.Now.ToString(), name);
                    else
                    {
                        this[0].Guid = value;
                        this[0].LastModified = DateTime.Now.ToString();
                        this[0].UserModified = CurrentUser.Name;
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

    }
        
}