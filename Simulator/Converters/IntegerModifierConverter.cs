using System;
using System.Globalization;
using System.Windows.Data;

namespace KyleHughes.CIS2118.KPUSim.Converters
{
    /// <summary>
    /// Moifies an integet by adding (or subtracting) the parameter
    /// </summary>
    public class IntegerModifierConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return int.Parse(value.ToString()) + int.Parse(parameter.ToString());
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, 0 - (int) parameter, culture);
        }
    }
}