using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PaeoniaTechSpectroMeter.Model
{
    public class ValueToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int? intValue = (int?)value;

            if (intValue >= 1 && intValue <= 20)
            {
                return "Red";
            }
            else if (intValue >= 21 && intValue <= 50)
            {
                return "Yellow";
            }
            else if (intValue >= 51 && intValue <= 100)
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
