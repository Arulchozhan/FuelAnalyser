using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace PaeoniaTechSpectroMeter.Model
{
    public class PageNumberToBackgroundColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 2 && values[0] != null && values[1] != null)
            {
                int currentPage, pageNumber;
                if (int.TryParse(values[0].ToString(), out currentPage) && int.TryParse(values[1].ToString(), out pageNumber))
                {
                    if (currentPage == pageNumber)
                        return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#005fb8"));  // Highlight color
                }
            }
            return Brushes.Transparent; // Default color
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}
