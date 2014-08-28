using System;

namespace KyleHughes.CIS2118.KPUSim.Exceptions
{
    /// <summary>
    /// compile time exception for using an unknown word
    /// </summary>
    public class UnknownWordException : ProgramException
    {
        public UnknownWordException(string symbol)
            : base(true, String.Format("Unknown Word: {0}", symbol))
        {
        }
    }
}