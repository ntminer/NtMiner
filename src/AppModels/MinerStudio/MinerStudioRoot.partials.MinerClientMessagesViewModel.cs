using NTMiner.MinerStudio.Vms;
using NTMiner.Vms;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace NTMiner.MinerStudio {
    public static partial class MinerStudioRoot {
        public class MinerClientMessagesViewModel : ViewModelBase {
            private ObservableCollection<LocalMessageDtoViewModel> _vms = new ObservableCollection<LocalMessageDtoViewModel>();
            private readonly object _locker = new object();
            private MinerClientViewModel _minerClientVm;

            private static bool _called = false;

            public MinerClientMessagesViewModel() {
                if (_called) {
                    throw new InvalidProgramException("只能调用一次");
                }
                _called = true;
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
                if (ClientAppType.IsMinerStudio) {
                    VirtualRoot.BuildEventPath<MinerClientSelectionChangedEvent>("刷新矿机消息列表", LogEnum.DevConsole, path: message => {
                        bool isChanged = true;
                        if (message.MinerClientVm != null && this._minerClientVm != null && this._minerClientVm.ClientId == message.MinerClientVm.ClientId) {
                            isChanged = false;
                        }
                        if (isChanged) {
                            lock (_locker) {
                                _vms.Clear();
                                this._minerClientVm = message.MinerClientVm;
                                OnPropertyChanged(nameof(IsNoRecord));
                            }
                            SendGetLocalMessagesMqMessage();
                        }
                    }, this.GetType());
                    VirtualRoot.BuildEventPath<ClientLocalMessagesEvent>("将收到的挖矿端本地消息展示到消息列表", LogEnum.DevConsole,
                        path: message => {
                            if (this._minerClientVm == null || this._minerClientVm.ClientId != message.ClientId) {
                                return;
                            }
                            if (message.Data == null || message.Data.Count == 0) {
                                return;
                            }
                            UIThread.Execute(() => {
                                foreach (var item in message.Data) {
                                    _vms.Insert(0, new LocalMessageDtoViewModel(item));
                                }
                                OnPropertyChanged(nameof(IsNoRecord));
                            });
                        }, location: this.GetType());
                    VirtualRoot.BuildEventPath<Per5SecondEvent>("周期获取当前选中的那台矿机的本地消息", LogEnum.DevConsole, path: message => {
                        SendGetLocalMessagesMqMessage();
                    }, this.GetType());
                }
            }

            public ObservableCollection<LocalMessageDtoViewModel> ClientLocalMessages {
                get {
                    return _vms;
                }
            }

            public bool IsNoRecord {
                get {
                    return _vms.Count == 0;
                }
            }

            private DateTime _preSendMqMessageOn = DateTime.MinValue;
            private MinerClientViewModel _preMinerClientVm;
            public void SendGetLocalMessagesMqMessage() {
                if (this._minerClientVm == null || !IsMinerClientMessagesVisible) {
                    return;
                }
                foreach (var vm in _vms) {
                    vm.OnPropertyChanged(nameof(LocalMessageDtoViewModel.TimestampText));
                }
                if (_preSendMqMessageOn.AddSeconds(4) > DateTime.Now && _preMinerClientVm == _minerClientVm) {
                    return;
                }
                _preSendMqMessageOn = DateTime.Now;
                _preMinerClientVm = _minerClientVm;
                long afterTime = 0;
                var minerClientVm = this._minerClientVm;
                lock (_locker) {
                    var item = _vms.FirstOrDefault();
                    if (item != null) {
                        afterTime = item.Timestamp;
                    }
                }
                MinerStudioService.GetLocalMessagesAsync(minerClientVm, afterTime);
            }
        }
    }
}
