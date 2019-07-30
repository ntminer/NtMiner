using System.Diagnostics;
using System.IO;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class BrandTagViewModel : ViewModelBase {
        public ICommand TagKernelBrand { get; private set; }
        public ICommand TagPoolBrand { get; private set; }

        public BrandTagViewModel() {
            this.TagKernelBrand = new DelegateCommand<SysDicItemViewModel>(brandItem => {
                string outFileName = Path.GetFileNameWithoutExtension(VirtualRoot.AppFileFullName) + $"_{brandItem.Value}.exe";
                string outDir = Path.GetDirectoryName(VirtualRoot.AppFileFullName);
                string outFileFullName = Path.Combine(outDir, outFileName);
                VirtualRoot.TagBrandId(Consts.KernelBrandId, brandItem.GetId(), VirtualRoot.AppFileFullName, outFileFullName);
                NotiCenterWindowViewModel.Instance.Manager.ShowSuccessMessage($"打码成功:{outFileName}");
                Process.Start(outDir);
            }, brandItem => brandItem != SysDicItemViewModel.PleaseSelect);
            this.TagPoolBrand = new DelegateCommand<SysDicItemViewModel>(brandItem => {
                string outFileName = Path.GetFileNameWithoutExtension(VirtualRoot.AppFileFullName) + $"_{brandItem.Value}.exe";
                string outDir = Path.GetDirectoryName(VirtualRoot.AppFileFullName);
                string outFileFullName = Path.Combine(outDir, outFileName);
                VirtualRoot.TagBrandId(Consts.PoolBrandId, brandItem.GetId(), VirtualRoot.AppFileFullName, outFileFullName);
                NotiCenterWindowViewModel.Instance.Manager.ShowSuccessMessage($"打码成功:{outFileName}");
                Process.Start(outDir);
            }, brandItem => brandItem != SysDicItemViewModel.PleaseSelect);
        }

        public AppContext.SysDicItemViewModels SysDicItemVms {
            get {
                return AppContext.Instance.SysDicItemVms;
            }
        }
    }
}
