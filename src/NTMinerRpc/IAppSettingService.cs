using System;

namespace NTMiner {
    public interface IAppSettingService : IDisposable {
        GetAppSettingResponse GetAppSetting(Guid messageId, string key);

        GetAppSettingsResponse GetAppSettings(Guid messageId);

        ResponseBase SetAppSetting(SetAppSettingRequest request);
    }
}
