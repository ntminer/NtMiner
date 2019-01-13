using NTMiner.Core;
using NTMiner.Core.Kernels;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using System;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class KernelInputViewModel : ViewModelBase, IKernelInput {
        public static readonly KernelInputViewModel PleaseSelect = new KernelInputViewModel(Guid.Empty) {
            _name = "请选择"
        };

        private Guid _id;
        private string _name;
        private string _args;
        private bool _isSupportDualMine;
        private string _dualFullArgs;
        private Guid _dualCoinGroupId;
        private double _dualWeightMin;
        private double _dualWeightMax;
        private bool _isAutoDualWeight;
        private string _dualWeightArg;

        private GroupViewModel _dualCoinGroup;

        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand Save { get; private set; }

        public Action CloseWindow { get; set; }

        public KernelInputViewModel() {
            if (!Design.IsInDesignMode) {
                throw new InvalidProgramException();
            }
        }

        public KernelInputViewModel(IKernelInput data) : this(data.GetId()) {
            _name = data.Name;
            _args = data.Args;
            _isSupportDualMine = data.IsSupportDualMine;
            _dualFullArgs = data.DualFullArgs;
            _dualCoinGroupId = data.DualCoinGroupId;
            _dualWeightMin = data.DualWeightMin;
            _dualWeightMax = data.DualWeightMax;
            _isAutoDualWeight = data.IsAutoDualWeight;
            _dualWeightArg = data.DualWeightArg;
        }

        public KernelInputViewModel(Guid id) {
            _id = id;
            this.Save = new DelegateCommand(() => {
                if (NTMinerRoot.Current.KernelInputSet.Contains(this.Id)) {
                    Global.Execute(new UpdateKernelInputCommand(this));
                }
                else {
                    Global.Execute(new AddKernelInputCommand(this));
                }
                CloseWindow?.Invoke();
            });
            this.Edit = new DelegateCommand(() => {
                KernelInputEdit.ShowEditWindow(this);
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                DialogWindow.ShowDialog(message: $"您确定删除{this.Name}内核输入吗？", title: "确认", onYes: () => {
                    Global.Execute(new RemoveKernelInputCommand(this.Id));
                }, icon: "Icon_Confirm");
            });
        }

        public GroupViewModels GroupVms {
            get {
                return GroupViewModels.Current;
            }
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id {
            get => _id;
            private set {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string Name {
            get { return _name; }
            set {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public Guid DualCoinGroupId {
            get => _dualCoinGroupId;
            set {
                _dualCoinGroupId = value;
                OnPropertyChanged(nameof(DualCoinGroupId));
            }
        }

        public GroupViewModel DualCoinGroup {
            get {
                if (this.DualCoinGroupId == Guid.Empty) {
                    return GroupViewModel.PleaseSelect;
                }
                if (_dualCoinGroup == null || _dualCoinGroup.Id != this.DualCoinGroupId) {
                    GroupViewModels.Current.TryGetGroupVm(DualCoinGroupId, out _dualCoinGroup);
                    if (_dualCoinGroup == null) {
                        _dualCoinGroup = GroupViewModel.PleaseSelect;
                    }
                }
                return _dualCoinGroup;
            }
            set {
                if (DualCoinGroupId != value.Id) {
                    DualCoinGroupId = value.Id;
                    OnPropertyChanged(nameof(DualCoinGroup));
                }
            }
        }

        public string Args {
            get { return _args; }
            set {
                _args = value;
                OnPropertyChanged(nameof(Args));
            }
        }

        public bool IsSupportDualMine {
            get => _isSupportDualMine;
            set {
                _isSupportDualMine = value;
                OnPropertyChanged(nameof(IsSupportDualMine));
            }
        }

        public Visibility IsSupportDualMineVisible {
            get {
                if (DevMode.IsDevMode) {
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
                _dualWeightMin = value;
                OnPropertyChanged(nameof(DualWeightMin));
            }
        }

        public double DualWeightMax {
            get => _dualWeightMax;
            set {
                _dualWeightMax = value;
                OnPropertyChanged(nameof(DualWeightMax));
            }
        }

        public bool IsAutoDualWeight {
            get => _isAutoDualWeight;
            set {
                _isAutoDualWeight = value;
                OnPropertyChanged(nameof(IsAutoDualWeight));
            }
        }

        public string DualWeightArg {
            get {
                return _dualWeightArg;
            }
            set {
                _dualWeightArg = value;
                OnPropertyChanged(nameof(DualWeightArg));
            }
        }

        public string DualFullArgs {
            get { return _dualFullArgs; }
            set {
                _dualFullArgs = value;
                OnPropertyChanged(nameof(DualFullArgs));
            }
        }
    }
}
