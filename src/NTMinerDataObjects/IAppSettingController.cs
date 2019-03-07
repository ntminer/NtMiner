using NTMiner.MinerServer;

namespace NTMiner {
    public interface IAppSettingController {
        GetAppSettingResponse AppSetting(AppSettingRequest request);
        GetAppSettingsResponse AppSettings(AppSettingsRequest request);
        ResponseBase SetAppSetting(SetAppSettingRequest request);
    }
}
