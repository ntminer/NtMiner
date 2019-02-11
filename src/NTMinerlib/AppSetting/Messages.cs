using NTMiner.Bus;
using NTMiner.MinerServer;

namespace NTMiner.AppSetting {
    [MessageType(messageType: typeof(SetAppSettingCommand), description: "设置AppSetting")]
    public class SetAppSettingCommand : Cmd {
        public SetAppSettingCommand(IAppSetting appSetting) {
            this.AppSetting = appSetting;
        }

        public IAppSetting AppSetting {
            get; private set;
        }
    }

    [MessageType(messageType: typeof(AppSettingChangedEvent), description: "AppSetting变更后")]
    public class AppSettingChangedEvent : DomainEvent<IAppSetting> {
        public AppSettingChangedEvent(IAppSetting source) : base(source) {
        }
    }
}
