using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PaeoniaTechSpectroMeter.Model
{
    public class DenaturantValueToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double? doubleValue = (double?)value;

            if (doubleValue > 100 || doubleValue < 0)
            {
                return "Red";
            }
            else if (doubleValue == 0 || doubleValue == 100)
            {
                return "Yellow";
            }
            else if (doubleValue > 0 && doubleValue < 100)
            {
                return "Green";
            }

            return "Orange";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
