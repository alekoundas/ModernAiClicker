//using Model.ConverterModels;
//using System.Globalization;
//using System.Windows.Data;

//namespace StepinFlow.Converters
//{
//    public class EventParammeterConverter : IMultiValueConverter
//    {
//        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
//        {
//            EventParammeters findCommandParameters = new EventParammeters();

//            if (values.Length >= 2)
//                findCommandParameters.FlowId = values[1];

//            if (values.Length >= 3)
//                findCommandParameters.FlowStepId = values[2];

//            return findCommandParameters;
//        }

//        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
