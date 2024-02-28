using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace PasswordAssist
{
    // ICommand is required by the View to communicate
    // with the command object
    public class RelayCommand : ICommand
    {
        // View calls this method to check whether it should call Execute()
        // if CanExecute returns true, View calls execute
        // if CanExecute return false, UI element to which this command
        // is bound will get disabled
        public bool CanExecute(object parameter)
        {
            if (_canEx != null)
                return _canEx(); // may or may-not execute the command
            else
                return true; // always execute the command
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }

        private Action _ex; // delegate that can hold address of the method
        private Func<bool> _canEx;

        public RelayCommand(Action ex, Func<bool> canEx = null)
        {
            _ex = ex;
            _canEx = canEx;
        }

        // View call this method if CanExecute returns true
        public void Execute(object parameter)
        {
            if (_ex != null)
                _ex(); // call the method whose address it holds
        }
    }
}
