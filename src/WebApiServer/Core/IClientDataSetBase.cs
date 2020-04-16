using NTMiner.Core.MinerServer;
using NTMiner.User;
using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public class ClientCount {
        public ClientCount() { }

        public void Update(int onlineCount, int miningCount) {
            this.OnlineCount = onlineCount;
            this.MiningCount = miningCount;
        }
        public int OnlineCount { get; private set; }
        public int MiningCount { get; private set; }
    }

    public interface IClientDataSetBase {
        ClientCount ClientCount { get; }
        void UpdateClient(string objectId, string propertyName, object value);
        void UpdateClients(string propertyName, Dictionary<string, object> values);
        void RemoveByObjectId(string objectId);
        List<ClientData> QueryClients(
            IUser user,
            QueryClientsRequest query,
            out int total,
            out List<CoinSnapshotData> coinSnapshots,
            out int onlineCount,
            out int miningCount);
        ClientData GetByClientId(Guid clientId);
        ClientData GetByObjectId(string objectId);
        bool IsAnyClientInGroup(Guid groupId);
        bool IsAnyClientInWork(Guid workId);
        IEnumerable<ClientData> AsEnumerable();
    }
}
