using System.Collections.Generic;
using NTMiner.MinerServer;

namespace NTMiner.Controllers {
    public interface IAppSettingController {
        DataResponse<AppSettingData> AppSetting(AppSettingRequest request);
        DataResponse<List<AppSettingData>> AppSettings(AppSettingsRequest request);
        ResponseBase SetAppSetting(DataRequest<AppSettingData> request);
    }
}
