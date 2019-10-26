using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Input;

namespace NTMiner {
    public partial class AppContext {
        public class NTMinerWalletViewModels : ViewModelBase, IEnumerable<NTMinerWalletViewModel> {
            public static readonly NTMinerWalletViewModels Instance = new NTMinerWalletViewModels();
            private readonly Dictionary<Guid, NTMinerWalletViewModel> _dicById = new Dictionary<Guid, NTMinerWalletViewModel>();

            public ICommand Add { get; private set; }

            private NTMinerWalletViewModels() {
#if DEBUG
                Write.Stopwatch.Restart();
#endif
                if (Design.IsInDesignMode) {
                    return;
                }
                Init(refresh: false);
                AppContextEventPath<NTMinerWalletSetInitedEvent>("NTMiner钱包集初始化后", LogEnum.DevConsole,
                    action: message => {
                        Init(refresh: true);
                    });
                this.Add = new DelegateCommand(() => {
                    new NTMinerWalletViewModel(Guid.NewGuid()).Edit.Execute(FormType.Add);
                });
                AppContextEventPath<NTMinerWalletAddedEvent>("添加NTMiner钱包后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (!_dicById.ContainsKey(message.Source.GetId())) {
                            _dicById.Add(message.Source.GetId(), new NTMinerWalletViewModel(message.Source));
                            CoinViewModel coinVm;
                            if (AppContext.Instance.CoinVms.TryGetCoinVm(message.Source.CoinId, out coinVm)) {
                                coinVm.OnPropertyChanged(nameof(coinVm.NTMinerWallets));
                            }
                        }
                    });
                AppContextEventPath<NTMinerWalletUpdatedEvent>("更新NTMiner钱包后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicById[message.Source.GetId()].Update(message.Source);
                    });
                AppContextEventPath<NTMinerWalletRemovedEvent>("删除NTMiner钱包后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicById.Remove(message.Source.GetId());
                        CoinViewModel coinVm;
                        if (AppContext.Instance.CoinVms.TryGetCoinVm(message.Source.CoinId, out coinVm)) {
                            coinVm.OnPropertyChanged(nameof(coinVm.NTMinerWallets));
                        }
                    });
#if DEBUG
                Write.DevTimeSpan($"耗时{Write.Stopwatch.ElapsedMilliseconds}毫秒 {this.GetType().Name}.ctor");
#endif
            }

            private void Init(bool refresh) {
                _dicById.Clear();
                foreach (var item in NTMinerRoot.Instance.NTMinerWalletSet) {
                    _dicById.Add(item.GetId(), new NTMinerWalletViewModel(item));
                }
                if (refresh) {
                    foreach (var coinVm in AppContext.Instance.CoinVms.AllCoins) {
                        coinVm.OnPropertyChanged(nameof(coinVm.NTMinerWallets));
                    }
                }
            }

            public bool TryGetMineWorkVm(Guid id, out NTMinerWalletViewModel ntMinerWalletVm) {
                return _dicById.TryGetValue(id, out ntMinerWalletVm);
            }

            public IEnumerator<NTMinerWalletViewModel> GetEnumerator() {
                return _dicById.Values.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return _dicById.Values.GetEnumerator();
            }
        }
    }
}
