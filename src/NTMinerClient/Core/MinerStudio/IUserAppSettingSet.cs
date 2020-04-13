using NTMiner.User;
using System.Collections.Generic;

namespace NTMiner.Core.MinerStudio {
    public interface IUserAppSettingSet {
        bool TryGetAppSetting(string key, out IUserAppSetting appSetting);
        IEnumerable<IUserAppSetting> AsEnumerable();
    }
}
