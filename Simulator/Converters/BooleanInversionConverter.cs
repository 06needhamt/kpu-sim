using System;
using System.Globalization;
using System.Windows.Data;

namespace KyleHughes.CIS2118.KPUSim.Converters
{
    /// <summary>
    /// Converter for inverting booleans
    /// </summary>
    public class BooleanInversionConverter : IValueConverter
    {
        /// <summary>
        /// Inverts a boolean
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
                return !(bool) value;
            throw new Exception("Invalid binding type - expected boolean, got " + value.GetType());
        }

        /// <summary>
        /// Inverts a boolean!
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}