using System;

namespace KyleHughes.CIS2118.KPUSim.Assembly
{
    /// <summary>
    /// A class representing a complete instruction
    /// </summary>
    public class Instruction : IDisassemblyValue
    {
        /// <summary>
        /// Constructs an instruction with the given opcode and operands
        /// </summary>
        /// <param name="code">the opcode</param>
        /// <param name="operands">the operands (varargs)</param>
        public Instruction(OpCode code, params IOperand[] operands)
        {
            OpCode = code;
            Operands = operands;
        }

        /// <summary>
        /// This instruction's operands
        /// </summary>
        public IOperand[] Operands { get; private set; }
        /// <summary>
        /// this instruction's opcode
        /// </summary>
        public OpCode OpCode { get; private set; }

        /// <summary>
        /// Gets the length of this instruction in words
        /// </summary>
        public byte LengthInWords
        {
            get
            {
                //For each possible instruction, if we use it, add 1!
                byte count = 1;
                if (Operands[0] != null && Operands[0].Identifier != IdentifierKind.Zero)
                    count++;
                if (Operands[1] != null && Operands[1].Identifier != IdentifierKind.Zero)
                    count++;

                return count;
            }
        }
        /// <summary>
        /// Executes this instruction
        /// </summary>
        public void Execute()
        {
            //Increment PC by the length of this instruction
            Registers.PC.ActualValue += LengthInWords;
            //invoke the opcode's action
            OpCode.Execution.Invoke(Operands);
        }
        /// <summary>
        /// The literal value of this instruction
        /// </summary>
        public ushort ActualValue
        {
            get
            {
                //generate an identifier word for this instruction
                ushort value = OpCode.Code;
                if (OpCode.RequiredOperands >= 1)
                    value = (ushort)(value | (ushort)((int)Operands[0].Identifier << 8));
                if (OpCode.RequiredOperands >= 2)
                    value = (ushort)(value | (ushort)((int)Operands[1].Identifier << 12));
                return value;
            }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Gets a string representation of this object
        /// </summary>
        public string DisplayString
        {
            get
            {
                string msg = OpCode.Mnemonic;
                foreach (IOperand v in Operands)
                    if (v != null)
                        msg += " " + v.Text;
                return msg;
            }
        }
        
        public override string ToString()
        {
            return DisplayString;
        }
        /// <summary>
        /// A string value of this object to use for disassembly
        /// </summary>
        public string ReassemblyString
        {
            get
            {
                string msg = OpCode.Mnemonic;

                foreach (IOperand v in Operands)
                    if (v != null)
                        msg += " " + v.Text;
                return msg;
            }
        }
    }
}