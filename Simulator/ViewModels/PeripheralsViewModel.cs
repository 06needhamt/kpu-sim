using KyleHughes.CIS2118.KPUSim.Peripherals;
using KyleHughes.CIS2118.KPUSim.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace KyleHughes.CIS2118.KPUSim.ViewModels
{
    /// <summary>
    /// basically the exact same as examplesviewmodel
    /// </summary>
    public class PeripheralsViewModel
    {
        public PeripheralView Window { get; set; }
        public Type SelectedPeripheral { get; set; }
        public List<Type> AllPeripherals { get; set; }
        public ActionCommand AttachPeripheralCommand { get; set; }

        public PeripheralsViewModel()
        {
            this.AllPeripherals = new List<Type>(
                (from Type p in System.Reflection.Assembly.GetCallingAssembly().GetTypes()
                where !p.Name.Equals("PeripheralBase")
                where typeof(PeripheralBase).IsAssignableFrom(p)
                select p).AsEnumerable() //love me some linq
            );
            this.AttachPeripheralCommand = new ActionCommand(() =>
            {
                ushort key = 0;
                if (MainViewModel.Instance.AttachedPeripherals.Count >= 1)
                    key = (ushort)(MainViewModel.Instance.AttachedPeripherals.Last().Key+1);
                MainViewModel.Instance.AttachedPeripherals.Add(key,(PeripheralBase)Activator.CreateInstance(SelectedPeripheral,key));
                Window.Close();
            });
        }
        /// <summary>
        /// start a peripheralsview.
        /// </summary>
        public void Start()
        {
            this.Window = new PeripheralView();
            this.Window.ShowDialog();
        }
    }
}
