using NTMiner.Core;
using NTMiner.Core.MinerStudio;
using NTMiner.JsonDb;
using NTMiner.Vms;
using System;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.MinerStudio.Vms {
    public class MineWorkViewModel : ViewModelBase, IMineWork, IEditableViewModel {
        public static readonly MineWorkViewModel PleaseSelect = new MineWorkViewModel(Guid.Empty) {
            _name = "不指定"
        };
        public static readonly MineWorkViewModel SelfMineWork = new MineWorkViewModel(MineWorkData.SelfMineWorkId) {
            _name = "自主作业"
        };

        private Guid _id;
        private string _name;
        private string _description;
        private string _serverJsonSha1;

        public string Sha1 { get; private set; }

        public ICommand Edit { get; private set; }
        public ICommand Remove { get; private set; }
        public ICommand Save { get; private set; }

        [Obsolete(message: NTKeyword.WpfDesignOnly, error: true)]
        public MineWorkViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException(NTKeyword.WpfDesignOnly);
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
                    VirtualRoot.Out.ShowError("作业名称是必须的", autoHideSeconds: 4);
                }
                bool isMinerProfileChanged = false;
                bool isShowEdit = false;
                MineWorkData mineWorkData = new MineWorkData().Update(this); 
                if (MinerStudioRoot.MineWorkVms.TryGetMineWorkVm(this.Id, out MineWorkViewModel vm)) {
                    string sha1 = NTMinerContext.Instance.MinerProfile.GetSha1();
                    // 如果作业设置变更了则一定变更了
                    if (this.Sha1 != sha1) {
                        isMinerProfileChanged = true;
                    }
                    else {
                        // 如果作业设置没变更但作业引用的服务器数据库记录状态变更了则变更了
                        LocalJsonDb localJsonObj = new LocalJsonDb(NTMinerContext.Instance, mineWorkData);
                        ServerJsonDb serverJsonObj = new ServerJsonDb(NTMinerContext.Instance, localJsonObj);
                        var serverJson = VirtualRoot.JsonSerializer.Serialize(serverJsonObj);
                        sha1 = HashUtil.Sha1(serverJson);
                        if (sha1 != this.ServerJsonSha1) {
                            isMinerProfileChanged = true;
                        }
                    }
                }
                else {
                    isMinerProfileChanged = true;
                    isShowEdit = true;
                }
                if (RpcRoot.IsOuterNet) {
                    RpcRoot.OfficialServer.UserMineWorkService.AddOrUpdateMineWorkAsync(new MineWorkData().Update(this), (r, ex) => {
                        if (r.IsSuccess()) {
                            if (isMinerProfileChanged) {
                                Write.DevDebug("检测到MinerProfile状态变更");
                                NTMinerContext.ExportWorkJson(mineWorkData, out string localJson, out string serverJson);
                                if (!string.IsNullOrEmpty(localJson) && !string.IsNullOrEmpty(serverJson)) {
                                    RpcRoot.OfficialServer.UserMineWorkService.ExportMineWorkAsync(this.Id, localJson, serverJson, (response, e) => {
                                        if (response.IsSuccess()) {
                                            if (isShowEdit) {
                                                VirtualRoot.RaiseEvent(new MineWorkAddedEvent(Guid.Empty, this));
                                            }
                                            else {
                                                VirtualRoot.RaiseEvent(new MineWorkUpdatedEvent(Guid.Empty, this));
                                            }
                                            if (isShowEdit) {
                                                this.Edit.Execute(FormType.Edit);
                                            }
                                        }
                                        else {
                                            VirtualRoot.Out.ShowError(response.ReadMessage(e), autoHideSeconds: 4);
                                        }
                                    });
                                }
                                if (mineWorkData.ServerJsonSha1 != this.ServerJsonSha1) {
                                    this.ServerJsonSha1 = mineWorkData.ServerJsonSha1;
                                }
                            }
                        }
                        else {
                            VirtualRoot.Out.ShowError(r.ReadMessage(ex), autoHideSeconds: 4);
                        }
                    });
                }
                else {
                    if (isMinerProfileChanged) {
                        Write.DevDebug("检测到MinerProfile状态变更");
                        NTMinerContext.ExportWorkJson(mineWorkData, out string localJson, out string serverJson);
                        if (!string.IsNullOrEmpty(localJson) && !string.IsNullOrEmpty(serverJson)) {
                            try {
                                string localJsonFileFullName = MinerStudioPath.GetMineWorkLocalJsonFileFullName(this.Id);
                                string serverJsonFileFullName = MinerStudioPath.GetMineWorkServerJsonFileFullName(this.Id);
                                File.WriteAllText(localJsonFileFullName, localJson);
                                File.WriteAllText(serverJsonFileFullName, serverJson);
                            }
                            catch (Exception e) {
                                VirtualRoot.Out.ShowError(e.Message, autoHideSeconds: 4);
                                Logger.ErrorDebugLine(e);
                            }
                        }
                        if (mineWorkData.ServerJsonSha1 != this.ServerJsonSha1) {
                            this.ServerJsonSha1 = mineWorkData.ServerJsonSha1;
                        }
                    }
                    if (NTMinerContext.Instance.MinerStudioContext.MineWorkSet.Contains(mineWorkData.Id)) {
                        VirtualRoot.Execute(new UpdateMineWorkCommand(mineWorkData));
                    }
                    else {
                        VirtualRoot.Execute(new AddMineWorkCommand(mineWorkData));
                    }
                    if (isShowEdit) {
                        this.Edit.Execute(FormType.Edit);
                    }
                }
            });
            this.Edit = new DelegateCommand<FormType?>((formType) => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                if (!MinerStudioRoot.MineWorkVms.TryGetMineWorkVm(this.Id, out MineWorkViewModel mineWorkVm)) {
                    WpfUtil.ShowInputDialog("作业名称", string.Empty, string.Empty, workName => {
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
                    if (RpcRoot.IsOuterNet) {
                        RpcRoot.OfficialServer.UserMineWorkService.GetLocalJsonAsync(this.Id, (response, e) => {
                            if (response.IsSuccess()) {
                                string data = response.Data;
                                if (!string.IsNullOrEmpty(data)) {
                                    File.WriteAllText(HomePath.LocalJsonFileFullName, data);
                                }
                                NTMinerContext.Instance.ReInitMinerProfile();
                                this.Sha1 = NTMinerContext.Instance.MinerProfile.GetSha1();
                                VirtualRoot.Execute(new EditMineWorkCommand(formType ?? FormType.Edit, new MineWorkViewModel(this)));
                            }
                        });
                    }
                    else {
                        try {
                            string localJsonFileFullName = MinerStudioPath.GetMineWorkLocalJsonFileFullName(this.Id);
                            string data = string.Empty;
                            if (File.Exists(localJsonFileFullName)) {
                                data = File.ReadAllText(localJsonFileFullName);
                            }
                            if (!string.IsNullOrEmpty(data)) {
                                File.WriteAllText(HomePath.LocalJsonFileFullName, data);
                            }
                            else {
                                File.Delete(HomePath.LocalJsonFileFullName);
                            }
                            NTMinerContext.Instance.ReInitMinerProfile();
                            this.Sha1 = NTMinerContext.Instance.MinerProfile.GetSha1();
                            VirtualRoot.Execute(new EditMineWorkCommand(formType ?? FormType.Edit, new MineWorkViewModel(this)));
                        }
                        catch (Exception e) {
                            Logger.ErrorDebugLine(e);
                        }
                    }
                }
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                this.ShowSoftDialog(new DialogWindowViewModel(message: $"您确定删除 “{this.Name}” 作业吗？", title: "确认", onYes: () => {
                    if (RpcRoot.IsOuterNet) {
                        RpcRoot.OfficialServer.UserMineWorkService.RemoveMineWorkAsync(this.Id, (response, e) => {
                            if (response.IsSuccess()) {
                                VirtualRoot.RaiseEvent(new MineWorkRemovedEvent(PathId.Empty, this));
                            }
                        });
                    }
                    else {
                        VirtualRoot.Execute(new RemoveMineWorkCommand(this.Id));
                    }
                }));
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

        public bool IsPleaseSelect {
            get {
                return this == PleaseSelect;
            }
        }

        public bool IsCanDelete {
            get {
                return this != PleaseSelect && this != SelfMineWork;
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
                    if (MinerStudioRoot.MineWorkVms.List.Any(a => a.Name == value && a.Id != this.Id)) {
                        throw new ValidationException("名称重复");
                    }
                }
            }
        }

        public MinerProfileViewModel MinerProfile {
            get {
                return AppRoot.MinerProfileVm;
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
