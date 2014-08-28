using KyleHughes.CIS2118.KPUSim.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KyleHughes.CIS2118.KPUSim.Peripherals
{
    /// <summary>
    /// abstract base class for all peripherals
    /// </summary>
    public abstract class PeripheralBase : NotifiableBase
    {
        /// <summary>
        /// the peripheral's id
        /// </summary>
        public ushort ID { get; private set; }
        /// <summary>
        /// on configuring the peripheral
        /// </summary>
        public abstract void Configure();
        /// <summary>
        /// the peripheral's name (ignored by json)
        /// </summary>
        [JsonIgnore]
        public string Name { get; private set; }
        /// <summary>
        /// construct a new peripheral
        /// </summary>
        /// <param name="id"></param>
        public PeripheralBase(ushort id)
        {
            this.ID = id;
            //get the peripheralattribute from reflection
            MemberInfo info = this.GetType();
            this.Name = info.GetCustomAttribute<PeripheralAttribute>(false).Name;
            this.ConfigureCommand = new ActionCommand(Configure, () => info.GetCustomAttribute<PeripheralAttribute>(false).CanConfigure && MainViewModel.Instance.IsProgramRunning == false);
        }
        /// <summary>
        /// handle an interrupt on this peripheral with the given command, IOD and IOM
        /// </summary>
        /// <param name="command"></param>
        /// <param name="data"></param>
        /// <param name="extra"></param>
        public abstract void HandleInterrupt(ushort command, ushort data, ushort extra);

        /// <summary>
        /// cleans up this peripheral
        /// </summary>
        public abstract void CleanUp();
        /// <summary>
        /// Command to configure. ignored by json
        /// </summary>
        [JsonIgnore]
        public ActionCommand ConfigureCommand { get; private set; }
    }
}
