using System.Windows;
using KyleHughes.CIS2118.KPUSim.Properties;
using System;
using KyleHughes.CIS2118.KPUSim.Views;

namespace KyleHughes.CIS2118.KPUSim
{
    // <summary>
    // Interaction logic for App.xaml
    // </summary>
    public partial class App
    {
        public ParameteredActionCommand UniversalCloseWindowCommand{get;private set;}
        public App()
        {
            if (Type.GetType("Mono.Runtime") != null)
            {
                MessageBox.Show("Sorry, this program is not fully supported by Mono and needs to be ran on Windows with .Net 4.5 or higher");
                App.Current.Shutdown();
            }
            this.UniversalCloseWindowCommand = new ParameteredActionCommand((o) =>
            {
                (o as Window).Close();
            });
        }
        public static Settings AppSettings
        {
            get { return App.Current.Resources["AppSettings"] as Settings; }
        }
    }
}