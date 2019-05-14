using System;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class KernelInputPageViewModel : ViewModelBase {
        public ICommand Add { get; private set; }

        public KernelInputPageViewModel() {
            if (Design.IsInDesignMode) {
                return;
            }
            this.Add = new DelegateCommand(() => {
                new KernelInputViewModel(Guid.NewGuid()).Edit.Execute(FormType.Add);
            });
        }

        public AppContext.KernelInputViewModels KernelInputVms {
            get {
                return AppContext.Instance.KernelInputVms;
            }
        }
    }
}
