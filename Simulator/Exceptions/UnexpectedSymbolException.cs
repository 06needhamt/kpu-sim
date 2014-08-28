using System;

namespace KyleHughes.CIS2118.KPUSim.Exceptions
{
    /// <summary>
    /// compile time exception for an unknown symbol
    /// </summary>
    public class UnexpectedSymbolException : ProgramException
    {
        public UnexpectedSymbolException(char symbol)
            : base(true, String.Format("Unexpected symbol: {0}", symbol))
        {
        }
    }
}