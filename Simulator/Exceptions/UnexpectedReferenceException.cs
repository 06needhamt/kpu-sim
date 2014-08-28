using System;

namespace KyleHughes.CIS2118.KPUSim.Exceptions
{
    /// <summary>
    /// compile time exception for referencing a register where you shouldnt be
    /// </summary>
    public class UnexpectedReferenceException : ProgramException
    {
        public UnexpectedReferenceException(string symbol)
            : base(true, String.Format("Reference {0} was unexpected here", symbol))
        {
        }
    }
}