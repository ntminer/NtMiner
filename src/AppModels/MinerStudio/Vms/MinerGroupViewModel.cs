using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.MinerStudio.Vms {
    public class MinerGroupViewModel : ViewModelBase, IMinerGroup, IEditableViewModel {
        public static readonly MinerGroupViewModel PleaseSelect = new MinerGroupViewModel(Guid.Empty) {
            _name = "不指定"
        };

        private Guid _id;
        private string _name;
        private string _description;

        public ICommand Edit { get; private set; }
        public ICommand Remove { get; private set; }
        public ICommand Save { get; private set; }

        [Obsolete(message: NTKeyword.WpfDesignOnly, error: true)]
        public MinerGroupViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException(NTKeyword.WpfDesignOnly);
            }
        }

        public MinerGroupViewModel(Guid id) {
            _id = id;
            this.Save = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                if (RpcRoot.IsOuterNet) {
                    RpcRoot.OfficialServer.UserMinerGroupService.AddOrUpdateMinerGroupAsync(new MinerGroupData().Update(this), (response, e) => {
                        if (response.IsSuccess()) {
                            if (MinerStudioRoot.MinerGroupVms.TryGetMineWorkVm(this.Id, out MinerGroupViewModel vm)) {
                                VirtualRoot.RaiseEvent(new MinerGroupUpdatedEvent(Guid.Empty, this));
                            }
                            else {
                                VirtualRoot.RaiseEvent(new MinerGroupAddedEvent(Guid.Empty, this));
                            }
                            VirtualRoot.Execute(new CloseWindowCommand(this.Id));
                        }
                    });
                }
                else {
                    if (NTMinerContext.Instance.MinerStudioContext.MinerGroupSet.Contains(this.Id)) {
                        VirtualRoot.Execute(new UpdateMinerGroupCommand(this));
                    }
                    else {
                        VirtualRoot.Execute(new AddMinerGroupCommand(this));
                    }
                    VirtualRoot.Execute(new CloseWindowCommand(this.Id));
                }
            });
            this.Edit = new DelegateCommand<FormType?>((formType) => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                VirtualRoot.Execute(new EditMinerGroupCommand(formType ?? FormType.Edit, this));
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定删除 “{this.Name}” 矿机分组吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemoveMinerGroupCommand(this.Id));
                }));
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

        public bool IsPleaseSelect {
            get {
                return this == PleaseSelect;
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
                    if (MinerStudioRoot.MinerGroupVms.List.Any(a => a.Name == value && a.Id != this.Id)) {
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
