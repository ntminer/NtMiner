using System.Linq;

namespace NTMiner.Vms {
    public class GroupPageViewModel : ViewModelBase {
        public GroupPageViewModel() {
            if (Design.IsInDesignMode) {
                return;
            }
            this._currentGroup = GroupVms.List.FirstOrDefault();
        }

        private GroupViewModel _currentGroup;
        public GroupViewModel CurrentGroup {
            get { return _currentGroup; }
            set {
                if (_currentGroup != value) {
                    _currentGroup = value;
                    OnPropertyChanged(nameof(CurrentGroup));
                }
            }
        }

        public AppContext.GroupViewModels GroupVms {
            get {
                return AppContext.Instance.GroupVms;
            }
        }
    }
}
