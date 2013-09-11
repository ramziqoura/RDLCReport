using System;
using System.Windows.Media;
using System.Windows.Data;

namespace DSoft.Converter
{
    public class ValueToForegroundColorConverter: IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SolidColorBrush brush = new SolidColorBrush(Colors.Black);

            Double doubleValue = 0.0;

            if (value != null)
                Double.TryParse(value.ToString(), out doubleValue);
            
            if (doubleValue < 0)
                brush = new SolidColorBrush(Colors.Red);

            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}