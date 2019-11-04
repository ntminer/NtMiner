using System.Collections.ObjectModel;
using System.Linq;

namespace NTMiner.Vms {
    public class ServerMessagesViewModel : ViewModelBase {
        private ObservableCollection<ServerMessageViewModel> _serverMessageVms;

        public ServerMessagesViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            VirtualRoot.BuildEventPath<NewServerMessageLoadedEvent>("从服务器加载了新消息后刷新Vm内存", LogEnum.DevConsole,
                action: message => {
                    foreach (var item in message.Data) {
                        _serverMessageVms.Add(new ServerMessageViewModel(item));
                    }
                });
            _serverMessageVms = new ObservableCollection<ServerMessageViewModel>(VirtualRoot.LocalServerMessageSet.Select(a => new ServerMessageViewModel(a)));
        }
    }
}
