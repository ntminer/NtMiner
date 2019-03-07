using NTMiner.Core.Kernels;
using System;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class EnvironmentVariableEditViewModel : ViewModelBase {
        private string _key;
        private string _value;

        public ICommand Save { get; private set; }
        public Action CloseWindow { get; set; }

        public EnvironmentVariableEditViewModel(CoinKernelViewModel coinKernelViewModel, EnvironmentVariable environmentVariable) {
            _key = environmentVariable.Key;
            _value = environmentVariable.Value;
            this.Save = new DelegateCommand(() => {
                environmentVariable.Key = this.Key;
                environmentVariable.Value = this.Value;
                if (!coinKernelViewModel.EnvironmentVariables.Contains(environmentVariable)) {
                    coinKernelViewModel.EnvironmentVariables.Add(environmentVariable);
                }
                coinKernelViewModel.EnvironmentVariables = coinKernelViewModel.EnvironmentVariables.ToList();
                CloseWindow?.Invoke();
            });
        }

        public string Key {
            get { return _key; }
            set {
                _key = value;
                OnPropertyChanged(nameof(Key));
            }
        }

        public string Value {
            get { return _value; }
            set {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }
    }
}
