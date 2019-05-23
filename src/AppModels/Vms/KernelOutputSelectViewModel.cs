using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class KernelOutputSelectViewModel : ViewModelBase {
        private KernelOutputViewModel _selectedResult;
        public readonly Action<KernelOutputViewModel> OnOk;

        public ICommand HideView { get; set; }

        public KernelOutputSelectViewModel(KernelOutputViewModel selected, Action<KernelOutputViewModel> onOk) {
            _selectedResult = selected;
            OnOk = onOk;
        }

        public KernelOutputViewModel SelectedResult {
            get => _selectedResult;
            set {
                if (_selectedResult != value) {
                    _selectedResult = value;
                    OnPropertyChanged(nameof(SelectedResult));
                }
            }
        }

        public List<KernelOutputViewModel> PleaseSelectVms {
            get {
                return AppContext.Instance.KernelOutputVms.PleaseSelectVms;
            }
        }
    }
}
