using Model.ConverterModels;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace ModernAiClicker.Converters
{
    public class EventParammeterConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Button button = (Button)values[0];
            EventParammeters findCommandParameters = new EventParammeters();

            findCommandParameters.ElementName = button.Name;
            findCommandParameters.Value = values[1]?.ToString();

            if (values.Length >= 3)
                findCommandParameters.SecondValue = values[2].ToString();

            return findCommandParameters;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
