using NTMiner.Vms;
using System.Windows;

namespace NTMiner.MinerStudio.Vms {
    public class WsServerNodePageViewModel : ViewModelBase {
        private WebApiServerStateViewModel _webApiServerStateVm;

        public WsServerNodePageViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            Refresh();
        }

        public void Refresh() {
            JsonRpcRoot.OfficialServer.WebApiServerNodeService.GetServerStateAsync((response, e) => {
                if (response.IsSuccess()) {
                    UIThread.Execute(() => {
                        if (_webApiServerStateVm == null) {
                            WebApiServerStateVm = new WebApiServerStateViewModel(response.Data);
                        }
                        else {
                            _webApiServerStateVm.Update(response.Data);
                            OnPropertyChanged(nameof(IsNodeRecordVisible));
                        }
                    });
                }
                else {
                    VirtualRoot.Out.ShowError(response.ReadMessage(e), autoHideSeconds: 4);
                }
            });
        }

        public WebApiServerStateViewModel WebApiServerStateVm {
            get {
                return _webApiServerStateVm;
            }
            set {
                _webApiServerStateVm = value;
                OnPropertyChanged(nameof(WebApiServerStateVm));
                OnPropertyChanged(nameof(IsNodeRecordVisible));
            }
        }

        public Visibility IsNodeRecordVisible {
            get {
                if (_webApiServerStateVm == null || _webApiServerStateVm.WsServerNodes.Count == 0) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }
    }
}
