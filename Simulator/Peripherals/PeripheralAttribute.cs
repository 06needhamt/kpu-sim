using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KyleHughes.CIS2118.KPUSim.Peripherals
{
    /// <summary>
    /// Attribute used for defining a peripheral by its type
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class PeripheralAttribute : Attribute
    {
        /// <summary>
        /// the peripheral's name
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// whether the peripheral can be configured
        /// </summary>
        public bool CanConfigure { get; private set; }
        /// <summary>
        /// the peripheral's description
        /// </summary>
        public string Description { get; private set; }
        /// <summary>
        /// Constructs a new peripheralhelpattribute
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="desc">description</param>
        /// <param name="canconfig">can this be configured?</param>
        public PeripheralAttribute(string name, string desc, bool canconfig)
        {
            this.Name = name;
            this.Description = desc;
            this.CanConfigure = canconfig;
        }
    }
}
