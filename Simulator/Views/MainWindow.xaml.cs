using KyleHughes.CIS2118.KPUSim.ViewModels;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace KyleHughes.CIS2118.KPUSim.Views
{
    // / <summary>
    // / Interaction logic for MainWindow.xaml
    // / </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void DoubleClickLabel(object sender, MouseButtonEventArgs e)
        {
            if (LabelView.SelectedItem == null)
                return;
            RamView.SelectedIndex = (((KeyValuePair<string, ushort>) LabelView.SelectedItem)).Value;
            RamView.ScrollIntoView(RamView.SelectedItem);
            RamView.Focus();
        }

        private void window_Closed(object sender, System.EventArgs e)
        {
            if (MainViewModel.Instance.IsProgramRunning)
                MainViewModel.Instance.StopProgramCommand.Execute(null);
            foreach(Window v in App.Current.Windows){
                v.Close();
            }
        }
    }
}