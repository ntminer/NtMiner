using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class MainWindowViewModel : ViewModelBase {
        public class ScanArgs {
            public ScanArgs(Socket socket, string[] ips, string ip) {
                this.Soket = socket;
                this.Ips = ips;
                this.Ip = ip;
            }

            public readonly Socket Soket;
            public readonly string[] Ips;
            public readonly string Ip;
            public readonly ManualResetEvent Set = new ManualResetEvent(false);
            public bool IsTimeouted { get; set; }
        }

        public class IpResult {
            public IpResult(string ip, string selfIp) {
                this.Ip = ip;
                IsSelf = selfIp == ip;
            }

            public string Ip { get; set; }
            public bool IsSelf { get; set; }
        }

        private IpAddressViewModel _fromIpAddressVm;
        private IpAddressViewModel _toIpAddressVm;
        private string _localIps;
        private string _selfIp;
        private readonly ObservableCollection<IpResult> _results = new ObservableCollection<IpResult>();
        private int _percent;
        private int _count = 0;
        private bool _isScanning;
        private int _timeout;

        public ICommand Start { get; private set; }

        public ICommand ShowLocalIps { get; private set; } = new DelegateCommand(() => {
            VirtualRoot.Execute(new ShowLocalIpsCommand());
        });

        public MainWindowViewModel() {
            this.Start = new DelegateCommand(() => {
                if (IsScanning) {
                    IsScanning = false;
                }
                else {
                    if (_fromIpAddressVm.IsAnyEmpty || _toIpAddressVm.IsAnyEmpty) {
                        throw new ValidationException("IP地址不能为空");
                    }
                    if (!IPAddress.TryParse(_fromIpAddressVm.AddressText, out _) || !IPAddress.TryParse(_toIpAddressVm.AddressText, out _)) {
                        throw new ValidationException("IP地址格式不正确");
                    }
                    if (Results.Count != 0) {
                        Results.Clear();
                    }
                    List<string> ipList = Net.IpUtil.CreateIpRange(_fromIpAddressVm.AddressText, _toIpAddressVm.AddressText);
                    Task.Factory.StartNew(() => {
                        Scan(ipList.ToArray());
                    });
                }
            });
            Task.Factory.StartNew(() => {
                var localIp = VirtualRoot.LocalIpSet.AsEnumerable().FirstOrDefault();
                if (localIp != null) {
                    if (!string.IsNullOrEmpty(localIp.DefaultIPGateway)) {
                        string[] parts = localIp.DefaultIPGateway.Split('.');
                        parts[parts.Length - 1] = "1";
                        this.FromIpAddressVm = new IpAddressViewModel(string.Join(".", parts));
                        parts[parts.Length - 1] = "254";
                        this.ToIpAddressVm = new IpAddressViewModel(string.Join(".", parts));
                    }
                }
                LocalIps = GetLocalIps();
            });
        }

        private string GetLocalIps() {
            StringBuilder sb = new StringBuilder();
            int len = sb.Length;
            foreach (var localIp in VirtualRoot.LocalIpSet.AsEnumerable()) {
                if (len != sb.Length) {
                    sb.Append("，");
                }
                else {
                    this._selfIp = localIp.IPAddress;
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
            Parallel.ForEach(ipList, ip => {
                if (!IsScanning) {
                    return;
                }
                IPAddress ipAddress = IPAddress.Parse(ip);
                IPEndPoint endPoint = new IPEndPoint(ipAddress, 3337);
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                var state = new ScanArgs(socket, ipList, ip);
                socket.BeginConnect(endPoint, Callback, state);
                if (!state.Set.WaitOne(Timeout)) {
                    state.IsTimeouted = true;
                    socket.Close();
                }
            });
        }

        private void Callback(IAsyncResult ar) {
            ScanArgs data = ((ScanArgs)ar.AsyncState);
            try {
                if (data.IsTimeouted) {
                    return;
                }
                data.Set.Set();
                if (!IsScanning) {
                    return;
                }
                data.Soket.EndConnect(ar);
                UIThread.Execute(() => () => {
                    _results.Add(new IpResult(data.Ip, this._selfIp));
                });
            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
            }
            finally {
                data.Soket.Close();
                int count = Interlocked.Increment(ref _count);
                Percent = count * 100 / data.Ips.Length;
                if (count == data.Ips.Length) {
                    IsScanning = false;
                }
            }
        }

        public int Timeout {
            get { return _timeout; }
            set {
                _timeout = value;
                OnPropertyChanged(nameof(Timeout));
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

        public ObservableCollection<IpResult> Results {
            get => _results;
        }
    }
}
