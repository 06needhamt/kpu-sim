using System;

namespace KyleHughes.CIS2118.KPUSim.Exceptions
{
    /// <summary>
    /// run time exception for misplaced label declarations
    /// </summary>
    public class UnexpectedLabelDeclarationException : ProgramException
    {
        public UnexpectedLabelDeclarationException(string symbol)
            : base(true, String.Format("Label declaration {0} unexpected here", symbol))
        {
        }
    }
}