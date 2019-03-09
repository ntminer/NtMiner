using NTMiner.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class CoinKernelViewModels : ViewModelBase {
        public static readonly CoinKernelViewModels Current = new CoinKernelViewModels();
        private readonly Dictionary<Guid, CoinKernelViewModel> _dicById = new Dictionary<Guid, CoinKernelViewModel>();
        private CoinKernelViewModels() {
            VirtualRoot.On<CoinKernelAddedEvent>(
                "添加了币种内核后刷新VM内存",
                LogEnum.Console,
                action: (message) => {
                    var coinKernelVm = new CoinKernelViewModel(message.Source);
                    _dicById.Add(message.Source.GetId(), coinKernelVm);
                    OnPropertyChanged(nameof(AllCoinKernels));
                    CoinViewModel coinVm;
                    if (CoinViewModels.Current.TryGetCoinVm(message.Source.CoinId, out coinVm)) {
                        coinVm.OnPropertyChanged(nameof(CoinViewModel.CoinKernel));
                        coinVm.OnPropertyChanged(nameof(CoinViewModel.CoinKernels));
                        coinVm.OnPropertyChanged(nameof(CoinViewModel.IsSupported));
                    }
                    coinKernelVm.Kernel.OnPropertyChanged(nameof(coinKernelVm.Kernel.IsSupported));
                    coinKernelVm.Kernel.OnPropertyChanged(nameof(coinKernelVm.Kernel.IsNvidiaIconVisible));
                    coinKernelVm.Kernel.OnPropertyChanged(nameof(coinKernelVm.Kernel.IsAMDIconVisible));
                    coinKernelVm.Kernel.OnPropertyChanged(nameof(coinKernelVm.Kernel.CoinKernels));
                    coinKernelVm.Kernel.OnPropertyChanged(nameof(coinKernelVm.Kernel.CoinVms));
                    coinKernelVm.Kernel.OnPropertyChanged(nameof(coinKernelVm.Kernel.SupportedCoinVms));
                    coinKernelVm.Kernel.OnPropertyChanged(nameof(coinKernelVm.Kernel.SupportedCoins));
                }).AddToCollection(NTMinerRoot.Current.ContextHandlers);
            VirtualRoot.On<CoinKernelUpdatedEvent>(
                "更新了币种内核后刷新VM内存",
                LogEnum.Console,
                action: (message) => {
                    CoinKernelViewModel entity = _dicById[message.Source.GetId()];
                    var supportedGpu = entity.SupportedGpu;
                    int sortNumber = entity.SortNumber;
                    Guid dualCoinGroupId = entity.DualCoinGroupId;
                    entity.Update(message.Source);
                    if (supportedGpu != entity.SupportedGpu) {
                        var coinKernels = AllCoinKernels.Where(a => a.KernelId == entity.Id);
                        foreach (var coinKernel in coinKernels) {
                            CoinViewModel coinVm;
                            if (CoinViewModels.Current.TryGetCoinVm(coinKernel.CoinId, out coinVm)) {
                                coinVm.OnPropertyChanged(nameof(coinVm.IsSupported));
                                coinVm.OnPropertyChanged(nameof(coinVm.CoinKernels));
                            }
                            coinKernel.Kernel.OnPropertyChanged(nameof(coinKernel.Kernel.IsSupported));
                        }
                        entity.Kernel.OnPropertyChanged(nameof(entity.Kernel.IsNvidiaIconVisible));
                        entity.Kernel.OnPropertyChanged(nameof(entity.Kernel.IsAMDIconVisible));
                        entity.Kernel.OnPropertyChanged(nameof(entity.Kernel.CoinKernels));
                    }
                    if (dualCoinGroupId != entity.DualCoinGroupId) {
                        entity.OnPropertyChanged(nameof(entity.DualCoinGroup));
                    }
                    if (sortNumber != entity.SortNumber) {
                        CoinViewModel coinVm;
                        if (CoinViewModels.Current.TryGetCoinVm(entity.CoinId, out coinVm)) {
                            coinVm.OnPropertyChanged(nameof(coinVm.CoinKernels));
                        }
                    }
                }).AddToCollection(NTMinerRoot.Current.ContextHandlers);
            VirtualRoot.On<CoinKernelRemovedEvent>(
                "移除了币种内核后刷新VM内存",
                LogEnum.Console,
                action: (message) => {
                    var coinKernelVm = _dicById[message.Source.GetId()];
                    _dicById.Remove(message.Source.GetId());
                    OnPropertyChanged(nameof(AllCoinKernels));
                    CoinViewModel coinVm;
                    if (CoinViewModels.Current.TryGetCoinVm(message.Source.CoinId, out coinVm)) {
                        coinVm.OnPropertyChanged(nameof(CoinViewModel.CoinKernel));
                        coinVm.OnPropertyChanged(nameof(CoinViewModel.CoinKernels));
                        coinVm.OnPropertyChanged(nameof(CoinViewModel.IsSupported));
                    }
                    coinKernelVm.Kernel.OnPropertyChanged(nameof(coinKernelVm.Kernel.IsSupported));
                    coinKernelVm.Kernel.OnPropertyChanged(nameof(coinKernelVm.Kernel.IsNvidiaIconVisible));
                    coinKernelVm.Kernel.OnPropertyChanged(nameof(coinKernelVm.Kernel.IsAMDIconVisible));
                    coinKernelVm.Kernel.OnPropertyChanged(nameof(coinKernelVm.Kernel.CoinKernels));
                    coinKernelVm.Kernel.OnPropertyChanged(nameof(coinKernelVm.Kernel.CoinVms));
                    coinKernelVm.Kernel.OnPropertyChanged(nameof(coinKernelVm.Kernel.SupportedCoinVms));
                    coinKernelVm.Kernel.OnPropertyChanged(nameof(coinKernelVm.Kernel.SupportedCoins));
                }).AddToCollection(NTMinerRoot.Current.ContextHandlers);
            Init();
        }

        private void Init() {
            foreach (var item in NTMinerRoot.Current.CoinKernelSet) {
                _dicById.Add(item.GetId(), new CoinKernelViewModel(item));
            }
        }

        public bool TryGetCoinKernelVm(Guid id, out CoinKernelViewModel vm) {
            return _dicById.TryGetValue(id, out vm);
        }

        public List<CoinKernelViewModel> AllCoinKernels {
            get {
                return _dicById.Values.ToList();
            }
        }
    }
}
