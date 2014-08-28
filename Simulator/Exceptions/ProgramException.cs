using System;

namespace KyleHughes.CIS2118.KPUSim.Exceptions
{
    /// <summary>
    /// Base exception for this program
    /// </summary>
    public class ProgramException : Exception
    {
        public ProgramException(bool iscompile, string msg = "Error!") : base(msg)
        {
            IsCompileError = iscompile;
        }

        public bool IsCompileError { get; private set; }
    }
}