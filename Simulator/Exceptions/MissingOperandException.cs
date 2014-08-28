using System;
using KyleHughes.CIS2118.KPUSim.Assembly;

namespace KyleHughes.CIS2118.KPUSim.Exceptions
{
    /// <summary>
    /// compile time exception for missing an operand
    /// </summary>
    public class MissingOperandException : ProgramException
    {
        public MissingOperandException(OpCode opcode)
            : base(true, String.Format("Operand for {0} is missing", opcode))
        {
        }
    }
}