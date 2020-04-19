using NTMiner.User;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public interface IUserAppSettingController {
        DataResponse<List<UserAppSettingData>> AppSettings(DataRequest<string> request);
        ResponseBase SetAppSetting(DataRequest<UserAppSettingData> request);
    }
}
