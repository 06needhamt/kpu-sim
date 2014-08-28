using System;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace KyleHughes.CIS2118.KPUSim
{
    /// <summary>
    /// An actioncommand that takes a parameter
    /// </summary>
    public class ParameteredActionCommand : ICommand
    {
        private event EventHandler _canExChange;
        private readonly Action<object> _action;
        private readonly Func<object, bool> _canrun;

        public ParameteredActionCommand(Action<object> a, Func<object, bool> c = null)
        {
            this._action = a;
            if (c == null)
                c = x => true;
            this._canrun = c;
        }
        /// <summary>
        /// Whether this command can execute with the given parameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return this._canrun(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                _canExChange += value;
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                _canExChange -= value;
                CommandManager.RequerySuggested -= value;
            }
        }
        /// <summary>
        /// Update the state of whether this can execute
        /// </summary>
        public void NotifyCanExecute()
        {
            if (_canExChange != null)
                App.Current.Dispatcher.Invoke(()=>{
                    _canExChange(this, EventArgs.Empty);   
                });
        }
        /// <summary>
        /// execute this command with the given parameter
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            this._action(parameter);
        }
    }
    /// <summary>
    /// A way of binding a command to a delegate
    /// </summary>
    public class ActionCommand : ICommand
    {
        private event EventHandler _canExChange;
        private readonly Action _action;
        private readonly Func<bool> _canrun;

        public ActionCommand(Action a, Func<bool> c = null)
        {
            if (c == null)
                c = () => true;
            this._action = a;
            this._canrun = c;
        }
        /// <summary>
        /// Whether this command can execute with the given parameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            if (this._canrun == null)
                return true;
            return this._canrun();
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                _canExChange += value;
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                _canExChange -= value;
                CommandManager.RequerySuggested -= value;
            }
        }
        /// <summary>
        /// Update the state of whether this can execute
        /// </summary>
        public void NotifyCanExecute()
        {
            if (_canExChange != null)
                App.Current.Dispatcher.Invoke(()=>{
                    _canExChange(this, EventArgs.Empty);   
                });
                    
        }

        /// <summary>
        /// execute this command with the given parameter
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            this._action();
        }
    }
}