using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class KernelInputSelectViewModel : ViewModelBase {
        private KernelInputViewModel _selectedResult;
        public readonly Action<KernelInputViewModel> OnOk;

        public ICommand HideView { get; set; }

        [Obsolete("这是供WPF设计时使用的构造，不应在业务代码中被调用")]
        public KernelInputSelectViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException();
            }
        }

        public KernelInputSelectViewModel(KernelInputViewModel selected, Action<KernelInputViewModel> onOk) {
            _selectedResult = selected;
            OnOk = onOk;
        }

        public KernelInputViewModel SelectedResult {
            get => _selectedResult;
            set {
                if (_selectedResult != value) {
                    _selectedResult = value;
                    OnPropertyChanged(nameof(SelectedResult));
                }
            }
        }

        public List<KernelInputViewModel> PleaseSelectVms {
            get {
                return AppContext.Instance.KernelInputVms.PleaseSelectVms;
            }
        }
    }
}
