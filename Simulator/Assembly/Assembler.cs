using System;
using System.Collections;
using System.Collections.Generic;
using KyleHughes.CIS2118.KPUSim.Exceptions;

namespace KyleHughes.CIS2118.KPUSim.Assembly
{
    /// <summary>
    /// A boring old Comparer used to compare two integers. Used for ObservableDictonary
    /// </summary>
    public class KeyComparer : IComparer<DictionaryEntry>
    {
        /// <summary>
        /// Compare the values
        /// </summary>
        /// <param name="d1">entry 1</param>
        /// <param name="d2">entry 2</param>
        /// <returns>whichever entry has the highest key</returns>
        public int Compare(DictionaryEntry d1, DictionaryEntry d2)
        {
            int key1 = int.Parse(d1.Key + "");
            int key2 = int.Parse(d2.Key + "");
            if (key1 > key2)
                return 1;
            if (key1 < key2)
                return -1;
            return 0;
        }
    }
    /// <summary>
    /// The different kinds of operand identifers and their values
    /// </summary>
    public enum IdentifierKind
    {
        Zero = 0,
        Register = 1,
        MemoryRegister = 2,
        Literal = 3,
        MemoryLiteral = 4
    }
    /// <summary>
    /// Contains assembly logic
    /// </summary>
    public static class Assembler
    {
        /// <summary>
        /// Gets the correct IdentifierKind for the given byte
        /// </summary>
        /// <param name="b">byte to find a IdentifierKind for</param>
        /// <returns>IdentifierKind for given byte</returns>
        public static IdentifierKind GetIdentifierType(byte b)
        {
            return (IdentifierKind) b;
        }

        /// <summary>
        /// Generate an operand from a string value
        /// </summary>
        /// <param name="value">the string to generate an operand from</param>
        /// <returns>an IOperand generated from the given string</returns>
        public static IOperand GetOperand(string value)
        {
            //get the type of word (not identifer)
            WordKind type = FindWordKind(value);
            if (type == WordKinds.Register)
                return new RegisterOperand(Registers.GetRegisterFromName(value));

            if (type == WordKinds.HexadecimalLiteral)
            {
                long l = Convert.ToInt64(value.Substring(2), 16);
                if (l >= ushort.MinValue && l <= ushort.MaxValue)
                    return new LiteralOperand((ushort) l);
                throw new OutOfRangeLiteralException(l, true);
            }
            if (type == WordKinds.DecimalLiteral)
            {
                long l = long.Parse(value);
                if (l >= ushort.MinValue && l <= ushort.MaxValue)
                    return new LiteralOperand(ushort.Parse(value));
                throw new OutOfRangeLiteralException(l);
            }
            if (type == WordKinds.MemoryLiteral || type == WordKinds.MemoryRegister)
            {
                //If it's a memory operand, do some recursion!
                return new MemoryOperand(GetOperand(value.Substring(1)));
            }
            if (type == WordKinds.LabelDeclaration)
            {
                //not valid operand
                throw new UnexpectedLabelDeclarationException(value.Substring(1));
            }
            if (type == WordKinds.LabelReference)
            {
                return new LabelOperand(value.Substring(1));
            }
            return null;
        }
        /// <summary>
        /// Returns the type of word which the given string is
        /// </summary>
        /// <param name="word">The string to parse</param>
        /// <returns>a WordKind representing the given string</returns>
        public static WordKind FindWordKind(string word)
        {
            //For each type of word (opcode, register, etc)
            foreach (WordKind v in WordKinds.AllItems)
            {
                if (v.DoesMatch(word))
                    return v;
            }
            return null;
        }
    }

    /// <summary>
    /// The base of everything assembly!
    /// </summary>
    public interface IAssemblerValue
    {
        /// <summary>
        /// The actual value this represents
        /// </summary>
        ushort ActualValue { get; set; }
    }

    /// <summary>
    /// Interface for disassembled values
    /// </summary>
    public interface IDisassemblyValue : IAssemblerValue
    {
        /// <summary>
        /// String version of this object. Not .ToString()!
        /// </summary>
        string DisplayString { get; }
        /// <summary>
        /// The string value to use if we try to convert back to code
        /// </summary>
        string ReassemblyString { get; }
        /// <summary>
        /// Execute this value
        /// </summary>
        void Execute();
    }
}