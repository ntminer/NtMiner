using System;

namespace NTMiner.ServiceContracts {
    public interface IAppSettingService : IDisposable {
        GetAppSettingResponse GetAppSetting(Guid messageId, string key);

        GetAppSettingsResponse GetAppSettings(Guid messageId);

        ResponseBase SetAppSetting(SetAppSettingRequest request);
    }
}
