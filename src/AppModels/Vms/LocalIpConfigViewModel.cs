using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class LocalIpConfigViewModel : ViewModelBase {
        private List<LocalIpViewModel> _localIpVms = new List<LocalIpViewModel>();

        public ICommand Save { get; private set; }

        public Action CloseWindow { get; set; }

        public LocalIpConfigViewModel() {
            foreach (var localIp in VirtualRoot.LocalIpSet.AsEnumerable()) {
                _localIpVms.Add(new LocalIpViewModel(localIp));
            }
            this.Save = new DelegateCommand<LocalIpViewModel>((vm) => {
                if (!vm.IsAutoDNSServer) {
                    if (vm.DNSServer0Vm.IsAnyEmpty) {
                        vm.DNSServer0Vm.SetAddress("114.114.114.114");
                    }
                    if (vm.DNSServer1Vm.IsAnyEmpty) {
                        vm.DNSServer1Vm.SetAddress("114.114.114.115");
                    }
                }
                VirtualRoot.Execute(new SetLocalIpCommand(vm, vm.IsAutoDNSServer));
                if (_localIpVms.Count == 1) {
                    CloseWindow?.Invoke();
                }
            }, (vm) => vm.IsChanged);
        }

        public void Refresh() {
            List<LocalIpViewModel> toRemoves = new List<LocalIpViewModel>();
            for (int i = 0; i < _localIpVms.Count; i++) {
                var item = _localIpVms[i];
                var data = VirtualRoot.LocalIpSet.AsEnumerable().FirstOrDefault(a => a.SettingID == item.SettingID);
                if (data != null) {
                    item.Update(data);
                }
                else {
                    toRemoves.Add(item);
                }
            }
            bool isAdded = false;
            foreach (var item in VirtualRoot.LocalIpSet.AsEnumerable()) {
                var exist = _localIpVms.FirstOrDefault(a => a.SettingID == item.SettingID);
                if (exist == null) {
                    _localIpVms.Add(new LocalIpViewModel(item));
                    isAdded = true;
                }
            }
            if (toRemoves.Count != 0) {
                foreach (var item in toRemoves) {
                    _localIpVms.Remove(item);
                }
            }
            if (toRemoves.Count != 0 || isAdded) {
                LocalIpVms = new List<LocalIpViewModel>(_localIpVms);
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
