using System;
using System.Globalization;
using System.Windows.Data;

namespace KyleHughes.CIS2118.KPUSim.Converters
{
    /// <summary>
    /// converts an integer to a hex string without the 0x notation
    /// </summary>
    public class QuietIntToHexStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                int num = int.Parse(parameter.ToString());
                return ushort.Parse(value.ToString()).ToString("X" + num) + "";
            }
            catch (Exception)
            {
                return "DEAD";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (((string)value).StartsWith("0x"))
                value = ((string)value).Substring(2);
            return System.Convert.ToInt32(value as string);
        }
    }
}