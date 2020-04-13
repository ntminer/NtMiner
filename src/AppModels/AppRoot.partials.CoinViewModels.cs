using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NTMiner {
    public static partial class AppRoot {
        public class CoinViewModels : ViewModelBase {
            public static readonly CoinViewModels Instance = new CoinViewModels();

            private readonly Dictionary<Guid, CoinViewModel> _dicById = new Dictionary<Guid, CoinViewModel>();

            private CoinViewModels() {
#if DEBUG
                NTStopwatch.Start();
#endif
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
                VirtualRoot.AddEventPath<ServerContextReInitedEvent>("ServerContext刷新后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicById.Clear();
                        Init();
                    }, location: this.GetType());
                VirtualRoot.AddEventPath<ServerContextVmsReInitedEvent>("ServerContext的VM集刷新后刷新视图界面", LogEnum.DevConsole,
                    action: message => {
                        AllPropertyChanged();
                    }, location: this.GetType());
                AddEventPath<CoinAddedEvent>("添加了币种后刷新VM内存", LogEnum.DevConsole,
                    action: (message) => {
                        _dicById.Add(message.Target.GetId(), new CoinViewModel(message.Target));
                        AllPropertyChanged();
                        VirtualRoot.RaiseEvent(new CoinVmAddedEvent(message));
                    }, location: this.GetType());
                AddEventPath<CoinRemovedEvent>("移除了币种后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        _dicById.Remove(message.Target.GetId());
                        AllPropertyChanged();
                        VirtualRoot.RaiseEvent(new CoinVmRemovedEvent(message));
                    }, location: this.GetType());
                AddEventPath<CoinKernelVmAddedEvent>("币种内核Vm集添加了新Vm后刷新币种Vm集的关联内存", LogEnum.DevConsole, action: message => {
                    if (_dicById.TryGetValue(message.Event.Target.CoinId, out CoinViewModel coinVm)) {
                        coinVm.OnPropertyChanged(nameof(CoinViewModel.CoinKernel));
                        coinVm.OnPropertyChanged(nameof(CoinViewModel.CoinKernels));
                        coinVm.OnPropertyChanged(nameof(CoinViewModel.IsSupported));
                    }
                }, this.GetType());
                AddEventPath<CoinKernelVmRemovedEvent>("币种内核Vm集删除了新Vm后刷新币种Vm集的关联内存", LogEnum.DevConsole, action: message => {
                    if (_dicById.TryGetValue(message.Event.Target.CoinId, out CoinViewModel coinVm)) {
                        coinVm.OnPropertyChanged(nameof(CoinViewModel.CoinKernel));
                        coinVm.OnPropertyChanged(nameof(CoinViewModel.CoinKernels));
                        coinVm.OnPropertyChanged(nameof(CoinViewModel.IsSupported));
                    }
                }, this.GetType());
                AddEventPath<CoinUpdatedEvent>("更新了币种后刷新VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (_dicById.TryGetValue(message.Target.GetId(), out CoinViewModel vm)) {
                            bool justAsDualCoin = vm.JustAsDualCoin;
                            vm.Update(message.Target);
                            vm.TestWalletVm.Address = message.Target.TestWallet;
                            vm.OnPropertyChanged(nameof(vm.Wallets));
                            vm.OnPropertyChanged(nameof(vm.WalletItems));
                            if (MinerProfileVm.CoinId == message.Target.GetId()) {
                                MinerProfileVm.OnPropertyChanged(nameof(MinerProfileVm.CoinVm));
                            }
                            CoinKernelViewModel coinKernelVm = MinerProfileVm.CoinVm.CoinKernel;
                            if (coinKernelVm != null
                                && coinKernelVm.CoinKernelProfile.SelectedDualCoin != null
                                && coinKernelVm.CoinKernelProfile.SelectedDualCoin.GetId() == message.Target.GetId()) {
                                coinKernelVm.CoinKernelProfile.OnPropertyChanged(nameof(coinKernelVm.CoinKernelProfile.SelectedDualCoin));
                            }
                            if (justAsDualCoin != vm.JustAsDualCoin) {
                                OnPropertyChanged(nameof(MainCoins));
                            }
                        }
                    }, location: this.GetType());
                AddEventPath<CoinIconDownloadedEvent>("下载了币种图标后", LogEnum.DevConsole,
                    action: message => {
                        try {
                            if (string.IsNullOrEmpty(message.Target.Icon)) {
                                return;
                            }
                            string iconFileFullName = TempPath.GetIconFileFullName(message.Target.Icon);
                            if (string.IsNullOrEmpty(iconFileFullName) || !File.Exists(iconFileFullName)) {
                                return;
                            }
                            if (_dicById.TryGetValue(message.Target.GetId(), out CoinViewModel coinVm)) {
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
                    }, location: this.GetType());
                Init();
#if DEBUG
                var elapsedMilliseconds = NTStopwatch.Stop();
                if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                    Write.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.ctor");
                }
#endif
            }

            private void Init() {
                foreach (var item in NTMinerContext.Instance.ServerContext.CoinSet.AsEnumerable()) {
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
