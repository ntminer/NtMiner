using NTMiner.Vms;
using System;
using System.Windows.Input;

namespace NTMiner.MinerStudio.Vms {
    public class ColumnsShowSelectViewModel : ViewModelBase {
        private ColumnsShowViewModel _selectedResult;
        public readonly Action<ColumnsShowViewModel> OnOk;

        public ICommand HideView { get; set; }

        [Obsolete(message: NTKeyword.WpfDesignOnly, error: true)]
        public ColumnsShowSelectViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException(NTKeyword.WpfDesignOnly);
            }
        }

        public ColumnsShowSelectViewModel(ColumnsShowViewModel selected, Action<ColumnsShowViewModel> onOk) {
            _selectedResult = selected;
            OnOk = onOk;
        }

        public ColumnsShowViewModel SelectedResult {
            get => _selectedResult;
            set {
                if (_selectedResult != value) {
                    _selectedResult = value;
                    OnPropertyChanged(nameof(SelectedResult));
                }
            }
        }

        public MinerStudioRoot.ColumnsShowViewModels ColumnsShowVms {
            get {
                return MinerStudioRoot.ColumnsShowVms;
            }
        }
    }
}
