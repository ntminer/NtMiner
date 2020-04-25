using NTMiner.Core;
using NTMiner.Core.MinerServer;
using System;

namespace NTMiner.Controllers {
    public interface IAppSettingController {
        DateTime GetTime();
        string GetJsonFileVersion(AppSettingRequest request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase SetAppSetting(DataRequest<AppSettingData> request);
    }
}
