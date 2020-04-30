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
            _name = "单机作业"
        };

        private Guid _id;
        private string _name;
        private string _description;
        private string _serverJsonSha1;
        private MinerClientViewModel _minerClientVm;

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
            _minerClientVm = vm._minerClientVm;
        }

        public MineWorkViewModel(Guid id) {
            _id = id;
            // 作业编辑窗口关闭时自动调用的
            this.Save = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                DoSave();
            });
            this.Edit = new DelegateCommand<FormType?>((formType) => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                DoEdit(formType);
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

        private void DoEdit(FormType? formType) {
            if (!MinerStudioRoot.MineWorkVms.TryGetMineWorkVm(this.Id, out MineWorkViewModel mineWorkVm) && this.Id != MineWorkData.SelfMineWorkId) {
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
                _minerClientVm = MinerStudioRoot.MinerClientsWindowVm.SelectedMinerClients.FirstOrDefault();
                if (_minerClientVm == null) {
                    VirtualRoot.Out.ShowError("未选中矿机", autoHideSeconds: 4);
                    return;
                }
                if (this.Id == MineWorkData.SelfMineWorkId) {
                    SelfMineWork.Description = $"{_minerClientVm.GetMinerOrClientName()} 矿机的单机作业";
                    if (RpcRoot.IsOuterNet) {
                        if (!_minerClientVm.IsOuterUserEnabled) {
                            VirtualRoot.Out.ShowError("无法操作，因为选中的矿机未开启外网群控。", autoHideSeconds: 6);
                            return;
                        }
                        VirtualRoot.AddOnecePath<GetSelfWorkLocalJsonResponsedEvent>("获取到响应结果后填充Vm内存", LogEnum.DevConsole, action: message => {
                            if (message.ClientId == _minerClientVm.ClientId) {
                                string data = message.Data;
                                EditJson(formType, WorkType.SelfWork, data);
                            }
                        }, PathId.Empty, typeof(MineWorkViewModel));
                        MinerStudioService.Instance.GetSelfWorkLocalJsonAsync(_minerClientVm);
                    }
                    else {
                        RpcRoot.Client.NTMinerDaemonService.GetSelfWorkLocalJsonAsync(_minerClientVm, (json, e) => {
                            string data = json;
                            EditJson(formType, WorkType.SelfWork, data);
                        });
                    }
                }
                else {
                    // 编辑作业前切换上下文
                    // 根据workId下载json保存到本地并调用LocalJson.Instance.ReInit()
                    if (RpcRoot.IsOuterNet) {
                        RpcRoot.OfficialServer.UserMineWorkService.GetLocalJsonAsync(this.Id, (response, e) => {
                            if (response.IsSuccess()) {
                                string data = response.Data;
                                EditJson(formType, WorkType.MineWork, data);
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
                            EditJson(formType, WorkType.MineWork, data);
                        }
                        catch (Exception e) {
                            Logger.ErrorDebugLine(e);
                        }
                    }
                }
            }
        }

        private void EditJson(FormType? formType, WorkType workType, string json) {
            if (workType == WorkType.SelfWork) {
                if (!string.IsNullOrEmpty(json)) {
                    File.WriteAllText(HomePath.SelfWorkLocalJsonFileFullName, json);
                }
                else {
                    File.Delete(HomePath.SelfWorkLocalJsonFileFullName);
                }
            }
            else {
                if (!string.IsNullOrEmpty(json)) {
                    File.WriteAllText(HomePath.MineWorkLocalJsonFileFullName, json);
                }
                else {
                    File.Delete(HomePath.MineWorkLocalJsonFileFullName);
                }
            }
            NTMinerContext.Instance.ReInitMinerProfile(workType);
            this.Sha1 = NTMinerContext.Instance.MinerProfile.GetSha1();
            MineWorkData mineWorkData = new MineWorkData().Update(this);
            LocalJsonDb localJsonObj = new LocalJsonDb(NTMinerContext.Instance, mineWorkData);
            ServerJsonDb serverJsonObj = new ServerJsonDb(NTMinerContext.Instance, localJsonObj);
            var serverJson = VirtualRoot.JsonSerializer.Serialize(serverJsonObj);
            this.ServerJsonSha1 = HashUtil.Sha1(serverJson);
            VirtualRoot.Execute(new EditMineWorkCommand(formType ?? FormType.Edit, new MineWorkViewModel(this)));
        }

        private void DoSave() {
            if (string.IsNullOrEmpty(this.Name)) {
                VirtualRoot.Out.ShowError("作业名称是必须的", autoHideSeconds: 4);
            }
            bool isMinerProfileChanged = false;
            // 表示是否随后打开编辑界面，如果是新建的作业则保存后随即会打开作业编辑界面
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
                Write.UserInfo("保存作业。");
            }
            if (RpcRoot.IsOuterNet) {
                if (this.Id != MineWorkData.SelfMineWorkId) {
                    RpcRoot.OfficialServer.UserMineWorkService.AddOrUpdateMineWorkAsync(new MineWorkData().Update(this), (r, ex) => {
                        if (r.IsSuccess()) {
                            if (isMinerProfileChanged) {
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
                        NTMinerContext.ExportWorkJson(mineWorkData, out string localJson, out string serverJson);
                        if (!string.IsNullOrEmpty(localJson) && !string.IsNullOrEmpty(serverJson)) {
                            MinerStudioService.Instance.SaveSelfWorkLocalJsonAsync(_minerClientVm, localJson, serverJson);
                        }
                        if (mineWorkData.ServerJsonSha1 != this.ServerJsonSha1) {
                            this.ServerJsonSha1 = mineWorkData.ServerJsonSha1;
                        }
                    }
                }
            }
            else {
                if (this.Id != MineWorkData.SelfMineWorkId) {
                    if (isMinerProfileChanged) {
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
                else {
                    if (isMinerProfileChanged) {
                        NTMinerContext.ExportWorkJson(mineWorkData, out string localJson, out string serverJson);
                        if (!string.IsNullOrEmpty(localJson) && !string.IsNullOrEmpty(serverJson)) {
                            MinerStudioService.Instance.SaveSelfWorkLocalJsonAsync(_minerClientVm, localJson, serverJson);
                        }
                        if (mineWorkData.ServerJsonSha1 != this.ServerJsonSha1) {
                            this.ServerJsonSha1 = mineWorkData.ServerJsonSha1;
                        }
                    }
                }
            }
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
                return this.Id == PleaseSelect.Id;
            }
        }

        public bool IsSelfMineWork {
            get {
                return this.Id == SelfMineWork.Id;
            }
        }

        public bool IsSystem {
            get {
                return this.Id == PleaseSelect.Id || this.Id == SelfMineWork.Id;
            }
        }

        public string Name {
            get { return _name; }
            set {
                if (_name != value) {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                    if (this.Id == PleaseSelect.Id) {
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
