using NTMiner.Core;
using NTMiner.JsonDb;
using NTMiner.MinerServer;
using System;
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
        private string _serverJsonSha1;

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
            _serverJsonSha1 = mineWork.ServerJsonSha1;
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
                    NotiCenterWindowViewModel.Instance.Manager.ShowErrorMessage("作业名称是必须的");
                }
                bool isMineWorkChanged = false;
                bool isMinerProfileChanged = false;
                MineWorkData mineWorkData = new MineWorkData(this);
                if (NTMinerRoot.Instance.MineWorkSet.TryGetMineWork(this.Id, out IMineWork entity)) {
                    string sha1 = NTMinerRoot.Instance.MinerProfile.GetSha1();
                    // 如果作业设置变更了则一定变更了
                    if (this.Sha1 != sha1) {
                        isMinerProfileChanged = true;
                    }
                    else {
                        // 如果作业设置没变更但作业引用的服务器数据库记录状态变更了则变更了
                        LocalJsonDb localJsonObj = new LocalJsonDb(NTMinerRoot.Instance, mineWorkData);
                        ServerJsonDb serverJsonObj = new ServerJsonDb(NTMinerRoot.Instance, localJsonObj);
                        var serverJson = VirtualRoot.JsonSerializer.Serialize(serverJsonObj);
                        sha1 = HashUtil.Sha1(serverJson);
                        if (sha1 != this.ServerJsonSha1) {
                            isMinerProfileChanged = true;
                        }
                    }
                    if (entity.Name != this.Name || entity.Description != this.Description) {
                        isMineWorkChanged = true;
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
                    Write.DevDebug("检测到MinerProfile状态变更");
                    NTMinerRoot.ExportWorkJson(mineWorkData, out string localJson, out string serverJson);
                    if (!string.IsNullOrEmpty(localJson) && !string.IsNullOrEmpty(serverJson)) {
                        Server.ControlCenterService.ExportMineWorkAsync(this.Id, localJson, serverJson, callback: null);
                    }
                    if (mineWorkData.ServerJsonSha1 != this.ServerJsonSha1) {
                        this.ServerJsonSha1 = mineWorkData.ServerJsonSha1;
                        isMineWorkChanged = true;
                    }
                }
                if (isMineWorkChanged) {
                    VirtualRoot.Execute(new UpdateMineWorkCommand(mineWorkData));
                }
            });
            this.Edit = new DelegateCommand<FormType?>((formType) => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                if (!AppContext.Instance.MineWorkVms.TryGetMineWorkVm(this.Id, out MineWorkViewModel mineWorkVm)) {
                    Wpf.Util.ShowInputDialog("作业名称", string.Empty, workName => {
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
                    NTMinerRoot.Instance.ReInitMinerProfile();
                    this.Sha1 = NTMinerRoot.Instance.MinerProfile.GetSha1();
                    VirtualRoot.Execute(new MineWorkEditCommand(formType ?? FormType.Edit, new MineWorkViewModel(this)));
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
                this.ShowDialog(message: $"您确定删除吗？", title: "确认", onYes: () => {
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
                    if (AppContext.Instance.MineWorkVms.List.Any(a => a.Name == value && a.Id != this.Id)) {
                        throw new ValidationException("名称重复");
                    }
                }
            }
        }

        public MinerProfileViewModel MinerProfile {
            get {
                return AppContext.Instance.MinerProfileVm;
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

        public string ServerJsonSha1 {
            get => _serverJsonSha1;
            set {
                if (_serverJsonSha1 != value) {
                    _serverJsonSha1 = value;
                    OnPropertyChanged(nameof(ServerJsonSha1));
                }
            }
        }
    }
}
