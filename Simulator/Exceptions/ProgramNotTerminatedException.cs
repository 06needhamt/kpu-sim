using System;

namespace KyleHughes.CIS2118.KPUSim.Exceptions
{
    /// <summary>
    /// run time exception for a program running into unknown memories
    /// </summary>
    public class ProgramNotTerminatedException : ProgramException
    {
        public ProgramNotTerminatedException()
            : base(true, String.Format("Program ended abruptly! Don't forget about the END instruction"))
        {
        }
    }
}