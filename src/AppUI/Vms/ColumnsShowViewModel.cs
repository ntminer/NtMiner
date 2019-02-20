using NTMiner.MinerServer;
using System.Reflection;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class ColumnsShowViewModel : ViewModelBase, IColumnsShow {
        private bool _work = true;
        private bool _minerName = true;
        private bool _minerIp = true;
        private bool _minerGroup = true;
        private bool _mainCoinCode = true;
        private bool _mainCoinSpeedText = true;
        private bool _mainCoinWallet = true;
        private bool _mainCoinPool = true;
        private bool _kernel = true;
        private bool _dualCoinCode = true;
        private bool _dualCoinSpeedText = true;
        private bool _dualCoinWallet = true;
        private bool _dualCoinPool = true;
        private bool _lastActivedOnText = true;
        private bool _version = true;
        private bool _remoteUserNameAndPassword = true;
        private bool _gpuInfo = true;
        private bool _mainCoinRejectPercentText = true;
        private bool _dualCoinRejectPercentText = true;

        public ICommand Hide { get; private set; }

        public ColumnsShowViewModel() {
            this.Hide = new DelegateCommand<string>((propertyName) => {
                PropertyInfo propertyInfo = this.GetType().GetProperty(propertyName);
                if (propertyInfo != null) {
                    propertyInfo.SetValue(this, false, null);
                }
            });
        }

        public bool Work {
            get => _work;
            set {
                _work = value;
                OnPropertyChanged(nameof(Work));
            }
        }

        public bool MinerName {
            get { return _minerName; }
            set {
                _minerName = value;
                OnPropertyChanged(nameof(MinerName));
            }
        }

        public bool MinerIp {
            get { return _minerIp; }
            set {
                _minerIp = value;
                OnPropertyChanged(nameof(MinerIp));
            }
        }

        public bool MinerGroup {
            get { return _minerGroup; }
            set {
                _minerGroup = value;
                OnPropertyChanged(nameof(MinerGroup));
            }
        }

        public bool MainCoinCode {
            get {
                return _mainCoinCode;
            }
            set {
                _mainCoinCode = value;
                OnPropertyChanged(nameof(MainCoinCode));
            }
        }

        public bool MainCoinSpeedText {
            get { return _mainCoinSpeedText; }
            set {
                _mainCoinSpeedText = value;
                OnPropertyChanged(nameof(MainCoinSpeedText));
            }
        }

        public bool MainCoinRejectPercentText {
            get => _mainCoinRejectPercentText;
            set {
                _mainCoinRejectPercentText = value;
                OnPropertyChanged(nameof(MainCoinRejectPercentText));
            }
        }

        public bool MainCoinWallet {
            get => _mainCoinWallet;
            set {
                _mainCoinWallet = value;
                OnPropertyChanged(nameof(MainCoinWallet));
            }
        }

        public bool MainCoinPool {
            get => _mainCoinPool;
            set {
                _mainCoinPool = value;
                OnPropertyChanged(nameof(MainCoinPool));
            }
        }

        public bool Kernel {
            get => _kernel;
            set {
                _kernel = value;
                OnPropertyChanged(nameof(Kernel));
            }
        }
        public bool DualCoinCode {
            get => _dualCoinCode;
            set {
                _dualCoinCode = value;
                OnPropertyChanged(nameof(DualCoinCode));
            }
        }

        public bool DualCoinSpeedText {
            get => _dualCoinSpeedText;
            set {
                _dualCoinSpeedText = value;
                OnPropertyChanged(nameof(DualCoinSpeedText));
            }
        }

        public bool DualCoinRejectPercentText {
            get => _dualCoinRejectPercentText;
            set {
                _dualCoinRejectPercentText = value;
                OnPropertyChanged(nameof(DualCoinRejectPercentText));
            }
        }

        public bool DualCoinWallet {
            get => _dualCoinWallet;
            set {
                _dualCoinWallet = value;
                OnPropertyChanged(nameof(DualCoinWallet));
            }
        }

        public bool DualCoinPool {
            get => _dualCoinPool;
            set {
                _dualCoinPool = value;
                OnPropertyChanged(nameof(DualCoinPool));
            }
        }

        public bool LastActivedOnText {
            get => _lastActivedOnText;
            set {
                _lastActivedOnText = value;
                OnPropertyChanged(nameof(LastActivedOnText));
            }
        }

        public bool Version {
            get => _version;
            set {
                _version = value;
                OnPropertyChanged(nameof(Version));
            }
        }

        public bool RemoteUserNameAndPassword {
            get => _remoteUserNameAndPassword;
            set {
                _remoteUserNameAndPassword = value;
                OnPropertyChanged(nameof(RemoteUserNameAndPassword));
            }
        }

        public bool GpuInfo {
            get => _gpuInfo;
            set {
                _gpuInfo = value;
                OnPropertyChanged(nameof(GpuInfo));
            }
        }
    }
}
