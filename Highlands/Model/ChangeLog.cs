using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Highlands.StaticModel;
using System.IO;

namespace Highlands.Model
{
    class ChangeLog
    {
        static public void LogDiffs(IList<Change> diffs)
        {
            diffs.ToList().ForEach(d => Log(d));
        }

        static private void Log(Change diff)
        {
            var line = DateTime.Now.ToString() + " Object: " + diff.Tag + " Changed: " + diff.Label + " From: " + diff.From + " To: " + diff.To + " By: " + Gradebook.CurrentUser;
            File.AppendAllText("diff.log", line + Environment.NewLine);
        }

    }
}
