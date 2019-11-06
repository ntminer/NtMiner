using System;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class PackagesWindowViewModel : ViewModelBase {
        public ICommand Add { get; private set; }

        public PackagesWindowViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Add = new DelegateCommand(() => {
                new PackageViewModel(Guid.NewGuid()).Edit.Execute(FormType.Add);
            });
        }

        public AppContext.PackageViewModels PackageVms {
            get {
                return AppContext.Instance.PackageVms;
            }
        }
    }
}
