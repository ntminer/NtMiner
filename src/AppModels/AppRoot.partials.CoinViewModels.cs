using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NTMiner {
    public static partial class AppRoot {
        public class CoinViewModels : ViewModelBase {
            public static CoinViewModels Instance { get; private set; } = new CoinViewModels();

            private readonly Dictionary<Guid, CoinViewModel> _dicById = new Dictionary<Guid, CoinViewModel>();

            private CoinViewModels() {
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
                VirtualRoot.BuildEventPath<ServerContextReInitedEvent>("刷新VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        _dicById.Clear();
                        Init();
                    });
                VirtualRoot.BuildEventPath<ServerContextReInitedEvent>("刷新视图界面", LogEnum.DevConsole, location: this.GetType(), PathPriority.BelowNormal,
                    path: message => {
                        AllPropertyChanged();
                    });
                BuildEventPath<CoinAddedEvent>("刷新VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: (message) => {
                        _dicById.Add(message.Source.GetId(), new CoinViewModel(message.Source));
                        AllPropertyChanged();
                    });
                BuildEventPath<CoinRemovedEvent>("刷新VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        _dicById.Remove(message.Source.GetId());
                        AllPropertyChanged();
                    });
                BuildEventPath<CoinKernelAddedEvent>("刷新币种Vm集的关联内存", LogEnum.DevConsole, this.GetType(), PathPriority.BelowNormal, path: message => {
                    if (_dicById.TryGetValue(message.Source.CoinId, out CoinViewModel coinVm)) {
                        coinVm.OnPropertyChanged(nameof(CoinViewModel.CoinKernel));
                        coinVm.OnPropertyChanged(nameof(CoinViewModel.CoinKernels));
                        coinVm.OnPropertyChanged(nameof(CoinViewModel.IsSupported));
                    }
                });
                BuildEventPath<CoinKernelRemovedEvent>("刷新币种Vm集的关联内存", LogEnum.DevConsole, this.GetType(), PathPriority.BelowNormal, path: message => {
                    if (_dicById.TryGetValue(message.Source.CoinId, out CoinViewModel coinVm)) {
                        coinVm.OnPropertyChanged(nameof(CoinViewModel.CoinKernel));
                        coinVm.OnPropertyChanged(nameof(CoinViewModel.CoinKernels));
                        coinVm.OnPropertyChanged(nameof(CoinViewModel.IsSupported));
                    }
                });
                BuildEventPath<CoinUpdatedEvent>("刷新VM内存", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        if (_dicById.TryGetValue(message.Source.GetId(), out CoinViewModel vm)) {
                            bool justAsDualCoin = vm.JustAsDualCoin;
                            vm.Update(message.Source);
                            vm.TestWalletVm.Address = message.Source.TestWallet;
                            vm.OnPropertyChanged(nameof(vm.Wallets));
                            vm.OnPropertyChanged(nameof(vm.WalletItems));
                            if (MinerProfileVm.CoinId == message.Source.GetId()) {
                                MinerProfileVm.OnPropertyChanged(nameof(MinerProfileVm.CoinVm));
                            }
                            CoinKernelViewModel coinKernelVm = MinerProfileVm.CoinVm.CoinKernel;
                            if (coinKernelVm != null
                                && coinKernelVm.CoinKernelProfile.SelectedDualCoin != null
                                && coinKernelVm.CoinKernelProfile.SelectedDualCoin.GetId() == message.Source.GetId()) {
                                coinKernelVm.CoinKernelProfile.OnPropertyChanged(nameof(coinKernelVm.CoinKernelProfile.SelectedDualCoin));
                            }
                            if (justAsDualCoin != vm.JustAsDualCoin) {
                                OnPropertyChanged(nameof(MainCoins));
                            }
                        }
                    });
                BuildEventPath<CoinIconDownloadedEvent>("刷新图标", LogEnum.DevConsole, location: this.GetType(), PathPriority.Normal,
                    path: message => {
                        try {
                            if (string.IsNullOrEmpty(message.Source.Icon)) {
                                return;
                            }
                            string iconFileFullName = MinerClientTempPath.GetIconFileFullName(message.Source.Icon);
                            if (string.IsNullOrEmpty(iconFileFullName) || !File.Exists(iconFileFullName)) {
                                return;
                            }
                            if (_dicById.TryGetValue(message.Source.GetId(), out CoinViewModel coinVm)) {
                                try {
                                    coinVm.IconImageSource = new Uri(iconFileFullName, UriKind.Absolute).ToString();
                                }
                                catch (Exception e) {
                                    File.Delete(iconFileFullName);
                                    Logger.ErrorDebugLine(e);
                                }
                            }
                        }
                        catch (Exception e) {
                            Logger.ErrorDebugLine(e);
                        }
                    });
                Init();
            }

            private void Init() {
                foreach (var item in NTMinerContext.Instance.ServerContext.CoinSet.AsEnumerable().ToArray()) {
                    _dicById.Add(item.GetId(), new CoinViewModel(item));
                }
                foreach (var coinVm in _dicById.Values) {
                    coinVm.RefreshIcon();
                }
            }

            public bool TryGetCoinVm(Guid coinId, out CoinViewModel coinVm) {
                return _dicById.TryGetValue(coinId, out coinVm);
            }

            public bool TryGetCoinVm(string coinCode, out CoinViewModel coinVm) {
                if (NTMinerContext.Instance.ServerContext.CoinSet.TryGetCoin(coinCode, out ICoin coin)) {
                    return TryGetCoinVm(coin.GetId(), out coinVm);
                }
                coinVm = CoinViewModel.Empty;
                return false;
            }

            public List<CoinViewModel> AllCoins {
                get {
                    return _dicById.Values.OrderBy(a => a.Code).ToList();
                }
            }

            public List<CoinViewModel> MainCoins {
                get {
                    return AllCoins.Where(a => !a.JustAsDualCoin).ToList();
                }
            }

            private IEnumerable<CoinViewModel> GetPleaseSelect() {
                yield return CoinViewModel.PleaseSelect;
                foreach (var item in AllCoins) {
                    yield return item;
                }
            }
            public List<CoinViewModel> PleaseSelect {
                get {
                    return GetPleaseSelect().ToList();
                }
            }

            private IEnumerable<CoinViewModel> GetMainCoinPleaseSelect() {
                yield return CoinViewModel.PleaseSelect;
                foreach (var item in MainCoins) {
                    yield return item;
                }
            }
            public List<CoinViewModel> MainCoinPleaseSelect {
                get {
                    return GetMainCoinPleaseSelect().ToList();
                }
            }

            private IEnumerable<CoinViewModel> GetDualPleaseSelect() {
                yield return CoinViewModel.PleaseSelect;
                yield return CoinViewModel.DualCoinEnabled;
                foreach (var group in GroupVms.List) {
                    foreach (var item in group.DualCoinVms) {
                        yield return item;
                    }
                }
            }
            public List<CoinViewModel> DualPleaseSelect {
                get {
                    return GetDualPleaseSelect().Distinct().ToList();
                }
            }
        }
    }
}
