using NTMiner.ServerNode;
using NTMiner.Vms;

namespace NTMiner.MinerStudio.Vms {
    public class MqReceivedCountViewModel : ViewModelBase, IMqReceivedCount {
        private string _queue;
        private string _routingKey;
        private long _count;

        public MqReceivedCountViewModel(IMqReceivedCount data) {
            _queue = data.Queue;
            _routingKey = data.RoutingKey;
            _count = data.Count;
        }

        public string Queue {
            get => _queue;
            set {
                if (_queue != value) {
                    _queue = value;
                    OnPropertyChanged(nameof(Queue));
                }
            }
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
