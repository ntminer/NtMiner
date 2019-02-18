using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Data {
    public interface IClientSet {
        ClientCount Count();

        int CountMainCoinOnline(string coinCode);

        int CountDualCoinOnline(string coinCode);

        int CountMainCoinMining(string coinCode);

        int CountDualCoinMining(string coinCode);

        void Add(ClientData clientData);

        void UpdateClient(Guid clientId, string propertyName, object value);

        void UpdateClientProperties(Guid clientId, Dictionary<string, object> values);

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
            out int total);

        List<ClientData> LoadClients(IEnumerable<Guid> clientIds);

        ClientData LoadClient(Guid clientId);
    }
}
