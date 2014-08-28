using System.Collections.ObjectModel;

namespace KyleHughes.CIS2118.KPUSim
{
    /// <summary>
    /// very basic 'fake' stack which just adds pop and push commands to an observablecollection
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableStack<T> : ObservableCollection<T>
    {
        public T Pop()
        {
            if (Count == 0)
                return default(T); //default is zero
            T obj = this[0];
            RemoveAt(0);
            return obj;
        }

        public void Push(T obj)
        {
            Insert(0, obj);
        }
    }
}