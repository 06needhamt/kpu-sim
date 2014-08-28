using System;
using KyleHughes.CIS2118.KPUSim.Assembly;

namespace KyleHughes.CIS2118.KPUSim.Exceptions
{
    /// <summary>
    /// compile time exception for giving an invalid first operand
    /// </summary>
    public class FirstOperandLiteralException : ProgramException
    {
        public FirstOperandLiteralException(OpCode opcode)
            : base(true, String.Format("The first operand for {0} cannot be a literal", opcode))
        {
        }
    }
}