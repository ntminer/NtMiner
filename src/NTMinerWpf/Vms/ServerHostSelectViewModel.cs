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
            var data = NTMinerRegistry.GetControlCenterAddresses().ToList();
            if (!data.Contains("127.0.0.1") && !data.Contains(NTKeyword.Localhost)) {
                data.Add("127.0.0.1");
            }
            if (!data.Contains("server.ntminer.com:3339")) {
                data.Add("server.ntminer.com:3339");
            }
            _serverHosts = data.Select(a => new ServerHostItem(a)).ToList();
            _selectedResult = _serverHosts.FirstOrDefault(a => a.IpOrHost == selected);
            OnOk = onOk;
            this.Remove = new DelegateCommand<ServerHostItem>((serverHost) => {
                if (this.ServerHosts.Remove(serverHost)) {
                    if (NTMinerRegistry.GetControlCenterAddress() == serverHost.IpOrHost) {
                        NTMinerRegistry.SetControlCenterAddress(string.Empty);
                    }
                    this.ServerHosts = this.ServerHosts.ToList();
                }
            });
        }

        public List<ServerHostItem> ServerHosts {
            get {
                return _serverHosts;
            }
            set {
                _serverHosts = value;
                NTMinerRegistry.SetControlCenterAddresses(value.Select(a => a.IpOrHost).ToList());
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
