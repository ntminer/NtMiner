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
            NTMinerRoot.Current.OnContextReInited += () => {
                _dicById.Clear();
                Init();
            };
            NTMinerRoot.Current.OnReRendContext += () => {
                AllPropertyChanged();
            };
            Init();
        }

        private void Init() {            
            foreach (var item in NTMinerRoot.Current.CoinSet) {
                _dicById.Add(item.GetId(), new CoinViewModel(item));
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

        private IEnumerable<CoinViewModel> GetMainCoinPleaseSelect() {
            yield return CoinViewModel.PleaseSelect;
            foreach (var item in MainCoins) {
                yield return item;
            }
        }
        public List<CoinViewModel> MainCoinPleaseSelect {
            get {
                return GetMainCoinPleaseSelect().OrderBy(a => a.SortNumber).ToList();
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
