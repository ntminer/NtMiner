using NTMiner.AppSetting;
using System.Runtime.Serialization;
using System.Text;

namespace NTMiner.ServiceContracts.DataObjects {
    [DataContract]
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

        [DataMember]
        [LiteDB.BsonId]
        public string Key { get; set; }

        [DataMember]
        public object Value { get; set; }

        public string GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(Key)).Append(Key)
                .Append(nameof(Value)).Append(Value);
            return sb.ToString();
        }
    }
}
