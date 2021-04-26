using NTMiner.ServerNode;
using NTMiner.Vms;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.MinerStudio.Vms {
    public class MqCountsPageViewModel : ViewModelBase {
        private List<MqCountViewModel> _mqCountVms;
        private List<string> _serverNodes;
        private string _selectedServerNode;

        public MqCountsPageViewModel() {
            this._mqCountVms = new List<MqCountViewModel>();
            this._serverNodes = new List<string>();
        }

        public void SetData(MqCountData[] data) {
            if (data == null || data.Length == 0) {
                return;
            }
            _mqCountVms = data.Select(a => new MqCountViewModel(a)).ToList();
            this.ServerNodes = _mqCountVms.Select(a => a.ServerNodeName).ToList();
            if (string.IsNullOrEmpty(this.SelectedServerNode) || !this.ServerNodes.Contains(this.SelectedServerNode)) {
                this.SelectedServerNode = this.ServerNodes.FirstOrDefault();
            }
            else {
                OnPropertyChanged(nameof(CurrentMqCountVm));
            }
        }

        public List<string> ServerNodes {
            get => _serverNodes;
            set {
                if (!IsEquals(_serverNodes, value)) {
                    _serverNodes = value;
                    OnPropertyChanged(nameof(ServerNodes));
                }
            }
        }

        public string SelectedServerNode {
            get => _selectedServerNode;
            set {
                if (_selectedServerNode != value) {
                    _selectedServerNode = value;
                    OnPropertyChanged(nameof(SelectedServerNode));
                    OnPropertyChanged(nameof(CurrentMqCountVm));
                }
            }
        }

        public MqCountViewModel CurrentMqCountVm {
            get {
                return _mqCountVms.FirstOrDefault(a => a.ServerNodeName == this.SelectedServerNode);
            }
        }

        private static bool IsEquals(List<string> left, List<string> right) {
            if (left == right) {
                return true;
            }
            if (left == null || right == null) {
                return false;
            }
            if (left.Count != right.Count) {
                return false;
            }
            return left.All(a => right.Contains(a));
        }
    }
}
