using System;
using System.Windows.Input;

namespace NTMiner {
    public class DelegateCommand : ICommand {
        private readonly Func<bool> _canExecute;
        private readonly Action _execute;

        public DelegateCommand(Action execute)
            : this(execute, null) {
        }

        public DelegateCommand(Action execute, Func<bool> canExecute) {
            this._execute = execute;
            this._canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) {
            if (this._canExecute == null) {
                return true;
            }

            return this._canExecute();
        }

        public void Execute(object parameter) {
            this._execute();
        }
    }
}
