using System.Linq;

namespace NTMiner.Vms {
    public class GroupPageViewModel : ViewModelBase {
        public static readonly GroupPageViewModel Current = new GroupPageViewModel();

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

        public GroupViewModels GroupVms {
            get {
                return GroupViewModels.Current;
            }
        }
    }
}
