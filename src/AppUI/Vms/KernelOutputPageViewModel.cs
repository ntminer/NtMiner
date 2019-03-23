using System;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class KernelOutputPageViewModel : ViewModelBase {
        public static readonly KernelOutputPageViewModel Current = new KernelOutputPageViewModel();

        public ICommand Add { get; private set; }

        private KernelOutputPageViewModel() {
            if (Design.IsInDesignMode) {
                return;
            }
            this.Add = new DelegateCommand(() => {
                new KernelOutputViewModel(Guid.NewGuid()).Edit.Execute(FormType.Add);
            });
            _currentKernelOutputVm = KernelOutputViewModels.Current.AllKernelOutputVms.FirstOrDefault();
        }

        private KernelOutputViewModel _currentKernelOutputVm;

        public KernelOutputViewModel CurrentKernelOutputVm {
            get {
                return _currentKernelOutputVm;
            }
            set {
                if (_currentKernelOutputVm != value) {
                    _currentKernelOutputVm = value;
                    OnPropertyChanged(nameof(CurrentKernelOutputVm));
                }
            }
        }

        public KernelOutputViewModels KernelOutputVms {
            get {
                return KernelOutputViewModels.Current;
            }
        }
    }
}
