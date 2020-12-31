using System;
using System.Windows.Input;

namespace NTMiner {
    public class DelegateCommand<T> : ICommand {
        private readonly Func<T, bool> _canExecute;
        private readonly Action<T> _execute;

        public DelegateCommand(Action<T> execute)
            : this(execute, null) {
        }

        public DelegateCommand(Action<T> execute, Func<T, bool> canExecute) {
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

            return this._canExecute((T)parameter);
        }

        public void Execute(object parameter) {
            this._execute((T)parameter);
        }
    }
}
