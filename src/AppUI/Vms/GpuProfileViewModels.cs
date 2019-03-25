using NTMiner.Core;
using NTMiner.Core.Profiles;
using NTMiner.MinerClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class GpuProfileViewModels : ViewModelBase {
        public static readonly GpuProfileViewModels Current = new GpuProfileViewModels();

        private readonly Dictionary<Guid, List<GpuProfileViewModel>> _listByCoinId = new Dictionary<Guid, List<GpuProfileViewModel>>();
        private readonly Dictionary<Guid, GpuProfileViewModel> _gpuAllVmDicByCoinId = new Dictionary<Guid, GpuProfileViewModel>();

        private GpuProfileViewModels() {
            VirtualRoot.On<GpuProfileSetRefreshedEvent>("Gpu超频集合刷新后刷新附着在当前币种上的超频数据", LogEnum.DevConsole,
                action: message => {
                    lock (_locker) {
                        _listByCoinId.Clear();
                        _gpuAllVmDicByCoinId.Clear();
                    }
                    var coinVm = MinerProfileViewModel.Current.CoinVm;
                    if (coinVm != null) {
                        coinVm.OnOverClockPropertiesChanges();
                        VirtualRoot.Execute(new CoinOverClockCommand(coinVm.Id));
                    }
                });
            VirtualRoot.On<GpuProfileAddedOrUpdatedEvent>("添加或更新了Gpu超频数据后刷新VM内存", LogEnum.DevConsole,
                action: message => {
                    lock (_locker) {
                        List<GpuProfileViewModel> list;
                        if (_listByCoinId.TryGetValue(message.Source.CoinId, out list)) {
                            var vm = list.FirstOrDefault(a => a.Index == message.Source.Index);
                            if (vm != null) {
                                vm.Update(message.Source);
                            }
                            else {
                                if (GpuViewModels.Current.TryGetGpuVm(message.Source.Index, out GpuViewModel gpuVm)) {
                                    var item = new GpuProfileViewModel(message.Source, gpuVm);
                                    list.Add(item);
                                    list.Sort(new CompareByGpuIndex());
                                    if (item.Index == NTMinerRoot.GpuAllId) {
                                        _gpuAllVmDicByCoinId.Add(message.Source.CoinId, item);
                                    }
                                }
                            }
                        }
                        else {
                            list = new List<GpuProfileViewModel>();
                            if (GpuViewModels.Current.TryGetGpuVm(message.Source.Index, out GpuViewModel gpuVm)) {
                                var item = new GpuProfileViewModel(message.Source, gpuVm);
                                list.Add(item);
                                list.Sort(new CompareByGpuIndex());
                                if (item.Index == NTMinerRoot.GpuAllId) {
                                    _gpuAllVmDicByCoinId.Add(message.Source.CoinId, item);
                                }
                            }
                            _listByCoinId.Add(message.Source.CoinId, list);
                        }
                    }
                });
        }

        private readonly object _locker = new object();
        public GpuProfileViewModel GpuAllVm(Guid coinId) {
            GpuProfileViewModel result;
            if (!_gpuAllVmDicByCoinId.TryGetValue(coinId, out result)) {
                lock (_locker) {
                    if (!_gpuAllVmDicByCoinId.TryGetValue(coinId, out result)) {
                        GpuViewModel gpuVm;
                        GpuViewModels.Current.TryGetGpuVm(NTMinerRoot.GpuAllId, out gpuVm);
                        result = GetGpuProfileVm(coinId, gpuVm);
                        _gpuAllVmDicByCoinId.Add(coinId, result);
                    }
                }
            }
            return result;
        }

        public List<GpuProfileViewModel> List(Guid coinId) {
            List<GpuProfileViewModel> list;
            if (!_listByCoinId.TryGetValue(coinId, out list)) {
                lock (_locker) {
                    if (!_listByCoinId.TryGetValue(coinId, out list)) {
                        list = new List<GpuProfileViewModel>();
                        foreach (var gpu in GpuViewModels.Current) {
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
            IGpuProfile data = GpuProfileSet.Instance.GetGpuProfile(coinId, gpuVm.Index);
            return new GpuProfileViewModel(data, gpuVm);
        }

        private class CompareByGpuIndex : IComparer<GpuProfileViewModel> {
            public int Compare(GpuProfileViewModel x, GpuProfileViewModel y) {
                return x.Index - y.Index;
            }
        }
    }
}
