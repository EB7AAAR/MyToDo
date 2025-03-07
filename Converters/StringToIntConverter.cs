﻿using System.Globalization;

namespace MyToDo.Converters;

public class StringToIntConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int intValue)
        {
            return intValue.ToString();
        }
        return "0"; // Default value
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string stringValue && int.TryParse(stringValue, out int intValue))
        {
            return intValue;
        }
        return 0; // Default value if parsing fails
    }
}