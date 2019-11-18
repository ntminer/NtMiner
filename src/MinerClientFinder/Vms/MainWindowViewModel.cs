using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class MainWindowViewModel : ViewModelBase {
        public class ResultItem : ViewModelBase {
            private string _ip;
            private bool _isOnline;

            public ResultItem(string ip) {
                _ip = ip;
            }

            public string Ip {
                get => _ip;
                set {
                    _ip = value;
                    OnPropertyChanged(nameof(Ip));
                }
            }

            public bool IsOnline {
                get => _isOnline;
                set {
                    _isOnline = value;
                    OnPropertyChanged(nameof(IsOnline));
                }
            }
        }

        private string _fromIp;
        private string _toIp;
        private List<ResultItem> _results;

        public ICommand Start { get; private set; }

        public MainWindowViewModel() {
            this.Start = new DelegateCommand(() => {
                List<ResultItem> items = new List<ResultItem>();
                for (long i = Ip.Util.GetIpNum(FromIp); i <= Ip.Util.GetIpNum(ToIp); i++) {
                    items.Add(new ResultItem(Ip.Util.GetIpString(i)));
                }
                this.Results = items;
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
            this._results = new List<ResultItem>();
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

        public List<ResultItem> Results {
            get => _results;
            set {
                _results = value;
                OnPropertyChanged(nameof(Results));
            }
        }
    }
}
