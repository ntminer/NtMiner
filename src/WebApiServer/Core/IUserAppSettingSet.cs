using NTMiner.User;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IUserAppSettingSet {
        List<UserAppSettingData> GetAppSettings(string loginName);
        void SetAppSetting(UserAppSettingData appSetting);
    }
}
