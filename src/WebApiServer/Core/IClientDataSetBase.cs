using NTMiner.Core.MinerServer;
using NTMiner.User;
using System;
using System.Collections.Generic;

namespace NTMiner.Core {
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
