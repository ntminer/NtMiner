namespace NTMiner.Core {
    [DataSchemaId("8E7F5E17-F3AF-47A2-98AA-18CF52C13021")]
    public class AppSettingData : IAppSetting {
        public AppSettingData() { }

        public static AppSettingData Create(IAppSetting appSetting) {
            if (appSetting == null) {
                return null;
            }
            if (appSetting is AppSettingData result) {
                return result;
            }
            return new AppSettingData {
                Key = appSetting.Key,
                Value = appSetting.Value
            };
        }

        [LiteDB.BsonId]
        public string Key { get; set; }

        public object Value { get; set; }
    }
}
