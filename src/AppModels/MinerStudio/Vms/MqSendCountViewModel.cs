using NTMiner.ServerNode;
using NTMiner.Vms;

namespace NTMiner.MinerStudio.Vms {
    public class MqSendCountViewModel : ViewModelBase, IMqSendCount {
        private string _routingKey;
        private long _count;

        public MqSendCountViewModel(IMqSendCount data) {
            _routingKey = data.RoutingKey;
            _count = data.Count;
        }

        public string RoutingKey {
            get => _routingKey;
            set {
                if (_routingKey != value) {
                    _routingKey = value;
                    OnPropertyChanged(nameof(RoutingKey));
                }
            }
        }

        public long Count {
            get => _count;
            set {
                if (_count != value) {
                    _count = value;
                    OnPropertyChanged(nameof(Count));
                }
            }
        }
    }
}
