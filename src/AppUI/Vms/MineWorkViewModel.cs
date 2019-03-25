using NTMiner.Core;
using NTMiner.Core.Impl;
using NTMiner.MinerServer;
using NTMiner.Profile;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class MineWorkViewModel : ViewModelBase, IMineWork, IEditableViewModel {
        public static readonly MineWorkViewModel PleaseSelect = new MineWorkViewModel(Guid.Empty) {
            _name = "不指定"
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
                    NotiCenterWindowViewModel.Current.Manager.ShowErrorMessage("作业名称是必须的");
                }
                bool isMinerProfileChanged = false;
                IMineWork entity;
                if (NTMinerRoot.Current.MineWorkSet.TryGetMineWork(this.Id, out entity)) {
                    string sha1 = NTMinerRoot.Current.MinerProfile.GetSha1();
                    if (this.Sha1 != sha1) {
                        isMinerProfileChanged = true;
                    }
                    if (entity.Name != this.Name || entity.Description != this.Description) {
                        VirtualRoot.Execute(new UpdateMineWorkCommand(this));
                    }
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
                    string localJson;
                    string serverJson;
                    NTMinerRoot.ExportWorkJson(new MineWorkData(this), out localJson, out serverJson);
                    if (!string.IsNullOrEmpty(localJson) && !string.IsNullOrEmpty(serverJson)) {
                        Server.ControlCenterService.ExportMineWorkAsync(this.Id, localJson, serverJson, callback: null);
                    }
                }
            });
            this.Edit = new DelegateCommand<FormType?>((formType) => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                MineWorkViewModel mineWorkVm;
                if (!MineWorkViewModels.Current.TryGetMineWorkVm(this.Id, out mineWorkVm)) {
                    InputWindow.ShowDialog("作业名称", string.Empty, workName => {
                        if (string.IsNullOrEmpty(workName)) {
                            return "作业名称是必须的";
                        }
                        return string.Empty;
                    }, onOk: workName => {
                        new MineWorkViewModel(this) { Name = workName }.Save.Execute(null);
                    });
                }
                else {
                    // 编辑作业前切换上下文
                    // 根据workId下载json保存到本地并调用LocalJson.Instance.ReInit()
                    string json = Server.ControlCenterService.GetLocalJson(this.Id);
                    if (!string.IsNullOrEmpty(json)) {
                        File.WriteAllText(SpecialPath.LocalJsonFileFullName, json);
                    }
                    else {
                        File.Delete(SpecialPath.LocalJsonFileFullName);
                    }
                    NTMinerRoot.Current.ReInitMinerProfile();
                    this.Sha1 = NTMinerRoot.Current.MinerProfile.GetSha1();
                    MineWorkEdit.ShowWindow(formType ?? FormType.Edit, new MineWorkViewModel(this));
                }
            }, formType => {
                if (this == PleaseSelect) {
                    return false;
                }
                return true;
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                DialogWindow.ShowDialog(message: $"您确定删除吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemoveMineWorkCommand(this.Id));
                }, icon: IconConst.IconConfirm);
            }, () => {
                if (this == PleaseSelect) {
                    return false;
                }
                return true;
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
                    if (string.IsNullOrEmpty(value)) {
                        throw new ValidationException("名称是必须的");
                    }
                    if (MineWorkViewModels.Current.List.Any(a => a.Name == value && a.Id != this.Id)) {
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
