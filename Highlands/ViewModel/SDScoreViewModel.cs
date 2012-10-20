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
        }
        public string Quarter
        {
            get
            {
                return _row.Quarter;
            }
        }
        public int Score
        {
            get
            {
                return _row.Score;
            }
        }
        public string Teacher
        {
            get
            {
                return _row.Teacher;
            }
        }
    }

}
