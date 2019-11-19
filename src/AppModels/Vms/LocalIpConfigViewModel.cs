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
            foreach (var item in _localIpVms) {
                var data = VirtualRoot.LocalIpSet.AsEnumerable().FirstOrDefault(a => a.SettingID == item.SettingID);
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
