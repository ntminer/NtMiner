using NTMiner.Vms;
using System.Collections.ObjectModel;
using System.Windows;

namespace NTMiner.MinerStudio.Vms {
    public class WsServerNodePageViewModel : ViewModelBase {
        private readonly ObservableCollection<WsServerNodeViewModel> _wsServerNodeVms = new ObservableCollection<WsServerNodeViewModel>();

        public WsServerNodePageViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            Refresh();
        }

        public void Refresh() {
            RpcRoot.OfficialServer.WsServerNodeService.GetNodesAsync((response, e) => {
                if (response.IsSuccess()) {
                    UIThread.Execute(() => () => {
                        for (int i = 0; i < response.Data.Count; i++) {
                            var item = response.Data[i];
                            if (_wsServerNodeVms.Count > i) {
                                var exist = _wsServerNodeVms[i];
                                if (exist.Address != item.Address) {
                                    _wsServerNodeVms.Insert(i, new WsServerNodeViewModel(item));
                                }
                                else {
                                    exist.Update(item);
                                }
                            }
                            else {
                                _wsServerNodeVms.Add(new WsServerNodeViewModel(item));
                            }
                        }
                        while (_wsServerNodeVms.Count > response.Data.Count) {
                            _wsServerNodeVms.RemoveAt(_wsServerNodeVms.Count - 1);
                        }
                        OnPropertyChanged(nameof(IsNodeRecordVisible));
                    });
                }
                else {
                    VirtualRoot.Out.ShowError(response.ReadMessage(e), autoHideSeconds: 4);
                }
            });
        }

        public ObservableCollection<WsServerNodeViewModel> WsServerNodeVms {
            get {
                return _wsServerNodeVms;
            }
        }

        public Visibility IsNodeRecordVisible {
            get {
                if (WsServerNodeVms.Count == 0) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }
    }
}
