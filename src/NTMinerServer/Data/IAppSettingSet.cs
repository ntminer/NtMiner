using NTMiner.ServiceContracts.DataObjects;
using System.Collections.Generic;

namespace NTMiner.Data {
    public interface IAppSettingSet {
        AppSettingData GetAppSetting(string key);

        List<AppSettingData> GetAppSettings(string[] keys);

        List<AppSettingData> GetAllAppSettings();

        void SetAppSetting(AppSettingData appSettingData);
    }
}
