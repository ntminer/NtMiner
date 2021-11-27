using NTMiner.Core;
using NTMiner.Core.MinerServer;
using NTMiner.Hub;
using NTMiner.User;
using System;
using System.Collections.Generic;
using System.Net;

namespace NTMiner {
    #region abstract
    public abstract class AddEntityCommand<TEntity> : Cmd where TEntity : class, IEntity<Guid> {
        protected AddEntityCommand(TEntity input) {
            this.Input = input ?? throw new ArgumentNullException(nameof(input));
        }

        public TEntity Input { get; private set; }
    }

    public abstract class RemoveEntityCommand : Cmd {
        protected RemoveEntityCommand(Guid entityId) {
            this.EntityId = entityId;
        }

        public Guid EntityId { get; private set; }
    }

    public abstract class UpdateEntityCommand<TEntity> : Cmd where TEntity : class, IEntity<Guid> {
        protected UpdateEntityCommand(TEntity input) {
            this.Input = input ?? throw new ArgumentNullException(nameof(input));
        }

        public TEntity Input { get; private set; }
    }
    #endregion

    public abstract class VmEventBase<T> : EventBase {
        public VmEventBase(T evt) {
            this.Event = evt;
        }

        public T Event { get; private set; }
    }

    [MessageType(description: "WebSocket服务的TcpListener Accept了一个客户端")]
    public class WsTcpClientAcceptedEvent : EventBase {
        public WsTcpClientAcceptedEvent(IPAddress remoteIp) {
            this.RemoteIp = remoteIp;
        }

        public IPAddress RemoteIp { get; private set; }
    }

    [MessageType(description: "WebApi服务收到请求时")]
    public class WebApiRequestEvent : EventBase {
        public WebApiRequestEvent(IPAddress remoteIp) {
            this.RemoteIp = remoteIp;
        }

        public IPAddress RemoteIp { get; private set; }
    }

    [MessageType(description: "注册了外网群控用户后")]
    public class SignUpedEvent : EventBase {
        public SignUpedEvent(string loginName) {
            this.LoginName = loginName;
        }

        public string LoginName { get; private set; }
    }

    [MessageType(description: "打开用户注册页")]
    public class ShowSignUpPageCommand : Cmd {
        public ShowSignUpPageCommand() { }
    }

    [MessageType(description: "升级")]
    public class UpgradeCommand : Cmd {
        public UpgradeCommand(string fileName, Action callback) {
            this.FileName = fileName;
            this.Callback = callback;
        }

        public string FileName { get; private set; }
        public Action Callback { get; private set; }
    }

    [MessageType(description: "关闭窗口")]
    public class CloseWindowCommand : Cmd {
        public CloseWindowCommand(Guid id) : base(id) { }
    }

    [MessageType(description: "打开本机IP管理页")]
    public class ShowLocalIpsCommand : Cmd {
        public ShowLocalIpsCommand() { }
    }

    [MessageType(description: "显式主界面")]
    public class ShowMainWindowCommand : Cmd {
        public ShowMainWindowCommand(bool isToggle) {
            this.IsToggle = isToggle;
        }

        public bool IsToggle { get; private set; }
    }

    [MessageType(description: "设置UserAppSetting")]
    public class SetUserAppSettingCommand : Cmd {
        public SetUserAppSettingCommand(IUserAppSetting appSetting) {
            this.AppSetting = appSetting;
        }

        public IUserAppSetting AppSetting {
            get; private set;
        }
    }


    [MessageType(description: "设置LocalAppSetting")]
    public class SetLocalAppSettingCommand : Cmd {
        public SetLocalAppSettingCommand(IAppSetting appSetting) {
            this.AppSetting = appSetting;
        }

        public IAppSetting AppSetting {
            get; private set;
        }
    }

    [MessageType(description: "LocalAppSetting变更后")]
    public class LocalAppSettingChangedEvent : SourcedEvent<IAppSetting> {
        public LocalAppSettingChangedEvent(PathId targetPathId, IAppSetting source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "发生了用户活动后")]
    public class UserActionEvent : EventBase {
        public UserActionEvent() {
        }
    }

    #region KernelOutputKeyword Messages
    [MessageType(description: "添加或修改内核输出关键字")]
    public class AddOrUpdateKernelOutputKeywordCommand : Cmd {
        public AddOrUpdateKernelOutputKeywordCommand(IKernelOutputKeyword input) {
            this.Input = input;
        }

        public IKernelOutputKeyword Input { get; private set; }
    }

    [MessageType(description: "清空给定输出标识之外的记录")]
    public class ClearKernelOutputKeywordsCommand : Cmd {
        public ClearKernelOutputKeywordsCommand(Guid[] exceptedOutputIds) {
            this.ExceptedOutputIds = exceptedOutputIds;
        }

        public Guid[] ExceptedOutputIds { get; private set; }
    }

    [MessageType(description: "添加了用户自定义内核输出关键字后")]
    public class UserKernelOutputKeywordAddedEvent : SourcedEvent<IKernelOutputKeyword> {
        public UserKernelOutputKeywordAddedEvent(PathId targetPathId, IKernelOutputKeyword source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "更新了用户自定义内核输出关键字后")]
    public class UserKernelOutputKeywordUpdatedEvent : SourcedEvent<IKernelOutputKeyword> {
        public UserKernelOutputKeywordUpdatedEvent(PathId targetPathId, IKernelOutputKeyword source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "移除内核输出关键字")]
    public class RemoveKernelOutputKeywordCommand : RemoveEntityCommand {
        public RemoveKernelOutputKeywordCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(description: "移除了用户自定义内核输出关键字后")]
    public class UserKernelOutputKeywordRemovedEvent : SourcedEvent<IKernelOutputKeyword> {
        public UserKernelOutputKeywordRemovedEvent(PathId targetPathId, IKernelOutputKeyword source) : base(targetPathId, source) {

        }
    }
    #endregion

    #region LocalMessage
    [MessageType(description: "记录了本地事件后")]
    public class LocalMessageAddedEvent : SourcedEvent<ILocalMessage> {
        public LocalMessageAddedEvent(PathId targetPathId, ILocalMessage source, List<ILocalMessage> removes) : base(targetPathId, source) {
            this.Removes = removes ?? new List<ILocalMessage>();
        }

        public List<ILocalMessage> Removes { get; private set; }
    }

    [MessageType(description: "清空本地消息集")]
    public class ClearLocalMessageSetCommand : Cmd {
        public ClearLocalMessageSetCommand() { }
    }

    [MessageType(description: "本地消息集清空后")]
    public class LocalMessageSetClearedEvent : EventBase {
        public LocalMessageSetClearedEvent() { }
    }
    #endregion

    #region ServerMessage
    [MessageType(description: "清空服务器消息集")]
    public class ClearServerMessagesCommand : Cmd {
        public ClearServerMessagesCommand() { }
    }

    [MessageType(description: "服务器消息集清空后")]
    public class ServerMessagesClearedEvent : EventBase {
        public ServerMessagesClearedEvent() { }
    }

    [MessageType(description: "接收从服务器得到的服务器消息")]
    public class ReceiveServerMessageCommand : Cmd {
        public ReceiveServerMessageCommand(List<ServerMessageData> data) {
            this.Data = data;
        }

        public List<ServerMessageData> Data { get; private set; }
    }

    [MessageType(description: "从服务器获取新的服务器消息")]
    public class LoadNewServerMessageCommand : Cmd {
        public LoadNewServerMessageCommand() {
            this.KnowServerMessageTimestamp = Timestamp.GetTimestamp();
        }

        public LoadNewServerMessageCommand(long knowServerMessageTimestamp) {
            this.KnowServerMessageTimestamp = knowServerMessageTimestamp;
        }

        public long KnowServerMessageTimestamp { get; private set; }
    }

    [MessageType(description: "从服务器获取内核输出关键字")]
    public class LoadKernelOutputKeywordCommand : Cmd {
        public LoadKernelOutputKeywordCommand() {
            this.KnowKernelOutputKeywordTimestamp = Timestamp.GetTimestamp();
        }
        public LoadKernelOutputKeywordCommand(long knowKernelOutputKeywordTimestamp) {
            this.KnowKernelOutputKeywordTimestamp = knowKernelOutputKeywordTimestamp;
        }

        public long KnowKernelOutputKeywordTimestamp { get; private set; }
    }

    [MessageType(description: "服务器Ws服务可用了")]
    public class WsServerOkEvent : EventBase {
        public WsServerOkEvent() { }
    }

    [MessageType(description: "从服务器获取到新的服务器消息后")]
    public class NewServerMessageLoadedEvent : EventBase {
        public NewServerMessageLoadedEvent(LinkedList<ServerMessageData> data) {
            this.Data = data;
        }

        public LinkedList<ServerMessageData> Data { get; private set; }
    }

    [MessageType(description: "从服务器获取了内核输出关键字后")]
    public class KernelOutputKeywordLoadedEvent : EventBase {
        public KernelOutputKeywordLoadedEvent(List<KernelOutputKeywordData> data) {
            this.Data = data;
        }

        public List<KernelOutputKeywordData> Data { get; private set; }
    }

    [MessageType(description: "添加或修改服务器消息")]
    public class AddOrUpdateServerMessageCommand : AddEntityCommand<IServerMessage> {
        public AddOrUpdateServerMessageCommand(IServerMessage input) : base(input) {
        }
    }

    [MessageType(description: "标记删除服务器消息")]
    public class MarkDeleteServerMessageCommand : RemoveEntityCommand {
        public MarkDeleteServerMessageCommand(Guid id) : base(id) {

        }
    }
    #endregion

    [MessageType(description: "矿工集已初始化完成")]
    public class ClientSetInitedEvent : EventBase {
        public ClientSetInitedEvent() { }
    }

    [MessageType(description: "用户集已初始化完成")]
    public class UserSetInitedEvent : EventBase {
        public UserSetInitedEvent() { }
    }

    [MessageType(description: "矿机签名集已初始化完成")]
    public class MinerSignSetInitedEvent: EventBase {
        public MinerSignSetInitedEvent() { }
    }

    [MessageType(description: "Ws服务器节点地址集已初始化完成")]
    public class WsServerNodeAddressSetInitedEvent : EventBase {
        public WsServerNodeAddressSetInitedEvent() { }
    }

    [MessageType(description: "收益计算器配置集已初始化完成")]
    public class CalcConfigSetInitedEvent : EventBase {
        public CalcConfigSetInitedEvent() { }
    }
}
