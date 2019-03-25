using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public interface IAppSettingController {
        DateTime GetTime();
        DataResponse<AppSettingData> AppSetting(AppSettingRequest request);
        DataResponse<List<AppSettingData>> AppSettings(AppSettingsRequest request);
        ResponseBase SetAppSetting(DataRequest<AppSettingData> request);
        ResponseBase SetAppSettings(DataRequest<List<AppSettingData>> request);
    }
}
