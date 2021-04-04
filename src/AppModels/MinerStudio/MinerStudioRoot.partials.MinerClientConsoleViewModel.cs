using NTMiner.MinerStudio.Vms;
using NTMiner.Vms;
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
                        return string.Empty;
                    }
                    return Timestamp.GetTimeSpanBeforeText(LatestTimestamp);
                }
            }

            public MinerClientConsoleViewModel() {
                if (ClientAppType.IsMinerStudio) {
                    VirtualRoot.BuildEventPath<MinerClientSelectionChangedEvent>("刷新矿机控制台输出", LogEnum.DevConsole, path: message => {
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
                            SendGetConsoleOutLinesMqMessage(isFast: true);
                        }
                    }, this.GetType());
                    VirtualRoot.BuildEventPath<ClientConsoleOutLinesEvent>("将收到的挖矿端控制台消息输出到输出窗口", LogEnum.DevConsole, path: message => {
                        if (this._minerClientVm == null
                            || this._minerClientVm.ClientId != message.ClientId
                            || message.Data == null
                            || message.Data.Count == 0) {
                            return;
                        }
                        lock (_locker) {
                            foreach (var item in message.Data) {
                                _outLines.Add(item);
                                NTMinerConsole.UserLine(item.Line, ConsoleColor.White, withPrefix: false);
                            }
                            // 因为客户端的时间可能不准所以不能使用客户端的时间
                            LatestTimestamp = DateTime.Now;
                        }
                    }, this.GetType());
                    VirtualRoot.BuildEventPath<Per5SecondEvent>("周期获取当前选中的那台矿机的控制台输出", LogEnum.DevConsole, path: message => {
                        SendGetConsoleOutLinesMqMessage(isFast: false);
                    }, this.GetType());
                    VirtualRoot.BuildEventPath<Per1SecondEvent>("客户端控制台输出倒计时秒表", LogEnum.None, path: message => {
                        if (this._minerClientVm == null || this._latestTimestamp == Timestamp.UnixBaseTime) {
                            return;
                        }
                        OnPropertyChanged(nameof(LatestTimeSpanText));
                    }, this.GetType());
                }
            }

            private void SendGetConsoleOutLinesMqMessage(bool isFast) {
                if (this._minerClientVm == null) {
                    return;
                }
                long afterTime = 0;
                var minerClientVm = this._minerClientVm;
                lock (_locker) {
                    var item = _outLines.LastOrDefault();
                    if (item != null) {
                        afterTime = item.Timestamp;
                    }
                }
                if (isFast) {
                    MinerStudioService.FastGetConsoleOutLinesAsync(minerClientVm, afterTime);
                }
                else {
                    MinerStudioService.GetConsoleOutLinesAsync(minerClientVm, afterTime);
                }
            }
        }
    }
}
