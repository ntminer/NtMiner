using NTMiner.Views.Ucs;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class ControlCenterWindowViewModel : ViewModelBase {
        private string _title = "开源矿工中控客户端";

        public ICommand ConfigMinerServerHost { get; private set; }

        public ControlCenterWindowViewModel() {
            this.ConfigMinerServerHost = new DelegateCommand(() => {
                MinerServerHostConfig.ShowWindow();
            });
        }

        public string Title {
            get {
                return _title;
            }
            set {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public LangViewModels LangVms {
            get {
                return LangViewModels.Current;
            }
        }
    }
}
