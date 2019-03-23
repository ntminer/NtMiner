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
                    Server.ControlCenterHost = this.ControlCenterHost;
                    CloseWindow?.Invoke();
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                }
            });
            _controlCenterHost = Server.ControlCenterHost;
        }

        private string _controlCenterHost;
        public string ControlCenterHost {
            get {
                return _controlCenterHost;
            }
            set {
                if (_controlCenterHost != value) {
                    _controlCenterHost = value;
                    OnPropertyChanged(nameof(ControlCenterHost));
                }
            }
        }

        public int ControlCenterPort {
            get {
                return WebApiConst.ControlCenterPort;
            }
        }
    }
}
