using System;

namespace KyleHughes.CIS2118.KPUSim.Exceptions
{
    /// <summary>
    /// compile time exception for having a literal be too big or too small
    /// </summary>
    public class OutOfRangeLiteralException : ProgramException
    {
        public OutOfRangeLiteralException(long symbol, bool isHex = false)
            : base(true, GetErrorString(symbol, isHex))
        {
        }

        /// <summary>
        /// Gets a relevant error message for the given number and whether it was in hexadecimal
        /// </summary>
        /// <param name="num">value</param>
        /// <param name="hex">was hex?</param>
        /// <returns></returns>
        private static string GetErrorString(long num, bool hex)
        {
            //just gets a relevant error messaage
            if (hex)
            {
                if (num > ushort.MaxValue)
                    return String.Format("0x{0:X} is too big. The maximum value is 0x{1:X}", num, ushort.MaxValue);
                if (num < ushort.MinValue)
                    return String.Format("0x{0:X} is too small. The mininum value is 0x{1:X}", num, ushort.MinValue);
            }
            if (num > ushort.MaxValue)
                return String.Format("{0} is too big. The maximum value is {1}", num, ushort.MaxValue);
            if (num < ushort.MinValue)
                return String.Format("{0} is too small. Negative numbers are not allowed", num);
            return "ERRORS";
        }
    }
}