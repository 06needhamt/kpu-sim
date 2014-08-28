using KyleHughes.CIS2118.KPUSim.Assembly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KyleHughes.CIS2118.KPUSim.Peripherals
{
    /// <summary>
    /// biiig description.
    /// </summary>
    [Peripheral("Clock", "Provides ways of getting the current time.\n\n"
        + "Interrupts:\n"
        + "0: Sets IOD to one greater than the number of years expired since 0 AD\n"
        + "1: Sets IOD to one greater than the number of months expired since the start of the current year\n"
        + "2: Sets IOD to one greater than the number of days expired since the start of the current year\n"
        + "3: Sets IOD to one greater than the number of hours expired since the start of the current day\n"
        + "4: Sets IOD to one greater than the number of minutes expired since the start of the current hour\n"
        + "5: Sets IOD to one greater than the number of seconds expired since the start of the current minute\n\n"
        + "6: Sets IOD to one greater than the number of milliseconds expired since the start of the current second\n\n"
        + "All times and dates are based on those used by the system running the simulator and follow the Georgian calendar."
        , false)]
    public class ClockPeripheral : PeripheralBase
    {
        /// <summary>
        /// constructs a new clock peripheral with the given id
        /// </summary>
        /// <param name="id">id</param>
        public ClockPeripheral(ushort id) : base(id) { }

        /// <summary>
        /// handles an interrupt with command, IOD and IOM
        /// </summary>
        /// <param name="command">command</param>
        /// <param name="data">IOD</param>
        /// <param name="extra">IOM</param>
        public override void HandleInterrupt(ushort command, ushort data, ushort extra)
        {
            switch (command)
            {
                case 0:
                    Registers.IOD.ActualValue = (ushort)DateTime.Now.Year;
                    break;
                case 1:
                    Registers.IOD.ActualValue = (ushort)DateTime.Now.Month;
                    break;
                case 2:
                    Registers.IOD.ActualValue = (ushort)DateTime.Now.Day;
                    break;
                case 3:
                    Registers.IOD.ActualValue = (ushort)DateTime.Now.Hour;
                    break;
                case 4:
                    Registers.IOD.ActualValue = (ushort)DateTime.Now.Minute;
                    break;
                case 5:
                    Registers.IOD.ActualValue = (ushort)DateTime.Now.Second;
                    break;
                case 6:
                    Registers.IOD.ActualValue = (ushort)DateTime.Now.Millisecond;
                    break;
            }
        }
        /// <summary>
        /// on configuring this. this should never happen!
        /// </summary>
        public override void Configure()
        {
            MessageBox.Show("configuring ");
        }
        /// <summary>
        /// nothing to clean
        /// </summary>
        public override void CleanUp()
        {
            
        }
    }
}
