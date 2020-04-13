using NTMiner.Core;
using NTMiner.Core.MinerServer;
using System;

namespace NTMiner.Controllers {
    public interface IAppSettingController {
        DateTime GetTime();
        string GetJsonFileVersion(AppSettingRequest request);
        ResponseBase SetAppSetting(DataRequest<AppSettingData> request);
    }
}
