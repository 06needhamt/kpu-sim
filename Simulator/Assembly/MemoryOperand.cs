using System.Reflection.Emit;
using KyleHughes.CIS2118.KPUSim.ViewModels;

namespace KyleHughes.CIS2118.KPUSim.Assembly
{
    /// <summary>
    /// Memory reference operand
    /// </summary>
    public class MemoryOperand : IOperand
    {
        /// <summary>
        /// Constructs a new memory reference operand with the given inner operand
        /// </summary>
        /// <param name="operand">the inner operand</param>
        public MemoryOperand(IOperand operand)
        {
            Operand = operand;
        }
        /// <summary>
        /// this memory reference's inner operand
        /// </summary>
        public IOperand Operand { get; private set; }

        public IdentifierKind Identifier
        {
            get
            {
                //basically use the identifier of the inner operand
                if (Operand is RegisterOperand)
                    return IdentifierKind.MemoryRegister;
                if (Operand is LiteralOperand || Operand is LabelOperand)
                    return IdentifierKind.MemoryLiteral;
                return Operand.Identifier;
            }
        }
        /// <summary>
        /// the word representation of this operand
        /// </summary>
        public ushort Word
        {
            get
            {
                return Operand.Word;
            }
        }
        /// <summary>
        /// the string representaiton of this operand
        /// </summary>
        public string Text
        {
            get { return "*" + Operand.Text; }
        }
        /// <summary>
        /// whether this operand uses a word or not
        /// </summary>
        public bool UsesWord
        {
            get { return true; }
        }
        /// <summary>
        /// The actual value of this operand
        /// </summary>
        public ushort ActualValue
        {
            get
            {
                //if the address doesn't exist in ram yet
                if (!MainViewModel.Instance.Ram.ContainsIndex(Operand.ActualValue))
                    return 0; //return 0
                return MainViewModel.Instance.Ram[Operand.ActualValue]; //otherwise, add it
            }
            set
            {
                //if the address exists then set the new value
                if (MainViewModel.Instance.Ram.ContainsIndex(Operand.ActualValue))
                    MainViewModel.Instance.Ram[Operand.ActualValue] = value;
                else//otherwise
                    MainViewModel.Instance.Ram.Add(Operand.ActualValue, value); //add the value 
                MainViewModel.Instance.DisassembleProgram(); // disassemble the program again to update the display
            }
        }
    }
}