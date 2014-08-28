using System.Reflection.Emit;
using KyleHughes.CIS2118.KPUSim.Exceptions;
using KyleHughes.CIS2118.KPUSim.ViewModels;

namespace KyleHughes.CIS2118.KPUSim.Assembly
{
    /// <summary>
    /// Label reference operands
    /// </summary>
    public class LabelOperand : IOperand
    {
        /// <summary>
        /// Constructs a new labeloperand with the given label name
        /// </summary>
        /// <param name="labelName">label name</param>
        public LabelOperand(string labelName)
        {
            LabelName = labelName;
        }
        /// <summary>
        /// The name of this label
        /// </summary>
        public string LabelName { get; private set; }
        /// <summary>
        /// The identifier this operand uses
        /// </summary>
        public IdentifierKind Identifier
        {
            get
            {
                if (UsesWord)
                    return IdentifierKind.Literal;
                return IdentifierKind.Zero;
            }
        }
        /// <summary>
        /// the word representation of this object
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
            get { return ActualValue + ""; }
        }
        /// <summary>
        /// whether this operand uses a word or not
        /// </summary>
        public bool UsesWord
        {
            get
            {
                //if this doesn't use a word then it must be zero. so if it's not zero then it has to use a word
                if (MainViewModel.Instance.LabelMap.ContainsKey(LabelName) && MainViewModel.Instance.LabelMap[LabelName] == 0)
                    return false;
                return true;
            }
        }
        /// <summary>
        /// Literal value of this object
        /// </summary>
        public ushort ActualValue
        {
            get
            {
                //this is only ever called after compilation so if it doesn't exist it hasn't been defined
                if (MainViewModel.Instance.LabelMap.ContainsKey(LabelName))
                {
                    return MainViewModel.Instance.LabelMap[LabelName];
                }
                throw new UnsatisfiedLabelException(LabelName);
            }
            set { }
        }
    }
}