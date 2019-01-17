using NTMiner.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class PoolViewModels : ViewModelBase {
        public static readonly PoolViewModels Current = new PoolViewModels();
        private readonly Dictionary<Guid, PoolViewModel> _dicById = new Dictionary<Guid, PoolViewModel>();
        private PoolViewModels() {
            Global.Access<PoolAddedEvent>(
                Guid.Parse("1fdd642a-5976-4da1-afae-def9730a91bd"),
                "添加矿池后刷新VM内存",
                LogEnum.Log,
                action: (message) => {
                    _dicById.Add(message.Source.GetId(), new PoolViewModel(message.Source));
                    OnPropertyChanged(nameof(AllPools));
                    ICoin coin;
                    if (NTMinerRoot.Current.CoinSet.TryGetCoin(message.Source.CoinId, out coin)) {
                        CoinViewModels.Current[coin.GetId()].CoinProfile.OnPropertyChanged(nameof(CoinProfileViewModel.MainCoinPool));
                        CoinViewModels.Current[coin.GetId()].CoinProfile.OnPropertyChanged(nameof(CoinProfileViewModel.DualCoinPool));
                        CoinViewModels.Current[coin.GetId()].OnPropertyChanged(nameof(CoinViewModel.Pools));
                        CoinViewModels.Current[coin.GetId()].OnPropertyChanged(nameof(CoinViewModel.OptionPools));
                    }
                });
            Global.Access<PoolRemovedEvent>(
                Guid.Parse("6d5d276b-4ad7-4280-bd4d-cfdeb1712aab"),
                "删除矿池后刷新VM内存",
                LogEnum.Log,
                action: (message) => {
                    _dicById.Remove(message.Source.GetId());
                    OnPropertyChanged(nameof(AllPools));
                    ICoin coin;
                    if (NTMinerRoot.Current.CoinSet.TryGetCoin(message.Source.CoinId, out coin)) {
                        CoinViewModels.Current[coin.GetId()].CoinProfile.OnPropertyChanged(nameof(CoinProfileViewModel.MainCoinPool));
                        CoinViewModels.Current[coin.GetId()].CoinProfile.OnPropertyChanged(nameof(CoinProfileViewModel.DualCoinPool));
                        CoinViewModels.Current[coin.GetId()].OnPropertyChanged(nameof(CoinViewModel.Pools));
                        CoinViewModels.Current[coin.GetId()].OnPropertyChanged(nameof(CoinViewModel.OptionPools));
                    }
                });
            Global.Access<PoolUpdatedEvent>(
                Guid.Parse("2f7ae88f-b6af-499c-aed2-eece17a536b8"),
                "更新矿池后刷新VM内存",
                LogEnum.Log,
                action: (message) => {
                    _dicById[message.Source.GetId()].Update(message.Source);
                });
            foreach (var item in NTMinerRoot.Current.PoolSet) {
                _dicById.Add(item.GetId(), new PoolViewModel(item));
            }
        }

        public bool TryGetPoolVm(Guid poolId, out PoolViewModel poolVm) {
            return _dicById.TryGetValue(poolId, out poolVm);
        }

        public List<PoolViewModel> AllPools {
            get {
                return _dicById.Values.ToList();
            }
        }
    }
}
