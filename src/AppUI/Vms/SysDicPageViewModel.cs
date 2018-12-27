using System.Linq;

namespace NTMiner.Vms {
    public class SysDicPageViewModel : ViewModelBase {
        public SysDicPageViewModel() {
            if (NTMinerRoot.IsInDesignMode) {
                return;
            }
            this._currentSysDic = SysDicVms.List.FirstOrDefault();
        }

        private SysDicViewModel _currentSysDic;
        public SysDicViewModel CurrentSysDic {
            get { return _currentSysDic; }
            set {
                _currentSysDic = value;
                OnPropertyChanged(nameof(CurrentSysDic));
            }
        }

        public SysDicViewModels SysDicVms {
            get {
                return SysDicViewModels.Current;
            }
        }
    }
}
