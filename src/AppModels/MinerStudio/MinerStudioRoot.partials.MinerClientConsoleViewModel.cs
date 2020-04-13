using NTMiner.MinerStudio.Vms;
using NTMiner.Vms;
using NTMiner.Ws;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.MinerStudio {
    public static partial class MinerStudioRoot {
        public class MinerClientConsoleViewModel : ViewModelBase {
            private readonly List<ConsoleOutLine> _outLines = new List<ConsoleOutLine>();
            private readonly object _locker = new object();
            private MinerClientViewModel _minerClientVm;
            private DateTime _latestTimestamp = Timestamp.UnixBaseTime;

            public DateTime LatestTimestamp {
                get { return _latestTimestamp; }
                set {
                    if (_latestTimestamp != value) {
                        _latestTimestamp = value;
                        OnPropertyChanged(nameof(LatestTimestamp));
                        OnPropertyChanged(nameof(LatestTimeSpanText));
                    }
                }
            }

            public string LatestTimeSpanText {
                get {
                    if (LatestTimestamp == Timestamp.UnixBaseTime) {
                        return "未知";
                    }
                    return Timestamp.GetTimeSpanText(LatestTimestamp);
                }
            }

            public MinerClientConsoleViewModel() {
                VirtualRoot.AddEventPath<MinerClientSelectionChangedEvent>("矿机列表页选中了和上次选中的不同的矿机时刷新矿机控制台输出", LogEnum.DevConsole, action: message => {
                    bool isChanged = true;
                    if (message.MinerClientVm != null && this._minerClientVm != null && this._minerClientVm.ClientId == message.MinerClientVm.ClientId) {
                        isChanged = false;
                    }
                    LatestTimestamp = Timestamp.UnixBaseTime;
                    if (isChanged) {
                        lock (_locker) {
                            _outLines.Clear();
                            try {
                                Console.Clear();
                            }
                            catch {
                            }
                            this._minerClientVm = message.MinerClientVm;
                        }
                        SendGetConsoleOutLinesMqMessage();
                    }
                }, this.GetType());
                VirtualRoot.AddEventPath<ClientConsoleOutLinesEvent>("收到了挖矿端控制台消息", LogEnum.DevConsole, action: message => {
                    if (this._minerClientVm == null 
                        || this._minerClientVm.ClientId != message.ClientId 
                        || message.Data == null 
                        || message.Data.Count == 0) {
                        return;
                    }
                    lock (_locker) {
                        foreach (var item in message.Data) {
                            _outLines.Add(item);
                            Console.WriteLine(item.Line);
                        }
                        // 因为客户端的时间可能不准所以不能使用客户端的时间
                        LatestTimestamp = DateTime.Now;
                    }
                }, this.GetType());
                VirtualRoot.AddEventPath<Per5SecondEvent>("周期获取当前选中的那台矿机的控制台输出", LogEnum.DevConsole, action: message => {
                    SendGetConsoleOutLinesMqMessage();
                }, this.GetType());
                VirtualRoot.AddEventPath<Per1SecondEvent>("客户端控制台输出倒计时秒表", LogEnum.None, action: message => {
                    if (this._minerClientVm == null || this._latestTimestamp == Timestamp.UnixBaseTime) {
                        return;
                    }
                    OnPropertyChanged(nameof(LatestTimeSpanText));
                }, this.GetType());
            }

            private void SendGetConsoleOutLinesMqMessage() {
                if (this._minerClientVm == null) {
                    return;
                }
                long timestamp = 0;
                var minerClientVm = this._minerClientVm;
                lock (_locker) {
                    var item = _outLines.LastOrDefault();
                    if (item != null) {
                        timestamp = item.Timestamp;
                    }
                }
                if (RpcRoot.IsOuterNet) {
                    WsClient.SendAsync(new WsMessage(Guid.NewGuid(), WsMessage.GetConsoleOutLines) {
                        Data = new WrapperClientIdData {
                            ClientId = minerClientVm.ClientId,
                            Data = timestamp
                        }
                    });
                }
                else {
                    RpcRoot.Client.MinerClientService.GetConsoleOutLinesAsync(minerClientVm.GetLocalIp(), timestamp, (data, e) => {
                        if (data != null && data.Count > 0) {
                            VirtualRoot.RaiseEvent(new ClientConsoleOutLinesEvent(minerClientVm.ClientId, data));
                        }
                    });
                }
            }
        }
    }
}
