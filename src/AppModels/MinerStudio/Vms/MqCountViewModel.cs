using NTMiner.ServerNode;
using NTMiner.Vms;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.MinerStudio.Vms {
    public class MqCountViewModel : ViewModelBase {
        private List<MqReceivedCountViewModel> _receivedCounts;
        private List<MqSendCountViewModel> _sendCounts;

        public MqCountViewModel(MqCountData data) {
            this.ReceivedCounts = new List<MqReceivedCountViewModel>();
            this.SendCounts = new List<MqSendCountViewModel>();
            foreach (var item in data.ReceivedCounts) {
                this.ReceivedCounts.Add(new MqReceivedCountViewModel(item));
            }
            foreach (var item in data.SendCounts) {
                this.SendCounts.Add(new MqSendCountViewModel(item));
            }
            this.ReceivedCounts.Sort((l, r) => {
                return (int)r.Count - (int)l.Count;
            });
            this.SendCounts.Sort((l, r) => {
                return (int)r.Count - (int)l.Count;
            });
        }

        public string ServerNodeName {
            get {
                if (ReceivedCounts == null || ReceivedCounts.Count == 0) {
                    return string.Empty;
                }
                string queue = ReceivedCounts[0].Queue;
                string[] parts = queue.Split('.');
                if (parts.Length >= 5) {
                    return string.Join(".", parts.Take(5));
                }
                return string.Empty;
            }
        }

        public List<MqReceivedCountViewModel> ReceivedCounts {
            get => _receivedCounts;
            set {
                _receivedCounts = value;
                OnPropertyChanged(nameof(ReceivedCounts));
            }
        }

        public List<MqSendCountViewModel> SendCounts {
            get => _sendCounts;
            set {
                _sendCounts = value;
                OnPropertyChanged(nameof(SendCounts));
            }
        }
    }
}
