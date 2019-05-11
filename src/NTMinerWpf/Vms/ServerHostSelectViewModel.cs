using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class ServerHostSelectViewModel : ViewModelBase {
        private ServerHostItem _selectedResult;
        public readonly Action<ServerHostItem> OnOk;
        private List<ServerHostItem> _serverHosts;
        public ICommand HideView { get; set; }
        public ICommand Remove { get; private set; }

        public ServerHostSelectViewModel(string selected, Action<ServerHostItem> onOk) {
            var data = NTMinerRegistry.GetControlCenterHosts().ToList();
            if (!data.Contains("127.0.0.1") && !data.Contains("localhost")) {
                data.Add("127.0.0.1");
            }
            _serverHosts = data.Select(a => new ServerHostItem(a)).ToList();
            _selectedResult = _serverHosts.FirstOrDefault(a => a.IpOrHost == selected);
            OnOk = onOk;
            this.Remove = new DelegateCommand<ServerHostItem>((serverHost) => {
                var list = this.ServerHosts;
                if (list.Remove(serverHost)) {
                    this.ServerHosts = list.ToList();
                }
            });
        }

        public List<ServerHostItem> ServerHosts {
            get {
                return _serverHosts;
            }
            set {
                _serverHosts = value;
                NTMinerRegistry.SetControlCenterHosts(value.Select(a => a.IpOrHost).ToList());
                OnPropertyChanged(nameof(ServerHosts));
            }
        }

        public ServerHostItem SelectedResult {
            get => _selectedResult;
            set {
                _selectedResult = value;
                OnPropertyChanged(nameof(SelectedResult));
            }
        }
    }
}
