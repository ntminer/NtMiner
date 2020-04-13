using System.Linq;

namespace NTMiner.Vms {
    public class SysDicPageViewModel : ViewModelBase {
        public SysDicPageViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this._currentSysDic = SysDicVms.List.FirstOrDefault();
        }

        private SysDicViewModel _currentSysDic;
        public SysDicViewModel CurrentSysDic {
            get { return _currentSysDic; }
            set {
                if (_currentSysDic != value) {
                    _currentSysDic = value;
                    OnPropertyChanged(nameof(CurrentSysDic));
                }
            }
        }

        private SysDicItemViewModel _currentSysDicItem;
        public SysDicItemViewModel CurrentSysDicItem {
            get { return _currentSysDicItem; }
            set {
                _currentSysDicItem = value;
                OnPropertyChanged(nameof(CurrentSysDicItem));
            }
        }

        public AppRoot.SysDicViewModels SysDicVms {
            get {
                return AppRoot.SysDicVms;
            }
        }
    }
}
