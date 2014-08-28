using System;
using System.Globalization;
using System.Windows;
using System.Reflection;
using System.Windows.Data;
using KyleHughes.CIS2118.KPUSim.Peripherals;

namespace KyleHughes.CIS2118.KPUSim.Converters
{
    /// <summary>
    /// gets the description of a peripheral from its type
    /// </summary>
    public class PeripheralTypeToPeripheralDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return "";
            Type t = value as Type;
            return t.GetCustomAttribute<PeripheralAttribute>(false).Description;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}