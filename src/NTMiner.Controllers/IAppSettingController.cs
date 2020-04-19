using NTMiner.Core;
using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public interface IAppSettingController {
        List<AppSettingData> AppSettings();
        DateTime GetTime();
        string GetJsonFileVersion(AppSettingRequest request);
        ResponseBase SetAppSetting(DataRequest<AppSettingData> request);
    }
}
