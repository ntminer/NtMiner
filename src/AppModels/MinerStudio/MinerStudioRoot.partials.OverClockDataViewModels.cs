using NTMiner.MinerStudio.Vms;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.MinerStudio {
    public static partial class MinerStudioRoot {
        public class OverClockDataViewModels : ViewModelBase {
            public static OverClockDataViewModels Instance { get; private set; } = new OverClockDataViewModels();
            private readonly Dictionary<Guid, OverClockDataViewModel> _dicById = new Dictionary<Guid, OverClockDataViewModel>();

            private OverClockDataViewModels() {
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
                Init(refresh: false);
                AppRoot.BuildEventPath<OverClockDataSetInitedEvent>("超频菜谱集初始化后", LogEnum.DevConsole,
                    path: message => {
                        Init(refresh: true);
                    }, location: this.GetType());
                AppRoot.BuildEventPath<OverClockDataAddedEvent>("添加超频菜谱后刷新VM内存", LogEnum.DevConsole,
                    path: message => {
                        if (!_dicById.ContainsKey(message.Source.GetId())) {
                            _dicById.Add(message.Source.GetId(), new OverClockDataViewModel(message.Source));
                            if (AppRoot.CoinVms.TryGetCoinVm(message.Source.CoinId, out CoinViewModel coinVm)) {
                                coinVm.OnPropertyChanged(nameof(coinVm.OverClockDatas));
                            }
                        }
                    }, location: this.GetType());
                AppRoot.BuildEventPath<OverClockDataUpdatedEvent>("更新超频菜谱后刷新VM内存", LogEnum.DevConsole,
                    path: message => {
                        if (_dicById.TryGetValue(message.Source.GetId(), out OverClockDataViewModel vm)) {
                            vm.Update(message.Source);
                        }
                    }, location: this.GetType());
                AppRoot.BuildEventPath<OverClockDataRemovedEvent>("删除超频菜谱后刷新VM内存", LogEnum.DevConsole,
                    path: message => {
                        _dicById.Remove(message.Source.GetId());
                        if (AppRoot.CoinVms.TryGetCoinVm(message.Source.CoinId, out CoinViewModel coinVm)) {
                            coinVm.OnPropertyChanged(nameof(coinVm.OverClockDatas));
                        }
                    }, location: this.GetType());
            }

            private void Init(bool refresh) {
                _dicById.Clear();
                foreach (var item in NTMinerContext.Instance.OverClockDataSet.AsEnumerable().ToArray()) {
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
