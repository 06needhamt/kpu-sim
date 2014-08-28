using System.Windows;
using KyleHughes.CIS2118.KPUSim.Exceptions;
using KyleHughes.CIS2118.KPUSim.ViewModels;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace KyleHughes.CIS2118.KPUSim.Assembly
{
    /// <summary>
    /// Represents a callable opcode
    /// </summary>
    public class OpCode : IAssemblerValue
    {
        /// <summary>
        /// the number of opcodes created up until this point
        /// </summary>
        private static byte _numOpcodes;
        /// <summary>
        /// whether the first operand (if it exists) can be literal
        /// </summary>
        public bool CanFirstOperandBeLiteral { get; private set; }
        /// <summary>
        /// the number of operands for this opcode
        /// </summary>
        public ushort RequiredOperands { get; private set; }
        /// <summary>
        /// the byte value for this opcode
        /// </summary>
        public byte Code { get; private set; }
        /// <summary>
        /// the action this opcode executes when called
        /// </summary>
        public Action<IOperand[]> Execution { get; private set; }
        /// <summary>
        /// this opcode's mnemonic
        /// </summary>
        public string Mnemonic { get; private set; }
        /// <summary>
        /// this opcode's description for documentation
        /// </summary>
        public string Description { get; private set; }
        public OpCode(string desc, Action<IOperand[]> runCommand, ushort opcount = 2, bool firstLiteral = false, byte? code = null, [CallerMemberName]string mnemonic = null)
        {
            //Bascially finish the documentation
            desc += "\nAccepts " + opcount + " operand" + (opcount == 1 ? "" : "s");
            if (opcount == 1)
            {
                desc += " which can" + (firstLiteral ? "" : "not") + " be a literal value";
            }
            else if (opcount > 1)
            {
                desc += ", the first of which can" + (firstLiteral ? "" : "not") + " be a literal value";
            }
            desc += ".";
            this.Description = desc;

            //set all the values
            if (mnemonic == null)
                throw new Exception("Null mnemonic with opcode " + code);
            if (code == null)
                this.Code = _numOpcodes++;
            else
                this.Code = (byte)code;
            this.CanFirstOperandBeLiteral = firstLiteral;
            this.RequiredOperands = opcount;
            this.Execution = runCommand;
            this.Mnemonic = mnemonic;
            if (OpCodes.All == null)
                OpCodes.InitList();
            OpCodes.All.Add(this);

        }
        /// <summary>
        /// String representation of this opcode
        /// </summary>
        /// <returns>the opcode's mnemonic</returns>
        public override string ToString()
        {
            return Mnemonic;
        }
        /// <summary>
        /// the alphabet. used to uniquely identify operands in documentation
        /// </summary>
        private static string alphabet = "ABCDEFGHIJKLMNOPRSTUVWXYZ";
        /// <summary>
        /// get the string used for this in documentation
        /// </summary>
        public string HelpValue
        {
            get
            {
                string str = Mnemonic;
                for (ushort i = 0; i < RequiredOperands; i++)
                    str += " " + alphabet[i];
                return str;
            }
        }
        /// <summary>
        /// This implements IAssemblerValue but does not need a value
        /// </summary>
        public ushort ActualValue
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }

    /// <summary>
    /// Used to create all opcodes
    /// </summary>
    public static class OpCodes
    {
        /// <summary>
        /// attempts to get an opcode from a givem mnemonic
        /// </summary>
        /// <param name="mne">the mnemonic</param>
        /// <returns>the opcode or null</returns>
        public static OpCode GetOpCodeFromMnemonic(string mne)
        {
            foreach (var v in All)
            {
                if (v.Mnemonic.ToUpper().Equals(mne.ToUpper()))
                    return v;
            }
            return null;
        }

        /// <summary>
        /// attempts to get an opcode from a given byte
        /// </summary>
        /// <param name="b">the byte</param>
        /// <returns>opcode or null</returns>
        public static OpCode GetOpCodeeFromByte(byte b)
        {
            foreach (var v in All)
            {
                if (v.Code == b)
                    return v;
            }
            return null;
        }
        /// <summary>
        /// Stores all opcodes
        /// </summary>
        public static List<OpCode> All { get; private set; }
        /// <summary>
        /// initialisies the list of all opcodes
        /// </summary>
        internal static void InitList()
        {
            All = new List<OpCode>();
        }

        /*
         * I really don't want to comment all of these so i'll
         * comment the interesting ones. They all have descriptions anyway
         */

        /// <summary>
        /// Sets a value
        /// </summary>
        public static OpCode SET = new OpCode("Sets the value of A to the value of B",
            e =>
            {
                //Have to call this on the UI thread or accessibility issues.
                // Would love to find a nice solution to this without destroying the UI
                Application.Current.Dispatcher.Invoke(() =>
                {
                    e[0].ActualValue = e[1].ActualValue;
                });
            });

        public static OpCode ADD = new OpCode("Adds the value of A to the value of B and stores it in A",
            e =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    e[0].ActualValue += e[1].ActualValue;
                });
            });
        public static OpCode SUB = new OpCode("Subtracts the value of B from the value of A and stores it in A",
            e =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    e[0].ActualValue -= e[1].ActualValue;
                });
            });
        public static OpCode MUL = new OpCode("Multiplies the value of A by the value of B and stores it in A",
            e =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    e[0].ActualValue *= e[1].ActualValue;
                });
            });
        public static OpCode DIV = new OpCode("Divides the value of A by the value of B and stores it in A",
            e =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (e[1].ActualValue == 0)
                    {
                        MainViewModel.Instance.ThrowException(new ZeroDivisionException());
                        return;
                    }
                    e[0].ActualValue /= e[1].ActualValue;
                });
            });
        public static OpCode MOD = new OpCode("Stores the value of A mod the value of B in A",
            e =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    e[0].ActualValue %= e[1].ActualValue;
                });
            });
        public static OpCode PSH = new OpCode("Pushes the value of A to the top of the hardware stack",
            e =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MainViewModel.Instance.Stack.Push(e[0].ActualValue);
                });
            }, 1, true);
        public static OpCode POP = new OpCode("Pops the value at the top of the hardware stack and stores it in A",
            e =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    e[0].ActualValue = MainViewModel.Instance.Stack.Pop();
                });
            }, 1);
        public static OpCode DUP = new OpCode("Duplicates the value at the top of the hardware stack",
            e =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ushort value = MainViewModel.Instance.Stack.Pop();
                    MainViewModel.Instance.Stack.Push(value);
                    MainViewModel.Instance.Stack.Push(value);
                });
            }, 0);
        public static OpCode DRP = new OpCode("Drops (removes) the value at the top of the hardware stack",
            e =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MainViewModel.Instance.Stack.Pop();
                });
            }, 0);
        public static OpCode SWP = new OpCode("Swaps the two values at the top of the hardware stack",
            e =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ushort v1 = MainViewModel.Instance.Stack.Pop();
                    ushort v2 = MainViewModel.Instance.Stack.Pop();
                    MainViewModel.Instance.Stack.Push(v1);
                    MainViewModel.Instance.Stack.Push(v2);
                });
            }, 0);
        public static OpCode EQU = new OpCode("If the value of A and the value of B are equal, pushes 1 to the stack. Otherwise pushes 0",
            e =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (e[0].ActualValue == e[1].ActualValue)
                        MainViewModel.Instance.Stack.Push(1);
                    else
                        MainViewModel.Instance.Stack.Push(0);
                });
            }, 2, true);
        public static OpCode NEQ = new OpCode("If the value of A and the value of B are not equal, pushes 1 to the stack. Otherwise pushes 0",
            e =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (e[0].ActualValue == e[1].ActualValue)
                        MainViewModel.Instance.Stack.Push(0);
                    else
                        MainViewModel.Instance.Stack.Push(1);
                });
            }, 2, true);
        public static OpCode LTH = new OpCode("If the value of A is less than the value of B, pushes 1 to the stack. Otherwise pushes 0",
            e =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (e[0].ActualValue < e[1].ActualValue)
                        MainViewModel.Instance.Stack.Push(1);
                    else
                        MainViewModel.Instance.Stack.Push(0);
                });
            }, 2, true);
        public static OpCode GTH = new OpCode("If the value of A is greater than the value of B, pushes 1 to the stack. Otherwise pushes 0",
            e =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (e[0].ActualValue > e[1].ActualValue)
                        MainViewModel.Instance.Stack.Push(1);
                    else
                        MainViewModel.Instance.Stack.Push(0);
                });
            }, 2, true);
        public static OpCode NOT = new OpCode("Pops the value at the top of the stack. If it is 0, pushes 1 to the stack, if it is 1, pushes 0 to the stack. Otherwise pushes no new value",
            e =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ushort val = MainViewModel.Instance.Stack.Pop();
                    if (val == 0)
                    {
                        MainViewModel.Instance.Stack.Push(1);
                    }
                    else if (val == 1)
                    {
                        MainViewModel.Instance.Stack.Push(0);
                    }
                });
            }, 0);
        public static OpCode BAN = new OpCode("Performs a bitwise (binary) AND operation on the value of A and the value of B. Stores the result in A",
            e =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    e[0].ActualValue &= e[1].ActualValue;
                });
            });
        public static OpCode BOR = new OpCode("Performs a bitwise (binary) OR operation on the value of A and the value of B. Stores the result in A",
            e =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    e[0].ActualValue |= e[1].ActualValue;
                });
            });
        public static OpCode BXO = new OpCode("Performs a bitwise (binary) exclusive-OR operation on the value of A and the value of B. Stores the result in A",
            e =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    e[0].ActualValue ^= e[1].ActualValue;
                });
            });
        public static OpCode SHL = new OpCode("Performs a bitwise (binary) left shift on the value of A by B places",
            e =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    e[0].ActualValue <<= e[1].ActualValue;
                });
            });
        public static OpCode SHR = new OpCode("Performs a bitwise (binary) right shift on the value of A by B places",
            e =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    e[0].ActualValue >>= e[1].ActualValue;
                });
            });
        public static OpCode JUM = new OpCode("Pushes the value of the program counter to the stack, then sets the value of the program counter to the value of A",
            e =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MainViewModel.Instance.Stack.Push(Registers.PC.ActualValue);
                    Registers.PC.ActualValue = e[0].ActualValue;
                });
            }, 1, true);
        public static OpCode SJM = new OpCode("Sets the value of the program counter to the value of A",
        e =>
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Registers.PC.ActualValue = e[0].ActualValue;
            });
        }, 1, true);
        public static OpCode CJM = new OpCode("Pops the value from the top of the hardware stack. If it is 1 it pushes the value of the program counter to the stack, then sets the value of the program counter to the value of A. Otherwise does nothing.",
            e =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ushort val = MainViewModel.Instance.Stack.Pop();
                    if (val == 1)
                    {
                        MainViewModel.Instance.Stack.Push(Registers.PC.ActualValue);
                        Registers.PC.ActualValue = e[0].ActualValue;
                    }
                });
            }, 1, true);
        public static OpCode IOX = new OpCode("Sends an interrupt with value B to the peripheral specified in the value of A.\n"
            + "If no peripheral is attached to the device with ID of the value A then nothing happens."
            + "See the add peripherals window for details on each peripheral", e =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    //send the register values for convenience
                    if (MainViewModel.Instance.AttachedPeripherals.ContainsKey(e[0].ActualValue))
                        MainViewModel.Instance.AttachedPeripherals[e[0].ActualValue].HandleInterrupt(e[1].ActualValue, Registers.IOD.ActualValue, Registers.IOM.ActualValue);
                });
            }, 2, true);
        public static OpCode END = new OpCode("Causes the program to stop executing.",
            e =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {//goodbye :(
                    MainViewModel.Instance.StopProgramCommand.Execute(null);
                });
            }, 0, false, 0xff);
    }
}
