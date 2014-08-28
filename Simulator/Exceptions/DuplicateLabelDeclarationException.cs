using System;

namespace KyleHughes.CIS2118.KPUSim.Exceptions
{
    /// <summary>
    /// compile time exception for declaring a label more than once
    /// </summary>
    public class DuplicateLabelDeclarationException : ProgramException
    {
        public DuplicateLabelDeclarationException(string name)
            : base(true, String.Format("Label {0} has been declared more than once!",name))
        {
        }
    }
}