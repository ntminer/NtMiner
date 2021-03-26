using NTMiner.MinerStudio.Vms;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.MinerStudio {
    public static partial class MinerStudioRoot {
        public class NTMinerWalletViewModels : ViewModelBase {
            public static NTMinerWalletViewModels Instance { get; private set; } = new NTMinerWalletViewModels();
            private readonly Dictionary<Guid, NTMinerWalletViewModel> _dicById = new Dictionary<Guid, NTMinerWalletViewModel>();

            public ICommand Add { get; private set; }

            private NTMinerWalletViewModels() {
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
                Init(refresh: false);
                AppRoot.BuildEventPath<NTMinerWalletSetInitedEvent>("NTMiner钱包集初始化后", LogEnum.DevConsole,
                    path: message => {
                        Init(refresh: true);
                    }, location: this.GetType());
                this.Add = new DelegateCommand(() => {
                    new NTMinerWalletViewModel(Guid.NewGuid()).Edit.Execute(FormType.Add);
                });
                AppRoot.BuildEventPath<NTMinerWalletAddedEvent>("添加NTMiner钱包后刷新VM内存", LogEnum.DevConsole,
                    path: message => {
                        if (!_dicById.ContainsKey(message.Source.GetId())) {
                            _dicById.Add(message.Source.GetId(), new NTMinerWalletViewModel(message.Source));
                            if (AppRoot.CoinVms.TryGetCoinVm(message.Source.CoinCode, out CoinViewModel coinVm)) {
                                coinVm.OnPropertyChanged(nameof(coinVm.NTMinerWallets));
                            }
                        }
                    }, location: this.GetType());
                AppRoot.BuildEventPath<NTMinerWalletUpdatedEvent>("更新NTMiner钱包后刷新VM内存", LogEnum.DevConsole,
                    path: message => {
                        if (_dicById.TryGetValue(message.Source.GetId(), out NTMinerWalletViewModel vm)) {
                            vm.Update(message.Source);
                        }
                    }, location: this.GetType());
                AppRoot.BuildEventPath<NTMinerWalletRemovedEvent>("删除NTMiner钱包后刷新VM内存", LogEnum.DevConsole,
                    path: message => {
                        _dicById.Remove(message.Source.GetId());
                        if (AppRoot.CoinVms.TryGetCoinVm(message.Source.CoinCode, out CoinViewModel coinVm)) {
                            coinVm.OnPropertyChanged(nameof(coinVm.NTMinerWallets));
                        }
                    }, location: this.GetType());
            }

            private void Init(bool refresh) {
                _dicById.Clear();
                foreach (var item in NTMinerContext.MinerStudioContext.NTMinerWalletSet.AsEnumerable().ToArray()) {
                    _dicById.Add(item.GetId(), new NTMinerWalletViewModel(item));
                }
                if (refresh) {
                    foreach (var coinVm in AppRoot.CoinVms.AllCoins) {
                        coinVm.OnPropertyChanged(nameof(coinVm.NTMinerWallets));
                    }
                }
            }

            public bool TryGetMineWorkVm(Guid id, out NTMinerWalletViewModel ntminerWalletVm) {
                return _dicById.TryGetValue(id, out ntminerWalletVm);
            }

            public IEnumerable<NTMinerWalletViewModel> Items {
                get {
                    return _dicById.Values;
                }
            }
        }
    }
}
