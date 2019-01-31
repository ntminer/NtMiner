using NTMiner.Bus;

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
}
