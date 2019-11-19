using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Data {
    public interface IClientSet {
        void Add(ClientData clientData);

        void AddMiner(string minerIp);

        void UpdateClient(string objectId, string propertyName, object value);

        void UpdateClients(string propertyName, Dictionary<string, object> values);

        void Remove(string objectId);

        List<ClientData> RefreshClients(List<string> objectId);

        List<ClientData> QueryClients(
            int pageIndex,
            int pageSize,
            Guid? groupId,
            Guid? workId,
            string minerIp,
            string minerName,
            MineStatus mineState,
            string coin,
            string pool,
            string wallet,
            string version,
            string kernel,
            out int total,
            out int miningCount);

        ClientData GetByClientId(Guid clientId);

        ClientData GetByObjectId(string objectId);

        bool IsAnyClientInGroup(Guid groupId);

        bool IsAnyClientInWork(Guid workId);
        IEnumerable<ClientData> AsEnumerable();
    }
}
