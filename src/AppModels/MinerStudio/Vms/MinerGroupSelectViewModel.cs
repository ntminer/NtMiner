using NTMiner.Vms;
using System;
using System.Windows.Input;

namespace NTMiner.MinerStudio.Vms {
    public class MinerGroupSelectViewModel : ViewModelBase {
        private MinerGroupViewModel _selectedResult;
        private string _description;
        public readonly Action<MinerGroupViewModel> OnOk;

        public ICommand HideView { get; set; }

        [Obsolete(message: NTKeyword.WpfDesignOnly, error: true)]
        public MinerGroupSelectViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException(NTKeyword.WpfDesignOnly);
            }
        }

        public MinerGroupSelectViewModel(string description, MinerGroupViewModel selected, Action<MinerGroupViewModel> onOk) {
            _description = description;
            _selectedResult = selected;
            OnOk = onOk;
        }

        public string Description {
            get { return _description; }
            set {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public MinerGroupViewModel SelectedResult {
            get => _selectedResult;
            set {
                if (_selectedResult != value) {
                    _selectedResult = value;
                    OnPropertyChanged(nameof(SelectedResult));
                }
            }
        }

        public MinerStudioRoot.MinerGroupViewModels MinerGroupVms {
            get {
                return MinerStudioRoot.MinerGroupVms;
            }
        }
    }
}
