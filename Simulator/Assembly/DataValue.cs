using System.Globalization;
using KyleHughes.CIS2118.KPUSim.Converters;
using KyleHughes.CIS2118.KPUSim.Exceptions;

namespace KyleHughes.CIS2118.KPUSim.Assembly
{
    /// <summary>
    /// Data values that have been disassembled
    /// </summary>
    public class DataValue : IDisassemblyValue
    {
        /// <summary>
        /// Constructs a new datavalue with the given value
        /// </summary>
        /// <param name="value">The value</param>
        public DataValue(ushort value)
        {
            ActualValue = value;
        }

        public ushort ActualValue { get; set; }

        /// <summary>
        /// We cannot execute literal values. That would be silly
        /// </summary>
        public void Execute()
        {
            throw new ExecutedNonExecutableException(ActualValue);
        }

        /// <summary>
        /// Gets a string representation of this value
        /// </summary>
        public string DisplayString
        {
            get
            {
                //Convert the value to a hex string (0xblah)
                var converter = new IntToHexStringConverter();
                return ActualValue + " (" +
                       converter.Convert(ActualValue, typeof (string), 4, CultureInfo.CurrentUICulture) + ")";
            }
        }

        public override string ToString()
        {
            return DisplayString;
        }
        /// <summary>
        /// String to usee to reassemble this values
        /// </summary>
        public string ReassemblyString
        {
            get
            {
                var converter = new IntToHexStringConverter();
                return converter.Convert(ActualValue, typeof (string), 3, CultureInfo.CurrentUICulture)+"";
            }
        }
    }
}