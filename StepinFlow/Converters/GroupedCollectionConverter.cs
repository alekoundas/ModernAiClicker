using Model.Models;
using System.Globalization;
using System.Windows.Data;

namespace StepinFlow.Converters
{
    public class GroupedCollectionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var result = new List<object>();

            // Handle the single FlowParameter instance
            //if (values[0] is FlowParameter flowParameter && flowParameter != null)
            //{
            //    result.Add(new GroupNode { Name = flowParameter.Name, FlowParameter = flowParameter, Items = new List<FlowParameter> { flowParameter. } });
            //}

            //// Handle the single FlowStep instance
            //if (values[1] is FlowStep flowStep && flowStep != null)
            //{
            //    result.Add(new GroupNode { Name = flowStep.Name, FlowStep = flowStep, Items = new List<FlowStep> { flowStep }});
            //}
            if (values[0] is FlowParameter flowParameter && flowParameter != null)
                result.Add(flowParameter);

            if (values[1] is FlowStep flowStep && flowStep != null)
                result.Add(flowStep);


            return result;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}