using System;

namespace KyleHughes.CIS2118.KPUSim.Exceptions
{
    /// <summary>
    /// compiletime exception for chained memory operands (e.g. **7)
    /// </summary>
    public class ChainedMemoryOperandException : ProgramException
    {
        public ChainedMemoryOperandException(string operand)
            : base(true, String.Format("Tried chaining memory operands: {0}. This is not allowed!", operand))
        {
        }
    }
}