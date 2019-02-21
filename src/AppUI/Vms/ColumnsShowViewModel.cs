using NTMiner.Core;
using NTMiner.MinerServer;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using System;
using System.Reflection;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class ColumnsShowViewModel : ViewModelBase, IColumnsShow {
        // 默认列显，可编辑
        public static readonly ColumnsShowViewModel PleaseSelect = new ColumnsShowViewModel(Guid.Empty) {
            ColumnsShowName = "请选择"
        };

        private Guid _id;
        private string _columnsShowName;
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
        private bool _bootTimeSpanText = true;
        private bool _mineTimeSpanText = true;

        public ICommand Hide { get; private set; }

        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand Save { get; private set; }

        public Action CloseWindow { get; set; }

        public ColumnsShowViewModel() {
            if (!Design.IsInDesignMode) {
                throw new InvalidProgramException();
            }
        }

        public ColumnsShowViewModel(Guid id) {
            this.Id = id;
            this.Hide = new DelegateCommand<string>((propertyName) => {
                PropertyInfo propertyInfo = this.GetType().GetProperty(propertyName);
                if (propertyInfo != null) {
                    propertyInfo.SetValue(this, false, null);
                }
            });
            this.Save = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                if (NTMinerRoot.Current.ColumnsShowSet.Contains(this.Id)) {
                    VirtualRoot.Execute(new UpdateColumnsShowCommand(this));
                }
                else {
                    VirtualRoot.Execute(new AddColumnsShowCommand(this));
                }
                CloseWindow?.Invoke();
            });
            this.Edit = new DelegateCommand(() => {
                ColumnsShowEdit.ShowEditWindow(this);
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                DialogWindow.ShowDialog(message: $"您确定删除{this.ColumnsShowName}吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemoveColumnsShowCommand(this.Id));
                }, icon: "Icon_Confirm");
            });
        }

        public ColumnsShowViewModel(IColumnsShow data) : this(data.GetId()) {
            _columnsShowName = data.ColumnsShowName;
            _work = data.Work;
            _minerName = data.MinerName;
            _minerIp = data.MinerIp;
            _minerGroup = data.MinerGroup;
            _mainCoinCode = data.MainCoinCode;
            _mainCoinSpeedText = data.MainCoinSpeedText;
            _mainCoinWallet = data.MainCoinWallet;
            _mainCoinPool = data.MainCoinPool;
            _kernel = data.Kernel;
            _dualCoinCode = data.DualCoinCode;
            _dualCoinSpeedText = data.DualCoinSpeedText;
            _dualCoinWallet = data.DualCoinWallet;
            _dualCoinPool = data.DualCoinPool;
            _lastActivedOnText = data.LastActivedOnText;
            _version = data.Version;
            _remoteUserNameAndPassword = data.RemoteUserNameAndPassword;
            _gpuInfo = data.GpuInfo;
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id {
            get => _id;
            set {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string ColumnsShowName {
            get { return _columnsShowName; }
            set {
                _columnsShowName = value;
                OnPropertyChanged(nameof(ColumnsShowName));
            }
        }

        public bool BootTimeSpanText {
            get { return _bootTimeSpanText; }
            set {
                _bootTimeSpanText = value;
                OnPropertyChanged(nameof(BootTimeSpanText));
            }
        }

        public bool MineTimeSpanText {
            get { return _mineTimeSpanText; }
            set {
                _mineTimeSpanText = value;
                OnPropertyChanged(nameof(MineTimeSpanText));
            }
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
