using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Highlands.StaticModel
{
    public class MarkingPeriod : IEquatable<MarkingPeriod>
    {
        static List<MarkingPeriod> _markingPeriods;
        static public MarkingPeriod Current
        {
            get
            {
                return new MarkingPeriod(2, 2013);
            }
        }

        static public List<MarkingPeriod> MarkingPeriods
        {
            get
            {
                if (_markingPeriods == null)
                {
                    var rv = new List<MarkingPeriod>();
                    for (int year = 2011; year <= 2013; year++)
                    {
                        for (int quarter = 1; quarter <= 4; quarter++)
                        {
                            var mp = new MarkingPeriod(quarter, year);
                            if (mp.ApproximateEndDate > Current.ApproximateEndDate)
                                break;
                            rv.Add(mp);
                        }
                    }
                    _markingPeriods = rv;
                }
                return _markingPeriods;
            }
        }

        public MarkingPeriod(int quarter, int endingSchoolYear)
        {
            Quarter = quarter;
            EndingSchoolYear = endingSchoolYear;
        }
        public DateTime ApproximateEndDate
        {
            get
            {
                if (Quarter == 1)
                    return new DateTime(EndingSchoolYear - 1, 11, 10);
                else if (Quarter == 2)
                    return new DateTime(EndingSchoolYear, 1, 20);
                else if (Quarter == 3)
                    return new DateTime(EndingSchoolYear, 4, 1);
                else //if (Quarter == 4)
                    return new DateTime(EndingSchoolYear, 6, 12);
            }
        }
        public Tuple<int, int> SchoolYear
        {
            get
            {
                if (Quarter == 1 || Quarter == 2)
                    return new Tuple<int, int>(EndingSchoolYear, EndingSchoolYear + 1);
                else
                    return new Tuple<int, int>(EndingSchoolYear - 1, EndingSchoolYear);
            }
        }
        public int EndingSchoolYear { get; private set; }
        public int Quarter { get; private set; }
        public override string ToString()
        {
            return (EndingSchoolYear - 1) + @"/" + (EndingSchoolYear - 2000) + "-Q" + Quarter;
        }


        #region IEquatable<MarkingPeriod> Members

        public bool Equals(MarkingPeriod other)
        {
            return ApproximateEndDate.Equals(other.ApproximateEndDate);
        }

        #endregion

        internal static MarkingPeriod Parse(string str)
        {
            var slash = str.IndexOf(@"/");
            var startYear = str.Substring(0, slash);
            var year = int.Parse(startYear) + 1;
            var q = str.Substring(str.Length - 1, 1);
            return new MarkingPeriod(int.Parse(q), year);
        }
    }
}
