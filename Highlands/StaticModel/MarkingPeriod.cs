using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Highlands.StaticModel
{
    public class MarkingPeriodKey : IEquatable<MarkingPeriodKey>
    {
        public int Quarter { get; set; }
        public int EndingSchoolYear { get; set; }
        static MarkingPeriodKey _current;

        public string QuarterString
        {
            get
            {
                return new[] { "First", "Second", "Third", "Fourth" }[Quarter - 1];
            }
        }

        public MarkingPeriodKey(int quarter, int year)
        {
            Quarter = quarter;
            EndingSchoolYear = year;
        }

        static public MarkingPeriodKey Current
        {
            get
            {
                if (_current == null)
                {
                    var str = Maintenance.ReadFromFile("currentmarkingperiod");
                    _current = MarkingPeriodKey.Parse(str);
                    if (_current == null)
                    {
                        _current = new MarkingPeriodKey(2, 2013);
                        Maintenance.WriteToFile("currentmarkingperiod", _current.ToString());
                    }
                }
                return _current;
            }
        }

        internal static MarkingPeriodKey Parse(string str)
        {
            try
            {
                var slash = str.IndexOf(@"/");
                var startYear = str.Substring(0, slash);
                var year = int.Parse(startYear) + 1;
                var q = str.Substring(str.Length - 1, 1);
                return new MarkingPeriodKey(int.Parse(q), year);
            }
            catch (Exception)
            {
                return null;
            }
        }


        public DateTime ApproximateStartDate
        {
            get
            {
                if (Quarter == 1)
                    return new DateTime(EndingSchoolYear - 1, 9, 1);
                else if (Quarter == 2)
                    return new DateTime(EndingSchoolYear - 1, 11, 11);
                else if (Quarter == 3)
                    return new DateTime(EndingSchoolYear, 1, 21);
                else //if (Quarter == 4)
                    return new DateTime(EndingSchoolYear, 4, 2);
            }
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

        public override string ToString()
        {
            return (EndingSchoolYear - 1) + @"/" + (EndingSchoolYear - 2000) + "-Q" + Quarter;

        }

        #region IEquatable<MarkingPeriodKey> Members

        public bool Equals(MarkingPeriodKey other)
        {
            return ToString().Equals(other.ToString());
            throw new NotImplementedException();
        }

        #endregion
    }
    public class MarkingPeriod
    {
        static List<MarkingPeriod> _markingPeriods;
        public MarkingPeriodKey Key { get; set; }
        
        static public List<MarkingPeriod> MarkingPeriods
        {
            get
            {
                if (_markingPeriods == null)
                {
                    var lines = Maintenance.ReadArrayFromFile("MarkingPeriods");
                    var rv = new List<MarkingPeriod>();
                    if (lines != null)
                    {
                        foreach (var line in lines)
                            rv.Add(MarkingPeriod.FromCsv(line));

                    }
                    else
                    {
                        for (int year = 2011; year <= 2013; year++)
                        {
                            for (int quarter = 1; quarter <= 4; quarter++)
                            {
                                var key = new MarkingPeriodKey(quarter, year);
                                var mp = new MarkingPeriod(key, key.ApproximateStartDate, key.ApproximateEndDate);
                                if (mp.EndDate > MarkingPeriodKey.Current.ApproximateEndDate)
                                    break;
                                rv.Add(mp);
                            }
                        }
                        Maintenance.WriteArrayToFile("MarkingPeriods", rv.Select(m => m.ToCsv()).ToList());
                    }
                    _markingPeriods = rv;
                }
                return _markingPeriods;
            }
        }

        private string ToCsv()
        {
            return StartDate.ToShortDateString() + "," + EndDate.ToShortDateString() + "," + Key.Quarter;
        }

        static private MarkingPeriod FromCsv(string str)
        {
            var mp = new MarkingPeriod();
            var parts = str.Split(",".ToCharArray());
            mp.StartDate = DateTime.Parse(parts[0]);
            mp.EndDate = DateTime.Parse(parts[1]);
            mp.Key = new MarkingPeriodKey(int.Parse(parts[2]), mp.EndDate.Year);
            return mp;
        }

        public MarkingPeriod()
        {
        }

        public MarkingPeriod(MarkingPeriodKey key, DateTime startDate, DateTime endDate)
        {
            Key = key;
            StartDate = startDate;
            EndDate = endDate;
        }
        public Tuple<int, int> SchoolYear
        {
            get
            {
                if (Key.Quarter == 1 || Key.Quarter == 2)
                    return new Tuple<int, int>(Key.EndingSchoolYear, Key.EndingSchoolYear + 1);
                else
                    return new Tuple<int, int>(Key.EndingSchoolYear - 1, Key.EndingSchoolYear);
            }
        }
        public override string ToString()
        {
            return Key.ToString();
        }


        #region IEquatable<MarkingPeriod> Members

        public bool Equals(MarkingPeriod other)
        {
            return Key.Equals(other.Key);
        }

        #endregion

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public static MarkingPeriod Current
        {
            get
            {
                var key = MarkingPeriodKey.Current;
                return MarkingPeriods.SingleOrDefault(m => m.Key.ToString() == key.ToString());
            }
        }

    }
}
