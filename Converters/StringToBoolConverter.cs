// In MyToDo/Converters/StringToBoolConverter.cs (make sure the folder is correct)
using System.Globalization;
using Microsoft.Maui.Controls; // Ensure correct namespace

namespace MyToDo.Converters; // Correct namespace

public class StringToBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string stringValue && parameter is string parameterString)
        {
            return stringValue != parameterString;
        }
        return true;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}