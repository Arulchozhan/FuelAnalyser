using PaeoniaTechSpectroMeter.Views;
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
    public class ButtonBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int buttonIndex && parameter is CtrlHistory historyPage)
            {
                return buttonIndex == historyPage.SelectedButtonIndex ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#005fb8")) : Brushes.Transparent;
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
