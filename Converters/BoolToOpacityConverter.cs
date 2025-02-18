using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace MyToDo.Converters
{
    public class BoolToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Return 0.5 opacity if true, 1.0 opacity if false
            return (bool)value ? 0.5 : 1.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Not needed for this converter
            throw new NotImplementedException();
        }
    }
}