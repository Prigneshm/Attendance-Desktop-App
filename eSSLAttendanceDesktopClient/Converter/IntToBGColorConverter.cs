using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace eSSLAttendanceDesktopClient.Converter
{
    public class IntToBGColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                SolidColorBrush brush = new SolidColorBrush();
                var input = (int)value;
                if (input == 1)
                {
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F5F5"));
                }
                else
                {
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E6E6E6"));
                }
            }
            else
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E6E6E6"));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
