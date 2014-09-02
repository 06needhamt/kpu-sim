using KyleHughes.CIS2118.KPUSim.Assembly;
using KyleHughes.CIS2118.KPUSim.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KyleHughes.CIS2118.KPUSim.Peripherals
{
    /// <summary>
    /// biiig description.
    /// </summary>
    [Peripheral("Storage Device", "Provides a way of storing data in files"
        + "Interrupts:\n"
        + "0: Show the visualisation display\n"
        + "1: Write the value in IOM to the location specified in IOD\n"
        + "2: Save the contents of memory to bank id IOD\n"
        + "3: Load the contents of memory from bank id IOD"
        , false)]
    public class StoragePeripheral : PeripheralBase
    {
        public StorageView Window { get; private set; }
        public ushort[] Values { get; private set; }
        /// <summary>
        /// constructs a new clock peripheral with the given id
        /// </summary>
        /// <param name="id">id</param>
        public StoragePeripheral(ushort id)
            : base(id)
        {
            this.Window = new StorageView(this);
            this.Values = new ushort[256 * 256];
        }

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
                    if (this.Window == null || this.Window.IsLoaded == false)
                        this.Window = new StorageView(this);
                    this.Window.Show();
                    break;
                case 1:
                    this.Values[Registers.IOD.ActualValue] = Registers.IOM.ActualValue;
                    this.Window.Form.Refresh();
                    break;
                case 2:
                    this.SaveToFile(Registers.IOD.ActualValue);
                    break;
                case 3:
                    this.LoadFromFile(Registers.IOD.ActualValue);
                    break;
            }
        }

        private void SaveToFile(ushort id)
        {
            using (var stream = new FileStream(string.Format("save_{0}",id), FileMode.Create, FileAccess.Write))
            {
                using (var writer = new BinaryWriter(stream))
                {
                    foreach (ushort val in this.Values)
                    {
                        writer.Write(val);
                    }
                }
            }
        }

        private void LoadFromFile(ushort id)
        {
            try
            {
                using (var stream = new FileStream(string.Format("save_{0}", id), FileMode.Open, FileAccess.Read))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        ushort i = 0;
                        while (reader.BaseStream.Position < reader.BaseStream.Length)
                        {
                            this.Values[i] = reader.ReadUInt16();
                            i++;
                        }
                    }
                }
            }
            catch (FileNotFoundException) { }
            this.Window.Form.Refresh();
        }

        /// <summary>
        /// on configuring this. this should never happen!
        /// </summary>
        public override void Configure()
        {
            // Nothing :)
        }
        /// <summary>
        /// nothing to clean
        /// </summary>
        public override void CleanUp()
        {
            if (this.Window.IsLoaded)
                this.Window.Close();
        }
    }
}
