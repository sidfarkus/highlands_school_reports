using Highlands.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Highlands.ViewModel
{
    public class SDScoreViewModel
    {
        Gradebook.SelfDevelopmentRow _row;
        public SDScoreViewModel(Gradebook.SelfDevelopmentRow row)
        {
            _row = row;
        }
        public string Area
        {
            get
            {
                return _row.Area;
            }
            set
            {
                _row.Area = value;
            }
        }
        public string Quarter
        {
            get
            {
                return _row.Quarter;
            }
            set
            {
                _row.Quarter = value;
            }
        }
        public int Score
        {
            get
            {
                return _row.Score;
            }
            set
            {
                _row.Score = value;
            }
        }
        public string Teacher
        {
            get
            {
                return _row.Teacher;
            }
            set
            {
                _row.Teacher = value;
            }
        }
    }

}
