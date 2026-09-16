using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace eSSLAttendanceDesktopClient.Converter
{
    public class StatusToBGColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                SolidColorBrush brush = new SolidColorBrush();
                var input  = (string)value;
                string result = input.Split(' ')[0];
                if (result == "Complete" || result == "Active")
                {
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#14A44D")); 
                }
                else if (result == "Incomplete"|| result == "Disabled")
                {
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E4A11B")); 
                }
                else
                {
                    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EC0A15")); 
                }

                #region [Old]
                //if (result == "Complete")
                //{
                //    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#09A6EB")); 
                //}
                //else if (result == "Complete")
                //{
                //    //return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E6E6E6")); //E6E6E6
                //    return Brushes.Red;
                //}
                //else if (result == "Late") 
                //{
                //    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC507")); 
                //}
                //else
                //{
                //    return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EC0A15")); 
                //} 
                #endregion
            }
            else
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString("#EC0A15")); 
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
