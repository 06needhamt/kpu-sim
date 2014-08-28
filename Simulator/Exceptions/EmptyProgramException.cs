namespace KyleHughes.CIS2118.KPUSim.Exceptions
{
    /// <summary>
    /// compile time exception for having no codes
    /// </summary>
    public class EmptyProgramException : ProgramException
    {
        public EmptyProgramException()
            : base(true, "Your program has no code")
        {
        }
    }
}