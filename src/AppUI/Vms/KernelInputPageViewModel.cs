using System;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class KernelInputPageViewModel : ViewModelBase {
        public static readonly KernelInputPageViewModel Current = new KernelInputPageViewModel();

        public ICommand Add { get; private set; }

        private KernelInputPageViewModel() {
            if (Design.IsInDesignMode) {
                return;
            }
            this.Add = new DelegateCommand(() => {
                new KernelInputViewModel(Guid.NewGuid()).Edit.Execute(FormType.Add);
            });
        }

        public KernelInputViewModels KernelInputVms {
            get {
                return KernelInputViewModels.Current;
            }
        }
    }
}
