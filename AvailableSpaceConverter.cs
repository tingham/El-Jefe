using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace El_Jefe
{
  public class AvailableSpaceConverter : IValueConverter
  {
    public object OriginalValue;
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(float.TryParse(parameter.ToString(), out float parsedParameter)))
      {
        parsedParameter = 1.0f;
      }
      if (!(float.TryParse(value.ToString(), out float parsedValue)))
      {
        parsedValue = 44.0f;
      }
      return parsedParameter * parsedValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return null;
    }
  }
}
