namespace KyleHughes.CIS2118.KPUSim.Assembly
{
    /// <summary>
    /// an operand representing a register reference
    /// </summary>
    public class RegisterOperand : IOperand
    {
        /// <summary>
        /// constructs a new register operand using the given register
        /// </summary>
        /// <param name="r">the register</param>
        public RegisterOperand(Register r)
        {
            Register = r;
        }
        /// <summary>
        /// this operand's register
        /// </summary>
        public Register Register { get; private set; }

        /// <summary>
        /// this operand's register's value
        /// </summary>
        public ushort ActualValue
        {
            get { return Register.ActualValue; }
            set { Register.ActualValue = value; }
        }
        /// <summary>
        /// the identifiere kind for this operand
        /// </summary>
        public IdentifierKind Identifier
        {
            get { return IdentifierKind.Register; }
        }
        /// <summary>
        /// the word value for this operand
        /// </summary>
        public ushort Word
        {
            get { return Register.Code; }
        }
        /// <summary>
        /// th string value for this operand
        /// </summary>
        public string Text
        {
            get
            {
                if (Register == null)
                    return "?";
                return Register.Name;
            }
        }

        /// <summary>
        /// whether this operand uses a word
        /// </summary>
        public bool UsesWord
        {
            get { return true; }
        }
    }
}