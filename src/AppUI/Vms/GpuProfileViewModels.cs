using NTMiner.Core;
using NTMiner.Core.Profiles;
using NTMiner.MinerClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class GpuProfileViewModels : ViewModelBase {
        public static readonly GpuProfileViewModels Current = new GpuProfileViewModels();

        private readonly List<GpuProfileViewModel> _list = new List<GpuProfileViewModel>();
        private readonly Dictionary<Guid, GpuProfileViewModel> _gpuAllVmDicByCoinId = new Dictionary<Guid, GpuProfileViewModel>();

        private GpuProfileViewModels() {
            VirtualRoot.On<GpuProfileAddedOrUpdatedEvent>(
                "添加或更新了Gpu超频数据后刷新VM内存",
                LogEnum.Console,
                action: message => {
                    var vm = _list.FirstOrDefault(a => a.CoinId == message.Source.CoinId && a.Index == message.Source.Index);
                    if (vm != null) {
                        vm.Update(message.Source);
                    }
                    else {
                        if (GpuViewModels.Current.TryGetGpuVm(message.Source.Index, out GpuViewModel gpuVm)) {
                            _list.Add(new GpuProfileViewModel(message.Source, gpuVm));
                        }
                    }
                });
            foreach (var coin in NTMinerRoot.Current.CoinSet) {
                Guid coinId = coin.GetId();
                foreach (var gpuVm in GpuViewModels.Current) {
                    IGpuProfile data = GpuProfileSet.Instance.GetGpuProfile(coinId, gpuVm.Index);
                    var vm = new GpuProfileViewModel(data, gpuVm);
                    _list.Add(vm);
                    if (gpuVm.Index == NTMinerRoot.GpuAllId) {
                        _gpuAllVmDicByCoinId.Add(coinId, vm);
                    }
                }
            }
        }

        public GpuProfileViewModel GpuAllVm(Guid coinId) {
            if (_gpuAllVmDicByCoinId.TryGetValue(coinId, out GpuProfileViewModel result)) {
                return result;
            }
            return result;
        }

        public List<GpuProfileViewModel> List(Guid coinId) {
            return _list.Where(a => a.CoinId == coinId).OrderBy(a => a.Index).ToList();
        }
    }
}
