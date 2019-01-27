using NTMiner.ServiceContracts.DataObjects;
using System;

namespace NTMiner.ServiceContracts {
    public interface IAppSettingService : IDisposable {
        GetAppSettingsResponse GetAllAppSettings(Guid messageId);
        GetAppSettingResponse GetAppSetting(Guid messageId, string key);
        GetAppSettingsResponse GetAppSettings(Guid messageId, string[] keys);
        ResponseBase SetAppSetting(SetAppSettingRequest request);
    }
}
