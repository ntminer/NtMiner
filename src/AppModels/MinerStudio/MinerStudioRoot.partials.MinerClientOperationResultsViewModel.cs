using NTMiner.MinerStudio.Vms;
using NTMiner.Vms;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace NTMiner.MinerStudio {
    public static partial class MinerStudioRoot {
        public class MinerClientOperationResultsViewModel : ViewModelBase {
            private ObservableCollection<OperationResultViewModel> _vms = new ObservableCollection<OperationResultViewModel>();
            private readonly object _locker = new object();
            private MinerClientViewModel _minerClientVm;
            private const string NO_RECORD_TEXT = "没有群控操作记录";
            private const string LOADING = "加载中";
            private string _noRecordText;
            private DateTime _preClientOperationResultsOn = DateTime.Now;

            private static bool _called = false;

            public MinerClientOperationResultsViewModel() {
                if (_called) {
                    throw new InvalidProgramException("只能调用一次");
                }
                _called = true;
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
                VirtualRoot.AddEventPath<MinerClientSelectionChangedEvent>("矿机列表页选中了和上次选中的不同的矿机时刷新矿机本地群控响应消息列表", LogEnum.DevConsole, action: message => {
                    bool isChanged = true;
                    if (message.MinerClientVm != null && this._minerClientVm != null && this._minerClientVm.ClientId == message.MinerClientVm.ClientId) {
                        isChanged = false;
                    }
                    if (isChanged) {
                        lock (_locker) {
                            _vms.Clear();
                            this._minerClientVm = message.MinerClientVm;
                            if (_minerClientVm != null) {
                                this.NoRecordText = LOADING;
                            }
                            else {
                                this.NoRecordText = "未选中矿机";
                            }
                            OnPropertyChanged(nameof(IsNoRecord));
                        }
                        SendGetOperationResultsMqMessage();
                    }
                }, this.GetType());
                VirtualRoot.AddEventPath<ClientOperationResultsEvent>("收到了挖矿端本地群控响应消息", LogEnum.DevConsole,
                    action: message => {
                        if (this._minerClientVm == null || this._minerClientVm.ClientId != message.ClientId) {
                            return;
                        }
                        _preClientOperationResultsOn = message.BornOn;
                        if (message.Data == null || message.Data.Count == 0) {
                            this.NoRecordText = NO_RECORD_TEXT;
                            return;
                        }
                        UIThread.Execute(() => () => {
                            foreach (var item in message.Data) {
                                _vms.Insert(0, new OperationResultViewModel(item));
                            }
                            OnPropertyChanged(nameof(IsNoRecord));
                        });
                    }, location: this.GetType());
                VirtualRoot.AddEventPath<ClientOperationReceivedEvent>("收到了挖矿端群控响应了群控操作的通知", LogEnum.DevConsole,
                    action: message => {
                        if (_minerClientVm != null && _minerClientVm.ClientId == message.ClientId) {
                            SendGetOperationResultsMqMessage();
                        }
                    }, location: this.GetType());
                VirtualRoot.AddEventPath<Per5SecondEvent>("周期获取当前选中的那台矿机的本地群控响应消息", LogEnum.DevConsole, action: message => {
                    SendGetOperationResultsMqMessage();
                }, this.GetType());
            }

            public ObservableCollection<OperationResultViewModel> ClientOperationResults {
                get {
                    return _vms;
                }
            }

            public bool IsNoRecord {
                get {
                    return _vms.Count == 0;
                }
            }

            public string NoRecordText {
                get => _noRecordText;
                set {
                    if (_noRecordText != value) {
                        _noRecordText = value;
                        OnPropertyChanged(nameof(NoRecordText));
                    }
                }
            }

            private DateTime _preSendMqMessageOn = DateTime.MinValue;
            private MinerClientViewModel _preMinerClientVm;
            private void SendGetOperationResultsMqMessage() {
                if (this._minerClientVm == null) {
                    return;
                }
                foreach (var vm in _vms) {
                    vm.OnPropertyChanged(nameof(OperationResultViewModel.TimestampText));
                }
                if (_preClientOperationResultsOn.AddSeconds(4) < DateTime.Now) {
                    this.NoRecordText = NO_RECORD_TEXT;
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
                MinerStudioService.GetOperationResultsAsync(minerClientVm, afterTime);
            }
        }
    }
}
