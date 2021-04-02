using NTMiner.ServerNode;

namespace NTMiner.Core {
    public interface IMqCountSet {
        MqCountData[] GetAll();
        MqCountData GetByAppId(string appId);
        void Clear();
    }
}
