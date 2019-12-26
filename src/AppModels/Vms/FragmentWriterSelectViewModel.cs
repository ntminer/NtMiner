using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class FragmentWriterSelectViewModel : ViewModelBase {
        private FragmentWriterViewModel _selectedResult;
        public readonly Action<FragmentWriterViewModel> OnOk;

        public ICommand HideView { get; set; }

        [Obsolete("这是供WPF设计时使用的构造，不应在业务代码中被调用")]
        public FragmentWriterSelectViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException();
            }
        }

        public FragmentWriterSelectViewModel(Action<FragmentWriterViewModel> onOk) {
            OnOk = onOk;
        }

        public FragmentWriterViewModel SelectedResult {
            get => _selectedResult;
            set {
                if (_selectedResult != value) {
                    _selectedResult = value;
                    OnPropertyChanged(nameof(SelectedResult));
                }
            }
        }

        public List<FragmentWriterViewModel> FragmentWriterVms {
            get {
                return AppContext.Instance.FragmentWriterVms.List;
            }
        }
    }
}
