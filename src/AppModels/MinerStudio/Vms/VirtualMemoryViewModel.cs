using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.MinerStudio.Vms {
    public class VirtualMemoryViewModel : ViewModelBase {
        private List<DriveViewModel> _drives = new List<DriveViewModel>();
        private bool _isLoading = true;

        public ICommand Apply { get; private set; }

        [Obsolete(message: NTKeyword.WpfDesignOnly, error: true)]
        public VirtualMemoryViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException();
            }
        }

        public VirtualMemoryViewModel(MinerClientViewModel minerClientVm) {
            this.MinerClientVm = minerClientVm;
            this.Apply = new DelegateCommand(() => {
                MinerStudioService.Instance.SetVirtualMemoryAsync(minerClientVm, _drives.ToDictionary(a => a.Name, a => a.VirtualMemoryMaxSizeMb));
                OnPropertyChanged(nameof(TotalVirtualMemoryMb));
                OnPropertyChanged(nameof(IsStateChanged));
            });
        }

        public bool IsLoading {
            get => _isLoading;
            set {
                if (_isLoading != value) {
                    _isLoading = value;
                    OnPropertyChanged(nameof(IsLoading));
                }
            }
        }

        public MinerClientViewModel MinerClientVm { get; set; }

        public List<DriveViewModel> Drives {
            get {
                return _drives;
            }
            set {
                _drives = value;
                OnPropertyChanged(nameof(Drives));
                OnPropertyChanged(nameof(IsStateChanged));
                OnPropertyChanged(nameof(TotalVirtualMemoryMb));
                IsLoading = false;
            }
        }

        public bool IsStateChanged {
            get {
                if (_drives.Any(a => a.VirtualMemoryMaxSizeMb != a.InitialVirtualMemoryMaxSizeMb)) {
                    return true;
                }
                return false;
            }
        }


        public int TotalVirtualMemoryMb {
            get {
                return _drives.Sum(a => a.VirtualMemoryMaxSizeMb);
            }
        }

        public string Description {
            get {
                return AppRoot.VirtualMemoryDescription;
            }
        }
    }
}
