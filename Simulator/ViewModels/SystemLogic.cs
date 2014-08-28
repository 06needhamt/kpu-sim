using DrWPF.Windows.Data;
using KyleHughes.CIS2118.KPUSim.Assembly;
using KyleHughes.CIS2118.KPUSim.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

// This file is just to make MainViewModel cleaner by seperating all of the logic for the simulator into here
namespace KyleHughes.CIS2118.KPUSim.ViewModels
{
    public partial class MainViewModel
    {
        public const string AllowedCharacters =
            @"ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
            @"abcdefghijklmnopqrstuvwxyz" +
            @"0123456789_*$>" +
            @"\s";

        private readonly BackgroundWorker _codeRunner = new BackgroundWorker();
        public ObservableStack<ushort> Stack { get; set; }
        public ObservableSortedDictionary<ushort, ushort> Ram { get; set; }
        public ObservableDictionary<string, ushort> LabelMap { get; set; }
        public List<string> UnsatisfiedLabels { get; set; }

        public ObservableSortedDictionary<ushort, IDisassemblyValue> Instructions { get; set; }
        public ActionCommand AssembleProgramCommand { get; set; }
        public ActionCommand RunProgramCommand { get; set; }
        public ActionCommand StopProgramCommand { get; set; }
        public bool IsProgramRunning { get; set; }
        /// <summary>
        /// has the code changed since last compile
        /// </summary>
        public bool HasCodeChanged
        {
            get { return !CodeText.Trim().Equals(_lastCompiledCode.Trim()); }
        }
        /// <summary>
        /// starts building the program
        /// </summary>
        private void BuildProgram()
        {
            // Reset previous state
            Ram = new ObservableSortedDictionary<ushort, ushort>(new KeyComparer());
            LabelMap = new ObservableDictionary<string, ushort>();
            Stack = new ObservableStack<ushort>();
            Registers.Reset();
            ReformatCode();
            RunningState = RunState.Compiling;
            try
            {
                DoLabelPass();
                // Assemble the program
                AssembleProgram(Ram);
                // Disassemble it again
                DisassembleProgram();
                RunningState = RunState.Compiled;
                _lastCompiledCode = CodeText;
            }
            catch (ProgramException exception)
            {
                ThrowException(exception);
            }
            Notify("All");
        }
        /// <summary>
        /// assembles the program from code
        /// </summary>
        /// <param name="memory">the location to store the program</param>
        private void AssembleProgram(ObservableSortedDictionary<ushort, ushort> memory)
        {
            // Split the code into words
            string[] words = CorrectCode(CodeText).Split(' ');

            // If the program is empty we can't really do much
            if (string.IsNullOrEmpty(CodeText) || string.IsNullOrWhiteSpace(CodeText))
                throw new EmptyProgramException();

            // For each word
            ushort ramIndex = 0;
            for (ushort position = 0; position < words.Length; )
            {
                string word = words[position];
                // Get the type of thing the word is (opcode, literal or register)
                WordKind type = Assembler.FindWordKind(word);
                // Switch on them
                if (type.IsLiteral())
                {
                    // If they typed a literal then add it to ram and increase the position
                    memory.Add(ramIndex, Assembler.GetOperand(word).ActualValue);
                    position += 1;
                    ramIndex++;
                }
                else if (type == WordKinds.Register || type == WordKinds.MemoryLiteral || type == WordKinds.MemoryRegister)
                {
                    // If they typed a register reference then something went wrong. It shouldn't be there!
                    throw new UnexpectedReferenceException(word);
                }
                else if (type == WordKinds.OpCode)
                {
                    // If they typed an opcode then assemble it
                    AssembleOpCode(memory, words, ref position, ref ramIndex);
                }
                else if (type == null)
                {
                    throw new UnknownWordException(word);
                }
                else if (type == WordKinds.LabelDeclaration)
                {
                    position++;
                }
            }
        }

        private void DoLabelPass()
        {
            //this is the same as the above method just faster because it doesn't do any real work

            // Split the code into words
            string[] words = CorrectCode(CodeText).Split(' ');

            // If the program is empty we can't really do much
            if (string.IsNullOrEmpty(CodeText) || string.IsNullOrWhiteSpace(CodeText))
                throw new EmptyProgramException();

            // For each word
            ushort ramIndex = 0;
            for (ushort position = 0; position < words.Length; )
            {
                string word = words[position];
                WordKind type = Assembler.FindWordKind(word);
                if (type.IsLiteral())
                {
                    position += 1;
                    ramIndex++;
                }
                else if (type == WordKinds.Register || type == WordKinds.MemoryLiteral || type == WordKinds.MemoryRegister)
                {
                    position++;
                }
                else if (type == WordKinds.OpCode)
                {
                    SilentlyAssembleOpCode(words, ref position, ref ramIndex);
                }
                else if (type == null)
                {
                    throw new UnknownWordException(word);
                }
                else if (type == WordKinds.LabelDeclaration)
                {
                    position++;
                    if (LabelMap.ContainsKey(word.Substring(1)))
                        throw new DuplicateLabelDeclarationException(word.Substring((1)));
                    LabelMap.Add(word.Substring(1), ramIndex);
                }
            }
        }
        /// <summary>
        /// Disassembles the program
        /// </summary>
        public void DisassembleProgram()
        {
            Instructions = new ObservableSortedDictionary<ushort, IDisassemblyValue>(new KeyComparer());
            for (ushort position = 0; position < Ram.Count; )
            {
                if (!Ram.ContainsIndex(position))
                {
                    position++;
                    continue;
                }

                ushort oldpos = position;
                Instruction instruction = GetValidInstruction(Ram, ref position);
                if (instruction == null)
                {
                    Instructions.Add(oldpos, new DataValue(Ram[position]));
                    position++;
                }
                else
                {
                    Instructions.Add(oldpos, instruction);
                    position++;
                }
            }
            Notify(null);
        }
        /// <summary>
        /// Attempts tog et a valid instruction from given words starting from given position
        /// </summary>
        /// <param name="words">words</param>
        /// <param name="position">position</param>
        /// <returns></returns>
        private Instruction GetValidInstruction(ObservableSortedDictionary<ushort, ushort> words, ref ushort position)
        {
            ushort word = words[position];
            ushort oldpos = position;
            OpCode opcode = OpCodes.GetOpCodeeFromByte((byte)(0xff & word));
            if (opcode == null)
            {
                return null;
            }
            //get identifiers
            IOperand operand1 = null, operand2 = null;
            IdentifierKind operand1Identifier = Assembler.GetIdentifierType((byte)((word & 0x0f00) >> 8));
            IdentifierKind operand2Identifier = Assembler.GetIdentifierType((byte)((word & 0xf000) >> 12));
            try
            {
                //we need them!
                switch (opcode.RequiredOperands)
                {
                    case 0:
                        break;
                    case 1:
                        operand1 = GetOperandFromIdentifier(words, ref position, operand1Identifier);
                        break;
                    case 2:
                        operand1 = GetOperandFromIdentifier(words, ref position, operand1Identifier);
                        operand2 = GetOperandFromIdentifier(words, ref position, operand2Identifier);
                        break;
                }
            }
            catch (KeyNotFoundException)
            {
                position = oldpos;
                return null;
            }
            //return our result
            return new Instruction(opcode, operand1, operand2);
        }

        /// <summary>
        /// gets the type of operand from a given identifier at a given position in a list of words
        /// </summary>
        /// <param name="words"></param>
        /// <param name="position"></param>
        /// <param name="identifier"></param>
        /// <returns></returns>
        private IOperand GetOperandFromIdentifier(ObservableSortedDictionary<ushort, ushort> words, ref ushort position,
            IdentifierKind identifier)
        {
            switch (identifier)
            {
                case IdentifierKind.Zero:
                    return new LiteralOperand(0);
                case IdentifierKind.Literal:
                    position++;
                    return new LiteralOperand(words[position], true);
                case IdentifierKind.Register:
                    position++;
                    return new RegisterOperand(Registers.GetRegisterFromID(words[position]));
                case IdentifierKind.MemoryLiteral:
                    position++;
                    return new MemoryOperand(new LiteralOperand(words[position]));
                case IdentifierKind.MemoryRegister:
                    position++;
                    return new MemoryOperand(new RegisterOperand(Registers.GetRegisterFromID(words[position])));
            }
            return null;
        }

        /// <summary>
        /// tidies codde and makes it nicer
        /// </summary>
        private void ReformatCode()
        {
            CodeText = CodeText.Trim();

            // Replace all duplicate whitespace (excluding newlines)
            CodeText = Regex.Replace(CodeText, "[ \t]+", " ");
            // Fix uppercase x-es in hex numbers after capitalisation above (0XFF43 becomes 0xFF43, etc)
            CodeText = Regex.Replace(CodeText, "(?<=0)X(?=[abcdefABCDEF0123456789])", "x");
        }
        /// <summary>
        /// removes comments, replaces string with literals and checks for any disallowed characters
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private string CorrectCode(string code)
        {
            //replace all comments
            IEnumerable<Match> matches = WordKinds.Comment.GetMatches(code);
            foreach (Match v in matches.Reverse())
            {
                code = code.Remove(v.Index, v.Length);
            }

            IEnumerable<Match> m = WordKinds.StringLiteral.GetMatches(code);
            while (m != null && m.Count() > 0)
            {
                Match ele = m.ElementAt(0);
                char[] chars = ele.Value.Replace("\"", "").ToCharArray();
                string insert = " ";
                foreach (var c in chars)
                {
                    insert += ((int)c) + " ";
                }
                string start = code.Substring(0, ele.Index);
                string end = code.Substring(ele.Index + ele.Length);
                code = start + insert + end;

                m = WordKinds.StringLiteral.GetMatches(code);
            }

            //If the code has any disallowed characters
            if (Regex.IsMatch(code, @"[^" + AllowedCharacters + "]"))
                throw new UnexpectedSymbolException(Regex.Matches(code, @"[^" + AllowedCharacters + "]")[0].Value[0]);
            // Replace all occurances of multiple whitespace with a single space
            code = Regex.Replace(code, @"\s+", " ");
            return code.Trim();
        }
        /// <summary>
        /// tries to assemble an opcode at a given index
        /// </summary>
        /// <param name="memory"></param>
        /// <param name="words"></param>
        /// <param name="wordIndex"></param>
        /// <param name="ramIndex"></param>
        private void AssembleOpCode(ObservableSortedDictionary<ushort, ushort> memory, string[] words,
            ref ushort wordIndex, ref ushort ramIndex)
        {
            OpCode opcode = OpCodes.GetOpCodeFromMnemonic(words[wordIndex]);
            // Check we have valid operands. If we do not then this method will throw exceptions for us
            EnsureHasValidOperandsForOpCode(words, wordIndex, opcode);
            ushort identifierByte = opcode.Code;
            IOperand operand1, operand2;
            // Switch on the operands
            switch (opcode.RequiredOperands)
            {
                // If it uses no operands then we're already good
                case 0:
                    memory.Add(ramIndex, identifierByte);
                    break;
                case 1:
                    // If we use 1 operand then 
                    operand1 = Assembler.GetOperand(words[wordIndex + 1]);
                    identifierByte |= (ushort)((int)operand1.Identifier << 8);
                    // Add the identifier
                    memory.Add(ramIndex, identifierByte);
                    // If the operand uses a word then add that
                    if (operand1.UsesWord)
                    {
                        ramIndex++;
                        memory.Add(ramIndex, operand1.Word);
                    }
                    break;
                case 2:
                    // If we use two operands, do what we did for one operand
                    operand1 = Assembler.GetOperand(words[wordIndex + 1]);
                    identifierByte |= (ushort)((int)operand1.Identifier << 8);
                    operand2 = Assembler.GetOperand(words[wordIndex + 2]);
                    identifierByte |= (ushort)((int)operand2.Identifier << 12);
                    memory.Add(ramIndex, identifierByte);
                    // add the operand's words if it uses them
                    if (operand1.UsesWord)
                    {
                        ramIndex++;
                        memory.Add(ramIndex, operand1.Word);
                    }
                    if (operand2.UsesWord)
                    {
                        ramIndex++;
                        memory.Add(ramIndex, operand2.Word);
                    }
                    break;
            }
            ramIndex++;
            wordIndex += (ushort)(opcode.RequiredOperands + 1);
        }
        /// <summary>
        /// pretends to assemble. used for label pass
        /// </summary>
        /// <param name="words"></param>
        /// <param name="wordIndex"></param>
        /// <param name="ramIndex"></param>
        private void SilentlyAssembleOpCode(string[] words, ref ushort wordIndex, ref ushort ramIndex)
        {




            OpCode opcode = OpCodes.GetOpCodeFromMnemonic(words[wordIndex]);
            // Check we have valid operands. If we do not then this method will throw exceptions for us
            EnsureHasValidOperandsForOpCode(words, wordIndex, opcode);
            IOperand operand1;
            // Switch on the operands
            switch (opcode.RequiredOperands)
            {
                // If it uses no operands then we're already good
                case 0:
                    break;
                case 1:
                    // If we use 1 operand then 
                    operand1 = Assembler.GetOperand(words[wordIndex + 1]);
                    // Add the identifier
                    // If the operand uses a word then add that
                    if (operand1.UsesWord)
                    {
                        ramIndex++;
                    }
                    break;
                case 2:
                    // If we use two operands, do what we did for one operand
                    operand1 = Assembler.GetOperand(words[wordIndex + 1]);
                    IOperand operand2 = Assembler.GetOperand(words[wordIndex + 2]);
                    // add the operand's words if it uses them
                    if (operand1.UsesWord)
                    {
                        ramIndex++;
                    }
                    if (operand2.UsesWord)
                    {
                        ramIndex++;
                    }
                    break;
            }
            ramIndex++;
            wordIndex += (ushort)(opcode.RequiredOperands + 1);
        }

        private void EnsureHasValidOperandsForOpCode(string[] words, ushort position, OpCode opcode)
        {
            // Switch on the required operands
            switch (opcode.RequiredOperands)
            {
                // If it requires none then it is allowed
                case 0:
                    return;
                // If it required one then
                case 1:
                    // If the index does not exist in the array, return false (not enough words for it to be valid)
                    if (words.ContainsIndex(position + 1) == false)
                        throw new MissingOperandException(opcode);
                    // If the type of value is an opcode then throw exception 
                    // (they probably missed an operand)
                    if (Assembler.FindWordKind(words[position + 1]) == WordKinds.OpCode)
                        throw new MissingOperandException(opcode);
                    // If the opcode allows its first operand to be literal then return true 
                    // (we know it is either a literal or register)
                    if (opcode.CanFirstOperandBeLiteral)
                        return;
                    if (Assembler.FindWordKind(words[position + 1]).IsLiteral())
                        throw new FirstOperandLiteralException(opcode);
                    return;
                // If it requires two then
                case 2:
                    // If the index does not exist, return false, as above
                    if (words.ContainsIndex(position + 2) == false)
                        throw new MissingOperandException(opcode);
                    // If the type of value is an opcode then throw exception 
                    // (they probably missed an operand)
                    if (Assembler.FindWordKind(words[position + 2]) == WordKinds.OpCode)
                        throw new MissingOperandException(opcode);
                    // Goto case 1 t
                    goto case 1;
            }
            throw new Exception("opcode count was not valid. this should never happen");
        }
        /// <summary>
        /// run the program. the coderunner's logic
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        private void RunProgramBody(object s, object e)
        {
            RunningState = RunState.Running;
            try
            {
                IsProgramRunning = true;
                while (IsProgramRunning)
                {
                    IDisassemblyValue instruct = Instructions[Registers.PC.ActualValue];
                    if (_codeRunner.CancellationPending)
                        return;
                    if (instruct is DataValue)
                        throw new ExecutedNonExecutableException(instruct.ActualValue);
                    Instructions[Registers.PC.ActualValue].Execute();
                    Notify(null);
                    Thread.Sleep(1001 - Speed);
                }
                IsProgramRunning = false;
            }
            catch (KeyNotFoundException)
            {
                ThrowException(new ProgramNotTerminatedException());
            }
            catch (ExecutedNonExecutableException exc)
            {
                ThrowException(exc);
            }
        }
        public bool ResetOnRun { get; set; }
        /// <summary>
        /// start running
        /// </summary>
        private void RunProgram()
        {
            if (ResetOnRun)
            {
                Stack.Clear();
                Registers.Reset();
            }
            Registers.PC.ActualValue = 0;
            // Run the now disassembled program
            if (!_codeRunner.IsBusy)
                _codeRunner.RunWorkerAsync();
        }

        #region oldcode
    }

        #endregion
}