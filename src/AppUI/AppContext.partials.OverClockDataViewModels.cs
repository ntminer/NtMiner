using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public partial class AppContext {
        public class OverClockDataViewModels : ViewModelBase, IEnumerable<OverClockDataViewModel> {
            private readonly Dictionary<Guid, OverClockDataViewModel> _dicById = new Dictionary<Guid, OverClockDataViewModel>();

            public OverClockDataViewModels() {
                if (Design.IsInDesignMode) {
                    return;
                }
                Init(refresh: false);
                VirtualRoot.On<OverClockDataSetInitedEvent>("超频建议集初始化后", LogEnum.DevConsole,
                    action: message => {
                        Init(refresh: true);
                    });
                VirtualRoot.On<OverClockDataAddedEvent>("添加超频建议后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (!_dicById.ContainsKey(message.Source.GetId())) {
                            _dicById.Add(message.Source.GetId(), new OverClockDataViewModel(message.Source));
                            OnPropertyChanged(nameof(List));
                            CoinViewModel coinVm;
                            if (Current.CoinVms.TryGetCoinVm(message.Source.CoinId, out coinVm)) {
                                coinVm.OnPropertyChanged(nameof(coinVm.OverClockDatas));
                            }
                        }
                    });
                VirtualRoot.On<OverClockDataUpdatedEvent>("更新超频建议后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicById[message.Source.GetId()].Update(message.Source);
                    });
                VirtualRoot.On<OverClockDataRemovedEvent>("删除超频建议后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicById.Remove(message.Source.GetId());
                        OnPropertyChanged(nameof(List));
                        CoinViewModel coinVm;
                        if (Current.CoinVms.TryGetCoinVm(message.Source.CoinId, out coinVm)) {
                            coinVm.OnPropertyChanged(nameof(coinVm.OverClockDatas));
                        }
                    });
            }

            private void Init(bool refresh) {
                _dicById.Clear();
                foreach (var item in NTMinerRoot.Current.OverClockDataSet) {
                    _dicById.Add(item.GetId(), new OverClockDataViewModel(item));
                }
                if (refresh) {
                    OnPropertyChanged(nameof(List));
                    foreach (var coinVm in Current.CoinVms.AllCoins) {
                        coinVm.OnPropertyChanged(nameof(coinVm.OverClockDatas));
                    }
                }
            }

            public List<OverClockDataViewModel> List {
                get {
                    return _dicById.Values.ToList();
                }
            }

            public bool TryGetMineWorkVm(Guid id, out OverClockDataViewModel minerGroupVm) {
                return _dicById.TryGetValue(id, out minerGroupVm);
            }

            public IEnumerator<OverClockDataViewModel> GetEnumerator() {
                return _dicById.Values.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return _dicById.Values.GetEnumerator();
            }
        }
    }
}
