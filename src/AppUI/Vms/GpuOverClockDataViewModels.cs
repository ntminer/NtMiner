using NTMiner.Core;
using NTMiner.Core.Gpus;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class GpuOverClockDataViewModels : ViewModelBase {
        public static readonly GpuOverClockDataViewModels Current = new GpuOverClockDataViewModels();

        private readonly Dictionary<int, GpuOverClockDataViewModel> _dicByIndex = new Dictionary<int, GpuOverClockDataViewModel>();

        private GpuOverClockDataViewModels() {
            VirtualRoot.On<GpuOverClockDataAddedOrUpdatedEvent>(
                "添加或更新了Gpu超频数据后刷新VM内存",
                LogEnum.Console,
                action: message => {
                    GpuOverClockDataViewModel vm;
                    if (_dicByIndex.TryGetValue(message.Source.Index, out vm)) {
                        vm.Update(message.Source);
                    }
                    else {
                        _dicByIndex.Add(message.Source.Index, new GpuOverClockDataViewModel(message.Source));
                    }
                });
            foreach (var gpu in NTMinerRoot.Current.GpuSet) {
                IGpuOverClockData data = NTMinerRoot.Current.GpuOverClockDataSet.GetGpuOverClockData(gpu.Index);
                _dicByIndex.Add(data.Index, new GpuOverClockDataViewModel(data) {
                    Name = gpu.Name
                });
            }
        }

        public List<GpuOverClockDataViewModel> List {
            get {
                return _dicByIndex.Values.ToList();
            }
        }
    }
}
