using System.Collections.Generic;

namespace NTMiner.AppSetting {
    public interface IAppSettingSet : IEnumerable<IAppSetting> {
        bool TryGetAppSetting(string key, out IAppSetting appSetting);
    }
}
