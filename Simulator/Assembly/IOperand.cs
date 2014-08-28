namespace KyleHughes.CIS2118.KPUSim.Assembly
{
    /// <summary>
    /// All operands are one of these
    /// </summary>
    public interface IOperand : IAssemblerValue
    {
        /// <summary>
        /// The type of identifier this operanduses
        /// </summary>
        IdentifierKind Identifier { get; }
        /// <summary>
        /// the word value of this operand
        /// </summary>
        ushort Word { get; }
        /// <summary>
        /// the string value of this operand
        /// </summary>
        string Text { get; }
        /// <summary>
        /// wether this operand uses a word or not
        /// </summary>
        bool UsesWord { get; }
    }
}