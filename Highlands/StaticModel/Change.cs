using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Highlands.StaticModel
{
    public class Change
    {

        static public string FormatDiffs(List<Change> diffs)
        {
            var lines = new List<string>();
            foreach (var diff in diffs)
                lines.Add(diff.FormatDiff());

            return string.Join(Environment.NewLine, lines);
        }

        private string FormatDiff()
        {
            var line = "Changing " + Label + Environment.NewLine;
            if (!string.IsNullOrWhiteSpace(From))
            {
                line += "  " + "From" + Environment.NewLine +
                        "    " + From + Environment.NewLine;
            }
            line += "  " + "To" + Environment.NewLine +
                    "    " + To;
            return line;
        }

        public Change(object tag, string label, string from, string to)
        {
            Tag = tag;
            Label = label;
            From = from;
            To = to;
        }
        public object Tag { get; set; }
        public string Label { get; private set; }
        public string From { get; private set; }
        public string To { get; private set; }
    }

}
