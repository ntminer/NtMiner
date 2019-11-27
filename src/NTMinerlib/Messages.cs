using NTMiner.Bus;
using NTMiner.Core;
using NTMiner.MinerClient;
using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

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

    [MessageType(description: "显式主界面")]
    public class ShowMainWindowCommand : Cmd {
        public ShowMainWindowCommand(bool isToggle) {
            this.IsToggle = isToggle;
        }

        public bool IsToggle { get; private set; }
    }

    [MessageType(description: "设置ServerAppSetting")]
    public class SetServerAppSettingCommand : Cmd {
        public SetServerAppSettingCommand(IAppSetting appSetting) {
            this.AppSetting = appSetting;
        }

        public IAppSetting AppSetting {
            get; private set;
        }
    }

    [MessageType(description: "设置ServerAppSetting")]
    public class SetServerAppSettingsCommand : Cmd {
        public SetServerAppSettingsCommand(IEnumerable<IAppSetting> appSettings) {
            this.AppSettings = appSettings;
        }

        public IEnumerable<IAppSetting> AppSettings {
            get; private set;
        }
    }

    [MessageType(description: "ServerAppSetting变更后")]
    public class ServerAppSettingSetedEvent : DomainEvent<IAppSetting> {
        public ServerAppSettingSetedEvent(Guid bornPathId, IAppSetting source) : base(bornPathId, source) {
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

    [MessageType(description: "设置LocalAppSetting")]
    public class SetLocalAppSettingsCommand : Cmd {
        public SetLocalAppSettingsCommand(IEnumerable<IAppSetting> appSettings) {
            this.AppSettings = appSettings;
        }

        public IEnumerable<IAppSetting> AppSettings {
            get; private set;
        }
    }

    [MessageType(description: "本机IP集初始化后")]
    public class LocalIpSetInitedEvent : EventBase {
        public LocalIpSetInitedEvent() { }
    }

    [MessageType(description: "LocalAppSetting变更后")]
    public class LocalAppSettingChangedEvent : DomainEvent<IAppSetting> {
        public LocalAppSettingChangedEvent(Guid bornPathId, IAppSetting source) : base(bornPathId, source) {
        }
    }

    [MessageType(description: "发生了用户活动后")]
    public class UserActionEvent : EventBase {
        public UserActionEvent() {
        }
    }

    [MessageType(description: "设置本机Ip")]
    public class SetLocalIpCommand : Cmd {
        public SetLocalIpCommand(ILocalIp input, bool isAutoDNSServer) {
            this.Input = input;
            this.IsAutoDNSServer = isAutoDNSServer;
        }

        public ILocalIp Input { get; private set; }
        public bool IsAutoDNSServer { get; private set; }
    }

    #region KernelOutputKeyword Messages
    [MessageType(description: "添加或修改内核输出关键字")]
    public class AddOrUpdateKernelOutputKeywordCommand : Cmd {
        public AddOrUpdateKernelOutputKeywordCommand(IKernelOutputKeyword input) {
            this.Input = input;
        }

        public IKernelOutputKeyword Input { get; private set; }
    }

    [MessageType(description: "添加了用户自定义内核输出关键字后")]
    public class UserKernelOutputKeywordAddedEvent : DomainEvent<IKernelOutputKeyword> {
        public UserKernelOutputKeywordAddedEvent(Guid bornPathId, IKernelOutputKeyword source) : base(bornPathId, source) {
        }
    }

    [MessageType(description: "更新了用户自定义内核输出关键字后")]
    public class UserKernelOutputKeywordUpdatedEvent : DomainEvent<IKernelOutputKeyword> {
        public UserKernelOutputKeywordUpdatedEvent(Guid bornPathId, IKernelOutputKeyword source) : base(bornPathId, source) {
        }
    }

    [MessageType(description: "移除内核输出关键字")]
    public class RemoveKernelOutputKeywordCommand : RemoveEntityCommand {
        public RemoveKernelOutputKeywordCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(description: "移除了用户自定义内核输出关键字后")]
    public class UserKernelOutputKeywordRemovedEvent : DomainEvent<IKernelOutputKeyword> {
        public UserKernelOutputKeywordRemovedEvent(Guid bornPathId, IKernelOutputKeyword source) : base(bornPathId, source) {

        }
    }
    #endregion

    #region LocalMessage
    [MessageType(description: "添加本地消息")]
    public class AddLocalMessageCommand : AddEntityCommand<ILocalMessage> {
        public AddLocalMessageCommand(ILocalMessage input) : base(input) { }
    }

    [MessageType(description: "记录了本地事件后")]
    public class LocalMessageAddedEvent : DomainEvent<ILocalMessage> {
        public LocalMessageAddedEvent(Guid bornPathId, ILocalMessage source, List<ILocalMessage> removes) : base(bornPathId, source) {
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
    public class ClearServerMessages : Cmd {
        public ClearServerMessages() { }
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

        public LoadNewServerMessageCommand(ulong knowServerMessageTimestamp) {
            this.KnowServerMessageTimestamp = knowServerMessageTimestamp;
        }

        public ulong KnowServerMessageTimestamp { get; private set; }
    }

    [MessageType(description: "从服务器获取内核输出关键字")]
    public class LoadKernelOutputKeywordCommand : Cmd {
        public LoadKernelOutputKeywordCommand() {
            this.KnowKernelOutputKeywordTimestamp = Timestamp.GetTimestamp();
        }
        public LoadKernelOutputKeywordCommand(ulong knowKernelOutputKeywordTimestamp) {
            this.KnowKernelOutputKeywordTimestamp = knowKernelOutputKeywordTimestamp;
        }

        public ulong KnowKernelOutputKeywordTimestamp { get; private set; }
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
}
