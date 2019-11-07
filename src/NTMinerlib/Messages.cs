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
        public ServerAppSettingSetedEvent(IAppSetting source) : base(source) {
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

    [MessageType(description: "本机IP集刷新后")]
    public class LocalIpSetRefreshedEvent : EventBase {
        public LocalIpSetRefreshedEvent() { }
    }

    [MessageType(description: "LocalAppSetting变更后")]
    public class LocalAppSettingChangedEvent : DomainEvent<IAppSetting> {
        public LocalAppSettingChangedEvent(IAppSetting source) : base(source) {
        }
    }

    [MessageType(description: "发生了用户活动后")]
    public class UserActionEvent : EventBase {
        public UserActionEvent() {
        }
    }

    #region KernelOutputKeyword Messages
    [MessageType(description: "添加或修改内核输出关键字")]
    public class SetKernelOutputKeywordCommand : Cmd {
        public SetKernelOutputKeywordCommand(IKernelOutputKeyword input) {
            this.Input = input;
        }

        public IKernelOutputKeyword Input { get; private set; }
    }

    [MessageType(description: "移除内核输出关键字")]
    public class RemoveKernelOutputKeywordCommand : RemoveEntityCommand {
        public RemoveKernelOutputKeywordCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(description: "添加或修改内核输出关键字后")]
    public class KernelOutputKeyworSetedEvent : DomainEvent<IKernelOutputKeyword> {
        public KernelOutputKeyworSetedEvent(IKernelOutputKeyword source) : base(source) {
        }
    }

    [MessageType(description: "移除了内核输出关键字后")]
    public class KernelOutputKeywordRemovedEvent : DomainEvent<IKernelOutputKeyword> {
        public KernelOutputKeywordRemovedEvent(IKernelOutputKeyword source) : base(source) {
        }
    }
    #endregion

    [MessageType(description: "记录了本地事件后")]
    public class LocalMessageAddedEvent : DomainEvent<ILocalMessage> {
        public LocalMessageAddedEvent(ILocalMessage source, List<ILocalMessage> removes) : base(source) {
            this.Removes = removes ?? new List<ILocalMessage>();
        }

        public List<ILocalMessage> Removes { get; private set; }
    }

    [MessageType(description: "本地消息集清空后")]
    public class LocalMessageClearedEvent : EventBase {
        public LocalMessageClearedEvent() { }
    }

    [MessageType(description: "记录了服务器事件后")]
    public class ServerMessageAddedEvent : DomainEvent<IServerMessage> {
        public ServerMessageAddedEvent(IServerMessage source, List<IServerMessage> removes) : base(source) {
            this.Removes = removes ?? new List<IServerMessage>();
        }

        public List<IServerMessage> Removes { get; private set; }
    }

    [MessageType(description: "服务器消息集清空后")]
    public class ServerMessageClearedEvent : EventBase {
        public ServerMessageClearedEvent() { }
    }

    #region ServerMessage
    [MessageType(description: "从服务器获取新的服务器消息")]
    public class LoadNewServerMessageCommand : Cmd {
        public LoadNewServerMessageCommand() { }
    }

    [MessageType(description: "从服务器获取到新的服务器消息后")]
    public class NewServerMessageLoadedEvent : EventBase {
        public NewServerMessageLoadedEvent(LinkedList<IServerMessage> data) {
            this.Data = data;
        }

        public LinkedList<IServerMessage> Data { get; }
    }

    [MessageType(description: "添加服务器消息")]
    public class AddServerMessageCommand : AddEntityCommand<IServerMessage> {
        public AddServerMessageCommand(IServerMessage input) : base(input) {
        }
    }

    [MessageType(description: "更新服务器消息")]
    public class UpdateServerMessageCommand : UpdateEntityCommand<IServerMessage> {
        public UpdateServerMessageCommand(IServerMessage input) : base(input) {
        }
    }

    [MessageType(description: "标记删除服务器消息")]
    public class MarkDeleteServerMessageCommand : RemoveEntityCommand {
        public MarkDeleteServerMessageCommand(Guid id) : base(id) {

        }
    }
    #endregion
}
