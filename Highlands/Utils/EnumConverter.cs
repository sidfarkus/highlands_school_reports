using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Highlands.Utils
{
    public sealed class EnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string enumStr;
            try
            {
                enumStr = Enum.GetName((value.GetType()), value);
                string outStr = "";
                for (int i = 1; i < enumStr.Length; i++)
                {
                    if (Char.IsUpper(enumStr[i]))
                    {
                        outStr += " ";
                    }
                    outStr += enumStr[i];
                }
                return outStr;
            }
            catch
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType,
          object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
