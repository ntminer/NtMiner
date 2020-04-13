using NTMiner.Core;
using NTMiner.Core.Profile;
using System;
using System.Linq;

namespace NTMiner.Vms {
    public class CoinKernelProfileViewModel : ViewModelBase, ICoinKernelProfile {
        private readonly ICoinKernelProfile _inner;
        public CoinKernelProfileViewModel(ICoinKernelProfile inner) {
            _inner = inner;
            _dualCoinWeight = inner.DualCoinWeight;
        }

        public Guid CoinKernelId {
            get => _inner.CoinKernelId;
            set {
                if (_inner.CoinKernelId != value) {
                    NTMinerContext.Instance.MinerProfile.SetCoinKernelProfileProperty(this.CoinKernelId, nameof(CoinKernelId), value);
                    OnPropertyChanged(nameof(CoinKernelId));
                }
            }
        }

        public bool IsDualCoinEnabled {
            get {
                if (AppRoot.CoinKernelVms.TryGetCoinKernelVm(this.CoinKernelId, out CoinKernelViewModel coinKernelVm) && !coinKernelVm.IsSupportDualMine) {
                    return false;
                }
                return _inner.IsDualCoinEnabled;
            }
            set {
                if (_inner.IsDualCoinEnabled != value) {
                    NTMinerContext.Instance.MinerProfile.SetCoinKernelProfileProperty(this.CoinKernelId, nameof(IsDualCoinEnabled), value);
                    OnPropertyChanged(nameof(IsDualCoinEnabled));
                    // 不必判断该对象是否是主界面上当前展示的对象，因为若不是主界面上当前显式的对象的话没有机会变更
                    NTMinerContext.RefreshArgsAssembly.Invoke("币种内核Profile上放置的挖矿双挖启用状态发生了变更");
                }
            }
        }
        public Guid DualCoinId {
            get => _inner.DualCoinId;
            set {
                if (_inner.DualCoinId != value) {
                    NTMinerContext.Instance.MinerProfile.SetCoinKernelProfileProperty(this.CoinKernelId, nameof(DualCoinId), value);
                    OnPropertyChanged(nameof(DualCoinId));
                }
            }
        }

        private double _dualCoinWeight;
        public double DualCoinWeight {
            get => _dualCoinWeight;
            set {
                if (Math.Abs(_dualCoinWeight - value) > 0.01) {
                    _dualCoinWeight = value;
                    OnPropertyChanged(nameof(DualCoinWeight));
                }
            }
        }

        public bool IsAutoDualWeight {
            get {
                ICoinKernel coinKernel;
                if (NTMinerContext.Instance.ServerContext.CoinKernelSet.TryGetCoinKernel(this.CoinKernelId, out coinKernel)) {
                    IKernel kernel;
                    if (NTMinerContext.Instance.ServerContext.KernelSet.TryGetKernel(coinKernel.KernelId, out kernel)) {
                        IKernelInput kernelInput;
                        if (NTMinerContext.Instance.ServerContext.KernelInputSet.TryGetKernelInput(kernel.KernelInputId, out kernelInput)) {
                            if (!kernelInput.IsAutoDualWeight) {
                                return false;
                            }
                        }
                    }
                }
                return _inner.IsAutoDualWeight;
            }
            set {
                if (_inner.IsAutoDualWeight != value) {
                    NTMinerContext.Instance.MinerProfile.SetCoinKernelProfileProperty(this.CoinKernelId, nameof(IsAutoDualWeight), value);
                    OnPropertyChanged(nameof(IsAutoDualWeight));
                    // 不必判断该对象是否是主界面上当前展示的对象，因为若不是主界面上当前显式的对象的话没有机会变更
                    NTMinerContext.RefreshArgsAssembly.Invoke("币种内核Profile上放置的挖矿双挖权重发生了变更");
                }
            }
        }

        public string CustomArgs {
            get => _inner.CustomArgs;
            set {
                if (_inner.CustomArgs != value) {
                    if (AppRoot.CoinKernelVms.TryGetCoinKernelVm(this.CoinKernelId, out CoinKernelViewModel coinKernelVm)) {
                        NTMinerContext.Instance.MinerProfile.SetCoinKernelProfileProperty(this.CoinKernelId, nameof(CustomArgs), value);
                        OnPropertyChanged(nameof(CustomArgs));
                        // 不必判断该对象是否是主界面上当前展示的对象，因为若不是主界面上当前显式的对象的话没有机会变更
                        NTMinerContext.RefreshArgsAssembly.Invoke("币种内核Profile上放置的用户自定义挖矿参数发生了变更");
                        foreach (var inputSegmentVm in coinKernelVm.InputSegmentVms) {
                            inputSegmentVm.OnPropertyChanged(nameof(inputSegmentVm.IsChecked));
                        }
                        foreach (var gpuVm in AppRoot.GpuVms.Items) {
                            if (gpuVm.Index == NTMinerContext.GpuAllId) {
                                continue;
                            }
                            gpuVm.OnPropertyChanged(nameof(gpuVm.IsDeviceArgInclude));
                        }
                    }
                }
            }
        }

        public string TouchedArgs {
            get { return _inner.TouchedArgs; }
            set {
                NTMinerContext.Instance.MinerProfile.SetCoinKernelProfileProperty(this.CoinKernelId, nameof(TouchedArgs), value);
                OnPropertyChanged(nameof(TouchedArgs));
            }
        }

        public CoinViewModel SelectedDualCoin {
            get {
                if (!AppRoot.CoinVms.TryGetCoinVm(this.DualCoinId, out CoinViewModel coin)) {
                    if (AppRoot.CoinKernelVms.TryGetCoinKernelVm(this.CoinKernelId, out CoinKernelViewModel coinKernelVm)) {
                        coin = coinKernelVm.SelectedDualCoinGroup.DualCoinVms.FirstOrDefault();
                    }
                    if (coin != null) {
                        DualCoinId = coin.Id;
                    }
                }
                return coin;
            }
            set {
                if (value != null && value.Id != Guid.Empty) {
                    DualCoinId = value.Id;
                    OnPropertyChanged(nameof(SelectedDualCoin));
                    // 不必判断该对象是否是主界面上当前展示的对象，因为若不是主界面上当前显式的对象的话没有机会变更
                    NTMinerContext.RefreshArgsAssembly.Invoke("币种内核Profile上放置的挖矿选用的双挖币种发生了变更");
                }
            }
        }
    }
}
