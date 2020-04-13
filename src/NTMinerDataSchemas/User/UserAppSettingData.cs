using System.Text;

namespace NTMiner.User {
    public class UserAppSettingData : IUserAppSetting, ISignableData {
        public UserAppSettingData() {
        }

        public static UserAppSettingData Create(IUserAppSetting appSetting) {
            if (appSetting == null) {
                return null;
            }
            if (appSetting is UserAppSettingData result) {
                return result;
            }
            return new UserAppSettingData {
                Key = appSetting.Key,
                Value = appSetting.Value
            };
        }

        public string Id { get; set; }

        public string LoginName { get; set; }

        public string Key { get; set; }

        public object Value { get; set; }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
