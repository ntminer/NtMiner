using NTMiner.Core.Profile;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public static partial class AppRoot {
        public class GpuProfileViewModels : ViewModelBase {
            public static GpuProfileViewModels Instance { get; private set; } = new GpuProfileViewModels();

            private readonly Dictionary<Guid, List<GpuProfileViewModel>> _listByCoinId = new Dictionary<Guid, List<GpuProfileViewModel>>();
            private readonly Dictionary<Guid, GpuProfileViewModel> _gpuAllVmDicByCoinId = new Dictionary<Guid, GpuProfileViewModel>();

            private GpuProfileViewModels() {
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
                VirtualRoot.BuildEventPath<GpuProfileSetRefreshedEvent>("刷新附着在当前币种上的超频数据", LogEnum.DevConsole,
                    path: message => {
                        lock (_locker) {
                            _listByCoinId.Clear();
                            _gpuAllVmDicByCoinId.Clear();
                        }
                        var coinVm = MinerProfileVm.CoinVm;
                        if (coinVm != null) {
                            coinVm.OnOverClockPropertiesChanges();
                        }
                    }, location: this.GetType());
                BuildEventPath<GpuProfileAddedOrUpdatedEvent>("刷新VM内存", LogEnum.DevConsole,
                    path: message => {
                        lock (_locker) {
                            if (_listByCoinId.TryGetValue(message.Source.CoinId, out List<GpuProfileViewModel> list)) {
                                var vm = list.FirstOrDefault(a => a.Index == message.Source.Index);
                                if (vm != null) {
                                    vm.Update(message.Source);
                                }
                                else {
                                    if (GpuVms.TryGetGpuVm(message.Source.Index, out GpuViewModel gpuVm)) {
                                        var item = new GpuProfileViewModel(message.Source, gpuVm);
                                        list.Add(item);
                                        list.Sort(new CompareByGpuIndex());
                                        if (item.Index == NTMinerContext.GpuAllId) {
                                            _gpuAllVmDicByCoinId.Add(message.Source.CoinId, item);
                                        }
                                    }
                                }
                            }
                            else {
                                list = new List<GpuProfileViewModel>();
                                if (GpuVms.TryGetGpuVm(message.Source.Index, out GpuViewModel gpuVm)) {
                                    var item = new GpuProfileViewModel(message.Source, gpuVm);
                                    list.Add(item);
                                    list.Sort(new CompareByGpuIndex());
                                    if (item.Index == NTMinerContext.GpuAllId) {
                                        _gpuAllVmDicByCoinId.Add(message.Source.CoinId, item);
                                    }
                                }
                                _listByCoinId.Add(message.Source.CoinId, list);
                            }
                        }
                    }, location: this.GetType());
            }

            private readonly object _locker = new object();
            public GpuProfileViewModel GpuAllVm(Guid coinId) {
                if (!_gpuAllVmDicByCoinId.TryGetValue(coinId, out GpuProfileViewModel result)) {
                    lock (_locker) {
                        if (!_gpuAllVmDicByCoinId.TryGetValue(coinId, out result)) {
                            GpuVms.TryGetGpuVm(NTMinerContext.GpuAllId, out GpuViewModel gpuVm);
                            result = GetGpuProfileVm(coinId, gpuVm);
                            _gpuAllVmDicByCoinId.Add(coinId, result);
                        }
                    }
                }
                return result;
            }

            public List<GpuProfileViewModel> List(Guid coinId) {
                if (!_listByCoinId.TryGetValue(coinId, out List<GpuProfileViewModel> list)) {
                    lock (_locker) {
                        if (!_listByCoinId.TryGetValue(coinId, out list)) {
                            list = new List<GpuProfileViewModel>();
                            foreach (var gpu in GpuVms.Items) {
                                GpuProfileViewModel gpuProfileVm = GetGpuProfileVm(coinId, gpu);
                                list.Add(gpuProfileVm);
                            }
                            list.Sort(new CompareByGpuIndex());
                            _listByCoinId.Add(coinId, list);
                        }
                    }
                }
                return list;
            }

            private GpuProfileViewModel GetGpuProfileVm(Guid coinId, GpuViewModel gpuVm) {
                IGpuProfile data = NTMinerContext.Instance.GpuProfileSet.GetGpuProfile(coinId, gpuVm.Index);
                return new GpuProfileViewModel(data, gpuVm);
            }

            private class CompareByGpuIndex : IComparer<GpuProfileViewModel> {
                public int Compare(GpuProfileViewModel x, GpuProfileViewModel y) {
                    return x.Index - y.Index;
                }
            }
        }
    }
}
