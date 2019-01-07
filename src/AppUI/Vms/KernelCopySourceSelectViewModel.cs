namespace NTMiner.Vms {
    public class KernelCopySourceSelectViewModel : ViewModelBase {
        private KernelViewModel _selectedKernelVm;

        public KernelCopySourceSelectViewModel() {
            if (!Design.IsInDesignMode) {
                throw new System.InvalidProgramException();
            }
        }

        public KernelCopySourceSelectViewModel(KernelViewModel kernelVm) {
            this.KernelVm = kernelVm;
        }

        public KernelViewModel KernelVm { get; private set; }

        public KernelViewModel SelectedKernelVm {
            get => _selectedKernelVm;
            set {
                _selectedKernelVm = value;
                OnPropertyChanged(nameof(SelectedKernelVm));
                TopWindow.GetTopWindow().Close();
            }
        }
    }
}
