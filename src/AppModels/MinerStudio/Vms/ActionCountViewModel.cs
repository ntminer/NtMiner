using NTMiner.ServerNode;
using NTMiner.Vms;

namespace NTMiner.MinerStudio.Vms {
    public class ActionCountViewModel : ViewModelBase, IActionCount {
        private string _actionName;
        private int _count;

        public ActionCountViewModel(IActionCount data) {
            _actionName = data.ActionName;
            _count = data.Count;
        }

        public string ActionName {
            get => _actionName;
            set {
                if (_actionName != value) {
                    _actionName = value;
                    OnPropertyChanged(nameof(ActionName));
                }
            }
        }

        public int Count {
            get => _count;
            set {
                if (_count != value) {
                    _count = value;
                    OnPropertyChanged(nameof(Count));
                }
            }
        }
    }
}
