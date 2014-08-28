using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KyleHughes.CIS2118.KPUSim.ViewModels
{
    /// <summary>
    /// base class for viewmodels and data things
    /// </summary>
    public abstract class NotifiableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void Notify([CallerMemberName] string prop = "Null")
        {
            if (PropertyChanged == null)
                return;
            if (prop != null && prop.Equals("All"))
                prop = null;
            PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}