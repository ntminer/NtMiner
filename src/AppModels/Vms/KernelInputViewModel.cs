using NTMiner.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class KernelInputViewModel : ViewModelBase, IKernelInput, IEditableViewModel {
        public static readonly KernelInputViewModel PleaseSelect = new KernelInputViewModel(Guid.Empty) {
            _name = "请选择"
        };

        private Guid _id;
        private string _name;
        private string _args;
        private bool _isSupportDualMine;
        private double _dualWeightMin;
        private double _dualWeightMax;
        private bool _isAutoDualWeight;
        private string _dualWeightArg;
        private string _devicesArg;
        private int _deviceBaseIndex;
        private string _devicesSeparator;

        private bool _isDeviceAllNotEqualsNone;
        private string _nDevicePrefix;
        private string _nDevicePostfix;
        private string _aDevicePrefix;
        private string _aDevicePostfix;

        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand Save { get; private set; }

        public KernelInputViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException();
            }
        }

        public KernelInputViewModel(IKernelInput data) : this(data.GetId()) {
            _name = data.Name;
            _args = data.Args;
            _isSupportDualMine = data.IsSupportDualMine;
            _dualWeightMin = data.DualWeightMin;
            _dualWeightMax = data.DualWeightMax;
            _isAutoDualWeight = data.IsAutoDualWeight;
            _dualWeightArg = data.DualWeightArg;
            _deviceBaseIndex = data.DeviceBaseIndex;
            _isDeviceAllNotEqualsNone = data.IsDeviceAllNotEqualsNone;
            _devicesArg = data.DevicesArg;
            _devicesSeparator = data.DevicesSeparator;
            _nDevicePrefix = data.NDevicePrefix;
            _nDevicePostfix = data.NDevicePostfix;
            _aDevicePrefix = data.ADevicePrefix;
            _aDevicePostfix = data.ADevicePostfix;
        }

        public KernelInputViewModel(Guid id) {
            _id = id;
            this.Save = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                if (NTMinerRoot.Instance.ServerContext.KernelInputSet.Contains(this.Id)) {
                    VirtualRoot.Execute(new UpdateKernelInputCommand(this));
                }
                else {
                    VirtualRoot.Execute(new AddKernelInputCommand(this));
                }
                VirtualRoot.Execute(new CloseWindowCommand(this.Id));
            });
            this.Edit = new DelegateCommand<FormType?>((formType) => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                VirtualRoot.Execute(new KernelInputEditCommand(formType ?? FormType.Edit, this));
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定删除{this.Name}内核输入吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemoveKernelInputCommand(this.Id));
                }));
            });
        }

        public AppContext.GroupViewModels GroupVms {
            get {
                return AppContext.Instance.GroupVms;
            }
        }

        public string ParameterNames {
            get {
                return string.Join("、", new string[] {
                    NTKeyword.MainCoinParameterName, NTKeyword.WalletParameterName,
                    NTKeyword.UserNameParameterName, NTKeyword.PasswordParameterName,
                    NTKeyword.HostParameterName, NTKeyword.PortParameterName,
                    NTKeyword.PoolParameterName, NTKeyword.WorkerParameterName
                });
            }
        }

        public string DualParameterNames {
            get {
                return string.Join("、", new string[] {
                    NTKeyword.DualCoinParameterName, NTKeyword.DualWalletParameterName,
                    NTKeyword.DualUserNameParameterName, NTKeyword.DualPasswordParameterName,
                    NTKeyword.DualHostParameterName, NTKeyword.DualPortParameterName,
                    NTKeyword.DualPoolParameterName
                });
            }
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id {
            get => _id;
            private set {
                if (_id != value) {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public string Name {
            get { return _name; }
            set {
                if (_name != value) {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string Args {
            get { return _args; }
            set {
                if (_args != value) {
                    _args = value;
                    OnPropertyChanged(nameof(Args));
                }
            }
        }

        public bool IsSupportDualMine {
            get => _isSupportDualMine;
            set {
                if (_isSupportDualMine != value) {
                    _isSupportDualMine = value;
                    OnPropertyChanged(nameof(IsSupportDualMine));
                }
            }
        }

        public Visibility IsSupportDualMineVisible {
            get {
                if (DevMode.IsDebugMode) {
                    return Visibility.Visible;
                }
                if (IsSupportDualMine) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public double DualWeightMin {
            get => _dualWeightMin;
            set {
                if (_dualWeightMin != value) {
                    _dualWeightMin = value;
                    OnPropertyChanged(nameof(DualWeightMin));
                }
            }
        }

        public double DualWeightMax {
            get => _dualWeightMax;
            set {
                if (_dualWeightMax != value) {
                    _dualWeightMax = value;
                    OnPropertyChanged(nameof(DualWeightMax));
                }
            }
        }

        public bool IsAutoDualWeight {
            get => _isAutoDualWeight;
            set {
                if (_isAutoDualWeight != value) {
                    _isAutoDualWeight = value;
                    OnPropertyChanged(nameof(IsAutoDualWeight));
                }
            }
        }

        public string DualWeightArg {
            get {
                return _dualWeightArg;
            }
            set {
                if (_dualWeightArg != value) {
                    _dualWeightArg = value;
                    OnPropertyChanged(nameof(DualWeightArg));
                }
            }
        }

        public string KernelFullNames {
            get {
                string names = string.Join(";", AppContext.Instance.KernelVms.AllKernels.Where(a => a.KernelInputId == this.Id).Select(a => a.FullName));
                if (string.IsNullOrEmpty(names)) {
                    return "无";
                }
                return names;
            }
        }

        public bool IsDeviceAllNotEqualsNone {
            get => _isDeviceAllNotEqualsNone;
            set {
                _isDeviceAllNotEqualsNone = value;
                OnPropertyChanged(nameof(IsDeviceAllNotEqualsNone));
            }
        }

        public string DevicesArg {
            get => _devicesArg;
            set {
                _devicesArg = value;
                OnPropertyChanged(nameof(DevicesArg));
                OnPropertyChanged(nameof(DevicesArgShape));
                OnPropertyChanged(nameof(IsSupportDevicesArg));
            }
        }

        public int DeviceBaseIndex {
            get => _deviceBaseIndex;
            set {
                _deviceBaseIndex = value;
                OnPropertyChanged(nameof(DeviceBaseIndex));
                OnPropertyChanged(nameof(DevicesArgShape));
            }
        }

        public string DevicesSeparator {
            get => _devicesSeparator;
            set {
                _devicesSeparator = value;
                OnPropertyChanged(nameof(DevicesSeparator));
                OnPropertyChanged(nameof(DevicesArgShape));
                OnPropertyChanged(nameof(IsDevicesSpaceSeparator));
            }
        }

        public bool IsDevicesSpaceSeparator {
            get {
                return DevicesSeparator == VirtualRoot.SpaceKeyword;
            }
        }

        public string DevicesArgShape {
            get {
                if (string.IsNullOrEmpty(DevicesArg)) {
                    return string.Empty;
                }
                List<int> list = new List<int>();
                for (int i = 0; i < 6; i++) {
                    list.Add(i + DeviceBaseIndex);
                }
                string separator = DevicesSeparator;
                if (IsDevicesSpaceSeparator) {
                    separator = " ";
                }
                return $"{DevicesArg} {string.Join(separator, list)}";
            }
        }

        public bool IsSupportDevicesArg {
            get {
                if (VirtualRoot.IsMinerStudio) {
                    return false;
                }
                return !string.IsNullOrWhiteSpace(DevicesArg);
            }
        }

        public string NDevicePrefix {
            get => _nDevicePrefix;
            set {
                _nDevicePrefix = value;
                OnPropertyChanged(nameof(NDevicePrefix));
            }
        }

        public string NDevicePostfix {
            get => _nDevicePostfix;
            set {
                _nDevicePostfix = value;
                OnPropertyChanged(nameof(NDevicePostfix));
            }
        }

        public string ADevicePrefix {
            get => _aDevicePrefix;
            set {
                _aDevicePrefix = value;
                OnPropertyChanged(nameof(ADevicePrefix));
            }
        }

        public string ADevicePostfix {
            get => _aDevicePostfix;
            set {
                _aDevicePostfix = value;
                OnPropertyChanged(nameof(ADevicePostfix));
            }
        }
    }
}
