namespace NTMiner.MinerStudio.Vms {
    public class GpuNamePageViewModel {
        public GpuNamePageViewModel() {
            this.GpuNamesVm = new GpuNamesViewModel(OnQueryResponsed);
            this.GpuNameCountsVm = new GpuNameCountsViewModel(OnQueryResponsed);
        }

        private void OnQueryResponsed() {
            if (this.GpuNamesVm.GpuNames.Count > 0 && this.GpuNameCountsVm.GpuNameCounts.Count > 0) {
                foreach (var item in this.GpuNameCountsVm.GpuNameCounts) {
                    item.Match(this.GpuNamesVm.GpuNames);
                }
            }
        }

        public GpuNamesViewModel GpuNamesVm { get; private set; }

        public GpuNameCountsViewModel GpuNameCountsVm { get; private set; }
    }
}
