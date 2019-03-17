using NTMiner.Core;
using NTMiner.Profile;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class GpuProfileViewModels : ViewModelBase {
        public static readonly GpuProfileViewModels Current = new GpuProfileViewModels();

        private readonly Dictionary<Guid, GpuProfileViewModel> _dicById = new Dictionary<Guid, GpuProfileViewModel>();
        private readonly Dictionary<Guid, GpuProfileViewModel> _gpuAllVmDicByCoinId = new Dictionary<Guid, GpuProfileViewModel>();

        private GpuProfileViewModels() {
            VirtualRoot.On<GpuProfileAddedOrUpdatedEvent>(
                "添加或更新了Gpu超频数据后刷新VM内存",
                LogEnum.Console,
                action: message => {
                    GpuProfileViewModel vm;
                    if (_dicById.TryGetValue(message.Source.GetId(), out vm)) {
                        vm.Update(message.Source);
                    }
                    else {
                        _dicById.Add(message.Source.GetId(), new GpuProfileViewModel(message.Source));
                    }
                });
            foreach (var coin in NTMinerRoot.Current.CoinSet) {
                Guid coinId = coin.GetId();
                foreach (var gpu in NTMinerRoot.Current.GpuSet) {
                    IGpuProfile data = NTMinerRoot.Current.MinerProfile.GetGpuProfile(coinId, gpu.Index);
                    var vm = new GpuProfileViewModel(data);
                    _dicById.Add(data.GetId(), vm);
                    if (gpu.Index == NTMinerRoot.GpuAllId) {
                        _gpuAllVmDicByCoinId.Add(coinId, vm);
                    }
                }
            }
            VirtualRoot.On<MinerProfileReInitedEvent>(
                "MinerProfile切换后刷新Vm内存",
                LogEnum.Console,
                action: message => {
                    _dicById.Clear();
                    _gpuAllVmDicByCoinId.Clear();
                });
        }

        public GpuProfileViewModel GpuAllVm(Guid coinId) {
            GpuProfileViewModel result;
            if (_gpuAllVmDicByCoinId.TryGetValue(coinId, out result)) {
                return result;
            }
            return result;
        }

        public List<GpuProfileViewModel> List(Guid coinId) {
            return _dicById.Values.Where(a => a.CoinId == coinId).OrderBy(a => a.Index).ToList();
        }
    }
}
