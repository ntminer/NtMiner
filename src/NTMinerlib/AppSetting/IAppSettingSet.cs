using System.Collections.Generic;

namespace NTMiner.AppSetting {
    public interface IAppSettingSet {
        bool TryGetAppSetting(string key, out IAppSetting appSetting);
        IEnumerable<IAppSetting> AsEnumerable();
    }
}
