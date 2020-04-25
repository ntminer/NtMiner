using NTMiner.User;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public interface IUserAppSettingController {
        /// <summary>
        /// 需签名
        /// </summary>
        DataResponse<List<UserAppSettingData>> AppSettings(DataRequest<string> request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase SetAppSetting(DataRequest<UserAppSettingData> request);
    }
}
