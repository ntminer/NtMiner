using NTMiner.Core;
using NTMiner.MinerServer;
using System;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class MinerGroupViewModel : ViewModelBase, IMinerGroup, IEditableViewModel {
        public static readonly MinerGroupViewModel PleaseSelect = new MinerGroupViewModel(Guid.Empty) {
            _name = "不指定"
        };

        private Guid _id;
        private string _name;
        private string _description;

        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand Save { get; private set; }

        [Obsolete("这是供WPF设计时使用的构造，不应在业务代码中被调用")]
        public MinerGroupViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException();
            }
        }

        public MinerGroupViewModel(Guid id) {
            _id = id;
            this.Save = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                if (NTMinerRoot.Instance.MinerGroupSet.TryGetMinerGroup(this.Id, out IMinerGroup group)) {
                    VirtualRoot.Execute(new UpdateMinerGroupCommand(this));
                }
                else {
                    VirtualRoot.Execute(new AddMinerGroupCommand(this));
                }
                VirtualRoot.Execute(new CloseWindowCommand(this.Id));
            });
            this.Edit = new DelegateCommand<FormType?>((formType) => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                VirtualRoot.Execute(new MinerGroupEditCommand(formType ?? FormType.Edit, this));
            }, (formType) => {
                return this != PleaseSelect;
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定删除{this.Name}吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemoveMinerGroupCommand(this.Id));
                }));
            }, () => {
                return this != PleaseSelect;
            });
        }

        public MinerGroupViewModel(IMinerGroup data) : this(data.GetId()) {
            _name = data.Name;
            _description = data.Description;
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id {
            get => _id;
            set {
                if (_id != value) {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public string Name {
            get => _name;
            set {
                if (_name != value) {
                    _name = value;
                    if (this == PleaseSelect) {
                        return;
                    }
                    OnPropertyChanged(nameof(Name));
                    if (string.IsNullOrEmpty(value)) {
                        throw new ValidationException("名称是必须的");
                    }
                    if (AppContext.Instance.MinerGroupVms.List.Any(a => a.Name == value && a.Id != this.Id)) {
                        throw new ValidationException("名称重复");
                    }
                }
            }
        }

        public string Description {
            get => _description;
            set {
                if (_description != value) {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }
    }
}
