using System.Globalization;
namespace MyToDo.Converters;
public class ThemeToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is AppTheme theme)
        {
            if (theme == AppTheme.Dark)
            {
                return (Color)Application.Current.Resources["Gray950"]; // Dark theme color
            }
            else
            {
                return (Color)Application.Current.Resources["Gray200"]; // Light theme color
            }
        }
        return (Color)Application.Current.Resources["Gray200"]; // Default to light theme
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException(); // Not needed for this scenario
    }
}