using Highlands.ViewModel;
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

    public class MarkingPeriods : List<MarkingPeriod>
    {
        static MarkingPeriods _singleton;
        static public MarkingPeriods Singleton
        {
            get
            {
                if (_singleton == null)
                {
                    bool loaded = false;
                    var lines = Maintenance.ReadArrayFromFile("MarkingPeriods");
                    var rv = new MarkingPeriods();
                    if (lines != null)
                    {
                        try
                        {
                            foreach (var line in lines)
                                rv.Add(MarkingPeriod.FromCsv(line));
                            loaded = true;
                        }
                        catch (Exception)
                        {
                        }
                    }
                    if (!loaded)
                    {
                        rv = DemoFactory.DemoMarkingPeriod();
                        Maintenance.WriteArrayToFile("MarkingPeriods", rv.Select(m => m.ToCsv()).ToList());
                    }
                    _singleton = rv;
                }
                return _singleton;
            }
        }

        public void AddMarkingPeriod(MarkingPeriod mp)
        {
            MarkingPeriods.Singleton.Add(mp);
            Maintenance.WriteArrayToFile("MarkingPeriods", this.Select(m => m.ToCsv()).ToList());
        }

        internal MarkingPeriod Find(MarkingPeriodKey mpk)
        {
            return this.First(q => mpk.ToString().Equals(q.Key.ToString()));
        }
    }

    public class Period
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DaysIn { get; set; }
    }

    public class MarkingYear : Period
    {
        public MarkingYear(MarkingPeriod mp)
        {
            var mps = MarkingPeriods.Singleton.Where(q => q.Key.EndingSchoolYear == mp.Key.EndingSchoolYear && q.Key.Quarter <= mp.Key.Quarter).OrderBy(q => q.Key.Quarter);
            DaysIn = mps.Sum(q => q.DaysIn);
            StartDate = mps.First().StartDate;
            EndDate = mps.Last().EndDate;
        }

        static public MarkingYear Current
        {
            get
            {
                return new MarkingYear(MarkingPeriod.Current);
            }
        }
    }

    public class MarkingPeriod : Period
    {
        public MarkingPeriodKey Key { get; set; }
        
        internal string ToCsv()
        {
            return StartDate.ToShortDateString() + "," + EndDate.ToShortDateString() + "," + Key.Quarter + "," + DaysIn;
        }

        static internal MarkingPeriod FromCsv(string str)
        {
            var mp = new MarkingPeriod();
            var parts = str.Split(",".ToCharArray());
            mp.StartDate = DateTime.Parse(parts[0]);
            mp.EndDate = DateTime.Parse(parts[1]);
            var quarter = int.Parse(parts[2]);
            int year = mp.EndDate.Year;
            if (quarter == 1)
                year++;
            mp.Key = new MarkingPeriodKey(quarter, year);
            mp.DaysIn = int.Parse(parts[3]);
            return mp;
        }

        public MarkingPeriod()
        {
        }

        public MarkingPeriod(MarkingPeriodKey key, DateTime startDate, DateTime endDate, int daysInQuarter)
        {
            Key = key;
            StartDate = startDate;
            EndDate = endDate;
            DaysIn = daysInQuarter;
        }
        public Tuple<int, int> SchoolYear
        {
            get
            {
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

        public static MarkingPeriod Current
        {
            get
            {
                var key = MarkingPeriodKey.Current;
                return MarkingPeriods.Singleton.SingleOrDefault(m => m.Key.ToString() == key.ToString());
            }
        }
    }
}
