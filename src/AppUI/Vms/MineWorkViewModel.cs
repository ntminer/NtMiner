using NTMiner.Core;
using NTMiner.Core.Impl;
using NTMiner.MinerServer;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using System;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class MineWorkViewModel : ViewModelBase, IMineWork, IEditableViewModel {
        public static readonly MineWorkViewModel PleaseSelect = new MineWorkViewModel(Guid.Empty) {
            _name = "请选择"
        };
        public static readonly MineWorkViewModel FreeMineWork = new MineWorkViewModel(Guid.Empty) {
            _name = "自由作业"
        };

        private Guid _id;
        private string _name;
        private string _description;

        public string Sha1 { get; private set; }

        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand Save { get; private set; }

        public Action CloseWindow { get; set; }

        public MineWorkViewModel() {
            if (!Design.IsInDesignMode) {
                throw new InvalidProgramException();
            }
        }

        public MineWorkViewModel(IMineWork mineWork) : this(mineWork.GetId()) {
            _name = mineWork.Name;
            _description = mineWork.Description;
        }

        public MineWorkViewModel(MineWorkViewModel vm) : this((IMineWork)vm) {
            Sha1 = vm.Sha1;
        }

        public MineWorkViewModel(Guid id) {
            _id = id;
            this.Save = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                if (string.IsNullOrEmpty(this.Name)) {
                    throw new ValidationException("作业名称是必须的");
                }
                bool isMinerProfileChanged = false;
                if (NTMinerRoot.Current.MineWorkSet.Contains(this.Id)) {
                    string sha1 = NTMinerRoot.Current.MinerProfile.GetSha1();
                    if (this.Sha1 != sha1) {
                        isMinerProfileChanged = true;
                    }
                    VirtualRoot.Execute(new UpdateMineWorkCommand(this));
                    CloseWindow?.Invoke();
                }
                else {
                    isMinerProfileChanged = true;
                    VirtualRoot.Execute(new AddMineWorkCommand(this));
                    CloseWindow?.Invoke();
                    this.Edit.Execute(FormType.Edit);
                }
                if (isMinerProfileChanged) {
                    Write.DevLine("检测到MinerProfile状态变更");
                    // TODO:调用中控服务的导出json
                }
            });
            this.Edit = new DelegateCommand<FormType?>((formType) => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                MineWorkViewModel mineWorkVm;
                if (!MineWorkViewModels.Current.TryGetMineWorkVm(this.Id, out mineWorkVm)) {
                    MineWorkAdd.ShowWindow(formType ?? FormType.Edit, new MineWorkViewModel(this));
                }
                else {
                    // 编辑作业前切换上下文
                    VirtualRoot.Execute(new SwichMinerProfileCommand(this.Id));
                    this.Sha1 = NTMinerRoot.Current.MinerProfile.GetSha1();
                    MineWorkEdit.ShowWindow(formType ?? FormType.Edit, new MineWorkViewModel(this));
                }
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                DialogWindow.ShowDialog(message: $"您确定删除吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemoveMineWorkCommand(this.Id));
                }, icon: "Icon_Confirm");
            });
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
            get { return _name; }
            set {
                if (_name != value) {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                    if (this == PleaseSelect) {
                        return;
                    }
                    if (this == FreeMineWork) {
                        return;
                    }
                    if (string.IsNullOrEmpty(value)) {
                        throw new ValidationException("名称是必须的");
                    }
                    if (MineWorkViewModels.Current.List.Any(a=>a.Name == value && a.Id != this.Id)) {
                        throw new ValidationException("名称重复");
                    }
                }
            }
        }

        public MinerProfileViewModel MinerProfile {
            get {
                return MinerProfileViewModel.Current;
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
