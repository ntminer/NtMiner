using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class MainWindowViewModel : ViewModelBase {
        private string _fromIp;
        private string _toIp;
        private readonly ObservableCollection<string> _results = new ObservableCollection<string>();
        private int _percent;

        public ICommand StartOrStop { get; private set; }

        public MainWindowViewModel() {
            this.StartOrStop = new DelegateCommand(() => {
                List<string> ipList = new List<string>();
                for (long i = Ip.Util.GetIpNum(FromIp); i <= Ip.Util.GetIpNum(ToIp); i++) {
                    ipList.Add(Ip.Util.GetIpString(i));
                }
                if (Results.Count != 0) {
                    Results.Clear();
                }
                Scan(ipList.ToArray());
            });
            var localIp = VirtualRoot.LocalIpSet.FirstOrDefault();
            if (localIp != null) {
                this._fromIp = localIp.DefaultIPGateway;
                if (!string.IsNullOrEmpty(_fromIp)) {
                    _fromIp = Ip.Util.GetIpString(Ip.Util.GetIpNum(_fromIp) + 1);
                    string[] parts = _fromIp.Split('.');
                    parts[parts.Length - 1] = "255";
                    this._toIp = string.Join(".", parts);
                }
            }
        }

        private int _count = 0;
        private void Scan(string[] ipList) {
            _count = 0;
            Percent = 0;
            foreach (var ip in ipList) {
                Task.Factory.StartNew(() => {
                    try {
                        using (TcpClient client = new TcpClient(ip, 3337)) {
                            if (client.Connected) {
                                UIThread.Execute(() => {
                                    _results.Add(ip);
                                });
                            }
                        }
                    }
                    catch (Exception e) {
                    }
                    finally {
                        Interlocked.Increment(ref _count);
                        Percent = _count * 100 / ipList.Length;
                    }
                });
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
            }
        }
        public string ToIp {
            get => _toIp;
            set {
                _toIp = value;
                OnPropertyChanged(nameof(ToIp));
            }
        }

        public ObservableCollection<string> Results {
            get => _results;
        }
    }
}
