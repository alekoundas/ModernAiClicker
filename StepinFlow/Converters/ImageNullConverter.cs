using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace StepinFlow.Converters
{
    public class ImageNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Avoids binding errors.
            return value ?? DependencyProperty.UnsetValue; 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Not needed for one-way binding.
            return value; 
        }
    }
}
