using System;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class ControlCenterHostConfigViewModel : ViewModelBase {
        public ICommand Save { get; private set; }

        public Action CloseWindow { get; set; }

        public ControlCenterHostConfigViewModel() {
            this.Save = new DelegateCommand(() => {
                try {
                    if (string.IsNullOrEmpty(this.ControlCenterHost)) {
                        this.ControlCenterHost = NTMinerRegistry.DefaultControlCenterHost;
                    }
                    CloseWindow?.Invoke();
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e);
                }
            });
        }

        public string ControlCenterHost {
            get {
                return NTMinerRegistry.GetControlCenterHost();
            }
            set {
                if (NTMinerRegistry.GetControlCenterHost() != value) {
                    NTMinerRegistry.SetControlCenterHost(value);
                    OnPropertyChanged(nameof(ControlCenterHost));
                }
            }
        }

        public int ControlCenterPort {
            get {
                return Consts.ControlCenterPort;
            }
        }
    }
}
