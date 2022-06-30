using System;
using System.Windows;
using System.Windows.Data;

namespace KeyManager.Converter
{
    class DateTimeConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var date = (DateTime?) value;

            return date?.ToString("dd.MM.yyyy");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strValue = value as string;

            DateTime date;

            if (DateTime.TryParse(strValue, out date))
            {
                return date;
            }
            return DependencyProperty.UnsetValue;
        }
    }
}
