using NTMiner.Core;

namespace NTMiner.Vms {
    public class KernelInputFragmentViewModel : ViewModelBase, IKernelInputFragment {
        private string _name;
        private string _fragment;
        private string _description;

        public KernelInputFragmentViewModel() { }

        public KernelInputFragmentViewModel(IKernelInputFragment data) {
            _name = data.Name;
            _fragment = data.Fragment;
            _description = data.Description;
        }

        public string Name {
            get => _name;
            set {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Fragment {
            get => _fragment;
            set {
                _fragment = value;
                OnPropertyChanged(nameof(Fragment));
            }
        }

        public string Description {
            get => _description;
            set {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
    }
}
