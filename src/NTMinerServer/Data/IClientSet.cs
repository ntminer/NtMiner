using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using LiteDB;

namespace NTMiner.Data {
    public interface IClientSet : IEnumerable<ClientData> {
        ClientCount Count();

        ClientCoinCount Count(string coinCode);

        void Add(ClientData clientData);

        void UpdateClient(ObjectId objectId, string propertyName, object value);

        void UpdateClientProperties(ObjectId objectId, Dictionary<string, object> values);

        void Remove(ObjectId objectId);

        List<ClientData> QueryClients(
            int pageIndex,
            int pageSize,
            Guid? groupId,
            Guid? workId,
            string minerIp,
            string minerName,
            MineStatus mineState,
            string mainCoin,
            string mainCoinPool,
            string mainCoinWallet,
            string dualCoin,
            string dualCoinPool,
            string dualCoinWallet,
            string version,
            string kernel,
            out int total,
            out int miningCount);

        ClientData GetByClientId(Guid clientId);
    }
}
