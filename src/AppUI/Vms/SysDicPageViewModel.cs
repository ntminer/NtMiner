using System.Linq;

namespace NTMiner.Vms {
    public class SysDicPageViewModel : ViewModelBase {
        public SysDicPageViewModel() {
            if (Design.IsInDesignMode) {
                return;
            }
            this._currentSysDic = Vm.Root.SysDicVms.List.FirstOrDefault();
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

        public Vm Vm {
            get {
                return Vm.Instance;
            }
        }
    }
}
