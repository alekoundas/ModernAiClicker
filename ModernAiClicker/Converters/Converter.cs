using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace ModernAiClicker.Converters
{
        public class Converter : IMultiValueConverter
        {
            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                IEnumerable<object> folders = values[0] as IEnumerable<object>;

                if (folders != null )
                    return folders;

                return null;
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) =>
                throw new NotSupportedException();
    }
}
