using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class LocalIpConfigViewModel : ViewModelBase {
        private List<LocalIpViewModel> _localIpVms = new List<LocalIpViewModel>();

        public LocalIpConfigViewModel() {
            foreach (var localIp in VirtualRoot.LocalIpSet) {
                _localIpVms.Add(new LocalIpViewModel(localIp));
            }
        }

        public void Refresh() {
            foreach (var item in _localIpVms) {
                var data = VirtualRoot.LocalIpSet.FirstOrDefault(a => a.SettingID == item.SettingID);
                if (data != null) {
                    item.Update(data);
                }
            }
        }

        public List<LocalIpViewModel> LocalIpVms {
            get => _localIpVms;
            set {
                _localIpVms = value;
                OnPropertyChanged(nameof(LocalIpVms));
            }
        }
    }
}
