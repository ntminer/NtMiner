using NTMiner.ServiceContracts.DataObjects;
using System;

namespace NTMiner.ServiceContracts {
    public interface IAppSettingService : IDisposable {
        GetAppSettingsResponse GetAppSettings();
        GetAppSettingResponse GetAppSetting(string key);
        GetAppSettingsResponse GetAppSettings(string[] keys);
        ResponseBase SetAppSetting(SetAppSettingRequest request);
    }
}
