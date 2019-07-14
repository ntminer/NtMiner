using System.Text;

namespace NTMiner.MinerServer {
    public class AppSettingData : IAppSetting, IGetSignData {
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

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
