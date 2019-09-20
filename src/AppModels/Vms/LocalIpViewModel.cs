using NTMiner.MinerClient;
using System;

namespace NTMiner.Vms {
    public class LocalIpViewModel : ViewModelBase, ILocalIp {
        private string _iPAddress;
        private string _iPSubnet;
        private string _dHCPServer;
        private bool _dHCPEnabled;
        private string _defaultIPGateway;
        private Guid _settingID;
        private string _dNSServer0;
        private string _dNSServer1;

        public LocalIpViewModel(ILocalIp data) {
            _iPAddress = data.IPAddress;
            _iPSubnet = data.IPSubnet;
            _dHCPServer = data.DHCPServer;
            _dHCPEnabled = data.DHCPEnabled;
            _defaultIPGateway = data.DefaultIPGateway;
            _settingID = data.SettingID;
            _dNSServer0 = data.DNSServer0;
            _dNSServer1 = data.DNSServer1;
        }

        public Guid SettingID {
            get => _settingID;
            set {
                _settingID = value;
                OnPropertyChanged(nameof(SettingID));
            }
        }

        public string DefaultIPGateway {
            get => _defaultIPGateway;
            set {
                _defaultIPGateway = value;
                OnPropertyChanged(nameof(DefaultIPGateway));
            }
        }

        public bool DHCPEnabled {
            get => _dHCPEnabled;
            set {
                _dHCPEnabled = value;
                OnPropertyChanged(nameof(DHCPEnabled));
            }
        }

        public string DHCPServer {
            get => _dHCPServer;
            set {
                _dHCPServer = value;
                OnPropertyChanged(nameof(DHCPServer));
            }
        }

        public string IPAddress {
            get => _iPAddress;
            set {
                _iPAddress = value;
                OnPropertyChanged(nameof(IPAddress));
            }
        }

        public string IPSubnet {
            get { return _iPSubnet; }
            set {
                _iPSubnet = value;
                OnPropertyChanged(nameof(IPSubnet));
            }
        }

        public string DNSServer0 {
            get => _dNSServer0;
            set {
                _dNSServer0 = value;
                OnPropertyChanged(nameof(DNSServer0));
            }
        }

        public string DNSServer1 {
            get => _dNSServer1;
            set {
                _dNSServer1 = value;
                OnPropertyChanged(nameof(DNSServer1));
            }
        }
    }
}
