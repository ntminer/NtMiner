using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class ServerMessagesViewModel : ViewModelBase {
        private ObservableCollection<ServerMessageViewModel> _serverMessageVms;

        public ICommand Add { get; private set; }

        public ServerMessagesViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Add = new DelegateCommand(() => {

            });
            VirtualRoot.BuildEventPath<NewServerMessageLoadedEvent>("从服务器加载了新消息后刷新Vm内存", LogEnum.DevConsole,
                action: message => {
                    foreach (var item in message.Data) {
                        _serverMessageVms.Add(new ServerMessageViewModel(item));
                    }
                });
            _serverMessageVms = new ObservableCollection<ServerMessageViewModel>(VirtualRoot.LocalServerMessageSet.Select(a => new ServerMessageViewModel(a)));
        }

        public ObservableCollection<ServerMessageViewModel> ServerMessageVms {
            get {
                return _serverMessageVms;
            }
        }
    }
}
