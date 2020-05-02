using NTMiner.User;
using System.Collections.Generic;

namespace NTMiner.Core.MinerStudio {
    public interface IUserAppSettingSet {
        void Init(List<UserAppSettingData> userAppSettings);
        bool TryGetAppSetting(string key, out IUserAppSetting appSetting);
        IEnumerable<IUserAppSetting> AsEnumerable();
    }
}
