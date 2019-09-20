using NTMiner.MinerClient;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class LocalIpViewModel : ViewModelBase, ILocalIp {
        private string _settingID;
        private string _name;
        private string _iPAddress;
        private string _iPSubnet;
        private string _dHCPServer;
        private bool _dHCPEnabled;
        private string _defaultIPGateway;
        private string _dNSServer0;
        private string _dNSServer1;
        private bool _isAutoDNSServer;
        private IpAddressViewModel _iPAddressVm;
        private IpAddressViewModel _iPSubnetVm;
        private IpAddressViewModel _defaultIPGatewayVm;
        private IpAddressViewModel _dNSServer0Vm;
        private IpAddressViewModel _dNSServer1Vm;

        public ICommand Save { get; private set; }

        public LocalIpViewModel(ILocalIp data) {
            _settingID = data.SettingID;
            _name = data.Name;
            _iPAddress = data.IPAddress;
            _iPSubnet = data.IPSubnet;
            _dHCPServer = data.DHCPServer;
            _dHCPEnabled = data.DHCPEnabled;
            _defaultIPGateway = data.DefaultIPGateway;
            if (data.DefaultIPGateway == data.DNSServer0) {
                _isAutoDNSServer = true;
            }
            else {
                _dNSServer0 = data.DNSServer0;
                _dNSServer1 = data.DNSServer1;
                _isAutoDNSServer = false;
            }
            _iPAddressVm = new IpAddressViewModel(data.IPAddress);
            _iPSubnetVm = new IpAddressViewModel(data.IPSubnet);
            _defaultIPGatewayVm = new IpAddressViewModel(data.DefaultIPGateway);
            _dNSServer0Vm = new IpAddressViewModel(data.DNSServer0);
            _dNSServer1Vm = new IpAddressViewModel(data.DNSServer1);
            this.Save = new DelegateCommand(() => {

            });
        }

        public string SettingID {
            get => _settingID;
            set {
                _settingID = value;
                OnPropertyChanged(nameof(SettingID));
            }
        }

        public string Name {
            get { return _name; }
            set {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string DefaultIPGateway {
            get => _defaultIPGateway;
            set {
                _defaultIPGateway = value;
                OnPropertyChanged(nameof(DefaultIPGateway));
            }
        }

        public IpAddressViewModel DefaultIPGatewayVm {
            get => _defaultIPGatewayVm;
            set {
                _defaultIPGatewayVm = value;
                OnPropertyChanged(nameof(DefaultIPGatewayVm));
            }
        }

        public bool DHCPEnabled {
            get => _dHCPEnabled;
            set {
                if (_dHCPEnabled != value) {
                    _dHCPEnabled = value;
                    OnPropertyChanged(nameof(DHCPEnabled));
                }
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

        public IpAddressViewModel IPAddressVm {
            get => _iPAddressVm;
            set {
                _iPAddressVm = value;
                OnPropertyChanged(nameof(IPAddressVm));
            }
        }

        public string IPSubnet {
            get { return _iPSubnet; }
            set {
                _iPSubnet = value;
                OnPropertyChanged(nameof(IPSubnet));
            }
        }

        public IpAddressViewModel IPSubnetVm {
            get => _iPSubnetVm;
            set {
                _iPSubnetVm = value;
                OnPropertyChanged(nameof(IPSubnetVm));
            }
        }

        public string DNSServer0 {
            get => _dNSServer0;
            set {
                _dNSServer0 = value;
                OnPropertyChanged(nameof(DNSServer0));
            }
        }

        public IpAddressViewModel DNSServer0Vm {
            get => _dNSServer0Vm;
            set {
                _dNSServer0Vm = value;
                OnPropertyChanged(nameof(DNSServer0Vm));
            }
        }

        public string DNSServer1 {
            get => _dNSServer1;
            set {
                _dNSServer1 = value;
                OnPropertyChanged(nameof(DNSServer1));
            }
        }

        public IpAddressViewModel DNSServer1Vm {
            get => _dNSServer1Vm;
            set {
                _dNSServer1Vm = value;
                OnPropertyChanged(nameof(DNSServer1Vm));
            }
        }

        public bool IsAutoDNSServer {
            get => _isAutoDNSServer;
            set {
                if (_isAutoDNSServer != value) {
                    _isAutoDNSServer = value;
                    OnPropertyChanged(nameof(IsAutoDNSServer));
                }
            }
        }
    }
}
