using Highlands.StaticModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Highlands.Model
{
    
    
    public partial class Gradebook {

        public partial class StudentDataTable
        {
            public void ReadCsv()
            {
                var file = "studentTable.txt";
                var lines = File.ReadLines(file);
                var separator = ",".ToCharArray();
                foreach (string line in lines)
                {
                    string[] fields = line.Split(separator);
                    object[] values = new object[] { fields };
                    Rows.Add(values);
                }
            }
        }

        partial class GradeDataTable
        {
            public void ReadCsv()
            {
                var file = "gradeTable.txt";
                var lines = File.ReadLines(file);
                var separator = ",".ToCharArray();
                foreach (string line in lines)
                {
                    string[] fields = line.Split(separator);
                    object[] values = new object[] { fields };
                    Rows.Add(values);
                }
            }
        }

        partial class CourseDataTable
        {
            public void ReadCsv()
            {
                var file = "courseTable.txt";
                var lines = File.ReadLines(file);
                var separator = ",".ToCharArray();
                foreach (string line in lines)
                {
                    string[] fields = line.Split(separator);
                    object[] values = new object[] { fields };
                    Rows.Add(values);
                }
            }
        }
        static Gradebook _singleton;
        public static Gradebook Read()
        {
            if (_singleton == null)
            {
                var rv = new Gradebook();
                //rv.ReadXml("Gradebook.xml");
                try
                {
                    rv.ReadXml("gradebook.xml");
                    /*var files = Directory.GetFiles(".", "grade*.xml");
                    foreach (var file in files)
                    {
                        var gradeTable = new GradeDataTable();
                        gradeTable.ReadXml(file);
                        rv.Merge(gradeTable);
                    }
                    rv.Student.ReadXml("studentTable.xml");
                    rv.Course.ReadXml("courseTable.xml");
                    */
                }
                catch (System.Exception)
                {
                    return null;
                }
                _singleton = rv;
            }
            return _singleton;
        }

        public void Save()
        {
            _singleton = this;
            this.WriteXml("gradebook.xml");
            /*Student.WriteXml("studentTable.xml");
            Grade.WriteXml("gradeTable.xml");
            Course.WriteXml("courseTable.xml");
            */
        }

        static public User CurrentUser { get; set; }

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