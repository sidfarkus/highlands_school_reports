using Highlands.StaticModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace Highlands.Model
{
    
    
    public partial class Gradebook {
        partial class GradeDataTable
        {
        }
        static public User CurrentUser { get; set; }
        public static Gradebook Read()
        {
            var rv = new Gradebook();
            try
            {
                rv.ReadXml("gradebook.xml");
            }
            catch (System.Exception)
            {
                return null;
            }
            return rv;
        }

        partial class StudentRow
        {
            internal bool HasTeacher(string teacher)
            {
                // TODO pass in marking period
                return GetGradeRows().Any(c => c.CourseRow.Quarter == MarkingPeriod.Current.ToString() && c.CourseRow.Teacher == teacher);
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
                return (enrolled < mp.ApproximateEndDate && (withdrawn == null || withdrawn > mp.ApproximateEndDate));
            }
        }

    }
        
}