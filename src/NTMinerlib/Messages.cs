using NTMiner.Bus;
using System;
using System.Collections.Generic;

namespace NTMiner {

    #region abstract
    public abstract class DomainEvent<TEntity> : IEvent {
        protected DomainEvent(TEntity source) {
            this.Id = Guid.NewGuid();
            this.Source = source;
            this.Timestamp = DateTime.Now;
        }
        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; private set; }
        public DateTime Timestamp { get; private set; }
        public TEntity Source { get; private set; }
    }

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
    public class ChangeServerAppSettingCommand : Cmd {
        public ChangeServerAppSettingCommand(IAppSetting appSetting) {
            this.AppSetting = appSetting;
        }

        public IAppSetting AppSetting {
            get; private set;
        }
    }

    [MessageType(description: "设置ServerAppSetting")]
    public class ChangeServerAppSettingsCommand : Cmd {
        public ChangeServerAppSettingsCommand(IEnumerable<IAppSetting> appSettings) {
            this.AppSettings = appSettings;
        }

        public IEnumerable<IAppSetting> AppSettings {
            get; private set;
        }
    }

    [MessageType(description: "ServerAppSetting变更后")]
    public class ServerAppSettingChangedEvent : DomainEvent<IAppSetting> {
        public ServerAppSettingChangedEvent(IAppSetting source) : base(source) {
        }
    }

    [MessageType(description: "设置LocalAppSetting")]
    public class ChangeLocalAppSettingCommand : Cmd {
        public ChangeLocalAppSettingCommand(IAppSetting appSetting) {
            this.AppSetting = appSetting;
        }

        public IAppSetting AppSetting {
            get; private set;
        }
    }

    [MessageType(description: "设置LocalAppSetting")]
    public class ChangeLocalAppSettingsCommand : Cmd {
        public ChangeLocalAppSettingsCommand(IEnumerable<IAppSetting> appSettings) {
            this.AppSettings = appSettings;
        }

        public IEnumerable<IAppSetting> AppSettings {
            get; private set;
        }
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
}
