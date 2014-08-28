using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace KyleHughes.CIS2118.KPUSim.Converters
{
    /// <summary>
    /// converts a boolean to a visibility
    /// </summary>
    public class BooleanVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
                return (bool) value ? Visibility.Visible : Visibility.Collapsed;
            throw new Exception("Invalid binding type - expected boolean, got " + value.GetType());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}