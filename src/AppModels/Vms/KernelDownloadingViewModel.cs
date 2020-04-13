using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class KernelDownloadingViewModel : ViewModelBase {
        public KernelDownloadingViewModel() { }

        public void Download(Guid kernelId, Action<bool, string> downloadComplete) {
            if (AppRoot.KernelVms.TryGetKernelVm(kernelId, out KernelViewModel kernelVm)) {
                kernelVm.KernelProfileVm.Download(downloadComplete);
            }
        }

        public List<KernelViewModel> DownloadingVms {
            get {
                return AppRoot.KernelVms.AllKernels.Where(a => a.KernelProfileVm.IsDownloading).OrderBy(a => a.Code + a.Version).ToList();
            }
        }
    }
}
