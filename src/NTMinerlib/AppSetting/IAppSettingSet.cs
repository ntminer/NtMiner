using NTMiner;
using System.Collections.Generic;

namespace NTMiner.AppSetting {
    public interface IAppSettingSet {
        IAppSetting GetAppSetting(string key);

        List<IAppSetting> GetAppSettings(string[] keys);
    }
}
