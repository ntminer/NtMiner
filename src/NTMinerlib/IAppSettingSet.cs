using NTMiner.MinerServer;
using System.Collections.Generic;

namespace NTMiner {
    public interface IAppSettingSet {
        IAppSetting GetAppSetting(string key);

        List<IAppSetting> GetAppSettings();
    }
}
