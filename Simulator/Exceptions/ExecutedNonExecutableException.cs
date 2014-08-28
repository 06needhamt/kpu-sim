using System;

namespace KyleHughes.CIS2118.KPUSim.Exceptions
{
    /// <summary>
    /// run time exception for trying to execute a literal
    /// </summary>
    public class ExecutedNonExecutableException : ProgramException
    {
        public ExecutedNonExecutableException(ushort opcode)
            : base(false, String.Format("Attempted to execute invalid data ({0}) as instruction", opcode))
        {
        }
    }
}