using System;
using System.Globalization;
using System.Windows.Data;

namespace KyleHughes.CIS2118.KPUSim.Converters
{
    /// <summary>
    /// converts an integer to a hex string
    /// </summary>
    public class IntToHexStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                int num = int.Parse(parameter.ToString());
                return "0x" + ushort.Parse("" + value).ToString("X" + num) + "";
            }
            catch (Exception)
            {
                return "0xDEAD";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (((string) value).StartsWith("0x"))
                value = ((string) value).Substring(2);
            return System.Convert.ToInt32(value as string);
        }
    }
}