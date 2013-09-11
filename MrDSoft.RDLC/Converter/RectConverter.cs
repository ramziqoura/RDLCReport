using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace DSoft.Converter
{
    public class RectConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double width = 0;
            double height = 0;

            if (values[0] != DependencyProperty.UnsetValue)
                width = (double)values[0];

            if (values[1] != DependencyProperty.UnsetValue)
                height = (double)values[1];

            return new Rect(0, 0, width, height);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
