using NTMiner.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class PoolViewModels : ViewModelBase {
        public static readonly PoolViewModels Current = new PoolViewModels();
        private readonly Dictionary<Guid, PoolViewModel> _dicById = new Dictionary<Guid, PoolViewModel>();
        private PoolViewModels() {
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
            VirtualRoot.On<PoolAddedEvent>("添加矿池后刷新VM内存", LogEnum.DevConsole,
                action: (message) => {
                    _dicById.Add(message.Source.GetId(), new PoolViewModel(message.Source));
                    OnPropertyChanged(nameof(AllPools));
                    CoinViewModel coinVm;
                    if (CoinViewModels.Current.TryGetCoinVm(message.Source.CoinId, out coinVm)) {
                        coinVm.CoinProfile.OnPropertyChanged(nameof(CoinProfileViewModel.MainCoinPool));
                        coinVm.CoinProfile.OnPropertyChanged(nameof(CoinProfileViewModel.DualCoinPool));
                        coinVm.OnPropertyChanged(nameof(CoinViewModel.Pools));
                        coinVm.OnPropertyChanged(nameof(CoinViewModel.OptionPools));
                    }
                }).AddToCollection(NTMinerRoot.Current.ContextHandlers);
            VirtualRoot.On<PoolRemovedEvent>("删除矿池后刷新VM内存", LogEnum.DevConsole,
                action: (message) => {
                    _dicById.Remove(message.Source.GetId());
                    OnPropertyChanged(nameof(AllPools));
                    CoinViewModel coinVm;
                    if (CoinViewModels.Current.TryGetCoinVm(message.Source.CoinId, out coinVm)) {
                        coinVm.CoinProfile.OnPropertyChanged(nameof(CoinProfileViewModel.MainCoinPool));
                        coinVm.CoinProfile.OnPropertyChanged(nameof(CoinProfileViewModel.DualCoinPool));
                        coinVm.OnPropertyChanged(nameof(CoinViewModel.Pools));
                        coinVm.OnPropertyChanged(nameof(CoinViewModel.OptionPools));
                    }
                }).AddToCollection(NTMinerRoot.Current.ContextHandlers);
            VirtualRoot.On<PoolUpdatedEvent>("更新矿池后刷新VM内存", LogEnum.DevConsole,
                action: (message) => {
                    _dicById[message.Source.GetId()].Update(message.Source);
                }).AddToCollection(NTMinerRoot.Current.ContextHandlers);
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
