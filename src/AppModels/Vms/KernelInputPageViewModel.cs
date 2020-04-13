using System;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class KernelInputPageViewModel : ViewModelBase {
        public ICommand Add { get; private set; }

        public KernelInputPageViewModel() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Add = new DelegateCommand(() => {
                new KernelInputViewModel(Guid.NewGuid()).Edit.Execute(FormType.Add);
            });
        }

        public AppRoot.KernelInputViewModels KernelInputVms {
            get {
                return AppRoot.KernelInputVms;
            }
        }
    }
}
