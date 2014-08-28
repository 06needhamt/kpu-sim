using KyleHughes.CIS2118.KPUSim.Peripherals;
using KyleHughes.CIS2118.KPUSim.Properties;
using KyleHughes.CIS2118.KPUSim.Views;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KyleHughes.CIS2118.KPUSim.ViewModels
{
    /// <summary>
    /// contains an example program
    /// </summary>
    public struct ExampleProgram
    {
        public SaveContainer SaveFile { get; set; }
        public string Name { get; set; }
    }
    /// <summary>
    /// example view model
    /// </summary>
    public class ExamplesViewModel
    {
        /// <summary>
        /// this window
        /// </summary>
        public ExamplesView Window { get; set; }
        /// <summary>
        /// the current one
        /// </summary>
        public ExampleProgram? SelectedExample { get; set; }
        /// <summary>
        /// all examples
        /// </summary>
        public ObservableCollection<ExampleProgram> ExamplePrograms { get; set; }
        /// <summary>
        /// command to load
        /// </summary>
        public ActionCommand LoadExampleProgram { get; set; }
        /// <summary>
        /// construct an exampleviewmodel
        /// </summary>
        public ExamplesViewModel()
        {
            //if in design mode then return
            if ((bool)(DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue))
                return;
            //iterate through resources and load the example
            ResourceSet resourceSet = Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
            this.ExamplePrograms = new ObservableCollection<ExampleProgram>(
                from DictionaryEntry n in resourceSet
                where !n.Key.Equals("Font") //not the font file
                select new ExampleProgram()
                {
                    Name = (string)n.Key, //gotta deserialize json
                    SaveFile = JsonConvert.DeserializeObject<SaveContainer>(Encoding.Default.GetString((byte[])n.Value),new JsonSerializerSettings(){
                        TypeNameHandling=TypeNameHandling.All
                    })
                }
            );
            //load command
            LoadExampleProgram = new ActionCommand(() =>
            {
                if (!SelectedExample.HasValue)
                    return;
                //clear current everything!
                MainViewModel.Instance.CodeText = SelectedExample.Value.SaveFile.Code;
                foreach(KeyValuePair<ushort, PeripheralBase> v in MainViewModel.Instance.AttachedPeripherals){
                    v.Value.CleanUp();
                }
                MainViewModel.Instance.AttachedPeripherals.Clear();
                foreach (KeyValuePair<ushort, PeripheralBase> v in SelectedExample.Value.SaveFile.UsedPeripherals) 
                    MainViewModel.Instance.AttachedPeripherals.Add(v.Key,v.Value);
                this.Window.Close();
                this.Window.listBox.SelectedIndex = -1;
            }, () => SelectedExample.HasValue);
        }
        /// <summary>
        /// starts the viewmodel
        /// </summary>
        public void Start()
        {
            this.Window = new ExamplesView();
            this.Window.ShowDialog();
        }
    }
}
