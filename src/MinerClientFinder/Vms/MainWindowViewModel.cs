using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class MainWindowViewModel : ViewModelBase {
        private string _fromIp;
        private string _toIp;
        private readonly ObservableCollection<string> _results = new ObservableCollection<string>();
        private int _percent;
        private int _count = 0;
        private bool _isScanning;
        private Thread _thread;

        public ICommand Start { get; private set; }

        public MainWindowViewModel() {
            this.Start = new DelegateCommand(() => {
                if (IsScanning) {
                    _thread?.Abort();
                    IsScanning = false;
                }
                else {
                    _thread?.Abort();
                    if (!IPAddress.TryParse(FromIp, out _) || !IPAddress.TryParse(ToIp, out _)) {
                        throw new ValidationException("IP地址格式不正确");
                    }
                    if (Results.Count != 0) {
                        Results.Clear();
                    }
                    List<string> ipList = Net.Util.CreateIpRange(FromIp, ToIp);
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
                this._fromIp = localIp.DefaultIPGateway;
                if (!string.IsNullOrEmpty(_fromIp)) {
                    _fromIp = Net.Util.ConvertToIpString(Net.Util.ConvertToIpNum(_fromIp) + 1);
                    string[] parts = _fromIp.Split('.');
                    parts[parts.Length - 1] = "255";
                    this._toIp = string.Join(".", parts);
                }
            }
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
                            Thread.Sleep(100);
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

        public string FromIp {
            get => _fromIp;
            set {
                _fromIp = value;
                OnPropertyChanged(nameof(FromIp));
                if (!IPAddress.TryParse(value, out _)) {
                    throw new ValidationException("IP地址格式错误");
                }
            }
        }
        public string ToIp {
            get => _toIp;
            set {
                _toIp = value;
                OnPropertyChanged(nameof(ToIp));
                if (!IPAddress.TryParse(value, out _)) {
                    throw new ValidationException("IP地址格式错误");
                }
            }
        }

        public ObservableCollection<string> Results {
            get => _results;
        }
    }
}
