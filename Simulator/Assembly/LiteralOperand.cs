using System.Globalization;
using KyleHughes.CIS2118.KPUSim.Converters;

namespace KyleHughes.CIS2118.KPUSim.Assembly
{
    /// <summary>
    /// A literal operand
    /// </summary>
    public class LiteralOperand : IOperand
    {
        /// <summary>
        /// whether this requires a word
        /// </summary>
        private readonly bool requiresWord = false;
        /// <summary>
        /// Constructs a new literaloperand with the given value. optionally force it to use a word
        /// </summary>
        /// <param name="value">literal value</param>
        /// <param name="forceUseWord">use a word?</param>
        public LiteralOperand(ushort value, bool forceUseWord = false)
        {
            this.ActualValue = value;
            this.requiresWord = value != 0;
            if (forceUseWord)
                requiresWord = true;
        }
        /// <summary>
        /// the value of this operand
        /// </summary>
        public virtual ushort ActualValue { get; set; }
        /// <summary>
        /// the identifier kind of this value
        /// </summary>
        public IdentifierKind Identifier
        {
            get
            {
                //the documentation explains this! :)
                if (requiresWord)
                    return IdentifierKind.Literal;
                if (ActualValue == 0)
                    return IdentifierKind.Zero;
                return IdentifierKind.Literal;
            }
        }
        /// <summary>
        /// the word representation of this operand
        /// </summary>
        public ushort Word
        {
            get { return ActualValue; }
        }
        /// <summary>
        /// the string representation of this operand
        /// </summary>
        public string Text
        {
            get
            {
                if (ActualValue == 0)
                    return "0";
                //convert it to a hex string (0xblah)
                var converter = new IntToHexStringConverter();
                return converter.Convert(ActualValue, typeof (string), 4, CultureInfo.CurrentUICulture) + "";
            }
        }

        /// <summary>
        /// whether this operand uses a word
        /// </summary>
        public bool UsesWord
        {
            get { return requiresWord; }
        }
    }
}