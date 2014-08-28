using DrWPF.Windows.Data;
using KyleHughes.CIS2118.KPUSim.Peripherals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KyleHughes.CIS2118.KPUSim
{
    /// <summary>
    /// plain old data for storing the state of a program
    /// </summary>
    public class SaveContainer
    {
        public ObservableDictionary<ushort,PeripheralBase> UsedPeripherals { get; set; }
        public string Code { get; set; }
    }
}
