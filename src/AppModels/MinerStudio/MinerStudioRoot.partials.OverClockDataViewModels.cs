using NTMiner.MinerStudio.Vms;
using NTMiner.Vms;
using System;
using System.Collections.Generic;

namespace NTMiner.MinerStudio {
    public static partial class MinerStudioRoot {
        public class OverClockDataViewModels : ViewModelBase {
            public static readonly OverClockDataViewModels Instance = new OverClockDataViewModels();
            private readonly Dictionary<Guid, OverClockDataViewModel> _dicById = new Dictionary<Guid, OverClockDataViewModel>();

            private OverClockDataViewModels() {
#if DEBUG
                NTStopwatch.Start();
#endif
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
                Init(refresh: false);
                AppRoot.AddEventPath<OverClockDataSetInitedEvent>("超频建议集初始化后", LogEnum.DevConsole,
                    action: message => {
                        Init(refresh: true);
                    }, location: this.GetType());
                AppRoot.AddEventPath<OverClockDataAddedEvent>("添加超频建议后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (!_dicById.ContainsKey(message.Target.GetId())) {
                            _dicById.Add(message.Target.GetId(), new OverClockDataViewModel(message.Target));
                            if (AppRoot.CoinVms.TryGetCoinVm(message.Target.CoinId, out CoinViewModel coinVm)) {
                                coinVm.OnPropertyChanged(nameof(coinVm.OverClockDatas));
                            }
                        }
                    }, location: this.GetType());
                AppRoot.AddEventPath<OverClockDataUpdatedEvent>("更新超频建议后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (_dicById.TryGetValue(message.Target.GetId(), out OverClockDataViewModel vm)) {
                            vm.Update(message.Target);
                        }
                    }, location: this.GetType());
                AppRoot.AddEventPath<OverClockDataRemovedEvent>("删除超频建议后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicById.Remove(message.Target.GetId());
                        if (AppRoot.CoinVms.TryGetCoinVm(message.Target.CoinId, out CoinViewModel coinVm)) {
                            coinVm.OnPropertyChanged(nameof(coinVm.OverClockDatas));
                        }
                    }, location: this.GetType());
#if DEBUG
                var elapsedMilliseconds = NTStopwatch.Stop();
                if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                    Write.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.ctor");
                }
#endif
            }

            private void Init(bool refresh) {
                _dicById.Clear();
                foreach (var item in NTMinerContext.Instance.OverClockDataSet.AsEnumerable()) {
                    _dicById.Add(item.GetId(), new OverClockDataViewModel(item));
                }
                if (refresh) {
                    foreach (var coinVm in AppRoot.CoinVms.AllCoins) {
                        coinVm.OnPropertyChanged(nameof(coinVm.OverClockDatas));
                    }
                }
            }

            public bool TryGetMineWorkVm(Guid id, out OverClockDataViewModel minerGroupVm) {
                return _dicById.TryGetValue(id, out minerGroupVm);
            }

            public IEnumerable<OverClockDataViewModel> Items {
                get {
                    return _dicById.Values;
                }
            }
        }
    }
}
