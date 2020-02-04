using NTMiner.Core.MinerClient;

namespace NTMiner.Vms {
    public class LocalIpViewModel : ViewModelBase, ILocalIp {
        private string _settingID;
        private string _macAddress;
        private string _name;
        private bool _dHCPEnabled;
        private bool _isAutoDNSServer;
        private IpAddressViewModel _iPAddressVm;
        private IpAddressViewModel _iPSubnetVm;
        private IpAddressViewModel _defaultIPGatewayVm;
        private IpAddressViewModel _dNSServer0Vm;
        private IpAddressViewModel _dNSServer1Vm;

        private readonly ILocalIp _data;
        private readonly bool _isAutoDNSServerInitial;
        public LocalIpViewModel(ILocalIp data) {
            _data = data;
            _settingID = data.SettingID;
            _macAddress = data.MACAddress;
            _name = data.Name;
            _dHCPEnabled = data.DHCPEnabled;
            if (!data.DHCPEnabled) {
                _isAutoDNSServer = false;
            }
            else {
                _isAutoDNSServer = string.IsNullOrEmpty(data.DNSServer0);
            }
            _isAutoDNSServerInitial = _isAutoDNSServer;
            _iPAddressVm = new IpAddressViewModel(data.IPAddress);
            _iPSubnetVm = new IpAddressViewModel(data.IPSubnet);
            _defaultIPGatewayVm = new IpAddressViewModel(data.DefaultIPGateway);
            _dNSServer0Vm = new IpAddressViewModel(data.DNSServer0);
            _dNSServer1Vm = new IpAddressViewModel(data.DNSServer1);
        }

        public void Update(ILocalIp data) {
            this.Name = data.Name;
            this._macAddress = data.MACAddress;
            _iPAddressVm.SetAddress(data.IPAddress);
            _iPSubnetVm.SetAddress(data.IPSubnet);
            _defaultIPGatewayVm.SetAddress(data.DefaultIPGateway);
            _dNSServer0Vm.SetAddress(data.DNSServer0);
            _dNSServer1Vm.SetAddress(data.DNSServer1);
        }

        public bool IsChanged {
            get {
                if (_dHCPEnabled != _data.DHCPEnabled || _isAutoDNSServer != _isAutoDNSServerInitial) {
                    return true;
                }
                if (_dHCPEnabled == _data.DHCPEnabled) {
                    if (_iPAddressVm.AddressText != _data.IPAddress || _iPSubnetVm.AddressText != _data.IPSubnet || _defaultIPGatewayVm.AddressText != _data.DefaultIPGateway) {
                        return true;
                    }
                }
                if (_isAutoDNSServer == _isAutoDNSServerInitial) {
                    string dns0 = _dNSServer0Vm.AddressText;
                    if (dns0 == "0.0.0.0") {
                        dns0 = string.Empty;
                    }
                    string dns1 = _dNSServer1Vm.AddressText;
                    if (dns1 == "0.0.0.0") {
                        dns1 = string.Empty;
                    }
                    if (dns0 != _data.DNSServer0 || dns1 != _data.DNSServer1) {
                        return true;
                    }
                }
                return false;
            }
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
            get => DefaultIPGatewayVm.AddressText;
        }

        public IpAddressViewModel DefaultIPGatewayVm {
            get => _defaultIPGatewayVm;
            set {
                _defaultIPGatewayVm = value;
                OnPropertyChanged(nameof(DefaultIPGatewayVm));
            }
        }

        public string DHCPEnabledGroupName1 {
            get {
                return this.SettingID + 1;
            }
        }

        public string DHCPEnabledGroupName2 {
            get {
                return this.SettingID + 2;
            }
        }

        public string DHCPEnabledGroupName3 {
            get {
                return this.SettingID + 3;
            }
        }

        public string DHCPEnabledGroupName4 {
            get {
                return this.SettingID + 4;
            }
        }

        public bool DHCPEnabled {
            get => _dHCPEnabled;
            set {
                if (_dHCPEnabled != value) {
                    _dHCPEnabled = value;
                    OnPropertyChanged(nameof(DHCPEnabled));
                    if (!value) {
                        IsAutoDNSServer = false;
                    }
                }
            }
        }

        public string IPAddress {
            get => IPAddressVm.AddressText;
        }

        public string MACAddress {
            get { return _macAddress; }
            set {
                _macAddress = value;
                OnPropertyChanged(nameof(MACAddress));
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
            get { return IPSubnetVm.AddressText; }
        }

        public IpAddressViewModel IPSubnetVm {
            get => _iPSubnetVm;
            set {
                _iPSubnetVm = value;
                OnPropertyChanged(nameof(IPSubnetVm));
            }
        }

        public string DNSServer0 {
            get => DNSServer0Vm.AddressText;
        }

        public IpAddressViewModel DNSServer0Vm {
            get => _dNSServer0Vm;
            set {
                _dNSServer0Vm = value;
                OnPropertyChanged(nameof(DNSServer0Vm));
            }
        }

        public string DNSServer1 {
            get => DNSServer1Vm.AddressText;
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
