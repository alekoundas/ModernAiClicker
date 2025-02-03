using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;

namespace StepinFlow.Converters
{
    public class TreeviewOrdering : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IList collection = value as IList;
            if (collection == null)
                return new object();

            ListCollectionView view = new ListCollectionView(collection);
            SortDescription sort = new SortDescription(parameter.ToString(), ListSortDirection.Ascending);
            view.SortDescriptions.Add(sort);

            return view;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
