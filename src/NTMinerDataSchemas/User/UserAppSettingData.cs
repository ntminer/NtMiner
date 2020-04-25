namespace NTMiner.User {
    [DataSchemaId("5C9D1CD3-5A56-4297-9C61-35FFE9A74F7D")]
    public class UserAppSettingData : IUserAppSetting {
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
    }
}
