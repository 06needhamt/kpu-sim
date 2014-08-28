using System.Collections.Generic;
using System.Runtime.CompilerServices;
using KyleHughes.CIS2118.KPUSim.ViewModels;

namespace KyleHughes.CIS2118.KPUSim.Assembly
{
    /// <summary>
    /// The register class
    /// </summary>
    public class Register : NotifiableBase, IAssemblerValue
    {
        private ushort _value;

        /// <summary>
        /// constructs a register with the given id
        /// </summary>
        /// <param name="code">id</param>
        /// <param name="name">name</param>
        public Register(byte code, [CallerMemberName] string name = null)
        {
            Code = code;
            this.Name = name;
            if (Registers.All == null)
                Registers.InitList();
            // ReSharper disable once PossibleNullReferenceException
            Registers.All.Add(this);
        }

        /// <summary>
        /// this register's id
        /// </summary>
        public byte Code { get; private set; }
        /// <summary>
        /// this register's name
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// the value in this register
        /// </summary>
        public ushort ActualValue
        {
            get { return _value; }
            set
            {
                _value = value;
                Notify();
            }
        }
    }

    public static class Registers
    {
        //Create all the registers
        public static Register A = new Register(0x00);
        public static Register B = new Register(0x01);
        public static Register C = new Register(0x02);
        public static Register D = new Register(0x03);
        public static Register E = new Register(0x04);
        public static Register PC = new Register(0x05);
        public static Register IOM = new Register(0x06);
        public static Register IOD = new Register(0x07);
        /// <summary>
        /// store all registers
        /// </summary>
        public static List<Register> All { get; private set; }
        //reset all registers
        public static void Reset()
        {
            foreach (Register v in All)
                v.ActualValue = 0;
        }
        /// <summary>
        /// attempt to get a register from its name
        /// </summary>
        /// <param name="name">the name</param>
        /// <returns>register or null</returns>
        public static Register GetRegisterFromName(string name)
        {
            foreach (Register v in All)
            {
                if (v.Name.ToUpper().Equals(name.ToUpper()))
                    return v;
            }
            return null;
        }

        /// <summary>
        /// attempt to get a register from its id
        /// </summary>
        /// <param name="b">the id</param>
        /// <returns>register or null</returns>
        public static Register GetRegisterFromID(ushort b)
        {
            foreach (Register v in All)
            {
                if (v.Code == b)
                    return v;
            }
            return null;
        }

        /// <summary>
        /// initialise the register list
        /// </summary>
        internal static void InitList()
        {
            All = new List<Register>();
        }
    }
}