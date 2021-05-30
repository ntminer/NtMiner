using NTMiner.ServerNode;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IMqCountSet {
        MqCountData[] GetAll();
        MqCountData GetByAppId(string appId);
        List<string> GetAppIds();
        void Clear();
    }
}
