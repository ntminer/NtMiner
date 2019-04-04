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

        public bool IsChecked {
            get {
                if (MinerProfileViewModel.Current.CoinVm?.CoinKernel?.CoinKernelProfile?.CustomArgs?.Contains(this.Fragment) ?? false) {
                    return true;
                }
                return false;
            }
            set {
                CoinKernelProfileViewModel coinKernelProfileVm = MinerProfileViewModel.Current.CoinVm?.CoinKernel?.CoinKernelProfile;
                string customArgs = coinKernelProfileVm?.CustomArgs ?? string.Empty;
                bool b = customArgs.Contains(this.Fragment);
                if (value) {
                    if (b == false) {
                        if (string.IsNullOrEmpty(customArgs)) {
                            customArgs = this.Fragment;
                        }
                        else {
                            customArgs += " " + this.Fragment;
                        }
                    }
                }
                else {
                    if (b == true) {
                        string str = " " + this.Fragment;
                        if (!customArgs.Contains(str)) {
                            str = this.Fragment;
                        }
                        customArgs = customArgs.Replace(str, string.Empty);
                    }
                }
                if (coinKernelProfileVm != null) {
                    coinKernelProfileVm.CustomArgs = customArgs;
                }
                OnPropertyChanged(nameof(IsChecked));
            }
        }
    }
}
