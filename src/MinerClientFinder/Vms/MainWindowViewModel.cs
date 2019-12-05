using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class MainWindowViewModel : ViewModelBase {
        private IpAddressViewModel _fromIpAddressVm;
        private IpAddressViewModel _toIpAddressVm;
        private string _localIps;
        private readonly ObservableCollection<string> _results = new ObservableCollection<string>();
        private int _percent;
        private int _count = 0;
        private bool _isScanning;
        private Thread _thread;

        public ICommand Start { get; private set; }

        public ICommand ShowLocalIps { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowLocalIpsCommand());
        });

        public MainWindowViewModel() {
            this.Start = new DelegateCommand(() => {
                if (IsScanning) {
                    _thread?.Abort();
                    IsScanning = false;
                }
                else {
                    _thread?.Abort();
                    if (!IPAddress.TryParse(_fromIpAddressVm.AddressText, out _) || !IPAddress.TryParse(_toIpAddressVm.AddressText, out _)) {
                        throw new ValidationException("IP地址格式不正确");
                    }
                    if (Results.Count != 0) {
                        Results.Clear();
                    }
                    List<string> ipList = Net.Util.CreateIpRange(_fromIpAddressVm.AddressText, _toIpAddressVm.AddressText);
                    _thread = new Thread(new ThreadStart(() => {
                        Scan(ipList.ToArray());
                    })) {
                        IsBackground = true
                    };
                    _thread.Start();
                }
            });
            var localIp = VirtualRoot.LocalIpSet.AsEnumerable().FirstOrDefault();
            if (localIp != null) {
                if (!string.IsNullOrEmpty(localIp.DefaultIPGateway)) {
                    this._fromIpAddressVm = new IpAddressViewModel(Net.Util.ConvertToIpString(Net.Util.ConvertToIpNum(localIp.DefaultIPGateway) + 1));
                    string[] parts = localIp.DefaultIPGateway.Split('.');
                    parts[parts.Length - 1] = "255";
                    this._toIpAddressVm = new IpAddressViewModel(string.Join(".", parts));
                }
            }
            _localIps = GetLocalIps();
        }

        private string GetLocalIps() {
            StringBuilder sb = new StringBuilder();
            int len = sb.Length;
            foreach (var localIp in VirtualRoot.LocalIpSet.AsEnumerable()) {
                if (len != sb.Length) {
                    sb.Append("，");
                }
                sb.Append(localIp.IPAddress).Append(localIp.DHCPEnabled ? "(动态)" : "🔒");
            }
            return sb.ToString();
        }

        public void RefreshLocalIps() {
            LocalIps = GetLocalIps();
        }

        private void Scan(string[] ipList) {
            if (ipList.Length != 0) {
                IsScanning = true;
            }
            _count = 0;
            Percent = 0;
            foreach (var ip in ipList) {
                Socket socket = null;
                try {
                    IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(ip), 3337);
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    int n = 0;
                    Thread t = new Thread(new ThreadStart(() => {
                        while (true) {
                            Thread.Sleep(200);
                            n++;
                            if (n >= 2) {
                                try {
                                    socket.Close();
                                }
                                catch {
                                }
                            }
                        }
                    })) {
                        IsBackground = true
                    };
                    t.Start();
                    socket.Connect(endPoint);
                    UIThread.Execute(() => {
                        _results.Add(ip);
                    });
                }
                catch {
                }
                finally {
                    socket?.Close();
                    int count = Interlocked.Increment(ref _count);
                    Percent = count * 100 / ipList.Length;
                    if (count == ipList.Length) {
                        IsScanning = false;
                    }
                }
            }
        }

        public string LocalIps {
            get { return _localIps; }
            set {
                _localIps = value;
                OnPropertyChanged(nameof(LocalIps));
            }
        }

        public bool IsScanning {
            get => _isScanning;
            set {
                _isScanning = value;
                OnPropertyChanged(nameof(IsScanning));
                OnPropertyChanged(nameof(BtnStartText));
            }
        }

        public string BtnStartText {
            get {
                if (IsScanning) {
                    return "取消";
                }
                return "开始";
            }
        }

        public int Percent {
            get { return _percent; }
            set {
                _percent = value;
                OnPropertyChanged(nameof(Percent));
            }
        }

        public IpAddressViewModel FromIpAddressVm {
            get => _fromIpAddressVm;
            set {
                _fromIpAddressVm = value;
                OnPropertyChanged(nameof(FromIpAddressVm));
            }
        }

        public IpAddressViewModel ToIpAddressVm {
            get => _toIpAddressVm;
            set {
                _toIpAddressVm = value;
                OnPropertyChanged(nameof(ToIpAddressVm));
            }
        }

        public ObservableCollection<string> Results {
            get => _results;
        }
    }
}
