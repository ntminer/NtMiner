using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Data {
    public interface IClientSet : IEnumerable<ClientData> {
        ClientCount Count();

        ClientCoinCount Count(string coinCode);

        void Add(ClientData clientData);

        void UpdateClient(Guid clientId, string propertyName, object value);

        void UpdateClientProperties(Guid clientId, Dictionary<string, object> values);

        List<ClientData> QueryClients(
            int pageIndex,
            int pageSize,
            DateTime? timeLimit,
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

        ClientData LoadClient(Guid clientId, bool isPull);
    }
}
