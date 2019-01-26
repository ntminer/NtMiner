using NTMiner.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class CoinViewModels : ViewModelBase {
        public static readonly CoinViewModels Current = new CoinViewModels();

        private readonly Dictionary<Guid, CoinViewModel> _dicById = new Dictionary<Guid, CoinViewModel>();

        private CoinViewModels() {
            if (Design.IsInDesignMode) {
                return;
            }
            Global.Access<CoinSetRefreshedEvent>(
                Guid.Parse("E1BD1C77-2845-4EC1-9262-BAFBC2C0532C"),
                "币种数据集刷新后刷新Vm内存",
                LogEnum.Console,
                action: message => {
                    Init(isRefresh: true);
                    OnPropertyChangeds();
                });
            Global.Access<CoinAddedEvent>(
                Guid.Parse("1ee6e72d-d98f-42ab-8732-dcee2e42f4b8"),
                "添加了币种后刷新VM内存",
                LogEnum.Log,
                action: (message) => {
                    _dicById.Add(message.Source.GetId(), new CoinViewModel(message.Source));
                    MinerProfileViewModel.Current.OnPropertyChanged(nameof(MinerProfileViewModel.Current.CoinVm));
                    OnPropertyChangeds();
                    CoinPageViewModel.Current.OnPropertyChanged(nameof(CoinPageViewModel.List));
                });
            Global.Access<CoinRemovedEvent>(
                Guid.Parse("6c966862-6dfa-4473-94b5-1133a16180a1"),
                "移除了币种后刷新VM内存",
                LogEnum.Log,
                action: message => {
                    _dicById.Remove(message.Source.GetId());
                    MinerProfileViewModel.Current.OnPropertyChanged(nameof(MinerProfileViewModel.Current.CoinVm));
                    OnPropertyChangeds();
                    CoinPageViewModel.Current.OnPropertyChanged(nameof(CoinPageViewModel.List));
                });
            Global.Access<CoinUpdatedEvent>(
                Guid.Parse("114c90e5-6a0a-4aa4-9ba8-5ed603286c51"),
                "更新了币种后刷新VM内存",
                LogEnum.Log,
                action: message => {
                    CoinViewModel coinVm = _dicById[message.Source.GetId()];
                    bool justAsDualCoin = coinVm.JustAsDualCoin;
                    coinVm.Update(message.Source);
                    coinVm.TestWalletVm.Address = message.Source.TestWallet;
                    coinVm.OnPropertyChanged(nameof(coinVm.Wallets));
                    coinVm.OnPropertyChanged(nameof(coinVm.WalletItems));
                    if (MinerProfileViewModel.Current.CoinId == message.Source.GetId()) {
                        MinerProfileViewModel.Current.OnPropertyChanged(nameof(MinerProfileViewModel.Current.CoinVm));
                    }
                    CoinKernelViewModel coinKernelVm = MinerProfileViewModel.Current.CoinVm.CoinKernel;
                    if (coinKernelVm != null
                        && coinKernelVm.CoinKernelProfile.SelectedDualCoin != null
                        && coinKernelVm.CoinKernelProfile.SelectedDualCoin.GetId() == message.Source.GetId()) {
                        coinKernelVm.CoinKernelProfile.OnPropertyChanged(nameof(coinKernelVm.CoinKernelProfile.SelectedDualCoin));
                    }
                    if (justAsDualCoin != coinVm.JustAsDualCoin) {
                        OnPropertyChanged(nameof(MainCoins));
                    }
                });
            Init();
        }

        private void Init(bool isRefresh = false) {
            if (isRefresh) {
                foreach (var item in NTMinerRoot.Current.CoinSet) {
                    CoinViewModel vm;
                    if (_dicById.TryGetValue(item.GetId(), out vm)) {
                        vm.Update(item);
                    }
                    else {
                        _dicById.Add(item.GetId(), new CoinViewModel(item));
                    }
                }
            }
            else {
                foreach (var item in NTMinerRoot.Current.CoinSet) {
                    _dicById.Add(item.GetId(), new CoinViewModel(item));
                }
            }
        }

        private void OnPropertyChangeds() {
            OnPropertyChanged(nameof(AllCoins));
            OnPropertyChanged(nameof(MainCoins));
            OnPropertyChanged(nameof(PleaseSelect));
            OnPropertyChanged(nameof(DualPleaseSelect));
        }

        public CoinViewModel this[Guid coinId] {
            get {
                return _dicById[coinId];
            }
        }

        public bool TryGetCoinVm(Guid coinId, out CoinViewModel coinVm) {
            return _dicById.TryGetValue(coinId, out coinVm);
        }

        public bool TryGetCoinVm(string coinCode, out CoinViewModel coinVm) {
            ICoin coin;
            if (NTMinerRoot.Current.CoinSet.TryGetCoin(coinCode, out coin)) {
                return TryGetCoinVm(coin.GetId(), out coinVm);
            }
            coinVm = CoinViewModel.Empty;
            return false;
        }

        public List<CoinViewModel> AllCoins {
            get {
                return _dicById.Values.OrderBy(a => a.SortNumber).ToList();
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

        private IEnumerable<CoinViewModel> GetDualPleaseSelect() {
            yield return CoinViewModel.PleaseSelect;
            yield return CoinViewModel.DualCoinEnabled;
            foreach (var group in GroupViewModels.Current.List) {
                foreach (var item in group.DualCoinVms) {
                    yield return item;
                }
            }
        }
        public List<CoinViewModel> DualPleaseSelect {
            get {
                return GetDualPleaseSelect().Distinct().OrderBy(a => a.SortNumber).ToList();
            }
        }
    }
}
