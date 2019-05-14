namespace NTMiner.Vms {
    public class AlgoSelectItem : ViewModelBase {
        private SysDicItemViewModel _sysDicItemVm;
        private bool _isChecked;

        public AlgoSelectItem(SysDicItemViewModel sysDicItemVm, bool isChecked) {
            _sysDicItemVm = sysDicItemVm;
            _isChecked = isChecked;
        }

        public SysDicItemViewModel SysDicItemVm {
            get => _sysDicItemVm;
            private set {
                _sysDicItemVm = value;
                OnPropertyChanged(nameof(SysDicItemVm));
            }
        }
        public bool IsChecked {
            get => _isChecked;
            set {
                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }
    }
}
