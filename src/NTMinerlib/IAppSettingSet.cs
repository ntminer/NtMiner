using System.Collections.Generic;

namespace NTMiner {
    public interface IAppSettingSet : IEnumerable<IAppSetting> {
        IAppSetting this[string key] { get; }
    }
}
