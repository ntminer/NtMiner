using NTMiner.Core;
using NTMiner.Core.MinerClient;
using NTMiner.Core.MinerServer;
using NTMiner.Hub;
using NTMiner.JsonDb;
using NTMiner.MinerStudio.Vms;
using NTMiner.VirtualMemory;
using NTMiner.Vms;
using System;
using System.Collections.Generic;

namespace NTMiner.MinerStudio {
    [MessageType(description: "打开用户列表页")]
    public class ShowUserPageCommand : Cmd {
        public ShowUserPageCommand() {
        }
    }

    [MessageType(description: "打开显卡名称列表页")]
    public class ShowGpuNamePageCommand : Cmd {
        public ShowGpuNamePageCommand() { }
    }

    [MessageType(description: "打开密码修改界面")]
    public class ShowChangePassword : Cmd {
        public ShowChangePassword() { }
    }

    [MessageType(description: "打开外网群控服务器节点列表页")]
    public class ShowWsServerNodePageCommand : Cmd {
        public ShowWsServerNodePageCommand() {
        }
    }

    [MessageType(description: "打开超频菜谱列表页")]
    public class ShowOverClockDataPageCommand : Cmd {
        public ShowOverClockDataPageCommand() {
        }
    }

    [MessageType(description: "打开NTMiner钱包列表页")]
    public class ShowNTMinerWalletPageCommand : Cmd {
        public ShowNTMinerWalletPageCommand() {
        }
    }

    [MessageType(description: "打开群控算力图表窗口")]
    public class ShowChartsWindowCommand : Cmd {
        public ShowChartsWindowCommand() {
        }
    }

    [MessageType(description: "打开列分组页面")]
    public class ShowColumnsShowPageCommand : Cmd {
        public ShowColumnsShowPageCommand() {
        }
    }

    [MessageType(description: "打开升级器设置页面")]
    public class ShowNTMinerUpdaterConfigCommand : Cmd {
        public ShowNTMinerUpdaterConfigCommand() {
        }
    }

    [MessageType(description: "打开挖矿端远程设置界面")]
    public class ShowMinerClientSettingCommand : Cmd {
        public ShowMinerClientSettingCommand(MinerClientSettingViewModel vm) {
            this.Vm = vm;
        }

        public MinerClientSettingViewModel Vm { get; private set; }
    }

    [MessageType(description: "打开群控矿机列表页")]
    public class ShowMinerClientsWindowCommand : Cmd {
        public ShowMinerClientsWindowCommand(bool isToggle) {
            this.IsToggle = isToggle;
        }

        public bool IsToggle { get; private set; }
    }

    [MessageType(description: "打开作群控名设置界面")]
    public class ShowMinerNamesSeterCommand : Cmd {
        public ShowMinerNamesSeterCommand(MinerNamesSeterViewModel vm) {
            this.Vm = vm;
        }

        public MinerNamesSeterViewModel Vm { get; private set; }
    }

    [MessageType(description: "打开群控超频界面")]
    public class ShowGpuProfilesPageCommand : Cmd {
        public ShowGpuProfilesPageCommand(MinerClientsWindowViewModel minerClientsWindowVm) {
            this.MinerClientsWindowVm = minerClientsWindowVm;
        }

        public MinerClientsWindowViewModel MinerClientsWindowVm { get; private set; }
    }

    [MessageType(description: "打开群控矿机的虚拟内存界面")]
    public class ShowMinerStudioVirtualMemoryCommand : Cmd {
        public ShowMinerStudioVirtualMemoryCommand(VirtualMemoryViewModel vm) {
            this.Vm = vm;
        }

        public VirtualMemoryViewModel Vm { get; private set; }
    }

    [MessageType(description: "打开群控矿机的Ip管理界面")]
    public class ShowMinerStudioLocalIpsCommand : Cmd {
        public ShowMinerStudioLocalIpsCommand(Vms.LocalIpConfigViewModel vm) {
            this.Vm = vm;
        }

        public Vms.LocalIpConfigViewModel Vm { get; private set; }
    }

    [MessageType(description: "打开添加矿机界面")]
    public class ShowMinerClientAddCommand : Cmd {
        public ShowMinerClientAddCommand() {
        }
    }

    [MessageType(description: "打开矿工组编辑界面")]
    public class EditMinerGroupCommand : EditCommand<MinerGroupViewModel> {
        public EditMinerGroupCommand(FormType formType, MinerGroupViewModel source) : base(formType, source) {
        }
    }

    [MessageType(description: "打开NTMiner钱包编辑界面")]
    public class EditNTMinerWalletCommand : EditCommand<NTMinerWalletViewModel> {
        public EditNTMinerWalletCommand(FormType formType, NTMinerWalletViewModel source) : base(formType, source) {
        }
    }

    [MessageType(description: "打开作业编辑界面")]
    public class EditMineWorkCommand : EditCommand<MineWorkViewModel> {
        public EditMineWorkCommand(FormType formType, MineWorkViewModel source) : base(formType, source) {
        }
    }

    [MessageType(description: "打开超频菜谱编辑界面")]
    public class EditOverClockDataCommand : EditCommand<OverClockDataViewModel> {
        public EditOverClockDataCommand(FormType formType, OverClockDataViewModel source) : base(formType, source) {
        }
    }

    [MessageType(description: "打开列显编辑界面")]
    public class EditColumnsShowCommand : EditCommand<ColumnsShowViewModel> {
        public EditColumnsShowCommand(ColumnsShowViewModel source) : base(FormType.Edit, source) {
        }
    }

    public abstract class OperationResultEvent<T> : EventBase {
        public OperationResultEvent(Guid clientId, T data) {
            this.ClientId = clientId;
            this.Data = data;
        }

        public Guid ClientId { get; private set; }
        public T Data { get; private set; }
    }

    [MessageType(description: "收到了QueryClientsResponse消息后")]
    public class QueryClientsResponseEvent : EventBase {
        public QueryClientsResponseEvent(QueryClientsResponse response) {
            this.Response = response;
        }

        public QueryClientsResponse Response { get; private set; }
    }

    [MessageType(description: "收到了ClientConsoleOutLines消息后")]
    public class ClientConsoleOutLinesEvent : OperationResultEvent<List<ConsoleOutLine>> {
        public ClientConsoleOutLinesEvent(Guid clientId, List<ConsoleOutLine> data) : base(clientId, data) {
        }
    }

    [MessageType(description: "收到了ClientLocalMessages消息后")]
    public class ClientLocalMessagesEvent : OperationResultEvent<List<LocalMessageDto>> {
        public ClientLocalMessagesEvent(Guid clientId, List<LocalMessageDto> data) : base(clientId, data) {
        }
    }

    [MessageType(description: "收到了ClientOperationResults消息后")]
    public class ClientOperationResultsEvent : OperationResultEvent<List<OperationResultData>> {
        public ClientOperationResultsEvent(Guid clientId, List<OperationResultData> data) : base(clientId, data) {
        }
    }

    [MessageType(description: "收到了GetSelfWorkLocalJsonResponsed消息后")]
    public class GetSelfWorkLocalJsonResponsedEvent : OperationResultEvent<string> {
        public GetSelfWorkLocalJsonResponsedEvent(Guid clientId, string data) : base(clientId, data) {
        }
    }

    [MessageType(description: "收到了GetGpuProfilesResponsed消息后")]
    public class GetGpuProfilesResponsedEvent : OperationResultEvent<GpuProfilesJsonDb> {
        public GetGpuProfilesResponsedEvent(Guid clientId, GpuProfilesJsonDb data) : base(clientId, data) {
        }
    }

    [MessageType(description: "收到了ClientOperationReceived消息后")]
    public class ClientOperationReceivedEvent : EventBase {
        public ClientOperationReceivedEvent(Guid clientId) {
            this.ClientId = clientId;
        }

        public Guid ClientId { get; private set; }
    }

    [MessageType(description: "收到了OperationResult消息后")]
    public class OperationResultEvent : OperationResultEvent<ResponseBase> {
        public OperationResultEvent(Guid clientId, ResponseBase data) : base(clientId, data) {
        }
    }

    [MessageType(description: "更新给定的矿机Vm内存")]
    public class UpdateMinerClientVmCommand : Cmd {
        public UpdateMinerClientVmCommand(ClientData clientData) {
            this.ClientData = clientData;
        }

        public ClientData ClientData { get; private set; }
    }

    [MessageType(description: "矿机列表页选中了和上次选中的不同的矿机时")]
    public class MinerClientSelectionChangedEvent : EventBase {
        public MinerClientSelectionChangedEvent(MinerClientViewModel minerClientVm) {
            this.MinerClientVm = minerClientVm;
        }

        public MinerClientViewModel MinerClientVm { get; private set; }
    }

    [MessageType(description: "收到了GetDrives的响应")]
    public class GetDrivesResponsedEvent : EventBase {
        public GetDrivesResponsedEvent(Guid clientId, List<DriveDto> data) {
            this.ClientId = clientId;
            this.Data = data;
        }

        public Guid ClientId { get; private set; }
        public List<DriveDto> Data { get; private set; }
    }

    [MessageType(description: "收到了GetLocalIps的响应")]
    public class GetLocalIpsResponsedEvent : EventBase {
        public GetLocalIpsResponsedEvent(Guid clientId, List<LocalIpDto> data) {
            this.ClientId = clientId;
            this.Data = data;
        }

        public Guid ClientId { get; private set; }
        public List<LocalIpDto> Data { get; private set; }
    }

    [MessageType(description: "添加了显卡特征名后")]
    public class GpuNameAddedEvent : EventBase {
        public GpuNameAddedEvent() { }
    }

    [MessageType(description: "添加显卡特征名")]
    public class AddGpuNameCommand : Cmd {
        public AddGpuNameCommand(GpuNameViewModel gpuNameVm) {
            this.GpuNameVm = gpuNameVm;
        }

        public GpuNameViewModel GpuNameVm { get; private set; }
    }
}
