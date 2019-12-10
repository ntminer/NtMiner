using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace NTMiner {
    public partial class AppContext {
        public class NTMinerWalletViewModels : ViewModelBase {
            public static readonly NTMinerWalletViewModels Instance = new NTMinerWalletViewModels();
            private readonly Dictionary<Guid, NTMinerWalletViewModel> _dicById = new Dictionary<Guid, NTMinerWalletViewModel>();

            public ICommand Add { get; private set; }

            private NTMinerWalletViewModels() {
#if DEBUG
                NTStopwatch.Start();
#endif
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
                Init(refresh: false);
                AddEventPath<NTMinerWalletSetInitedEvent>("NTMiner钱包集初始化后", LogEnum.DevConsole,
                    action: message => {
                        Init(refresh: true);
                    }, location: this.GetType());
                this.Add = new DelegateCommand(() => {
                    new NTMinerWalletViewModel(Guid.NewGuid()).Edit.Execute(FormType.Add);
                });
                AddEventPath<NTMinerWalletAddedEvent>("添加NTMiner钱包后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (!_dicById.ContainsKey(message.Target.GetId())) {
                            _dicById.Add(message.Target.GetId(), new NTMinerWalletViewModel(message.Target));
                            if (AppContext.Instance.CoinVms.TryGetCoinVm(message.Target.CoinId, out CoinViewModel coinVm)) {
                                coinVm.OnPropertyChanged(nameof(coinVm.NTMinerWallets));
                            }
                        }
                    }, location: this.GetType());
                AddEventPath<NTMinerWalletUpdatedEvent>("更新NTMiner钱包后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicById[message.Target.GetId()].Update(message.Target);
                    }, location: this.GetType());
                AddEventPath<NTMinerWalletRemovedEvent>("删除NTMiner钱包后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicById.Remove(message.Target.GetId());
                        if (AppContext.Instance.CoinVms.TryGetCoinVm(message.Target.CoinId, out CoinViewModel coinVm)) {
                            coinVm.OnPropertyChanged(nameof(coinVm.NTMinerWallets));
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
                foreach (var item in NTMinerRoot.Instance.NTMinerWalletSet.AsEnumerable()) {
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

            public IEnumerable<NTMinerWalletViewModel> Items {
                get {
                    return _dicById.Values;
                }
            }
        }
    }
}
