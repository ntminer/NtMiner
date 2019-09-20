using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class LocalIpConfigViewModel : ViewModelBase {
        private List<LocalIpViewModel> _localIpVms;

        public ICommand Save { get; private set; }

        public Action CloseWindow { get; set; }

        public LocalIpConfigViewModel() {
            _localIpVms = new List<LocalIpViewModel>();
            foreach (var localIp in VirtualRoot.LocalIpSet) {
                _localIpVms.Add(new LocalIpViewModel(localIp));
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
