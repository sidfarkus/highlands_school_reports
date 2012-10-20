using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Highlands
{
    public class ViewUtils
    {
        public static void Mail(string address, string subject, string body)
        {
            System.Diagnostics.Process.Start("mailto:" + address + "?subject=" + subject +"&body=" + EncodeForMail(body));
        }
        
        private static string EncodeForMail(string str)
        {
            str = str.Replace("%", "%25");
            str = str.Replace("+", "%2B");
            str = str.Replace(Environment.NewLine, "%0A");
            str = str.Replace("\n", "%0A");
            str = str.Replace("\"", "'");
            return str;
        }
        
        static string Filename(Control control)
        {
            return control.Name + ".cfg";
        }

        static public void ReadFromConfig(TextBox ent)
        {
            try
            {
                ent.Text = System.IO.File.ReadAllText(Filename(ent));
            }
            catch (Exception)
            {
            }
        }

        static public void WriteToConfig(TextBox ent)
        {
            try
            {
                System.IO.File.WriteAllText(Filename(ent), ent.Text);
            }
            catch (Exception)
            {
            }
        }

        static public void ReadFromConfig(ComboBox cmb)
        {
            try
            {
                var lines = System.IO.File.ReadAllLines(Filename(cmb));
                cmb.Items.Clear();
                if (lines.Count() == 0)
                    return;
                lines.ToList().ForEach(l => cmb.Items.Add(l));
                cmb.Text = lines[0];
            }
            catch (Exception)
            {
            }
        }

        static public void WriteToConfig(ComboBox cmb)
        {
            try
            {
                
                var lines = new List<string>();
                foreach (string item in cmb.Items)
                    lines.Add(item);

                // put current text at top of list
                var text = cmb.Text;
                if (lines.Contains(text))
                    lines.Remove(text);
                lines.Insert(0, text);

                System.IO.File.WriteAllLines(Filename(cmb), lines);
            }
            catch (Exception)
            {
            }
        }

    }
}

