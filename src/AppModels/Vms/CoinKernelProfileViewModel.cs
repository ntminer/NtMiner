using NTMiner.Core;
using NTMiner.Profile;
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
                    NTMinerRoot.Instance.MinerProfile.SetCoinKernelProfileProperty(this.CoinKernelId, nameof(CoinKernelId), value);
                    OnPropertyChanged(nameof(CoinKernelId));
                }
            }
        }

        public bool IsDualCoinEnabled {
            get {
                if (AppContext.Instance.CoinKernelVms.TryGetCoinKernelVm(this.CoinKernelId, out CoinKernelViewModel coinKernelVm) && !coinKernelVm.IsSupportDualMine) {
                    return false;
                }
                return _inner.IsDualCoinEnabled;
            }
            set {
                if (_inner.IsDualCoinEnabled != value) {
                    NTMinerRoot.Instance.MinerProfile.SetCoinKernelProfileProperty(this.CoinKernelId, nameof(IsDualCoinEnabled), value);
                    OnPropertyChanged(nameof(IsDualCoinEnabled));
                    NTMinerRoot.RefreshArgsAssembly.Invoke();
                }
            }
        }
        public Guid DualCoinId {
            get => _inner.DualCoinId;
            set {
                if (_inner.DualCoinId != value) {
                    NTMinerRoot.Instance.MinerProfile.SetCoinKernelProfileProperty(this.CoinKernelId, nameof(DualCoinId), value);
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
                if (NTMinerRoot.Instance.CoinKernelSet.TryGetCoinKernel(this.CoinKernelId, out coinKernel)) {
                    IKernel kernel;
                    if (NTMinerRoot.Instance.KernelSet.TryGetKernel(coinKernel.KernelId, out kernel)) {
                        IKernelInput kernelInput;
                        if (NTMinerRoot.Instance.KernelInputSet.TryGetKernelInput(kernel.KernelInputId, out kernelInput)) {
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
                    NTMinerRoot.Instance.MinerProfile.SetCoinKernelProfileProperty(this.CoinKernelId, nameof(IsAutoDualWeight), value);
                    OnPropertyChanged(nameof(IsAutoDualWeight));
                    NTMinerRoot.RefreshArgsAssembly.Invoke();
                }
            }
        }

        public string CustomArgs {
            get => _inner.CustomArgs;
            set {
                if (_inner.CustomArgs != value) {
                    if (AppContext.Instance.CoinKernelVms.TryGetCoinKernelVm(this.CoinKernelId, out CoinKernelViewModel coinKernelVm)) {
                        NTMinerRoot.Instance.MinerProfile.SetCoinKernelProfileProperty(this.CoinKernelId, nameof(CustomArgs), value);
                        OnPropertyChanged(nameof(CustomArgs));
                        NTMinerRoot.RefreshArgsAssembly.Invoke();
                        foreach (var inputSegmentVm in coinKernelVm.InputSegmentVms) {
                            inputSegmentVm.OnPropertyChanged(nameof(inputSegmentVm.IsChecked));
                        }
                        foreach (var gpuVm in AppContext.Instance.GpuVms) {
                            if (gpuVm.Index == NTMinerRoot.GpuAllId) {
                                continue;
                            }
                            gpuVm.OnPropertyChanged(nameof(gpuVm.IsDeviceArgInclude));
                        }
                    }
                }
            }
        }

        public CoinViewModel SelectedDualCoin {
            get {
                if (!AppContext.Instance.CoinVms.TryGetCoinVm(this.DualCoinId, out CoinViewModel coin)) {
                    if (AppContext.Instance.CoinKernelVms.TryGetCoinKernelVm(this.CoinKernelId, out CoinKernelViewModel coinKernelVm)) {
                        coin = coinKernelVm.DualCoinGroup.DualCoinVms.FirstOrDefault();
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
                    NTMinerRoot.RefreshArgsAssembly.Invoke();
                }
            }
        }
    }
}
