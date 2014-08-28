using System;

namespace KyleHughes.CIS2118.KPUSim.Exceptions
{
    /// <summary>
    /// run time exception for trying to divide by zero
    /// </summary>
    public class ZeroDivisionException : ProgramException
    {
        public ZeroDivisionException()
            : base(false, String.Format("Attempted to divide by zero! This is impossible!"))
        {
        }
    }
}