namespace NTMiner.MinerStudio.Vms {
    public class GpuNamePageViewModel {
        public GpuNamePageViewModel() {
            this.GpuNamesVm = new GpuNamesViewModel();
            this.GpuNameCountsVm = new GpuNameCountsViewModel();
        }

        public GpuNamesViewModel GpuNamesVm { get; private set; }

        public GpuNameCountsViewModel GpuNameCountsVm { get; private set; }
    }
}
