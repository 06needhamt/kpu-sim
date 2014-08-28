using System;

namespace KyleHughes.CIS2118.KPUSim.Exceptions
{
    /// <summary>
    /// compile time exception for not declaring a label
    /// </summary>
    public class UnsatisfiedLabelException : ProgramException
    {
        public UnsatisfiedLabelException(string opcode)
            : base(false, String.Format("Label '{0}' was referenced but not declared", opcode))
        {
        }
    }
}